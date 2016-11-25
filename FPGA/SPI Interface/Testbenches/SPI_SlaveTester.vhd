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
-- Tests the SPI slave. The address space and thus the number of receiver and
-- transmitter data buffers can be changed. Also, the optional synchronizers can
-- be activated or deactivated. The additional delays caused by the synchronizer
-- (two rising CLK clock edges) are considered in this testbench.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.Globals.all;
use work.TestTools.all;
 
entity SPI_Slave_Tester is
end entity;
 
architecture stdarch of SPI_Slave_Tester is
 
    -- Constants
    constant test_delay: time := 1ps;
    constant address_width: positive := 2;
    constant number_of_data_buffers: positive := 2**address_width;
    constant synchronize_data_to_clk: boolean := true;
    constant clk_period: time := 20ns; -- 50 MHz system clock
    constant sclk_period: time := 91ns; -- about 11 MHz serial clock
    
    -- Inputs
    signal clk: std_logic := '0';
    signal sclk: std_logic := '1';
    signal ss_address: std_logic := '1';
    signal ss_data: std_logic := '1';
    signal transmit_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0) :=
           (others => (others => '0'));
    signal mosi: std_logic := '0';

    -- Outputs
    signal miso: std_logic;
    signal received_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal ready_x: std_logic_vector(number_of_data_buffers-1 downto 0);
    
    -- Internals
    signal deserialized_data: std_logic_vector(data_width-1 downto 0) := (others => '0');
    signal run_test: boolean := true;


    -------------------------------------------------------------------------
    -- Waits until the received signals appear at the output.
    -------------------------------------------------------------------------
    procedure wait_for_receiver_output_change
    is
    begin
        wait until rising_edge(clk);
        if synchronize_data_to_clk then
            wait until rising_edge(clk);
            wait until rising_edge(clk);
        end if;
    end procedure;
    
    
    -------------------------------------------------------------------------
    -- Create a test value unique for the current address and separately for
    -- transmission and reception.
    -------------------------------------------------------------------------
    function get_test_value(transmit: boolean; address: natural) return natural
    is
    begin
        if transmit then
            return 32 + (address * 2);
        else
            return 16 + (address * 2);
        end if;
    end function;
    
    
    -------------------------------------------------------------------------
    -- Sends an address to the receiver and checks the correct behaviour of
    -- some control signals.
    -------------------------------------------------------------------------
    procedure receive_address(address: natural)
    is
    begin

        -- Enable the address slave select and check whether all control signals
        -- react accordingly.
        ss_address <= '0';
        wait for test_delay;
        assert (ready_x = (ready_x'range => '1'))
            report "At least one READY_x unintentionally deactivated while transferring address."
            severity error;
        
        -- Serialize the current address to the receiver큦 serial input.
        wait for sclk_period;
        serialize_byte(sclk_period, std_logic_vector(to_unsigned(address,8)), sclk, mosi);
        wait for sclk_period;

        -- Disable the address slave select and wait for all signals to settle down.
        ss_address <= '1';
        wait for test_delay;
  
    end procedure;
    
    -------------------------------------------------------------------------
    -- Serializes a test value to the slave큦 serial receiver input, cycles 
    -- the slave while deserializing the slave큦 serial transmitter output.
    -- Validates this deserialized value.
    -- Also checks the correct behaviour of the slave큦 control signals.
    -------------------------------------------------------------------------
    procedure receive_and_transmit_values(receiver_send_value: data_buffer;
                                          transmitter_verify_value: data_buffer;
                                          current_address: natural)
    is
    begin

        deserialized_data <= (deserialized_data'range => '0');

        -- Enable the data slave select and check whether all control signals
        -- react accordingly.
        ss_data <= '0';
        wait_for_receiver_output_change;
        wait for test_delay;
        assert (ready_x = not std_logic_vector(shift_left(to_unsigned(1, number_of_data_buffers), current_address)))
            report "READY_x not deactivated correctly while transferring data."
            severity error;
        
        -- Serialize a test value to the receiver큦 serial input and simultaneously
        -- deserialize a test value from the transmitter큦 serial output.
        serialize_and_deserialize_longword(sclk_period,
                                           receiver_send_value, miso,
                                           sclk,
                                           mosi, deserialized_data);
    
        -- Disable the data slave select and check whether the transmission data has
        -- been correctyl serialized.
        ss_data <= '1';
        assert (deserialized_data = transmitter_verify_value)
            report "Data for address(" & integer'image(current_address) &
                ") not correctly transmitted."
            severity error;

        -- Check whether all control signals react accordingly.
        wait_for_receiver_output_change;
        wait for test_delay;
        assert (ready_x = (ready_x'range => '1'))
            report "At least one READY_x not activated after transferring data."
            severity error;
  
    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.SPI_Slave
    generic map
    (
        address_width => address_width,
        synchronize_data_to_clk => synchronize_data_to_clk
    )
    port map
    (
        clk => clk, 
        sclk => sclk, 
        ss_address => ss_address, 
        ss_data => ss_data,
        transmit_data_x => transmit_data_x,
        mosi => mosi,
        miso => miso,
        received_data_x => received_data_x,
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
        variable test_value: data_buffer;
    begin
    
        -- Wait for the UUT's initial output values to settle down and check them.
        wait_for_receiver_output_change;
        wait for test_delay;
        assert (ready_x = (ready_x'range => '1'))
            report "At least one READY_x unintentionally not active."
            severity error;

        -- Prepare the test values used for the transmission test.
        for current_address in 0 to number_of_data_buffers-1 loop
            transmit_data_x(current_address) <=
                std_logic_vector(to_unsigned(get_test_value(true, current_address), data_width));
        end loop;

        -- Transmit and receive unique test values from and to all addresses consecutively.
        for current_address in 0 to number_of_data_buffers-1 loop

            wait for sclk_period; -- for a better readable timing diagram

            -- Send the address to the receiver큦 serial input and check the correct
            -- behaviour of some control signals.            
            receive_address(current_address);

            wait for sclk_period; -- for a better readable timing diagram

            -- Pass a test value to the receiver and verify the test value fetched
            -- by the transmitter.
            receive_and_transmit_values(
                std_logic_vector(to_unsigned(get_test_value(false, current_address), data_width)),
                std_logic_vector(to_unsigned(get_test_value(true, current_address), data_width)),
                current_address);

        end loop;

        wait for sclk_period; -- for a better readable timing diagram

        -- Check whether all received data have been buffered and preserved correctly.
        for current_address in 0 to number_of_data_buffers-1 loop
            test_value := std_logic_vector(to_unsigned(get_test_value(false, current_address), data_width));
            assert (received_data_x(current_address) = test_value)
                report "Data for address(" & integer'image(current_address) &
                    ") not correctly received and preserved."
                severity error;
        end loop;

        run_test <= false;
        wait;
        
    end process;
    
end architecture;
