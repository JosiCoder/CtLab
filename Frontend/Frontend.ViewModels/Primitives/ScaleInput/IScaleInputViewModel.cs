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
using System.ComponentModel;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides access to a scale input viewmodel using a value represented by
    /// a base value and an exponent.
    /// </summary>
    public interface IScaleInputViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the value to be shown on base value controls.
        /// </summary>
        double BaseValue
        { get; set; }

        /// <summary>
        /// Gets the step width used by base value controls.
        /// </summary>
        double BaseValueStepWidth
        { get; }

        /// <summary>
        /// Gets the number of decimal places base value controls must use
        /// to handle the least significant digit according to the current
        /// base value step width.
        /// </summary>
        uint BaseValueDecimalPlaces
        { get; }

        /// <summary>
        /// Gets the lower boundary used by base value controls.
        /// </summary>
        double BaseValueLower
        { get; }

        /// <summary>
        /// Gets the upper boundary used by base value controls.
        /// </summary>
        double BaseValueUpper
        { get; }

        /// <summary>
        /// Gets or sets the exponent of the scale factor. A factor of e.g.
        /// 1000 (1k, 1e3) is represented by an exponent of +3.
        /// </summary>
        int ScaleExponent
        { get; set; }

        /// <summary>
        /// Gets or sets the value shown on overview value controls.
        /// </summary>
        double OverviewValue
        { get; set; }

        /// <summary>
        /// Gets the linear or logarithmic interval of the ticks shown on
        /// overview value controls.
        /// </summary>
        double OverviewTickInterval
        { get; }

        /// <summary>
        /// Gets a value indicating whether the overview value is logarithmic.
        /// </summary>
        bool OverviewValueIsLogarithmic
        { get; }
    }
}

