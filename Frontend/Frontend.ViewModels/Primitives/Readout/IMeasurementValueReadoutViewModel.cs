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
    /// Provides access to a readout viewmodel for a measurement value.
    /// </summary>
    public interface IMeasurementValueReadoutViewModel : IReadoutViewModel
    {
        /// <summary>
        /// Gets or sets the value to be shown in the according base unit (e.g. Hz or s).
        /// </summary>
        double Value
        { get; set; }


        /// <summary>
        /// Gets or sets a function providing the exponent of the scale factor.
        /// A factor of e.g. 1000 (1k, 1e3) is represented by an exponent of +3.
        /// </summary>
        Func<int> ScaleExponentProvider
        { get; set; }


        /// <summary>
        /// Gets or sets a function providing the exponent of the least significant
        /// digit for the current prescaler mode. If, for example, the least
        /// significant digit is 1/10 (0.1), an exponent of -1 is returned.
        /// </summary>
        Func<int> LeastSignificantDigitExponentProvider
        { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the value was derived
        /// instead of being measured.
        /// </summary>
        bool ValueWasDerived
        { get; set; }

    }
}

