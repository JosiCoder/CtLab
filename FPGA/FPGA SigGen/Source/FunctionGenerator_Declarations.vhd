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
 
package FunctionGenerator_Declarations is

    -- Subtypes used for enumerations.
    subtype waveform is std_logic_vector(2 downto 0);

    -- Waveforms.
    constant waveform_square: waveform := "000";
    constant waveform_sawtooth: waveform := "001";
    constant waveform_sine: waveform := "010";

    -- Configuration structure.
    type generator_config is record
        waveform: waveform;
    end record;
    type generator_config_vector is array (natural range <>) of generator_config;

end;
