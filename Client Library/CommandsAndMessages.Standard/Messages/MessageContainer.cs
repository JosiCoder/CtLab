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
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Holds and returns a message that may have been received from a c't Lab device.
    /// </summary>
    public class MessageContainer : IMessageContainer
    {
        private Message _message;

        /// <summary>
        /// Occurs when the message has been updated.
        /// </summary>
        public event EventHandler<EventArgs> MessageUpdated;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">
        /// The channel this container is assigned to.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel this container is assigned to.
        /// </param>
        public MessageContainer(byte channel, ushort subchannel)
        {
            _message.Channel = channel;
            _message.Subchannel = subchannel;
        }

        /// <summary>
        /// Gets the contained message.
        /// </summary>
        /// <returns>A message.</returns>
        public Message Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Updates the contained message.
        /// </summary>
        /// <param name="message">The new message.</param>
        public void UpdateMessage(Message message)
        {
            if (!message.Equals(_message))
            {
                _message = message;
                if (MessageUpdated != null)
                {
                    MessageUpdated(this, EventArgs.Empty);
                }
            }
        }
    }
}