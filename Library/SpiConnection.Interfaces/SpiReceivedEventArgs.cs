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

namespace CtLab.SpiConnection.Interfaces
{
    /// <summary>
    /// Provides data for the SpiReceived event.
    /// </summary>
    public class SpiReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="spiAddress">The SPI address the event belongs to.</param>
        /// <param name="receivedValue">The received value.</param>
        public SpiReceivedEventArgs(byte spiAddress, uint receivedValue)
        {
            SpiAddress = spiAddress;
            ReceivedValue = receivedValue;
        }

        /// <summary>
        /// Gets or sets the SPI address the event belongs to.
        /// </summary>
        public byte SpiAddress;

        /// <summary>
        /// Gets or sets the received value.
        /// </summary>
        public uint ReceivedValue;
    }
}