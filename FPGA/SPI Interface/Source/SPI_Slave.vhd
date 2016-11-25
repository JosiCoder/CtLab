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
-- Provides an SPI slave receiver consisting of a receiver, a transmitter, and an
-- optional synchronizer. The parallel output and input data are synchronous to
-- the system clock (CLK) so that they can be read from or written to by internal
-- components controlled by that clock. Without synchronization the system clock
-- (CLK) and the SPI shift clock (SCLK) must be related to each other and satisfy
-- specific timing requirements between each other. With synchronization there is
-- a delay of three rising system (CLK) clock edges before received data are
-- available at the parallel output. There must also be three rising system clock
-- (CLK) edges between the indication of a transmission (by SS_DATA becoming active)
-- and the beginning of that transmission (the first leading SCLK edge).
----------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
library Common;
use work.globals.all;

entity SPI_Slave is
    generic
    (
        -- The width of the address.
        address_width: positive;
        -- Indicates whether the data in- and outputs are to be synchronized to CLK.
        synchronize_data_to_clk: boolean := true
    );
    port
    (
        -- The system clock.
        clk: in std_logic; 
        -- The clock controlling the serial data transmission.
        sclk: in std_logic; 
        -- The (active low) address slave select.
        ss_address: in std_logic; 
        -- The (active low) data slave select.
        ss_data: in std_logic; 
        -- The parallel inputs used to get the data to be sent from.
        transmit_data_x: in data_buffer_vector((2**address_width)-1 downto 0);
        -- The serial input.
        mosi: in std_logic; 
        -- The serial output.
        miso: out std_logic; 
        -- The parallel output for each buffer providing the data received.
        received_data_x: out data_buffer_vector((2**address_width)-1 downto 0);
        -- Indicates for each buffer whether the received data are stable.
        ready_x: out std_logic_vector((2**address_width)-1 downto 0)
    );
end entity;

architecture stdarch of SPI_Slave is
    signal buffer_enable: std_logic;
    signal address: unsigned(address_width-1 downto 0);
begin

    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    -- The optional synchronizer or shortcuts replacing them.
    generate_synchronizer: if synchronize_data_to_clk generate
        -- Synchronizes the data slave select signal to the system clock (CLK).
        ss_data_synchronizer: entity Common.Synchronizer
        port map
        (
            clk => clk,
            in_async => ss_data,
            out_sync => buffer_enable
        );
    end generate;
    dont_generate_synchronizer: if not synchronize_data_to_clk generate
        -- Don't synchronizes the data slave select signal.
        buffer_enable <= ss_data;
    end generate;


    -- The receiver (holding n data buffers and an additional address buffer).
    receiver: entity work.SPI_SlaveReceiver
    generic map
    (
        address_width => address_width
    )
    port map
    (
        clk => clk, 
        buffer_enable => buffer_enable,
        sclk => sclk, 
        ss_address => ss_address, 
        address => address, 
        mosi => mosi, 
        data_x => received_data_x,
        ready_x => ready_x
    );


    -- The transmitter.
    transmitter: entity work.SPI_SlaveTransmitter
    generic map
    (
        address_width => address_width
    )
    port map
    (
        clk => clk, 
        buffer_enable => buffer_enable,
        sclk => sclk, 
        ss => ss_data, 
        address => address,
        data_x => transmit_data_x,
        miso => miso
    );

end architecture;
