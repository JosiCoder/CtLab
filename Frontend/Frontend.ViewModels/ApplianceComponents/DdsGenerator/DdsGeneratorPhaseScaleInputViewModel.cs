//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

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

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides a scale input viewmodel for the amplitude value of a DDS generator.
    /// </summary>
    public class DdsGeneratorPhaseScaleInputViewModel
        : LinearScaleInputViewModel<short>, IScaleInputViewModel
    {
        private const double maxExternalValueAsShownOnControl = 1.0; // periods (*2Pi)
        private const double ExternalValuePerInternalUnit = maxExternalValueAsShownOnControl / short.MaxValue;
        private const short InternalValueLower = short.MinValue;
        private const short InternalValueUpper = short.MaxValue;
        private const double InternalValueStepWidth = 10.0 * ExternalValuePerInternalUnit;
        private const double MinBaseValueLower = double.NegativeInfinity;
        private const double MaxBaseValueUpper = double.PositiveInfinity;
        private const double overviewTickInterval = 0.25;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="setter">An action used to set the value when the scale input has been changed.</param>
        /// <param name="getter">A function used to get the value to set the scale input to.</param>
        public DdsGeneratorPhaseScaleInputViewModel(IApplianceServices applianceServices,
            Action<short> setter, Func<short> getter)
            : base(applianceServices, setter, getter,
                ExternalValuePerInternalUnit, InternalValueLower, InternalValueUpper,
                InternalValueStepWidth,
                MinBaseValueLower, MaxBaseValueUpper,
                overviewTickInterval)
        {}
    }
}

