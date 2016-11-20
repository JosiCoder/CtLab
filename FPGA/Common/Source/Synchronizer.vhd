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
-- Synchronizes an asynchronous signal to a clock. Optionally, the asynchronous
-- signal´s high pulses can be stretched to be long enough to get reliably
-- fetched by the synchronizer.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
 
entity Synchronizer is
    generic
    (
        -- Determines whether to stretch the incoming pulse to match the destination
        -- clock's timing.
        pulse_stretcher: boolean := false;
        -- The number of FFs the synchronizer consists of (usually 2 is recommended).
        nr_of_stages: natural range 1 to 3 := 2
    );
    port
    (
        -- The system clock the asynchronous signal is synchronized to.
        clk: in std_logic;
        -- The asynchronous signal to be synchronized.
        in_async: in std_logic; 
        -- The synchronization stages.
        out_sync_stages: out std_logic_vector(nr_of_stages - 1 downto 0);
        -- The synchronized signal (i.e. the last synchronization stages´ output signal).
        out_sync: out std_logic
    );
end entity;

architecture stdarch of Synchronizer is

    type reg_type is record
        out_sync_stages: std_logic_vector(nr_of_stages - 1 downto 0);
    end record;
    signal state, next_state: reg_type := (out_sync_stages => (others => '0'));
    signal in_async_stretched: std_logic := '0';
    
    -- Constraints-
    attribute TIG: string;
    attribute TIG of in_async: signal is "TRUE";

begin

    ------------------------------------------
    -- Optional asynchronous pulse stretcher.
    ------------------------------------------
    
    stretch_pulse: if (pulse_stretcher) generate
        -- Set the stretched pulse immediately if a pulse arrives, reset it as soon as the
        -- synchronizer has fetched it and the pulse has been deactivated.
        stretcher: process(in_async, state.out_sync_stages(nr_of_stages-1)) is
        begin
            if (state.out_sync_stages(state.out_sync_stages'high) = '1' and in_async = '0') then
                in_async_stretched <= '0';
            elsif (rising_edge(in_async)) then
                in_async_stretched <= '1';
            end if;            
        end process;
    end generate;

    do_not_stretch_pulse: if (not pulse_stretcher) generate
        in_async_stretched <= in_async;
    end generate;


    ------------------------------------------
    -- State register.
    ------------------------------------------
    state_register: process is
    begin
        wait until rising_edge(clk);
        state <= next_state;
    end process;


    ------------------------------------------
    -- Next state logic.
    ------------------------------------------
    create_nr_of_stages: for i in nr_of_stages - 1 downto 1 generate
        next_state.out_sync_stages(i) <= state.out_sync_stages(i - 1);
    end generate;
    next_state.out_sync_stages(0) <= in_async_stretched;
 
 
    -------------------------------------
    -- Output logic
    -------------------------------------
    out_sync_stages <= state.out_sync_stages;
    out_sync <= state.out_sync_stages(state.out_sync_stages'high);

end architecture;
