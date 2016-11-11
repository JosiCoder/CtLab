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
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Holds the most-recently received messages for channels´ subchannels registered
    /// for message caching.
    /// </summary>
    public class ReceivedMessagesCache : IMessageCache
    {
        private struct MessageKey
        {
            public MessageKey(byte channel, ushort subchannel)
            {
                Channel = channel;
                Subchannel = subchannel;
            }
            public byte Channel;
            public ushort Subchannel;
        }

        private readonly IMessageReceiver _messageReceiver;
        private readonly Dictionary<MessageKey, MessageContainer> _messageDictionary = new Dictionary<MessageKey, MessageContainer>();

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
        /// Registers a channel´s subchannel for message caching and returns the message
        /// container for that subchannel.
        /// </summary>
        /// <param name="channel">
        /// The channel to register a subchannel for.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel to register.
        /// </param>
        /// <returns>The message container.</returns>
        public IMessageContainer Register(byte channel, ushort subchannel)
        {
            // Add a container holding a default message to the dictionary.
            var container = new MessageContainer(channel, subchannel);
            _messageDictionary.Add(new MessageKey(channel, subchannel), container);
            return container;
        }


        /// <summary>
        /// Unregisters all subchannels belonging to a specific channel for message caching.
        /// </summary>
        /// <param name="channel">The channel to unregister the subchannels for.</param>
        public void UnregisterSubchannelsForChannel(byte channel)
        {
            var affectedKeys = (from key in _messageDictionary.Keys
                                where key.Channel == channel
                                select key
                                ).ToArray();

            foreach (var key in affectedKeys)
            {
                _messageDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Gets the message container for a channel´s registered subchannel.
        /// </summary>
        /// <param name="channel">
        /// The channel to get a subchannel´s message container for.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel to get the message container for.
        /// </param>
        /// <returns>The message container.</returns>
        public IMessageContainer GetMessageContainer(byte channel, ushort subchannel)
        {
            return _messageDictionary[new MessageKey(channel, subchannel)];
        }

        /// <summary>
        /// Updates a cached message within the cache based on the specified channel and
        /// subchannel. Messages for unregistered subchannels are ignored.
        /// </summary>
        /// <param name="message">The message used to update the cache.</param>
        /// <returns>
        /// The container of the updated message or null for unregistered subchannels.
        /// </returns>
        public IMessageContainer UpdateMessage(Message message)
        {
            var key = new MessageKey(message.Channel, message.Subchannel);
            if (_messageDictionary.ContainsKey(key))
            {
                _messageDictionary[key].UpdateMessage(message);
                return _messageDictionary[key];
            }
            return null;
        }
    }
}