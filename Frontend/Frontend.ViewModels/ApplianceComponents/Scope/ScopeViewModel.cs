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
using System.ComponentModel;
using System.Collections.ObjectModel;
using ScopeLib.Utilities;
using ScopeLib.Sampling;
using ScopeLib.Display.ViewModels;
using CtLab.FpgaSignalGenerator.Interfaces;

namespace CtLab.Frontend.ViewModels
{
    // TODO: Remove demo parts?

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
        private readonly IScopeScreenViewModel _masterScopeScreenVM;
        private readonly IScopeScreenViewModel _slaveScopeScreenVM;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="scope">The scope to use.</param>        // TODO: similar to signalGenerator for SignalGeneratorViewModel?
        /// <param name="masterScopeScreenVM">The viewmodel of the master scope screen.</param>
        /// <param name="slaveScopeScreenVM">The viewmodel of the slave scope screen.</param>
        public ScopeViewModel (IApplianceServices applianceServices, /*ISignalGenerator signalGenerator*/
            ScopeScreenViewModel masterScopeScreenVM, ScopeScreenViewModel slaveScopeScreenVM)
            : base(applianceServices)
        {
            _masterScopeScreenVM = masterScopeScreenVM;
            _slaveScopeScreenVM = slaveScopeScreenVM;
        }

        /// <summary>
        /// Gets the master scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel MasterScopeScreenVM
        {
            get { return _masterScopeScreenVM; }
        }

        /// <summary>
        /// Gets the slave scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel SlaveScopeScreenVM
        {
            get { return _slaveScopeScreenVM; }
        }
    }
}

