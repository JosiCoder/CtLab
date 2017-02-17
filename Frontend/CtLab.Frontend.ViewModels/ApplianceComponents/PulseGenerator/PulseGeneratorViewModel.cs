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
    /// Provides the viewmodel of a pulse generator.
    /// </summary>
    public class PulseGeneratorViewModel : ViewModelBase, IPulseGeneratorViewModel
    {
        //private readonly IPulseGenerator _pulseGenerator;
        private readonly IPulseGeneratorDurationScaleInputViewModel _pulseDurationScaleInputVM;
        private readonly IPulseGeneratorDurationScaleInputViewModel _pauseDurationScaleInputVM;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="universalCounter">The pulse generator to use.</param>
        /// <param name="pulseDurationScaleInputVM">The viewmodel of the pulse duration scale.</param>
        /// <param name="pauseDurationScaleInputVM">The viewmodel of the pause duration scale.</param>
        public PulseGeneratorViewModel (IApplianceServices applianceServices, IPulseGenerator pulseGenerator,
            IPulseGeneratorDurationScaleInputViewModel pulseDurationScaleInputVM,
            IPulseGeneratorDurationScaleInputViewModel pauseDurationScaleInputVM
        )
            : base(applianceServices)
        {
            //_pulseGenerator = pulseGenerator;

            _pulseDurationScaleInputVM = pulseDurationScaleInputVM;
            _pauseDurationScaleInputVM = pauseDurationScaleInputVM;
        }

        /// <summary>
        /// Gets the pulse scale input viewmodel.
        /// </summary>
        public IPulseGeneratorDurationScaleInputViewModel PulseScaleInputVM
        {
            get
            {
                return _pulseDurationScaleInputVM;
            }
        }

        /// <summary>
        /// Gets the pause scale input viewmodel.
        /// </summary>
        public IPulseGeneratorDurationScaleInputViewModel PauseScaleInputVM
        {
            get
            {
                return _pauseDurationScaleInputVM;
            }
        }
    }
}

