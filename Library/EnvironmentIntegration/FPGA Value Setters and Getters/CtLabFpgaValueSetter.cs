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
using CtLab.FpgaSignalGenerator.Standard;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Sets Fpga values by sending according c't Lab set commands.
    /// </summary>
    public class CtLabFpgaValueSetter : IFpgaValueSetter
    {
        private SetCommandClass _setCommandClass;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClass">
        /// The command class representing the set commands sent to the c't Lab device.
        /// </param>
        public CtLabFpgaValueSetter(SetCommandClass setCommandClass)
        {
            _setCommandClass = setCommandClass;
        }

        /// <summary>
        /// Sets a signed integer value.
        /// </summary>
        public void SetValue(int value)
        {
            _setCommandClass.SetValue (value);
        }

        /// <summary>
        /// Sets an unsigned integer value.
        /// </summary>
        public void SetValue(uint value)
        {
            _setCommandClass.SetValue (value);
        }

        /// <summary>
        /// Sets a floating point value.
        /// </summary>
        public void SetValue(double value)
        {
            _setCommandClass.SetValue (value);
        }

        /// <summary>
        /// Sets a boolean value.
        /// </summary>
        public void SetValue(bool value)
        {
            _setCommandClass.SetValue (value);
        }
    }
}