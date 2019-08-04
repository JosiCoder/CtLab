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
using System.Collections.ObjectModel;
using CtLab.Environment;
using CtLab.FpgaSignalGenerator.Interfaces;
using CtLab.FpgaScope.Interfaces;
using ScopeLib.Display.ViewModels;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Specifies the type of the connection an appliance uses.
    /// </summary>
    public enum ApplianceConnectionType
    {
        SpiDirect,
        Serial,
        Dummy,
    }

    /// <summary>
    /// Provides the main viewmodel that acts as the entry point.
    /// </summary>
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly ObservableCollection<IApplianceViewModel> _applianceVMs =
            new ObservableCollection<IApplianceViewModel>();
        private readonly IApplianceFactory _applianceFactory;
        private readonly IDialogServiceViewModelFactory _dialogServiceViewModelFactory;
        private readonly IApplicationSettingsWriter _applicationSettingsWriter;

        private ApplicationSettings _applicationSettings = new ApplicationSettings();

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceFactory">The factory used to create appliances.</param>
        /// <param name="dialogServiceViewModelFactory">
        /// The factory used to create dialog service viewmodels.
        /// </param>
        /// <param name="applicationSettingsWriter">
        /// The writer used to write the application settings to the settings file.
        /// </param>
        public MainViewModel(IApplianceFactory applianceFactory,
            IDialogServiceViewModelFactory dialogServiceViewModelFactory, IApplicationSettingsWriter applicationSettingsWriter)
            : base(null)
        {
            _applianceFactory = applianceFactory;
            _dialogServiceViewModelFactory = dialogServiceViewModelFactory;
            _applicationSettingsWriter = applicationSettingsWriter;
        }

        /// <summary>
        /// Adjusts the application settings.
        /// </summary>
        public void AdjustApplicationSettings()
        {
            if (DialogService != null)
            {
                var tempApplicationSettings = _applicationSettings.Clone ();

                var settingsVM = _dialogServiceViewModelFactory
                    .CreateApplicationSettingsViewModel (_applianceFactory.AvailablePortNames,
                        tempApplicationSettings);
                
                var response = DialogService.ShowAndAdjustApplicationSettings(settingsVM);

                if (response == DialogResult.Ok)
                {
                    _applicationSettingsWriter.Write (tempApplicationSettings);
                    InitializeAppliances (tempApplicationSettings);
                }
            }
        }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        public IMainViewModelDialogService DialogService
        { get; set; }

        /// <summary>
        /// Initializes the appliances using the current application settings
        /// (replacing and disposing any existing appliances).
        /// </summary>
        public void InitializeAppliancesWithCurrentSettings()
        {
            InitializeAppliances (_applicationSettings);
        }

        /// <summary>
        /// Initializes the appliances using the specified application settings
        /// (replacing and disposing any existing appliances).
        /// </summary>
        /// <param name="applicationSettings">The application settings.</param>
        public void InitializeAppliances(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;

            InitializeAppliances (applicationSettings.ConnectionType,
                applicationSettings.PortName, applicationSettings.Channel);
        }

        /// <summary>
        /// Initializes the appliances (replacing and disposing any existing appliances).
        /// </summary>
        /// <param name="connectionType">
        /// The type of appliance connection to use.
        /// </param>
        /// <param name="portName">The name of the serial port to use.</param>
        /// <param name="channel">
        /// The number of the channel to send to and receive from.
        /// </param>
        private void InitializeAppliances(ApplianceConnectionType connectionType,
            string portName, byte channel)
        {
            try
            {
                DisposeApplianceVMs();

                // Currently, we have only one appliance, but later we might have more.
                var portDescriptionFormat = 
                    connectionType == ApplianceConnectionType.SpiDirect
                    ? "(SPI direct)"
                    : connectionType == ApplianceConnectionType.Serial
                    ? "(serial port {0}, channel {1})"
                    : connectionType == ApplianceConnectionType.Dummy
                    ? "(simulated, channel {1})"
                    : "(unknown, channel {1})";

                var appliance = CreateAppliance(connectionType, portName, channel);
                _applianceVMs.Add(CreateApplianceViewModel(appliance,
                    string.Format(portDescriptionFormat, portName, channel)));
            }
            catch (Exception ex)
            {
                if (DialogService != null)
                {
                    DialogService.ShowMessageAndWaitForResponse (DialogType.Error,
                        "Could not initialize the appliances.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Disposes all existing appliances.
        /// </summary>
        public void DisposeAppliances()
        {
            DisposeApplianceVMs ();
        }

        /// <summary>
        /// Gets the appliance viewmodel.
        /// </summary>
        public ObservableCollection<IApplianceViewModel> ApplianceVMs
        {
            get { return _applianceVMs; }
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            DisposeApplianceVMs();
        }

        /// <summary>
        /// Creates a new appliance.
        /// </summary>
        private Appliance CreateAppliance(ApplianceConnectionType connectionType,
            string portName, byte channel)
        {
            Appliance appliance;

            switch (connectionType)
            {
                case ApplianceConnectionType.SpiDirect:
                    appliance = _applianceFactory.CreateSpiAppliance ();
                    break;
                case ApplianceConnectionType.Serial:
                    if (_applianceFactory.AvailablePortNames.Contains(portName))
                    {
                        appliance = _applianceFactory.CreateSerialAppliance (portName, channel);
                    }
                    else
                    {
                        throw new InvalidOperationException (string.Format("A serial port named \"{0}\" does not exist.", portName));
                    }
                    break;
                case ApplianceConnectionType.Dummy:
                    appliance = _applianceFactory.CreateDummyAppliance (channel);
                    break;
                default:
                    throw new InvalidOperationException ("Unknown appliance connection type.");
            }

            return appliance;
        }

        /// <summary>
        /// Creates a new appliance viewmodel wrapping the specified appliance.
        /// </summary>
        private ApplianceViewModel CreateApplianceViewModel(Appliance appliance,
            string connectionDescription)
        {
            var applianceServices = new ApplianceServices (appliance);

            // === Get the signal generator and its parts. ===

            var signalGenerator = appliance.SignalGenerator;
            var universalCounter = signalGenerator.UniversalCounter;
            var pulseGenerator = signalGenerator.PulseGenerator;
            var ddsGenerators = signalGenerator.DdsGenerators;

            // === Build the viewmodel hierarchy for the signal generator. ===

            var universalCounterVM = new UniversalCounterViewModel (applianceServices,
                universalCounter,
                new MeasurementValueReadoutViewModel("Hz"),
                new MeasurementValueReadoutViewModel("s"));

            var pulseDurationScaleInputVM = new PulseGeneratorDurationScaleInputViewModel(
                applianceServices,
                val => pulseGenerator.PulseDuration = val,
                () => pulseGenerator.PulseDuration);

            var pauseDurationScaleInputVM = new PulseGeneratorDurationScaleInputViewModel(
                applianceServices,
                val => pulseGenerator.PauseDuration = val,
                () => pulseGenerator.PauseDuration);

            var pulseGeneratorVM = new PulseGeneratorViewModel (applianceServices,
                pulseGenerator,
                pulseDurationScaleInputVM,
                pauseDurationScaleInputVM);

            var sourceDdsGenerators = new Dictionary<ModulationAndSynchronizationSource, IDdsGenerator> ();
            sourceDdsGenerators.Add (ModulationAndSynchronizationSource.DdsGenerator0, ddsGenerators [0]);
            sourceDdsGenerators.Add (ModulationAndSynchronizationSource.DdsGenerator1, ddsGenerators [1]);
            sourceDdsGenerators.Add (ModulationAndSynchronizationSource.DdsGenerator2, ddsGenerators [2]);
            sourceDdsGenerators.Add (ModulationAndSynchronizationSource.DdsGenerator3, ddsGenerators [3]);

            var ddsGeneratorsVMs = new ObservableCollection<IDdsGeneratorViewModel>(
                ddsGenerators.Select (ddsGenerator =>
                {
                    var frequencyScaleInputVM = new DdsGeneratorFrequencyScaleInputViewModel(
                        applianceServices,
                        val => ddsGenerator.Frequency = val,
                        () => ddsGenerator.Frequency);

                    var amplitudeScaleInputVM = new DdsGeneratorAmplitudeScaleInputViewModel(
                        applianceServices,
                        val => ddsGenerator.Amplitude = val,
                        () => ddsGenerator.Amplitude);

                    var phaseScaleInputVM = new DdsGeneratorPhaseScaleInputViewModel(
                        applianceServices,
                        val => ddsGenerator.Phase = val,
                        () => ddsGenerator.Phase);

                    var specificSourceDdsGenerators = sourceDdsGenerators
                        .Where(dictItem => dictItem.Value != ddsGenerator)
                        .ToDictionary(dictItem => dictItem.Key, dictItem => dictItem.Value);

                    return new DdsGeneratorViewModel (applianceServices,
                        ddsGenerator, specificSourceDdsGenerators,
                        frequencyScaleInputVM, amplitudeScaleInputVM, phaseScaleInputVM);
                }));

            // Let the DDS generators notify each other about property changes that
            // could influence modulation properties.
            foreach (var publisher in ddsGeneratorsVMs)
            {
                foreach (var subscriber in ddsGeneratorsVMs)
                {
                    if (subscriber != publisher)
                    {
                        publisher.ModulationPropertyChanged += (sender, e) =>
                        {
                            subscriber.OtherDdsGeneratorModulationHasChanged();
                        };
                    }
                }
            }

            var signalGeneratorVM = new SignalGeneratorViewModel(applianceServices,
                signalGenerator,
                universalCounterVM, pulseGeneratorVM, ddsGeneratorsVMs);

            // === Get the scope and its parts. ===

            var scope = appliance.Scope;

            // === Build the viewmodel hierarchy for the scope. ===

            var masterScopeScreenVM = new ScopeScreenViewModel();
            var slaveScopeScreenVM = new ScopeScreenViewModel();

            var scopeVM = new ScopeViewModel(applianceServices,
                scope,
                masterScopeScreenVM, slaveScopeScreenVM);

            var applianceViewModel = new ApplianceViewModel (applianceServices,
                appliance,
                signalGeneratorVM,
                scopeVM,
                connectionDescription);

            // === Start operation. ===

            // TODO: Move demo somewhere else, replace it with real hardware access.
            var hardwareScopeDemo = new RealHardwareScopeDemo();
            //hardwareScopeDemo.WriteAndReadStorageValues(appliance);
            var capturedValueSets = hardwareScopeDemo.CaptureAndReadStorageValues(appliance);
            //var sampleSequences = hardwareScopeDemo.CreateSampleSequences();
            // Our signal has 21 samples. Specifying a sample rate of 5 samples per second treats
            // is as being 4s long. In fact, it was sampled with 11.1 MS/s (90ns sample period).
            var sampleSequences = hardwareScopeDemo.CreateSampleSequences(5, capturedValueSets);

            // TODO: Move demo somewhere else, replace it with real hardware access.
            var scopeDemo = new ScopeDemo();
            scopeDemo.ConfigureMainScopeScreenVM(masterScopeScreenVM, sampleSequences);
            scopeDemo.ConfigureFFTScopeScreenVM(slaveScopeScreenVM, sampleSequences);

            // Start sending the query commands periodically.
            const int queryCommandSendPeriodMilliseconds = 500;
            appliance.ApplianceConnection.StartSendingQueryCommands (queryCommandSendPeriodMilliseconds);

            return applianceViewModel;
        }

        //TODO
        private void startCapturingScopeData()
        {
            captureScopeData();
        }

        //TODO
        private void captureScopeData()
        {
        }

        //TODO
        private void updateScopeVM()
        {
        }

        /// <summary>
        /// Disposes all appliance viewmodels.
        /// </summary>
        private void DisposeApplianceVMs()
        {
            _applianceVMs.ForEach (applianceVM => applianceVM.Dispose ());
            _applianceVMs.Clear ();
        }
    }
}

