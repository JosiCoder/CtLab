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
using CtLab.Connection.Interfaces;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.Device.Base;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Environment
{
    /// <summary>
    /// Provides the appliance currently used.
    /// </summary>
    public class Appliance : ApplianceBase, IDisposable
    {
        private readonly IDeviceFactory _deviceFactory;
        private readonly IConnection _connection;
        private ISignalGenerator _signalGenerator;

        /// <summary>
        /// Gets the connection used by this instance.
        /// </summary>
        public IConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// Gets the signal generator.
        /// </summary>
        public ISignalGenerator SignalGenerator
        {
            get
            {
                return _signalGenerator;
            }
        }

        // More parts might go here (additional FPGA Lab instances or other c´t Lab devices).

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceFactory">The factory used to create the devices of the appliance.</param>
        /// <param name="connection">The connection used by this instance.</param>
        /// <param name="setCommandClassDictionary">The command clasSignalGenerators dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public Appliance(IDeviceFactory deviceFactory, IConnection connection, ISetCommandClassDictionary setCommandClassDictionary,
            IQueryCommandScheduler queryCommandScheduler, IMessageCache receivedMessagesCache)
            : base(setCommandClassDictionary, queryCommandScheduler, receivedMessagesCache)
        {
            _deviceFactory = deviceFactory;
            _connection = connection;
        }

        /// <summary>
        /// Sets the number of the channel assigned to the FPGA Lab device.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel assigned to the FPGA Lab.
        /// </param>
        public void SetSignalGeneratorChannel(byte channel)
        {
            InitializeSignalGenerator(channel);
        }

        /// <summary>
        /// Initializes or reinitializes a single FPGA Lab device instance.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel assigned to the FPGA Lab.
        /// </param>
        private void InitializeSignalGenerator(byte channel)
        {
            var schedulerSyncRoot = new object();
            if (QueryCommandScheduler != null)
                schedulerSyncRoot = QueryCommandScheduler.SyncRoot;

            lock (schedulerSyncRoot)
            {
                if (_signalGenerator != null)
                    _signalGenerator.Dispose();

                _signalGenerator = _deviceFactory.CreateSignalGenerator(channel);

                // Initialize the device.
                _signalGenerator.Reset();
                SendSetCommandsForModifiedValues();
            }
        }

        public void Dispose()
        {
            if (_signalGenerator != null)
                _signalGenerator.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }
    }
}