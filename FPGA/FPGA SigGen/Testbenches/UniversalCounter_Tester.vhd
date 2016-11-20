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
-- Tests the entire universal frequency or period counter.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.Globals.all;
 
entity UniversalCounter_Tester is
end entity;

architecture stdarch of UniversalCounter_Tester is
 
    --------------------
    -- Constants
    --------------------
    
    -- We use a counter width of 4 here to make tests easier.
    constant counter_width: natural := 4;

    -- Set the system clock period and a gate signal of clock/10, i.e. 100ns.
    constant clk_period: time := 10ns;
    constant gate_period: time := 10 * clk_period;
    

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal update_output: std_logic := '0';
    signal external_signal: std_logic := '0';


    --------------------
    -- Outputs
    --------------------
    signal value: unsigned(counter_width-1 downto 0);
    signal overflow: std_logic;
    signal ready: std_logic;


    --------------------
    -- Internals
    --------------------
    signal external_signal_period: time := 10ns;
    signal value_assumed_as_stable: boolean := false; -- for waveform diagram
    signal run_test: boolean := true;


    -------------------------------------------------------------------------
    -- Measures the external signal repeatedly and checks whether the counter
    -- works properly including overflow detection.
    -------------------------------------------------------------------------
    procedure verify_counter(signal_period: time;
                             expected_value_1: integer;
                             expected_value_2: integer;
                             some_must_overflow: boolean := false;
                             all_must_overflow: boolean := false)
    is
        variable overflow_cnt: integer;
    begin
  
        -- Change the external signal´s period and ensure that the counter
        -- value becomes stable and correct before verifying it.
        
        -- Set the new signal period and wait until it gets active.
        external_signal_period <= signal_period;
        wait until external_signal'event;
        
        -- Wait for the counter to settle down.
        value_assumed_as_stable <= false;
        wait for 2 * gate_period + 4 * signal_period;
        value_assumed_as_stable <= true;
        
        -- Read and verify the counter´s value several times.
        overflow_cnt := 0;
        for i in 0 to 9 loop

            -- Get and verify the counter value once per gate period.
            wait for gate_period;

            -- Verify the (propably overflown) counter value.
            assert (value = expected_value_1 or value = expected_value_2)
                report "Counter value incorrect, was " &
                    integer'image(to_integer(value)) & " instead of " &
                    integer'image(expected_value_1) & " or " &
                    integer'image(expected_value_2) & "."
                severity error;

            if (overflow = '1') then
                overflow_cnt := overflow_cnt + 1;
            end if;

            if (all_must_overflow) then
                assert (overflow = '1') report "Overflow not indicated." severity error;
            end if;

            if (not some_must_overflow and not all_must_overflow) then
                assert (overflow = '0') report "Overflow indicated unexpectedly." severity error;
            end if;

        end loop;
        
        -- Check the number of overflows happened (there should be some but not all for occasional
        -- overflows).
        if (some_must_overflow and not all_must_overflow) then
            assert (overflow_cnt > 0 and overflow_cnt < 10)
            report "Overflow for no or all measurements indicated unexpectedly." severity error;
        end if;

    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.UniversalCounter
    generic map
    (
        counter_width => counter_width,
        clock_divider_test_mode => true
    )
    port map
    (
        clk => clk,
        update_output => update_output,
        external_signal => external_signal,
        measure_period => '0',
        clk_division_mode => "0000", -- this is ignored in test mode
        value => value,
        overflow => overflow,
        ready => ready
    );


    --------------------------------------------------------------------------------
    -- Generate the system clock and other freely running periodic signals.
    --------------------------------------------------------------------------------
    clk <= not clk after clk_period/2 when run_test;
    external_signal <= not external_signal after external_signal_period/2 when run_test;


    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
        variable iteration: integer := 0;
    begin
    
        -- Activate the output.
        update_output <= '1';

        -- Test using some signals that fit exactly into a gate period.
        verify_counter(100ns, 1, 1, false, false);
        verify_counter(25ns, 4, 4, false, false);
        verify_counter(12.5ns, 8, 8, false, false);
        -- Test using some signals that don´t fit exactly into a gate period.
        verify_counter(30ns, 3, 4, false, false);
        -- Test using signals that cause all measurements to overflow.
        verify_counter(6.25ns, 0, 0, true, true); -- 16
        -- Test using signals that cause some measurements to overflow, but not all.
        verify_counter(6.5ns, 15, 0, true, false); -- 15..16

        -- Test the update output mechanism.
        update_output <= '1';
        verify_counter(100ns, 1, 1, false, false);
        update_output <= '0';
        verify_counter(25ns, 1, 1, false, false); -- Counter not affected by changed signal.
        update_output <= '1';
        verify_counter(25ns, 4, 4, false, false);
 
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
