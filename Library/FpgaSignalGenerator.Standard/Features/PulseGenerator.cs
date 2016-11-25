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
    /// Communicates with a pulse generator implemented within a c't Lab FPGA device configured
    /// as an FPGA Lab.
    /// </summary>
    public class PulseGenerator : IPulseGenerator
    {
        private UInt32ValueSubchannelWriter _pulseDurationConfigurator;
        private UInt32ValueSubchannelWriter _pauseDurationConfigurator;

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
            ISubchannelValueSetter pulseDurationSetter,
            ISubchannelValueSetter pauseDurationSetter)
        {
            _pulseDurationConfigurator = new UInt32ValueSubchannelWriter(pulseDurationSetter);
            _pauseDurationConfigurator = new UInt32ValueSubchannelWriter(pauseDurationSetter);
        }

        /// <summary>
        /// Gets or sets the pulse duration in steps of 10 ns.
        /// </summary>
        public uint PulseDuration
        {
            get { return _pulseDurationConfigurator.Value; }
            set { _pulseDurationConfigurator.Value = value; }
        }

        /// <summary>
        /// Gets or sets the pause duration in steps of 10 ns.
        /// </summary>
        public uint PauseDuration
        {
            get { return _pauseDurationConfigurator.Value; }
            set { _pauseDurationConfigurator.Value = value; }
        }
    }
}