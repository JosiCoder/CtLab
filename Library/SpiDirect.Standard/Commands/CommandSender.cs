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
using System.Collections.Generic;
using System.Globalization;
using CtLab.Messages.Interfaces;
using CtLab.SpiConnection.Interfaces;
using CtLab.SpiDirect.Interfaces;

namespace CtLab.SpiDirect.Standard
{
    /// <summary>
    /// Creates the SPI data for a set or query command class and sends that data.
    /// </summary>
    public class CommandSender : ISetCommandSender, IQueryCommandSender
    {
        private readonly ISpiSender _spiSender;
        private readonly Dictionary<byte, uint> _spiValues = new Dictionary<byte, uint> ();

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="spiSender">The sender used to send the data.</param>
        public CommandSender(ISpiSender spiSender)
        {
            _spiSender = spiSender;
        }

        /// <summary>
        /// Sends a set command including to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        public void Send(SetCommandClass commandClass)
        {
            var messageChannel = commandClass.Channel as MessageChannel;
            if (messageChannel == null)
            {
                throw new InvalidOperationException ("Message channel is not supported");
            }

            var spiValue = Convert.ToUInt32(commandClass.RawValue, CultureInfo.InvariantCulture);
            _spiValues [messageChannel.SpiAddress] = spiValue;
            _spiSender.Send(messageChannel.SpiAddress, spiValue);
        }

        /// <summary>
        /// Sends a query command to a c't Lab device.
        /// </summary>
        /// <param name="commandClass">The command class to send a command for.</param>
        public void Send(QueryCommandClass commandClass)
        {
            var messageChannel = commandClass.Channel as MessageChannel;
            if (messageChannel == null)
            {
                throw new InvalidOperationException ("Message channel is not supported");
            }

            // Send any value to receive a value from the same channel. To avoid destroying the value
            // previously sent, resend the previous value whenever possible.
            uint spiValue = 0;
            _spiValues.TryGetValue (messageChannel.SpiAddress, out spiValue);
            _spiSender.Send(messageChannel.SpiAddress, spiValue);
        }
    }
}