//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

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
using System.Threading;
using System.Threading.Tasks;
using StructureMap;
using CtLab.Connection.Interfaces;
using CtLab.Connection.Dummy;
using CtLab.Connection.Serial;
using CtLab.Environment;
using CtLab.BasicIntegration;
using CtLab.CtLabProtocol.Integration;
using CtLab.SpiDirect.Integration;
using CtLab.EnvironmentIntegration;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Creates appliances.
    /// </summary>
    public class ApplianceFactory
    {
        private const string _portName = "/dev/ttyUSB0";
        private const byte _channel = 7;

        /// <summary>
        /// Configures and returns an IoC using the specified connection registry.
        /// </summary>
        /// <returns>The configured IoC.</returns>
        /// <typeparam name="TConnectionRegistry">The type of the registry responsible for the connection.</typeparam>
        /// <typeparam name="TProtocolRegistry">The type of the registry responsible for the protocol.</typeparam>
        public static Container CreateContainer<TConnectionRegistry, TProtocolRegistry>()
            where TConnectionRegistry : Registry, new()
            where TProtocolRegistry : Registry, new()
        {
            // Configure the IoC container to provide specific implementations for several interfaces.
            var container = new Container (expression =>
                {
                    expression.AddRegistry<CommandsAndMessagesRegistry> ();
                    expression.AddRegistry<ApplianceRegistry> ();
                    expression.AddRegistry<TConnectionRegistry> ();
                    expression.AddRegistry<TProtocolRegistry> ();
                });

            // Display the effecive IoC container configuration.
            // Note: This line is not needed for proper operation, it just provides some information
            // that helps to understand how the IoC container works. Thus this line may be deactivated.
            //System.Diagnostics.Debug.WriteLine(container.WhatDoIHave());

            return container;
        }

        /// <summary>
        /// Creates and returns a new c't Lab appliance used for testing.
        /// </summary>
        /// <returns>The appliance created.</returns>
        public Appliance CreateTestAppliance()
        {
            //TODO: set interface to use
            return CreateSerialAppliance(_portName, _channel);
//            return CreateSpiAppliance();
        }

        /// <summary>
        /// Creates and returns a new c't Lab appliance.
        /// </summary>
        /// <returns>The appliance created.</returns>
        public Appliance CreateSpiAppliance()
        {
            // Each c't Lab appliance instance needs its own IoC container. 
            var container = CreateContainer<SpiConnectionRegistry, SpiDirectRegistry>();
            var appliance = container.GetInstance<Appliance>();

            appliance.InitializeSpiDirect(true, true); //TODO: set device to use
            return appliance;
        }


        /// <summary>
        /// Creates and returns a new c't Lab appliance.
        /// </summary>
        /// <param name="portName">The name of the serial port to use.</param>
        /// <param name="channel">
        /// The number of the channel to send to and receive from.
        /// </param>
        /// <returns>The appliance created.</returns>
        public Appliance CreateSerialAppliance(string portName, byte channel)
        {
            // Each c't Lab appliance instance needs its own IoC container. 
            var container = CreateContainer<SerialConnectionRegistry, CtLabProtocolRegistry>();
            var appliance = container.GetInstance<Appliance>();

            ((SerialConnection)appliance.ApplianceConnection.Connection).Open(portName);

            appliance.InitializeCtLabProtocol(channel, true, true); //TODO: set device to use
            return appliance;
        }


        /// <summary>
        /// Creates and returns a new dummy c't Lab appliance.
        /// Sample strings are injected to that connection to simulate reception of
        /// such strings.
        /// </summary>
        /// <param name="channel">
        /// The number of the channel to send to and receive from.
        /// </param>
        /// <returns>The appliance created.</returns>
        public Appliance CreateDummyAppliance(byte channel)
        {
            return CreateDummyAppliance (channel, injector =>
                {
                    //                Task.Run(() =>
                    //                    {
                    //                        int counterRawValue = 1000;
                    //                        int counterState = 0x1;
                    //                        while (true)
                    //                        {
                    //                            Thread.Sleep(500);
                    //
                    //                            // Simulate receiving messages by injecting them into
                    //                            // the configured dummy string receiver.
                    //
                    //                            injector(string.Format("#7:5={0}", counterRawValue));
                    //                            counterRawValue = counterRawValue == 1000000000 ? 1000 : 1000000000;
                    //                            //counterRawValue = counterRawValue == 500 ? 1000 : 500;
                    //
                    //                            injector(string.Format("#7:4={0}", counterState));
                    //                            counterState = counterState == 0x2 ? 0x1 : 0x2;
                    //                        }
                    //                    });
                });
        }

        /// <summary>
        /// Creates and returns a new dummy c't Lab appliance.
        /// </summary>
        /// <param name="injectorAcceptor">An action used to set the injection method.</param>
        /// <param name="channel">
        /// The number of the channel assigned to the FPGA Lab device.
        /// </param>
        /// <returns>The appliance created.</returns>
        private Appliance CreateDummyAppliance(byte channel, Action<Action<string>> injectorAcceptor)
        {
            // Each c't Lab appliance instance needs its own IoC container. 
            var container = CreateContainer<DummyConnectionRegistry, CtLabProtocolRegistry>();
            var appliance = container.GetInstance<Appliance>();

            injectorAcceptor (stringToInject =>
                {
                    ((DummyStringReceiver)container.GetInstance<IStringReceiver>())
                        .InjectReceivedString(stringToInject);
                });

            appliance.InitializeCtLabProtocol(channel, true, false);
            return appliance;
        }
    }
}

