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

using CtLab.CommandsAndMessages.Interfaces;
using CtLab.SubchannelAccess;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Writes the waveform, modulation, and synchronization settings of a DDS generator by
    /// setting the value of a set command that can be sent to a c't Lab FPGA device configured as an FPGA Lab.
    /// </summary>
    public class WaveformWriter : SubchannelWriterBase
    {
        private Waveforms _waveform;
        private ushort _maximumFrequencyModulationDepth;
        private ModulationAndSynchronizationSources _synchronizationSource;
        private ModulationAndSynchronizationSources _phaseModulationSource;
        private ModulationAndSynchronizationSources _frequencyModulationSource;
        private ModulationAndSynchronizationSources _amplitudeModulationSource;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="subchannelValueSetter">
		/// The setter used to set the subchannel's value.
        /// </param>
        public WaveformWriter(ISubchannelValueSetter subchannelValueSetter)
            : base (subchannelValueSetter)
        {
        }

        /// <summary>
        /// Gets or sets the signal waveform.
        /// </summary>
        public Waveforms Waveform
        {
            get { return _waveform; }
            set
            {
                _waveform = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets maximum frequency modulation depth.
        /// </summary>
        public ushort MaximumFrequencyModulationDepth
        {
            get { return _maximumFrequencyModulationDepth; }
            set
            {
                _maximumFrequencyModulationDepth = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the synchronization source. If this value ist set to the current
        /// instance itself, no synchronization takes place.
        /// </summary>
        public ModulationAndSynchronizationSources SynchronizationSource
        {
            get { return _synchronizationSource; }
            set
            {
                _synchronizationSource = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the phase modulation source. If this value ist set to the current
        /// instance itself, no phase modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSources PhaseModulationSource
        {
            get { return _phaseModulationSource; }
            set
            {
                _phaseModulationSource = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the frequency modulation source. If this value ist set to the current
        /// instance itself, no frequency modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSources FrequencyModulationSource
        {
            get { return _frequencyModulationSource; }
            set
            {
                _frequencyModulationSource = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the amplitude modulation source. If this value ist set to the current
        /// instance itself, no amplitude modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSources AmplitudeModulationSource
        {
            get { return _amplitudeModulationSource; }
            set
            {
                _amplitudeModulationSource = value;
                SetCommandValue();
            }
        }

        private void SetCommandValue()
        {
            var combinedValue =
                ((uint) _waveform) << 16
                | ((uint) _maximumFrequencyModulationDepth << 8)
                | ((uint) _synchronizationSource << 6)
                | ((uint) _phaseModulationSource << 4)
                | ((uint) _frequencyModulationSource << 2)
                | ((uint) _amplitudeModulationSource);

            _subchannelValueSetter.SetValue(combinedValue);
        }
    }
}