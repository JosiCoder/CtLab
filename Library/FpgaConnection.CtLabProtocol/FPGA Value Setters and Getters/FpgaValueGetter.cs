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
using CtLab.CtLabProtocol.Interfaces;
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaConnection.CtLabProtocol
{
    /// <summary>
    /// Gets FPGA values by evaluating according c't Lab messages.
    /// </summary>
    public class FpgaValueGetter : IFpgaValueGetter
    {
        /// <summary>
        /// Occurs when a value has been updated.
        /// </summary>
        public event EventHandler<EventArgs> ValueUpdated;

        private IMessageContainer<MessageChannel> _messageContainer;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageContainer">
        /// The message container used to get calues from the FPGA.
        /// </param>
        public FpgaValueGetter(IMessageContainer<MessageChannel> messageContainer)
        {
            _messageContainer = messageContainer;

            messageContainer.MessageUpdated += (sender, e) =>
            {
                var handler = ValueUpdated;
                if (handler != null)
                {
                    handler(sender, e);
                }
            };
        }

        /// <summary>
        /// Gets the value as an integer.
        /// </summary>
        public int ValueAsInt32
        {
            get
            {
                return _messageContainer.Message.ValueToInt32();
            }
        }

        /// <summary>
        /// Gets the value as an unsigned integer.
        /// </summary>
        public uint ValueAsUInt32
        {
            get
            {
                return _messageContainer.Message.ValueToUInt32();
            }
        }
    }
}