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
use ieee.math_real.all;

entity SRAM_Controller is
    generic
    (
        -- The number of clock cycles to wait before each SRAM access.
        num_of_total_wait_states: natural;
        -- The number of clock cycles the write pulse must be active.
        num_of_write_pulse_wait_states: natural;
        -- The number of clock cycles to wait before writing after reading (to
        -- provide the SRAM's data bus enough time to get high impedance).
        num_of_wait_states_before_write_after_read: natural;
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
        auto_increment_address: in std_logic;
        auto_increment_end_address_reached: out std_logic;
        -- The address and data towards the client.
        -- For auto_increment_address = '1' the address passed here is the end address.
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
    -- This generates enough bits for maximum number of wait states - 1.
    constant wait_states_counter_width : natural := integer(ceil(log2(real(num_of_total_wait_states + num_of_wait_states_before_write_after_read))));
    type reg_type is record
        wait_states_counter: unsigned(wait_states_counter_width-1 downto 0);
        reading: std_logic;
        writing: std_logic;
        auto_increment_end_address_reached: std_logic;
        last_access_cycle_was_a_write: std_logic;
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
        auto_increment_end_address_reached => '0',
        last_access_cycle_was_a_write => '0',
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
    next_state_logic: process(state, read, write, auto_increment_address, address, data_in, ram_data) is
        variable do_read, do_write: std_logic;
        variable last_wait_state, first_drive_data_to_ram_wait_state, first_write_pulse_wait_state: natural;
    begin
    
        -- Defaults.
        next_state <= state;
        next_state.wait_states_counter <= (others => '0');
        next_state.ram_we_n <= '1';
        next_state.ram_oe_n <= '1';
        next_state.drive_data_to_ram <= '0';
        
        if (auto_increment_address = '0') then
            next_state.auto_increment_end_address_reached <= '0';
        end if;
        
        -- Detect read or write mode.
        do_read := '0';
        do_write := '0';

        -- Enter read or write mode or continue the current mode until it is
        -- finished during the last clock cycle of the access cycle.
        if (state.reading = '1') then
            do_read := '1';
        elsif (state.writing = '1') then
            do_write := '1';
        else
            -- Check whether to enter read or write mode.
            if (read = '1' and write = '0') then
                do_read := '1';
                next_state.reading <= '1';
            elsif (read = '0' and write = '1') then
                do_write := '1';
                next_state.writing <= '1';
            end if;
        end if;

        -- Determine the timing necessary (we have to wait a little before writing
        -- immediately after reading).
        first_drive_data_to_ram_wait_state := 0;
        last_wait_state := num_of_total_wait_states - 1;
        if (do_write = '1' and state.last_access_cycle_was_a_write = '0') then
            first_drive_data_to_ram_wait_state := first_drive_data_to_ram_wait_state + num_of_wait_states_before_write_after_read;
            last_wait_state := last_wait_state + num_of_wait_states_before_write_after_read;
        end if;
        first_write_pulse_wait_state := last_wait_state - num_of_write_pulse_wait_states;

        -- Forward the address and data to the SRAM at the beginning of the access cycle
        -- and the data from the SRAM at the end of the access cycle.
        if (do_read = '1' or do_write = '1') then

            -- For read access only.
            if (do_read = '1') then
                next_state.ram_oe_n <= '0';
                next_state.last_access_cycle_was_a_write <= '0';
            end if;

            -- For write access only.
            if (do_write = '1') then
                -- Take the data at the beginning of the access cycle.
                if (state.wait_states_counter = to_unsigned(0, wait_states_counter_width)) then
                    next_state.ram_data <= data_in;
                end if;
                if (state.wait_states_counter >= to_unsigned(first_drive_data_to_ram_wait_state, wait_states_counter_width)) then
                    next_state.drive_data_to_ram <= '1';
                end if;
                -- For all clock cycles during which the write happens.
                if (state.wait_states_counter >= to_unsigned(first_write_pulse_wait_state, wait_states_counter_width)
                    and state.wait_states_counter < to_unsigned(last_wait_state, wait_states_counter_width)) then
                    next_state.ram_we_n <= '0';
                end if;            
            end if;

            -- For all clock cycles of the access cycle except the last one.
            if (state.wait_states_counter /= to_unsigned(last_wait_state, wait_states_counter_width)) then
                next_state.wait_states_counter <= state.wait_states_counter + 1;
                -- At the beginning of the access cycle, determine the address to access.
                if (state.wait_states_counter = to_unsigned(0, wait_states_counter_width)) then
                    if (auto_increment_address = '0') then
                        -- Auto-increment mode is not used, just use the the specified address.
                        next_state.ram_address <= address;
                    elsif (state.ram_address < address) then
                        -- Auto-increment mode is used and we have not reached the specified (end) address,
                        -- increment the address.
                        -- Note that the new address is accessed immediately in the next cycle if the read
                        -- or write signal is still active.
                        next_state.ram_address <= state.ram_address + 1;
                    else
                      -- Auto-increment mode is used but we have reached the specified (end) address,
                      -- keep the address from the previous cycle and set the end flag.
                      next_state.auto_increment_end_address_reached <= '1';
                    end if;
                end if;
            -- For the last clock cycle of the access cycle.
            else
                -- Memorize if this cycle was a write, so the next write can follow immediately.
                if (state.writing = '1') then
                    next_state.last_access_cycle_was_a_write <= '1';
                end if;

                -- Take the data at the end of the access cycle, reset the reading/writing state.
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
    auto_increment_end_address_reached <= state.auto_increment_end_address_reached;
    data_out <= state.data_out;
    ram_we_n <= state.ram_we_n;
    ram_oe_n <= state.ram_oe_n;
    ram_address <= state.ram_address;
    ram_data <= state.ram_data when state.drive_data_to_ram = '1' else (others => 'Z');

end architecture;
