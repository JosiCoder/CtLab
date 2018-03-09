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
-- Tests the SPI slave address decoder.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;
use work.TestTools.all;
 
entity SPI_SlaveAddressDecoder_Tester is
end entity;
 
architecture stdarch of SPI_SlaveAddressDecoder_Tester is
 
    -- Constants
    constant test_delay: time := 1ps;
    constant address_width: positive := 3;
    constant no_of_addresses: positive := 2**address_width;
    
    -- Inputs
    signal buffer_enable: std_logic := '1';
    signal address: unsigned(address_width-1 downto 0) := (others => '0');

    -- Outputs
    signal buffer_enable_x: std_logic_vector(no_of_addresses-1 downto 0);

begin

    --------------------------------------------------------------------------------
    -- Instantiate the UUT(s).
    --------------------------------------------------------------------------------
    uut: entity work.SPI_SlaveAddressDecoder
    generic map
    (
        address_width => address_width
    )
    port map
    (
        buffer_enable => buffer_enable, 
        address => address, 
        buffer_enable_x => buffer_enable_x
    );
    

    --------------------------------------------------------------------------------
    -- Stimulate the UUT.
    --------------------------------------------------------------------------------
    stimulus: process is
        variable expected_ss_x: std_logic_vector(buffer_enable_x'range);
    begin
    
        -- Wait for the UUT's initial output values to settle down and check them.
        wait for test_delay;
        assert (buffer_enable_x = (buffer_enable_x'range => '1'))
            report "At least one buffer_enable_x unintentionally active."
            severity error;

        -- Enable the slave select.
        buffer_enable <= '0';
        
        -- Now provide consecutive addresses and check whether the buffer_enable_x signals
        -- match them.
        for i in 0 to no_of_addresses-1 loop
            address <= to_unsigned(i, address_width);
            wait for test_delay;
            expected_ss_x := (buffer_enable_x'range => '1');
            expected_ss_x(to_integer(address)) := '0';
            assert (buffer_enable_x = expected_ss_x)
                report "One or more buffer_enable_x not set correctly."
                severity error;
        end loop;

        -- Disable the slave select again and check whether all buffer_enable_x signals
        -- are deactivated.
        buffer_enable <= '1';
        wait for test_delay;
        assert (buffer_enable_x = (buffer_enable_x'range => '1'))
            report "At least one buffer_enable_x not deactivated."
            severity error;

        wait;
        
    end process;

end architecture;
