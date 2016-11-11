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

using System;

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Provides data for the ValueChanged event.
    /// </summary>
    public class ValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="message">
		/// The counter's value in Hertz for frequency measurements or in seconds for period measurements.
        /// </param>
        public ValueChangedEventArgs(double value)
        {
            Value = value;
        }

        /// <summary>
		/// The counter's value in Hertz for frequency measurements or in seconds for period measurements.
        /// </summary>
        public double Value;
    }
}