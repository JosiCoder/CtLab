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
-- Tests the gated counter.
-- To allow testing the overflow circuits, the UUT is configured to a very
-- small counter width.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.Globals.all;
 
entity GatedCounter_Tester is
end entity;

architecture stdarch of GatedCounter_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant counter_width: natural := 4;
    constant pulse_period: time := 5ns;

    --------------------
    -- Inputs
    --------------------
    signal pulse_signal: std_logic := '0';
    signal gate_signal: std_logic := '0';

    --------------------
    -- Outputs
    --------------------
    signal counter_value: unsigned(counter_width-1 downto 0);
    signal overflow: std_logic;

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;


    ------------------------------------------------------------------------
    -- Prepares the counter to count the first value.
    ------------------------------------------------------------------------
    procedure init_counter is
    begin
        wait until falling_edge(pulse_signal);
        gate_signal <= '1';
        wait until falling_edge(pulse_signal);
        gate_signal <= '0';
    end procedure;


    ------------------------------------------------------------------------
    -- Makes the counter count the specified number of pulse cycles.
    ------------------------------------------------------------------------
    procedure count_pulse_cycles(no_of_cycles: integer) is
    begin
        wait for (no_of_cycles-1) * pulse_period;
        gate_signal <= '1';
        wait until falling_edge(pulse_signal);
        gate_signal <= '0';
    end procedure;
    

    --------------------------------------------------------------------------
    -- Counts different numbers of pulse cycles and checks whether the counter
    -- works properly including overflow detection.
    --------------------------------------------------------------------------
    procedure verify_counter(no_of_cycles: integer; must_overflow: boolean) is
        variable expected_value: integer;
    begin
    
        count_pulse_cycles(no_of_cycles);
        
        expected_value := no_of_cycles mod 2**counter_width;
        assert (counter_value = expected_value)
            report "Counter value incorrect, was " &
                integer'image(to_integer(counter_value)) & " instead of " &
                integer'image(expected_value) & "."
            severity error;
            
        if (must_overflow) then
            assert (overflow = '1') report "Overflow not indicated." severity error;
        else
            assert (overflow = '0') report "Overflow indicated unexpectedly." severity error;
        end if;
        
    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.GatedCounter
    generic map
    (
        counter_width => counter_width
    )
    port map
    (
        pulse_signal => pulse_signal,
        gate_signal => gate_signal,
        value => counter_value,
        overflow => overflow
    );


    --------------------------------------------------------------------------------
    -- Generate the counter clock.
    --------------------------------------------------------------------------------
    pulse_signal <= not pulse_signal after pulse_period/2 when run_test;


    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
        variable iteration: integer := 0;
    begin
    
        -- Initialize the counter.
        init_counter;

        -- Count different numbers of pulse cycles and check whether the counter works properly
        -- including overflow detection.
        verify_counter(10, false);
        verify_counter(1, false);
        verify_counter(15, false);
        verify_counter(16, true); -- will overflow once
        verify_counter(3, false);
        verify_counter(40, true); -- will overflow several times

        -- Repeat the test once to verify proper continuous operation, then
        -- terminate the test.
        if (iteration = 0) then
            iteration := iteration + 1;
        else
            run_test <= false;
            wait;
        end if;
        
    end process;
    
end architecture;
