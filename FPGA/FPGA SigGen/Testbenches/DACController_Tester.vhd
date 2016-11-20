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
-- Tests the DAC controller.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.ALL;
use ieee.numeric_std.all;
 
entity DACController_Tester is
end entity;

architecture stdarch of DACController_Tester is
 
    --------------------
    -- Constants
    --------------------
    constant clk_period: time := 10ns;
    constant test_width: natural := 8;
    constant test_no_of_channels: integer := 2;
    constant test_delay: time := 1ns;
    constant test_value_0_A: integer := 10;
    constant test_value_1_A: integer := -10;
    constant test_value_0_B: integer := 20;
    constant test_value_1_B: integer := -20;

    --------------------
    -- Inputs
    --------------------
    signal clk: std_logic := '0';
    signal channel_0_value : signed(test_width-1 downto 0) := (others => '0');
	signal channel_1_value : signed(test_width-1 downto 0) := (others => '0');

    --------------------
    -- Outputs
    --------------------
    signal dac_channel_select: std_logic;
    signal dac_write: std_logic;
    signal dac_value: unsigned(test_width-1 downto 0);

    --------------------
    -- Internals
    --------------------
    signal run_test: boolean := true;


    -------------------------------------------------------------------------
    -- Lets the DAC controller cycle through all channels and checks whether
    -- they´re handled correctly.
    -------------------------------------------------------------------------
    procedure test_all_adc_channels(channel_0_value: integer;
                                    channel_1_value: integer)
    is
        variable expected_value: integer;
		variable expected_value_with_offset: unsigned(test_width-1 downto 0);
        variable current_channel: integer := -1;
        variable channel_0_used, channel_1_used: boolean := false;
    begin

        -- We want to start with the clock edge deactivating the write signal.
        wait until dac_write = '1';
        
        -- Let the DAC controller cycle through all channels and see whether
        -- they´re handled correctly. Two clock cycles are necessary for each
        -- channel.
        for i in 0 to (2*test_no_of_channels)-1 loop
        
            -- Wait for the next rising edge and until output signals have settled down.
            wait until rising_edge(clk);
            wait for test_delay;
            
            if (dac_write = '0') then
                -- We expect a switchover to the next channel when the write signal
                -- gets deactivated.
                
                if (dac_channel_select = '0') then
                    channel_0_used := true;
                    current_channel := 0;
                    expected_value := channel_0_value;
                elsif (dac_channel_select = '1') then
                    channel_1_used := true;
                    current_channel := 1;
                    expected_value := channel_1_value;
                end if;

                -- We want to get the expected value converted to a positive value
                -- with an offset.
                expected_value_with_offset :=
                    unsigned(to_signed(expected_value,test_width));
                expected_value_with_offset(test_width-1) :=
                    not expected_value_with_offset(test_width-1);

                assert (dac_value = expected_value_with_offset)
                    report "Value for channel " & integer'image(current_channel) &
                           " not passed to the output."
                    severity error;

            else
                -- We expect that channel selection and output value remain
                -- unchanged when the write signal gets activated.

                assert ((dac_channel_select = '0' and current_channel = 0) or
                        (dac_channel_select = '1' and current_channel = 1))
                    report "Selected channel changed unexpectedly."
                    severity error;
            
                assert (dac_value = expected_value_with_offset)
                    report "Value for channel " & integer'image(current_channel) &
                           " changed unexpectedly."
                    severity error;
            
            end if;
                
        end loop;

        assert (channel_0_used and channel_1_used)
            report "Not all DAC channels have been served." severity error;

    end procedure;
    
begin

    --------------------------------------------------------------------------------
    -- UUT instantiation.
    --------------------------------------------------------------------------------

    uut: entity work.DACController
    generic map
    (
        data_width => test_width
    )
    port map
    (
        clk => clk,
		channel_0_value => channel_0_value,
		channel_1_value => channel_1_value,
        dac_channel_select => dac_channel_select,
        dac_write => dac_write,
        dac_value => dac_value
    );


    --------------------------------------------------------------------------------
    -- UUT stimulation.
    --------------------------------------------------------------------------------

    -- Generates the system clock.
    clk <= not clk after clk_period/2 when run_test;

    -- Stimulates and controls the UUT and the tests at all.
    stimulus: process is
    begin

        -- Prepare the first set of DAC values.
        channel_0_value <= to_signed(test_value_0_A, test_width);
        channel_1_value <= to_signed(test_value_1_A, test_width);

        -- Let the DAC controller cycle through all channels several times.
        for i in 0 to 2 loop
            test_all_adc_channels(test_value_0_A, test_value_1_A);
        end loop;

        -- Perform a further test with the second set of DAC values.
        channel_0_value <= to_signed(test_value_0_B, test_width);
        channel_1_value <= to_signed(test_value_1_B, test_width);
        test_all_adc_channels(test_value_0_B, test_value_1_B);

        run_test <= false;
        wait;
        
    end process;
    
end architecture;
