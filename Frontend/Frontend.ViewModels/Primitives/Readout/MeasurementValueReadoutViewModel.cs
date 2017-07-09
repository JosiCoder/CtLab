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
    /// Provides a readout viewmodel for a measurement value.
    /// </summary>
    public class MeasurementValueReadoutViewModel : ReadoutViewModelBase, IMeasurementValueReadoutViewModel
    {
        private readonly string _baseUnitString;

        /// <summary>
        /// Initializes a new instance of this class class.
        /// </summary>
        /// <param name="baseUnitString">A string representing the base unit (e.g. Hz or s).</param>
        public MeasurementValueReadoutViewModel (string baseUnitString)
        {
            _baseUnitString = baseUnitString;
        }

        private Func<int> _scaleExponentProvider;
        /// <summary>
        /// Gets or sets a function providing the exponent of the scale factor.
        /// A factor of e.g. 1000 (1k, 1e3) is represented by an exponent of +3.
        /// </summary>
        public Func<int> ScaleExponentProvider
        {
            get
            {
                return _scaleExponentProvider;
            }
            set
            {
                _scaleExponentProvider = value;
                RaisePropertyChanged ();
                RaisePropertyChanged (() => Text);
            }
        }

        private Func<int> _leastSignificantDigitExponentProvider;
        /// <summary>
        /// Gets or sets a function providing the exponent of the least significant
        /// digit for the current prescaler mode. If, for example, the least
        /// significant digit is 1/10 (0.1), an exponent of -1 is returned.
        /// </summary>
        public Func<int> LeastSignificantDigitExponentProvider
        {
            get
            {
                return _leastSignificantDigitExponentProvider;
            }
            set
            {
                _leastSignificantDigitExponentProvider = value;
                RaisePropertyChanged ();
                RaisePropertyChanged (() => Text);
            }
        }

        private bool _valueWasDerived;
        /// <summary>
        /// Gets or sets a value indicating whether the value was derived
        /// instead of being measured.
        /// </summary>
        public bool ValueWasDerived
        {
            get
            {
                return _valueWasDerived;
            }
            set
            {
                _valueWasDerived = value;
                RaisePropertyChanged ();
                RaisePropertyChanged (() => Text);
            }
        }

        private double _value;
        /// <summary>
        /// Gets or sets the value to be shown in the according base unit (e.g. Hz or s).
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged ();
                RaisePropertyChanged (() => Text);
            }
        }

        /// <summary>
        /// Gets the value to be shown, rendered as a text.
        /// </summary>
        public override string Text
        {
            get
            {
                return BuildValueText();
            }
        }

        /// <summary>
        /// Creates a value text according to the current value and settings like
        /// resolution and scale factor.
        /// </summary>
        private string BuildValueText()
        {
            try
            {
                var scaleExponent = ScaleExponentProvider();
                var leastSignificantDigitExponent = LeastSignificantDigitExponentProvider();

                var scaleFactorInfo = UnitHelper.GetScaleFactorInfo (scaleExponent);

                var decimalPlaces = Math.Max(0, -leastSignificantDigitExponent + scaleExponent);

                var formatString = string.Format("{{0:#,0.{0}}}{{1}} {{2}}",
                    "".PadRight(decimalPlaces, '0'));

                formatString = ValueWasDerived ? string.Format ("({0})", formatString) : formatString;

                return string.Format(formatString, Value / Math.Pow(10, scaleExponent),
                    scaleFactorInfo.ValueSuffix, scaleFactorInfo.UnitPrefix + _baseUnitString);
            }
            catch (Exception)
            {
                return ("-");
            }
        }
    }
}

