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
-- Tests the frequency generator.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
 
entity PhaseGenerator_Tester is
end entity;
 
architecture stdarch of PhaseGenerator_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 10ns;
    constant phase_width: natural := 32;
    constant sample_width: natural := 32;
    constant phase_increment_1: integer := 16#19999999#; -- needs about 10 clock cycles
    constant clk_cycles_per_counter_cycle: integer := 10;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal phase_increment: unsigned(phase_width-1 downto 0) :=
        to_unsigned(phase_increment_1, phase_width);
    signal reset_phase: std_logic := '0';

    --------------------
    -- Outputs
    --------------------
    signal frequency_signal: std_logic;
    signal phase: unsigned (phase_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;
    signal phase_reset_test_has_started: boolean := false;

begin

    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- Just use the phase´s MSB as a frequency signal.
    frequency_signal <= phase(phase'high);


    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------
    
    uut: entity work.PhaseGenerator
    generic map
    (
        phase_width => phase_width
    )
    port map
    (
        clk => clk,
        phase_increment => phase_increment,
        reset_phase => reset_phase,
        phase => phase
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;

    -- Stimulates and controls the UUT and the tests at all.
    stimulus: process is
        constant signal_cycles_to_test: integer := 10;
    begin
    
        -- Do the tests for the specified duration.
        wait until rising_edge(frequency_signal);
        wait for 2*signal_cycles_to_test * (clk_cycles_per_counter_cycle/2) * clk_period;

        phase_reset_test_has_started <= true;
        wait for 10*clk_period;
        wait until falling_edge(clk);
        reset_phase <= '1';
        wait until falling_edge(clk);
        reset_phase <= '0';
        wait for 10*clk_period;

        -- Stop the tests.
        run_test <= false;
        wait;
        
    end process;


    --------------------------------------------------------------------------------
    -- Specifications.
    --------------------------------------------------------------------------------

    -- Verifies proper frequency signal generation.
    must_create_correct_frequency_signal: process is
        variable previous_signal: std_logic;
    begin
    
        -- The first signal edge comes rather randomly (depending on the phase
        -- increment). Thus synchronize to that signal.
        wait until rising_edge(frequency_signal);
 
        if not phase_reset_test_has_started then

            -- Verify the correct duration of high and the low phase (i.e. the right
            -- frequency and the 50% duty cycle). This test might not work exactly for
            -- increments that aren´t a power of two.
            wait until falling_edge(clk);
            previous_signal := frequency_signal;
            
            for clk_cycle in 1 to (clk_cycles_per_counter_cycle/2)-1 loop
                wait until falling_edge(clk);
                assert (frequency_signal = previous_signal)
                    report "Signal not changed or changed unexpectedly."
                    severity error;
            end loop;

        end if;
        
    end process;

    
    -- Verifies whether the phase is incremented correctly.
    must_increment_phase_per_clk_cycle: process is
        variable previous_phase: unsigned (phase_width-1 downto 0) := (others => '0');
    begin
    
        wait until falling_edge(clk);
            
        if not phase_reset_test_has_started then

            assert (phase = previous_phase + to_unsigned(phase_increment_1, phase_width))
                report "Phase not incremented as expected." & integer'image(to_integer(previous_phase))
                severity error;
            previous_phase := phase;

        end if;
        
    end process;


    -- Verifies correct phase reset.
    must_reset_phase_on_request: process is
    begin
        wait until rising_edge(clk);
        wait until falling_edge(clk);
        if reset_phase = '1' then
            assert (phase = (phase'range => '0'))
                report "Phase not reset as expected."
                severity error;
        end if;
    end process;

end architecture;
