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
using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Interfaces
{
    /// <summary>
    /// Represents an empty message for a certain message channel.
    /// </summary>
    public class EmptyMessage : IMessage
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">The channel the message belongs to.</param>
        public EmptyMessage (IMessageChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets a value indicating whether this message is empty.
        /// </summary>
        public bool IsEmpty
        { get { return true; } }

        /// <summary>
        /// Determines whether the value of the message is equal to that of
        /// the specified message.
        /// </summary>
        /// <param name="otherMessage">The message to compare with.</param>
        /// <returns>A value indicating whether the message values are equal.</returns>
        public bool ValueEquals (IMessage otherMessage)
        {
            return false;
        }

        /// <summary>
        /// Gets the channel the message belongs to.
        /// </summary>
        public IMessageChannel Channel
        { get; private set; }

        /// <summary>
        /// Converts the message value to an integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        public int ValueToInt32()
        {
            return default(int);
        }

        /// <summary>
        /// Converts the message value to an unsigned integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        public uint ValueToUInt32()
        {
            return default(uint);
        }
    }
}