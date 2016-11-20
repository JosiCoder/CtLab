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
 
entity SPI_SlaveReceiver_Tester is
end entity;
 
architecture stdarch of SPI_SlaveReceiver_Tester is
 
    -- Constants
    constant test_delay: time := 1ps;
    constant address_width: positive := 2;
    constant number_of_data_outputs: positive := 2**address_width;
    constant clk_period: time := 20ns; -- 50 MHz system clock
    constant sclk_period: time := 91ns; -- about 11 MHz serial clock
    
    -- Inputs
    signal clk: std_logic := '0';
    signal sclk: std_logic := '1';
    signal ss_address: std_logic :='1';
    signal ss_data: std_logic :='1';
    signal mosi: std_logic := '0';

    -- Outputs
    signal address: unsigned(address_width-1 downto 0) := (others => '0');
    signal data_x: data_buffer_vector(number_of_data_outputs-1 downto 0);
    signal ready_x: std_logic_vector(number_of_data_outputs-1 downto 0);
    
    -- Internals
    signal run_test: boolean := true;


    -------------------------------------------------------------------------
    -- Create a test value unique for the current address.
    -------------------------------------------------------------------------
    function get_test_value(address: natural) return data_buffer
    is
    begin
        return std_logic_vector(to_unsigned(16 + (address * 2), data_width));
    end function;
    
    
    -------------------------------------------------------------------------
    -- Passes a test value to the receiver and verifies the data appearing
    -- at the specified receiver buffer큦 parallel output.
    -------------------------------------------------------------------------
    procedure receive_data_and_check_behaviour(output_address: natural)
    is
        variable previous_data: data_buffer;
    begin
    
        previous_data := data_x(output_address);
        
        -- Send the output address.
        ss_address <= '0';
        serialize_byte(sclk_period, std_logic_vector(to_unsigned(output_address,8)), sclk, mosi);
        ss_address <= '1';

        -- Activate the buffer큦 slave select (SS) signal and check whether
        -- the output buffer큦 ready signal is deactivated at the next
        -- rising CLK edge.
        ss_data <= '0';
        wait until rising_edge(clk);
        wait for test_delay;
        assert (ready_x(output_address) = '0')
            report "READY activated unexpectedly." severity error;

        -- Serialize a test value to the receiver큦 serial input.
        serialize_longword(sclk_period, get_test_value(output_address), sclk, mosi);
        
        -- Check whether the buffer still holds the previous value.
        assert (data_x(output_address) = previous_data)
            report "Previous data not correctly preserved." severity error;

        -- Deactivate the buffer큦 slave select (SS) signal and and check
        -- whether the output buffer fetches the value at the next rising CLK
        -- edge.
        ss_data <= '1';
        wait until rising_edge(clk);
        wait for test_delay;

        -- Check whether the buffer holds the new value.
        assert (ready_x(output_address) = '1')
            report "READY not activated." severity error;
        assert (data_x(output_address) = get_test_value(output_address))
            report "Data not correctly received." severity error;
  
    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.SPI_SlaveReceiver
    generic map
    (
        address_width => address_width
    )
    port map
    (
		clk => clk, 
		buffer_enable => ss_data,
		sclk => sclk, 
		ss_address => ss_address, 
		address => address, 
		mosi => mosi, 
		data_x => data_x,
		ready_x => ready_x
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

        -- Receive several values via different outputs and check whether they
        -- arrive without changing unselected outputs.
    
        for i in 0 to number_of_data_outputs-1 loop
            wait for sclk_period; -- for a better readable timing diagram
            receive_data_and_check_behaviour(i);
        end loop;

        wait for sclk_period; -- for a better readable timing diagram

        -- Check whether all received data have been buffered and preserved correctly.
        for i in 0 to number_of_data_outputs-1 loop
            assert (data_x(i) = get_test_value(i))
                report "Data for address(" & integer'image(i) &
                    ") not correctly received and preserved."
                severity error;
        end loop;

        run_test <= false;
        wait;
        
    end process;
    
end architecture;
