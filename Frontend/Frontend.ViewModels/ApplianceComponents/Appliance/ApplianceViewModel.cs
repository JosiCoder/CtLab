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
using System.Threading;
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides the viewmodel of an appliance.
    /// </summary>
    public class ApplianceViewModel : ViewModelBase, IApplianceViewModel
    {
        private readonly Appliance _appliance;
        private readonly ISignalGeneratorViewModel _signalGeneratorVM;
        private readonly Timer _monitorTimer;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="applianceServices">The services provided by the appliance.</param>
        /// <param name="appliance">The appliance to use.</param>
        /// <param name="signalGeneratorVM">The viewmodel of the signal generator.</param>
        /// <param name="connectionDescription">A description of the current connection.</param>
        public ApplianceViewModel (IApplianceServices applianceServices, Appliance appliance,
            ISignalGeneratorViewModel signalGeneratorVM, string connectionDescription)
            : base(applianceServices)
        {
            _appliance = appliance;

            _signalGeneratorVM = signalGeneratorVM;

            ConnectionDescription = connectionDescription;

            // Start monitoring the connection active state.
            const int monitorPeriodMilliseconds = 1000;
            _monitorTimer = new Timer(state =>
            {
                try
                {
                    DispatchOnUIThread(() =>
                    {
                        RaisePropertyChanged(() => ConnectionDescription);
                        RaisePropertyChanged(() => IsConnectionActive);
                    });
                }
                catch
                {
                    ; // intentionally ignore any errors here (e.g. timeouts)
                }
            },
            null, 0, monitorPeriodMilliseconds);
        }

        /// <summary>
        /// Gets a description of the current connection.
        /// </summary>
        public string ConnectionDescription
        { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the appliance's connection is active.
        /// </summary>
        public bool IsConnectionActive
        { get { return _appliance.ApplianceConnection.Connection.IsActive; } }

        /// <summary>
        /// Gets the signal generator of the appliance.
        /// </summary>
        /// <value>The signal generator V.</value>
        public ISignalGeneratorViewModel SignalGeneratorVM
        {
            get
            {
                return _signalGeneratorVM;
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            if (_monitorTimer != null)
            {
                _monitorTimer.Dispose ();
            }

            if (_appliance != null)
            {
                _appliance.Dispose();
            }
        }
    }
}

