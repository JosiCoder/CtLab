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

namespace CtLab.Environment
{
    public interface IApplianceConnection : IDisposable
    {
        /// <summary>
        /// Gets an object that can be used to synchronize access to the current object
        /// and its internals.
        /// </summary>
        object SyncRoot
        { get; }

        /// <summary>
        /// Gets the connection used by this instance.
        /// </summary>
        IConnection Connection
        { get; }

        /// <summary>
        /// Sends all set commands that have modified values.
        /// </summary>
        void SendSetCommandsForModifiedValues();

        /// <summary>
        /// Starts sending the scheduled query commands periodically using the specified period.
        /// </summary>
        /// <param name="period">The period in milliseconds.</param>
        void StartSendingQueryCommands(long period);

        /// <summary>
        /// Stops sending the scheduled query commands periodically.
        /// </summary>
        void StopSendingQueryCommands();

        /// <summary>
        /// Sends the storage query commands.
        /// </summary>
        void SendStorageQueryCommands();
    }
}