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

namespace CtLab.CommandsAndMessages.Interfaces
{
    /// <summary>
	/// Provides facilities to set the value of a c't Lab device's subchannel.
    /// </summary>
    public interface ISubchannelValueSetter
    {
        /// <summary>
        /// Converts the command value from a signed integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        void SetValue(int value);

        /// <summary>
        /// Converts the command value from an unsigned integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        void SetValue(uint value);

        /// <summary>
        /// Converts the command value from a floating point number.
        /// </summary>
        /// <returns>The converted value.</returns>
        void SetValue(double value);

        /// <summary>
        /// Converts the command value from a boolean value.
        /// </summary>
        /// <returns>The converted value.</returns>
        void SetValue(bool value);
    }
}