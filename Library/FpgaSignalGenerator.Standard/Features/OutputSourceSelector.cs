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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Communicates with the output source selection unit implemented within a c't Lab FPGA
    /// device configured as an FPGA Lab.
    /// </summary>
    public class OutputSourceSelector : IOutputSourceSelector
    {
        private OutputSourceWriter _outputSourceWriter;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="outputSourceSetter">
        /// The setter used to set the output signal source selection.
        /// </param>
        public OutputSourceSelector(ISubchannelValueSetter outputSourceSetter)
        {
            _outputSourceWriter = new OutputSourceWriter(outputSourceSetter);
        }

        /// <summary>
        /// Gets or sets the signal source for the first output (output 0).
        /// </summary>
        public OutputSource OutputSource0
        {
            get { return _outputSourceWriter.OutputSource0; }
            set { _outputSourceWriter.OutputSource0 = value; }
        }

        /// <summary>
        /// Gets or sets the signal source for the second output (output 1).
        /// </summary>
        public OutputSource OutputSource1
        {
            get { return _outputSourceWriter.OutputSource1; }
            set { _outputSourceWriter.OutputSource1 = value; }
        }
    }
}