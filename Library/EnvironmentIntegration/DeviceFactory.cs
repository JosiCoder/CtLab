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
using CtLab.Device.Base;
using CtLab.Messages.Interfaces;
using CtLab.CtLabProtocol.Interfaces;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;
using CtLab.FpgaConnection.CtLabProtocol;
using CtLab.Environment;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Creates the devices of the appliance.
    /// </summary>
    public class DeviceFactory : IDeviceFactory
    {
        private readonly ISetCommandClassDictionary<MessageChannel> _setCommandClassDictionary;
        private readonly IQueryCommandScheduler<MessageChannel> _queryCommandScheduler;
        private readonly IMessageCache<MessageChannel> _receivedMessagesCache;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public DeviceFactory(ISetCommandClassDictionary<MessageChannel> setCommandClassDictionary, IQueryCommandScheduler<MessageChannel> queryCommandScheduler,
            IMessageCache<MessageChannel> receivedMessagesCache)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandScheduler = queryCommandScheduler;
            _receivedMessagesCache = receivedMessagesCache;
        }

        /// <summary>
        /// Creates an FPGA-based signal generator.
        /// </summary>
        /// <param name="mainChannel">
        /// The number of the main channel assigned to the FPGA module.
        /// </param>
        public ISignalGenerator CreateSignalGenerator(byte mainChannel)
        {
            var deviceConnection = new DeviceConnection (mainChannel, _setCommandClassDictionary,
                _queryCommandScheduler.CommandClassDictionary, _receivedMessagesCache);

            var fpgaConnection = new
                CtLab.FpgaConnection.CtLabProtocol.FpgaConnection (deviceConnection);

            return new SignalGenerator(fpgaConnection);
        }
    }
}