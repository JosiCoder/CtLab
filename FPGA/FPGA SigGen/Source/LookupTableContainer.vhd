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
-- Provides lookup tables for periodic signals, e.g. sine.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use ieee.math_real.all;  
use work.globals.all;

entity LookupTableContainer is
    generic
    (
        -- The width of the phase values.
        phase_width: natural := 12;
        -- The width of the sample values.
        sample_width: natural := 16
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The current phase value.
        phase: in unsigned (phase_width-1 downto 0);
        -- The sample value according to the current phase.
        sample: out signed (sample_width-1 downto 0)
    );
end entity;

architecture stdarch of LookupTableContainer is
    -- Lookup tables in general.
    signal lookup_sample: signed (sample_width-1 downto 0) :=
        (others => '0');
    -- Quarter period lookup tables (0..Pi/2), e.g. for sine or cosine.
    constant lookup_length: natural := (2**phase_width)/4;
    type quarter_period_lookup_type is array (0 to lookup_length-1) of
        signed (sample_width-1 downto 0);
    signal quarter_period_lookup_address: unsigned (phase_width-3 downto 0) :=
        (others => '0');
    signal invert_lookup_sample: std_logic := '0';
    -- Sine wave lookup table (for 0..Pi/2).
    signal sine_lookup: quarter_period_lookup_type;   
begin

    --------------------------------------------------------------------------------
    -- Lookup tables used for waveform generation.
    --------------------------------------------------------------------------------

    -- Creates the address used to read a sample value from a quarter period lookup
    -- table. This includes switching the address to backward counting when
    -- necessary. Also detects whether the read sample value must be inverted.
    create_quarter_period_lookup_address: process is
    begin
    
        wait until rising_edge(clk);

        -- Depending on the quadrant, determine the lookup table address.
        if phase(phase'left-1) = '0' then
            -- First or third quadrant, read forward.
            quarter_period_lookup_address <=
                phase(phase'left-2 downto 0);
        else
            -- Second or fourth quadrant, read backwards.
            quarter_period_lookup_address <= (lookup_length-1) -
                phase(phase'left-2 downto 0);
        end if;
        
        -- Mark to invert the value read from the lookup table for the second
        -- half period (third or fourth quadrant).
        invert_lookup_sample <= phase(phase'left);
        
    end process;

    -- Initializes the sine lookup table for the first quarter (0..Pi/2).
    fill_sine_lookup_table: for i in 0 to lookup_length-1 generate
        sine_lookup(i) <=
            to_signed
            (
                integer
                (
                    sin
                    (
                        -- Each item reflects the center of the interval,
                        -- not the beginning. Doing so, we can use the table
                        -- for all quarters in a completely symmetric way.
                        (real(i) + 0.5) * (MATH_PI/2.0) / real(lookup_length)
                    )
                    * ((real(2**sample_width)/2.0)-1.0)
                ),
                sample_width
            );
    end generate;
    

    -- Reads the value from the lookup table.
    -- Note: The lookup table address must be synchronized to get a block RAM
    -- inferred, check synthesis report.
    lookup_sample <= sine_lookup(to_integer(quarter_period_lookup_address));
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    sample <= lookup_sample when invert_lookup_sample = '0' else
              -lookup_sample;

end architecture;
