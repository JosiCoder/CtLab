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
using CtLab.Messages.Interfaces;
using CtLab.Connection.Interfaces;
using CtLab.FpgaConnection.Interfaces;
using CtLab.Environment;

namespace CtLab.EnvironmentIntegration
{
    /// <summary>
    /// Provides access to an appliance, based on c't Lab set and query commands.
    /// </summary>
    public class CtLabApplianceConnection : IApplianceConnection
    {
        private readonly IConnection _connection;

        /// <summary>
        /// The command class dictionary used to send the set commands.
        /// </summary>
        private readonly ISetCommandClassDictionary _setCommandClassDictionary;

        /// <summary>
        /// The command scheduler used to send the query commands.
        /// </summary>
        private readonly IQueryCommandScheduler _queryCommandScheduler;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the current object
        /// and its internals.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                var syncRoot = new object();
                if (_queryCommandScheduler != null)
                {
                    syncRoot = _queryCommandScheduler.SyncRoot;
                }
                return syncRoot;
            }
        }

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
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="connection">The connection used by this instance.</param>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        public CtLabApplianceConnection(IConnection connection,
            ISetCommandClassDictionary setCommandClassDictionary, IQueryCommandScheduler queryCommandScheduler)
        {
            _connection = connection;
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandScheduler = queryCommandScheduler;
        }

        /// <summary>
        /// Sends all set commands that have modified values.
        /// </summary>
        public void SendSetCommandsForModifiedValues()
        {
            _setCommandClassDictionary.SendCommandsForModifiedValues();
        }

        /// <summary>
        /// Starts sending the scheduled query commands periodically using the specified period.
        /// </summary>
        /// <param name="period">The period in milliseconds.</param>
        public void StartSendingQueryCommands(long period)
        {
            if (_queryCommandScheduler != null)
                _queryCommandScheduler.StartSending(classGroup => classGroup is IPeriodicCommandClassGroup, period);
        }

        /// <summary>
        /// Stops sending the scheduled query commands periodically.
        /// </summary>
        public void StopSendingQueryCommands()
        {
            if (_queryCommandScheduler != null)
                _queryCommandScheduler.StopSending();
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }
    }
}