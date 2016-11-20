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
-- Provides an SPI slave transmitter serializer operating in SPI 3. This means
-- that the outgoing serial data are shifted at the leading (falling) SCLK edges
-- and thus can be sampled at the trailing (rising) edge by the SPI master´s
-- receiver.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity SPI_SlaveTransmitterSerializer is
    generic
    (
        -- The width of the data.
        width: positive
    );
    port
    (
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The slave select (low during transmission).
        ss: in std_logic; 
        -- The parallel input used to get the data to be sent from.
        data: in std_logic_vector(width-1 downto 0) := (others => '0');
        -- The serial output.
        miso: out std_logic
    );
end entity;

architecture stdarch of SPI_SlaveTransmitterSerializer is
    type reg_type is record
        ss: std_logic;
        data: std_logic_vector(width-1 downto 0);
    end record;
    signal state, next_state: reg_type :=
    (
        ss => '1',
        data => (others => '0')
    );
begin

    --------------------------------------------------------------------------------
    -- State register.
    --------------------------------------------------------------------------------
    state_register: process(sclk, ss) is
    begin
        if ss = '1' then
            -- We must reset asynchronously here because SS changes while SCLK is
            -- stopped (SCLK is only active when transferring data).
            state <=
            (
                ss => '1',
                data => (others => '0')
            );
        elsif falling_edge(sclk) then
            state <= next_state;
        end if;
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Next state logic.
    --------------------------------------------------------------------------------
    next_state_logic: process(state, ss, data) is
    begin
        -- Defaults.
        next_state <= state;

        -- For SS edge detection.
        next_state.ss <= ss;

        -- Load serializer on first falling SCLK edge since SS has become active,
        -- this also provides the first bit (MSB) at the serial output. Serialize
        -- on all following falling SCLK edges providing the remaining bits.
        if (state.ss = '1' and ss = '0') then
            next_state.data <= data;
        else
            next_state.data <= state.data(width-2 downto 0) & '0';
        end if;
    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    miso <= state.data(width-1);

end architecture;
