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
-- Provides a phase generator controlled by a phase accumulator. It is
-- recommended to use increments only up to the half of the accumulator´s
-- capacity (MSB=1, all others 0). This ensures a duty cycle of 50% in the MSB
-- of the phase when it is used as a frequency signal. The current phase might
-- be used for DDS signal generation.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity PhaseGenerator is
    generic
    (
        -- The width of the phase values.
        phase_width: natural := 32
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The increment to be added to the phase accumulator in each clock cycle.
        phase_increment: in unsigned (phase_width-1 downto 0);
        -- A signal used to reset the generator´s phase.
        reset_phase: in std_logic;
        -- The current phase value.
        phase: out unsigned (phase_width-1 downto 0) := (others => '0')
    );
end entity;

architecture stdarch of PhaseGenerator is
    type reg_type is record
        phase_accumulator: unsigned (phase_width-1 downto 0);
    end record;
    signal state, next_state: reg_type := (phase_accumulator => (others => '0'));
begin

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
    next_state.phase_accumulator <=
        (next_state.phase_accumulator'range => '0') when reset_phase = '1'
        else state.phase_accumulator + phase_increment;
    
    
    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    phase <= state.phase_accumulator;
        
end architecture;


