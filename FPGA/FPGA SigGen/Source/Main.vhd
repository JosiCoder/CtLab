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
-- Integrates all componentes to provide a synthesizable application.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library SPI_Interface;
use SPI_Interface.globals.all;
use work.FunctionGenerator_Declarations.all;
use work.ModulatedGenerator_Declarations.all;

entity Main is
    generic
    (
        -- The width of the DAC.
        dac_data_width: natural := modulated_generator_sample_width;
        -- The number of DDS generators.
        number_of_dds_generators: natural := 4
    );
    port
    (
        -- The system clock.
        sysclk: in std_logic; 
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The address slave select (low during transmission).
        ssreg: in std_logic;
        -- The data slave select (low during transmission).
        ssdat: in std_logic;
        -- The serial input.
        mosi: in std_logic; 
        -- The universal counter input.
        cntr_in: in std_logic;
        -- The serial output.
        miso: out std_logic; 
        -- The test LED output.
        test_led: out std_logic;
        -- The DDS generators´ synchronization outputs.
        dds_sync_out: out std_logic_vector(number_of_dds_generators-1 downto 0);
        -- The pulse generator output.
        pulse_out: out std_logic;
        -- The RAM´s write enable signal (active low).
        ram_we_n: out std_logic;
        -- The RAM´s output enable signal (active low).
        ram_oe_n: out std_logic;
        -- The DAC´s clock (single DAC U2 only).
        dac_clk: out std_logic;
        -- The DAC´s channel selection signal (dual DAC U5 only).
        dac_channel_select: out std_logic;
        -- The DAC´s write signal (dual DAC U5 only, active low).
        dac_write_n: out std_logic;
        -- The DAC´s value.
        dac_value: out std_logic_vector(dac_data_width-1 downto 0)
    );
end entity;

architecture stdarch of Main is

    -- Configuration constants.
    constant address_width: positive := 5; -- max. 8 (for addresses 0..255)
    constant dds_generator_lookup_table_phase_width: natural := 12;
    constant pulse_generator_counter_width: natural := 32;
    constant number_of_data_buffers: positive := 2**address_width;
    constant dds_generator_phase_width: natural := modulated_generator_phase_width;
    constant dds_generator_level_width: natural := modulated_generator_level_width;
    constant dds_generator_sample_width: natural := dac_data_width;
    constant dac_source_selector_width: natural := 4;

    -- SPI sub-address constants.
    type integer_vector is array (natural range <>) of integer;
    -- SPI receiver sub-addresses.
    constant dds_generator_base_subaddr: integer_vector(0 to number_of_dds_generators-1)
        := (0=>16, 1=>20, 2=>24, 3=>28); -- need n..n+2 each
    constant peripheral_configuration_subaddr: integer := 3;
    constant universal_counter_config_subaddr: integer := 12;
    constant pulse_generator_high_duration_subaddr: integer := 15;
    constant pulse_generator_low_duration_subaddr: integer := 14;
    -- SPI transmitter sub-addresses.
    constant universal_counter_state_subaddr: integer := 4;
    constant universal_counter_value_subaddr: integer := 5;

    -- Clocks.
    signal clk_50mhz: std_logic;
    signal clk_100mhz: std_logic;

    -- SPI.
    signal transmit_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0)
            := (others => (others => '0'));
    signal received_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal ready_x: std_logic_vector(number_of_data_buffers-1 downto 0);
    signal miso_int: std_logic;

    -- DDS signal generators.
    signal dds_generator_configs_raw: data_buffer_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_configs: modulated_generator_config_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_phase_increments: modulated_generator_phase_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_phase_shifts: modulated_generator_phase_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_levels: modulated_generator_level_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_phases: modulated_generator_phase_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_samples: modulated_generator_sample_vector(number_of_dds_generators-1 downto 0);
    signal dds_sync_int: std_logic_vector(number_of_dds_generators-1 downto 0);

    -- Pulse generator.
    signal pulse_generator_high_duration, pulse_generator_low_duration:
            unsigned(data_width-1 downto 0);
    signal pulse_int: std_logic;
            
    -- Universal counter.
    signal universal_counter_config: data_buffer;
    signal universal_counter_input: std_logic;
    signal universal_counter_value: unsigned(data_width-1 downto 0);
    signal universal_counter_state: data_buffer;
    signal update_universal_counter_output: std_logic;

    -- DAC controller.
    signal dac_channel_select_int: std_logic;
    signal dac_write_int: std_logic;
    subtype dac_channel_value is signed(dac_data_width-1 downto 0);
    type dac_channel_value_vector is array (natural range <>) of dac_channel_value;
    signal dac_channel_values: dac_channel_value_vector(1 downto 0);
    signal prepared_dac_value: unsigned(dac_data_width-1 downto 0);

    -- Peripheral (I/O) configuration.
    signal peripheral_config_raw: data_buffer;
    subtype dac_channel_source is unsigned(dac_source_selector_width-1 downto 0);
    type dac_channel_source_vector is array (natural range <>) of dac_channel_source;
    type peripheral_configuration_type is record
        dac_channel_sources: dac_channel_source_vector(0 to 1);
    end record;
    signal peripheral_configuration: peripheral_configuration_type :=
    (
        dac_channel_sources => (others => (others => '0'))
    );

