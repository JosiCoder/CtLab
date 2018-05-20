--------------------------------------------------------------------------------
-- Copyright (C) 2016 Josi Coder

-- This program is free software: you can redistribute it and/or modify it
-- under the terms of the GNU General Public License as published by the Free
-- Software Foundation, either version 3 of the License, or (at your option)
-- any later version.
--
-- This program is distributed in the hope that it will be useful, but WITHOUT
-- ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
-- FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
-- more details.
--
-- You should have received a copy of the GNU General Public License along with
-- this program. If not, see <http://www.gnu.org/licenses/>.
----------------------------------------------------------------------------------

----------------------------------------------------------------------------------
-- Provides a test application that simply writes to and reads from the SRAM.
-- Data and control signals are passed via SPI registers as follows:
-- SPI write registers:
--   0: data to write to SRAM; 1: address; 2: mode (0=off, 1=read, 2=write).
-- SPI read registers:
--   0: data read from SRAM; 1-3: loopback from according SPI write registers.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library SPI_Interface;
use SPI_Interface.globals.all;

entity Main is
    generic
    (
        -- The width of the SRAMs address and data buses.
        ram_address_width: natural := 19;
        ram_data_width: natural := 8
    );
    port
    (
        -- The system clock.
        sysclk: in std_logic; 
        -- The internal SPI interface.
        f_sck: in std_logic; -- 
        f_rs: in std_logic; -- low during transmission
        f_ds: in std_logic; -- low during transmission
        f_mosi: in std_logic; 
        f_miso: out std_logic; 
        -- The external SPI interface.
        ext_sck: in std_logic; -- 
        ext_rs: in std_logic; -- low during transmission
        ext_ds: in std_logic; -- low during transmission
        ext_mosi: in std_logic; 
        ext_miso: out std_logic; 
        -- The test LED output.
        test_led: out std_logic;
        -- The SRAMs signals.
        ram_we_n: out std_logic;
        ram_oe_n: out std_logic;
        ram_address: out unsigned(ram_address_width-1 downto 0);
        ram_data: inout std_logic_vector(ram_data_width-1 downto 0);
        -- The DACs signals.
        dac_clk: out std_logic;
        dac_channel_select: out std_logic;
        dac_write_n: out std_logic
    );
end entity;

architecture stdarch of Main is

    -- Constants
    constant address_width: positive := 4; -- max. 8 (for addresses 0..255)
    constant number_of_data_buffers: positive := 2**address_width;
    constant use_internal_spi: boolean := true;
    constant use_external_spi: boolean := false;
    constant num_of_total_wait_states: natural := 9; -- 90ns @ 100MHz (min 70ns)
    constant num_of_write_pulse_wait_states: natural := 6; -- 60ns @ 100MHz (min 50ns)
    constant num_of_wait_states_before_write_after_read: natural := 4; -- 40ns @ 100MHz (min 30ns)

    -- Clocks
    signal clk_50mhz: std_logic;
    signal clk_100mhz: std_logic;

    -- SPI interfaces
    type spi_in_type is record
        mosi: std_logic;
        sclk: std_logic;
        ss_address: std_logic;
        ss_data: std_logic;
    end record;
    signal selected_spi_in, internal_spi_in, external_spi_in, inactive_spi_in: spi_in_type :=
    (
        -- Initialize to proper idle values.
        mosi => '0',
        sclk => '1',
        ss_address => '1',
        ss_data => '1'
    );
    signal miso: std_logic;

    -- Memory controller
    signal memory_clk: std_logic;
    signal memory_read: std_logic;
    signal memory_write: std_logic;
    signal memory_ready: std_logic;
    signal memory_address: unsigned(ram_address_width-1 downto 0);
    signal memory_data_in: std_logic_vector(ram_data_width-1 downto 0);
    signal memory_data_out: std_logic_vector(ram_data_width-1 downto 0);

    -- Internals
    signal transmit_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal received_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal ready_x: std_logic_vector(number_of_data_buffers-1 downto 0);
    
