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
    /// Provides the Gtk# view of a readout.
    /// </summary>
    public class ReadoutView: Gtk.Bin
    {
        private readonly IReadoutViewModel _viewModel;
        [UI] Gtk.Container innerReadoutContainer;
        [UI] Gtk.Container outerReadoutContainer;
        [UI] Gtk.Label readoutLabel;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static ReadoutView Create(IReadoutViewModel viewModel)
        {
            var builder = new Builder (null, "ReadoutView.glade", null);
            return new ReadoutView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private ReadoutView(IReadoutViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            innerReadoutContainer.OverrideBackgroundColor(StateFlags.Normal,
                new Gdk.RGBA
                {
                    Red = 255,
                    Green = 255,
                    Blue = 255,
                    Alpha = 255,
                });

            outerReadoutContainer.OverrideBackgroundColor(StateFlags.Normal,
                new Gdk.RGBA
                {
                    Red = 0,
                    Green = 0,
                    Blue = 255,
                    Alpha = 255,
                });

            // === Create bindings. ===

            PB.Binding.Create (() => readoutLabel.Text == _viewModel.Text);
        }
    }
}