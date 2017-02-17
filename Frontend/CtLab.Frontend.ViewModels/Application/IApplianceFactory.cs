//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

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
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides access to an appliance factory.
    /// </summary>
    public interface IApplianceFactory
    {
        /// <summary>
        /// Gets the names of the available serial ports.
        /// </summary>
        IEnumerable<string> AvailablePortNames
        { get; }

        /// <summary>
        /// Creates and returns a new c't Lab appliance.
        /// </summary>
        /// <param name="portName">The name of the serial port to use.</param>
        /// <param name="channel">
        /// The number of the channel to send to and receive from.
        /// </param>
        /// <returns>The appliance created.</returns>
        Appliance CreateSerialAppliance(string portName, byte channel);

        /// <summary>
        /// Creates and returns a new dummy c't Lab appliance.
        /// Sample strings are injected to that connection to simulate reception of
        /// such strings.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel to send to and receive from.
        /// </param>
        /// <returns>The appliance created.</returns>
        Appliance CreateDummyAppliance(byte channel);
    }
}

