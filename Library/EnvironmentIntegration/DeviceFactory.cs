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
using CtLab.Device.Base;
using CtLab.Messages.Interfaces;
using CtLab.FpgaConnection.Interfaces;
using CtLab.FpgaConnection.Standard;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaSignalGenerator.Standard;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaScope.Standard;
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
        private readonly Dictionary<object, IFpgaConnection> _connections =
            new Dictionary<object, IFpgaConnection>();
        private readonly object _spiConnectionKey = new object();

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

        private IFpgaConnection GetConnection(object connectionKey)
        {
            IFpgaConnection fpgaConnection;
            if (!_connections.TryGetValue (connectionKey, out fpgaConnection))
            {
                IDeviceConnection deviceConnection;
                if (connectionKey is byte)
                {
                    deviceConnection =
                        new CtLabProtocolDeviceConnection ((byte)connectionKey,
                        _setCommandClassDictionary, _queryCommandScheduler.CommandClassDictionary,
                        _receivedMessagesCache);
                }
                else
                {
                    deviceConnection =
                        new SpiDirectDeviceConnection (_setCommandClassDictionary,
                            _queryCommandScheduler.CommandClassDictionary,
                            _receivedMessagesCache);
                }

                fpgaConnection = new
                    CtLab.FpgaConnection.Standard.FpgaConnection (deviceConnection);
                
                _connections.Add (connectionKey, fpgaConnection);
            }
            return fpgaConnection;
        }

        /// <summary>
        /// Creates an FPGA-based signal generator that can be accessed via the c't Lab protocol.
        /// </summary>
        /// <param name="mainchannel">
        /// The number of the mainchannel assigned to the FPGA module.
        /// </param>
        public ISignalGenerator CreateCtLabProtocolSignalGenerator(byte mainchannel)
        {
            return CreateSignalGenerator(GetConnection(mainchannel));
        }

        /// <summary>
        /// Creates an FPGA-based signal generator that can be accessed via the SPI interface.
        /// </summary>
        public ISignalGenerator CreateSpiDirectSignalGenerator()
        {
            return CreateSignalGenerator(GetConnection(_spiConnectionKey));
        }

        /// <summary>
        /// Creates an FPGA-based scope that can be accessed via the c't Lab protocol.
        /// </summary>
        /// <param name="mainchannel">
        /// The number of the mainchannel assigned to the FPGA module.
        /// </param>
        public IScope CreateCtLabProtocolScope(byte mainchannel)
        {
            return CreateScope(GetConnection(mainchannel));
        }

        /// <summary>
        /// Creates an FPGA-based scope that can be accessed via the SPI interface.
        /// </summary>
        public IScope CreateSpiDirectScope()
        {
            return CreateScope(GetConnection(_spiConnectionKey));
        }

        /// <summary>
        /// Creates an FPGA-based signal generator.
        /// </summary>
        /// <param name="fpgaConnection">The connection used to access the signal generator.</param>
        private ISignalGenerator CreateSignalGenerator(IFpgaConnection fpgaConnection)
        {
            return new SignalGenerator(fpgaConnection);
        }

        /// <summary>
        /// Creates an FPGA-based scope.
        /// </summary>
        /// <param name="fpgaConnection">The connection used to access the scope.</param>
        private IScope CreateScope(IFpgaConnection fpgaConnection)
        {
            return new Scope(fpgaConnection);
        }
    }
}