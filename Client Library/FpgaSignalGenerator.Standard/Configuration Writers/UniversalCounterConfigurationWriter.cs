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
    /// Writes the universal counter configuration by setting the value of a set command that
    /// can be sent to a c't Lab FPGA device configured as an FPGA Lab.
    /// </summary>
    public class UniversalCounterConfigurationWriter : SubchannelWriterBase
    {
        private UniversalCounterSources _inputSource;
        private MeasurementModes _measurementMode;
        private PrescalerModes _prescalerMode;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="subchannelValueSetter">
		/// The setter used to set the subchannel's value.
        /// </param>
        public UniversalCounterConfigurationWriter(ISubchannelValueSetter subchannelValueSetter)
            : base (subchannelValueSetter)
        {
        }

        /// <summary>
        /// Gets or sets the signal source.
        /// </summary>
        public UniversalCounterSources InputSource
        {
            get { return _inputSource; }
            set
            {
                _inputSource = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the measurement mode.
        /// </summary>
        public MeasurementModes MeasurementMode
        {
            get { return _measurementMode; }
            set
            {
                _measurementMode = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the prescaler mode used to generate the gate time or counter clock.
        /// </summary>
        public PrescalerModes PrescalerMode
        {
            get { return _prescalerMode; }
            set
            {
                _prescalerMode = value;
                SetCommandValue();
            }
        }

        private void SetCommandValue()
        {
            var combinedValue =
                ((uint)_inputSource) << 8
                | ((uint)_measurementMode) << 4
                | ((uint)_prescalerMode);
            _subchannelValueSetter.SetValue(combinedValue);
        }
    }
}