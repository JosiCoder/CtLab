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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Writes the amplitude and phase of a DDS generator by setting the according value of
    /// an FPGA device configured as an FPGA Lab.
    /// </summary>
    public class AmplitudePhaseWriter
    {
        private readonly IFpgaValueSetter _valueSetter;
        private short _amplitude;
        private short _phase;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="valueSetter">
		/// The setter used to set the FPGA's value.
        /// </param>
        public AmplitudePhaseWriter(IFpgaValueSetter valueSetter)
        {
            _valueSetter = valueSetter;
        }

        /// <summary>
        /// Gets or sets the amplitude.
        /// </summary>
        public short Amplitude
        {
            get { return _amplitude; }
            set
            {
                _amplitude = value;
                SetCommandValue();
            }
        }

        /// <summary>
        /// Gets or sets the phase.
        /// </summary>
        public short Phase
        {
            get { return _phase; }
            set
            {
                _phase = value;
                SetCommandValue();
            }
        }

        private void SetCommandValue()
        {
			// Here two signed numbers are interpreted as 16-bit two's complement patterns and
            // merged together.
            var combinedValue = ((uint)(ushort)_amplitude) << 16 | (ushort)_phase;
            _valueSetter.SetValue(combinedValue);
        }
    }
}