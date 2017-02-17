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
    /// Provides a scale input viewmodel for the pulse and pause duration values
    /// of a pulse generator.
    /// </summary>
    public class PulseGeneratorDurationScaleInputViewModel
        : LinLogScaleInputViewModel<uint>, IPulseGeneratorDurationScaleInputViewModel
    {
        // The pulse generator values are set in steps of 10ns up to uint.MaxValue.
        private const double ExternalValuePerInternalUnit = 1e-8;
        private const uint InternalValueUpper = uint.MaxValue;
        private const double InternalValueStepWidth = ExternalValuePerInternalUnit;
        private const double MaxBaseValueUpper = 1e4;
        private const double overviewTickInterval = 1;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="setter">An action used to set the value when the scale input has been changed.</param>
        /// <param name="getter">A function used to get the value to set the scale input to.</param>
        public PulseGeneratorDurationScaleInputViewModel(IApplianceServices applianceServices,
            Action<uint> setter, Func<uint> getter)
            : base(applianceServices, setter, getter,
                ExternalValuePerInternalUnit, InternalValueUpper, InternalValueStepWidth, MaxBaseValueUpper,
                overviewTickInterval)
        {}
    }
}

