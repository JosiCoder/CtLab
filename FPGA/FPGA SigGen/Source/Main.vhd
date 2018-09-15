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
library SRAM_Controller;
use SPI_Interface.globals.all;
use work.FunctionGenerator_Declarations.all;
use work.ModulatedGenerator_Declarations.all;

entity Main is
    generic
    (
        -- The width of the shared SRAM address and DAC data.
        ram_addr_dac_data_width: natural := 19;
        -- The width of the SRAM address and data.
        ram_address_width: natural := 19;
        ram_data_width: natural := 8;
        -- The width of the DAC data.
        dac_data_width: natural := modulated_generator_sample_width;
        -- The number of DDS generators.
        number_of_dds_generators: natural := 4
    );
    port
    (
        -- The system clock.
        sysclk: in std_logic; 
        -- The internal SPI interface.
        f_sck: in std_logic; -- 
        f_rs: in std_logic; -- low during transmission
        f_ds: in std_logic; -- low during transmission
        f_mosi: in std_logic; 
        f_miso: out std_logic; 
        -- The external SPI interface.
        ext_sck: in std_logic; -- 
        ext_rs: in std_logic; -- low during transmission
        ext_ds: in std_logic; -- low during transmission
        ext_mosi: in std_logic; 
        ext_miso: out std_logic; 
        -- The test LED output.
        test_led: out std_logic;
        -- The universal counter input.
        cntr_in: in std_logic;
        -- The DDS generators synchronization outputs.
        dds_sync_out: out std_logic_vector(number_of_dds_generators-1 downto 0);
        -- The pulse generator output.
        pulse_out: out std_logic;
        -- The SRAM data and control signals.
        ram_we_n: out std_logic;
        ram_oe_n: out std_logic;
        -- The shared output for SRAM address and DAC data.
        ram_addr_dac_data: out unsigned(ram_addr_dac_data_width-1 downto 0);
        ram_data: inout std_logic_vector(ram_data_width-1 downto 0);
        -- The DAC control signals.
        dac_clk: out std_logic;
        dac_channel_select: out std_logic;
        dac_write_n: out std_logic
    );
end entity;

