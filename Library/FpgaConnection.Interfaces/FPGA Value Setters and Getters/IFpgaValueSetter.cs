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

namespace CtLab.FpgaConnection.Interfaces
{
    /// <summary>
	/// Provides facilities to set a value within the FPGA.
    /// </summary>
    public interface IFpgaValueSetter
    {
        /// <summary>
        /// Sets a signed integer value.
        /// </summary>
        void SetValue(int value);

        /// <summary>
        /// Sets an unsigned integer value.
        /// </summary>
        void SetValue(uint value);

        /// <summary>
        /// Sets a floating point value.
        /// </summary>
        void SetValue(double value);

        /// <summary>
        /// Sets a boolean value.
        /// </summary>
        void SetValue(bool value);
    }
}