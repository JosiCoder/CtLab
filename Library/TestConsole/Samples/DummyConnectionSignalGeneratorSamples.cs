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
using CtLab.CtLabProtocol.Interfaces;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Environment;
using CtLab.CtLabProtocol.Integration;
using CtLab.EnvironmentIntegration;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some samples for a simulated dummy connection.
    /// No real c´t Lab hardware is necessary to run these samples.
    /// </summary>
    public class DummyConnectionSignalGeneratorSamples
    {
        private const int _queryCommandSendPeriod = 500; // ms

        /// <summary>
        /// Handles a specific c´t Lab appliance via a simulated dummy connection. The Appliance reflects
        /// the current c´t Lab hardware configuration, i.e. the c´t Lab devices used and the channels
        /// assigned to them.
        /// In this example, the appliance assumed consists of only one FPGA Lab instance (i.e. one c´t Lab
        /// FPFA device configured as an FPGA Lab).
        /// No real c´t Lab hardware is necessary to run this sample.
        /// </summary>
        public void HandleSampleCtLabAppliance()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            using (var appliance = container.GetInstance<Appliance>())
            {
                var applianceConnection = appliance.ApplianceConnection;

                // Set the channel of the appliance´s only FPGA lab.
                appliance.InitializeCtLabProtocol(5, true, false);

                // Change some sample settings of the signal generator. This results in modifying the values
                // of set command classes and sending all the commands that have modified values.
                var signalGenerator = appliance.SignalGenerator;
                signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator3;
                signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator2;
                signalGenerator.DdsGenerators [0].AmplitudeModulationSource = ModulationAndSynchronizationSource.DdsGenerator1;
                signalGenerator.DdsGenerators [1].AmplitudeModulationSource = ModulationAndSynchronizationSource.DdsGenerator2;
                signalGenerator.DdsGenerators [0].Amplitude = 1000;
                signalGenerator.DdsGenerators [1].Amplitude = 100;
                applianceConnection.SendSetCommandsForModifiedValues ();

                // Display some values resulting from the current settings.
                Console.WriteLine ("DDS channel 0: AM {0}%, {1}overmodulated",
                    signalGenerator.DdsGeneratorsAMInformationSets [0].RelativeModulationDepth * 100,
                    signalGenerator.DdsGeneratorsAMInformationSets [0].Overmodulated ? "" : "not ");

                // Send query commands periodically for some seconds.
                applianceConnection.StartSendingQueryCommands (_queryCommandSendPeriod);
                Thread.Sleep (5 * _queryCommandSendPeriod);
                applianceConnection.StopSendingQueryCommands ();
                Thread.Sleep (2 * _queryCommandSendPeriod); // wait for all pending 

                Utilities.WriteFooterAndWaitForKeyPress ();
            }
        }

        /// <summary>
        /// Configures and returns an IoC using a dummy connection. The resulting configuration can be used for tests
        /// and samples that don´t need real c´t Lab hardware.
        /// </summary>
        private static Container ConfigureIoC()
        {
            return ApplianceFactory.CreateContainer<DummyConnectionRegistry, CtLabProtocolRegistry>();
        }
    }
}
