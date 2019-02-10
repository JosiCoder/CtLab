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
using ScopeLib.Display.ViewModels;

namespace CtLab.Frontend.ViewModels
{
    // TODO: Implement scope, describe like the signal generator's comment:
    /// <summary>
    /// Provides access to a viewmodel of a signal generator consisting of
    /// different signal-generating units and counters.
    /// </summary>

    /// <summary>
    /// Provides access to a viewmodel of a scope.
    /// </summary>
    public interface IScopeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the master scope screen viewmodel.
        /// </summary>
        IScopeScreenViewModel MasterScopeScreenVM
        { get; }

        /// <summary>
        /// Gets the slave scope screen viewmodel.
        /// </summary>
        IScopeScreenViewModel SlaveScopeScreenVM
        { get; }
    }
}

