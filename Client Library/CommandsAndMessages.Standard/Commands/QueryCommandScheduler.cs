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
using System.Threading;
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Schedules query commands for classes contained in a query command class dictionary,
    /// i.e. sends them periodically.
    /// </summary>
    public class QueryCommandScheduler : IQueryCommandScheduler, IDisposable
    {
        private readonly IQueryCommandClassDictionary _commandClassDictionary;
        private readonly object _syncRoot = new object();
        private Timer _scheduleTimer;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the underlying query
        /// command class dictionary.
        /// </summary>
        public object SyncRoot { get { return _syncRoot; } }

        /// <summary>
        /// Gets the underlying dictionary holding the command classes to send commands for.
        /// When scheduling is activated, this dictionary will be periodically accessed
        /// asynchronously, i.e. from a different thread.
        /// </summary>
        public IQueryCommandClassDictionary CommandClassDictionary { get { return _commandClassDictionary; } }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="commandClassDictionary">
        /// The dictionary holding the command classes to send commands for.
        /// </param>
        public QueryCommandScheduler(IQueryCommandClassDictionary commandClassDictionary)
        {
            _commandClassDictionary = commandClassDictionary;
        }

        /// <summary>
        /// Sends all scheduled commands immediately.
        /// </summary>
        public void SendImmediately()
        {
            lock (SyncRoot)
            {
                _commandClassDictionary.SendCommands();
            }
        }

        /// <summary>
        /// Starts sending the scheduled commands periodically using the specified period.
        /// This will periodically access the underlying command class dictionary asynchronously,
        /// i.e. from a different thread.
        /// </summary>
        /// <param name="period">The period in milliseconds.</param>
        public void StartSending(long period)
        {
            StopSending();

            _scheduleTimer =
                new Timer(
                    state => SendImmediately(),
                    null, 0, period);
        }

        /// <summary>
        /// Stops sending the scheduled commands periodically.
        /// </summary>
        public void StopSending()
        {
            if (_scheduleTimer != null)
                _scheduleTimer.Dispose();

        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            StopSending();
        }
    }
}