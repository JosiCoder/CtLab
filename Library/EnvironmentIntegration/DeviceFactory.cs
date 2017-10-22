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
using CtLab.FpgaConnection.CtLabProtocol;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;
using CtLab.Environment;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Creates the devices of the appliance.
    /// </summary>
    public class DeviceFactory : IDeviceFactory
    {
        private readonly ISetCommandClassDictionary _setCommandClassDictionary;
        private readonly IQueryCommandScheduler _queryCommandScheduler;
        private readonly IMessageCache _receivedMessagesCache;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public DeviceFactory(ISetCommandClassDictionary setCommandClassDictionary, IQueryCommandScheduler queryCommandScheduler,
            IMessageCache receivedMessagesCache)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandScheduler = queryCommandScheduler;
            _receivedMessagesCache = receivedMessagesCache;
        }

        /// <summary>
        /// Creates an FPGA-based signal generator that can be accessed via the c't Lab protocol.
        /// </summary>
        /// <param name="mainchannel">
        /// The number of the mainchannel assigned to the FPGA module.
        /// </param>
        public ISignalGenerator CreateCtLabProtocolSignalGenerator(byte mainchannel)
        {
            var deviceConnection = new CtLabProtocolDeviceConnection (mainchannel,
                _setCommandClassDictionary, _queryCommandScheduler.CommandClassDictionary,
                _receivedMessagesCache);

            return CreateSignalGenerator(deviceConnection);
        }

        /// <summary>
        /// Creates an FPGA-based signal generator that can be accessed via the SPI interface.
        /// </summary>
        public ISignalGenerator CreateSpiDirectSignalGenerator()
        {
            var deviceConnection = new SpiDirectDeviceConnection(
                _setCommandClassDictionary, _queryCommandScheduler.CommandClassDictionary,
                _receivedMessagesCache);

            return CreateSignalGenerator(deviceConnection);
        }

        /// <summary>
        /// Creates an FPGA-based signal generator.
        /// </summary>
        /// <param name="deviceConnection">The connection used to access the signal generator.</param>
        private ISignalGenerator CreateSignalGenerator(IDeviceConnection deviceConnection)
        {
            var fpgaConnection = new
                CtLab.FpgaConnection.CtLabProtocol.FpgaConnection (deviceConnection);

            return new SignalGenerator(fpgaConnection);
        }
    }
}