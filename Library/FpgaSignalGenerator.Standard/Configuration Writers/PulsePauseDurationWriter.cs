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
    /// Writes the pulse or pause duration of a pulse generator by setting the according
    /// value of an FPGA device configured as an FPGA Lab.
    /// </summary>
    public class PulsePauseDurationWriter
    {
        private readonly IFpgaValueSetter _valueSetter;
        private uint _value;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueSetter">
        /// The setter used to set the FPGA's value.
        /// </param>
        public PulsePauseDurationWriter(IFpgaValueSetter valueSetter)
        {
            _valueSetter = valueSetter;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public uint Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetCommandValue();
            }
        }

        private void SetCommandValue()
        {
            _valueSetter.SetValue(_value);
        }
    }
}