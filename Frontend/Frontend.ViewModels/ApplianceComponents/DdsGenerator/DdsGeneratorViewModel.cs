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
using System.Collections.Generic;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the viewmodel of a DDS generator.
    /// </summary>
    public class DdsGeneratorViewModel : ViewModelBase, IDdsGeneratorViewModel
    {
        private readonly IDdsGenerator _ddsGenerator;
        private readonly IDictionary<ModulationAndSynchronizationSource, IDdsGenerator> _sourceDdsGenerators;
        private readonly IScaleInputViewModel _frequencyScaleInputVM;
        private readonly IScaleInputViewModel _amplitudeScaleInputVM;
        private readonly IScaleInputViewModel _phaseScaleInputVM;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="ddsGenerator">The DDS generator to use.</param>
        /// <param name="sourceDdsGenerators">
        /// The DDS generators that can act as modulation or synchronization sources.
        /// </param>
        /// <param name="frequencyScaleInputVM">The viewmodel of the frequency scale.</param>
        /// <param name="amplitudeScaleInputVM">The viewmodel of the amplitude scale.</param>
        /// <param name="phaseScaleInputVM">The viewmodel of the phase scale.</param>
        public DdsGeneratorViewModel (IApplianceServices applianceServices, IDdsGenerator ddsGenerator,
            IDictionary<ModulationAndSynchronizationSource, IDdsGenerator> sourceDdsGenerators,
            IScaleInputViewModel frequencyScaleInputVM, IScaleInputViewModel amplitudeScaleInputVM,
            IScaleInputViewModel phaseScaleInputVM
        )
            : base(applianceServices)
        {
            _ddsGenerator = ddsGenerator;
            _sourceDdsGenerators = sourceDdsGenerators;

            _frequencyScaleInputVM = frequencyScaleInputVM;
            _amplitudeScaleInputVM = amplitudeScaleInputVM;
            _phaseScaleInputVM = phaseScaleInputVM;

            _amplitudeScaleInputVM.PropertyChanged += (sender, e) =>
            {
                RaiseAMPropertyChangedNotifications();
                RaiseModulationPropertyChanged();
            };

            _frequencyScaleInputVM.PropertyChanged += (sender, e) =>
            {
                RaiseFMPropertyChangedNotifications();
            };
        }

        /// <summary>
        /// Gets or sets the signal waveform.
        /// </summary>
        public Waveform Waveform
        {
            get
            {
                return _ddsGenerator.Waveform;
            }
            set
            {
                _ddsGenerator.Waveform = value;
                Flush();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the signal waveform (<see cref="Waveform"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingWaveform
        {
            get
            {
                return (int)Waveform;
            }
            set
            {
                Waveform = (Waveform)Enum.ToObject(typeof(Waveform), value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum frequency modulation range. Accepts values in the range of 0..5.
        /// If set to n, modulation depth is +/-(<see cref="MaximumFrequency"/> / 2 / 8^(5-n)),
        /// e.g. +/-6103,5 Hz for n=1 and MaximumFrequency=50MHz.
        /// Setting this value also modifies <see cref="MaximumFrequencyModulationDepth"/>.
        /// </summary>
        public ushort MaximumFrequencyModulationRange
        {
            get
            {
                return _ddsGenerator.MaximumFrequencyModulationRange;
            }
            set
            {
                _ddsGenerator.MaximumFrequencyModulationRange = value;
                Flush();
                RaisePropertyChanged();
                RaisePropertyChanged(() => MaximumFrequencyModulationDepth);
                RaiseFMPropertyChangedNotifications();
            }
        }

        /// <summary>
        /// Gets or sets the maximum frequency modulation range (<see cref="MaximumFrequencyModulationRange"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingMaximumFrequencyModulationRange
        {
            get
            {
                return MaximumFrequencyModulationRange;
            }
            set
            {
                MaximumFrequencyModulationRange = (ushort)value;
            }
        }

        /// <summary>
        /// Gets the maximum frequency modulation depth in Hertz.
        /// This is modified by setting <see cref="MaximumFrequencyModulationRange"/>.
        /// </summary>
        public double MaximumFrequencyModulationDepth
        {
            get
            {
                return _ddsGenerator.MaximumFrequencyModulationDepth;
            }
        }

        /// <summary>
        /// Gets or sets the synchronization source. If this value ist set to the current
        /// instance itself, no synchronization takes place.
        /// </summary>
        public ModulationAndSynchronizationSource SynchronizationSource
        {
            get
            {
                return _ddsGenerator.SynchronizationSource;
            }
            set
            {
                _ddsGenerator.SynchronizationSource = value;
                Flush();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the synchronization source (<see cref="SynchronizationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingSynchronizationSource
        {
            get
            {
                return (int)SynchronizationSource;
            }
            set
            {
                SynchronizationSource = (ModulationAndSynchronizationSource)Enum
                    .ToObject(typeof(ModulationAndSynchronizationSource), value);
            }
        }

        /// <summary>
        /// Gets or sets the phase modulation source. If this value ist set to the current
        /// instance itself, no phase modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSource PhaseModulationSource
        {
            get
            {
                return _ddsGenerator.PhaseModulationSource;
            }
            set
            {
                _ddsGenerator.PhaseModulationSource = value;
                Flush();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the phase modulation source (<see cref="PhaseModulationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingPhaseModulationSource
        {
            get
            {
                return (int)PhaseModulationSource;
            }
            set
            {
                PhaseModulationSource = (ModulationAndSynchronizationSource)Enum
                    .ToObject(typeof(ModulationAndSynchronizationSource), value);
            }
        }

        /// <summary>
        /// Gets or sets the frequency modulation source. If this value ist set to the current
        /// instance itself, no frequency modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSource FrequencyModulationSource
        {
            get
            {
                return _ddsGenerator.FrequencyModulationSource;
            }
            set
            {
                _ddsGenerator.FrequencyModulationSource = value;
                Flush();
                RaisePropertyChanged();
                RaisePropertyChanged(() => BindingFrequencyModulationSource);
                RaiseFMPropertyChangedNotifications ();
            }
        }

        /// <summary>
        /// Gets or sets the frequency modulation source (<see cref="FrequencyModulationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingFrequencyModulationSource
        {
            get
            {
                return (int)FrequencyModulationSource;
            }
            set
            {
                FrequencyModulationSource = (ModulationAndSynchronizationSource)Enum
                    .ToObject(typeof(ModulationAndSynchronizationSource), value);
            }
        }

        /// <summary>
        /// Gets or sets the amplitude modulation source. If this value ist set to the current
        /// instance itself, no amplitude modulation takes place.
        /// </summary>
        public ModulationAndSynchronizationSource AmplitudeModulationSource
        {
            get
            {
                return _ddsGenerator.AmplitudeModulationSource;
            }
            set
            {
                _ddsGenerator.AmplitudeModulationSource = value;
                Flush();
                RaisePropertyChanged();
                RaisePropertyChanged(() => BindingAmplitudeModulationSource);
                RaiseAMPropertyChangedNotifications ();
            }
        }

        /// <summary>
        /// Gets or sets the amplitute modulation source (<see cref="AmplitudeModulationSource"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingAmplitudeModulationSource
        {
            get
            {
                return (int)AmplitudeModulationSource;
            }
            set
            {
                AmplitudeModulationSource = (ModulationAndSynchronizationSource)Enum
                    .ToObject(typeof(ModulationAndSynchronizationSource), value);
            }
        }

        /// <summary>
        /// Gets the amount the amplitude is swinging around its center value.
        /// </summary>
        public double AmplitudeModulationSwing
        {
            get
            {
                IDdsGenerator modulatingGenerator;
                return _sourceDdsGenerators.TryGetValue (AmplitudeModulationSource, out modulatingGenerator)
                    ? modulatingGenerator.Amplitude
                    : 0;
            }
        }

        /// <summary>
        /// Gets the relative amount the amplitude is swinging around its center value.
        /// </summary>
        public double RelativeAmplitudeModulationSwing
        {
            get
            {
                return AmplitudeModulationSwing / _ddsGenerator.Amplitude;
            }
        }

        /// <summary>
        /// Gets a value indicating whether amplitude modulation exceeds its limits.
        /// </summary>
        public bool AmplitudeModulationExceedsLimits
        {
            get
            {
                return IsOvermodulated (
                    () => _ddsGenerator.Amplitude,
                    () => 0,
                    () => _ddsGenerator.MaximumAmplitude,
                    () => AmplitudeModulationSwing
                );
            }
        }

        /// <summary>
        /// Gets the amount the frequency is swinging around its center value.
        /// </summary>
        public double FrequencyModulationSwing
        {
            get
            {
                IDdsGenerator modulatingGenerator;
                return _sourceDdsGenerators.TryGetValue (FrequencyModulationSource, out modulatingGenerator)
                    ? MaximumFrequencyModulationDepth * modulatingGenerator.Amplitude / modulatingGenerator.MaximumAmplitude
                    : 0;
            }
        }

        /// <summary>
        /// Gets the relative amount the frequency is swinging around its center value.
        /// </summary>
        public double RelativeFrequencyModulationSwing
        {
            get
            {
                return FrequencyModulationSwing / _ddsGenerator.Frequency;
            }
        }

        /// <summary>
        /// Gets a value indicating whether frequency modulation exceeds its limits.
        /// </summary>
        public bool FrequencyModulationExceedsLimits
        {
            get
            {
                return IsOvermodulated (
                    () => _ddsGenerator.Frequency,
                    () => 0,
                    () => _ddsGenerator.MaximumFrequency,
                    () => FrequencyModulationSwing
                );
            }
        }

        /// <summary>
        /// Gets the frequency scale input viewmodel.
        /// </summary>
        public IScaleInputViewModel FrequencyScaleInputVM
        {
            get
            {
                return _frequencyScaleInputVM;
            }
        }

        /// <summary>
        /// Gets the amplitude scale input viewmodel.
        /// </summary>
        public IScaleInputViewModel AmplitudeScaleInputVM
        {
            get
            {
                return _amplitudeScaleInputVM;
            }
        }

        /// <summary>
        /// Gets the phase scale input viewmodel.
        /// </summary>
        public IScaleInputViewModel PhaseScaleInputVM
        {
            get
            {
                return _phaseScaleInputVM;
            }
        }

        /// <summary>
        /// Occurs when a property has changed that could influence the
        /// current modulation properties of other DDS generators.
        /// </summary>
        public event EventHandler ModulationPropertyChanged;

        /// <summary>
        /// Informs the object that a property of another DDS generator has changed that
        /// could influence its modulation properties.
        /// </summary>
        public void OtherDdsGeneratorModulationHasChanged()
        {
            RaiseAMPropertyChangedNotifications ();
            RaiseFMPropertyChangedNotifications ();
        }

        /// <summary>
        /// Raises the ModulationPropertyChanged event.
        /// </summary>
        private void RaiseModulationPropertyChanged()
        {
            var evt = ModulationPropertyChanged;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises property changed notifications for AM-related properties.
        /// </summary>
        private void RaiseAMPropertyChangedNotifications()
        {
            RaisePropertyChanged(() => AmplitudeModulationSwing);
            RaisePropertyChanged(() => RelativeAmplitudeModulationSwing);
            RaisePropertyChanged(() => AmplitudeModulationExceedsLimits);
        }

        /// <summary>
        /// Raises property changed notifications for FM-related properties.
        /// </summary>
        private void RaiseFMPropertyChangedNotifications()
        {
            RaisePropertyChanged(() => FrequencyModulationSwing);
            RaisePropertyChanged(() => RelativeFrequencyModulationSwing);
            RaisePropertyChanged(() => FrequencyModulationExceedsLimits);
        }

        /// <summary>
        /// Determines whether a modulation exceeds its limits.
        /// </summary>
        private bool IsOvermodulated(Func<double> modulatedParameterProvider,
            Func<double> modulatedParameterMinimumProvider,
            Func<double> modulatedParameterMaximumProvider,
            Func<double> modulationSwingProvider)
        {
            double modulatedParameter = modulatedParameterProvider();
            double modulatedParameterMaximum = modulatedParameterMaximumProvider();
            double modulatedParameterMinimum = modulatedParameterMinimumProvider();
            double modulationSwing = modulationSwingProvider();
            return
                (Math.Abs (modulatedParameter) - Math.Abs (modulationSwing)) < modulatedParameterMinimum
                ||
                (Math.Abs (modulatedParameter) + Math.Abs (modulationSwing)) > modulatedParameterMaximum;
        }
    }
}

