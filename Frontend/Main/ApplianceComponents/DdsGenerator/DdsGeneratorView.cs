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
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
using PB = Praeclarum.Bind;
using System.Collections.Specialized;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend
{
    /// <summary>
    /// Provides the Gtk# view of a DDS generator.
    /// </summary>
    public class DdsGeneratorView: Gtk.Bin
    {
        private static readonly ModAndSyncSourceInfoSet[] _modAndSyncSourceInfoSets = new []
        {
            new ModAndSyncSourceInfoSet(ModulationAndSynchronizationSource.DdsGenerator0, "DDS 1"),
            new ModAndSyncSourceInfoSet(ModulationAndSynchronizationSource.DdsGenerator1, "DDS 2"),
            new ModAndSyncSourceInfoSet(ModulationAndSynchronizationSource.DdsGenerator2, "DDS 3"),
            new ModAndSyncSourceInfoSet(ModulationAndSynchronizationSource.DdsGenerator3, "DDS 4"),
        };

        private readonly IDdsGeneratorViewModel _viewModel;
        [UI] Gtk.ComboBoxText waveformComboBoxText;
        [UI] Gtk.ComboBoxText frequencyScaleComboBoxText;
        [UI] Gtk.ComboBoxText amSourceComboBoxText;
        [UI] Gtk.ComboBoxText fmSourceComboBoxText;
        [UI] Gtk.ComboBoxText fmRangeComboBoxText;
        [UI] Gtk.ComboBoxText pmSourceComboBoxText;
        [UI] Gtk.ComboBoxText syncSourceComboBoxText;
        [UI] Gtk.Widget amOvermodulationIndicator;
        [UI] Gtk.Widget fmOvermodulationIndicator;
        [UI] Gtk.Container frequencyScaleInputContainer;
        [UI] Gtk.Container amplitudeScaleInputContainer;
        [UI] Gtk.Container phaseScaleInputContainer;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        /// <param name="index">The generator index.</param>
        public static DdsGeneratorView Create(IDdsGeneratorViewModel viewModel, int index)
        {
            var builder = new Builder (null, "DdsGeneratorView.glade", null);
            return new DdsGeneratorView (viewModel, index, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="index">The generator index.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private DdsGeneratorView(IDdsGeneratorViewModel viewModel, int index,
            Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            BuildModAndSyncSourceComboBox(amSourceComboBoxText, index);
            BuildModAndSyncSourceComboBox(fmSourceComboBoxText, index);
            BuildModAndSyncSourceComboBox(pmSourceComboBoxText, index);
            BuildModAndSyncSourceComboBox(syncSourceComboBoxText, index);

            //  === Create sub-views. ===

            var frequencyScaleInput = DdsGeneratorFrequencyScaleInputView.Create(_viewModel.FrequencyScaleInputVM);
            frequencyScaleInputContainer.Add(frequencyScaleInput);

            var amplitudeScaleInput = DdsGeneratorAmplitudeScaleInputView.Create(_viewModel.AmplitudeScaleInputVM);
            amplitudeScaleInputContainer.Add(amplitudeScaleInput);

            var periodScaleInput = DdsGeneratorPhaseScaleInputView.Create(_viewModel.PhaseScaleInputVM);
            phaseScaleInputContainer.Add(periodScaleInput);

            // === Register event handlers. ===

            // === Create value converters. ===

            var waveformConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<Waveform>();

            var frequencyScaleExponentConverter = ValueConverterViewModelBuilder
                .BuildInt32ValueConverterViewModel();

            var fmRangeConverter = ValueConverterViewModelBuilder
                .BuildInt32ValueConverterViewModel();

            var amSourceConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<ModulationAndSynchronizationSource>();

            var fmSourceConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<ModulationAndSynchronizationSource>();

            var pmSourceConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<ModulationAndSynchronizationSource>();

            var syncSourceConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<ModulationAndSynchronizationSource>();

            // === Create bindings. ===

            PB.Binding.Create (() => waveformConverter.OriginalValue == _viewModel.BindingWaveform);
            PB.Binding.Create (() => waveformComboBoxText.ActiveId == waveformConverter.DerivedValue);

            PB.Binding.Create (() => amOvermodulationIndicator.Visible == _viewModel.AmplitudeModulationExceedsLimits);
            PB.Binding.Create (() => fmOvermodulationIndicator.Visible == _viewModel.FrequencyModulationExceedsLimits);

            PB.Binding.Create (() => frequencyScaleExponentConverter.OriginalValue == viewModel.FrequencyScaleInputVM.ScaleExponent);
            PB.Binding.Create (() => frequencyScaleComboBoxText.ActiveId == frequencyScaleExponentConverter.DerivedValue);

            PB.Binding.Create (() => fmRangeConverter.OriginalValue == viewModel.BindingMaximumFrequencyModulationRange);
            PB.Binding.Create (() => fmRangeComboBoxText.ActiveId == fmRangeConverter.DerivedValue);

            PB.Binding.Create (() => amSourceConverter.OriginalValue == _viewModel.BindingAmplitudeModulationSource);
            PB.Binding.Create (() => amSourceComboBoxText.ActiveId == amSourceConverter.DerivedValue);

            PB.Binding.Create (() => fmSourceConverter.OriginalValue == _viewModel.BindingFrequencyModulationSource);
            PB.Binding.Create (() => fmSourceComboBoxText.ActiveId == fmSourceConverter.DerivedValue);

            PB.Binding.Create (() => pmSourceConverter.OriginalValue == _viewModel.BindingPhaseModulationSource);
            PB.Binding.Create (() => pmSourceComboBoxText.ActiveId == pmSourceConverter.DerivedValue);

            PB.Binding.Create (() => syncSourceConverter.OriginalValue == _viewModel.BindingSynchronizationSource);
            PB.Binding.Create (() => syncSourceComboBoxText.ActiveId == syncSourceConverter.DerivedValue);
        }

        /// <summary>
        /// Provides modulation and synchronization sources accompanied by some metadata.
        /// </summary>
        private class ModAndSyncSourceInfoSet
        {
            public ModAndSyncSourceInfoSet(ModulationAndSynchronizationSource source, string caption)
            {
                Source = source;
                Caption = caption;
            }
            public readonly ModulationAndSynchronizationSource Source;
            public readonly string Caption;
        }

        /// <summary>
        /// Populates a combo box used for selecting a modulation and synchronization source.
        /// </summary>
        private void BuildModAndSyncSourceComboBox(Gtk.ComboBoxText sourceComboBoxText,
            int index)
        {
            var currentIndex = 0;
            sourceComboBoxText.RemoveAll ();
            _modAndSyncSourceInfoSets.ForEach(infoSet =>
            {
                if (currentIndex != index)
                {
                    sourceComboBoxText.Append(infoSet.Source.ToString(), infoSet.Caption);
                }
                currentIndex++;
            });
            sourceComboBoxText.Append(_modAndSyncSourceInfoSets[index].Source.ToString(), "None");
        }
    }
}