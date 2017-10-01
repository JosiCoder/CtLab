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
using System.Linq;
using System.Text;
using System.Threading;
using StructureMap;
using CtLab.Connection.Interfaces;
using CtLab.Connection.Dummy;
using CtLab.Messages.Interfaces;
using CtLab.CtLabProtocol.Interfaces;
using CtLab.BasicIntegration;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some low-level samples for a simulated dummy connection.
    /// These low-level samples are useful to understand how the CtLab library works internally,
    /// developers just using this library can safely ignore these samples.
    /// No real c´t Lab hardware is necessary to run these samples.
    /// </summary>
    public static class DummyConnectionLowLevelSamples
    {
        private const int _queryCommandSendPeriod = 500; // ms

        /// <summary>
        /// Simulates sending a command and receiving a message via a simulated dummy connection.
        /// No real c´t Lab hardware is necessary to run this sample.
        /// </summary>
        public static void SendCommandAndReceiveInjectedMessage()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            // Send a command via the configured dummy string sender.
            var setCommandDictionary = container.GetInstance<ISetCommandClassDictionary>();
            var setCommandChannel = new MessageChannel(1, 11);
            var setCommandClass = new SetCommandClass(setCommandChannel);
            setCommandDictionary.Add(setCommandClass);
            setCommandClass.SetValue(15);
            Console.WriteLine("Sending command, channel {0}/{1}, raw value {2}",
                setCommandChannel.Main,
                setCommandChannel.Sub,
                setCommandClass.RawValue);
            setCommandDictionary.SendCommandsForModifiedValues();

            // Prepare to receive messages via the configured dummy string receiver.
            var messageCache = container.GetInstance<IMessageCache>();
            var messageChannel = new MessageChannel(7, 255);
            var messageContainer = messageCache.Register(messageChannel);
            var typedMessage = messageContainer.Message as Message;
            messageContainer.MessageUpdated +=
                (sender, e) => Console.WriteLine("Message received, channel {0}/{1}, raw value {2}",
                    messageChannel.Main,
                    messageChannel.Sub,
                    typedMessage != null ? typedMessage.RawValue : "?");

            // Simulate receiving a message by injecting it into the configured dummy string receiver.
            ((DummyStringReceiver)container.GetInstance<IStringReceiver>())
                .InjectReceivedString("#7:255=7 [CHKSUM]");

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Sends periodic query commands via a simulated dummy connection.
        /// No real c´t Lab hardware is necessary to run this sample.
        /// </summary>
        public static void SendPeriodicQueryCommands()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            // Prepare a query command class and add it to the dictionary maintained
            // by the scheduler.
            var commandScheduler = container.GetInstance<IQueryCommandScheduler>();
            var queryCommandClass = new QueryCommandClass(new MessageChannel(1, 4));
            commandScheduler.CommandClassDictionary.Add(queryCommandClass);

            // Start sending query commands periodically via the configured dummy string
            // sender. Sending is done asynchronously on a separate thread.
            Console.WriteLine("Start sending query commands.");
            commandScheduler.StartSending(_queryCommandSendPeriod);

            // Wait a little.
            Thread.Sleep(10 * _queryCommandSendPeriod);

            // Stop sending query commands.
            Console.WriteLine("Stop sending query commands.");
            commandScheduler.StopSending();

            // Wait a little to show that sending really stops.
            Thread.Sleep(5 * _queryCommandSendPeriod);

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Configures and returns an IoC using a dummy connection. The resulting configuration can be used for tests
        /// and samples that don´t need real c´t Lab hardware.
        /// </summary>
        private static Container ConfigureIoC()
        {
            return Utilities.ConfigureIoC<DummyConnectionRegistry>();
        }
    }
}
