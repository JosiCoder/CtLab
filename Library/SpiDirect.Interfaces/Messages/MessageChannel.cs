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
using CtLab.Messages.Interfaces;

namespace CtLab.SpiDirect.Interfaces
{
    /// <summary>
    /// Specifies a channel for SPI messages (messages passed via the SPI interface).
    /// </summary>
    #pragma warning disable 0660 // operator == or != defined without Equals() override
    public class MessageChannel : MessageChannelBase<MessageChannel>
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="id">The SPI address.</param>
        public MessageChannel(byte spiAddress)
        {
            SpiAddress = spiAddress;
        }

        /// <summary>
        /// Gets the SPI address.
        /// </summary>
        public byte SpiAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether contents the specified object are equal to contents of the
        /// current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// A value indicating whether contents the specified object are equal to contents
        /// of the current object.
        /// </returns>
        protected override bool CompareContents(MessageChannel other)
        {
            return this.SpiAddress == other.SpiAddress;
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return SpiAddress;
        }

        public static bool operator ==(MessageChannel item1, MessageChannel item2)
        {
            return
                CompareReferences(item1, item2) &&
                item1.CompareContents(item2);
        }

        public static bool operator !=(MessageChannel item1, MessageChannel item2)
        {
            return !(item1 == item2);
        }
    }
    #pragma warning restore 0660
}