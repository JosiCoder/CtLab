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
-- Provides a loopback test application that simply connects SPI receivers to 
-- transmitters. Thus data received on a specific address are retransmitted on
-- the same address.
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
    constant num_of_sram_wait_states: natural := 10;
    constant sram_wait_states_counter_width: natural := 4;

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

--TODO
    -- Connections to and from internal signals.
--    transmit_data_x <= received_data_x; -- test loop back
    transmit_data_x(number_of_data_buffers-1 downto 1) <= received_data_x(number_of_data_buffers-1 downto 1);
--    transmit_data_x(0) <= received_data_x(0);
    transmit_data_x(0) <= x"000000" & ram_data;

    -- The SPI slave.
    slave: entity SPI_Interface.SPI_Slave
    generic map
    (
        address_width => address_width,
        synchronize_data_to_clk => true
    )
    port map
    (
        clk => sysclk,
        sclk => selected_spi_in.sclk, 
        ss_address => selected_spi_in.ss_address, 
        ss_data => selected_spi_in.ss_data,
        transmit_data_x => transmit_data_x,
        mosi => selected_spi_in.mosi,
        miso => miso,
        received_data_x => received_data_x,
        ready_x => ready_x
    );


    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------

    -- SPI & test LED.
    f_miso <= miso when f_ds = '0' else 'Z';
    ext_miso <= miso when ext_ds = '0' else 'Z';
    test_led <= received_data_x(0)(0);

    -- SRAM.
    ram_we_n <= '1';
--    ram_oe_n <= '1';
    ram_oe_n <= received_data_x(0)(0);
--    ram_address <= (others => '0');
    ram_address <= unsigned(received_data_x(0)(ram_address_width-1 downto 0));
    ram_data <= (others => 'Z');

    -- Single and dual DAC.
    dac_clk <= '1';
    dac_channel_select <= '1';
    dac_write_n <= '1';

end architecture;
