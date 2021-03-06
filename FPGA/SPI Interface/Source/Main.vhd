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
use work.globals.all;

entity Main is
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
        test_led: out std_logic
    );
end entity;

architecture stdarch of Main is

    -- Constants
    constant address_width: positive := 4; -- max. 8 (for addresses 0..255)
    constant number_of_data_buffers: positive := 2**address_width;
    constant use_internal_spi: boolean := true;
    constant use_external_spi: boolean := false;

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
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- NOTE: Reading to and writing from an SPI address always happen together. Each time
    -- the SPI master reads a value from the slave's transmit register, it also writes a value
    -- to the slave's receive register of the same address, overwriting any previous value.
    --
    -- If the internal SPI connection is used, the microcontroller of the c'Lab FPGA board
    -- acts as the SPI master. It accesses a particular SPI adress as follows:
    -- 1) If one of the Param or Value screens is selected on the panel, the microcontroller
    --    accesses the SPI bus periodically to read the value from and write the parameter to
    --    the according SPI address.
    -- 2) When processing a c't Lab protocol set command, the microcontroller writes the 
    --    according parameter to the SPI slave and ignores the value read from the SPI slave.
    -- 3) When processing a c't Lab protocol query command, the microcontroller writes an
    --    arbitrary parameter to the SPI slave and returns the value read from the SPI slave.
    --    It happens to be that the parameter sent most recently to the same or any other SPI
    --    address is reused as this arbitrary parameter.
    --
    -- If the external SPI connection is used, it's up to the external SPI master how to handle
    -- values read from the SPI slave and how to generate parameters written to the SPI slave.


    -- Test loop back
    transmit_data_x <= received_data_x;


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

    -- Select the SPI connection to use.
    -- NOTE: If one of the Param or Value screens is selected on the panel, the microcontroller
    -- of the c'Lab FPGA board accesses the SPI bus periodically to read the value from and write
    -- the parameter to the according SPI address (SPI reading and writing always happen together).
    -- Thus, when both connections are activated, while using the *external* connection, set the
    -- panel to the file selection screen to avoid this interference.
    -- Also, when both connections are activated, while using the *internal* connection, ensure
    -- that the selection pins of the external connection (ext_rs and ext_ds) are pulled up properly.
    -- If they are e.g. connected to the SPI interface of a Raspberry Pi, ensure that the latter is
    -- switched on. Don't leave the pins unconnected, pull them up instead.
    selected_spi_in <=
        internal_spi_in when use_internal_spi and
                             (internal_spi_in.ss_address = '0' or internal_spi_in.ss_data = '0') else
        external_spi_in when use_external_spi and
                             (external_spi_in.ss_address = '0' or external_spi_in.ss_data = '0') else
        inactive_spi_in;


    --------------------------------------------------------------------------------
    -- Component instantiation.
    --------------------------------------------------------------------------------

    -- The SPI slave.
    slave: entity work.SPI_Slave
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
    test_led <= received_data_x(0)(0); -- LED is active low

end architecture;
