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
    /// Provides the Gtk# view of a scale input representing a duration value of a
    /// pulse generator.
    /// </summary>
    public class PulseGeneratorDurationScaleInputView : ScaleInputViewBase
    {
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static PulseGeneratorDurationScaleInputView Create(IPulseGeneratorDurationScaleInputViewModel viewModel)
        {
            var builderAndHandle = CreateBuilderAndMainWidgetHandle("HorizontalScaleInputView.glade");
            return new PulseGeneratorDurationScaleInputView (viewModel,
                builderAndHandle.Item1, builderAndHandle.Item2);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        private PulseGeneratorDurationScaleInputView(IPulseGeneratorDurationScaleInputViewModel viewModel, Builder builder, IntPtr handle)
            : base (viewModel, builder, handle)
        {}
   }
}