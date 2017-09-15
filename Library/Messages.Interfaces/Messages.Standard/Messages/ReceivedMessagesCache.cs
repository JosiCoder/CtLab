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
using System.Collections.Generic;
using System.Linq;
using CtLab.Messages.Interfaces;

namespace CtLab.Messages.Standard
{
    /// <summary>
    /// Holds the most-recently received messages for sources registered for message caching.
    /// </summary>
    /// <typeparam name="TMessageSource">The type of the message source.</typeparam>
    public class ReceivedMessagesCache<TMessageSource> : IMessageCache<TMessageSource>
    {
        private readonly IMessageReceiver<TMessageSource> _messageReceiver;
        private readonly Dictionary<TMessageSource, MessageContainer<TMessageSource>> _messageDictionary
            = new Dictionary<TMessageSource, MessageContainer<TMessageSource>>();

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageReceiver">The message receiver used to receive the messages.</param>
        public ReceivedMessagesCache(IMessageReceiver<TMessageSource> messageReceiver)
        {
            _messageReceiver = messageReceiver;
            _messageReceiver.MessageReceived += (sender, e) => UpdateMessage(e.Message);
        }

        /// <summary>
        /// Registers a message source for caching and returns the message container
        /// for that message source.
        /// </summary>
        /// <param name="messageSource">
        /// The message source to register.
        /// </param>
        /// <returns>The message container for the specified message source.</returns>
        public IMessageContainer<TMessageSource> Register(TMessageSource messageSource)
        {
            // Add a container holding a default message to the dictionary.
            var container = new MessageContainer<TMessageSource>(messageSource);
            _messageDictionary.Add(messageSource, container);
            return container;
        }

        /// <summary>
        /// Unregisters all message sources that meet the specified predicate from caching.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        public void UnregisterSubchannelsForChannel(Func<TMessageSource, bool> predicate)
        {
            var affectedKeys = (from key in _messageDictionary.Keys
                                where predicate(key)
                                select key
                                ).ToArray();

            foreach (var key in affectedKeys)
            {
                _messageDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Gets the message container for the specified message source.
        /// </summary>
        /// <param name="messageSource">
        /// The message source to get the message container for.
        /// </param>
        /// <returns>The message container.</returns>
        public IMessageContainer<TMessageSource> GetMessageContainer(TMessageSource messageSource)
        {
            return _messageDictionary[messageSource];
        }

        /// <summary>
        /// Updates a cached message within the cache based on the specified message source.
        /// Messages for unregistered message sources are ignored.
        /// </summary>
        /// <param name="message">The message used to update the cache.</param>
        /// <returns>
        /// The container of the updated message or null for unregistered message sources.
        /// </returns>
        public IMessageContainer<TMessageSource> UpdateMessage(Message<TMessageSource> message)
        {
            var key = message.Source;
            if (_messageDictionary.ContainsKey(key))
            {
                _messageDictionary[key].UpdateMessage(message);
                return _messageDictionary[key];
            }
            return null;
        }
    }
}