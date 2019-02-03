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
using System.Collections.ObjectModel;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    // TODO: Implement scope, describe like the signal generator's comment:
    /// <summary>
    /// Provides the viewmodel of a signal generator consisting of different
    /// signal-generating units and counters.
    /// </summary>

    /// <summary>
    /// Provides the viewmodel of a scope.
    /// </summary>
    public class ScopeViewModel : ViewModelBase, IScopeViewModel
    {
        private readonly ISignalGenerator _signalGenerator;
        private readonly IUniversalCounterViewModel _universalCounterVM;
        private readonly IPulseGeneratorViewModel _pulseGeneratorVM;
        private readonly ObservableCollection<IDdsGeneratorViewModel> _ddsGeneratorVMs;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="signalGenerator">The signal generator to use.</param>
        /// <param name="universalCounterVM">The viewmodel of the universal counter.</param>
        /// <param name="pulseGeneratorVM">The viewmodel of the pulse generator.</param>
        /// <param name="ddsGeneratorVMs">The viewmodels of the DDS generators.</param>
        public ScopeViewModel (IApplianceServices applianceServices, ISignalGenerator signalGenerator,
            IUniversalCounterViewModel universalCounterVM, IPulseGeneratorViewModel pulseGeneratorVM,
            ObservableCollection<IDdsGeneratorViewModel> ddsGeneratorVMs)
            : base(applianceServices)
        {
            _signalGenerator = signalGenerator;

            _universalCounterVM = universalCounterVM;
            _pulseGeneratorVM = pulseGeneratorVM;
            _ddsGeneratorVMs = ddsGeneratorVMs;
        }

        /// <summary>
        /// Gets or sets the signal source for the first output (output 0).
        /// </summary>
        public OutputSource OutputSource0
        {
            get
            {
                return _signalGenerator.OutputSourceSelector.OutputSource0;
            }
            set
            {
                _signalGenerator.OutputSourceSelector.OutputSource0 = value;
                Flush();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the signal source for the first output (<see cref="OutputSource0"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingOutputSource0
        {
            get
            {
                return (int)OutputSource0;
            }
            set
            {
                OutputSource0 = (OutputSource)Enum
                    .ToObject(typeof(ModulationAndSynchronizationSource), value);
            }
        }

        /// <summary>
        /// Gets or sets the signal source for the second output (output 1).
        /// </summary>
        public OutputSource OutputSource1
        {
            get
            {
                return _signalGenerator.OutputSourceSelector.OutputSource1;
            }
            set
            {
                _signalGenerator.OutputSourceSelector.OutputSource1 = value;
                Flush();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the signal source for the second output (<see cref="OutputSource1"/>)
        /// via its int representation. This is intended to be used for data binding as enums
        /// and non-int integers include a Convert operation that makes binding fail.
        /// </summary>
        public int BindingOutputSource1
        {
            get
            {
                return (int)OutputSource1;
            }
            set
            {
                OutputSource1 = (OutputSource)Enum
                    .ToObject(typeof(ModulationAndSynchronizationSource), value);
            }
        }

        /// <summary>
        /// Gets the universal counter viewmodel.
        /// </summary>
        public IUniversalCounterViewModel UniversalCounterVM
        {
            get { return _universalCounterVM; }
        }

        /// <summary>
        /// Gets the pulse generator viewmodel.
        /// </summary>
        public IPulseGeneratorViewModel PulseGeneratorVM
        {
            get { return _pulseGeneratorVM; }
        }

        /// <summary>
        /// Gets the DDS generator viewmodels.
        /// </summary>
        public ObservableCollection<IDdsGeneratorViewModel> DdsGeneratorVMs
        {
            get { return _ddsGeneratorVMs; }
        }
    }
}

