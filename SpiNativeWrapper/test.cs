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

namespace SpiDemo.Examples
{
    public class Spi
    {
        private const int _receiveBufferSize = 32;
        private const string _addressDevice = "/dev/spidev0.0";
        private const string _dataDevice = "/dev/spidev0.1";

        [DllImport ("interop")]
        private static extern int transfer_spi_data(string device, byte[] tx_buf, byte[] rx_buf,
            int len, StringBuilder error_string_buffer, int error_string_maxlen);
     
        public static IEnumerable<byte> TransferRaw(string device, IEnumerable<byte> sendData)
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
         
        public static IEnumerable<byte> Transfer(byte address, IEnumerable<byte> sendData)
        {
            TransferRaw(_addressDevice, new []{address});
            return TransferRaw(_dataDevice, sendData);
        }
    }
    
    public class Test
    {
        private static void PrintBuffer(string caption, IEnumerable<byte> buffer)
        {
            Console.Write("{0}: ", caption);
            foreach (byte b in buffer)
            {
                System.Console.Write("{0:X2} ", b);
            }
            Console.WriteLine();
        }

        private static void TransferRaw()
        {
            var sendData = new byte[]
            {
                0x00, 0x55, 0x00, 0x44,
            };

            PrintBuffer("sending", sendData);
            var receivedData = Spi.TransferRaw("/dev/spidev0.1", sendData);
            PrintBuffer("received", receivedData);
        }
     
        private static void TransferToAddress()
        {
            byte address = 0x05;
            var sendData = new byte[]
            {
                0x00, 0x55, 0x00, 0x44,
            };

            PrintBuffer(string.Format("sending to address {0:X2}", address), sendData);
            var receivedData = Spi.Transfer(address, sendData);
            PrintBuffer("received", receivedData);
        }
     
        public static void Main(string[] args)
        {
            //TransferRaw();
            //Console.WriteLine();
            TransferToAddress();
        }
    } 
}
