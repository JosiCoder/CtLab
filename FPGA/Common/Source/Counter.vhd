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
-- Provides a simple counter with a synchronous clear.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;

entity Counter is
    generic
    (
        width: natural
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The synchronous clear.
		clear: in std_logic;
        -- The counter enable.
        ce: in std_logic; 
        -- The counter´s value.
        value: out unsigned (width-1 downto 0)
    );
end entity;

architecture stdarch of Counter is
    type reg_type is record
        value: unsigned (width-1 downto 0);
    end record;
    signal state, next_state: reg_type := (value => (others => '0'));
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
    next_state_logic: process(state, ce, clear) is
    begin
    
        -- Defaults.
        next_state <= state;
        
        -- Clear or increment.
        if (clear = '1') then
            next_state <= (value => (others => '0'));
        elsif (ce = '1') then
            next_state.value <= state.value + 1;
        end if;

    end process;


    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    value <= state.value;

end architecture;


