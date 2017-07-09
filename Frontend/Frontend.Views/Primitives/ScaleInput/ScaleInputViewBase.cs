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
    /// Provides a base implementation for Gtk# views of scale inputs using a
    /// spin button and a scale (slider).
    /// </summary>
    public abstract class ScaleInputViewBase : Gtk.Bin
    {
        private const int _spinButtonStepsPageRatio = 10;
        private const int _spinButtonStandardAndAcceleratedStepsRatio = 10;
        private const int _numberOfScaleSteps = 100;
        private const int _numberOfScalePages = 10;

        private readonly IScaleInputViewModel _viewModel;
        [UI] Gtk.Scale scale;
        [UI] Gtk.SpinButton spinButton;
        [UI] Gtk.Adjustment scaleAdjustment;
        [UI] Gtk.Adjustment spinButtonAdjustment;

        /// <summary>
        /// Creates a Gtk# builder for a scale input view and loads the main widget.
        /// </summary>
        /// <param name="resourceName">The resource to create the builder from.</param>
        /// <returns>The Gtk# builder and the main widget's handle.</returns>
        protected static Tuple<Builder, IntPtr>  CreateBuilderAndMainWidgetHandle(string resourceName)
        {
            var builder = new Builder(null, resourceName, null);
            var handle = builder.GetObject("mainWidget").Handle;
            return new Tuple<Builder, IntPtr>(builder, handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        protected ScaleInputViewBase(IScaleInputViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            // === Configure the controls. ===

            ConfigureSpinButtonAdjustment ();
            ConfigureScaleAdjustment ();

            // === Register event handlers. ===

            SpinButtonAdjustment.Changed += (sender, e) =>
            {
                ConfigureSpinButtonAdjustment();
            };

            ScaleAdjustment.Changed += (sender, e) =>
            {
                ConfigureScaleAdjustment();
            };

            // === Create value converters. ===

            var spinButtonValueConverter = new ValueConverterViewModel<double, double>(
                val => val,
                val => val);

            var scaleValueConverter = new ValueConverterViewModel<double, double>(
                val => val,
                val => val);

            var spinButtonPageIncrementConverter = new ValueConverterViewModel<double, double>(
                val => _spinButtonStepsPageRatio * val,
                val => val / _spinButtonStepsPageRatio /* derived to original is unused */);

            var scaleStepIncrementConverter = new ValueConverterViewModel<double, double>(
                val => val / _numberOfScaleSteps,
                val => val * _numberOfScaleSteps /* derived to original is unused */);

            var scalePageIncrementConverter = new ValueConverterViewModel<double, double>(
                val => val / _numberOfScalePages,
                val => val * _numberOfScalePages /* derived to original is unused */);

            var scaleLowerConverter = CreateBaseToOverviewValueConverter();

            var scaleUpperConverter = CreateBaseToOverviewValueConverter();

            // === Create bindings. ===

            // Bind the spin button's value to the base value.
            PB.Binding.Create (() => spinButtonAdjustment.Value == spinButtonValueConverter.DerivedValue);
            PB.Binding.Create (() => spinButtonValueConverter.OriginalValue == viewModel.BaseValue);
            // Bind the spin button's lower boundary to base lower value.
            PB.Binding.Create (() => SpinButtonAdjustment.Lower == viewModel.BaseValueLower);
            // Bind the spin button's upper boundary to base upper value.
            PB.Binding.Create (() => SpinButtonAdjustment.Upper == viewModel.BaseValueUpper);
            // Bind the spin button's step increment.
            PB.Binding.Create (() => SpinButtonAdjustment.StepIncrement == viewModel.BaseValueStepWidth);
            // Bind the spin button's page increment.
            PB.Binding.Create (() => spinButtonPageIncrementConverter.OriginalValue == SpinButtonAdjustment.StepIncrement);
            PB.Binding.Create (() => SpinButtonAdjustment.PageIncrement == spinButtonPageIncrementConverter.DerivedValue);

            // Bind the scale's value to the overview value.
            PB.Binding.Create (() => scaleAdjustment.Value == scaleValueConverter.DerivedValue);
            PB.Binding.Create (() => scaleValueConverter.OriginalValue == viewModel.OverviewValue);
            // Bind scale's lower boundary to base lower value.
            PB.Binding.Create (() => scaleLowerConverter.OriginalValue == viewModel.BaseValueLower);
            PB.Binding.Create (() => ScaleAdjustment.Lower == scaleLowerConverter.DerivedValue);
            // Bind scale's upper boundary to base upper value.
            PB.Binding.Create (() => scaleUpperConverter.OriginalValue == viewModel.BaseValueUpper);
            PB.Binding.Create (() => ScaleAdjustment.Upper == scaleUpperConverter.DerivedValue);
            // Bind the scale's step increment.
            PB.Binding.Create (() => scaleStepIncrementConverter.OriginalValue == ScaleAdjustment.Upper);
            PB.Binding.Create (() => ScaleAdjustment.StepIncrement == scaleStepIncrementConverter.DerivedValue);
            // Bind the scale's page increment.
            PB.Binding.Create (() => scalePageIncrementConverter.OriginalValue == ScaleAdjustment.Upper);
            PB.Binding.Create (() => ScaleAdjustment.PageIncrement == scalePageIncrementConverter.DerivedValue);
        }

        /// <summary>
        /// Configures the spin button adjustment.
        /// </summary>
        private void ConfigureSpinButtonAdjustment()
        {
            // Set the decimal places of the spin button.
            spinButton.Digits = _viewModel.BaseValueDecimalPlaces;
            // Set the accelerated step increment of the spin button.
            spinButton.ClimbRate = _spinButtonStandardAndAcceleratedStepsRatio * _viewModel.BaseValueStepWidth;
        }

        /// <summary>
        /// Configures the scale adjustment.
        /// </summary>
        private void ConfigureScaleAdjustment()
        {
            var tickInterval = _viewModel.OverviewTickInterval;

            // Start the ticks at the first integer multiple of the interval.
            var firstTick =
                Math.Ceiling(ScaleAdjustment.Lower / tickInterval) * tickInterval;

            Scale.ClearMarks();
            for (var tickVal = firstTick; tickVal <= ScaleAdjustment.Upper;
                tickVal += tickInterval)
            {
                Scale.AddMark(tickVal, PositionType.Top, null);
            }
        }

        /// <summary>
        /// Creates a converter that can be used to convert base to overview values 
        /// and vice versa.
        /// </summary>
        /// <returns>a new base value to overview value converter.</returns>
        protected ValueConverterViewModel<double, double> CreateBaseToOverviewValueConverter()
        {
            return _viewModel.OverviewValueIsLogarithmic
                ? new ValueConverterViewModel<double, double>(
                    val => Math.Log10(val),
                    val => Math.Pow(10, val) /* derived to original is unused */)
                : new ValueConverterViewModel<double, double>(
                        val => val,
                        val => val /* derived to original is unused */);
        }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        protected Gtk.Scale Scale
        {
            get
            {
                return scale;
            }
        }

        /// <summary>
        /// Gets the spin button.
        /// </summary>
        protected Gtk.SpinButton SpinButton
        {
            get
            {
                return spinButton;
            }
        }

        /// <summary>
        /// Gets the scale adjustment.
        /// </summary>
        protected Gtk.Adjustment ScaleAdjustment
        {
            get
            {
                return scaleAdjustment;
            }
        }

        /// <summary>
        /// Gets the spin button adjustment.
        /// </summary>
        protected Gtk.Adjustment SpinButtonAdjustment
        {
            get
            {
                return spinButtonAdjustment;
            }
        }
   }
}