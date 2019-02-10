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

namespace CtLab.Frontend.Views
{
    /// <summary>
    /// Provides the Gtk# view of a scope appliance.
    /// </summary>
    public class ScopeApplianceView: ApplianceViewBase
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static ScopeApplianceView Create(IApplianceViewModel viewModel)
        {
            var builder = new Builder (null, "ScopeApplianceView.glade", null);
            return new ScopeApplianceView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        protected ScopeApplianceView(IApplianceViewModel viewModel, Builder builder, IntPtr handle)
            : base(viewModel, builder, handle)
        {}

        /// <summary>
        /// Creates the main part's view.
        /// </summary>
        protected override Gtk.Bin CreateMainPartView (IApplianceViewModel viewModel)
        {
            return ScopeView.Create(viewModel.ScopeVM);
        }
    }
}

