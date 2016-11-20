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
-- Tests the pulse generator.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
 
entity PulseGenerator_Tester is
end entity;

architecture stdarch of PulseGenerator_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant counter_width: natural := 32;
    constant clk_period: time := 5ns;
    constant test_high_duration: unsigned(counter_width-1 downto 0) := to_unsigned(16, counter_width);
    constant test_low_duration: unsigned(counter_width-1 downto 0) := to_unsigned(8, counter_width);

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal high_duration: unsigned(counter_width-1 downto 0) := test_high_duration;
    signal low_duration: unsigned(counter_width-1 downto 0) := test_low_duration;

    --------------------
    -- Outputs
    --------------------
    signal pulse_signal: std_logic;

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;

begin

    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------
    uut: entity work.PulseGenerator
    generic map
    (
        counter_width => counter_width
    )
    port map
    (
        clk => clk,
        high_duration => high_duration,
        low_duration => low_duration,
        pulse_signal => pulse_signal
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;

    -- Stimulates and controls the UUT and the tests at all.
    stimulus: process is
    begin
    
        -- Do the tests for the specified duration.
        wait for 5 * (to_integer(test_high_duration) + to_integer(test_low_duration)) * clk_period;
        
        -- Stop the tests.
        run_test <= false;
        wait;
        
    end process;

    --------------------------------------------------------------------------------
    -- Specifications.
    --------------------------------------------------------------------------------

    -- Verifies proper frequency signal generation.
    must_create_correct_pulse_signal: process is
        variable startup: boolean := true;
    begin

        -- Wait for the pulse generator to settle down after powered up.
        if startup then
            wait until rising_edge(pulse_signal);
            wait until falling_edge(pulse_signal);
            startup := false;
        end if;

        -- Verify the correct duration of high and the low phase.
        for i in 1 to to_integer(test_low_duration) loop
            wait until falling_edge(clk);
            assert (pulse_signal = '0') report "Signal not set or held to '0'." severity error;
        end loop;
        for i in 1 to to_integer(test_high_duration) loop
            wait until falling_edge(clk);
            assert (pulse_signal = '1') report "Signal not set or held to '1'." severity error;
        end loop;

    end process;

end architecture;
