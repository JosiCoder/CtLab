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

namespace CtLab.FpgaSignalGenerator.Interfaces
{
    /// <summary>
    /// Provides a signal source.
    /// </summary>
    public interface IDdsSignalSource
    {
    }

    /// <summary>
    /// Provides a signal whose amplitude can be modulated.
    /// </summary>
    public interface IAmplitudeModulationCarrierSource : IDdsSignalSource
    {
        /// <summary>
        /// Gets the maximum amplitude in internal units (not Volt or dB!).
        /// </summary>
        short MaximumAmplitude { get; }

        /// <summary>
        /// Gets or sets the amplitude. Range is +/- <see cref="MaximumAmplitude"/>.
        /// </summary>
        short Amplitude { get; set; }
    }

    /// <summary>
    /// Provides a signal whose frequency can be modulated.
    /// </summary>
    public interface IFrequencyModulationCarrierSource
    {
        /// <summary>
        /// Gets maximum frequency modulation depth in Hertz.
        /// </summary>
        double MaximumFrequencyModulationDepth { get; }

        /// <summary>
        /// Gets the maximum frequency in Hz.
        /// </summary>
        double MaximumFrequency { get; }

        /// <summary>
        /// Gets or sets the frequency in Hertz.
        /// Accepts values in the range of 0..<see cref="MaximumFrequency"/>.
        /// </summary>
        double Frequency { get; set; }
    }

    /// <summary>
    /// Provides a signal used to modulate a paramter of an other signal.
    /// </summary>
    public interface IModulatorSource : IDdsSignalSource
    {
        /// <summary>
        /// Gets or sets the amplitude. Range is +/- maximum output voltage.
        /// </summary>
        short Amplitude { get; set; }
    }

    /// <summary>
    /// Provides a signal used to modulate the amplitude of an other signal.
    /// </summary>
    public interface IAmplitudeModulatorSource : IModulatorSource
    {
    }

    /// <summary>
    /// Provides a signal used to modulate the frequency of an other signal.
    /// </summary>
    public interface IFrequencyModulatorSource : IModulatorSource
    {
        /// <summary>
        /// Gets the maximum amplitude in internal units (not Volt or dB!).
        /// </summary>
        short MaximumAmplitude { get; }
    }
}