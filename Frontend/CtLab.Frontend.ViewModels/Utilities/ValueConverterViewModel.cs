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
    /// Converts original to derived values and vice versa.
    /// </summary>
    /// <typeparam name="TOriginalValue">The type of the original values.</param>
    /// <typeparam name="TDerivedValue">The type of the derived values.</param>
    public class ValueConverterViewModel<TOriginalValue, TDerivedValue> : ViewModelBase
    {
        private readonly Func<TOriginalValue, TDerivedValue> _originalToDerivedValueConverter;
        private readonly Func<TDerivedValue, TOriginalValue> _derivedToOriginalValueConverter;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="originalToDerivedValueConverter">
        /// The function used to convert an original value to its derived counterpart.
        /// </param>
        /// <param name="derivedToOriginalValueConverter">
        /// The function used to convert a derived value to its original counterpart.
        /// </param>
        public ValueConverterViewModel (
            Func<TOriginalValue, TDerivedValue> originalToDerivedValueConverter,
            Func<TDerivedValue, TOriginalValue> derivedToOriginalValueConverter)
        {
            _originalToDerivedValueConverter = originalToDerivedValueConverter;
            _derivedToOriginalValueConverter = derivedToOriginalValueConverter;
        }

        /// <summary>
        /// Gets or sets the orifginal value.
        /// </summary>
        private TOriginalValue _originalValue;
        public TOriginalValue OriginalValue
        {
            get
            {
                return _originalValue;
            }
            set
            {
                _originalValue = value;
                RaisePropertyChanged ();
                RaisePropertyChanged (() => DerivedValue);
            }
        }

        /// <summary>
        /// Gets or sets the derived value.
        /// </summary>
        public TDerivedValue DerivedValue
        {
            get
            {
                try
                {
                    return _originalToDerivedValueConverter(OriginalValue);
                }
                catch
                {
                    return default(TDerivedValue);
                }
            }
            set
            {
                try
                {
                    OriginalValue = _derivedToOriginalValueConverter(value);
                }
                catch
                {
                    OriginalValue = default(TOriginalValue);
                }
            }
        }
    }
}

