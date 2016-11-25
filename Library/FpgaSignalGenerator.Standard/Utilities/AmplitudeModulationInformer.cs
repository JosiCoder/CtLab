using System;
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

using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Provides some information belonging to the amplitude modulation of the carrier assigned
    /// to the current instance.
    /// </summary>
    public class AmplitudeModulationInformer
    {
        /// <summary>
        /// The carrier source this instance belongs to.
        /// </summary>
        public readonly IAmplitudeModulationCarrierSource CarrierSource;

        /// <summary>
        /// Gets or sets the source supplying the signal used for modulation.
        /// </summary>
        public IAmplitudeModulatorSource ModulatorSource { get; set; }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="carrierSource">The carrier source this instance belongs to.</param>
        public AmplitudeModulationInformer(IAmplitudeModulationCarrierSource carrierSource)
        {
            CarrierSource = carrierSource;
        }

        /// <summary>
        /// Gets the modulation depth relative to its possible maximum value. A value of 1.0
        /// (i.e. 100%) indicates the maximum relative modulation depth possible without
		/// distortions. This happens when the carrier's amplitude either reaches its maximum
        /// value or zero.
        /// </summary>
        public float RelativeModulationDepth
        {
            get
            {
                if (ModulatorSource == null)
                    return (float)0.0;

                // Ensure 0 to be returned for zero modulation instead of NaN as this would happen
                // if the carrier has no modulation room (i.e. is zero or maximum).
                if (ModulatorSource.Amplitude == 0)
                    return (float)0.0;

                var carrierAmplitudeRoom =
                    CarrierSource.Amplitude < ((float)CarrierSource.MaximumAmplitude) / 2
                        ? CarrierSource.Amplitude
                        : CarrierSource.MaximumAmplitude - CarrierSource.Amplitude;
                return Math.Abs(((float)ModulatorSource.Amplitude) / carrierAmplitudeRoom);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the maximun possible modulation is exceeded
        /// and thus the signal is distorted.
        /// </summary>
        public bool Overmodulated
        {
            get { return RelativeModulationDepth > 1; }
        }

        /// <summary>
        /// Gets a structure holding the essential modulation information.
        /// </summary>
        public AmplitudeModulationInformationSet ModulationInformation
        {
            get
            {
                AmplitudeModulationInformationSet infoSet;
                infoSet.RelativeModulationDepth = RelativeModulationDepth;
                return infoSet;
            }
        }
    }
}