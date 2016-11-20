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
-- Tests some entities used for synchronization.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;
 
entity Synchronization_Tester is
end entity;

architecture stdarch of Synchronization_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant slow_clock_period: time := 19ns;
    constant fast_clock_period: time := 5ns;
    constant test_nr_of_stages: integer := 2;

    --------------------
    -- Inputs
    --------------------
    signal slow_clock: std_logic := '0';
    signal fast_clock: std_logic := '0';
    signal fast_pulse: std_logic := '0';

    --------------------
    -- Outputs
    --------------------
    signal out_sync_slow_clock: std_logic;
    signal out_sync_fast_clock: std_logic;
    signal out_sync_fast_pulse: std_logic;
    signal out_sync_fast_toggled_pulse: std_logic;
    signal out_sync_slow_clock_stages: std_logic_vector(test_nr_of_stages - 1 downto 0);
    signal out_sync_fast_clock_stages: std_logic_vector(test_nr_of_stages - 1 downto 0);
    signal out_sync_fast_pulse_stages: std_logic_vector(test_nr_of_stages - 1 downto 0);
    signal out_sync_fast_toggled_pulse_stages: std_logic_vector(test_nr_of_stages - 1 downto 0);
    signal fast_toggled_pulse: std_logic;
    signal slow_clock_edge: std_logic := '0';
    signal fast_clock_edge: std_logic := '0';
    signal fast_pulse_edge: std_logic := '0';
    signal fast_pulse_edge_from_toggled: std_logic := '0';

begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------

    -- A circuit synchronizing a slow clock to a fast clock.
    ---------------------------------------------------------

    slow_clock_to_fast_clock_syncer: entity work.Synchronizer
    generic map
    (
        pulse_stretcher => false,
        nr_of_stages => test_nr_of_stages
    )
    port map
    (
        clk => fast_clock,
        in_async => slow_clock,
        out_sync_stages => out_sync_slow_clock_stages,
        out_sync => out_sync_slow_clock
    );

    slow_clock_edge_detector: entity work.EdgeDetector
    port map
    (
        clk => fast_clock,
        sigin => out_sync_slow_clock,
        edge => slow_clock_edge
    );


    -- A circuit synchronizing a fast clock to a slow clock without a pulse stretcher
    -- or toggler. Note that this results in missed clock periods. A pulse stretcher
    -- or toggler won't help here because the distance between the clock pulses is
    -- too small.
    ---------------------------------------------------------------------------------

    fast_clock_to_slow_clock_syncer: entity work.Synchronizer
    generic map
    (
        pulse_stretcher => false,
        nr_of_stages => test_nr_of_stages
    )
    port map
    (
        clk => slow_clock,
        in_async => fast_clock,
        out_sync_stages => out_sync_fast_clock_stages,
        out_sync => out_sync_fast_clock
    );

    fast_clock_edge_detector: entity work.EdgeDetector
    port map
    (
        clk => slow_clock,
        sigin => out_sync_fast_clock,
        edge => fast_clock_edge
    );


    -- A circuit synchronizing a fast pulse to a slow clock using a stretcher.
    --------------------------------------------------------------------------

    fast_pulse_to_slow_clock_syncer: entity work.Synchronizer
    generic map
    (
        pulse_stretcher => true,
        nr_of_stages => test_nr_of_stages
    )
    port map
    (
        clk => slow_clock,
        in_async => fast_pulse,
        out_sync_stages => out_sync_fast_pulse_stages,
        out_sync => out_sync_fast_pulse
    );

    fast_pulse_edge_detector: entity work.EdgeDetector
    port map
    (
        clk => slow_clock,
        sigin => out_sync_fast_pulse,
        edge => fast_pulse_edge
    );


    -- A circuit synchronizing a fast pulse to a slow clock using a toggler.
    ------------------------------------------------------------------------

    fast_pulse_toggler: entity work.Toggler
    port map
    (
        clk => fast_clock,
        sigin => fast_pulse,
        toggled_sigout => fast_toggled_pulse
    );

    toggled_fast_pulse_to_slow_clock_syncer: entity work.Synchronizer
    generic map
    (
        pulse_stretcher => false,
        nr_of_stages => test_nr_of_stages
    )
    port map
    (
        clk => slow_clock,
        in_async => fast_toggled_pulse,
        out_sync_stages => out_sync_fast_toggled_pulse_stages,
        out_sync => out_sync_fast_toggled_pulse
    );

    toggled_fast_pulse_edge_detector: entity work.EdgeDetector
    generic map
    (
        detect_rising_edges => true,
        detect_falling_edges => true
    )
    port map
    (
        clk => slow_clock,
        sigin => out_sync_fast_toggled_pulse,
        edge => fast_pulse_edge_from_toggled
    );


    --------------------------------------------------------------------------------
    -- Generate the periodic signals.
    --------------------------------------------------------------------------------
    slow_clock <= not slow_clock after slow_clock_period/2;
    fast_clock <= not fast_clock after fast_clock_period/2;
    fast_pulse <= '1' after 10*fast_clock_period when fast_pulse = '0' else
                  '0' after fast_clock_period;
                  
                  
    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
        variable iteration: integer := 0;
    begin
    
        -- Automatic verification is not provided here.
        wait for 1us;
        report "Test finished, verify timing visually." severity failure;
        
    end process;
    
end architecture;
