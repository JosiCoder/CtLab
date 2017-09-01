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
using CtLab.FpgaConnection.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Communicates with a DDS generator implemented within a c't Lab FPGA device configured as
    /// an FPGA Lab.
    /// </summary>
    public class DdsGenerator :
        IAmplitudeModulationCarrierSource, IFrequencyModulationCarrierSource,
        IAmplitudeModulatorSource, IFrequencyModulatorSource, IDdsGenerator
    {
        private WaveformWriter _waveformWriter;
        private PhaseIncrementWriter _phaseIncrementWriter;
        private AmplitudePhaseWriter _amplitudePhaseWriter;

        /// <summary>
        /// Gets the maximum amplitude in internal units (not Volt or dB!).
        /// </summary>
        public short MaximumAmplitude { get { return short.MaxValue; } }

        /// <summary>
        /// Gets the maximum frequency in Hz.
        /// </summary>
        public double MaximumFrequency { get { return 50e6; } }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="waveformSetter">
		/// The setter used to set the generator's waveform, modulation, and synchronization
        /// settings.
        /// </param>
        /// <param name="phaseIncrementSetter">
		/// The setter used to set the generator's phase increment.
        /// </param>
        /// <param name="amplitudePhaseSetter">
		/// The setter used to set the generator's amplitude and phase.
        /// </param>
        public DdsGenerator(
            IFpgaValueSetter waveformSetter,
            IFpgaValueSetter phaseIncrementSetter,
            IFpgaValueSetter amplitudePhaseSetter)
        {
            _waveformWriter = new WaveformWriter(waveformSetter);
            _phaseIncrementWriter = new PhaseIncrementWriter(phaseIncrementSetter);
            _amplitudePhaseWriter = new AmplitudePhaseWriter(amplitudePhaseSetter);
        }

        /// <summary>
        /// Gets or sets the signal waveform.
        /// </summary>
        public Waveform Waveform
        {
            get { return _waveformWriter.Waveform; }
            set { _waveformWriter.Waveform = value; }
        }

        /// <summary>
        /// Gets or sets the maximum frequency modulation range. Accepts values in the range of 0..5.
        /// If set to n, modulation depth is +/-(<see cref="MaximumFrequency"/> / 2 / 8^(5-n)),
        /// e.g. +/-6103.5 Hz for n=1 and MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="MaximumFrequencyModulationDepth"/>.
        /// </summary>
        public ushort MaximumFrequencyModulationRange
        {
            get { return _waveformWriter.MaximumFrequencyModulationDepth; }
            set
            {
                if (value > 5)
                    throw new ArgumentOutOfRangeException("MaximumFrequencyModulationDepth");

                _waveformWriter.MaximumFrequencyModulationDepth = value;
            }
        }

        /// <summary>
        /// Gets the maximum frequency modulation depth in Hertz.
        /// This is modified by setting <see cref="MaximumFrequencyModulationRange"/>.
        /// </summary>
        public double MaximumFrequencyModulationDepth
        {
            get
            {
                return MaximumFrequency / 2 / Math.Pow(8, 5 - MaximumFrequencyModulationRange);
            }
        }

        /// <summary>
        /// Gets or sets the synchronization source. If this value ist set to the current
        /// instance itself, no synchronization takes place.
        /// </summary>
        public ModulationAndSynchronizationSource SynchronizationSource
        {
            get { return _waveformWriter.SynchronizationSource; }
            set { _waveformWriter.SynchronizationSource = value; }
        }

        /// <summary>
        /// Gets or sets the phase modulation source. If this value ist set to the current
        /// instance itself, no phase modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSource PhaseModulationSource
        {
            get { return _waveformWriter.PhaseModulationSource; }
            set { _waveformWriter.PhaseModulationSource = value; }
        }

        /// <summary>
        /// Gets or sets the frequency modulation source. If this value ist set to the current
        /// instance itself, no frequency modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSource FrequencyModulationSource
        {
            get { return _waveformWriter.FrequencyModulationSource; }
            set { _waveformWriter.FrequencyModulationSource = value; }
        }

        /// <summary>
        /// Gets or sets the amplitude modulation source. If this value ist set to the current
        /// instance itself, no amplitude modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSource AmplitudeModulationSource
        {
            get { return _waveformWriter.AmplitudeModulationSource; }
            set { _waveformWriter.AmplitudeModulationSource = value; }
        }

        /// <summary>
        /// Gets or sets the value added to the 32 bit phase accumulator in each cycle.
        /// Accepts values in the range of 0..2^31, resolution is
        /// <see cref="MaximumFrequency"/> / 2^31, e.g. 0.23Hz for MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="Frequency"/>.
        /// </summary>
        public uint PhaseIncrement
        {
            get { return _phaseIncrementWriter.Value; }
            set { _phaseIncrementWriter.Value = value; }
        }

        /// <summary>
        /// Gets or sets the frequency in Hertz.
        /// Accepts values in the range of 0..<see cref="MaximumFrequency"/>, resolution is
        /// <see cref="MaximumFrequency"/> / 2^31, e.g. 0.23Hz for MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="PhaseIncrement"/>.
        /// </summary>
        public double Frequency
        {
            get { return PhaseIncrement / Math.Pow(2, 31) * MaximumFrequency; }
            set
            {
                double phaseIncrement = (value / MaximumFrequency) * Math.Pow(2, 31);
                if (phaseIncrement > uint.MaxValue)
                    throw new ArgumentOutOfRangeException("Frequency");
                PhaseIncrement = (uint)phaseIncrement;
            }
        }

        /// <summary>
        /// Gets or sets the amplitude. Range is +/- <see cref="MaximumAmplitude"/>.
        /// </summary>
        public short Amplitude
        {
            get { return _amplitudePhaseWriter.Amplitude; }
            set { _amplitudePhaseWriter.Amplitude = value; }
        }

        /// <summary>
        /// Gets or sets the phase. Range is +/- one period (+/-2*Pi).
        /// </summary>
        public short Phase
        {
            get { return _amplitudePhaseWriter.Phase; }
            set { _amplitudePhaseWriter.Phase = value; }
        }
    }
}