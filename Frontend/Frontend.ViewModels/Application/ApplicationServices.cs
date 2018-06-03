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
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides appliance-related services.
    /// </summary>
    public class ApplianceServices : IApplianceServices
    {
        private readonly Appliance _appliance;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="appliance">The appliance providing the services.</param>
        public ApplianceServices (Appliance appliance)
        {
            _appliance = appliance;
        }

        /// <summary>
        /// Flushes any modifications to the hardware.
        /// </summary>
        public void Flush ()
        {
            _appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
        }
    }
}

