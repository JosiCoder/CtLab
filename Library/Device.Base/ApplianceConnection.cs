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

using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.Device.Base
{
    /// <summary>
    /// Provides access to an appliance, based on c't Lab set and query commands.
    /// </summary>
    public class ApplianceConnection
    {
        /// <summary>
        /// The command class dictionary used to send the set commands.
        /// </summary>
        private readonly ISetCommandClassDictionary _setCommandClassDictionary;

        /// <summary>
        /// The command scheduler used to send the query commands.
        /// </summary>
        private readonly IQueryCommandScheduler _queryCommandScheduler;

        /// <summary>
        /// The message cache used to receive the messages.
        /// </summary>
        private readonly IMessageCache _receivedMessagesCache;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the underlying query
        /// command class dictionary.
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
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="setCommandClassDictionary">The command class dictionary used to send the set commands.</param>
        /// <param name="queryCommandScheduler">The scheduler used to send the query commands.</param>
        /// <param name="receivedMessagesCache">The message cache used to receive the messages.</param>
        public ApplianceConnection(ISetCommandClassDictionary setCommandClassDictionary, IQueryCommandScheduler queryCommandScheduler,
            IMessageCache receivedMessagesCache)
        {
            _setCommandClassDictionary = setCommandClassDictionary;
            _queryCommandScheduler = queryCommandScheduler;
            _receivedMessagesCache = receivedMessagesCache;
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
                _queryCommandScheduler.StartSending(period);
        }

        /// <summary>
        /// Stops sending the scheduled query commands periodically.
        /// </summary>
        public void StopSendingQueryCommands()
        {
            if (_queryCommandScheduler != null)
                _queryCommandScheduler.StopSending();
        }

        /// <summary>
        /// Sends all query commands immediately.
        /// </summary>
        public void SendQueryCommandsImmediately()
        {
            if (_queryCommandScheduler != null)
                _queryCommandScheduler.SendImmediately();
        }
    }
}