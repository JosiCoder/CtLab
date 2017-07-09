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
using System.Linq;
using System.ComponentModel;
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
using PB = Praeclarum.Bind;
using System.Collections.Specialized;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the Gtk# view of a signal generator.
    /// </summary>
    public class SignalGeneratorView: Gtk.Bin
    {
        private readonly ISignalGeneratorViewModel _viewModel;
        [UI] Gtk.ComboBoxText outputSource0ComboBoxText;
        [UI] Gtk.ComboBoxText outputSource1ComboBoxText;
        [UI] Gtk.Container universalCounterContainer;
        [UI] Gtk.Container pulseGeneratorContainer;
        [UI] Gtk.Container ddsGenerator0Container;
        [UI] Gtk.Container ddsGenerator1Container;
        [UI] Gtk.Container ddsGenerator2Container;
        [UI] Gtk.Container ddsGenerator3Container;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        public static SignalGeneratorView Create(ISignalGeneratorViewModel viewModel)
        {
            var builder = new Builder (null, "SignalGeneratorView.glade", null);
            return new SignalGeneratorView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private SignalGeneratorView(ISignalGeneratorViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            // === Create sub-views. ===

            var universalCounterView = UniversalCounterView.Create(_viewModel.UniversalCounterVM);
            universalCounterContainer.Add(universalCounterView);

            var pulseGeneratorView =  PulseGeneratorView.Create(_viewModel.PulseGeneratorVM);
            pulseGeneratorContainer.Add(pulseGeneratorView);

            var ddsGeneratorViews = _viewModel.DdsGeneratorVMs
                .Select ((ddsGeneratorVM, index) => DdsGeneratorView.Create (ddsGeneratorVM, index));
            var ddsGeneratorViewsEnumerator = ddsGeneratorViews.GetEnumerator ();

            var ddsGeneratorContainers = new []
            {
                ddsGenerator0Container,
                ddsGenerator1Container,
                ddsGenerator2Container,
                ddsGenerator3Container
            };
            ddsGeneratorContainers.ForEach(container =>
            {
                if (ddsGeneratorViewsEnumerator.MoveNext())
                {
                    container.Add(ddsGeneratorViewsEnumerator.Current);
                }
            });

            // === Register event handlers. ===

            // === Create value converters. ===

            var outputSource0Converter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<OutputSource>();

            var outputSource1Converter = ValueConverterViewModelBuilder
                .BuildEnumValueConverterViewModel<OutputSource>();

            // === Create bindings. ===

            PB.Binding.Create (() => outputSource0Converter.OriginalValue == _viewModel.BindingOutputSource0);
            PB.Binding.Create (() => outputSource0ComboBoxText.ActiveId == outputSource0Converter.DerivedValue);

            PB.Binding.Create (() => outputSource1Converter.OriginalValue == _viewModel.BindingOutputSource1);
            PB.Binding.Create (() => outputSource1ComboBoxText.ActiveId == outputSource1Converter.DerivedValue);
        }
    }
}