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
using CtLab.Utilities;
using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Standard
{
    /// <summary>
    /// Holds and returns a message received from a certain message channel.
    /// </summary>
    /// <typeparam name="TMessageChannel">The type of the message channel.</typeparam>
    public class MessageContainer<TMessageChannel>: IMessageContainer<TMessageChannel>
    {
        private Message<TMessageChannel> _message;

        /// <summary>
        /// Occurs when the message has been updated.
        /// </summary>
        public event EventHandler<EventArgs> MessageUpdated;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageChannel">
        /// The message channel this container is assigned to.
        /// </param>
        public MessageContainer(TMessageChannel messageChannel)
        {
            _message.Channel = messageChannel;
        }

        /// <summary>
        /// Gets the contained message.
        /// </summary>
        /// <returns>The message.</returns>
        public Message<TMessageChannel> Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Updates the contained message.
        /// </summary>
        /// <param name="message">The new message.</param>
        public void UpdateMessage(Message<TMessageChannel> message)
        {
            if (!message.Equals(_message))
            {
                _message = message;
                MessageUpdated.Raise(this, EventArgs.Empty);
            }
        }
    }
}