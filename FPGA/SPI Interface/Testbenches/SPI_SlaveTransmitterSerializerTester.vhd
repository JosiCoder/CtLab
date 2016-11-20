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
-- Tests the SPI slave transmitter serializer.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.TestTools.all;
 
entity SPI_SlaveTransmitterSerializer_Tester is
end entity;
 
architecture stdarch of SPI_SlaveTransmitterSerializer_Tester is
 
    -- Constants
    constant test_delay: time := 1ps;
    constant data_width: positive := 32;
    constant sclk_period: time := 91ns; -- about 11 MHz serial clock
    constant test_value_0: std_logic_vector(data_width-1 downto 0) := x"12345678";
    constant test_value_1: std_logic_vector(data_width-1 downto 0) := x"FEDCBA98";
    
    -- Inputs
    signal sclk: std_logic := '1';
    signal ss: std_logic := '1';
    signal data: std_logic_vector(data_width-1 downto 0) := (others => '0');

    -- Outputs
    signal miso: std_logic;
    signal deserialized_data: std_logic_vector(data_width-1 downto 0) := (others => '0');

    -- Internals

    -------------------------------------------------------------------------
    -- Passes a test value through the transmitter and deserialize it again.
    -------------------------------------------------------------------------
    procedure deserialize_data(test_value: std_logic_vector(data_width-1 downto 0))
    is
    begin
    
        deserialized_data <= (deserialized_data'range => '0');

        ss <= '0';
        data <= test_value;
        wait for test_delay;
        deserialize_longword(sclk_period, miso, sclk, deserialized_data);
        ss <= '1';
        
    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.SPI_SlaveTransmitterSerializer
    generic map
    (
        width => data_width
    )
    port map
    (
		ss => ss, 
		sclk => sclk, 
		data => data,
		miso => miso 
    );
    

    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
    begin
    
        -- Pass several values through the transmitter and check whether they arrive
        -- at its output output.

        wait for sclk_period; -- for a better readable timing diagram

        deserialize_data(test_value_0);
        assert (deserialized_data = test_value_0) report "Data not correctly transmitted." severity error;

        wait for sclk_period; -- for a better readable timing diagram
        
        deserialize_data(test_value_1);
        assert (deserialized_data = test_value_1) report "Data not correctly transmitted." severity error;

        wait for sclk_period; -- for a better readable timing diagram

        wait;
        
    end process;

end architecture;
