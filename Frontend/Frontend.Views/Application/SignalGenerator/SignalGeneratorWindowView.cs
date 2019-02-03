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
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
using PB = Praeclarum.Bind;
using System.Collections.Specialized;
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend.Views
{
    /// <summary>
    /// Provides the Gtk# view of the signal generator window.
    /// </summary>
    public class SignalGeneratorWindowView: WindowViewBase
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        /// <param name="acceptCloseFunction">
        /// A function handling any request to close the window.
        /// </param>
        public static SignalGeneratorWindowView Create(MainViewModel viewModel, Func<bool> closeRequestHandler)
        {
            var builder = new Builder (null, "SignalGeneratorWindowView.glade", null);
            return new SignalGeneratorWindowView (viewModel, builder,
                builder.GetObject ("mainWidget").Handle, closeRequestHandler);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        /// <param name="closeAction">
        /// A function handling any request to close the window.
        /// </param>
        private SignalGeneratorWindowView(MainViewModel viewModel, Builder builder, IntPtr handle,
            Func<bool> acceptCloseFunction)
            : base (viewModel, builder, handle, acceptCloseFunction)
        {}

        /// <summary>
        /// Creates views for the specified appliance viewmodels.
        /// </summary>
        protected override IEnumerable<ApplianceViewBase> CreateApplianceViews(IEnumerable<IApplianceViewModel> applianceVMs)
        {
            return applianceVMs.Select(vm => SignalGeneratorApplianceView.Create(vm));
        }
    }
}
