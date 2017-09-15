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
using System.Globalization;

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Represents a message received from a certain message channel.
    /// </summary>
    /// <typeparam name="TMessageChannel">The type of the message channel.</typeparam>
    public class Message<TMessageChannel>
    {
        /// <summary>
        /// Determines whether the value of the message is equal to that of
        /// the specified message.
        /// </summary>
        /// <param name="otherMessage">The message to compare with.</param>
        /// <returns>A value indicating whether the message values are equal.</returns>
        public bool ValueEquals (Message<TMessageChannel> otherMessage)
        {
            return RawValue == otherMessage.RawValue;
        }

        /// <summary>
        /// The channel the message comes from.
        /// </summary>
        public TMessageChannel Channel;

        /// <summary>
        /// The raw (unconverted) message value.
        /// </summary>
        public string RawValue;

        /// <summary>
        /// The message description. This might be empty.
        /// </summary>
        public string Description;

        /// <summary>
        /// Converts the message value to an integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        public int ValueToInt32()
        {
            return int.Parse(RawValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the message value to an unsigned integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        public uint ValueToUInt32()
        {
            return uint.Parse(RawValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the message value to a floating point number.
        /// </summary>
        /// <returns>The converted value.</returns>
        public double ValueToDouble()
        {
            return double.Parse(RawValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the message value to a boolean value.
        /// </summary>
        /// <returns>The converted value.</returns>
        public bool ValueToBoolean()
        {
            if (RawValue == null)
                throw new ArgumentNullException();
            if (RawValue == "0")
                return false;
            if (RawValue == "1")
                return true;
            throw new FormatException();
        }
    }
}