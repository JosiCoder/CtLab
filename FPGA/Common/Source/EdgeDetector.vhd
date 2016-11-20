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
-- Detects rising and/or a falling clock edges of a signal and provides a one-
-- clock-cycle tick on every edge.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity EdgeDetector is
    generic
    (
        detect_rising_edges: boolean := true;
        detect_falling_edges: boolean := false
    );
    port
    (
        -- The system clock.
        clk: in std_logic; 
        -- The signal to detect edges from.
        sigin: in std_logic;
        -- Indicates whether an edge has been detected in the current clock cycle.
        edge: out std_logic
    );
end entity;

architecture stdarch of EdgeDetector is
    signal reg_sigin: std_logic := '0';
begin

    -------------------------------------
    -- State register.
    -------------------------------------
    state_register: process is
    begin
        wait until rising_edge(clk);
        reg_sigin <= sigin;
    end process;

    
    -------------------------------------
    -- Output logic (Mealy).
    -------------------------------------

    detect_rising_edge: if (detect_rising_edges and (not detect_falling_edges)) generate
        edge <= (not reg_sigin) and sigin;
    end generate;

    detect_falling_edge: if ((not detect_rising_edges) and detect_falling_edges) generate
        edge <= reg_sigin and (not sigin);
    end generate;

    detect_both_edges: if (detect_rising_edges and detect_falling_edges) generate
        edge <= reg_sigin xor sigin;
    end generate;

end architecture;


