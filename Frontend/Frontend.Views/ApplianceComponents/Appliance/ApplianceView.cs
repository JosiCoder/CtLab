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
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the Gtk# view of an appliance.
    /// </summary>
    public class ApplianceView: Gtk.Bin
    {
        private readonly IApplianceViewModel _viewModel;
        [UI] Gtk.Label connectionLabel;
        [UI] Gtk.Widget connectionActiveIndicator;
        [UI] Gtk.Container signalGeneratorContainer;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static ApplianceView Create(IApplianceViewModel viewModel)
        {
            var builder = new Builder (null, "ApplianceView.glade", null);
            return new ApplianceView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private ApplianceView(IApplianceViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            // === Create sub-views. ===

            var signalGenerator =  SignalGeneratorView.Create(_viewModel.SignalGeneratorVM);
            signalGeneratorContainer.Add(signalGenerator);

            // === Create bindings. ===

            PB.Binding.Create (() => connectionLabel.Text == _viewModel.ConnectionDescription);
            PB.Binding.Create (() => connectionActiveIndicator.Visible == _viewModel.IsConnectionActive);
        }
    }
}