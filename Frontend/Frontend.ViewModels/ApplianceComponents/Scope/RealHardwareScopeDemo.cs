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
using ScopeLib.Utilities;
using ScopeLib.Sampling;
using CtLab.FpgaScope.Interfaces;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    // TODO: Move demo somewhere else, replace it with real hardware access.
    /// <summary>
    /// Provides some samples using real c´t Lab hardware connected via a (physical or emulated) serial
    /// port or a direct SPI connection.
    /// For these tests, a proper configuration supporting a scope must be loaded into the FPGA.
    /// </summary>
    public class RealHardwareScopeDemo
    {
        private const bool logAccess = true;

        /// <summary>
        /// Captures sample values to the storage and reads them using the low-level SRAM controller protocol.
        /// Note: No handshake is usually necessary for writing as the storage controller (VHDL) is much faster
        /// than this software. For reading, the entire roundtrip time necessary to provide the read value has
        /// to be considered, especially when using the c't Lab protocol. 
        /// </summary>
        public IEnumerable<IEnumerable<uint>> CaptureAndReadStorageValues(Appliance appliance)
        {
            var scope = SetupHardware(appliance, true);

            WriteLine ("=====================================================");
            WriteLine ("Capturing values");
            WriteLine ("=====================================================");

            var start = DateTime.Now;

            scope.Capture(0, 32767); // 32 K memory

            WriteLine (() => string.Format("Duration: {0}", DateTime.Now - start));

            WriteLine ("=====================================================");
            WriteLine ("Reading values");
            WriteLine ("=====================================================");

            start = DateTime.Now;

            // Read all values eagerly.
            var readValueSets = new List<IEnumerable<uint>>();
            readValueSets.Add(scope.Read(0, 21).ToList());

            WriteLine (() => string.Format("Duration: {0}", DateTime.Now - start));

            WriteLine ("=====================================================");
            WriteLine ("Summary");
            WriteLine ("=====================================================");

            WriteValueSets("Read", readValueSets);

            return readValueSets;
        }

        /// <summary>
        /// Creates sample sequences from the specified captured values, interpreted as a signed 8 bit value
        /// and scaled down by 100.
        /// </summary>
        public IEnumerable<SampleSequence> CreateSampleSequences(int sampleFrequency,
            IEnumerable<IEnumerable<uint>> valueSets)
        {
            return valueSets.Select(valueSet =>
                new SampleSequence(1f / sampleFrequency, valueSet.Select(v => ((sbyte) v) / 100d)));
        }

        /// <summary>
        /// Configures some signals for the storage inputs and the analog outputs.
        /// By observing the signals on the analog outputs we can see how accessing the
        /// storage briefly disconnects the DAC.
        /// </summary>
        public void SetupHardwareSignals(ISignalGenerator signalGenerator)
        {
            // Reset the hardware to cancel settings from previous configurations.
            signalGenerator.Reset();

            // Set the outputs to DDS channels 2 and 3.
            signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator2;
            signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator3;

            // Configure the pulse generator.
            signalGenerator.PulseGenerator.PulseDuration = 50;
            signalGenerator.PulseGenerator.PauseDuration = 50;

            // Configure DDS channel 2.
            signalGenerator.DdsGenerators[2].Waveform = Waveform.Sine;
            signalGenerator.DdsGenerators[2].Frequency = 1000000;
            signalGenerator.DdsGenerators[2].Amplitude = signalGenerator.DdsGenerators[2].MaximumAmplitude;

            // Configure DDS channel 3.
            signalGenerator.DdsGenerators[3].Waveform = Waveform.Sine;
            signalGenerator.DdsGenerators[3].Frequency = 500000;
            signalGenerator.DdsGenerators[3].Amplitude = signalGenerator.DdsGenerators[3].MaximumAmplitude;
        }

        /// <summary>
        /// Configures the appliance and returns the scope.
        /// Configures some signals on the storage inputs and the analog outputs.
        /// By observing the signals on the analog outputs we can see how accessing the
        /// storage briefly disconnects the DAC.
        /// </summary>
        private IScope SetupHardware(Appliance appliance, bool useHardwareSignal)
        {
            // Get the scope and reset the hardware to cancel settings from previous configurations.
            var scope = appliance.Scope;
            scope.Reset();

            // Set the scope input to a signal.
            scope.InputSource =  ScopeSource.DdsGenerator2;
            if (!useHardwareSignal)
            {
                scope.InputSource = ScopeSource.WriteValue;
            }
                
            // Flush all modifications, i.e. send all set commands that have modified values.
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();

            return scope;
        }

        /// <summary>
        /// Writes a line to the console if logging is activated.
        /// </summary>
        private void WriteLine(object value)
        {
            if (logAccess)
            {
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// Writes a line to the console if logging is activated.
        /// </summary>
        private void WriteLine(Func<object> valueProvider)
        {
            if (logAccess)
            {
                Console.WriteLine(valueProvider());
            }
        }

        /// <summary>
        /// Writes value sets to the console if logging is activated.
        /// </summary>
        private void WriteValueSets(object header, IEnumerable<IEnumerable<uint>> valueSets)
        {
            if (logAccess)
            {
                foreach(var valueSet in valueSets)
                {
                    Console.Write ("{0}: ", header);
                    foreach (var value in valueSet)
                    {
                        Console.Write(" {0} (x{0:X2})", value);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
