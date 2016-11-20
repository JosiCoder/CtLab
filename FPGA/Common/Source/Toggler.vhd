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
-- Creates a signal that toggles at every rising edge of an input signal. This
-- can be used before passing a signal (e.g. of short pulses) to a different
-- clock domain. A dual edge detector can be used in the destination clock domain
-- to reconstruct the signal.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity Toggler is
    port
    (
        -- The system clock.
        clk: in std_logic; 
        -- The signal to create a toggled signal from.
        sigin: in std_logic; 
        -- The created toggled signal.
        toggled_sigout: out std_logic
    );
end entity;

architecture stdarch of Toggler is
    type reg_type is record
        sigout: std_logic;
    end record;
    signal state, next_state: reg_type := (sigout => '0');
    signal sigin_edge: std_logic := '0';
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    detect_edges_of_sigin: entity work.EdgeDetector
    port map
    (
        clk => clk,
        sigin => sigin,
        edge => sigin_edge
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
    next_state_logic: process(state, sigin_edge) is
    begin
    
        -- Defaults.
        next_state <= state;
        
        if (sigin_edge = '1') then
            next_state.sigout <= not state.sigout;
        end if;
        
    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    toggled_sigout <= state.sigout;

end architecture;


