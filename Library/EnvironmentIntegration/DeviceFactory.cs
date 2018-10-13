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
        private readonly IFpgaValuesAccessor _fpgaValuesAccessor;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        /// <param name="fpgaValuesAccessor">The accessor used to control access to FPGA values.</param>
        public DeviceFactory(ISetCommandClassDictionary setCommandClassDictionary, IQueryCommandScheduler queryCommandScheduler,
            IMessageCache receivedMessagesCache, IFpgaValuesAccessor fpgaValuesAccessor)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandScheduler = queryCommandScheduler;
            _receivedMessagesCache = receivedMessagesCache;
            _fpgaValuesAccessor = fpgaValuesAccessor;
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
        /// <param name="applianceConnection">The connection used to access the appliance.</param>
        public IScope CreateCtLabProtocolScope(byte mainchannel, IApplianceConnection applianceConnection)
        {
            var hardwareSettings = new StorageHardwareSettings
            {
                WriteWithHandshake = false,                   // usually not needed at all
                ReadWithHandshake = true,                     // needed for c't Lab protocol
                OptimizeSpiReading = false,                   // only for SPI, considered only when ReadWithHandshake=false
                MillisecondsToWaitForAsynchronousReads = 100  // 10 or more needed for c't Lab protocol
            };
            return CreateScope(GetConnection(mainchannel), hardwareSettings);
        }

        /// <summary>
        /// Creates an FPGA-based scope that can be accessed via the SPI interface.
        /// </summary>
        /// <param name="applianceConnection">The connection used to access the appliance.</param>
        public IScope CreateSpiDirectScope(IApplianceConnection applianceConnection)
        {
            var hardwareSettings = new StorageHardwareSettings
            {
                WriteWithHandshake = false,                   // usually not needed at all
                ReadWithHandshake = false,                    // not needed as SPI is much faster than the client code
                OptimizeSpiReading = true,                    // optional for SPI, considered only when ReadWithHandshake=false
                MillisecondsToWaitForAsynchronousReads = 0    // not needed for SPI as it's synchronous
            };
            return CreateScope(GetConnection(_spiConnectionKey), hardwareSettings);
        }

        /// <summary>
        /// Creates an FPGA-based signal generator.
        /// </summary>
        private ISignalGenerator CreateSignalGenerator(IFpgaConnection fpgaConnection)
        {
            return new SignalGenerator(fpgaConnection);
        }

        /// <summary>
        /// Creates an FPGA-based scope.
        /// </summary>
        private IScope CreateScope(IFpgaConnection fpgaConnection, StorageHardwareSettings hardwareSettings)
        {
            return new Scope(fpgaConnection, hardwareSettings, _fpgaValuesAccessor);
        }
    }
}