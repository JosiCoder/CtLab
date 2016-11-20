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

----------------------------------------------------------------------------------
-- Provides an SPI slave buffer that can be used for buffering data received or to
-- be sent. The buffer can be either controlled by the rising edge or by the level
-- of the enable signal, both synchronous to CLK).
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity SPI_SlaveDataBuffer is
    generic
    (
        -- The width of the data.
        width: positive;
        -- Indicates whether the buffer is triggered on a rising edge of the enable
        -- signal instead of the enable signal being high.
        edge_triggered: boolean
    );
    port
    (
        -- The system clock.
        clk: in std_logic; 
        -- Controls when the buffer passes the input data to its output (behaviour
        -- depends on the edge_triggered generic: either on the rising edge or when
        -- '1', both synchronous to CLK).
        buffer_enable: in std_logic; 
        -- The parallel unbuffered data.
        data: in std_logic_vector(width-1 downto 0);
        -- The parallel buffered data.
        buffered_data: out std_logic_vector(width-1 downto 0) := (others => '0');
        -- Indicates whether the buffered data are stable.
        ready: out std_logic := '0'
    );
end entity;

architecture stdarch of SPI_SlaveDataBuffer is
    type reg_type is record
        buffer_enable: std_logic;
        data: std_logic_vector(width-1 downto 0);
    end record;
    signal state, next_state: reg_type :=
    (
        buffer_enable => '0',
        data => (others => '0')
    );
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
    next_state_logic: process(state, buffer_enable, data) is
    begin
        -- Defaults.
        next_state <= state;
        
        next_state.buffer_enable <= buffer_enable;

        -- Buffer the input data either on a rising buffer_enable edge or when
        -- buffer_enable is '1'.
        if ((edge_triggered and state.buffer_enable = '0' and buffer_enable = '1') or
            ((not edge_triggered) and buffer_enable = '1')) then
            next_state.data <= data;
        end if;
    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    buffered_data <= state.data;
    ready <= state.buffer_enable;

end architecture;
