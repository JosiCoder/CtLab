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

using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Writes the signal source selection for the outputs by setting the according value of
    /// an FPGA device configured as an FPGA Lab.
    /// </summary>
    public class OutputSourceWriter
    {
        private readonly IFpgaValueSetter _valueSetter;
        private OutputSource _outputSource0;
        private OutputSource _outputSource1;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueSetter">
        /// The setter used to set the FPGA's value.
        /// </param>
        public OutputSourceWriter(IFpgaValueSetter valueSetter)
        {
            _valueSetter = valueSetter;
        }

        /// <summary>
        /// Gets or sets the signal source for the first output (output 0).
        /// </summary>
        public OutputSource OutputSource0
        {
            get { return _outputSource0;  }
            set
            {
                _outputSource0 = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the signal source for the second output (output 1).
        /// </summary>
        public OutputSource OutputSource1
        {
            get { return _outputSource1; }
            set
            {
                _outputSource1 = value;
                SetCommandValue();
            }
        }

        private void SetCommandValue()
        {
            var combinedValue = ((uint) _outputSource1) << 4 | ((uint) _outputSource0);
            _valueSetter.SetValue(combinedValue);
        }
    }
}