begin

    --------------------------------------------------------------------------------
    -- SPI input selection logic.
    --------------------------------------------------------------------------------

    -- The internal SPI bus (i.e. the one connected to the microcontroller of the
    -- c'Lab FPGA board).
    internal_spi_in.mosi <= f_mosi;
    internal_spi_in.sclk <= f_sck;
    internal_spi_in.ss_address <= f_rs;
    internal_spi_in.ss_data <= f_ds;

    -- The external SPI bus (i.e. the one connected to the expansion ports of the
    -- c'Lab FPGA board).
    external_spi_in.mosi <= ext_mosi;
    external_spi_in.sclk <= ext_sck;
    external_spi_in.ss_address <= ext_rs;
    external_spi_in.ss_data <= ext_ds;

    -- Select the SPI bus to use.
    -- Note: The microcontroller of the c'Lab FPGA board accesses the SPI bus periodically
    -- if one of the Param or Value screens is selected on the panel. Thus, when both
    -- connections are activated, while using the external connections, set the panel to
    -- the file selection screen.
    selected_spi_in <=
        internal_spi_in when use_internal_spi and
                             (internal_spi_in.ss_address = '0' or internal_spi_in.ss_data = '0') else
        external_spi_in when use_external_spi and
                             (external_spi_in.ss_address = '0' or external_spi_in.ss_data = '0') else
        inactive_spi_in;


    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- Values shown on panel.
    --------------------------------------------------------
    
    -- Data and control signals:
    --   channel 0: data read from SRAM
    --   channel 1: address loopback
    --   channel 2: mode (0: off, 1: read, 2: write) + memory state (MSB=0: working, MSB=1: ready)
    transmit_data_x(0) <= x"000000" & memory_data_out;
    transmit_data_x(1) <= received_data_x(1);
    transmit_data_x(2) <= memory_ready & received_data_x(2)(received_data_x(2)'high-1 downto 0);
    -- Loopback for all remaining channels.
    transmit_data_x(number_of_data_buffers-1 downto 3) <= received_data_x(number_of_data_buffers-1 downto 3);

    -- Connections to SRAM controller.
    --------------------------------------------------------

    memory_clk <= clk_100mhz;
    -- Data and control signals:
    --   channel 0: data to write to SRAM
    --   channel 1: address
    --   channel 2: mode (0: off, 1: read, 2: write)
    memory_read <= received_data_x(2)(0);
    memory_write <= received_data_x(2)(1);
    memory_address <= unsigned(received_data_x(1)(ram_address_width-1 downto 0));
    memory_data_in <= received_data_x(0)(ram_data_width-1 downto 0);


    --------------------------------------------------------------------------------
    -- Component instantiation.
    --------------------------------------------------------------------------------

    -- The clock manager generating the clocks used throughout the system.
    clock_manager: entity work.ClockManager
    port map
    (
        clk => sysclk,
        clk_50mhz => clk_50mhz,
        clk_100mhz => clk_100mhz
    );


    -- The SPI slave.
    slave: entity SPI_Interface.SPI_Slave
    generic map
    (
        address_width => address_width,
        synchronize_data_to_clk => true
    )
    port map
    (
        clk => clk_50mhz,
        sclk => selected_spi_in.sclk, 
        ss_address => selected_spi_in.ss_address, 
        ss_data => selected_spi_in.ss_data,
        transmit_data_x => transmit_data_x,
        mosi => selected_spi_in.mosi,
        miso => miso,
        received_data_x => received_data_x,
        ready_x => ready_x
    );


    -- The SRAM controller.
    sram: entity work.SRAM_Controller
    generic map
    (
        num_of_total_wait_states => num_of_total_wait_states,
        num_of_write_pulse_wait_states => num_of_write_pulse_wait_states,
        num_of_wait_states_before_write_after_read => num_of_wait_states_before_write_after_read,
        data_width => ram_data_width,
        address_width => ram_address_width
    )
    port map
    (
        clk => memory_clk,
        read => memory_read,
        write => memory_write,
        ready => memory_ready,
        address => memory_address,
        data_in => memory_data_in,
        data_out => memory_data_out,
        ram_we_n => ram_we_n,
        ram_oe_n => ram_oe_n,
        ram_address => ram_address,
        ram_data => ram_data
    );
 

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------

    -- SPI & test LED.
    f_miso <= miso when f_ds = '0' else 'Z';
    ext_miso <= miso when ext_ds = '0' else 'Z';
    test_led <= not memory_ready; -- LED is active low

    -- Single and dual DAC.
    dac_clk <= '1';
    dac_channel_select <= '1';
    dac_write_n <= '1';

end architecture;
