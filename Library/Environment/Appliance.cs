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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Environment
{
    /// <summary>
    /// Provides the appliance currently used.
    /// </summary>
    public class Appliance : IDisposable
    {
        private readonly IApplianceConnection _applianceConnection;
        private readonly IDeviceFactory _deviceFactory;
        private ISignalGenerator _signalGenerator;

        /// <summary>
        /// Gets the appliance connection used by this instance.
        /// </summary>
        public IApplianceConnection ApplianceConnection
        {
            get
            {
                return _applianceConnection;
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
        /// <param name="applianceConnection">The connection used to access the appliance.</param>
        /// <param name="deviceFactory">The factory used to create the devices of the appliance.</param>
        public Appliance(IApplianceConnection applianceConnection, IDeviceFactory deviceFactory)
        {
            _applianceConnection = applianceConnection;
            _deviceFactory = deviceFactory;
        }

        /// <summary>
        /// Initializes or reinitializes a single FPGA Lab device instance
        /// running the signal generator.
        /// </summary>
        /// <param name="mainChannel">
        /// The number of the main channel assigned to the FPGA Lab.
        /// </param>
        public void InitializeSignalGenerator(byte mainChannel)
        {
            lock (_applianceConnection.SyncRoot)
            {
                if (_signalGenerator != null)
                    _signalGenerator.Dispose();

                _signalGenerator = _deviceFactory.CreateSignalGenerator(mainChannel);

                // Initialize the device.
                _signalGenerator.Reset();
                _applianceConnection.FlushModifications();
            }
        }

        public void Dispose()
        {
            if (_signalGenerator != null)
                _signalGenerator.Dispose();

            if (_applianceConnection != null)
                _applianceConnection.Dispose();
        }
    }
}