//------------------------------------------------------------------------------
// Copyright (C) 2016 Josi Coder

// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.

// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.

// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Provides c't Lab command menmonics along with the according subchannel numbers.
    /// </summary>
    public class CommandMnemonics
    {
        // Generic
        public const ushort GEN_VAL = 0;
        public const ushort GEN_WEN = 250;
        public const ushort GEN_ERC = 251;
        public const ushort GEN_SBD = 252;
        public const ushort GEN_IDN = 254;
        public const ushort GEN_STR = 255;

        // FPGA
        public const ushort FPGA_VAL = 0;
        public const ushort FPGA_DSP = 80;
        public const ushort FPGA_HEX = 88;
        public const ushort FPGA_OPT = 150;
        public const ushort FPGA_CLK = 90;
        public const ushort FPGA_CFG = 240;
        public const ushort FPGA_LST = 241;
        public const ushort FPGA_DIR = FPGA_LST;
        public const ushort FPGA_FNM = 242;
        public const ushort FPGA_AIR = 248;
        // da fehlen noch welche
    }

}
