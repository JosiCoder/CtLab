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

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides a simple console application for testing and demonstration purposes.
    /// </summary>
    public static class TestConsole
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static void Main()
        {
            Console.WriteLine("Test console started.");

            var spiDirect = false;

            // Evaluate regular expressions. This is just for testing and developing purposes.
            //RegularExpressions.Test();

            RunDummyConnectionLowLevelSamples();
            RunRealHardwareLowLevelSamples();

            RunDummyConnectionSignalGeneratorSamples();

            RunRealHardwareSignalGeneratorSamples(spiDirect);

            //TODO: activate after VHDL scope integration
//            RunRealHardwareScopeSamples(spiDirect);

            Console.WriteLine("Test console finished, press any key.");
            Console.ReadLine();
        }

        /// <summary>
        /// Runs samples that use a dummy connection and thus don't need real c't Lab hardware.
        /// Communication via the dummy connection will be reported to the console to get a better
        /// understanding of what's going on.
        /// </summary>
        private static void RunDummyConnectionLowLevelSamples()
        {
            //== Here you can activate or deactivate one or more of the samples below. They don't
            //== dependent on each other.

            // The following low-level samples are useful to understand how the CtLab library works
            // internally, developers just using this library can safely ignore these samples.

            // Sample: Send a command and receive a message via a simulated dummy connection.
            new DummyConnectionLowLevelSamples().SendCommandAndReceiveInjectedMessage();

            // Sample: Send periodic query commands via a simulated dummy connection.
            new DummyConnectionLowLevelSamples().SendPeriodicQueryCommands();
        }

        /// <summary>
        /// Runs samples that need real c't Lab hardware connected via a (physical or emulated) serial port.
        /// </summary>
        private static void RunRealHardwareLowLevelSamples()
        {
            //== Here you can activate or deactivate one or more of the samples below. They don't
            //== dependent on each other.

            // The following low-level samples are useful to understand how the CtLab library works
            // internally, developers just using this library can safely ignore these samples.

            // Sample: Send a raw command string to the c't lab.
            new RealHardwareLowLevelSamples().SendRawCommandString();

            // Sample: Send a command to the c't lab.
            new RealHardwareLowLevelSamples().SendCommandAndReceiveMessages();
        }

        /// <summary>
        /// Runs samples that use a dummy connection and thus don't need real c't Lab hardware.
        /// Communication via the dummy connection will be reported to the console to get a better
        /// understanding of what's going on.
        /// </summary>
        public static void RunDummyConnectionSignalGeneratorSamples()
        {
            //== Here you can activate or deactivate one or more of the samples below. They don't
            //== dependent on each other.

            // Sample: Handle a specific c't Lab appliance via a simulated dummy connection.
            new DummyConnectionSignalGeneratorSamples().HandleSampleCtLabAppliance();
        }

        /// <summary>
        /// Runs samples that need real c't Lab hardware connected via a (physical or emulated) serial port.
        /// For these tests, a proper configuration supporting a signal generator must be loaded into the FPGA.
        /// </summary>
        public static void RunRealHardwareSignalGeneratorSamples(bool spiDirect)
        {
            //== Here you can activate or deactivate one or more of the samples below. They don't
            //== dependent on each other.

            // Sample: Configure the signal generator to create an amplitude-modulated signal.
            new RealHardwareSignalGeneratorSamples(spiDirect).SetupAmplitudeModulatedSignal();

            // Sample: Configure the signal generator to create a frequency-modulated signal.
            new RealHardwareSignalGeneratorSamples(spiDirect).SetupFrequencyModulatedSignal();

            // Sample: Configure the signal generator to create signals that can be displayed as a Lissajous figure.
            new RealHardwareSignalGeneratorSamples(spiDirect).SetupLissajousFigure();

            // Sample: Configure the signal generator to create a pulse signal. 
            new RealHardwareSignalGeneratorSamples(spiDirect).SetupPulseSignal();

            // Sample: Configure the signal generator to measure the frequency of an internal signal.
            new RealHardwareSignalGeneratorSamples(spiDirect).SetupUniversalCounterToMeasureFrequencyOfInternalSignal();
        }

        /// <summary>
        /// Runs samples that need real c't Lab hardware connected via a (physical or emulated) serial port.
        /// For these tests, a proper configuration supporting a scope must be loaded into the FPGA.
        /// </summary>
        public static void RunRealHardwareScopeSamples(bool spiDirect)
        {
            //== Here you can activate or deactivate one or more of the samples below. They don't
            //== dependent on each other.

            // Sample: Write sample values to the storage and reads them.
            new RealHardwareScopeSamples(spiDirect).WriteAndReadStorageValues();
        }
    }
}
