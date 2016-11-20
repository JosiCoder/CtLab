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
-- Tests a simple counter.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;
 
entity Counter_Tester is
end entity;

architecture stdarch of Counter_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 5ns;
    constant test_width: integer := 32;
    constant test_delay: time := 1ns;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal reset: std_logic := '0';
    signal clear: std_logic := '0';
    signal counter_enable: std_logic := '0';

    --------------------
    -- Outputs
    --------------------
    signal value: unsigned(test_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;

begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------

    counter: entity work.Counter
    generic map
    (
        width => test_width
    )
    port map
    (
        clk => clk,
        clear => clear,
        ce => counter_enable,
        value => value
    );


    --------------------------------------------------------------------------------
    -- Generate the counter clock.
    --------------------------------------------------------------------------------
    clk <= not clk after clk_period/2 when run_test;


    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
        variable iteration: integer := 0;
    begin

        -- Count for a few cycles while enabling and disabling the counter and check
        -- whether the counter works properly.
        wait until falling_edge(clk);
        counter_enable <= '1';
        wait for 5 * clk_period;
        assert (value = to_unsigned(5, test_width)) report "Counter failed when enabled." severity error;
        counter_enable <= '0';
        wait for 5 * clk_period;
        assert (value = to_unsigned(5, test_width)) report "Counter failed when disabled." severity error;
        counter_enable <= '1';
        wait for 5 * clk_period;
        assert (value = to_unsigned(10, test_width)) report "Counter failed when re-enabled." severity error;

        -- Clear synchronously and check whether we get a zero value after the next counter
        -- clock cycle (but not before).
        wait until falling_edge(clk);
        clear <= '1';
        wait for test_delay;
        assert (value /= to_unsigned(0, test_width)) report "Counter cleared too early." severity error;
        wait until falling_edge(clk);
        assert (value = to_unsigned(0, test_width)) report "Counter not cleared." severity error;
        clear <= '0';
        
        -- Repeat the test once to verify proper continuous operation, then
        -- terminate the test.
        if (iteration = 0) then
            counter_enable <= '0';
            iteration := iteration + 1;
        else
            run_test <= false;
            wait;
        end if;
        
    end process;
    
end architecture;