architecture stdarch of Main is

    -- Configuration constants
    -----------------------------------------------------------------------------
    
    -- SPI interface.
    constant use_internal_spi: boolean := true;
    constant use_external_spi: boolean := false;
    constant address_width: positive := 5; -- max. 8 (for addresses 0..255)
    constant number_of_data_buffers: positive := 2**address_width;
    -- Signal generators and counter.
    constant dds_generator_lookup_table_phase_width: natural := 12;
    constant pulse_generator_counter_width: natural := 32;
    constant dds_generator_phase_width: natural := modulated_generator_phase_width;
    constant dds_generator_level_width: natural := modulated_generator_level_width;
    constant dds_generator_sample_width: natural := dac_data_width;
    constant dac_source_selector_width: natural := 4;
    -- SRAM controller.
    constant num_of_total_wait_states: natural := 9; -- 90ns @ 100MHz (min 70ns)
    constant num_of_write_pulse_wait_states: natural := 6; -- 60ns @ 100MHz (min 50ns)
    constant num_of_wait_states_before_write_after_read: natural := 4; -- 40ns @ 100MHz (min 30ns)

    -- SPI sub-address constants
    -----------------------------------------------------------------------------
    
    type integer_vector is array (natural range <>) of integer;
    -- Receiver.
    constant dds_generator_base_subaddr: integer_vector(0 to number_of_dds_generators-1)
        := (0=>8, 1=>12, 2=>16, 3=>20); -- need n..n+2 each
    constant peripheral_configuration_subaddr: integer := 7;
    constant universal_counter_config_subaddr: integer := 1;
    constant pulse_generator_high_duration_subaddr: integer := 6;
    constant pulse_generator_low_duration_subaddr: integer := 5;
    -- Transmitter.
    constant universal_counter_state_subaddr: integer := 2;
    constant universal_counter_value_subaddr: integer := 3;
    -- Bidirectional.
    constant sram_subaddr: integer := 24;

    -- Signals
    -----------------------------------------------------------------------------

    -- Clocks
    signal clk_50mhz: std_logic;
    signal clk_100mhz: std_logic;

    -- SPI interfaces
    type spi_in_type is record
        mosi: std_logic;
        sclk: std_logic;
        ss_address: std_logic;
        ss_data: std_logic;
    end record;
    signal selected_spi_in, internal_spi_in, external_spi_in, inactive_spi_in: spi_in_type :=
    (
        -- Initialize to proper idle values.
        mosi => '0',
        sclk => '1',
        ss_address => '1',
        ss_data => '1'
    );
    signal miso: std_logic;

    -- DDS signal generators
    signal dds_generator_configs_raw: data_buffer_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_configs: modulated_generator_config_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_phase_increments: modulated_generator_phase_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_phase_shifts: modulated_generator_phase_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_levels: modulated_generator_level_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_phases: modulated_generator_phase_vector(number_of_dds_generators-1 downto 0);
    signal dds_generator_samples: modulated_generator_sample_vector(number_of_dds_generators-1 downto 0);
    signal dds_sync_int: std_logic_vector(number_of_dds_generators-1 downto 0);

    -- Pulse generator
    signal pulse_generator_high_duration, pulse_generator_low_duration:
            unsigned(data_width-1 downto 0);
    signal pulse_int: std_logic;
            
    -- Universal counter
    signal universal_counter_config: data_buffer;
    signal universal_counter_input: std_logic;
    signal universal_counter_value: unsigned(data_width-1 downto 0);
    signal universal_counter_state: data_buffer;
    signal update_universal_counter_output: std_logic;

    -- DAC
    signal dac_channel_select_int: std_logic;
    signal dac_write_int: std_logic;
    subtype dac_channel_value is signed(dac_data_width-1 downto 0);
    type dac_channel_value_vector is array (natural range <>) of dac_channel_value;
    signal dac_channel_values: dac_channel_value_vector(1 downto 0);
    signal dac_value: unsigned(dac_data_width-1 downto 0);
    signal dac_data: std_logic_vector(dac_data_width-1 downto 0);
    
    -- Memory controller
    signal memory_read: std_logic;
    signal memory_write: std_logic;
    signal memory_ready: std_logic;
    signal memory_auto_increment_address: std_logic;
    signal memory_address: unsigned(ram_address_width-1 downto 0);
    signal memory_data_in: std_logic_vector(ram_data_width-1 downto 0);
    signal memory_data_out: std_logic_vector(ram_data_width-1 downto 0);
    signal internal_ram_we_n: std_logic;
    signal internal_ram_oe_n: std_logic;
    signal internal_ram_address: unsigned(ram_address_width-1 downto 0);

    -- Peripheral (I/O) configuration
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

    -- Interconnection
    signal transmit_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0)
            := (others => (others => '0'));
    signal received_data_x: data_buffer_vector(number_of_data_buffers-1 downto 0);
    signal ready_x: std_logic_vector(number_of_data_buffers-1 downto 0);

