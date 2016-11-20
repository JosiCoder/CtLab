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
-- Provides an SPI slave receiver deserializer operating in SPI mode 3. This means
-- that the incoming serial data are sampled at the trailing (rising) SCLK edges.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity SPI_SlaveReceiverDeserializer is
    generic
    (
        -- The width of the data.
        width: positive
    );
    port
    (
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The serial input.
        mosi: in std_logic; 
        -- The parallel output providing the data received.
        data: out std_logic_vector(width-1 downto 0) := (others => '0')
    );
end entity;

architecture stdarch of SPI_SlaveReceiverDeserializer is
    type reg_type is record
        data: std_logic_vector(width-1 downto 0);
    end record;
    signal state, next_state: reg_type :=
    (
        data => (others => '0')
    );
begin

    --------------------------------------------------------------------------------
    -- State register.
    --------------------------------------------------------------------------------
    state_register: process is
    begin
        wait until rising_edge(sclk);
        state <= next_state;
    end process;
    
    
    --------------------------------------------------------------------------------
    -- Next state logic.
    --------------------------------------------------------------------------------
    next_state_logic: process(state, mosi) is
    begin
        -- Defaults.
        next_state <= state;
        
        next_state.data <= state.data(width-2 downto 0) & mosi;
    end process;
    

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    data <= state.data;

end architecture;
