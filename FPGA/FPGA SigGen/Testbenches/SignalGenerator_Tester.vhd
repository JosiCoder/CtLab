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
-- Tests the signal generator.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.FunctionGenerator_Declarations.all;
 
entity SignalGenerator_Tester is
end entity;
 
architecture stdarch of SignalGenerator_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 10ns;
    constant phase_width: natural := 8;
    constant lookup_table_phase_width: natural := 8;
    constant level_width: natural := 4;
    constant sample_width: natural := 4;
    constant phase_increment_1: integer := 2**(phase_width-4); -- 8 clk cycles per sample cycle
    constant phase_shift_1: integer := 0; -- no phase shift
    constant phase_shift_2: integer := 2**(phase_width-1); -- phase shift of +Pi.
    constant phase_shift_3: integer := 2**(phase_width-2); -- phase shift of +Pi/2.
    constant level_1: integer := 2**(level_width-1)-1; -- positive maximum
    constant no_of_sample_periods: integer := 3;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal config: generator_config :=
    (
        waveform => waveform_square
    );
    signal phase_increment: unsigned(phase_width-1 downto 0) :=
        to_unsigned(phase_increment_1, phase_width);
    signal phase_shift: unsigned(phase_width-1 downto 0) :=
        to_unsigned(phase_shift_1, phase_width);
    signal level: signed (level_width-1 downto 0) :=
        to_signed(level_1, level_width);

    --------------------
    -- Outputs
    --------------------
    signal phase: unsigned (phase_width-1 downto 0);
    signal sample: signed(sample_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;
    signal phase_switch_in_progress: boolean := false;

begin

    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------
    
    uut: entity work.SignalGenerator
    generic map
    (
        phase_width => phase_width,
        lookup_table_phase_width => lookup_table_phase_width,
        level_width => level_width,
        sample_width => sample_width
    )
    port map
    (
        clk => clk,
        config => config,
        phase_increment => phase_increment,
        phase_shift => phase_shift,
        level => level,
        reset_phase => '0',
        phase => phase,
        sample => sample
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;

    -- Stimulates and controls the UUT and the tests at all.
    stimulus: process is
        constant configurations: generator_config_vector :=
        (
            (waveform => waveform_square),
            (waveform => waveform_sawtooth),
            (waveform => waveform_sine)
        );
    begin
    
        -- For each configuration, do the tests for the specified duration.
        for i in configurations'range loop
            config <= configurations(i);

            -- Phase shift of 0.
            phase_switch_in_progress <= true;
            phase_shift <= to_unsigned(phase_shift_1, phase_width);
            wait until falling_edge(phase(phase'high));
            phase_switch_in_progress <= false;
            for sample_cycle in 0 to no_of_sample_periods loop
                wait until falling_edge(phase(phase'high));
            end loop;

            -- Phase shift of Pi.
            phase_switch_in_progress <= true;
            phase_shift <= to_unsigned(phase_shift_2, phase_width);
            wait until falling_edge(phase(phase'high));
            phase_switch_in_progress <= false;
            for sample_cycle in 0 to no_of_sample_periods loop
                wait until falling_edge(phase(phase'high));
            end loop;
        end loop;

        -- Stop the tests.
        run_test <= false;
        wait;
        
    end process;


    --------------------------------------------------------------------------------
    -- Specifications.
    --------------------------------------------------------------------------------

    -- Verifies whether the output signals are in sync, i.e. whether their phases
    -- are identical.
    signals_must_be_in_sync: process is
    begin
    
        wait until falling_edge(clk);
        
        -- This assertion only works for signals that are positive during the first
        -- half of the period and negative during the second half (e.g. square,
        -- sawtooth, sine).
        if not phase_switch_in_progress then
            -- Check correct phase for phase shift of 0.
            if (phase_shift = to_unsigned(phase_shift_1, phase_width)) then
                assert (phase(phase'high) = '0' and to_integer(sample) >= 0) or
                       (phase(phase'high) = '1' and to_integer(sample) <= 0)
                    report "Phase and sample signals are not in sync for waveform=" & integer'image(to_integer(unsigned(config.waveform)))
                    & " at 0° phase shift" severity error;
            end if;
            -- Check correct phase for phase shift of Pi.
            if (phase_shift = to_unsigned(phase_shift_2, phase_width)) then
                assert (phase(phase'high) = '1' and to_integer(sample) >= 0) or
                       (phase(phase'high) = '0' and to_integer(sample) <= 0)
                    report "Phase and sample signals are not in sync for waveform=" & integer'image(to_integer(unsigned(config.waveform)))
                    & " at 180° phase shift" severity error;
            end if;
        end if;
            
    end process;

end architecture;
