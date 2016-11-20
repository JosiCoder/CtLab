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
-- Provides an SPI slave transmitter consisting of an input selector, a single
-- transmitter buffer, and a transmitter serializer. The data width is fixed (see
-- data_width constant value in the Globals package).
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.globals.all;

-- Note: It´s not possible to use generics for both the data width and the number
-- of buffers to be generated. This would need a signal that is an array of
-- unconstrained arrays which is not yet supported by VHDL. Thus, the data width
-- is fixed (see Globals package).
entity SPI_SlaveTransmitter is
    generic
    (
        -- The width of the address.
        address_width: positive
    );
    port
    (
        -- The system clock.
        clk: in std_logic; 
        -- Controls when the data to be transmitted are read (input is passed
        -- when enable is '1', synchronous to CLK).
        buffer_enable: in std_logic; 
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The (active low) slave select.
        ss: in std_logic; 
        -- The selected address to read the data to be transmitted from.
        address: in unsigned(address_width-1 downto 0);
        -- The parallel inputs used to get the data to be sent from.
        data_x: in data_buffer_vector((2**address_width)-1 downto 0);
        -- The serial output.
        miso: out std_logic
    );
end entity;

architecture stdarch of SPI_SlaveTransmitter is
    constant number_of_data_buffers: positive := 2**address_width;
    signal selected_data, transmitter_data: data_buffer;
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    -- The input data buffer (transparent if the enable signal is '1'; synchronous
    -- to clk).
    data_buffer: entity work.SPI_SlaveDataBuffer
    generic map
    (
        width => data_width,
        edge_triggered => false
    )
    port map
    (
        clk => clk,
        buffer_enable => buffer_enable,
        data => selected_data, 
        buffered_data => transmitter_data,
        ready => open
    );


    -- The slave transmitter serializer.
    serializer: entity work.SPI_SlaveTransmitterSerializer
    generic map
    (
        width => data_width
    )
    port map
    (
		sclk => sclk, 
		ss => ss, 
		data => transmitter_data, 
		miso => miso
    );


    --------------------------------------------------------------------------------
    -- Input selection logic.
    --------------------------------------------------------------------------------

    -- The data buffer.
    input_selector: process(address, data_x) is
    begin
        selected_data <= (others => '1');
        for i in number_of_data_buffers-1 downto 0 loop
            if (address = to_unsigned(i, address_width)) then
                selected_data <= data_x(i);
            end if;
        end loop;
    end process;

end architecture;
