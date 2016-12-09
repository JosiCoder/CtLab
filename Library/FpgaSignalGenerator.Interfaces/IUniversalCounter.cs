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

using System;

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Provides facilities to communicate with a universal counter implemented within a c't Lab FPGA
    /// device configured as an FPGA Lab.
    /// </summary>
    public interface IUniversalCounter
    {
        /// <summary>
        /// Occurs when the value has changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Occurs when the input signal active state has changed.
        /// </summary>
        event EventHandler<InputSignalActiveChangedEventArgs> InputSignalActiveChanged;

        /// <summary>
        /// Gets or sets the signal source.
        /// </summary>
        UniversalCounterSource InputSource { get; set; }

        /// <summary>
        /// Gets or sets the prescaler mode used to generate the gate time or counter clock.
        /// Setting this value also modifies <see cref="MeasurementMode"/>.
        /// </summary>
        PrescalerMode PrescalerMode { get; set; }

        /// <summary>
        /// Gets the measurement mode.
        /// This is modified implicitly when setting <see cref="PrescalerMode"/>.
        /// </summary>
        MeasurementMode MeasurementMode { get; }

        /// <summary>
        /// Gets the exponent of the least significant digit for the current
        /// prescaler mode. If, for example, the least significant digit is
        /// 1/10 (0.1), an exponent of -1 is returned.
        /// </summary>
        int LeastSignificantDigitExponent { get; }

        /// <summary>
		/// Gets the counter's value in Hertz for frequency measurements or in seconds
        /// for period measurements.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Gets a value indicating whether an overflow has occurred.
        /// </summary>
        bool Overflow { get; }

        /// <summary>
		/// Gets a value indicating whether the counter's input signal is active.
        /// </summary>
        bool InputSignalActive { get; }
    }
}