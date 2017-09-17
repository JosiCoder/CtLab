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

namespace CtLab.Device.Base
{
    /// <summary>
    /// Provides access to a c't Lab device.
    /// </summary>
    public class DeviceConnection
    {
        private readonly ISetCommandClassDictionary<MessageChannel> _setCommandClassDictionary;
        private readonly IQueryCommandClassDictionary<MessageChannel> _queryCommandClassDictionary;
        private readonly IMessageCache<MessageChannel> _receivedMessagesCache;

        /// <summary>
        /// The number of the main channel assigned to the c't Lab device controlled by this instance.
        /// </summary>
        public readonly byte MainChannel;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="mainChannel">
        /// The number of the main channel assigned to the c't Lab device controlled by this instance.
        /// </param>
        /// <param name="setCommandClassDictionary">The dictonary used for the set command classes.</param>
        /// <param name="queryCommandClassDictionary">The dictonary used for the query command classes.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public DeviceConnection(byte mainChannel, ISetCommandClassDictionary<MessageChannel> setCommandClassDictionary, IQueryCommandClassDictionary<MessageChannel> queryCommandClassDictionary, IMessageCache<MessageChannel> receivedMessagesCache)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandClassDictionary = queryCommandClassDictionary;
            _receivedMessagesCache = receivedMessagesCache;
            MainChannel = mainChannel;
        }

        /// <summary>
        /// Releases all resource used by this instance.
        /// </summary>
        public void Dispose()
        {
            Detach();
        }

        /// <summary>
        /// Detaches the c't Lab device from the command dictionaries and messages caches,
        /// i.e. removes all commands and unregisters all messages that belong to the
        /// specified device from these caches.
        /// </summary>
        private void Detach()
        {
            _setCommandClassDictionary.RemoveChannelCommands(key => key.Main == MainChannel);
            _queryCommandClassDictionary.RemoveChannelCommands(key => key.Main == MainChannel);
            _receivedMessagesCache.UnregisterMessageChannels(key => key.Main == MainChannel);
        }

        /// <summary>
        /// Builds a set command class and adds it to the dictionary.
        /// </summary>
        /// <param name="subchannel">
        /// The subchannel number corresponding to the commands sent.
        /// </param>
        /// <returns>The set command class.</returns>
        public SetCommandClass<MessageChannel> BuildAndRegisterSetCommandClass(ushort subchannel)
        {
            var commandClass = new SetCommandClass<MessageChannel>(new MessageChannel(MainChannel, subchannel));
            _setCommandClassDictionary.Add(commandClass);
            return commandClass;
        }

        /// <summary>
        /// Builds a query command class and adds it to the dictionary.
        /// </summary>
        /// <param name="subchannel">
        /// The subchannel number corresponding to the commands sent.
        /// </param>
        /// <returns>The query command class.</returns>
        public QueryCommandClass<MessageChannel> BuildAndRegisterQueryCommandClass(ushort subchannel)
        {
            var commandClass = new QueryCommandClass<MessageChannel>(new MessageChannel(MainChannel, subchannel));
            _queryCommandClassDictionary.Add(commandClass);
            return commandClass;
        }

        /// <summary>
        /// Registers a channel's subchannel for message caching and returns the message
        /// container for that subchannel.
        /// </summary>
        /// <param name="subchannel">
        /// The subchannel to register.
        /// </param>
        /// <returns>The message container.</returns>
        public IMessageContainer<MessageChannel> RegisterMessage(ushort subchannel)
        {
            return _receivedMessagesCache.Register(new MessageChannel(MainChannel, subchannel));
        }
    }
}