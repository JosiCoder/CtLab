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

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides access to the viewmodel of the application settings.
    /// </summary>
    public interface IApplicationSettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the names of the available serial ports.
        /// </summary>
        ObservableCollection<string> PortNames
        { get; }

        /// <summary>
        /// Gets or sets the name of the current serial port.
        /// </summary>
        string CurrentPortName
        { get; set; }

        /// <summary>
        /// Gets or sets the current connection type.
        /// </summary>
        ApplianceConnectionType CurrentConnectionType
        { get; set; }

        /// <summary>
        /// Gets or sets the current channel.
        /// </summary>
        int CurrentChannel
        { get; set; }
    }
}

