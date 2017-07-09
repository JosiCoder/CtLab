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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides access to a viewmodel of a universal counter (i.e. a counter
    /// used for frequency or period measurements).
    /// </summary>
    public interface IUniversalCounterViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the signal source.
        /// </summary>
        UniversalCounterSource InputSource
        { get; set; }


        /// <summary>
        /// Gets or sets the prescaler mode (<see cref="InputSource"/>) via its
        /// int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingInputSource
        { get; set; }


        /// <summary>
        /// Gets or sets the prescaler mode used to generate the gate time or counter clock.
        /// Setting this value also modifies <see cref="LeastSignificantDigitExponent"/>.
        /// </summary>
        PrescalerMode PrescalerMode
        { get; set; }


        /// <summary>
        /// Gets or sets the prescaler mode (<see cref="PrescalerMode"/>) via its
        /// int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingPrescalerMode
        { get; set; }


        /// <summary>
        /// Gets or sets the exponent of the scale factor. A factor of e.g.
        /// 1000 (1k, 1e3) is represented by an exponent of +3.
        /// </summary>
        int ScaleExponent
        { get; set; }


        /// <summary>
        /// Gets the exponent of the least significant digit for the current
        /// prescaler mode. If, for example, the least significant digit is
        /// 1/10 (0.1), an exponent of -1 is returned.
        /// </summary>
        int LeastSignificantDigitExponent
        { get; }


        /// <summary>
        /// Gets a value indicating whether an overflow has occurred.
        /// </summary>
        bool Overflow
        { get; }


        /// <summary>
        /// Gets a value indicating whether the counter's input signal is active.
        /// </summary>
        bool InputSignalActive
        { get; }


        /// <summary>
        /// Gets the universal counter's value in the according SI unit (Hz or s).
        /// </summary>
        /// <value>The value.</value>
        double Value
        { get; }


        /// <summary>
        /// Gets the frequency readout viewmodel.
        /// </summary>
        IMeasurementValueReadoutViewModel FrequencyReadoutVM
        { get; }


        /// <summary>
        /// Gets the frequency readout viewmodel.
        /// </summary>
        IMeasurementValueReadoutViewModel PeriodReadoutVM
        { get; }

    }
}
