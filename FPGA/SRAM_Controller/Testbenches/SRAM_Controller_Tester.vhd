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
-- Tests the SRAM controller.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
 
entity SRAM_Controller_Tester is
end entity;

architecture stdarch of SRAM_Controller_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 10ns; -- 100 MHz
    constant wait_states: natural := 10;
    constant num_of_wait_states: natural := 10;
    constant wait_states_counter_width: natural := 4;
    constant data_width: natural := 8;
    constant address_width: natural := 16;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal read: std_logic;
    signal write: std_logic;
    signal address: unsigned(address_width-1 downto 0);
    signal data_in: unsigned(data_width-1 downto 0);

    --------------------
    -- Outputs
    --------------------
    signal ready: std_logic;
    signal data_out: std_logic_vector(data_width-1 downto 0);
    signal ram_we_n: std_logic;
    signal ram_oe_n: std_logic;
    signal ram_address: unsigned(address_width-1 downto 0);

    --------------------
    -- Bidirectional
    --------------------
    signal ram_data: std_logic_vector(data_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;

begin

    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------
    uut: entity work.SRAM_Controller
    generic map
    (
        num_of_wait_states => num_of_wait_states,
        wait_states_counter_width => wait_states_counter_width,
        data_width => data_width,
        address_width => address_width
    )
    port map
    (
        clk => clk,
        read => read,
        write => write,
        ready => ready,
        address => address,
        data_in => data_in,
        data_out => data_out,
        ram_we_n => ram_we_n,
        ram_oe_n => ram_oe_n,
        ram_address => ram_address,
        ram_data => ram_data
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;

    -- Stimulates and controls the UUT and the tests at all.
    stimulus: process is
    begin
    
        wait for clk_period; -- for a better readable timing diagram

        write <= '0';
        read <= '1';

        -- Access the SRAM several times.
        for i in 100 to 104 loop
            address <= to_unsigned(i, address_width);
            wait for wait_states * clk_period;
        end loop;
    
        -- Stop the tests.
        run_test <= false;
        wait;
        
    end process;

    -- Simulates the external SRAM (worst conditions).
    sram: process is
    begin

        for i in 0 to 999 loop
            wait on ram_address;
            wait for 70ns;
            ram_data <= std_logic_vector(to_unsigned(i, data_width));
        end loop;
    
    end process;

    --------------------------------------------------------------------------------
    -- Specifications.
    --------------------------------------------------------------------------------
--
--    -- Verifies proper frequency signal generation.
--    must_create_correct_pulse_signal: process is
--        variable startup: boolean := true;
--    begin
--
--        -- Wait for the pulse generator to settle down after powered up.
--        if startup then
--            wait until rising_edge(pulse_signal);
--            wait until falling_edge(pulse_signal);
--            startup := false;
--        end if;
--
--        -- Verify the correct duration of high and the low phase.
--        for i in 1 to to_integer(test_low_duration) loop
--            wait until falling_edge(clk);
--            assert (pulse_signal = '0') report "Signal not set or held to '0'." severity error;
--        end loop;
--        for i in 1 to to_integer(test_high_duration) loop
--            wait until falling_edge(clk);
--            assert (pulse_signal = '1') report "Signal not set or held to '1'." severity error;
--        end loop;
--
--    end process;

end architecture;
