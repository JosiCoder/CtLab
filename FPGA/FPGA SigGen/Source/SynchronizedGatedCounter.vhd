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
-- Counts a pulse signal using a gate signal that might be asynchronous. Counting
-- is done using a gated counter that runs in the clock domain of the pulse
-- signal. The gate signal is synchronized to the pulse signal. The outputs also
-- belong to the pulse signal´s clock domain.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library Common;
use work.Globals.all;


entity SynchronizedGatedCounter is
    generic
    (
        -- The width of the measured frequency or period value.
        counter_width: natural := 32
    );
    port
    (
        -- The signal to measure the frequency or the period from.
        pulse_signal: in std_logic;
        -- The gate signal used control the counter. Both edges of this signal
        -- are treated as gate events.
        toggled_gate_signal: in std_logic;
        -- '0' to hold output values, '1' to update them.
        update_output: in std_logic;
        -- The counted value.
        value: out unsigned (counter_width-1 downto 0);
        -- '1' if an overflow has occurred in the current counting period.
        overflow: out std_logic;
        -- Toggles each time a gate event has been successfully detected which
        -- indicates that there are proper gate and pulse signals.
        toggled_gate_detected: out std_logic
    );
end entity;

architecture stdarch of SynchronizedGatedCounter is

    -- Internal and registered output values.
    type output_reg_type is record
        value: unsigned (counter_width-1 downto 0);
        overflow: std_logic;
    end record;
    signal output_state, next_output_state: output_reg_type :=
    (
        value => (others => '0'),
        overflow => '0'
    );

    -- Miscellaneous control signals.
    signal toggled_gate_signal_synced_to_pulse, gate_edge: std_logic := '0';
    signal update_output_synced_to_pulse: std_logic := '0';
    
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    -- Synchronizes the toggled counter gate signal to the counter pulse signal.
    gate_synchronizer: entity Common.Synchronizer
    generic map
    (
        pulse_stretcher => false,
        nr_of_stages => 2
    )
    port map
    (
        clk => pulse_signal,
        in_async => toggled_gate_signal,
        out_sync => toggled_gate_signal_synced_to_pulse
    );    


    -- Detects both edges of the toggled gate signal to reconstruct a real gate
    -- signal for the counter.
    toggling_gate_edge_detector: entity Common.EdgeDetector
    generic map
    (
        detect_rising_edges => true,
        detect_falling_edges => true
    )
    port map
    (
        clk => pulse_signal,
        sigin => toggled_gate_signal_synced_to_pulse,
        edge => gate_edge
    );


    -- Counts a pulse signal with respect to the gate signal. This counter runs in
    -- the clock domain of the pulse signal, all in- and output signals belong to
    -- the corresponding clock domain.
    counter: entity work.GatedCounter
    generic map
    (
        counter_width => counter_width
    )
    port map
    (
        pulse_signal => pulse_signal,
        gate_signal => gate_edge,
        value => next_output_state.value,
        overflow => next_output_state.overflow
    );


    -- Synchronizes the update output signal to the counter pulse signal.
    update_output_synchronizer: entity Common.Synchronizer
    generic map
    (
        pulse_stretcher => true,
        nr_of_stages => 2
    )
    port map
    (
        clk => pulse_signal,
        in_async => update_output,
        out_sync => update_output_synced_to_pulse
    );
    

    --------------------------------------------------------------------------------
    -- State and data register.
    --------------------------------------------------------------------------------
    pulse_state_register: process is
    begin
        wait until rising_edge(pulse_signal);
        if (update_output_synced_to_pulse = '1') then
            output_state <= next_output_state;
        end if;
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    value <= output_state.value;
    overflow <= output_state.overflow;
    toggled_gate_detected <= toggled_gate_signal_synced_to_pulse;

end architecture;
