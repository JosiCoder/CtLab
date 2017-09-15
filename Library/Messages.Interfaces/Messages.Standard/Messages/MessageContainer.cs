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
    /// Holds and returns a message received from a certain message source.
    /// </summary>
    /// <typeparam name="TMessageSource">The type of the message source.</typeparam>
    public class MessageContainer<TMessageSource>: IMessageContainer<TMessageSource>
    {
        private Message<TMessageSource> _message;

        /// <summary>
        /// Occurs when the message has been updated.
        /// </summary>
        public event EventHandler<EventArgs> MessageUpdated;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageSource">
        /// The message source this container is assigned to.
        /// </param>
        public MessageContainer(TMessageSource messageSource)
        {
            _message.Source = messageSource;
        }

        /// <summary>
        /// Gets the contained message.
        /// </summary>
        /// <returns>The message.</returns>
        public Message<TMessageSource> Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Updates the contained message.
        /// </summary>
        /// <param name="message">The new message.</param>
        public void UpdateMessage(Message<TMessageSource> message)
        {
            if (!message.Equals(_message))
            {
                _message = message;
                MessageUpdated.Raise(this, EventArgs.Empty);
            }
        }
    }
}