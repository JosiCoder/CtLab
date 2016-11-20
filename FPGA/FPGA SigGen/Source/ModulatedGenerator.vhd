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

---------------------------------------------------------------------------------
-- Provides a set of generators that create periodic signals (e.g. sine, square,
-- and sawtooth) and that can modulate each other. The generators´ current phases
-- are also available.
---------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.FunctionGenerator_Declarations.all;
use work.ModulatedGenerator_Declarations.all;

entity ModulatedGenerator is
    generic
    (
        -- The number of signal generators to create.
        number_of_generators: natural := 2;
        -- The width of the phase values.
        phase_width: natural := 32;
        -- The width of the phase values of the waveform lookup table (must not
        -- exceed phase_width).
        lookup_table_phase_width: natural := 12;
        -- The width of the level values.
        level_width: natural := 16;
        -- The width of the sample values.
        sample_width: natural := 16
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- For each generator, the configuration (e.g. modulation source or waveforms).
        configs: in modulated_generator_config_vector(number_of_generators-1 downto 0);
        -- For each generator, the increment to be added to the phase accumulator
        -- in each clock cycle.
        phase_increments: in modulated_generator_phase_vector(number_of_generators-1 downto 0);
        -- For each generator, the phase value to be added to the current phase value.
        phase_shifts: in modulated_generator_phase_vector(number_of_generators-1 downto 0);
        -- For each generator, the level value used to attenuate the sample signal.
        levels: in modulated_generator_level_vector(number_of_generators-1 downto 0);
        -- For each generator, the current internal phase value.  This is exactly
        -- synchronized to sample. Its MSB is 0 for the first half of the period
        -- and 1 for the second half.
        phases: out modulated_generator_phase_vector(number_of_generators-1 downto 0);
        -- For each generator, the sample value according to the current phase.
        samples: out modulated_generator_sample_vector(number_of_generators-1 downto 0)
    );
end entity;

architecture stdarch of ModulatedGenerator is
    -- Registered and internal input signals.
    type input_type is record
        configs: modulated_generator_config_vector(number_of_generators-1 downto 0);
        phase_increments: modulated_generator_phase_vector(number_of_generators-1 downto 0);
        phase_shifts: modulated_generator_phase_vector(number_of_generators-1 downto 0);
        levels: modulated_generator_level_vector(number_of_generators-1 downto 0);
    end record;
    signal reg_input, int_input: input_type;
    -- FM modulation phase increments shifted according to the selected FM range.
    signal shifted_phase_increments_reg: modulated_generator_phase_vector(number_of_generators-1 downto 0);
    -- Generator´s unregistered and registered interconnection signals.
    signal phases_int, phases_reg: modulated_generator_phase_vector(number_of_generators-1 downto 0);
    signal samples_int, samples_reg: modulated_generator_sample_vector(number_of_generators-1 downto 0);
    -- Generator´s synchronization signals.
    signal phase_resets: std_logic_vector(number_of_generators-1 downto 0);
