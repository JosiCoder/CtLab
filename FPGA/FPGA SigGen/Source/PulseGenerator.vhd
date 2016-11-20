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
-- Provides a pulse generator controlled by values specifying the number of
-- clock cycles to hold the signal in high or low state.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity PulseGenerator is
    generic
    (
        -- The width of the internal clock cycle counter.
        counter_width: natural := 32
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The duration of the pulse´s high or low phase in clock cycles.
        high_duration, low_duration: in unsigned(counter_width-1 downto 0);
        -- The generated signal.
        pulse_signal: out std_logic
    );
end entity;

architecture stdarch of PulseGenerator is
    signal reg_high_duration, reg_low_duration: unsigned(counter_width-1 downto 0)
        := (others => '0');
    type reg_type is record
        phase: std_logic;
        counter: unsigned(counter_width-1 downto 0);
        old_high_duration, old_low_duration: unsigned(counter_width-1 downto 0);
    end record;
    signal state, next_state: reg_type :=
    (
        phase => '0',
        counter => (others => '0'),
        old_high_duration => (others => '0'),
        old_low_duration => (others => '0')
    );
begin

    -- Adds one clock cycle latency for all input signals to satisfy the timing
    -- constraints.
    register_inputs: process is
    begin
        wait until rising_edge(clk);
        reg_high_duration <= high_duration;
        reg_low_duration <= low_duration;
    end process;
    
    
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
    next_state_logic: process(state, reg_low_duration, reg_high_duration) is
    begin

        -- Defaults.
        next_state <= state;
        
        if state.counter = (counter_width-1 downto 0 => '0') then
            if state.phase = '1' then
                next_state.counter <= reg_low_duration - 1;
            else
                next_state.counter <= reg_high_duration - 1;
            end if;
            next_state.phase <= not state.phase;
        else
            if (reg_low_duration /= state.old_low_duration or
                reg_high_duration /= state.old_high_duration) then
                -- The duration vales have been changed, reset the counter for immediate
                -- response (otherwise a delay up to a complete counter cycle might occur).
                next_state.counter <= (next_state.counter'range => '0');
            else
                next_state.counter <= state.counter - 1;
            end if;
        end if;

        next_state.old_low_duration <= reg_low_duration;
        next_state.old_high_duration <= reg_high_duration;

    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    pulse_signal <= state.phase;
    
end architecture;
