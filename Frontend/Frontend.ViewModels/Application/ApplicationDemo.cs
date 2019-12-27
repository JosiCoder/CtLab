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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    // TODO: comment, rework
    public class ApplicationDemo
    {
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

    }
}

