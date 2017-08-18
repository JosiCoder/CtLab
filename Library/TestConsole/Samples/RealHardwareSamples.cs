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
using CtLab.Connection.Serial;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.BasicIntegration;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Environment;
using CtLab.EnvironmentIntegration;

namespace CtLab.TestConsole
{
    /// <summary>
    /// Provides some samples using real c´t Lab hardware connected via a (physical or emulated) serial
    /// port.
    /// </summary>
    public static class RealHardwareSamples
    {
		private const string _portName = "/dev/ttyUSB0";
        private const byte _channel = 7;

        private const int _queryCommandSendPeriod = 500; // ms

        /// <summary>
        /// Configures the FPGA Lab to create an amplitude-modulated signal.
        /// Carrier is a sine signal with a frequency of 1 kHz, modulator is a sine signal with a
        /// frequency of 100 Hz. Modulation depth is 50%.
        /// DDS-channel 0 generates the carrier modulated by DDS channel 1.
        /// </summary>
        public static void SetupAmplitudeModulatedSignal()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            using (var appliance = container.GetInstance<Appliance>())
            {
                ((SerialConnection)appliance.ApplianceConnection.Connection).Open(_portName);

                // Set the channel of the appliance´s only FPGA lab.
                appliance.InitializeSignalGenerator(_channel);

                // Get the signal generator and reset the hardware to cancel settings from previous
                // configurations.
                var signalGenerator = appliance.SignalGenerator;
                signalGenerator.Reset();

                // Set the outputs to DDS channels 0 and 1.
                signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator0;
                signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator1;

                // Configure DDS channel 0 (carrier, modulated by DDS channel 1).
                signalGenerator.DdsGenerators[0].Waveform = Waveform.Sine;
                signalGenerator.DdsGenerators[0].Frequency = 1000;
                signalGenerator.DdsGenerators[0].Amplitude = (short)(signalGenerator.DdsGenerators[0].MaximumAmplitude * 1 / 2);
                signalGenerator.DdsGenerators[0].AmplitudeModulationSource = ModulationAndSynchronizationSource.DdsGenerator1;

                // Configure DDS channel 1 (modulator).
                signalGenerator.DdsGenerators[1].Waveform = Waveform.Sine;
                signalGenerator.DdsGenerators[1].Frequency = 100;
                signalGenerator.DdsGenerators[1].Amplitude = (short)(signalGenerator.DdsGenerators[1].MaximumAmplitude * 1 / 4);

                // Display some values resulting from the current settings.
                Console.WriteLine("DDS channel 0: AM {0}%, {1}overmodulated",
                                  signalGenerator.DdsGeneratorsAMInformationSets[0].RelativeModulationDepth * 100,
                                  signalGenerator.DdsGeneratorsAMInformationSets[0].Overmodulated ? "" : "not ");

                // Flush all modifications, i.e. send all set commands that have modified values.
                appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
            }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Configures the FPGA Lab to create a frequency-modulated signal.
        /// Carrier is a sine signal with a frequency of 6 kHz, modulator is a sawtooth signal with a
        /// frequency of 100 Hz. Modulation depth is +/-6.1 kHz, thus we get a slight overmodulation.
        /// DDS-channel 0 generates the carrier modulated by DDS channel 1.
        /// </summary>
        public static void SetupFrequencyModulatedSignal()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            using (var appliance = container.GetInstance<Appliance>())
            {
                ((SerialConnection)appliance.ApplianceConnection.Connection).Open(_portName);

                // Set the channel of the appliance´s only FPGA lab.
                appliance.InitializeSignalGenerator(_channel);

                // Get the signal generator and reset the hardware to cancel settings from previous
                // configurations.
                var signalGenerator = appliance.SignalGenerator;
                signalGenerator.Reset();

                // Set the outputs to DDS channels 0 and 1.
                signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator0;
                signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator1;

                // Configure DDS channel 0 (carrier, modulated by DDS channel 1).
                signalGenerator.DdsGenerators[0].Waveform = Waveform.Sine;
                signalGenerator.DdsGenerators[0].Frequency = 6000;
                signalGenerator.DdsGenerators[0].Amplitude = signalGenerator.DdsGenerators[0].MaximumAmplitude;
                signalGenerator.DdsGenerators[0].FrequencyModulationSource = ModulationAndSynchronizationSource.DdsGenerator1;
                signalGenerator.DdsGenerators[0].MaximumFrequencyModulationRange = 1;

                // Configure DDS channel 1 (modulator).
                signalGenerator.DdsGenerators[1].Waveform = Waveform.Sawtooth;
                signalGenerator.DdsGenerators[1].Frequency = 100;
                signalGenerator.DdsGenerators[1].Amplitude = signalGenerator.DdsGenerators[1].MaximumAmplitude;

                // Display some values resulting from the current settings.
                Console.WriteLine("DDS channel 0: FM modulation depth {0}Hz ({1}%), {2}overmodulated.",
                                  signalGenerator.DdsGeneratorsFMInformationSets[0].ModulationDepth,
                                  signalGenerator.DdsGeneratorsFMInformationSets[0].RelativeModulationDepth * 100,
                                  signalGenerator.DdsGeneratorsFMInformationSets[0].Overmodulated ? "" : "not ");
                Console.WriteLine("FM modulation range is {0}Hz.",
                                  signalGenerator.DdsGeneratorsFMInformationSets[0].ModulationDepth);

                // Flush all modifications, i.e. send all set commands that have modified values.
                appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
            }

