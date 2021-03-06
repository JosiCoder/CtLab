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
using CtLab.Messages.Interfaces;
using CtLab.Messages.Standard;
using CtLab.Connection.Interfaces;
using CtLab.CtLabProtocol.Interfaces;

namespace CtLab.CtLabProtocol.Standard
{
    /// <summary>
    /// Listens to c't Lab devices and, for each string received, converts that string to one or
    /// more messages, and raises one event per message.
    /// </summary>
    public class MessageReceiver : MessageReceiverBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="messageParser">
        /// The string parser used to parse the message string.
        /// </param>
        /// <param name="stringReceiver">The receiver used to wait for message strings.</param>
        public MessageReceiver(IMessageParser messageParser, IStringReceiver stringReceiver)
        {
            stringReceiver.StringReceived +=
                (sender, e) =>
                {
                    // Parse the received string to get one or more messages,
                    // raise an event for each message.
                    foreach (var message in messageParser.Parse(e.ReceivedString))
                    {
                        RaiseMessageReceived(this, new MessageReceivedEventArgs(message));
                    }
                };
        }
    }
}