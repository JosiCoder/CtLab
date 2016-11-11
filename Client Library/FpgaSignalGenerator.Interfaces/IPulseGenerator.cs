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

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Provides facilities to communicate with a pulse generator implemented within a c't Lab FPGA
    /// device configured as an FPGA Lab.
    /// </summary>
    public interface IPulseGenerator
    {
        /// <summary>
        /// Gets or sets the pulse duration in steps of 10 ns.
        /// </summary>
        uint PulseDuration { get; set; }

        /// <summary>
        /// Gets or sets the pause duration in steps of 10 ns.
        /// </summary>
        uint PauseDuration { get; set; }
    }
}