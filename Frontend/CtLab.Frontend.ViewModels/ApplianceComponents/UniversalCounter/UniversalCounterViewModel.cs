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
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the viewmodel of a universal counter (i.e. a counter used for
    /// frequency or period measurements).
    /// </summary>
    public class UniversalCounterViewModel : ViewModelBase, IUniversalCounterViewModel
    {
        private readonly IUniversalCounter _universalCounter;
        private readonly IMeasurementValueReadoutViewModel _frequencyReadoutVM;
        private readonly IMeasurementValueReadoutViewModel _periodReadoutVM;

        private IMeasurementValueReadoutViewModel _measuredValueReadoutVM;
        private IMeasurementValueReadoutViewModel _derivedValueReadoutVM;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="universalCounter">The universal counter to use.</param>
        /// <param name="frequencyReadoutVM">The readout viewmodel for the frequency.</param>
        /// <param name="periodReadoutVM">The readout viewmodel for the period.</param>
        public UniversalCounterViewModel (IApplianceServices applianceServices, IUniversalCounter universalCounter,
            IMeasurementValueReadoutViewModel frequencyReadoutVM, IMeasurementValueReadoutViewModel periodReadoutVM)
            : base(applianceServices)
        {
            _universalCounter = universalCounter;

            _frequencyReadoutVM = frequencyReadoutVM;
            _periodReadoutVM = periodReadoutVM;

            ConfigureReadoutVMs();

            _universalCounter.ValueChanged +=
                (sender, e) => DispatchOnUIThread(() =>
                    {
                        System.Diagnostics.Debug.WriteLine ("Counter value has changed to {0}", Value);

                        _measuredValueReadoutVM.Value = Value;
                        _derivedValueReadoutVM.Value = 1/Value;

                        RaisePropertyChanged(() => Value);
                        RaisePropertyChanged(() => Overflow);
                    });

            _universalCounter.InputSignalActiveChanged +=
                (sender, e) => DispatchOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => InputSignalActive);
                    });
        }

        /// <summary>
        /// Gets or sets the signal source.
        /// </summary>
        public UniversalCounterSource InputSource
        {
            get
            {
                return _universalCounter.InputSource;
            }
            set
            {
                _universalCounter.InputSource = value;
                Flush();
                RaisePropertyChanged();
                RaisePropertyChanged(() => BindingInputSource);
            }
        }

        /// <summary>
        /// Gets or sets the prescaler mode (<see cref="InputSource"/>) via its
        /// int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingInputSource
        {
            get
            {
                return (int)InputSource;
            }
            set
            {
                InputSource = (UniversalCounterSource)Enum.ToObject(typeof(UniversalCounterSource), value);
            }
        }

        /// <summary>
        /// Gets or sets the prescaler mode used to generate the gate time or counter clock.
        /// Setting this value also modifies <see cref="LeastSignificantDigitExponent"/>.
        /// </summary>
        public PrescalerMode PrescalerMode
        {
            get
            {
                return _universalCounter.PrescalerMode;
            }
            set
            {
                _universalCounter.PrescalerMode = value;
                Flush();
                ConfigureReadoutVMs();
                RaisePropertyChanged();
                RaisePropertyChanged(() => BindingPrescalerMode);
            }
        }

        /// <summary>
        /// Gets or sets the prescaler mode (<see cref="PrescalerMode"/>) via its
        /// int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingPrescalerMode
        {
            get
            {
                return (int)PrescalerMode;
            }
            set
            {
                PrescalerMode = (PrescalerMode)Enum.ToObject(typeof(PrescalerMode), value);
            }
        }

        private int _scaleExponent;
        /// <summary>
        /// Gets or sets the exponent of the scale factor. A factor of e.g.
        /// 1000 (1k, 1e3) is represented by an exponent of +3.
        /// </summary>
        public int ScaleExponent
        {
            get
            {
                return _scaleExponent;
            }
            set
            {
                _scaleExponent = value;
                ConfigureReadoutVMs();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the exponent of the least significant digit for the current
        /// prescaler mode. If, for example, the least significant digit is
        /// 1/10 (0.1), an exponent of -1 is returned.
        /// </summary>
        public int LeastSignificantDigitExponent
        {
            get { return _universalCounter.LeastSignificantDigitExponent; }
        }

        /// <summary>
        /// Gets a value indicating whether an overflow has occurred.
        /// </summary>
        public bool Overflow
        {
            get { return _universalCounter.Overflow; }
        }

        /// <summary>
        /// Gets a value indicating whether the counter's input signal is active.
        /// </summary>
        public bool InputSignalActive
        {
            get { return _universalCounter.InputSignalActive; }
        }

        /// <summary>
        /// Gets the universal counter's value in the according SI unit (Hz or s).
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get { return _universalCounter.Value; }
        }

        /// <summary>
        /// Gets the frequency readout viewmodel.
        /// </summary>
        public IMeasurementValueReadoutViewModel FrequencyReadoutVM
        {
            get { return _frequencyReadoutVM; }
        }

        /// <summary>
        /// Gets the frequency readout viewmodel.
        /// </summary>
        public IMeasurementValueReadoutViewModel PeriodReadoutVM
        {
            get { return _periodReadoutVM; }
        }

        /// <summary>
        /// Configures all readout viewmodels according to the current settings.
        /// </summary>
        private void ConfigureReadoutVMs()
        {
            // For inverse value, optionally use additional digits (might be
            // helpful though they aren't really accurate).
            const int inverseValueLeastSignificantDigitExponentAdjustment = -2;
            // For inverse value, optionally use bigger numbers with smaller
            // units or vice versa.
            const int inverseValueScaleExponentAdjustment = 0;

            _measuredValueReadoutVM =
                _universalCounter.MeasurementMode == MeasurementMode.Frequency
                ? _frequencyReadoutVM : _periodReadoutVM;
            
            _derivedValueReadoutVM =
                _universalCounter.MeasurementMode == MeasurementMode.Period
                ? _frequencyReadoutVM : _periodReadoutVM;

            ConfigureReadoutVM(_measuredValueReadoutVM,
                () => LeastSignificantDigitExponent,
                () => ScaleExponent, false);

            ConfigureReadoutVM(_derivedValueReadoutVM,
                () =>
                {
                    // Higher measurement values provide more digits and thus better inverse resolution.
                    // This might throw an exception if we haven't received a value so far.
                    var inverseLeastSignificantDigitExponent = -(int) Math.Log10(Value);
                    return inverseLeastSignificantDigitExponent + inverseValueLeastSignificantDigitExponentAdjustment;
                },
                () =>
                {
                    return -ScaleExponent + inverseValueScaleExponentAdjustment;
                },
                true);
        }

        /// <summary>
        /// Configures the specified readout viewmodels according to the specified values.
        /// </summary>
        private void ConfigureReadoutVM(IMeasurementValueReadoutViewModel readoutVM,
            Func<int> leastSignificantDigitExponentProvider, Func<int> scaleExponentProvider,
            bool valueWasDerived)
        {
            readoutVM.LeastSignificantDigitExponentProvider = leastSignificantDigitExponentProvider;
            readoutVM.ScaleExponentProvider = scaleExponentProvider;
            readoutVM.ValueWasDerived = valueWasDerived;
        }
    }
}

