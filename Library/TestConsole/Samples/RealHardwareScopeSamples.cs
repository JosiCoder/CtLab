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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Environment;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some samples using real c´t Lab hardware connected via a (physical or emulated) serial
    /// port or a direct SPI connection.
    /// For these tests, a proper configuration supporting a scope must be loaded into the FPGA.
    /// </summary>
    public class RealHardwareScopeSamples
    {
        private readonly bool _spiDirect;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="spiDirect">A value indicating whether the hardware is directly accessed via the SPI interface.</param>
        public RealHardwareScopeSamples (bool spiDirect)
        {
            _spiDirect = spiDirect;
        }

        /// <summary>
        /// Writes sample values to the storage and reads them using the low-level SRAM controller protocol.
        /// Note: No handshake is usually necessary for writing as the storage controller (VHDL) is much faster
        /// than this software. For reading, the entire roundtrip time necessary to provide the read value has
        /// to be considered, especially when using the c't Lab protocol. 
        /// </summary>
        public void WriteAndReadStorageValues()
        {
            Utilities.WriteHeader();

            using (var appliance = new ApplianceFactory(_spiDirect).CreateTestAppliance())
            {
                // Setup some signals on the analog outputs so that we can see how accessing the
                // storage briefly deactivates the analog outputs.
                SetDemoSignals(appliance);

                // Get the scope and reset the hardware to cancel settings from previous configurations.
                var scope = appliance.Scope;
                scope.Reset();

                var separatorStartAddress = 999u;
                var separatorValues = new []{99u};

                var startAddress = 10u;
                var random = new Random ();
                var values = Enumerable.Range(0, 5).Select(item => (uint)random.Next (255)).ToArray();

                Console.WriteLine ("=====================================================");
                Console.WriteLine ("Writing values");
                Console.WriteLine ("=====================================================");

                var start = DateTime.Now;

                scope.Write(startAddress, values);

                Console.WriteLine ("Duration: {0}", DateTime.Now-start);

                Console.WriteLine ("=====================================================");
                Console.WriteLine ("Writing separator values (to overwrite SPI registers)");
                Console.WriteLine ("=====================================================");

                scope.Write(separatorStartAddress, separatorValues);

                Console.WriteLine ("=====================================================");
                Console.WriteLine ("Reading values");
                Console.WriteLine ("=====================================================");

                start = DateTime.Now;

                // Read all values eagerly, then await any async 'String received' comments.
                // This is just for more beautiful output.
                var readValues = scope.Read(startAddress, values.Length).ToList();
                Thread.Sleep (100);

                Console.WriteLine ("Duration: {0}", DateTime.Now-start);

                Console.WriteLine ("=====================================================");
                Console.WriteLine ("Summary");
                Console.WriteLine ("=====================================================");

                Console.Write ("Written:");
                foreach (var value in values)
                {
                    Console.Write(" {0} (x{0:X2})", value);
                }
                Console.WriteLine();

                Console.Write ("Read:   ");
                foreach (var value in readValues)
                {
                    Console.Write(" {0} (x{0:X2})", value);
                }
                Console.WriteLine();
           }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        private void SetDemoSignals(Appliance appliance)
        {
            // Get the signal generator and reset the hardware to cancel settings from previous
            // configurations.
            var signalGenerator = appliance.SignalGenerator;
            signalGenerator.Reset();

            // Set the outputs to DDS channels 2 and 3.
            signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator2;
            signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator3;

            // Configure DDS channel 2 (vertical deflection).
            signalGenerator.DdsGenerators[2].Waveform = Waveform.Sine;
            signalGenerator.DdsGenerators[2].Frequency = 1000;
            signalGenerator.DdsGenerators[2].Amplitude = signalGenerator.DdsGenerators[2].MaximumAmplitude;

            // Configure DDS channel 3 (horizontal deflection, synchronized by DDS channel 2).
            signalGenerator.DdsGenerators[3].Waveform = Waveform.Sine;
            signalGenerator.DdsGenerators[3].Frequency = 2000;
            signalGenerator.DdsGenerators[3].Amplitude = signalGenerator.DdsGenerators[3].MaximumAmplitude;
            signalGenerator.DdsGenerators[3].SynchronizationSource = ModulationAndSynchronizationSource.DdsGenerator2;

            // Flush all modifications, i.e. send all set commands that have modified values.
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
        }
    }
}
