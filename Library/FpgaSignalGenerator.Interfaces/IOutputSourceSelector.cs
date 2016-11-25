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
    /// Provides facilities to communicate with a source selection unit implemented within a c't Lab
    /// FPGA device configured as an FPGA Lab.
    /// </summary>
    public interface IOutputSourceSelector
    {
        /// <summary>
        /// Gets or sets the signal source for the first output (output 0).
        /// </summary>
        OutputSources OutputSource0 { get; set; }

        /// <summary>
        /// Gets or sets the signal source for the second output (output 1).
        /// </summary>
        OutputSources OutputSource1 { get; set; }
    }
}