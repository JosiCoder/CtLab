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

namespace CtLab.SubchannelAccess
{
    /// <summary>
    /// Provides the base functionality for classes that read a single (non-combined) value from
    /// a message that may have been received from a c't Lab device.
    /// </summary>
    public abstract class SingleValueSubchannelReaderBase<TValue> : SubchannelReaderBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageContainer">
        /// The container holding the message.
        /// </param>
        protected SingleValueSubchannelReaderBase(IMessageContainer messageContainer)
            : base(messageContainer)
        {
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public abstract TValue Value{ get; }
    }

    /// <summary>
    /// Reads a single (non-combined) value from a message that may have been received from a
    /// c't Lab device.
    /// </summary>
    public class UInt32ValueSubchannelReader : SingleValueSubchannelReaderBase<uint>
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageContainer">
        /// The container holding the message.
        /// </param>
        public UInt32ValueSubchannelReader(IMessageContainer messageContainer)
            : base(messageContainer)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public override uint Value
        {
            get { return _messageContainer.Message.ValueToUInt32(); }
        }
    }
}