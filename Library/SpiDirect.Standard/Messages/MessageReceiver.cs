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
using CtLab.Messages.Standard;
using CtLab.SpiConnection.Interfaces;
using CtLab.SpiDirect.Interfaces;

namespace CtLab.SpiDirect.Standard
{
    /// <summary>
    /// Converts received SPI values to one or more messages, and raises one event per message.
    /// </summary>
    public class MessageReceiver : MessageReceiverBase
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="spiReceiver">The receiver used to wait for SPI data.</param>
        public MessageReceiver(ISpiReceiver spiReceiver)
        {
            spiReceiver.ValueReceived +=
                (sender, e) =>
                {
                    var message = new Message
                        (
                            new MessageChannel
                            (
                                e.SpiAddress
                            ),
                            e.ReceivedValue
                        );
                    RaiseMessageReceived(this, new MessageReceivedEventArgs(message));
                };
        }
    }
}