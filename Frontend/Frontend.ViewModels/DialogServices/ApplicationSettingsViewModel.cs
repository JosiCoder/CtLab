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

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the viewmodel of the application settings.
    /// </summary>
    public class ApplicationSettingsViewModel : ViewModelBase, IApplicationSettingsViewModel
    {
        private const string _dummyConnectionPortName = "Simulated";
        private readonly ApplicationSettings _applicationSettings;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="applicationSettings">The application settings.</param>
        public ApplicationSettingsViewModel (IEnumerable<string> portNames,
            ApplicationSettings applicationSettings)
        {
            PortNames = new ObservableCollection<string>(portNames.Concat(new []{_dummyConnectionPortName}));
            _applicationSettings = applicationSettings;
        }

        private ObservableCollection<string> _portNames;
        /// <summary>
        /// Gets the names of the available serial ports.
        /// </summary>
        public ObservableCollection<string> PortNames
        {
            get
            {
                return _portNames;
            }
            private set
            {
                _portNames = value;
                RaisePropertyChanged ();
            }
        }

        /// <summary>
        /// Gets or sets the current connection type.
        /// </summary>
        public ApplianceConnectionType CurrentConnectionType
        {
            get
            {
                return _applicationSettings.ConnectionType;
            }
            set
            {
                _applicationSettings.ConnectionType = value;
                _applicationSettings.PortName = value == ApplianceConnectionType.Dummy
                    ? _dummyConnectionPortName
                    : "";

                RaisePropertyChanged ();
                RaisePropertyChanged (() => CurrentPortName);
            }
        }

        /// <summary>
        /// Gets or sets the name of the current serial port.
        /// </summary>
        public string CurrentPortName
        {
            get
            {
                return _applicationSettings.PortName;
            }
            set
            {
                _applicationSettings.PortName = value;
                _applicationSettings.ConnectionType = value == _dummyConnectionPortName
                    ? ApplianceConnectionType.Dummy
                    : ApplianceConnectionType.Serial;
                
                RaisePropertyChanged ();
                RaisePropertyChanged (() => CurrentConnectionType);
            }
        }

        /// <summary>
        /// Gets or sets the current channel.
        /// </summary>
        public int CurrentChannel
        {
            get
            {
                return _applicationSettings.Channel;
            }
            set
            {
                _applicationSettings.Channel = (byte)value;
                RaisePropertyChanged ();
            }
        }
    }
}

