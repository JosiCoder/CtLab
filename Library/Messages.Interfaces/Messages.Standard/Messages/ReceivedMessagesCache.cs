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
    public class ReceivedMessagesCache : IMessageCache
    {
        private readonly IMessageReceiver _messageReceiver;
        private readonly Dictionary<IMessageChannel, MessageContainer> _messageDictionary
            = new Dictionary<IMessageChannel, MessageContainer>();

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageReceiver">The message receiver used to receive the messages.</param>
        public ReceivedMessagesCache(IMessageReceiver messageReceiver)
        {
            _messageReceiver = messageReceiver;
            _messageReceiver.MessageReceived += (sender, e) => UpdateMessage(e.Message);
        }

        /// <summary>
        /// Registers a message channel for caching and returns the message container
        /// for that message channel.
        /// </summary>
        /// <param name="messageChannel">
        /// The message channel to register.
        /// </param>
        /// <returns>The message container for the specified message channel.</returns>
        public IMessageContainer Register(IMessageChannel messageChannel)
        {
            // Add a container holding a default message to the dictionary.
            var container = new MessageContainer(messageChannel);
            _messageDictionary.Add(messageChannel, container);
            return container;
        }

        /// <summary>
        /// Unregisters all message channels that meet the specified predicate from caching.
        /// </summary>
        /// <param name="predicate">The predicate that must be met.</param>
        public void UnregisterMessageChannels(Func<IMessageChannel, bool> predicate)
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
        /// Gets the message container for the specified message channel.
        /// </summary>
        /// <param name="messageChannel">
        /// The message channel to get the message container for.
        /// </param>
        /// <returns>The message container.</returns>
        public IMessageContainer GetMessageContainer(IMessageChannel messageChannel)
        {
            return _messageDictionary[messageChannel];
        }

        /// <summary>
        /// Updates a cached message within the cache based on the specified message channel.
        /// Messages for unregistered message channels are ignored.
        /// </summary>
        /// <param name="message">The message used to update the cache.</param>
        /// <returns>
        /// The container of the updated message or null for unregistered message channels.
        /// </returns>
        public IMessageContainer UpdateMessage(IMessage message)
        {
            var key = message.Channel;
            if (_messageDictionary.ContainsKey(key))
            {
                _messageDictionary[key].UpdateMessage(message);
                return _messageDictionary[key];
            }
            return null;
        }
    }
}