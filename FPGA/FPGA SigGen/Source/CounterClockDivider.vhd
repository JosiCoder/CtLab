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
-- Generates the universal counter´s internal pulse or gate signal by dividing
-- the system clock.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library Common;
use work.Globals.all;


entity CounterClockDivider is
    generic
    (
        -- The width of the internal clock divider.
        clock_divider_width: integer := 32;
        -- '0' for normal operation, '1' to ignore the real system clock
        -- frequency when dividing the clock frequency.
        clock_divider_test_mode: boolean := false
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- A value indicating the clock division mode.
        clk_division_mode: in std_logic_vector (3 downto 0);
        -- Gets active for one clock cycle every time the clock division value
        -- has been reached.
        divider_tick: out std_logic
    );
end entity;

architecture stdarch of CounterClockDivider is
    constant clock_frequency: natural := 50 * 10**6;

    -- Clock divider.
    type reg_type is record
        division_counter: unsigned (clock_divider_width-1 downto 0);
        divider_tick: std_logic;
    end record;
    signal state, next_state: reg_type :=
    (
        division_counter => (others => '0'),
        divider_tick => '0'
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
    next_state_logic: process(state, clk_division_mode) is
        variable clock_divider: unsigned (clock_divider_width-1 downto 0);
    begin
    
        -- Defaults.
        next_state <= state;
        next_state.divider_tick <= '0';

        -- Select the clock division value.
        if (clock_divider_test_mode) then
            -- Special value for test purposes, don't divide by real system clock frequency.
            clock_divider := to_unsigned(10, clock_divider_width);
        else
            case clk_division_mode is
                -- Frequency measurement.
                when "0000" => -- 1 s gate signal
                    clock_divider := to_unsigned(1 * clock_frequency, clock_divider_width);
                when "0001" => -- 0.1 s gate signal
                    clock_divider := to_unsigned(10 * clock_frequency, clock_divider_width);
                when "0010" => -- 10 s gate signal
                    clock_divider := to_unsigned(clock_frequency / 10, clock_divider_width);
                -- Period measurement.
                when "0100" => -- 10 MHz pulse signal
                    clock_divider := to_unsigned(clock_frequency/10**7, clock_divider_width);
                when "0101" => -- 1 MHz pulse signal
                    clock_divider := to_unsigned(clock_frequency/10**6, clock_divider_width);
                when "0110" => -- 100 kHz pulse signal
                    clock_divider := to_unsigned(clock_frequency/10**5, clock_divider_width);
                when "0111" => -- 10 kHz pulse signal
                    clock_divider := to_unsigned(clock_frequency/10**4, clock_divider_width);
                when others =>
                    clock_divider := to_unsigned(1 * clock_frequency, clock_divider_width);
            end case;
        end if;
        
        -- Check the clock division counter and, if it has reached the division value, raise
        -- the divider tick signal for one clock cycle. Ensure that the clock division counter
        -- is reset if it reaches or exceeds the division value (exceeding might happen after
        -- changing the division value).
        if (state.division_counter >= clock_divider-1) then
            next_state.division_counter <= (others => '0');
            if (state.division_counter = clock_divider-1) then
                next_state.divider_tick <= '1';
            end if;
        else
            next_state.division_counter <= state.division_counter + 1;
        end if;

    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    divider_tick <= state.divider_tick;

end architecture;
