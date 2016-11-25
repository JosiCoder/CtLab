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
-- Generates all control signals for the DACs, multiplexes the values for the
-- dual DAC and adds an offset to the DAC value to make it purely positive.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity DACController is
    generic
    (
        -- The width of the DAC values.
        data_width: natural
    );
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The value for DAC channel 0.
        channel_0_value : in signed(data_width-1 downto 0);
        -- The value for DAC channel 1.
        channel_1_value : in signed(data_width-1 downto 0);
        -- The DAC´s channel selection signal.
        dac_channel_select: out std_logic;
        -- The DAC´s write signal.
        dac_write: out std_logic;
        -- The currently selected DAC value with an offset added.
        dac_value : out unsigned(data_width-1 downto 0)
    );
end entity;

architecture stdarch of DACController is
    type reg_type is record
        dac_channel_select, dac_write: std_logic;
        dac_value : unsigned(data_width-1 downto 0);
    end record;
    signal state, next_state: reg_type :=
    (
        dac_channel_select => '0',
        dac_write => '0',
        dac_value => (others => '0')
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
    next_state_logic: process(state, channel_0_value, channel_1_value) is
        variable next_dac_value: signed(data_width-1 downto 0);
    begin
    
        -- Defaults.
        next_state <= state;

        -- Switch to the next channel when the write signal gets deactivated.
        if (state.dac_write = '1') then
        
            -- Switch to the next channel and get this channel´s value.
            if state.dac_channel_select = '0' then
                next_state.dac_channel_select <= '1';
                next_dac_value := channel_1_value;
            else
                next_state.dac_channel_select <= '0';
                next_dac_value := channel_0_value;
            end if;

            -- Toggle the sign bit, i.e. convert the signed value to an unsigned value
            -- with an offset.
            next_dac_value(data_width-1) := not next_dac_value(data_width-1);
            next_state.dac_value <= unsigned(next_dac_value);

        end if;

        -- Toggle the write signal.
        next_state.dac_write <= not state.dac_write;
        
    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    dac_channel_select <= state.dac_channel_select;
    dac_write <= state.dac_write;
    dac_value <= state.dac_value;

end architecture;
