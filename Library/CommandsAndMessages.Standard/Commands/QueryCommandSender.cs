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

using CtLab.Connection.Interfaces;
using CtLab.Messages.Interfaces;
using CtLab.CtLabProtocol.Interfaces;

namespace CtLab.CtLabProtocol.Standard
{
    /// <summary>
    /// Creates a command string for a query command class and sends that string to a c't Lab device.
    /// </summary>
    public class QueryCommandSender : IQueryCommandSender
    {
        private IQueryCommandStringBuilder _stringBuilder;
        private IStringSender _stringSender;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="stringBuilder">
        /// The string builder used to build the command string.
        /// </param>
        /// <param name="stringSender">The sender used to send the string.</param>
        public QueryCommandSender(IQueryCommandStringBuilder stringBuilder, IStringSender stringSender)
        {
            _stringBuilder = stringBuilder;
            _stringSender = stringSender;
        }

        /// <summary>
        /// Sends a query command including a checksum to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        public void Send(QueryCommandClass commandClass)
        {
            Send(commandClass, true);
        }

        /// <summary>
        /// Sends a query command to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        public void Send(QueryCommandClass commandClass, bool generateChecksum)
        {
            _stringSender.Send(_stringBuilder.BuildCommand(commandClass, generateChecksum));
        }
    }
}