begin

    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- Shifts the phase increment according to the selected FM range. Also adds one
    -- clock cycle latency for all input signals to satisfy the timing constraints.
    shift_phase_increments: process is
        variable source: integer;
    begin
        wait until rising_edge(clk);
        for i in 0 to number_of_generators-1 loop
            -- Derive the phase increment from the (propably shifted) sample.
            -- If the sample is less bits wide than the phase, a trade-off must
            -- be found between range and resolution. Several ranges using a
            -- factor of 8 are supported here for this case.
            source := to_integer(reg_input.configs(i).FM_source);
            shifted_phase_increments_reg(i) <=
                    unsigned
                    (
                        shift_left(resize(samples_reg(source), phase_width),
                               3*to_integer(reg_input.configs(i).FM_range))
                    );
        end loop;        
    end process;


    -- Adds one clock cycle latency for all input signals to satisfy the timing
    -- constraints.
    register_inputs: process is
    begin
        wait until rising_edge(clk);
        reg_input.configs <= configs;
        reg_input.phase_increments <= phase_increments;
        reg_input.phase_shifts <= phase_shifts;
        reg_input.levels <= levels;
    end process;
    
    
    -- Adds one clock cycle latency for all signals connecting signal generators 
    -- together to satisfy the timing constraints.
    register_interconnection: process is
    begin
        wait until rising_edge(clk);
        phases_reg <= phases_int;
        samples_reg <= samples_int;
    end process;
    
    
    -- Synchronizes the generators according to the synchronization sources selected.
    select_synchronization: process is
        variable source: integer;
        variable last_phase_msb: std_logic_vector(number_of_generators-1 downto 0)
            := (others => '0');
    begin
    
        wait until rising_edge(clk);
        
        -- Defaults (no phase reset).
        phase_resets <= (phase_resets'range => '0');

        for i in 0 to number_of_generators-1 loop
            -- Phase reset (synchronizatition).
            source := to_integer(reg_input.configs(i).sync_source);
            if (i /= source) then
                if phases_reg(source)(phases_reg(source)'high) = '0' and
                   last_phase_msb(source) = '1' then
                   -- There was a falling edge on the source´s phase MSB.
                    phase_resets(i) <= '1';
                end if;
                
                last_phase_msb(source) :=
                    phases_reg(source)(phases_reg(source)'high);
            end if;
        end loop;
    end process;


    -- Modifies the generators´ base values according to the modulations selected.
    -- This is done synchronously to meet timing requirements (otherwise the adders
    -- need too much time).
    select_modulation: process is
        constant width_difference: integer := phase_width - sample_width;
        variable source: integer;
        variable phase_from_sample: modulated_generator_phase;
    begin
    
        wait until rising_edge(clk);
    
        -- Defaults (no modulation).
        int_input <= reg_input;

        -- Modulate each generator using the selected modulation sources. If a
        -- generator is selected as its own modulation source, modulation is
        -- deactivated.
        for i in 0 to number_of_generators-1 loop

            -- Amplitude modulation.
            source := to_integer(reg_input.configs(i).AM_source);
            if (i /= source) then
                int_input.levels(i) <= reg_input.levels(i) + samples_reg(source);
            end if;

            -- Frequency modulation.
            source := to_integer(reg_input.configs(i).FM_source);
            if (i /= source) then
                -- Derive the phase increment from the (propably shifted) sample.
                int_input.phase_increments(i) <=
                    reg_input.phase_increments(i) + shifted_phase_increments_reg(i);
            end if;

            -- Phase modulation.
            source := to_integer(reg_input.configs(i).PM_source);
            if (i /= source) then
                -- Derive the phase from the sample (pad or truncate LSBs when
                -- widths are different).
                phase_from_sample := (others => '0');
                if (width_difference >= 0) then
                    phase_from_sample(phase_from_sample'high downto width_difference) := unsigned(samples_reg(source));
                else
                    phase_from_sample := unsigned(samples_reg(source)(samples_reg(source)'high downto -width_difference));
                end if;
                -- Add derived phase to current phase.
                int_input.phase_shifts(i) <= reg_input.phase_shifts(i) + phase_from_sample;
            end if;

        end loop;
        
    end process;


    --------------------------------------------------------------------------------
    -- Component instantiation.
    --------------------------------------------------------------------------------

    -- Create all the signal generators.
    signal_generators: for i in 0 to number_of_generators-1 generate
        signal_generator: entity work.SignalGenerator
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
            config => int_input.configs(i).generator,
            phase_increment => int_input.phase_increments(i),
            phase_shift => int_input.phase_shifts(i),
            level => int_input.levels(i),
            phase => phases_int(i),
            reset_phase => phase_resets(i),
            sample => samples_int(i)
        );
    end generate;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------

    phases <= phases_reg;
    samples <= samples_reg;

end architecture;
