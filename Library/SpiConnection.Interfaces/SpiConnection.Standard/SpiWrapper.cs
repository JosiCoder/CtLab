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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CtLab.Utilities;
using CtLab.SpiConnection.Interfaces;

namespace CtLab.SpiConnection.Standard
{
    /// <summary>
    /// Provides a dummy connection that actually isn't a real connection.
    /// </summary>
    public class SpiWrapper : ISpiSender, ISpiReceiver
    {
        private const int _receiveBufferSize = 32;
        private const string _addressDevice = "/dev/spidev0.0";
        private const string _dataDevice = "/dev/spidev0.1";

        [DllImport ("interop")]
        private static extern int transfer_spi_data(string device, byte[] tx_buf, byte[] rx_buf,
            int len, StringBuilder error_string_buffer, int error_string_maxlen);

        /// <summary>
        /// Transfers data to and from the specified SPI device.
        /// </summary>
        /// <param name="device">The SPI device to transfer the data to and from.</param>
        /// <param name="sendData">The data to send.</param>
        /// <returns>The data received.</returns>
        private static IEnumerable<byte> TransferRaw(string device, IEnumerable<byte> sendData)
        {
            var receiveBuffer = new byte[_receiveBufferSize];

            int transmissionLength = sendData.Count();

            if (transmissionLength > _receiveBufferSize)
            {
                throw new InvalidOperationException("SPI transmission length must not exeed receive buffer size.");
            }

            var errorStringBuilder = new StringBuilder(100);
            var ret = transfer_spi_data(device, sendData.ToArray(), receiveBuffer, transmissionLength,
                errorStringBuilder, errorStringBuilder.Capacity);

            if (ret < 0)
            {
                throw new ApplicationException(string.Format("SPI transmission failed: {0}", errorStringBuilder));
            }
            return receiveBuffer.Take(transmissionLength);         
        }

        /// <summary>
        /// Transfers data to and from the specified SPI address.
        /// </summary>
        /// <param name="spiAddress">The SPI address to transfer the data to and from.</param>
        /// <param name="sendData">The data to send.</param>
        /// <returns>The data received.</returns>
        private static IEnumerable<byte> Transfer(byte spiAddress, IEnumerable<byte> sendData)
        {
            TransferRaw(_addressDevice, new []{spiAddress});
            return TransferRaw(_dataDevice, sendData);
        }

        /// <summary>
        /// Converts the contents of the specified data buffer to a string representation.
        /// </summary>
        /// <param name="buffer">The data buffer to convert the contents from.</param>
        /// <returns>The string representing the data buffer contents.</returns>
        private static string ConvertBufferToString(IEnumerable<byte> buffer)
        {
            var builder = new StringBuilder();
            foreach (byte b in buffer)
            {
                builder.AppendFormat("{0:X2} ", b);
            }
            return builder.ToString().Trim();
        }

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
            var sendData = BitConverter.GetBytes(valueToSend);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(sendData);
            }
            var sendBytesString = ConvertBufferToString(sendData);
            Debug.WriteLine("Value sent to SPI address {0}: {1} ({2})",
                spiAddress, valueToSend, sendBytesString);

            // Send and receive to and from SPI via interop library.
            var receivedData = Transfer(spiAddress, sendData).ToArray();

            var receiveBytesString = ConvertBufferToString(receivedData);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(receivedData);
            }
            var valueReceived = BitConverter.ToUInt32(receivedData, 0);
            Debug.WriteLine("Value received from SPI address {0}: {1} ({2})",
                spiAddress, valueReceived, receiveBytesString);

            ValueReceived.Raise(this, new SpiReceivedEventArgs(spiAddress, valueReceived));
        }
    }
}

