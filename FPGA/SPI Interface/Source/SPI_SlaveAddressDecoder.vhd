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
-- Provides an SPI slave address decoder for addressing a data buffer used for
-- receiving or sending data.
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

entity SPI_SlaveAddressDecoder is
    generic
    (
        -- The width of the address.
        address_width: positive
    );
    port
    (
        -- The enable signal.
        buffer_enable: in std_logic; 
        -- The selected data buffer address.
        address: in unsigned(address_width-1 downto 0);
        -- The output buffer enable signal for each address.
        buffer_enable_x: out std_logic_vector((2**address_width)-1 downto 0) := (others => '0')
    );
end entity;

architecture stdarch of SPI_SlaveAddressDecoder is
begin

    --------------------------------------------------------------------------------
    -- Output logic.
    --------------------------------------------------------------------------------
    output_logic: process(buffer_enable, address) is
    begin
        buffer_enable_x <= (others => '1');
        if (buffer_enable = '0') then
            buffer_enable_x(to_integer(address)) <= '0';
        end if;
    end process;

end architecture;
