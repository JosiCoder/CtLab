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

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Provides facilities to communicate with a DDS generator implemented within a c't Lab FPGA
    /// device configured as an FPGA Lab.
    /// </summary>
    public interface IDdsGenerator
    {
        /// <summary>
        /// Gets the maximum amplitude in internal units (not Volt or dB!).
        /// </summary>
        short MaximumAmplitude { get; }

        /// <summary>
        /// Gets the maximum frequency in Hz.
        /// </summary>
        double MaximumFrequency { get; }

        /// <summary>
        /// Gets or sets the signal waveform.
        /// </summary>
        Waveforms Waveform { get; set; }

        /// <summary>
        /// Gets or sets maximum frequency modulation depth. Accepts values in the range of 0..5.
        /// If set to n, modulation depth is +/-(<see cref="MaximumFrequency"/> / 2 / 8^(5-n)),
        /// e.g. +/-6103,5 Hz for n=1 and MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="MaximumFrequencyModulationDepth"/>.
        /// </summary>
        ushort MaximumFrequencyModulationRange { get; set; }

        /// <summary>
        /// Gets maximum frequency modulation depth in Hertz.
        /// This is modified by setting <see cref="MaximumFrequencyModulationRange"/>.
        /// </summary>
        double MaximumFrequencyModulationDepth { get; }

        /// <summary>
        /// Gets or sets the synchronization source. If this value ist set to the current
        /// instance itself, no synchronization takes place.
        /// </summary>
        ModulationAndSynchronizationSources SynchronizationSource { get; set; }

        /// <summary>
        /// Gets or sets the phase modulation source. If this value ist set to the current
        /// instance itself, no phase modulation takes place.
        /// </summary>
        ModulationAndSynchronizationSources PhaseModulationSource { get; set; }

        /// <summary>
        /// Gets or sets the frequency modulation source. If this value ist set to the current
        /// instance itself, no frequency modulation takes place.
        /// </summary>
        ModulationAndSynchronizationSources FrequencyModulationSource { get; set; }

        /// <summary>
        /// Gets or sets the amplitude modulation source. If this value ist set to the current
        /// instance itself, no amplitude modulation takes place.
        /// </summary>
        ModulationAndSynchronizationSources AmplitudeModulationSource { get; set; }

        /// <summary>
        /// Gets or sets the value added to the 32 bit phase accumulator in each cycle.
        /// Accepts values in the range of 0..2^31, resolution is
        /// <see cref="MaximumFrequency"/> / 2^31, e.g. 0.23Hz for MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="Frequency"/>.
        /// </summary>
        uint PhaseIncrement { get; set; }

        /// <summary>
        /// Gets or sets the frequency in Hertz.
        /// Accepts values in the range of 0..<see cref="MaximumFrequency"/>, resolution is
        /// <see cref="MaximumFrequency"/> / 2^31, e.g. 0.23Hz for MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="PhaseIncrement"/>.
        /// </summary>
        double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the amplitude. Range is +/- <see cref="MaximumAmplitude"/>.
        /// </summary>
        short Amplitude { get; set; }

        /// <summary>
        /// Gets or sets the phase. Range is +/- one period (+/-2*Pi).
        /// </summary>
        short Phase { get; set; }
    }
}