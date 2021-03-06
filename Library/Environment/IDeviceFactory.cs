﻿//------------------------------------------------------------------------------
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

using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaScope.Interfaces;

namespace CtLab.Environment
{
    /// <summary>
    /// Provides facilities to create the devices of the appliance.
    /// </summary>
    public interface IDeviceFactory
    {
        /// <summary>
        /// Creates an FPGA-based signal generator that can be accessed via the c't Lab protocol.
        /// </summary>
        /// <param name="mainchannel">
        /// The number of the mainchannel assigned to the FPGA module.
        /// </param>
        ISignalGenerator CreateCtLabProtocolSignalGenerator(byte mainchannel);

        /// <summary>
        /// Creates an FPGA-based signal generator that can be accessed via the SPI interface.
        /// </summary>
        ISignalGenerator CreateSpiDirectSignalGenerator();

        /// <summary>
        /// Creates an FPGA-based scope that can be accessed via the c't Lab protocol.
        /// </summary>
        /// <param name="mainchannel">
        /// The number of the mainchannel assigned to the FPGA module.
        /// </param>
        /// <param name="applianceConnection">The connection used to access the appliance.</param>
        IScope CreateCtLabProtocolScope(byte mainchannel, IApplianceConnection applianceConnection);

        /// <summary>
        /// Creates an FPGA-based scope that can be accessed via the SPI interface.
        /// </summary>
        /// <param name="applianceConnection">The connection used to access the appliance.</param>
        IScope CreateSpiDirectScope(IApplianceConnection applianceConnection);
    }
}