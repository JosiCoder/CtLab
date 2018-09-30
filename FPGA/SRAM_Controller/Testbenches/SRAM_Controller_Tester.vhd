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
-- Tests the SRAM controller.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
 
entity SRAM_Controller_Tester is
end entity;

architecture stdarch of SRAM_Controller_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 10ns; -- 100MHz
    constant num_of_total_wait_states: natural := 9; -- 90ns @ 100MHz (min 70ns)
    constant num_of_write_pulse_wait_states: natural := 6; -- 60ns @ 100MHz (min 50ns)
    constant num_of_wait_states_before_write_after_read: natural := 4; -- 40ns @ 100MHz (min 30ns)
    constant data_width: natural := 8;
    constant address_width: natural := 16;
    constant start_address: natural := 16#40#;
    constant data_offset: natural := 16#10#;
    constant num_of_test_cycles: natural := 5;
    constant ram_access_time: time := 70ns;
    constant ram_output_disable_time: time := 30ns;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal read: std_logic;
    signal write: std_logic;
    signal auto_increment_address: std_logic := '0';
    signal address: unsigned(address_width-1 downto 0);
    signal data_in: std_logic_vector(data_width-1 downto 0);

    --------------------
    -- Outputs
    --------------------
    signal ready: std_logic;
    signal data_out: std_logic_vector(data_width-1 downto 0);
    signal ram_we_n: std_logic;
    signal ram_oe_n: std_logic;
    signal ram_address: unsigned(address_width-1 downto 0);

    --------------------
    -- Bidirectional
    --------------------
    signal ram_data: std_logic_vector(data_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;

begin

    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------
    uut: entity work.SRAM_Controller
    generic map
    (
        num_of_total_wait_states => num_of_total_wait_states,
        num_of_write_pulse_wait_states => num_of_write_pulse_wait_states,
        num_of_wait_states_before_write_after_read => num_of_wait_states_before_write_after_read,
        data_width => data_width,
        address_width => address_width
    )
    port map
    (
        clk => clk,
        read => read,
        write => write,
        ready => ready,
        auto_increment_address => auto_increment_address,
        address => address,
        data_in => data_in,
        data_out => data_out,
        ram_we_n => ram_we_n,
        ram_oe_n => ram_oe_n,
        ram_address => ram_address,
        ram_data => ram_data
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    -------------------------------------------------------------------------
    -- Reads from and writes to the SRAM, either in non-burst or burst mode.
    -- For non-burst mode, the read or write signal is active for just the
    -- first clock cycle of the read or write cycle (this is the minimum,
    -- longer durations are allowed).
    -- For burst mode, the read or write signal is active until the entire
    -- batch of data is transferred.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;

    -- Stimulates and controls the UUT and the tests at all.
    stimulus: process is
        variable burst_mode, sync_ready_to_clk: boolean;
    begin
    
        -- This configures the test bench whether it tests in single operation or
        -- burst mode.
        -- In single operation mode, we activate the read or write signal just for
        -- one clock cycle. The according operation is completed anyway. In burst
        -- mode, we keep the read or write signal active all the time, thus reading
        -- or writing continuously.
        -- Burst mode cannot be used when synchronizing the ready signal to the clock
        -- (see below).
        burst_mode := false;
        
        -- This configures the test bench whether it uses the ready signal asynchronously
        -- or syncs it to the clock.
        -- When used asynchronously, the next address and input data are applied
        -- immediately and thus are available to the SRAM controller on the next clock
        -- cycle.
        -- When synchronized to the clock, the next address and input data are applied
        -- with a latency of one clock cycle. This is one clock cycle too late for burst
        -- mode (see above).
        sync_ready_to_clk := false;

        wait for ram_output_disable_time; -- wait a little for stabilization
        wait until rising_edge(clk);
        
        -- Read from the SRAM several times.
        write <= '0';
        for adr in start_address to start_address + num_of_test_cycles - 1 loop
            address <= to_unsigned(adr, address_width);
            read <= '1';
            if (not burst_mode) then
                wait for clk_period;
                read <= '0';
            end if;
            wait on clk,ready until ready = '1';
            if (sync_ready_to_clk) then
                wait until rising_edge(clk);
            end if;
        end loop;

        -- Deactivate SRAM access.
        write <= '0';
        read <= '0';

        -- Write to the SRAM several times.
        read <= '0';
        for adr in start_address to start_address + num_of_test_cycles - 1 loop
            address <= to_unsigned(adr, address_width);
            data_in <= std_logic_vector(to_unsigned(adr - data_offset, data_width));
            write <= '1';
            if (not burst_mode) then
                wait for clk_period;
                write <= '0';
            end if;
            wait on clk,ready until ready = '1';
            if (sync_ready_to_clk) then
                wait until rising_edge(clk);
            end if;
        end loop;

        -- Deactivate SRAM access.
        write <= '0';
        read <= '0';
        
        wait for 5 * clk_period; -- wait a little to finish

        -- Stop the tests.
        run_test <= false;
        wait;
        
    end process;

    -- Simulates the external SRAM (worst timing conditions).
    sram: process is
    begin

        wait on ram_we_n, ram_oe_n, ram_address;
        if (ram_we_n = '1' and ram_oe_n = '0') then
            ram_data <= inertial std_logic_vector(ram_address(data_width-1 downto 0) - data_offset)
                        after ram_access_time;
        else
            ram_data <= inertial (others => 'Z') after ram_output_disable_time;
        end if;
    
    end process;

    --------------------------------------------------------------------------------
    -- Specifications.
    --------------------------------------------------------------------------------

    -- Verifies proper RAM signal generation and overall timing.
    must_create_correct_signals: process is
            variable expected_address: unsigned(address_width-1 downto 0);
            variable expected_data: unsigned(data_width-1 downto 0);
    begin

        -- Synchronize with the stimulus.
        wait until falling_edge(clk);

        -- Verify that the SRAM is deactivated completely at the beginning.
        assert (ram_we_n = '1') report "SRAM WE signal is not initially inactive." severity error;
        assert (ram_oe_n = '1') report "SRAM OE signal is not initially inactive." severity error;

        -- For each read access to the SRAM.
        for adr in start_address to start_address + num_of_test_cycles - 1 loop

            -- Wait until the current read cycle starts.
            if read /= '1' then
                wait until read = '1';
                wait until rising_edge(clk);
            end if;
        
            -- Verify that the SRAM controller generates the correct signals for the entire read cycle.
            for wait_state in 0 to num_of_total_wait_states - 1 loop
            
                -- Verify that the SRAM gets the correct address and signals during a read cycle.
                wait until falling_edge(clk);
                assert (ram_we_n = '1') report "SRAM WE signal is not inactive during a read cycle." severity error;
                assert (ram_oe_n = '0') report "SRAM OE signal is not active during a read cycle." severity error;
                expected_address := to_unsigned(adr, address_width);
                assert (ram_address = expected_address)
                    report "SRAM address is wrong during a read cycle, expected " & integer'image(to_integer(expected_address)) & "."
                    severity error;
                    
                -- Verify that the ready signal is active exactly at the end of the read cycle and that the data
                -- are available then.
                if (wait_state < num_of_total_wait_states - 1) then
                    assert (ready = '0') report "The ready signal is not inactive during a read cycle." severity error;
                else
                    assert (ready = '1') report "The ready signal is not active at the end of a read cycle." severity error;
                    assert (data_out = ram_data) report "The data are not available at the end of a read cycle." severity error;
                end if;
                
            end loop;

        end loop;

        -- Verify that the SRAM is deactivated completely after the read.
        wait for clk_period;
        assert (ram_we_n = '1') report "SRAM WE signal is not inactive after the read." severity error;
        assert (ram_oe_n = '1') report "SRAM OE signal is not inactive after the read." severity error;
        report "Read has finished" severity note;

        -- For each write access to the SRAM except the first one.
        -- The first cycle is skipped here because we have missed the according write signal edge.
        for adr in start_address + 1 to start_address + num_of_test_cycles - 1 loop
        
            -- Wait until the current write cycle starts.
            if write /= '1' then
                wait until write = '1';
                wait until rising_edge(clk);
            end if;
        
            -- Verify that the SRAM controller generates the correct signals for the entire write cycle.
            for wait_state in 0 to num_of_total_wait_states - 1 loop
            
                -- Verify that the SRAM gets the correct address, data and signals during a write cycle.
                wait until falling_edge(clk);
                if (wait_state < num_of_total_wait_states - 1 - num_of_write_pulse_wait_states
                    or wait_state = num_of_total_wait_states - 1) then
                    assert (ram_we_n = '1') report "SRAM WE signal is not inactive while preparing a write cycle." severity error;
                else
                    assert (ram_we_n = '0') report "SRAM WE signal is not active while executing a write cycle." severity error;
                end if;
                assert (ram_oe_n = '1') report "SRAM OE signal is not inactive during a write cycle." severity error;
                expected_address := to_unsigned(adr, address_width);
                assert (ram_address = expected_address)
                    report "SRAM address is wrong during a write cycle, expected " & integer'image(to_integer(expected_address)) & "."
                    severity error;
                expected_data := to_unsigned(adr - data_offset, data_width);
                assert (ram_data = std_logic_vector(expected_data))
                    report "SRAM data is wrong during a write cycle, expected " & integer'image(to_integer(expected_data)) & "."
                    severity error;
                    
                -- Verify that the ready signal is active exactly at the end of the write cycle.
                if (wait_state < num_of_total_wait_states - 1) then
                    assert (ready = '0') report "The ready signal is not inactive during a write cycle." severity error;
                else
                    assert (ready = '1') report "The ready signal is not active at the end of a write cycle." severity error;
                end if;
                
            end loop;

        end loop;

        -- Verify that the SRAM is decativated completely after the write.
        assert (ram_we_n = '1') report "SRAM WE signal is not inactive after the write." severity error;
        assert (ram_oe_n = '1') report "SRAM OE signal is not inactive after the write." severity error;
        report "Write has finished" severity note;

        wait;

    end process;

end architecture;
