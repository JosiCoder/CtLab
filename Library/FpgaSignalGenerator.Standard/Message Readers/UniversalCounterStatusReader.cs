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

using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Reads the status of a universal counter by getting the according value of
    /// an FPGA device configured as an FPGA Lab.
    /// </summary>
    public class UniversalCounterStatusReader
    {
        private readonly IFpgaValueGetter _valueGetter;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueGetter">
        /// The setter used to get the FPGA's value.
        /// </param>
        public UniversalCounterStatusReader(IFpgaValueGetter valueGetter)
        {
            _valueGetter = valueGetter;
        }

        /// <summary>
        /// Gets a value indicating whether an overflow has occurred.
        /// </summary>
        public bool Overflow
        {
            get { return (Value & 0x00000002) != 0; }
        }

        /// <summary>
		/// Gets a value indicating whether the counter's input signal is active.
        /// </summary>
        public bool InputSignalActive
        {
            get { return (Value & 0x00000001) != 0; }
        }

        private uint Value
        {
            get { return _valueGetter.ValueAsUInt32; }
        }
    }
}