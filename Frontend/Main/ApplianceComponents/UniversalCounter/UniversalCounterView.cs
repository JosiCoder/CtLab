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
    /// Provides the Gtk# view of a universal counter.
    /// </summary>
    public class UniversalCounterView: Gtk.Bin
    {
        private readonly IUniversalCounterViewModel _viewModel;
        [UI] Gtk.ComboBoxText inputSourceComboBoxText;
        [UI] Gtk.ComboBoxText prescalerModesComboBoxText;
        [UI] Gtk.ComboBoxText scaleComboBoxText;
        [UI] Gtk.Widget overflowIndicator;
        [UI] Gtk.Widget signalActiveIndicator;
        [UI] Gtk.Container frequencyReadoutContainer;
        [UI] Gtk.Container periodReadoutContainer;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static UniversalCounterView Create(IUniversalCounterViewModel viewModel)
        {
            var builder = new Builder (null, "UniversalCounterView.glade", null);
            return new UniversalCounterView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private UniversalCounterView(IUniversalCounterViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            //  === Create sub-views. ===

            var frequencyReadoutView = ReadoutView.Create(_viewModel.FrequencyReadoutVM);
            frequencyReadoutContainer.Add(frequencyReadoutView);
            var periodReadoutView = ReadoutView.Create(_viewModel.PeriodReadoutVM);
            periodReadoutContainer.Add(periodReadoutView);

            // === Register event handlers. ===

            // === Create value converters. ===

            var inputSourceConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<UniversalCounterSource>();

            var prescalerModeConverter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<PrescalerMode>();

            var scaleExponentConverter = ValueConverterViewModelBuilder
                .BuildInt32ValueConverterViewModel();

            // === Create bindings. ===

            PB.Binding.Create (() => overflowIndicator.Visible == _viewModel.Overflow);
            PB.Binding.Create (() => signalActiveIndicator.Visible == _viewModel.InputSignalActive);

            PB.Binding.Create (() => inputSourceConverter.OriginalValue == _viewModel.BindingInputSource);
            PB.Binding.Create (() => inputSourceComboBoxText.ActiveId == inputSourceConverter.DerivedValue);

            PB.Binding.Create (() => prescalerModeConverter.OriginalValue == _viewModel.BindingPrescalerMode);
            PB.Binding.Create (() => prescalerModesComboBoxText.ActiveId == prescalerModeConverter.DerivedValue);

            PB.Binding.Create (() => scaleExponentConverter.OriginalValue == _viewModel.ScaleExponent);
            PB.Binding.Create (() => scaleComboBoxText.ActiveId == scaleExponentConverter.DerivedValue);
        }
    }
}