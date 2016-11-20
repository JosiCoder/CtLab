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
-- Tests the function generator.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;
use work.FunctionGenerator_Declarations.all;
 
entity FunctionGenerator_Tester is
end entity;

architecture stdarch of FunctionGenerator_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 10ns;
    constant phase_width: natural := 32;
    constant sample_width: natural := 16;
    constant lookup_table_phase_width: natural := 12;
    constant phase_increment: integer := 16#1000000#;
    constant no_of_sample_periods: integer := 3;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal config: generator_config :=
    (
        waveform => waveform_square
    );
    signal phase: unsigned (phase_width-1 downto 0) := (others => '0');
    
    --------------------
    -- Outputs
    --------------------
    signal sample: signed(sample_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;
    
begin

    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------

    uut: entity work.FunctionGenerator
    generic map
    (
        phase_width => phase_width,
        lookup_table_phase_width => lookup_table_phase_width,
        sample_width => sample_width
    )
    port map
    (
        clk => clk,
        config => config,
        phase => phase,
        sample => sample
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;


    -- Generates a periodically incremented phase.
    increment_phase: process is
    begin
        -- Do the tests for the specified duration.
        wait until rising_edge(clk);
        phase <= phase + to_unsigned(phase_increment, phase_width);
    end process;


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
            for sample_cycle in 0 to no_of_sample_periods loop
                wait until falling_edge(phase(phase'high));
            end loop;
        end loop;

        run_test <= false;
        wait;
        
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Specifications.
    --------------------------------------------------------------------------------

    -- Verifies proper waveworm generation.
    must_create_waveform_from_phase: process is
        constant positive_sample_maximum: signed(sample_width-1 downto 0) :=
            to_signed(2**(sample_width-1)-1, sample_width);
        constant negative_sample_maximum: signed(sample_width-1 downto 0) :=
            to_signed(-(2**(sample_width-1)), sample_width);
        variable previous_phase: unsigned(phase_width-1 downto 0) := (others => '0');
        variable previous_values_set: boolean := false;
        variable previous_config: generator_config :=
            (
                waveform => "111"
            );
    begin
    
        wait until falling_edge(clk);
        
        -- Skip the first clock cycle after each configuration change to let
        -- previous_phase settle down.
        if (config.waveform /= previous_config.waveform) then
            previous_values_set := false;
            previous_config.waveform := config.waveform;
        end if;
        
        -- The waveform is delayed by one clock cycle, thus check the sample
        -- values against the previous cycle´s phase if they´re valid.
        if (previous_values_set) then
        
            -- Verify the waveform depending on the currently selected configuration.
            case config.waveform is
                when waveform_square =>
                    -- The sample must be the positive or negative maximum corresponding
                    -- to the phase's sign.
                    assert (previous_phase(previous_phase'high) = '0' and sample = positive_sample_maximum) or
                           (previous_phase(previous_phase'high) = '1' and sample = negative_sample_maximum)
                        report "Square sample not set correctly." severity error;
                when waveform_sawtooth =>
                    -- The sample must be 0 at a phase of 0°, otherwise match the phase's sign.
                    assert (
                               previous_phase = (previous_phase'range => '0')
                               and
                               sample = (sample'range => '0')
                           )
                           or
                           (
                               previous_phase /= (previous_phase'range => '0')
                               and
                               previous_phase(previous_phase'high) = sample(sample'high)
                           )
                        report "Sawtooth sample has wrong zero or sign." severity error;
                when waveform_sine =>
                    -- The sample must be 0 at a phase of 0° or 180°, otherwise match the phase's sign.
                    assert (
                               previous_phase(previous_phase'left-1 downto 0) = (previous_phase'left-1 downto 0 => '0')
                               and
                               sample = (sample'range => '0')
                           )
                           or
                           (
                               previous_phase(previous_phase'left-1 downto 0) /= (previous_phase'left-1 downto 0 => '0')
                               and
                               previous_phase(previous_phase'high) = sample(sample'high)
                           )
                        report "Sine sample has wrong zero or sign." severity error;
                when others =>
                    report "Unknown waveform set." severity error;
            end case;
            
        end if;

        -- Memorize the current phase for the next cycle.
        previous_phase := phase;
        previous_values_set := true;
        
    end process;

end architecture;
