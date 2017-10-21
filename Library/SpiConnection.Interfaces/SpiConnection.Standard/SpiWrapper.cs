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
using CtLab.SpiConnection.Interfaces;

namespace CtLab.SpiConnection.Standard
{
    /// <summary>
    /// Provides a dummy connection that actually isn't a real connection.
    /// </summary>
    public class SpiWrapper : ISpiSender, ISpiReceiver
    {
        /// <summary>
        /// Occurs when a value has been received from an SPI slave. Note that
        /// this event might be called via a background thread.
        /// </summary>
        public event EventHandler<SpiReceivedEventArgs> ValueReceived;

        /// <summary>
        /// Sends a value to an SPI slave.
        /// </summary>
        /// <param name="spiAddress">The SPI address to sent the value to.</param>
        /// <param name="valueToSend">The value to be sent.</param>
        public void Send(byte spiAddress, uint valueToSend)
        {
//TODO: Send and receive from SPI via Interop, then raise received event.
            throw new NotImplementedException();

            ValueReceived.Raise(this, new SpiReceivedEventArgs(spiAddress, valueToSend));
        }
    }
}

