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

using StructureMap;
using CtLab.Environment;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Registers required classes with the dependency injection container.
    /// </summary>
    public class ApplianceRegistry : Registry
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public ApplianceRegistry()
        {
            // === Lab appliances and devices ===

            For<Appliance>()
                .Singleton()
                .Use<Appliance>();

            For<IDeviceFactory>()
                .Singleton()
                .Use<DeviceFactory>();
        }
    }
}
