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
    /// Communicates with a pulse generator implemented within a c't Lab FPGA device configured
    /// as an FPGA Lab.
    /// </summary>
    public class PulseGenerator : IPulseGenerator
    {
        private PulsePauseDurationWriter _pulseDurationWriter;
        private PulsePauseDurationWriter _pauseDurationWriter;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="pulseDurationSetter">
		/// The setter used to set the generator's pulse duration.
        /// </param>
        /// <param name="pauseDurationSetter">
		/// The setter used to set the generator's pause duration.
        /// </param>
        public PulseGenerator(
            IFpgaValueSetter pulseDurationSetter,
            IFpgaValueSetter pauseDurationSetter)
        {
            _pulseDurationWriter = new PulsePauseDurationWriter(pulseDurationSetter);
            _pauseDurationWriter = new PulsePauseDurationWriter(pauseDurationSetter);
        }

        /// <summary>
        /// Gets or sets the pulse duration in steps of 10 ns.
        /// </summary>
        public uint PulseDuration
        {
            get { return _pulseDurationWriter.Value; }
            set { _pulseDurationWriter.Value = value; }
        }

        /// <summary>
        /// Gets or sets the pause duration in steps of 10 ns.
        /// </summary>
        public uint PauseDuration
        {
            get { return _pauseDurationWriter.Value; }
            set { _pauseDurationWriter.Value = value; }
        }
    }
}