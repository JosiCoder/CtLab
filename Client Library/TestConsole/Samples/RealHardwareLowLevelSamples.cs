﻿//------------------------------------------------------------------------------
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
using System.Linq;
using System.Text;
using System.Threading;
using StructureMap;
using CtLab.Connection.Interfaces;
using CtLab.Connection.Serial;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.BasicIntegration;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some low-level samples using real c´t Lab hardware connected via a (physical or emulated)
    /// serial port.
    /// These low-level samples are useful to understand how the CtLab library works internally,
    /// developers just using this library can safely ignore these samples.
    /// </summary>
    public static class RealHardwareLowLevelSamples
    {
        private const string _portName = "/dev/ttyUSB0";
        private const byte _channel = 7;

        private const int _queryCommandSendPeriod = 500; // ms

        /// <summary>
        /// Sends a raw command string to the c't lab.
        /// </summary>
        public static void SendRawCommandString()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            // Using the serial connection the c't lab is attached to.
            using (var connection = container.GetInstance<SerialConnection>())
            {
                connection.Open(_portName);

                // Send a raw command string to the c't lab.
                var sendString = string.Format("{0}:1=255", _channel);
                Console.WriteLine("Sending string: \"{0}\"", sendString);
                connection.Send(sendString);
            }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Sends a command to the c't lab.
        /// </summary>
        public static void SendCommandAndReceiveMessages()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            // Using the serial connection the c't lab is attached to.
            using (var connection = container.GetInstance<SerialConnection>())
            {
                connection.Open(_portName);

                // Send a command via the configured string sender.
                var setCommandDictionary = container.GetInstance<ISetCommandClassDictionary>();
                var setCommandClass = new SetCommandClass(_channel, 1);
                setCommandDictionary.Add(setCommandClass);
                setCommandClass.SetValue(128);
                Console.WriteLine("Sending command, channel {0}/{1}, raw value {2}",
                                  setCommandClass.Channel,
                                  setCommandClass.Subchannel,
                                  setCommandClass.RawValue);
                setCommandDictionary.SendCommandsForModifiedValues();

                // Prepare to receive messages of subchannel 255 via the configured string receiver.
                var messageCache = container.GetInstance<IMessageCache>();
                var messageContainer = messageCache.Register(_channel, 255);
                messageContainer.MessageUpdated +=
                    (sender, e) => Console.WriteLine("Message received, channel {0}/{1}, raw value {2}",
                                                     messageContainer.Message.Channel,
                                                     messageContainer.Message.Subchannel,
                                                     messageContainer.Message.RawValue);

                Console.WriteLine("For the next seconds, operate the c´t Lab panels to generate messages...");

                // Wait a little to receive messages.
                Thread.Sleep(10000);
            }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Configures and returns an IoC using a serial connection. The resulting configuration can be used for tests
        /// and samples that need real c´t Lab hardware connected to a (physical or emulated) serial port.
        /// </summary>
        private static Container ConfigureIoC()
        {
            return Utilities.ConfigureIoC<SerialConnectionRegistry>();
        }
    }
}
