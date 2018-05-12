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
-- Generates all control signals and provides flow control for accessing the SRAM.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity SRAM_Controller is
    generic
    (
        -- The number of clock cycles to wait before each SRAM access.
        num_of_total_wait_states: natural;
        -- The number of clock cycles the write pulse must be active.
        num_of_write_pulse_wait_states: natural;
        -- The number of bits used for the wait states counter.
        wait_states_counter_width: natural;
        -- The width of the data stored in the memory.
        data_width: natural;
        -- The width of the address bus of the memory.
        address_width: natural
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The control signals towards the client.
        read: in std_logic; 
        write: in std_logic;
        ready: out std_logic;
        -- The address and data towards the client.
        address: in unsigned(address_width-1 downto 0);
        data_in: in std_logic_vector(data_width-1 downto 0);
        data_out: out std_logic_vector(data_width-1 downto 0);
        -- The control signals towards the SRAM (write enable and output enable, both active low).
        ram_we_n: out std_logic;
        ram_oe_n: out std_logic;
        -- The address and data towards the SRAM.
        ram_address: out unsigned(address_width-1 downto 0);
        ram_data: inout std_logic_vector(data_width-1 downto 0)
    );
end entity;

architecture stdarch of SRAM_Controller is
    type reg_type is record
        wait_states_counter: unsigned(wait_states_counter_width-1 downto 0);
        reading: std_logic;
        writing: std_logic;
        data_out: std_logic_vector(data_width-1 downto 0);
        ram_we_n: std_logic;
        ram_oe_n: std_logic;
        ram_address: unsigned(address_width-1 downto 0);
        ram_data: std_logic_vector(data_width-1 downto 0);
        drive_data_to_ram: std_logic;
    end record;
    signal state, next_state: reg_type :=
    (
        wait_states_counter => (others => '0'),
        reading => '0',
        writing => '0',
        data_out => (others => '0'),
        ram_we_n => '1',
        ram_oe_n => '1',
        ram_address => (others => '0'),
        ram_data => (others => '0'),
        drive_data_to_ram => '0'
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
    next_state_logic: process(state, read, write, address, data_in, ram_data) is
        variable do_read, do_write: std_logic;
    begin
    
        -- Defaults.
        next_state <= state;
        next_state.wait_states_counter <= (others => '0');
        next_state.ram_we_n <= '1';
        next_state.ram_oe_n <= '1';
        next_state.drive_data_to_ram <= '0';

        -- Detect read or write mode.
        do_read := '0';
        do_write := '0';

        -- Check whether to enter read or write mode.
        if (read = '1' and write = '0') then
            do_read := '1';
            next_state.reading <= '1';
        elsif (read = '0' and write = '1') then
            do_write := '1';
            next_state.writing <= '1';
        end if;

        -- Continue read or write mode until the last clock cycle of the access cycle.
        if (state.reading = '1') then
            do_read := '1';
        elsif (state.writing = '1')then
            do_write := '1';
        end if;

        -- Forward the address and data to the SRAM at the beginning of the access cycle
        -- and the data from the SRAM at the end of the access cycle.
        if (do_read = '1' or do_write = '1') then

            -- For read access only.
            if (do_read = '1') then
                next_state.ram_oe_n <= '0';
            end if;

            -- For write access only.
            if (do_write = '1') then
                -- Take the data at the beginning of the access cycle.
                if (state.wait_states_counter = to_unsigned(0, wait_states_counter_width)) then
                    next_state.ram_data <= data_in;
                end if;
                next_state.drive_data_to_ram <= '1';
                -- For all clock cycles during which the write happens.
                if (state.wait_states_counter >= to_unsigned(num_of_total_wait_states - 1 - num_of_write_pulse_wait_states, wait_states_counter_width)
                    and state.wait_states_counter < to_unsigned(num_of_total_wait_states - 1, wait_states_counter_width)) then
                    next_state.ram_we_n <= '0';
                end if;            
            end if;

            -- For all clock cycles of the access cycle except the last one.
            if (state.wait_states_counter /= to_unsigned(num_of_total_wait_states - 1, wait_states_counter_width)) then
                next_state.wait_states_counter <= state.wait_states_counter + 1;
                -- Take the address at the beginning of the access cycle.
                if (state.wait_states_counter = to_unsigned(0, wait_states_counter_width)) then
                    next_state.ram_address <= address;
                end if;
            -- For the last clock cycle of the access cycle.
            else
                -- Take the data at the end of the access cycle, set the ready flag.
                -- (For write access, the input data are mirrored back as output data.)
                next_state.data_out <= ram_data;
                next_state.reading <= '0';
                next_state.writing <= '0';
                next_state.wait_states_counter <= (others => '0');
            end if;
        end if;

    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------

    ready <= not state.reading and not state.writing;
    data_out <= state.data_out;
    ram_we_n <= state.ram_we_n;
    ram_oe_n <= state.ram_oe_n;
    ram_address <= state.ram_address;
    ram_data <= state.ram_data when state.drive_data_to_ram = '1' else (others => 'Z');

end architecture;
