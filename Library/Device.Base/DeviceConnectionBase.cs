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
    /// Provides the base functionality for device connections.
    /// </summary>
    public abstract class DeviceConnectionBase : IDeviceConnection
    {
        private readonly ISetCommandClassDictionary _setCommandClassDictionary;
        private readonly IQueryCommandClassDictionary _queryCommandClassDictionary;
        private readonly IMessageCache _receivedMessagesCache;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClassDictionary">The dictonary used for the set command classes.</param>
        /// <param name="queryCommandClassDictionary">The dictonary used for the query command classes.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        protected DeviceConnectionBase(ISetCommandClassDictionary setCommandClassDictionary,
            IQueryCommandClassDictionary queryCommandClassDictionary, IMessageCache receivedMessagesCache)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandClassDictionary = queryCommandClassDictionary;
            _receivedMessagesCache = receivedMessagesCache;
        }

        /// <summary>
        /// Releases all resource used by this instance.
        /// </summary>
        public void Dispose()
        {
            Detach();
        }

        /// <summary>
        /// Gets the predicate used to determine which message channels are affected when detaching
        /// the c't Lab device from the command dictionaries and messages caches.
        /// </summary>
        protected abstract Func<IMessageChannel, bool> DetachPredicate
        { get; }

        /// <summary>
        /// Creates a message channel for the specified FPGA register.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to create the message channel vor.
        /// </param>
        /// <returns>The created message channel.</returns>
        protected abstract IMessageChannel CreateMessageChannel (ushort registerNumber);

        /// <summary>
        /// Detaches the c't Lab device from the command dictionaries and messages caches,
        /// i.e. removes all commands and unregisters all messages that belong to the
        /// specified device from these caches.
        /// </summary>
        protected void Detach()
        {
            _setCommandClassDictionary.RemoveChannelCommands(DetachPredicate);
            _queryCommandClassDictionary.RemoveChannelCommands(DetachPredicate);
            _receivedMessagesCache.UnregisterMessageChannels(DetachPredicate);
        }

        /// <summary>
        /// Builds a set command class and adds it to the dictionary.
        /// </summary>
        /// <param name="registerNumber">
        /// The FPGA register number corresponding to the commands sent.
        /// </param>
        /// <param name="group">The group the command class is related to.</param>
        /// <returns>The set command class.</returns>
        public SetCommandClass BuildAndRegisterSetCommandClass(ushort registerNumber, CommandClassGroup group)
        {
            var commandClass = new SetCommandClass(CreateMessageChannel(registerNumber));
            _setCommandClassDictionary.Add(commandClass, group);
            return commandClass;
        }

        /// <summary>
        /// Builds a query command class and adds it to the dictionary.
        /// </summary>
        /// <param name="registerNumber">
        /// The FPGA register number corresponding to the commands sent.
        /// </param>
        /// <param name="group">The group the command class is related to.</param>
        /// <returns>The query command class.</returns>
        public QueryCommandClass BuildAndRegisterQueryCommandClass(ushort registerNumber, CommandClassGroup group)
        {
            var commandClass = new QueryCommandClass(CreateMessageChannel(registerNumber));
            _queryCommandClassDictionary.Add(commandClass, group);
            return commandClass;
        }

        /// <summary>
        /// Registers an FPGA register for message caching and returns the message
        /// container for that FPGA register.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to register.
        /// </param>
        /// <returns>The message container.</returns>
        public IMessageContainer RegisterMessage(ushort registerNumber)
        {
            return _receivedMessagesCache.Register(CreateMessageChannel(registerNumber));
        }
    }
}