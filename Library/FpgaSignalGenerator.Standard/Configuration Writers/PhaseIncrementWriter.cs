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

using CtLab.CommandsAndMessages.Interfaces;
using CtLab.SubchannelAccess;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Writes the phase increment of a DDS generator by setting the value of a set command
    /// that can be sent to a c't Lab FPGA device configured as an FPGA Lab.
    /// </summary>
    public class PhaseIncrementWriter : UInt32ValueSubchannelWriter
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="subchannelValueSetter">
		/// The setter used to set the subchannel's value.
        /// </param>
        public PhaseIncrementWriter(ISubchannelValueSetter subchannelValueSetter)
            : base(subchannelValueSetter)
        {
        }
    }
}