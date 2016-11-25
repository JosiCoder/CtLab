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
-- Tests the SPI slave receiver.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.Globals.all;
use work.TestTools.all;
 
entity SPI_SlaveTransmitter_Tester is
end entity;
 
architecture stdarch of SPI_SlaveTransmitter_Tester is
 
    -- Constants
    constant test_delay: time := 1ps;
    constant address_width: positive := 2;
    constant number_of_data_inputs: positive := 2**address_width;
    constant clk_period: time := 20ns; -- 50 MHz system clock
    constant sclk_period: time := 91ns; -- about 11 MHz serial clock
    
    -- Inputs
    signal clk: std_logic := '0';
    signal sclk: std_logic := '1';
    signal ss: std_logic :='1';
    signal address: unsigned(address_width-1 downto 0) := (others => '0');
    signal data_x: data_buffer_vector(number_of_data_inputs-1 downto 0) :=
        (others => (others => '0'));

    -- Outputs
    signal miso: std_logic;
    
    -- Internals
    signal deserialized_data: data_buffer := (others => '0');
    signal run_test: boolean := true;


    -------------------------------------------------------------------------
    -- Passes a test value to the transmitter and verifies the data appearing
    -- at the transmitter´s serial output.
    -------------------------------------------------------------------------
    procedure transmit_data_and_check_behaviour(current_address: natural)
    is
    begin
    
        deserialized_data <= (deserialized_data'range => '0');

        -- Set the address and wait for the input data being buffered at the
        -- next rising CLK edge.
        address <= to_unsigned(current_address, address_width);
        wait until rising_edge(clk);

        -- Activate the slave select (SS) signal, transmit the test value,
        -- and deactivate SS.
        ss <= '0';
        wait for test_delay;
        deserialize_longword(sclk_period, miso, sclk, deserialized_data);
        ss <= '1';

        -- Check whether the test value has been received.
        assert (deserialized_data = data_x(current_address))
            report "Data not correctly transmitted." severity error;

    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.SPI_SlaveTransmitter
    generic map
    (
        address_width => address_width
    )
    port map
    (
        clk => clk, 
        buffer_enable => ss,
        sclk => sclk, 
        ss => ss,
        address => address,
        data_x => data_x,
        miso => miso
    );
    

    --------------------------------------------------------------------------------
    -- Generate the system clock.
    --------------------------------------------------------------------------------
    clk <= not clk after clk_period/2 when run_test;
    

    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
    begin

        -- Create test value unique for each input.
        for i in 0 to number_of_data_inputs-1 loop
            data_x(i) <= std_logic_vector(to_unsigned(16 + (i * 2), data_width));
        end loop;
        wait for test_delay;
        
        -- Transmit several values and check whether they arrive.
        for i in 0 to number_of_data_inputs-1 loop
            wait for sclk_period; -- for a better readable timing diagram
            transmit_data_and_check_behaviour(i);
        end loop;

        wait for sclk_period; -- for a better readable timing diagram

        run_test <= false;
        wait;
        
    end process;
    
end architecture;
