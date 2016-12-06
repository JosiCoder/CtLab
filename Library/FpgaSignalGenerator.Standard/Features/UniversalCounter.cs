//------------------------------------------------------------------------------
// Copyright (C) 2016 Josi Coder

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
using CtLab.Utilities;
using CtLab.CommandsAndMessages.Interfaces;
using CtLab.SubchannelAccess;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.FpgaSignalGenerator.Standard
{
    /// <summary>
    /// Communicates with a universal counter implemented within a c't Lab FPGA device configured as
    /// an FPGA Lab.
    /// </summary>
    public class UniversalCounter : IUniversalCounter
    {
        private UniversalCounterConfigurationWriter _configurationWriter;

        private UInt32ValueSubchannelReader _rawValueReader;
        private UniversalCounterStatusReader _statusReader;

        private readonly object _anyEventRegistrationLock = new object();
        private event EventHandler<ValueChangedEventArgs> _valueChanged;
        private event EventHandler<InputSignalActiveChangedEventArgs> _inputSignalActiveChanged;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="configurationSetter">
		/// The setter used to set the universal counter's configuration.
        /// </param>
        /// <param name="rawValueContainer">
		/// The container used to get the universal counter's most-recent raw value message.
        /// </param>
        /// <param name="statusContainer">
		/// The container used to get the universal counter's most-recent status message.
        /// </param>
        public UniversalCounter(
            ISubchannelValueSetter configurationSetter,
            IMessageContainer rawValueContainer,
            IMessageContainer statusContainer)
        {
            _configurationWriter = new UniversalCounterConfigurationWriter(configurationSetter);
            _rawValueReader = new UInt32ValueSubchannelReader(rawValueContainer);
            _statusReader = new UniversalCounterStatusReader(statusContainer);

            if (rawValueContainer != null)
                rawValueContainer.MessageUpdated += this.RawValueContainerMessageUpdated;
            if (statusContainer != null)
                statusContainer.MessageUpdated += this.StatusContainerMessageUpdated;
        }

        /// <summary>
        /// Occurs when the value has changed.
        /// Note that this event might be called via a background thread.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add
            {
                lock (_anyEventRegistrationLock)
                {
                    _valueChanged += value;
                }
            }
            remove
            {
                lock (_anyEventRegistrationLock)
                {
                    _valueChanged -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when the input signal active state has changed.
        /// Note that this event might be called via a background thread.
        /// </summary>
        public event EventHandler<InputSignalActiveChangedEventArgs> InputSignalActiveChanged
        {
            add
            {
                lock (_anyEventRegistrationLock)
                {
                    _inputSignalActiveChanged += value;
                }
            }
            remove
            {
                lock (_anyEventRegistrationLock)
                {
                    _inputSignalActiveChanged -= value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the signal source.
        /// </summary>
        public UniversalCounterSources InputSource
        {
            get { return _configurationWriter.InputSource; }
            set { _configurationWriter.InputSource = value; }
        }

        /// <summary>
        /// Gets or sets the prescaler mode used to generate the gate time or counter clock.
        /// Setting this value also modifies <see cref="MeasurementMode"/>.
        /// </summary>
        public PrescalerModes PrescalerMode
        {
            get { return _configurationWriter.PrescalerMode; }
            set
            {
                _configurationWriter.PrescalerMode = value;

                // Set the measurement mode accordingly.
                switch (value)
                {
                    case PrescalerModes.GatePeriod_1s:
                    case PrescalerModes.GatePeriod_100ms:
                    case PrescalerModes.GatePeriod_10s:
                        _configurationWriter.MeasurementMode
                            = MeasurementModes.Frequency;
                        break;
                    default:
                        _configurationWriter.MeasurementMode
                            = MeasurementModes.Period;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the measurement mode.
        /// This is modified implicitly when setting <see cref="PrescalerMode"/>.
        /// </summary>
        public MeasurementModes MeasurementMode
        {
            get { return _configurationWriter.MeasurementMode; }
        }

        /// <summary>
        /// Gets the exponent of the least significant digit for the current
        /// prescaler mode. If, for example, the least significant digit is
        /// 1/10 (0.1), an exponent of -1 is returned.
        /// </summary>
        public int LeastSignificantDigitExponent
        {
            get
            {
                switch (_configurationWriter.PrescalerMode)
                {
                    // Frequency measurement
                    case PrescalerModes.GatePeriod_100ms:
                        return 1;
                    case PrescalerModes.GatePeriod_1s:
                        return 0;
                    case PrescalerModes.GatePeriod_10s:
                        return -1;
                    // Period measurement
                    case PrescalerModes.CounterClock_10kHz:
                        return -4;
                    case PrescalerModes.CounterClock_100kHz:
                        return -5;
                    case PrescalerModes.CounterClock_1MHz:
                        return -6;
                    case PrescalerModes.CounterClock_10MHz:
                        return -7;
                    default:
                        throw new ArgumentOutOfRangeException ("PrescalerMode");
                }
            }
        }

        /// <summary>
		/// Gets the counter's value in Hertz for frequency measurements or in seconds
        /// for period measurements.
        /// </summary>
        public double Value
        {
            get
            {
                var rawValue = (double) _rawValueReader.Value;
                return rawValue * Math.Pow (10, LeastSignificantDigitExponent);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an overflow has occurred.
        /// </summary>
        public bool Overflow
        {
            get { return _statusReader.Overflow; }
        }

        /// <summary>
		/// Gets a value indicating whether the counter's input signal is active.
        /// </summary>
        public bool InputSignalActive
        {
            get { return _statusReader.InputSignalActive; }
        }

        /// <summary>
		/// Performs actions whenever the raw value container's message has been updated.
        /// </summary>
        /// <remarks>
        /// Usually this will be called via a background thread.
        /// </remarks>
        private void RawValueContainerMessageUpdated(object sender, EventArgs e)
        {
            _valueChanged.Raise(this, new ValueChangedEventArgs(Value));
        }

        /// <summary>
		/// Performs actions whenever the status container's message has been updated.
        /// </summary>
        /// <remarks>
        /// Usually this will be called via a background thread.
        /// </remarks>
        private void StatusContainerMessageUpdated(object sender, EventArgs e)
        {
            _inputSignalActiveChanged.Raise(this, new InputSignalActiveChangedEventArgs(InputSignalActive));
        }
    }
}