            Utilities.WriteFooterAndWaitForKeyPress();
        }


        /// <summary>
        /// Configures the FPGA Lab to create signals that can be displayed as a Lissajous figure. DDS-
        /// channel 2 generates a signal of 1 kHz with phase 0° and DDS channel 3 generates a signal of
        /// 2 kHz.
        /// </summary>
        public static void SetupLissajousFigure()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            using (var appliance = container.GetInstance<Appliance>())
            {
                ((SerialConnection)appliance.ApplianceConnection.Connection).Open(_portName);

                // Set the channel of the appliance´s only FPGA lab.
                appliance.InitializeSignalGenerator(_channel);

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

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Configures the FPGA Lab to create a pulse signal. Pulse duration is 100 us and pause duration
        /// is 1 ms.
        /// </summary>
        public static void SetupPulseSignal()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            using (var appliance = container.GetInstance<Appliance>())
            {
                ((SerialConnection)appliance.ApplianceConnection.Connection).Open(_portName);

                // Set the channel of the appliance´s only FPGA lab.
                appliance.InitializeSignalGenerator(_channel);

                // Get the signal generator and reset the hardware to cancel settings from previous
                // configurations.
                var signalGenerator = appliance.SignalGenerator;
                signalGenerator.Reset();

                // Set the outputs to the pulse generator.
                signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.PulseGenerator;
                signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.PulseGenerator;

                // Configure pulse generator.
                signalGenerator.PulseGenerator.PulseDuration = 10000; // 100 us
                signalGenerator.PulseGenerator.PauseDuration = 100000; // 1 ms

                // Flush all modifications, i.e. send all set commands that have modified values.
                appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
            }

            Utilities.WriteFooterAndWaitForKeyPress();
        }

        /// <summary>
        /// Configures the FPGA Lab to measure the frequency of an internal signal. DDS channel 0 is connected
        /// to the counter as well as to both outputs.
        /// </summary>
        public static void SetupUniversalCounterToMeasureFrequencyOfInternalSignal()
        {
            Utilities.WriteHeader();

            // Each c't Lab appliance and associated connection needs its own IoC container. 
            var container = ConfigureIoC();

            using (var appliance = container.GetInstance<Appliance>())
            {
                ((SerialConnection)appliance.ApplianceConnection.Connection).Open(_portName);

                // Set the channel of the appliance´s only FPGA lab.
                appliance.InitializeSignalGenerator(_channel);

                // Get the signal generator and reset the hardware to cancel settings from previous
                // configurations.
                var signalGenerator = appliance.SignalGenerator;
                signalGenerator.Reset();

                // Set the outputs to DDS channel 0.
                signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator0;
                signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator0;

                // Configure DDS channel 0.
                signalGenerator.DdsGenerators[0].Amplitude = signalGenerator.DdsGenerators[0].MaximumAmplitude;

                // Configure the universal counter.
                signalGenerator.UniversalCounter.InputSource = UniversalCounterSource.DdsGenerator0;
                signalGenerator.UniversalCounter.PrescalerMode = PrescalerMode.GatePeriod_100ms;

                // Flush all modifications, i.e. send all set commands that have modified values.
                appliance.ApplianceConnection.SendSetCommandsForModifiedValues();

                // Listen to counter changes and display them.
                signalGenerator.UniversalCounter.ValueChanged +=
                    (sender, e) => Console.WriteLine("Counter reported a new frequency: {0}", e.Value);

                // Send query commands periodically for some seconds.
                appliance.ApplianceConnection.StartSendingQueryCommands(_queryCommandSendPeriod);

                // Change the generator frequency making the counter report new values.
                var millisecondsToWait = 3000;
                SetDds0FrequencyAndWait(appliance, 10000, millisecondsToWait);
                SetDds0FrequencyAndWait(appliance, 8000, millisecondsToWait);
                SetDds0FrequencyAndWait(appliance, 5000, millisecondsToWait);
                SetDds0FrequencyAndWait(appliance, 2000, millisecondsToWait);

                // Stop sending query commands.
                appliance.ApplianceConnection.StopSendingQueryCommands();
                Thread.Sleep(2 * _queryCommandSendPeriod); // wait for all pending 
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

        private static void SetDds0FrequencyAndWait(Appliance appliance, int frequency, int millisecondsToWait)
        {
            appliance.SignalGenerator.DdsGenerators[0].Frequency = frequency;
            appliance.ApplianceConnection.SendSetCommandsForModifiedValues();
            Console.WriteLine("Generator frequency set to {0}", frequency);
            Thread.Sleep(millisecondsToWait);
        }
    }
}
