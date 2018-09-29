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
-- Provides an SPI slave receiver consisting of an address decoder, an address
-- buffer, a receiver deserializer and several receiver buffers each having its
-- own enable signal. The data width is fixed (see data_width constant value in
-- the Globals package).
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.globals.all;

-- Note: It's not possible to use generics for both the data width and the number
-- of buffers to be generated. This would need a signal that is an array of
-- unconstrained arrays which is not yet supported by VHDL. Thus, the data width
-- is fixed (see Globals package).
entity SPI_SlaveReceiver is
    generic
    (
        -- The width of the address.
        address_width: positive
    );
    port
    (
        -- The system clock.
        clk: in std_logic; 
        -- Controls when the received data are passed to the output (triggered
        -- on the rising edge, synchronous to CLK).
        buffer_enable: in std_logic; 
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The (active low) address slave select.
        ss_address: in std_logic; 
        -- The serial input.
        mosi: in std_logic; 
        -- The parallel output providing the address received most recently
        -- (this might be used by the transmitter).
        address: out unsigned(address_width-1 downto 0) := (others => '0');
        -- The parallel outputs providing the data received.
        data_x: out data_buffer_vector((2**address_width)-1 downto 0);
        -- Indicates for each buffer whether the received data are stable.
        ready_x: out std_logic_vector((2**address_width)-1 downto 0)
    );
end entity;

architecture stdarch of SPI_SlaveReceiver is
    constant number_of_data_buffers: positive := 2**address_width;
    signal address_int: unsigned(address_width-1 downto 0) := (others => '0');
    signal receiver_data: data_buffer;
    -- Signals for n data buffers.
    signal buffer_enable_x_int: std_logic_vector(number_of_data_buffers-1 downto 0);
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------


    -- Internal connections.
    address <= address_int;


    -- The address decoder.
    address_decoder: entity work.SPI_SlaveAddressDecoder
    generic map
    (
        address_width => address_width
    )
    port map
    (
        buffer_enable => buffer_enable, 
        address => address_int, 
        buffer_enable_x => buffer_enable_x_int
    );


    -- The shared slave receiver deserializer.
    deserializer: entity work.SPI_SlaveReceiverDeserializer
    generic map
    (
        width => data_width
    )
    port map
    (
        sclk => sclk, 
        mosi => mosi, 
        data => receiver_data
    );


    -- The output data buffer (sensitive to the rising edge of the enable signal;
    -- synchronous to clk).
    data_buffers: for i in 0 to number_of_data_buffers-1 generate

        data_buffer: entity work.SPI_SlaveDataBuffer
        generic map
        (
            width => data_width,
            edge_triggered => true
        )
        port map
        (
            clk => clk,
            buffer_enable => buffer_enable_x_int(i),
            data => receiver_data, 
            buffered_data => data_x(i),
            ready => ready_x(i)
        );

    end generate;


    --------------------------------------------------------------------------------
    -- Address register.
    --------------------------------------------------------------------------------
    address_register: process is
    begin
        wait until rising_edge(ss_address);
        address_int <= unsigned(receiver_data(address_width-1 downto 0));
    end process;

end architecture;
