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
-- Manages the clocks used throughout the system.
--------------------------------------------------------------------------------
library ieee;
use ieee.std_logic_1164.all;
use ieee.numeric_std.all;

Library UNISIM;
use UNISIM.vcomponents.all;

entity ClockManager is
    port
    (
        -- The system clock.
        clk: in std_logic;
        -- The 50 MHz clock.
        clk_50mhz: out std_logic;
        -- The 100 MHz clock.
        clk_100mhz: out std_logic
    );
end entity;

architecture stdarch of ClockManager is
    signal clk50_unbuf, clk50_buf: std_logic;
    signal clk100_unbuf, clk100_buf: std_logic;
begin


    --------------------------------------------------------------------------------
    -- Connections to and from internal signals.
    --------------------------------------------------------------------------------

    clk_50mhz <= clk;
    clk_100mhz <= clk100_buf;


    --------------------------------------------------------------------------------
    -- Instantiate components.
    --------------------------------------------------------------------------------

    dcm_instance: DCM
    generic map
    (
        CLKIN_PERIOD => 20.0,
        -- DLL attributes
        CLK_FEEDBACK => "1X",
        DLL_FREQUENCY_MODE => "LOW",
        CLKIN_DIVIDE_BY_2 => FALSE,
        CLKDV_DIVIDE => 2.0,
        DUTY_CYCLE_CORRECTION => TRUE,
        -- DFS attributes
        DFS_FREQUENCY_MODE => "LOW",
        CLKFX_MULTIPLY => 2,
        CLKFX_DIVIDE => 1
    )
    port map
    (
        RST => '0',
        CLKIN => clk,
        CLKFB => clk50_buf,
        PSEN => '0',
        PSCLK => '0',
        PSINCDEC => '0',
        CLK0 => clk50_unbuf,
        CLK90 => open,
        CLK180 => open,
        CLK270 => open,
        CLK2X => open,
        CLK2X180 => open,
        CLKDV => open,
        CLKFX => clk100_unbuf,
        CLKFX180 => open,
        LOCKED => open,
        STATUS => open,
        PSDONE => open
    );

    clk50_bufg: BUFG
    port map
    (
        I => clk50_unbuf,
        O => clk50_buf
    );

    clk100_bufg: BUFG
    port map
    (
        I => clk100_unbuf,
        O => clk100_buf
    );

end architecture;
