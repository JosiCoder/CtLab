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

namespace CtLab.CtLabProtocol.Interfaces
{
    /// <summary>
    /// Specifies a channel for c't lab messages (messages based on the text protocol
    /// used througout the entire c't lab).
    /// </summary>
    #pragma warning disable 0660 // operator == or != defined without Equals() override
    public class MessageChannel : MessageChannelBase<MessageChannel>
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="main">The mainchannel.</param>
        /// <param name="sub">The subchannel.</param>
        public MessageChannel(byte main, ushort sub)
            : base((i1, i2) => CompareContents((MessageChannel)i1, (MessageChannel)i2))
        {
            Main = main;
            Sub = sub;
        }

        /// <summary>
        /// Gets the mainchannel.
        /// </summary>
        public byte Main
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the subchannel.
        /// </summary>
        public ushort Sub
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether contents the specified objects are equal.
        /// </summary>
        /// <param name="item1">The first object to compare.</param>
        /// <param name="item2">The second object to compare.</param>
        /// <returns>
        /// A value indicating whether contents the specified objects are equal.
        /// </returns>
        private static bool CompareContents(MessageChannel item1, MessageChannel item2)
        {
            return
                item1.Main == item2.Main &&
                item1.Sub == item2.Sub;
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            // Concatenate Main and Sub (fits into an int).
            return sizeof (ushort) * Main + Sub;
        }

        public static bool operator ==(MessageChannel item1, MessageChannel item2)
        {
            return CompareItems(item1, item2,
                (i1, i2) => CompareContents((MessageChannel)i1, (MessageChannel)i2));
        }

        public static bool operator !=(MessageChannel item1, MessageChannel item2)
        {
            return !(item1 == item2);
        }
    }
    #pragma warning restore 0660
}