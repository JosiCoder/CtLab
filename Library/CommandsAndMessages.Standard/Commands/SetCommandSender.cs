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
using CtLab.CommandsAndMessages.Interfaces;

namespace CtLab.CommandsAndMessages.Standard
{
    /// <summary>
    /// Creates a command string for a set command class and sends that string to a c't Lab device.
    /// </summary>
    public class SetCommandSender : ISetCommandSender<MessageChannel>
    {
        private ISetCommandStringBuilder<MessageChannel> _stringBuilder;
        private IStringSender _stringSender;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="stringBuilder">
        /// The string builder used to build the command string.
        /// </param>
        /// <param name="stringSender">The sender used to send the string.</param>
        public SetCommandSender(ISetCommandStringBuilder<MessageChannel> stringBuilder, IStringSender stringSender)
        {
            _stringBuilder = stringBuilder;
            _stringSender = stringSender;
        }

        /// <summary>
        /// Sends a set command including a checksum to a c't Lab device. Doesn't request an
        /// achnowledge.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        public void Send(SetCommandClass<MessageChannel> commandClass)
        {
            Send(commandClass, true, false);
        }

        /// <summary>
        /// Sends a set command to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        /// <param name="generateChecksum">true to append the checksum; otherwiese, false.</param>
        /// <param name="requestAcknowledge">
        /// true to request an acknowledge from the receiver; otherwiese, false.
        /// </param>
        public void Send(SetCommandClass<MessageChannel> commandClass, bool generateChecksum, bool requestAcknowledge)
        {
            _stringSender.Send(_stringBuilder.BuildCommand(commandClass, generateChecksum, requestAcknowledge));
        }
    }
}