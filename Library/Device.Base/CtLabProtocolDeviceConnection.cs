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
    /// Provides access to a c't Lab device via the c't Lab protocol.
    /// </summary>
    public class CtLabProtocolDeviceConnection : DeviceConnectionBase
    {
        private readonly byte _mainchannel;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="mainchannel">
        /// The number of the mainchannel assigned to the c't Lab device controlled by this instance.
        /// </param>
        /// <param name="setCommandClassDictionary">The dictonary used for the set command classes.</param>
        /// <param name="queryCommandClassDictionary">The dictonary used for the query command classes.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public CtLabProtocolDeviceConnection(byte mainchannel, ISetCommandClassDictionary setCommandClassDictionary,
            IQueryCommandClassDictionary queryCommandClassDictionary, IMessageCache receivedMessagesCache)
            : base(setCommandClassDictionary, queryCommandClassDictionary, receivedMessagesCache)
        {
            _mainchannel = mainchannel;
        }

        /// <summary>
        /// Gets the predicate used to determine which message channels are affected when detaching
        /// the c't Lab device from the command dictionaries and messages caches.
        /// </summary>
        protected override Func<IMessageChannel, bool> DetachPredicate
        {
            get
            {
                Func<IMessageChannel, bool> sameMainchannelPredicate = channel =>
                {
                    var ctLabChannel = channel as MessageChannel;
                    return ctLabChannel != null && ctLabChannel.Main == _mainchannel;
                };

                return sameMainchannelPredicate;
            }
        }

        /// <summary>
        /// Creates a message channel for the specified FPGA register.
        /// </summary>
        /// <param name="registerNumber">
        /// The number of the FPGA register to create the message channel vor.
        /// </param>
        /// <returns>The created message channel.</returns>
        protected override IMessageChannel CreateMessageChannel (ushort registerNumber)
        {
            return new MessageChannel(_mainchannel, registerNumber);
        }
    }
}