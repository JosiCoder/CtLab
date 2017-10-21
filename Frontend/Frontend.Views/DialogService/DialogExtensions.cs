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

namespace CtLab.Frontend.Views
{
    /// <summary>
    /// Provides some extension methods for Gtk# dialogs.
    /// </summary>
    public static class DialogExtensions
    {
        /// <summary>
        /// Configures the dialog.
        /// </summary>
        /// <param name="dialog">The dialog to configure.</param>
        /// <param name="parent">The parent window of the message dialog.</param>
        /// <param name="modal">
        /// A value indicating whether to show the dialog in a modal way.</param>
        /// <param name="closePredicate">
        /// A predicate that is used when the user attempts to close the dialog.
        /// </param>
        public static void Configure(this Dialog dialog, Window parent, bool modal,
            Predicate<Dialog> closePredicate)
        {
            dialog.TransientFor = parent;
            dialog.Modal = true;
            var titleFormat = string.IsNullOrEmpty(dialog.Title) ? "{0}" : "{0} - {1}";
            dialog.Title = string.Format(titleFormat, parent.Title, dialog.Title);
            dialog.SetClosePredicate(closePredicate);
        }

        /// <summary>
        /// Shows a dialog and waits for the user's response.
        /// </summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <returns>The option selected by the user.</returns>
        public static DialogResult ShowAndWaitForResponse(this Dialog dialog)
        {
            using (dialog)
            {
                var response = (ResponseType) dialog.Run();

                //TODO: Add other cases as necessary.
                return
                    response == ResponseType.Ok ? DialogResult.Ok
                        : DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Sets a predicate that is used when the user attempts to close the dialog.
        /// </summary>
        /// <param name="dialog">The dialog to set the close predicate for.</param>
        /// <param name="closePredicate">
        /// A predicate that is used when the user attempts to close the dialog.
        /// </param>
        public static void SetClosePredicate(this Dialog dialog, Predicate<Dialog> closePredicate)
        {
            dialog.Response += (object o, ResponseArgs args) =>
            {
                if (args.ResponseId == ResponseType.DeleteEvent)
                {
                    if (closePredicate(dialog))
                    {
                        dialog.Respond(ResponseType.Cancel);
                    }
                }

                if (args.ResponseId == ResponseType.Ok || args.ResponseId == ResponseType.Cancel)
                {
                    dialog.Hide();
                }
            };
        }
    }
}

