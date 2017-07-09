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

namespace CtLab.Frontend.Views
{
    /// <summary>
    /// Provides the Gtk# view of a universal counter.
    /// </summary>
    public class PulseGeneratorView: Gtk.Bin
    {
        private readonly IPulseGeneratorViewModel _viewModel;
        [UI] Gtk.Container pulseScaleInputContainer;
        [UI] Gtk.Container pauseScaleInputContainer;
        [UI] Gtk.ComboBoxText pulseScaleComboBoxText;
        [UI] Gtk.ComboBoxText pauseScaleComboBoxText;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static PulseGeneratorView Create(IPulseGeneratorViewModel viewModel)
        {
            var builder = new Builder (null, "PulseGeneratorView.glade", null);
            return new PulseGeneratorView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private PulseGeneratorView(IPulseGeneratorViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            //  === Create sub-views. ===

            var pulseScaleInputView = PulseGeneratorDurationScaleInputView.Create(_viewModel.PulseScaleInputVM);
            pulseScaleInputContainer.Add(pulseScaleInputView);

            var pauseScaleInputView = PulseGeneratorDurationScaleInputView.Create(_viewModel.PauseScaleInputVM);
            pauseScaleInputContainer.Add(pauseScaleInputView);

            // === Register event handlers. ===

            // === Create value converters. ===

            var pulseScaleExponentConverter = ValueConverterViewModelBuilder
                .BuildInt32ValueConverterViewModel();

            var pauseScaleExponentConverter = ValueConverterViewModelBuilder
                .BuildInt32ValueConverterViewModel();

            // === Create bindings. ===

            PB.Binding.Create (() => pulseScaleExponentConverter.OriginalValue == viewModel.PulseScaleInputVM.ScaleExponent);
            PB.Binding.Create (() => pulseScaleComboBoxText.ActiveId == pulseScaleExponentConverter.DerivedValue);

            PB.Binding.Create (() => pauseScaleExponentConverter.OriginalValue == viewModel.PauseScaleInputVM.ScaleExponent);
            PB.Binding.Create (() => pauseScaleComboBoxText.ActiveId == pauseScaleExponentConverter.DerivedValue);
        }
    }
}