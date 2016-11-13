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
using CtLab.Utilities;
using CtLab.Connection.Interfaces;

namespace CtLab.Connection.Serial
{
    /// Provides a string receiver that uses a serial connection.
    public class SerialStringReceiver : IStringReceiver
    {
        private readonly SerialConnection _connection;
        private readonly object _anyEventRegistrationLock = new object();
        private event EventHandler<StringReceivedEventArgs> _stringReceived;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="connection">The connection used by this instance.</param>
        public SerialStringReceiver(SerialConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Occurs when a string has been received from a c't Lab device. Note that
        /// this event might be called via a background thread.
        /// </summary>
        public event EventHandler<StringReceivedEventArgs> StringReceived
        {
            add
            {
                lock (_anyEventRegistrationLock)
                {
                    _connection.StringReceived += StringReceivedFromConnection;
                    _stringReceived += value;
                }
            }
            remove
            {
                lock (_anyEventRegistrationLock)
                {
                    _stringReceived -= value;
                    _connection.StringReceived -= StringReceivedFromConnection;
                }
            }
        }

        /// <summary>
        /// Performs actions whenever a string has been received.
        /// </summary>
        /// <remarks>
        /// Usually this will be called on the thread activated for the receive operation.
        /// </remarks>
        private void StringReceivedFromConnection(object sender, StringReceivedEventArgs e)
        {
            _stringReceived.Raise(this, e);
        }
    }
}