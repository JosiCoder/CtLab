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
    /// Holds frequency modulation information values.
    /// </summary>
    public struct FrequencyModulationInformationSet
    {
        /// <summary>
        /// The modulation depth in Hz.
        /// </summary>
        public double ModulationDepth;

        /// <summary>
        /// The modulation depth relative to its possible maximum value. A value of 1.0
        /// (i.e. 100%) indicates the maximum relative modulation depth possible without
        /// distortions. This happens when the carrier´s frequency either reaches its maximum
        /// value or zero.
        /// </summary>
        public float RelativeModulationDepth;

        /// <summary>
        /// Gets a value indicating whether the maximun possible modulation is exceeded
        /// and thus the signal is distorted.
        /// </summary>
        public bool Overmodulated
        {
            get { return RelativeModulationDepth > 1; }
        }
    }
}
