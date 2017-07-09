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
    /// Provides a base implementation for scale input viewmodel
    /// using a value represented by a base value and an exponent.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be shown.</param>
    public abstract class ScaleInputViewModelBase<TValue>
        : ViewModelBase, IScaleInputViewModel
    {
        protected readonly Action<TValue> Setter;
        protected readonly Func<TValue> Getter;

        private readonly double _externalValuePerInternalUnit;
        private readonly TValue _internalValueUpper;
        private readonly double _internalValueStepWidth;
        private readonly double _maxBaseValueUpper;

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
        protected ScaleInputViewModelBase(IApplianceServices applianceServices, 
            Action<TValue> setter, Func<TValue> getter,
            double externalValuePerInternalUnit, TValue internalValueUpper,
            double internalValueStepWidth, double maxBaseValueUpper,
            double overviewTickInterval)
            : base(applianceServices)
        {
            Setter = setter;
            Getter = getter;

            _externalValuePerInternalUnit = externalValuePerInternalUnit;
            _internalValueStepWidth = internalValueStepWidth;
            _internalValueUpper = internalValueUpper;
            _maxBaseValueUpper = maxBaseValueUpper;
            OverviewTickInterval = overviewTickInterval;
        }

        /// <summary>
        /// Gets or sets the value to be shown.
        /// </summary>
        public virtual TValue InternalValue
        {
            get
            {
                return Getter();
            }
            set
            {
                Setter(value);
                Flush();
                RaisePropertyChanged ();
                RaisePropertyChanged(() => BaseValue);
                RaisePropertyChanged(() => OverviewValue);
            }
        }

        /// <summary>
        /// Gets or sets the value to be shown on base value controls.
        /// </summary>
        public double BaseValue
        {
            get
            {
                var valueAsDouble = Convert.ToDouble (InternalValue);
                return valueAsDouble * BaseValueMultiplier;
            }
            set
            {
              //  var decimalPlaces = Math.Max(0, -leastSignificantDigitExponent + scaleExponent);
                var decimalPlaces = 3;
                var valueAsDouble = Math.Round (value / BaseValueMultiplier, decimalPlaces);
                InternalValue = (TValue)Convert.ChangeType (valueAsDouble, typeof(TValue));
            }
        }

        /// <summary>
        /// Gets or sets the value shown on overview value controls.
        /// </summary>
        public abstract double OverviewValue
        { get; set; }

        /// <summary>
        /// Gets the linear or logarithmic interval of the ticks shown on
        /// overview value controls.
        /// </summary>
        public double OverviewTickInterval
        { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the overview value is logarithmic.
        /// </summary>
        public abstract bool OverviewValueIsLogarithmic
        { get; }

        /// <summary>
        /// Gets the step width used by base value controls.
        /// </summary>
        public double BaseValueStepWidth
        {
            get
            {
                return _internalValueStepWidth * Math.Pow (10, -ScaleExponent);
            }
        }

        /// <summary>
        /// Gets the number of decimal places base value controls must use
        /// to handle the least significant digit according to the current
        /// base value step width.
        /// </summary>
        public uint BaseValueDecimalPlaces
        {
            get
            {
                var leastSignificantDigitExponent = Math.Log10(BaseValueStepWidth);
                return (uint)Math.Ceiling(Math.Max(0, -leastSignificantDigitExponent));
            }
        }

        /// <summary>
        /// Gets the lower boundary used by base value controls.
        /// </summary>
        public abstract double BaseValueLower
        { get; }

        /// <summary>
        /// Gets the upper boundary used by base value controls.
        /// </summary>
        public double BaseValueUpper
        {
            get
            {
                var valueUpperAsDouble = Convert.ToDouble (_internalValueUpper);
                var maxBaseValueUpperForCurrentMultiplier = valueUpperAsDouble * BaseValueMultiplier;
                return Math.Min(maxBaseValueUpperForCurrentMultiplier, _maxBaseValueUpper);
            }
        }

        private int _scaleExponent;
        /// <summary>
        /// Gets or sets the exponent of the scale factor. A factor of e.g.
        /// 1000 (1k, 1e3) is represented by an exponent of +3.
        /// </summary>
        public int ScaleExponent
        {
            get
            {
                return _scaleExponent;
            }
            set
            {
                _scaleExponent = value;

                // Restrict the current value to the new limit.
                BaseValue = Math.Min(BaseValue, BaseValueUpper);

                RaisePropertyChanged();
                RaisePropertyChanged(() => BaseValue);
                RaisePropertyChanged(() => OverviewValue);
                RaisePropertyChanged(() => BaseValueLower);
                RaisePropertyChanged(() => BaseValueUpper);
                RaisePropertyChanged(() => BaseValueStepWidth);
            }
        }

        /// <summary>
        /// Gets the multiplier the internal value must be multiplied with to get the value
        /// shown on base value controls.
        /// </summary>
        protected double BaseValueMultiplier
        {
            get
            {
                return _externalValuePerInternalUnit * Math.Pow(10, -ScaleExponent);
            }
        }
    }
}