begin

    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- Note: There seems to be a problem with the module firmware for channel number
    -- greater than 3: Transmitting a value to the SPI master (which always includes
    -- receiving a value from the SPI master) overwrites the received value.
    -- Thus only use transmitter or receiver for those channels.


    -- SPI receiver data (index 0 to 3 are also available via the FPGA panel).
    -----------------------------------------------------------------------------

    -- DDS generators.
    connect_dds_generators: for i in dds_generator_base_subaddr'range generate
        dds_generator_configs_raw(i) <= received_data_x(dds_generator_base_subaddr(i)+0);
        dds_generator_configs(i).generator.waveform <= dds_generator_configs_raw(i)(18 downto 16);
        dds_generator_configs(i).FM_range <= unsigned(dds_generator_configs_raw(i)(12 downto 8));
        dds_generator_configs(i).sync_source <= unsigned(dds_generator_configs_raw(i)(7 downto 6));
        dds_generator_configs(i).PM_source <= unsigned(dds_generator_configs_raw(i)(5 downto 4));
        dds_generator_configs(i).FM_source <= unsigned(dds_generator_configs_raw(i)(3 downto 2));
        dds_generator_configs(i).AM_source <= unsigned(dds_generator_configs_raw(i)(1 downto 0));
        dds_generator_phase_increments(i) <=
            unsigned(received_data_x(dds_generator_base_subaddr(i)+1));
        dds_generator_phase_shifts(i) <=
            unsigned(received_data_x(dds_generator_base_subaddr(i)+2)(15 downto 0)) & x"0000";
        dds_generator_levels(i) <=
            signed(received_data_x(dds_generator_base_subaddr(i)+2)(dds_generator_level_width+15 downto 16));
    end generate;

    -- Pulse generator.
    pulse_generator_high_duration <= unsigned(received_data_x(pulse_generator_high_duration_subaddr));
    pulse_generator_low_duration <= unsigned(received_data_x(pulse_generator_low_duration_subaddr));

    -- Universal counter.
    universal_counter_config <= received_data_x(universal_counter_config_subaddr);

    -- Peripheral configuration.
    peripheral_config_raw <= received_data_x(peripheral_configuration_subaddr);
    peripheral_configuration.dac_channel_sources(0) <= unsigned(peripheral_config_raw(dac_source_selector_width-1 downto 0));
    peripheral_configuration.dac_channel_sources(1) <= unsigned(peripheral_config_raw(2*dac_source_selector_width-1 downto dac_source_selector_width));

    -- Panel parameters.
--    dac_channel_1_value <= signed(received_data_x(1)(dac_data_width-1 downto 0));


    -- SPI transmitter data (index 0 to 3 are also available via the FPGA panel).
    -----------------------------------------------------------------------------

    -- Universal counter.
    transmit_data_x(universal_counter_state_subaddr) <= std_logic_vector(universal_counter_state);
    transmit_data_x(universal_counter_value_subaddr) <= std_logic_vector(universal_counter_value);

    -- Panel values.
