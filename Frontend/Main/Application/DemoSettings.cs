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
using System.Threading.Tasks;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Environment;

namespace CtLab.Frontend
{
    /// <summary>
    /// Provides a demo configuration. Note that this acts on the model directly, circumventing
    /// the view models. Thus, the UI doesn't get any notifications about the changes done here.
    /// </summary>
    public class DemoSettings
    {
        /// <summary>
        /// Applies a demo configuration to an appliance so that we can
        /// immediately see something and have a starting point.
        /// <param name="appliance">The appliance to apply the settings to.</param>
        public void ApplyDemoSettings (Appliance appliance)
        {
            // Get the signal generator and reset the hardware to cancel settings from previous
            // configurations.
            var signalGenerator = appliance.SignalGenerator;
            signalGenerator.Reset ();

            // Set the outputs to DDS channel 0.
            signalGenerator.OutputSourceSelector.OutputSource0 = OutputSource.DdsGenerator0;
            signalGenerator.OutputSourceSelector.OutputSource1 = OutputSource.DdsGenerator1;

            // Configure DDS channels.
            signalGenerator.DdsGenerators[0].Amplitude = (short)(signalGenerator.DdsGenerators[0].MaximumAmplitude / 2);
            signalGenerator.DdsGenerators[1].Amplitude = (short)(signalGenerator.DdsGenerators[1].MaximumAmplitude / 2);
            signalGenerator.DdsGenerators[2].Amplitude = (short)(signalGenerator.DdsGenerators[1].MaximumAmplitude / 2);
            signalGenerator.DdsGenerators[3].Amplitude = (short)(signalGenerator.DdsGenerators[1].MaximumAmplitude / 2);

            signalGenerator.DdsGenerators[0].Waveform = Waveform.Rectangle;
            signalGenerator.DdsGenerators[1].Waveform = Waveform.Rectangle;
            signalGenerator.DdsGenerators[2].Waveform = Waveform.Sine;
            signalGenerator.DdsGenerators[3].Waveform = Waveform.Sine;

            // Also make the otherwise unused generators produce signals.
            appliance.SignalGenerator.DdsGenerators[0].Frequency = 1;
            appliance.SignalGenerator.DdsGenerators[1].Frequency = 1;
            appliance.SignalGenerator.DdsGenerators[2].Frequency = 1;
            appliance.SignalGenerator.DdsGenerators[3].Frequency = 1;
            appliance.SignalGenerator.PulseGenerator.PulseDuration = (uint)1e8;
            appliance.SignalGenerator.PulseGenerator.PauseDuration = (uint)1e8;

            // Configure the universal counter.
            signalGenerator.UniversalCounter.InputSource = UniversalCounterSource.DdsGenerator0;
            signalGenerator.UniversalCounter.PrescalerMode = PrescalerMode.GatePeriod_100ms;

            // Flush all modifications, i.e. send all set commands that have modified values.
            appliance.SendSetCommandsForModifiedValues();
        }
    }
}
