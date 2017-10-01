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

namespace CtLab.Messages.Standard.Specs
{
    /// <summary>
    /// Specifies a message for testing purposes. This can also be used as an example
    /// for developing own messages.
    /// </summary>
    public class SpecsMessage : IMessage
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">The channel the message belongs to.</param>
        /// <param name="testValue">The test message value.</param>
        public SpecsMessage (IMessageChannel channel, string testValue)
        {
            Channel = channel;
            TestValue = testValue;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="testValue">The test message value.</param>
        public SpecsMessage (string testValue)
        {
            TestValue = testValue;
        }

        /// <summary>
        /// Gets a value indicating whether this message is empty.
        /// </summary>
        public bool IsEmpty
        { get { return false; } }

        /// <summary>
        /// Determines whether the value of the message is equal to that of
        /// the specified message.
        /// </summary>
        /// <param name="otherMessage">The message to compare with.</param>
        /// <returns>A value indicating whether the message values are equal.</returns>
        public bool ValueEquals (IMessage otherMessage)
        {
            var other = otherMessage as SpecsMessage;
            return other != null && TestValue == other.TestValue;
        }

        /// <summary>
        /// Gets the channel the message belongs to.
        /// </summary>
        public IMessageChannel Channel
        { get; private set; }

        /// <summary>
        /// The raw (unconverted) message value.
        /// </summary>
        public string TestValue;

        /// <summary>
        /// Converts the message value to an integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        public int ValueToInt32()
        {
            return int.Parse(TestValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the message value to an unsigned integer.
        /// </summary>
        /// <returns>The converted value.</returns>
        public uint ValueToUInt32()
        {
            return uint.Parse(TestValue, CultureInfo.InvariantCulture);
        }
    }
}