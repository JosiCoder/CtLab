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
    /// Provides a scale input viewmodel for a value represented by
    /// a linear base and a logarithmic (log10) overview value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be shown.</param>
    public class LinLogScaleInputViewModel<TValue>
        : ScaleInputViewModelBase<TValue>, IScaleInputViewModel
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="setter">An action used to set the value when the scale input has been changed.</param>
        /// <param name="getter">A function used to get the value to set the scale input to.</param>
        /// <param name="externalValuePerInternalUnit">
        /// The external value represented by each internal unit.
        /// </param>
        /// <param name="internalValueUpper">The maximum of the internal value.</param>
        /// <param name="internalValueStepWidth">The internal value's step width used on base value controls.</param>
        /// <param name="maxBaseValueUpper">The maximum upper value shown on base value controls.</param>
        /// <param name="overviewTickInterval">
        /// The linear or logarithmic interval of the ticks shown on overview value controls.
        /// </param>
        public LinLogScaleInputViewModel(IApplianceServices applianceServices,
            Action<TValue> setter, Func<TValue> getter,
            double externalValuePerInternalUnit, TValue internalValueUpper,
            double internalValueStepWidth, double maxBaseValueUpper,
            double overviewTickInterval)
            : base(applianceServices, setter, getter, externalValuePerInternalUnit, internalValueUpper,
                internalValueStepWidth, maxBaseValueUpper, overviewTickInterval)
        {
        }

        /// <summary>
        /// Gets or sets the value shown on overview value controls.
        /// </summary>
        public override double OverviewValue
        {
            get
            {
                return Math.Log10(BaseValue);
            }
            set
            {
                BaseValue = Math.Pow(10, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the overview value is logarithmic.
        /// </summary>
        public override bool OverviewValueIsLogarithmic
        { get { return true; } }

        /// <summary>
        /// Gets the lower boundary used by base value controls.
        /// </summary>
        public override double BaseValueLower
        {
            get
            {
                return BaseValueStepWidth;
            }
        }
    }
}

