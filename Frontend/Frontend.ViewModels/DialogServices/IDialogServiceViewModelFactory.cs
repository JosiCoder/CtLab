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
using CtLab.Environment;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides access to a dialog service viewmodel factory.
    /// </summary>
    public interface IDialogServiceViewModelFactory
    {
        /// <summary>
        /// Creates and returns a new application settings viewmodel.
        /// </summary>
        /// <param name="applicationSettings">The application settings.</param>
        /// <returns>The application settings viewmodel created.</returns>
        IApplicationSettingsViewModel CreateApplicationSettingsViewModel (IEnumerable<string> portNames,
            ApplicationSettings applicationSettings);
    }
}

