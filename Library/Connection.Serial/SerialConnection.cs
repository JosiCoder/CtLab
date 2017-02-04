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
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CtLab.Utilities;
using CtLab.Connection.Interfaces;

namespace CtLab.Connection.Serial
{
    /// <summary>
    /// Provides a connection using a serial port.
    /// </summary>
    public class SerialConnection : IConnection
    {
        private SerialPort _serialPort;
        private readonly object _anyEventRegistrationLock = new object();
        private event EventHandler<StringReceivedEventArgs> _stringReceived;

        private System.Threading.Thread _readThread;
        private volatile bool _cancelReadThread = false;

        /// <summary>
        /// Gets a value indicating whether the connection is active.
        /// </summary>
        public bool IsActive
        { get; private set; }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="portName">
        /// The name of the serial port to connect to.
        /// </param>
        public SerialConnection()
        {
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <param name="portName">The name of the serial port to use.</param>
        public void Open(string portName)
        {
            _serialPort = new SerialPort(portName, 38400, Parity.None, 8, StopBits.One);
            _serialPort.ReadTimeout = 1000; // ms to wait
            _serialPort.WriteTimeout = 1000; // ms to wait
            _serialPort.Encoding = new ASCIIEncoding();
            _serialPort.NewLine = "\r\n";
            if (!IsNonWindows)
            {
                _serialPort.DataReceived += DataReceived;
            }
            _serialPort.Open();
            if (IsNonWindows)
            {
                _serialPort.ReadTimeout = 500;
                _readThread = new System.Threading.Thread(() => DoSerialRead ());
                _readThread.Start ();
            }

            IsActive = true;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            IsActive = false;

            if (_readThread != null)
            {
                _cancelReadThread = true;
                _readThread.Join();
                _readThread = null;
                _cancelReadThread = false;
            }

            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.DataReceived -= DataReceived;
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        /// <summary>
        /// Sends a string to the connection.
        /// </summary>
        /// <param name="stringToSend">The string to be sent.</param>
        public void Send(string stringToSend)
        {
            try
            {
                _serialPort.WriteLine(stringToSend);
            }
            catch
            {
                IsActive = false;
                throw;
            }
        }

        /// <summary>
        /// Occurs when a string has been received from the connection. Note that
        /// this event might be called via a background thread.
        /// </summary>
        public event EventHandler<StringReceivedEventArgs> StringReceived
        {
            add
            {
                lock (_anyEventRegistrationLock)
                    _stringReceived += value;
            }
            remove
            {
                lock (_anyEventRegistrationLock)
                    _stringReceived -= value;
            }
        }

        /// <summary>
        /// Performs actions whenever data have been received.
        /// </summary>
        /// <remarks>
        /// Usually this will be called on the thread activated for the receive operation.
        /// </remarks>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Read an entire line and forward it to objects listening to this connection.
                var receivedString = _serialPort.ReadLine();
                _stringReceived.Raise(this, new StringReceivedEventArgs(receivedString));
            }
            catch {}
        }

        private static bool IsNonWindows
        {
            get
            {
                var id = Environment.OSVersion.Platform;
                return id == PlatformID.Unix || id == PlatformID.MacOSX;
            }
        }

        /// <summary>
        /// Reads incoming bytes and raises a StringReceived event as soon as a line is complete.
        /// </summary>
        private void DoSerialRead()
        {
            var lineTermination = _serialPort.NewLine;
            var stringBuilder = new StringBuilder();

            while (true)
            {
                try
                {
                    if (_cancelReadThread)
                    {
                        break;
                    }
                    var byteRead = (byte)_serialPort.ReadByte();
                    stringBuilder.Append(Convert.ToChar(byteRead));
                    var stringRead = stringBuilder.ToString();
                    if (stringRead.EndsWith(lineTermination))
                    {
                        _stringReceived.Raise(this, new StringReceivedEventArgs(stringRead.Substring(0, stringRead.Length - lineTermination.Length)));
                        stringBuilder.Clear();
                    }
                }
                catch (TimeoutException)
                {
                    ; // continue reading...
                }
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
