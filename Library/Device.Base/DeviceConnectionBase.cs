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

namespace CtLab.Device.Base
{
    /// <summary>
    /// Provides the base functionality for accessing c't Lab devices.
    /// </summary>
    public abstract class DeviceConnectionBase : IDisposable
    {
        /// <summary>
        /// Gets the command namespace CtFpga. used to send the set commands.
        /// </summary>
        private readonly ISetCommandClassDictionary _setCommandClassDictionary;

        /// <summary>
        /// Gets the command namespace CtFpga. used to send the query commands.
        /// </summary>
        private readonly IQueryCommandClassDictionary _queryCommandClassDictionary;

        /// <summary>
        /// Gets the message cache used to receive the messages.
        /// </summary>
        private readonly IMessageCache _receivedMessagesCache;

        /// <summary>
        /// Gets the number of the channel assigned to the c't Lab device controlled by this instance.
        /// </summary>
        public readonly byte Channel;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel assigned to the c't Lab device controlled by this instance.
        /// </param>
        /// <param name="setCommandClassDictionary">The dictonary used for the set command classes.</param>
        /// <param name="queryCommandClassDictionary">The dictonary used for the query command classes.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        protected DeviceConnectionBase(byte channel, ISetCommandClassDictionary setCommandClassDictionary, IQueryCommandClassDictionary queryCommandClassDictionary, IMessageCache receivedMessagesCache)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandClassDictionary = queryCommandClassDictionary;
            _receivedMessagesCache = receivedMessagesCache;
            Channel = channel;
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
            _setCommandClassDictionary.RemoveCommandsForChannel(Channel);
            _queryCommandClassDictionary.RemoveCommandsForChannel(Channel);
            _receivedMessagesCache.UnregisterSubchannelsForChannel(Channel);
        }

        /// <summary>
        /// Builds a set command class and adds it to the dictionary.
        /// </summary>
        /// <param name="channel">
        /// The channel number of the device the commands are sent to.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel number corresponding to the commands sent.
        /// </param>
        /// <returns>The set command class.</returns>
        protected SetCommandClass BuildAndRegisterSetCommandClass(byte channel, ushort subchannel)
        {
            var commandClass = new SetCommandClass(channel, subchannel);
            _setCommandClassDictionary.Add(commandClass);
            return commandClass;
        }

        /// <summary>
        /// Builds a query command class and adds it to the dictionary.
        /// </summary>
        /// <param name="channel">
        /// The channel number of the device the commands are sent to.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel number corresponding to the commands sent.
        /// </param>
        /// <returns>The query command class.</returns>
        protected QueryCommandClass BuildAndRegisterQueryCommandClass(byte channel, ushort subchannel)
        {
            var commandClass = new QueryCommandClass(channel, subchannel);
            _queryCommandClassDictionary.Add(commandClass);
            return commandClass;
        }

        /// <summary>
        /// Registers a channel's subchannel for message caching and returns the message
        /// container for that subchannel.
        /// </summary>
        /// <param name="channel">
        /// The channel to register a subchannel for.
        /// </param>
        /// <param name="subchannel">
        /// The subchannel to register.
        /// </param>
        /// <returns>The message container.</returns>
        protected IMessageContainer RegisterMessage(byte channel, ushort subchannel)
        {
            return _receivedMessagesCache.Register(channel, subchannel);
        }
    }
}