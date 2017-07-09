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
    /// Provides access to a viewmodel of a DDS generator.
    /// </summary>
    public interface IDdsGeneratorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the frequency scale input viewmodel.
        /// </summary>
        IScaleInputViewModel FrequencyScaleInputVM
        { get; }

        /// <summary>
        /// Gets or sets the signal waveform.
        /// </summary>
        Waveform Waveform
        { get; set; }

        /// <summary>
        /// Gets or sets the signal waveform (<see cref="Waveform"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingWaveform
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum frequency modulation range. Accepts values in the range of 0..5.
        /// If set to n, modulation depth is +/-(<see cref="MaximumFrequency"/> / 2 / 8^(5-n)),
        /// e.g. +/-6103,5 Hz for n=1 and MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="MaximumFrequencyModulationDepth"/>.
        /// </summary>
        ushort MaximumFrequencyModulationRange
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum frequency modulation range (<see cref="MaximumFrequencyModulationRange"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingMaximumFrequencyModulationRange
        { get; set; }

        /// <summary>
        /// Gets the maximum frequency modulation depth in Hertz.
        /// This is modified by setting <see cref="MaximumFrequencyModulationRange"/>.
        /// </summary>
        double MaximumFrequencyModulationDepth { get; }

        /// <summary>
        /// Gets or sets the synchronization source. If this value ist set to the current
        /// instance itself, no synchronization takes place.
        /// </summary>
        ModulationAndSynchronizationSource SynchronizationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the synchronization source (<see cref="SynchronizationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingSynchronizationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the phase modulation source. If this value ist set to the current
        /// instance itself, no phase modulation takes place.
        /// </summary>
        ModulationAndSynchronizationSource PhaseModulationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the phase modulation source (<see cref="PhaseModulationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingPhaseModulationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the frequency modulation source. If this value ist set to the current
        /// instance itself, no frequency modulation takes place.
        /// </summary>
        ModulationAndSynchronizationSource FrequencyModulationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the frequency modulation source (<see cref="FrequencyModulationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingFrequencyModulationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the amplitude modulation source. If this value ist set to the current
        /// instance itself, no amplitude modulation takes place.
        /// </summary>
        ModulationAndSynchronizationSource AmplitudeModulationSource
        { get; set; }

        /// <summary>
        /// Gets or sets the amplitute modulation source (<see cref="AmplitudeModulationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        int BindingAmplitudeModulationSource
        { get; set; }

        /// <summary>
        /// Gets the amount the amplitude is swinging around its center value.
        /// </summary>
        double AmplitudeModulationSwing
        { get; }

        /// <summary>
        /// Gets the relative amount the amplitude is swinging around its center value.
        /// </summary>
        double RelativeAmplitudeModulationSwing
        { get; }

        /// <summary>
        /// Gets a value indicating whether amplitude modulation exceeds its limits.
        /// </summary>
        bool AmplitudeModulationExceedsLimits
        { get; }

        /// <summary>
        /// Gets the amount the frequency is swinging around its center value.
        /// </summary>
        double FrequencyModulationSwing
        { get; }

        /// <summary>
        /// Gets the relative amount the frequency is swinging around its center value.
        /// </summary>
        double RelativeFrequencyModulationSwing
        { get; }

        /// <summary>
        /// Gets a value indicating whether frequency modulation exceeds its limits.
        /// </summary>
        bool FrequencyModulationExceedsLimits
        { get; }

        /// <summary>
        /// Gets the amplitude scale input viewmodel.
        /// </summary>
        IScaleInputViewModel AmplitudeScaleInputVM
        { get; }

        /// <summary>
        /// Gets the phase scale input viewmodel.
        /// </summary>
        IScaleInputViewModel PhaseScaleInputVM
        { get; }

        /// <summary>
        /// Occurs when a property has changed that could influence the
        /// current modulation properties of other DDS generators.
        /// </summary>
        event EventHandler ModulationPropertyChanged;

        /// <summary>
        /// Informs the object that a property of another DDS generator has changed that
        /// could influence its modulation properties.
        /// </summary>
        void OtherDdsGeneratorModulationHasChanged();
    }
}
