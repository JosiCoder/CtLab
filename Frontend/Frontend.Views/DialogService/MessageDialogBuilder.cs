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
using Gtk;
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Shows Gtk# message dialogs.
    /// </summary>
    public class MessageDialogBuilder
    {
        /// <summary>
        /// Shows a message dialog and waits for the user's response.
        /// </summary>
        /// <param name="parent">The parent window of the message dialog.</param>
        /// <param name="dialogType">The type of the dialog to show.</param>
        /// <param name="message">The main message to show.</param>
        /// <param name="secondaryMessage">The secondary message to show.</param>
        /// <returns>The option selected by the user.</returns>
        public static DialogResult ShowMessageAndWaitForResponse(Window parent, DialogType dialogType, string message, string secondaryMessage)
        {
            var messageType =
                dialogType == DialogType.Error ? MessageType.Error
                : dialogType == DialogType.Warning ? MessageType.Warning
                : dialogType == DialogType.Info ? MessageType.Info
                : dialogType == DialogType.Question ? MessageType.Question
                : MessageType.Other;

            return Create (parent, messageType, message, secondaryMessage)
                .ShowAndWaitForResponse();
        }

        /// <summary>
        /// Creates a new Gtk# message dialog.
        /// </summary>
        /// <param name="parent">The parent window of the message dialog.</param>
        /// <param name="messageType">The type of the message to show.</param>
        /// <param name="message">The main message to show.</param>
        /// <param name="secondaryMessage">The secondary message to show.</param>
        private static MessageDialog Create(Window parent, MessageType messageType,
            string message, string secondaryMessage)
        {
            var dialog = new MessageDialog(parent, DialogFlags.Modal,
                messageType, ButtonsType.Ok, null, null);

            dialog.Title = parent.Title;
            dialog.Text = message;
            dialog.SecondaryText = secondaryMessage;
            dialog.SetClosePredicate(dia => true);

            return dialog;
        }
    }
}

