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

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Specifies the option the user has chosen within a dialog.
    /// </summary>
    public enum DialogResult
    {
        Ok,
        Cancel,
        //        Yes,
        //        No,
    }

    /// <summary>
    /// Specifies the type of a dialog.
    /// </summary>
    public enum DialogType
    {
        Question,
        Info,
        Warning,
        Error,
        Other
    }

    /// <summary>
    /// Provides access to the main viewmodel dialog service.
    /// </summary>
    public interface IMainViewModelDialogService
    {
        /// <summary>
        /// Shows a message and waits for the user's response
        /// </summary>
        /// <param name="parent">The parent window of the message dialog.</param>
        /// <param name="dialogType">The type of the dialog to show.</param>
        /// <param name="message">The main message to show.</param>
        /// <param name="secondaryMessage">The secondary message to show.</param>
        /// <returns>The option selected by the user.</returns>
        DialogResult ShowMessageAndWaitForResponse (DialogType dialogType, string message, string secondaryMessage);

        /// <summary>
        /// Shows and adjusts the application settings.
        /// </summary>
        /// <param name="applicationSettingsVM">The viewmodel of the application settings.</param>
        /// <returns>The option selected by the user.</returns>
        DialogResult ShowAndAdjustApplicationSettings (IApplicationSettingsViewModel applicationSettingsVM);
    }
}

