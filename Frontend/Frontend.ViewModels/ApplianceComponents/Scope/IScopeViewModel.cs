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
    /// Provides access to a viewmodel of a signal generator consisting of
    /// different signal-generating units and counters.
    /// </summary>

    /// <summary>
    /// Provides access to a viewmodel of a scope.
    /// </summary>
    public interface IScopeViewModel : INotifyPropertyChanged
    {
//        /// <summary>
//        /// Gets or sets the signal source for the first output (output 0).
//        /// </summary>
//        OutputSource OutputSource0
//        { get; set; }
//
//        /// <summary>
//        /// Gets or sets the signal source for the first output (<see cref="OutputSource0"/>)
//        /// via its int representation. This is intended to be used for data binding as enums
//        /// and non-int integers include a Convert operation that makes binding fail.
//        /// </summary>
//        int BindingOutputSource0
//        { get; set; }
//
//        /// <summary>
//        /// Gets or sets the signal source for the second output (output 1).
//        /// </summary>
//        OutputSource OutputSource1
//        { get; set; }
//
//        /// <summary>
//        /// Gets or sets the signal source for the second output (<see cref="OutputSource1"/>)
//        /// via its int representation. This is intended to be used for data binding as enums
//        /// and non-int integers include a Convert operation that makes binding fail.
//        /// </summary>
//        int BindingOutputSource1
//        { get; set; }
//
//        /// <summary>
//        /// Gets the universal counter viewmodel.
//        /// </summary>
//        IUniversalCounterViewModel UniversalCounterVM
//        { get; }
//
//        /// <summary>
//        /// Gets the pulse generator viewmodel.
//        /// </summary>
//        IPulseGeneratorViewModel PulseGeneratorVM
//        { get; }
//
//        /// <summary>
//        /// Gets the DDS generator viewmodels.
//        /// </summary>
//        ObservableCollection<IDdsGeneratorViewModel> DdsGeneratorVMs
//        { get; }
    }
}

