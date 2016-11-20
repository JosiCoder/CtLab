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

---------------------------------------------------------------------------------
-- Creates periodic signals (e.g. sine, square, and sawtooth) using a phase
-- accumulator. The current phase is also available.
---------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.FunctionGenerator_Declarations.all;

entity SignalGenerator is
    generic
    (
        -- The width of the phase values.
        phase_width: natural := 32;
        -- The width of the phase values of the waveform lookup table (must not
        -- exceed phase_width).
        lookup_table_phase_width: natural := 12;
        -- The width of the level values.
        level_width: natural := 16;
        -- The width of the sample values.
        sample_width: natural := 16
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The configuration (e.g. waveform).
        config: in generator_config;
        -- The increment to be added to the phase accumulator in each clock cycle.
        phase_increment: in unsigned (phase_width-1 downto 0);
        -- The phase value to be added to the current phase value before determining
        -- the sample value. Corresponds to a range of 0..2*Pi.
        phase_shift: in unsigned(phase_width-1 downto 0);
        -- The level value used to attenuate the sample signal.
        level: in signed (level_width-1 downto 0);
        -- A signal used to reset the generator큦 phase.
        reset_phase: in std_logic;
        -- The current internal phase value.  This is exactly synchronized to sample.
        -- Its MSB is 0 for the first half of the period and 1 for the second half.
        phase: out unsigned (phase_width-1 downto 0);
        -- The sample value according to the current phase.
        sample: out signed (sample_width-1 downto 0)
    );
end entity;

architecture stdarch of SignalGenerator is
    signal phase_int, phase_int_reg: unsigned(phase_width-1 downto 0) := (others => '0');
    signal phase_int_shifted: unsigned(phase_width-1 downto 0) := (others => '0');
    signal limited_level: signed (level_width-1 downto 0) := (others => '0');
    signal sample_int, sample_int_reg: signed (sample_width-1 downto 0) := (others => '0');
    signal sample_level_product: signed (2*sample_width-1 downto 0) := (others => '0');
    -- Declarations necessary to delay phase to get in sync with sample.
    constant phase_sync_delay: integer := 3; -- delay in clk cycles
    type phase_vector is array (natural range <>) of unsigned(phase_width-1 downto 0);
    signal phase_int_sync: phase_vector(phase_sync_delay-1 downto 0) := (others => (others => '0'));
begin

    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- Add one clock cycle latency to meet timing requirements when adding phase shift.
    add_phase_shift: process is
    begin
        wait until rising_edge(clk);
        phase_int_reg <= phase_int;
    end process;

    -- Shift the current phase value before determining the sample value.
    phase_int_shifted <= phase_int_reg + phase_shift;

    -- The MSB of the multiplier output is only needed if both inputs have the 
    -- smallest possible values: -(2**(n-1)). If at least one of the inputs is
    -- limited to -(2**(n-1)-1), the MSB is always identical to the next lower-order
    -- bit. To use the entire multiplier output range (and thus the entire sample
    -- value range), we limit the level큦 value here.
    process (level) is
    begin
        limited_level <= level;
        if (level(level_width-1) = '1' and
            level(level_width-2 downto 0) = (level_width-2 downto 0 => '0'))
        then
            limited_level(0) <= '1';
        end if;
    end process;


    -- Attenuate the sample value according to the level.
    attenuate_sample: process is
    begin
        -- Xilinx XST will infer a multiplier here. As an alternative, a multiplier can
        -- be instantiated explicitly. Either a multiplier supplied by the FPGA vendor
        -- or an own implementation can be used. Portable implementations can be found
        -- e.g. in that book: Pong P. Chu, "RTL Hardware Design Using VHDL".
        wait until rising_edge(clk);
        sample_level_product <= limited_level * sample_int_reg;
    end process;
        

    -- Delay returned signals to get them in sync with the sample signal.
    delay_signals: process is
    begin
        wait until rising_edge(clk);
        phase_int_sync(phase_int_sync'high downto 1) <=
            phase_int_sync(phase_int_sync'high-1 downto 0);
        phase_int_sync(0) <= phase_int_reg;
    end process;


    -- Add one clock cycle latency to satisfy timing constraints between function
    -- generator and level multiplier.
    decouple_sample: process is
    begin
        wait until rising_edge(clk);
        sample_int_reg <= sample_int;
    end process;


    --------------------------------------------------------------------------------
    -- Component instantiation.
    --------------------------------------------------------------------------------

    -- The phase generator controlled by a phase accumulator. Generates cyclic
    -- phase values used for DDS signal generation.
	phase_generator: entity work.PhaseGenerator
    generic map
    (
        phase_width => phase_width
    )
    port map
    (
		clk => clk,
		phase_increment => phase_increment,
        reset_phase => reset_phase,
		phase => phase_int
	);

    -- The function generator producing miscellaneous waveforms using cyclic phase
    -- values.
    function_generator: entity work.FunctionGenerator
    generic map
    (
        phase_width => phase_width,
        lookup_table_phase_width => lookup_table_phase_width,
        sample_width => sample_width
    )
    port map
    (
        clk => clk,
        config => config,
        phase => phase_int_shifted,
        sample => sample_int
    );


    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------

    -- Provides the output signals synchronously, i.e. registered. This is done to
    -- satisfy the timing requirements. Otherwise all delays including those of the
    -- multiplier are too long for one clock cycle.
    
    provide_output: process is
    begin
    
        wait until rising_edge(clk);

        -- Provide the MSB큦 of the product created from the level and sample values.
        -- We have ensured that only one input value of the multiplier can be
        -- -(2**(n-1)). Thus we can ignore the result큦 MSB and use the full output
        -- range.
        sample <= sample_level_product(2*sample_width-2 downto sample_width-1);

        -- Return the phase value that큦 synchronized to the sample value.
        phase <= phase_int_sync(phase_int_sync'high);
        
    end process;

end architecture;
