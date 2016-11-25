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
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The address slave select (low during transmission).
        ssreg: in std_logic;
        -- The data slave select (low during transmission).
        ssdat: in std_logic;
        -- The serial input.
        mosi: in std_logic; 
        -- The serial output.
        miso: out std_logic; 
        -- The test LED output.
        test_led: out std_logic
    );
end entity;

architecture stdarch of Main is

    -- Constants
    constant address_width: positive := 4; -- max. 8 (for addresses 0..255)
    constant number_of_data_buffers: positive := 2**address_width;

    -- Internals
    signal transmit_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal received_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal ready_x: std_logic_vector(number_of_data_buffers-1 downto 0);
    signal miso_int: std_logic;

begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    -- Connections to and from internal signals.
    test_led <= received_data_x(0)(0);
    transmit_data_x <= received_data_x; -- test loop back

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
        sclk => sclk, 
        ss_address => ssreg, 
        ss_data => ssdat,
        transmit_data_x => transmit_data_x,
        mosi => mosi,
        miso => miso_int,
        received_data_x => received_data_x,
        ready_x => ready_x
    );


    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    miso <= miso_int when ssdat = '0' else 'Z';

end architecture;