begin

    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    -- NOTE: Reading to and writing from an SPI address always happen together. Each time
    -- the SPI master reads a value from the slave's transmit register, it also writes a value
    -- to the slave's receive register of the same address, overwriting any previous value.
    --
    -- If the internal SPI connection is used, the microcontroller of the c'Lab FPGA board
    -- acts as the SPI master. It accesses a particular SPI adress as follows:
    -- 1) If one of the Param or Value screens is selected on the panel, the microcontroller
    --    accesses the SPI bus periodically to read the value from and write the parameter to
    --    the according SPI address.
    -- 2) When processing a c't Lab protocol set command, the microcontroller writes the 
    --    according parameter to the SPI slave and ignores the value read from the SPI slave.
    -- 3) When processing a c't Lab protocol query command, the microcontroller writes an
    --    arbitrary parameter to the SPI slave and returns the value read from the SPI slave.
    --    It happens to be that the parameter sent most recently to the same or any other SPI
    --    address is reused as this arbitrary parameter.
    --
    -- If the external SPI connection is used, it's up to the external SPI master how to handle
    -- values read from the SPI slave and how to generate parameters written to the SPI slave.


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

    -- Memory controller
    -- Combination of mode (3 bits; (MSB unused) + (0=off, 1=read, 2=write)), address and write data.
    memory_write <= received_data_x(sram_subaddr)(data_buffer'high-1); -- bit to the right of MSB
    memory_read <= received_data_x(sram_subaddr)(data_buffer'high-2); -- bit to the right of memory_write
    memory_address <= unsigned(received_data_x(sram_subaddr)(ram_address_width-1+ram_data_width downto ram_data_width));
    memory_data_in <= received_data_x(sram_subaddr)(ram_data_width-1 downto 0);

    -- Panel parameters (example only).
--    dac_channel_1_value <= signed(received_data_x(1)(dac_data_width-1 downto 0));


    -- SPI transmitter data (index 0 to 3 are also available via the FPGA panel).
    -----------------------------------------------------------------------------

    -- Universal counter.
    transmit_data_x(universal_counter_state_subaddr) <= std_logic_vector(universal_counter_state);
    transmit_data_x(universal_counter_value_subaddr) <= std_logic_vector(universal_counter_value);

    -- Memory controller
    -- Combination of state (3 bits; (0=working, 4=ready) + (0=off, 1=reading, 2=writing)), address and read data.
    transmit_data_x(sram_subaddr) <= memory_ready
                                   & received_data_x(sram_subaddr)(data_buffer'high-1 downto ram_data_width)
                                   & memory_data_out;

    -- Panel values.
    transmit_data_x(0) <= X"DEADBEEF";
--    transmit_data_x(0) <= universal_counter_config;
--    transmit_data_x(1) <= std_logic_vector(dds_generator_phase_increments(0));
--    transmit_data_x(2) <= std_logic_vector(dds_generator_phase_shifts(0));
--    transmit_data_x(3) <= std_logic_vector(dds_generator_phases(0));


    -- Miscellaneous.
    -----------------------------------------------------------------------------

    -- Generate DDS synchronization signals (high during 1st half period, i.e.
    -- the rising edge marks 0).
    generate_sync: for i in 0 to number_of_dds_generators-1 generate
        dds_sync_int(i) <= not dds_generator_phases(i)(dds_generator_phase_width-1);
    end generate;
    
    -- Freeze counter output while transmitting counter state or value.
    update_universal_counter_output <= ready_x(4) and ready_x(5);
    
    -- Provide the inverted DAC value to compensate the hardware inverter.
    invert_dac_value: for i in dac_data'range generate
        dac_data(i) <= not dac_value(i);
    end generate;

    -- Deactivate the memory controller's automatic address increment.
    memory_auto_increment_address <= '0';


    --------------------------------------------------------------------------------
    -- SPI input selection logic.
    --------------------------------------------------------------------------------

    -- The internal SPI bus (i.e. the one connected to the microcontroller of the
    -- c'Lab FPGA board).
    internal_spi_in.mosi <= f_mosi;
    internal_spi_in.sclk <= f_sck;
    internal_spi_in.ss_address <= f_rs;
    internal_spi_in.ss_data <= f_ds;

    -- The external SPI bus (i.e. the one connected to the expansion ports of the
    -- c'Lab FPGA board).
    external_spi_in.mosi <= ext_mosi;
    external_spi_in.sclk <= ext_sck;
    external_spi_in.ss_address <= ext_rs;
    external_spi_in.ss_data <= ext_ds;

    -- Select the SPI connection to use.
    -- NOTE: If one of the Param or Value screens is selected on the panel, the microcontroller
    -- of the c'Lab FPGA board accesses the SPI bus periodically to read the value from and write
    -- the parameter to the according SPI address (SPI reading and writing always happen together).
    -- Thus, when both connections are activated, while using the *external* connection, set the
    -- panel to the file selection screen to avoid this interference.
    -- Also, when both connections are activated, while using the *internal* connection, ensure
    -- that the selection pins of the external connection (ext_rs and ext_ds) are pulled up properly.
    -- If they are e.g. connected to the SPI interface of a Raspberry Pi, ensure that the latter is
    -- switched on. Don't leave the pins unconnected, pull them up instead.
    selected_spi_in <=
        internal_spi_in when use_internal_spi and
                             (internal_spi_in.ss_address = '0' or internal_spi_in.ss_data = '0') else
        external_spi_in when use_external_spi and
                             (external_spi_in.ss_address = '0' or external_spi_in.ss_data = '0') else
        inactive_spi_in;


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
        sclk => selected_spi_in.sclk, 
        ss_address => selected_spi_in.ss_address, 
        ss_data => selected_spi_in.ss_data,
        transmit_data_x => transmit_data_x,
        mosi => selected_spi_in.mosi,
        miso => miso,
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
        dac_value => dac_value
    );


    -- The SRAM controller.
    sram: entity SRAM_Controller.SRAM_Controller
    generic map
    (
        num_of_total_wait_states => num_of_total_wait_states,
        num_of_write_pulse_wait_states => num_of_write_pulse_wait_states,
        num_of_wait_states_before_write_after_read => num_of_wait_states_before_write_after_read,
        data_width => ram_data_width,
        address_width => ram_address_width
    )
    port map
    (
        clk => clk_100mhz,
        read => memory_read,
        write => memory_write,
        ready => memory_ready,
        auto_increment_address => memory_auto_increment_address,
        address => memory_address,
        data_in => memory_data_in,
        data_out => memory_data_out,
        ram_we_n => internal_ram_we_n,
        ram_oe_n => internal_ram_oe_n,
        ram_address => internal_ram_address,
        ram_data => ram_data
    );


    --------------------------------------------------------------------------------
    -- Internal configuration logic.
    --------------------------------------------------------------------------------

    -- Connects the selected signals to the DACs inputs.
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


    -- Connects the selected signals to the universal counters input.
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
    f_miso <= miso when f_ds = '0' else 'Z';
    ext_miso <= miso when ext_ds = '0' else 'Z';
    test_led <= received_data_x(0)(0); -- LED is active low

    -- SRAM address or DAC data (share the same pins and thus can only be used
    -- alternatively).
    -----------------------------------------------------------------------------
    
    -- Apply SRAM address.
    ----------------------
    
    ram_addr_dac_data <= internal_ram_address;
    dac_clk <= '1';
    dac_channel_select <= '1';
    dac_write_n <= '1';
    ram_we_n <= internal_ram_we_n;
    ram_oe_n <= internal_ram_oe_n;

    -- Apply DAC data.
    ------------------
    
--    -- Apply data in reverse bit order, padded on the left.
--    generate_dac_data: for i in 0 to dac_data_width-1 generate
--        ram_addr_dac_data(i) <= dac_data(dac_data_width-1-i);
--    end generate;
--    ram_addr_dac_data(ram_addr_dac_data_width-1 downto dac_data_width) <= (others => '0');
--    
--    -- Apply control signals for single or dual DAC. For the single DAC U2, we use
--    -- dac_channel_select as the clock. Thus, U2 always uses the value of channel 0.
--    dac_clk <= dac_channel_select_int; -- (single DAC U2 only)
--    dac_channel_select <= dac_channel_select_int; -- (dual DAC U5 only)
--    dac_write_n <= not dac_write_int; -- (dual DAC U5 only)
--    ram_we_n <= '1';
--    ram_oe_n <= '1';

end architecture;
