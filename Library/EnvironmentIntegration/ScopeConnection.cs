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

using System;
using CtLab.FpgaScope.Interfaces;
using CtLab.Environment;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Provides access to a scope.
    /// </summary>
    internal class ScopeConnection : IScopeConnection
    {
        private readonly IApplianceConnection _applianceConnection;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="applianceConnection">The appliance connection to use.</param>
        public ScopeConnection (IApplianceConnection applianceConnection)
        {
            _applianceConnection = applianceConnection;
        }

        /// <summary>
        /// Sends all modified setter values to the scope.
        /// </summary>
        public void FlushSetters()
        {
            _applianceConnection.SendSetCommandsForModifiedValues ();
        }

        /// <summary>
        /// Refreshes all getter values from the scope.
        /// </summary>
        public void RefreshGetters()
        {
            _applianceConnection.SendStorageQueryCommands ();
        }
    }
}

