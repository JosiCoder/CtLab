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

--------------------------------------------------------------------------------
-- Tests the SPI slave receiver deserializer.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.TestTools.all;
 
entity SPI_SlaveReceiverDeserializer_Tester is
end entity;
 
architecture stdarch of SPI_SlaveReceiverDeserializer_Tester is
 
    -- Constants
    constant test_delay: time := 1ps;
    constant data_width: positive := 32;
    constant sclk_period: time := 91ns; -- about 11 MHz serial clock
    constant test_value_0: std_logic_vector(data_width-1 downto 0) := x"12345678";
    constant test_value_1: std_logic_vector(data_width-1 downto 0) := x"FEDCBA98";
    
    -- Inputs
    signal sclk: std_logic := '1';
    signal mosi: std_logic := '0';

    -- Outputs
    signal data: std_logic_vector(data_width-1 downto 0);

begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.SPI_SlaveReceiverDeserializer
    generic map
    (
        width => data_width
    )
    port map
    (
        sclk => sclk, 
        mosi => mosi, 
        data => data
    );
    

    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
    begin
    
        -- Pass several values through the receiver and check whether they arrive
        -- at its output output.

        wait for sclk_period; -- for a better readable timing diagram
        
        serialize_longword(sclk_period, test_value_0, sclk, mosi);
        assert (data = test_value_0) report "Data not correctly received." severity error;

        wait for sclk_period; -- for a better readable timing diagram
        
        serialize_longword(sclk_period, test_value_1, sclk, mosi);
        assert (data = test_value_1) report "Data not correctly received." severity error;
        
        wait for sclk_period; -- for a better readable timing diagram

        wait;
        
    end process;

end architecture;
