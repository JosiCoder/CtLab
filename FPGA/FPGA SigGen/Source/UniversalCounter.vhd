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
-- Measures an external signal´s frequency or period. To do this, this component
-- counts an asynchronous external pulse signal using an internal gate signal or
-- an internal pulse signal using an asynchronous external gate signal. Counting
-- is done using a synchronized gated counter. The counter itself runs in the
-- clock domain of the pulse signal. The gate signal is synchronized to the
-- pulse signal. The outputs also belong to the pulse signal´s clock domain.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library Common;
use work.Globals.all;


entity UniversalCounter is
    generic
    (
        -- The width of the measured frequency or period value.
        counter_width: natural := 32;
        -- The width of the internal clock divider.
        clock_divider_width: natural := 32;
        -- '0' for normal operation, '1' to ignore the real system clock
        -- frequency when dividing the clock frequency.
        clock_divider_test_mode: boolean := false
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- '0' to hold output values, '1' to update them.
        update_output: in std_logic;
        -- The signal to measure the frequency or the period from.
        external_signal: in std_logic;
        -- '0' for frequency measurement, '1' for period measurement.
        measure_period: in std_logic; 
        -- A value indicating the clock division mode.
        clk_division_mode: in std_logic_vector (3 downto 0);
        -- The measured frequency or period value.
        value: out unsigned (counter_width-1 downto 0);
        -- '1' if an overflow has occurred in the current measurement period.
        overflow: out std_logic;
        -- Toggles each time a gate event has been successfully detected which
        -- indicates that there are proper gate and pulse signals.
        ready: out std_logic
--TODO: ready umbenennen und ggf. von toggeln auf Puls umstellen (muss dann aber bis zum Auslesen per SPI gespeichert werden).
    );
end entity;

architecture stdarch of UniversalCounter is

    -- Gate, pulse, and related signals.
    signal external_signal_synced_to_clk: std_logic := '0';
    signal internal_signal: std_logic := '0';
    signal pulse_signal: std_logic := '0';
    signal gate_signal, toggled_gate_signal: std_logic := '0';
    signal toggled_gate_detected, toggled_gate_detected_synced_to_clk: std_logic := '0';
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    -- Synchronizes the external signal to the system clock. This is necessary when
    -- it is used as a gate signal because in this case we must toggle it using clk.
    external_synchronizer: entity Common.Synchronizer
    generic map
    (
        pulse_stretcher => false,
        nr_of_stages => 2
    )
    port map
    (
        clk => clk,
        in_async => external_signal,
        out_sync => external_signal_synced_to_clk
    );    


    -- Toggles the gate signal so that we can pass it safely to the pulse´s clock
    -- domain. There, a dual edge detector will reconstruct the signal.
    fast_pulse_toggler: entity Common.Toggler
    port map
    (
        clk => clk,
        sigin => gate_signal,
        toggled_sigout => toggled_gate_signal
    );


    -- Generates the pulse or gate signal for the counter.
    clock_divider: entity work.CounterClockDivider
    generic map
    (
        clock_divider_width => clock_divider_width,
        clock_divider_test_mode => clock_divider_test_mode
    )
    port map
    (
        clk => clk,
        clk_division_mode => clk_division_mode,
        divider_tick => internal_signal
    );


    -- Counts a pulse signal with respect to a gate signal. This counter runs in the
    -- clock domain of the pulse signal. It synchronizes ingoing signals properly but
    -- outgoing signals belong to that domain.
    counter: entity work.SynchronizedGatedCounter
    generic map
    (
        counter_width => counter_width
    )
    port map
    (
        pulse_signal => pulse_signal,
        toggled_gate_signal => toggled_gate_signal,
        update_output => update_output,
        value => value,
        overflow => overflow,
        toggled_gate_detected => toggled_gate_detected
    );


    -- Synchronizes the value ready indicator to the system clock.
    overflow_synchronizer: entity Common.Synchronizer
    generic map
    (
        pulse_stretcher => false,
        nr_of_stages => 2
    )
    port map
    (
        clk => clk,
        in_async => toggled_gate_detected,
        out_sync => toggled_gate_detected_synced_to_clk
    );
    

    --------------------------------------------------------------------------------
    -- Configuration logic (asynchronous but rarely changed).
    --------------------------------------------------------------------------------

    -- Connect the external and internal signals to pulse and gate corresponding to
    -- the selected signal mode. Note that the two input signals (internal_signal
    -- and external_signal) are asynchronous to each other. One of these two signals
    -- is selected as the counters pulse (pulse_signal) that also acts as the
    -- reference for synchronization. Thus, this switchover must be done asynchronously.
    -- But this happens rather seldom and any metastability settles down automatically
    -- here. Thus this is ignored.
    select_measurement_mode: process (measure_period, internal_signal, external_signal,
                                      external_signal_synced_to_clk) is
    begin
        if (measure_period = '1') then
            -- External gate (synced to clk) and internal pulse.
            gate_signal <= external_signal_synced_to_clk;
            pulse_signal <= internal_signal;
        else
            -- Internal gate and external pulse (no sync necessary here).
            gate_signal <= internal_signal;
            pulse_signal <= external_signal;
        end if;
    end process;
    
    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    ready <= toggled_gate_detected_synced_to_clk;

end architecture;
