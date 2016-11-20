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
-- Creates sine, square, triangle, and sawtooth function values from an exter-
-- nally supplied phase value (i.e. no time-related signal is generated here).
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.FunctionGenerator_Declarations.all;

entity FunctionGenerator is
    generic
    (
        -- The width of the phase values.
        phase_width: natural := 32;
        -- The width of the phase values of the waveform lookup table (must not
        -- exceed phase_width).
        lookup_table_phase_width: natural := 12;
        -- The width of the sample values.
        sample_width: natural := 16
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The configuration (e.g. waveforms).
        config: in generator_config;
        -- The current phase value.
        phase: in unsigned (phase_width-1 downto 0);
        -- The sample value according to the current phase.
        sample: out signed (sample_width-1 downto 0) := (others => '0')
    );
end entity;

architecture stdarch of FunctionGenerator is
    type reg_type is record
        sample: signed(sample_width-1 downto 0);
    end record;
    signal state, next_state: reg_type :=
    (
        sample => (others => '0')
    );
    signal lookup_sample: signed (sample_width-1 downto 0);
begin

    --------------------------------------------------------------------------------
    -- Component instantiation.
    --------------------------------------------------------------------------------

    lookup_table: entity work.LookupTableContainer
    generic map
    (
        phase_width => lookup_table_phase_width,
        sample_width => sample_width
    )
    port map
    (
        clk => clk,
        phase => phase(phase'left downto phase_width-lookup_table_phase_width),
        sample => lookup_sample
    );


    --------------------------------------------------------------------------------
    -- State register.
    --------------------------------------------------------------------------------
    state_register: process is
    begin
        wait until rising_edge(clk);
        state <= next_state;
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Next state logic.
    --------------------------------------------------------------------------------
    next_state_logic: process(state, config, phase) is
        constant width_difference: integer := sample_width - phase_width;
    begin
    
        -- Defaults.
        next_state <= state;

        -- Create the next sample according to the selected waveform. Note that
        -- the sample is signed and thus uses two´s complement. Samples read from
        -- a lookup table are not handled here.
        case config.waveform is
            when waveform_square =>
                -- Derive the square signal from the phase´s MSB.
                next_state.sample(next_state.sample'high) <= phase(phase'high);
                next_state.sample(next_state.sample'high-1 downto 0) <= (others => not phase(phase'high));
            when waveform_sawtooth =>
                -- Derive the sawtooth signal from the phase (pad or truncate LSBs
                -- when widths are different).
                next_state.sample <= (others => '0');
                if (width_difference >= 0) then
                    next_state.sample(next_state.sample'high downto width_difference) <= signed(phase);
                else
                    next_state.sample <= signed(phase(phase'high downto -width_difference));
                end if;
            when others =>
        end case;
        
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    select_sample: process(state.sample, lookup_sample) is
    begin
    
        sample <= state.sample;

        -- Select whether to use samples created here or those read from a lookup
        -- table.
        if config.waveform = waveform_sine then
            sample <= lookup_sample;
        end if;

    end process;        

end architecture;
