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
-- Provides a simple counter capable of counting a faster pulse signal using a
-- slower gate signal. Uses a freely running counter that is not cleared. When
-- the gate signal is '1', the counter´s value is stored. The measurement value
-- is determined by calculating the difference between the current and the
-- previous counter value. Overflow is detected and signalled.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library Common;
use work.Globals.all;

entity GatedCounter is
    generic
    (
        -- The width of the measured frequency or period value.
        counter_width: natural := 32
    );
    port
    (
        -- The pulse signal that drives the counter.
        pulse_signal: in std_logic;
        -- The signal that controls the counter. The rising pulse edges between
        -- two consecutive gate '1' signals are counted. Note that the gate signal
        -- is level-sensitive, not edge-sensitive.
        gate_signal: in std_logic; 
        -- The measured frequency or period value.
        value: out unsigned (counter_width-1 downto 0);
        -- '1' if an overflow has occurred in the current measurement period.
        overflow: out std_logic
    );
end entity;

architecture stdarch of GatedCounter is
    type reg_type is record
        finished_counter_value, old_finished_counter_value: unsigned (counter_width-1 downto 0);
        running_overflow, finished_overflow: std_logic;
    end record;
    signal state, next_state: reg_type :=
    (
        finished_counter_value => (others => '0'),
        old_finished_counter_value => (others => '0'),
        running_overflow => '0',
        finished_overflow => '0'
    );
    signal running_counter_value: unsigned (counter_width-1 downto 0) := (others => '0');
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    -- Counts the pulse signal continuously.
    counter: entity Common.Counter
    generic map
    (
        width => counter_width
    )
    port map
    (
        clk => pulse_signal,
        clear => '0',
        ce => '1',
        value => running_counter_value
    );


    --------------------------------------------------------------------------------
    -- State and data register.
    --------------------------------------------------------------------------------
    state_register: process is
    begin
        wait until rising_edge(pulse_signal);
        state <= next_state;
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Next state logic.
    --------------------------------------------------------------------------------
    next_state_logic: process(state, running_counter_value, gate_signal) is
    begin
    
        -- Defaults.
        next_state <= state;
        
        if (gate_signal = '1') then
            -- An active ('1') gate signal has been detected. We finish the current
            -- measurement cycle and start a new one.
        
            -- Memorize the counter value and overflow status.
            next_state.finished_counter_value <= running_counter_value;
            next_state.old_finished_counter_value <= state.finished_counter_value;
            next_state.finished_overflow <= state.running_overflow;
            
            if (running_counter_value = state.finished_counter_value) then
                -- There was an overflow at the very end of the measurement cycle(i.e. the counter
                -- has wrapped around and reached the same value again). Set the finished counter
                -- value´s overflow marker directly.
                next_state.finished_overflow <= '1';
            else
                -- There was no overflow at the very end of the measurement cycle, we just keep
                -- the overflows that have happened so far.
                next_state.finished_overflow <= state.running_overflow;
            end if;

            -- Reset the running counter value´s overflow marker as we start a new measurement
            -- cycle.
            next_state.running_overflow <= '0';
            
        elsif (running_counter_value = state.finished_counter_value) then
            -- We´re not at the end of a measurement cycle but there was an overflow (i.e. the counter
            -- has wrapped around and reached the same value again). This might occur even multiple
            -- times per gate signal period, we capture the first occurrence.
            next_state.running_overflow <= '1';
        end if;

    end process;


    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    
    -- Calculate the number of pulse cycles between the last and the last but one
    -- active gate signal. This works even if the freely running counter wraps around
    -- as long as the last value stays below the last but one value.
    value <= state.finished_counter_value - state.old_finished_counter_value;
    
    overflow <= state.finished_overflow;
    
end architecture;


