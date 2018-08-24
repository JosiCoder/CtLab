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
using System.Diagnostics;
using CtLab.Connection.Interfaces;

namespace CtLab.Connection.Serial
{
    /// <summary>
    /// Provides a string sender that uses a serial connection.
    /// </summary>
    public class SerialStringSender : IStringSender
    {
        private readonly SerialConnection _connection;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="connection">The connection used by this instance.</param>
        public SerialStringSender(SerialConnection connection)
        {
            _connection = connection; 
        }

        /// <summary>
        /// Sends a string to a c't Lab device.
        /// </summary>
        /// <param name="stringToSend">The string to be sent.</param>
        public void Send(string stringToSend)
        {
            lock (_connection)
            {
                _connection.Send(stringToSend);
                Debug.WriteLine("String sent: {0}", new []{stringToSend}); // enforce the correct overload
            }
        }
    }
}