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
-- Provides some generic constants and types.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;
use work.FunctionGenerator_Declarations.all;
 
package ModulatedGenerator_Declarations is

    -- Configuration structure.
    type modulated_generator_config is record
        generator: generator_config;
        AM_source: unsigned(1 downto 0);
        FM_source: unsigned(1 downto 0);
        PM_source: unsigned(1 downto 0);
        FM_range: unsigned(4 downto 0);
        sync_source: unsigned(1 downto 0);
    end record;
    type modulated_generator_config_vector is array (natural range <>) of modulated_generator_config;

    -- Unfortunately, VHDL doesn´t yet support arrays of unconstrained arrays. Thus
    -- we can´t use generics in this case and have to declare the ranges here.
    constant modulated_generator_phase_width: natural := 32;
    constant modulated_generator_level_width: natural := 16;
    constant modulated_generator_sample_width: natural := 16;


    -- Generator value subtypes
    subtype modulated_generator_phase is unsigned(modulated_generator_phase_width-1 downto 0);
    subtype modulated_generator_level is signed(modulated_generator_level_width-1 downto 0);
    subtype modulated_generator_sample is signed(modulated_generator_sample_width-1 downto 0);

    -- Generator value aggregates.
    type modulated_generator_phase_vector is
        array (natural range <>) of modulated_generator_phase;
    type modulated_generator_level_vector is
        array (natural range <>) of modulated_generator_level;
    type modulated_generator_sample_vector is
        array (natural range <>) of modulated_generator_sample;

end;
