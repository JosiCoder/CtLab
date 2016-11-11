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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Writes the signal source selection for the outputs by setting the value of a set
    /// command that can be sent to a c't Lab FPGA device configured as an FPGA Lab.
    /// </summary>
    public class OutputSourceWriter : SubchannelWriterBase
    {
        private OutputSources _outputSource0;
        private OutputSources _outputSource1;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="subchannelValueSetter">
		/// The setter used to set the subchannel's value.
        /// </param>
        public OutputSourceWriter(ISubchannelValueSetter subchannelValueSetter)
            : base (subchannelValueSetter)
        {
        }

        /// <summary>
        /// Gets or sets the signal source for the first output (output 0).
        /// </summary>
        public OutputSources OutputSource0
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
        public OutputSources OutputSource1
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
            _subchannelValueSetter.SetValue(combinedValue);
        }
    }
}