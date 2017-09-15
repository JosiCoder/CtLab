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
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaConnection.CtLabProtocol
{
    /// <summary>
    /// Sets FPGA values by sending according c't Lab set commands.
    /// </summary>
    public class FpgaValueSetter : IFpgaValueSetter
    {
        private ISubchannelValueSetter _valueSetter;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueSetter">
        /// The value setter used to set values within the FPGA.
        /// </param>
        public FpgaValueSetter(ISubchannelValueSetter valueSetter)
        {
            _valueSetter = valueSetter;
        }

        /// <summary>
        /// Sets a signed integer value.
        /// </summary>
        public void SetValue(int value)
        {
            _valueSetter.SetValue (value);
        }

        /// <summary>
        /// Sets an unsigned integer value.
        /// </summary>
        public void SetValue(uint value)
        {
            _valueSetter.SetValue (value);
        }
    }
}