--    transmit_data_x(0) <= universal_counter_config;
--    transmit_data_x(1) <= std_logic_vector(dds_generator_phase_increments(0));
--    transmit_data_x(2) <= std_logic_vector(dds_generator_phase_shifts(0));
--    transmit_data_x(3) <= std_logic_vector(dds_generator_phases(0));


    -- Miscellaneous.
    -----------------------------------------------------------------------------

    -- Generate DDS synchronization signals (high during 1st half period, i.e.
    -- the rising edge marks 0°).
    generate_sync: for i in 0 to number_of_dds_generators-1 generate
        dds_sync_int(i) <= not dds_generator_phases(i)(dds_generator_phase_width-1);
    end generate;
    
    -- Freeze counter output while transmitting counter state or value.
    update_universal_counter_output <= ready_x(4) and ready_x(5);
    

    --------------------------------------------------------------------------------
    -- Component instantiation.
    --------------------------------------------------------------------------------

    -- The clock manager generating the clocks used throughout the system.
    clock_manager: entity work.ClockManager
    port map
    (
        clk => sysclk,
        clk_50mhz => clk_50mhz,
        clk_100mhz => clk_100mhz
    );


    -- The slave of the SPI interface.
    slave: entity SPI_Interface.SPI_Slave
    generic map
    (
        address_width => address_width,
        synchronize_data_to_clk => true
    )
    port map
    (
        clk => clk_50mhz, 
        sclk => sclk, 
        ss_address => ssreg, 
        ss_data => ssdat,
        transmit_data_x => transmit_data_x,
        mosi => mosi,
        miso => miso_int,
        received_data_x => received_data_x,
        ready_x => ready_x
    );


    -- The universal counter for frequency and period measurements.
    universal_counter: entity work.UniversalCounter
    port map
    (
        clk => clk_50mhz,
        update_output => update_universal_counter_output,
        external_signal => universal_counter_input,
        measure_period => universal_counter_config(4),
        clk_division_mode => universal_counter_config(3 downto 0),
        value => universal_counter_value,
        overflow => universal_counter_state(1),
        ready => universal_counter_state(0)
    );


    -- The pulse generator producing a rectangular signal with specific pulse
    -- and pause durations.
    pulse_generator: entity work.PulseGenerator
    generic map
    (
        counter_width => pulse_generator_counter_width
    )
    port map
    (
        clk => clk_100mhz,
        high_duration => pulse_generator_high_duration,
        low_duration => pulse_generator_low_duration,
        pulse_signal => pulse_int
    );


    -- The modulated DDS signal generator producing multiple signals with
    -- miscellaneous waveforms.
    dds_generator: entity work.ModulatedGenerator
    generic map
    (
        number_of_generators => number_of_dds_generators,
        phase_width => dds_generator_phase_width,
        lookup_table_phase_width => dds_generator_lookup_table_phase_width,
        level_width => dds_generator_level_width,
        sample_width => dds_generator_sample_width
    )
    port map
    (
        clk => clk_100mhz,
        configs => dds_generator_configs,
        phase_increments => dds_generator_phase_increments,
        phase_shifts => dds_generator_phase_shifts,
        levels => dds_generator_levels,
        phases => dds_generator_phases,
        samples => dds_generator_samples
    );
    

    -- The DAC controller used to pass the signals to be D/A-converted to the DACs.
    dac_controller: entity work.DACController
    generic map
    (
        data_width => dac_data_width
    )
    port map
    (
        clk => clk_100mhz,
        channel_0_value => dac_channel_values(0),
        channel_1_value => dac_channel_values(1),
        dac_channel_select => dac_channel_select_int,
        dac_write => dac_write_int,
        dac_value => prepared_dac_value
    );


    --------------------------------------------------------------------------------
    -- Internal configuration logic.
    --------------------------------------------------------------------------------

    -- Connects the selected signals to the DACs´ inputs.
    connect_dac_input: process is
        constant dac_channel_source_pulse: integer := 4;
        variable dac_channel_source: integer;
    begin
        wait until rising_edge(clk_100mhz);
        for i in dac_channel_values'range loop
            dac_channel_source := to_integer(peripheral_configuration.dac_channel_sources(i));
            case (dac_channel_source) is 
                when dac_channel_source_pulse =>
                    -- Connect a positive signal corresponding to the pulse.
                    dac_channel_values(i) <= (others => pulse_int);
                    dac_channel_values(i)(dac_data_width-1) <= '0';
                when others =>
                    -- Connect the DDS signal generator specified.
                    dac_channel_values(i) <= dds_generator_samples(dac_channel_source);
            end case;
        end loop;
    end process;


    -- Connects the selected signals to the universal counter´s input.
    connect_counter_input: process is
        constant universal_counter_source_pulse: integer := 4;
        constant universal_counter_source_external: integer := 5;
        variable universal_counter_source: integer;
    begin
        wait until rising_edge(clk_100mhz);
        universal_counter_source := to_integer(unsigned(universal_counter_config(10 downto 8)));
        case (universal_counter_source) is 
            when universal_counter_source_pulse =>
                -- Connect the pulse signal.
                universal_counter_input <= pulse_int;
            when universal_counter_source_external =>
                -- Connect the external signal.
                universal_counter_input <= cntr_in;
            when others =>
                -- Connect the DDS signal generator specified.
                universal_counter_input <= dds_sync_int(universal_counter_source);
        end case;
    end process;


    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------

    -- Pulse and DDS generators.
    pulse_out <= pulse_int;
    dds_sync_out <= dds_sync_int;

    -- SPI & test LED.
    miso <= miso_int when ssdat = '0' else 'Z';
    test_led <= received_data_x(0)(0);

    -- SRAM.
    ram_we_n <= '1';
    ram_oe_n <= '1';

    -- Single and dual DAC. For the single DAC U2, we use dac_channel_select as the
    -- clock. Thus, U2 always uses the value of channel 0.
    dac_clk <= dac_channel_select_int; -- (single DAC U2 only)
    dac_channel_select <= dac_channel_select_int; -- (dual DAC U5 only)
    dac_write_n <= not dac_write_int; -- (dual DAC U5 only)
    -- Invert the DAC value to compensate the hardware´s inverter.
    invert_dac_value: for i in dac_value'range generate
        dac_value(i) <= not prepared_dac_value(i);
    end generate;

end architecture;
