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
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
using PB = Praeclarum.Bind;
using System.Collections.Specialized;
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend.Views
{
    /// <summary>
    /// Provides a base implementation for Gtk# views of windows.
    /// </summary>
    public abstract class WindowViewBase: Gtk.Window, IMainViewModelDialogService
    {
        protected readonly MainViewModel _viewModel;
        protected readonly Func<bool> _closeRequestHandler;
        [UI] Gtk.Container applianceContainer;
        [UI] Gtk.Action applicationSettingsAction;
        [UI] Gtk.Action connectAndInitializeAppliancesAction;
        [UI] Gtk.Action disconnectAppliancesAction;


        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        /// <param name="closeAction">
        /// A function handling any request to close the window.
        /// </param>
        protected WindowViewBase (MainViewModel viewModel, Builder builder, IntPtr handle,
            Func<bool> acceptCloseFunction)
            : base (handle)
        {
            _viewModel = viewModel;
            _closeRequestHandler = acceptCloseFunction;
            viewModel.DialogService = this;
            builder.Autoconnect(this);

            // === Create sub-views. ===

            var applianceViews = CreateApplianceViews (_viewModel.ApplianceVMs);
            applianceViews.ForEach (widget => applianceContainer.Add (widget));

            // === Register event handlers. ===

            _viewModel.ApplianceVMs.CollectionChanged += ApplianceVMCollectionChanged;

            DeleteEvent += Deleted;

            // === Register action handlers. ===

            applicationSettingsAction.Activated += (sender, e) =>
            {
                _viewModel.AdjustApplicationSettings();
            };

            connectAndInitializeAppliancesAction.Activated += (sender, e) =>
            {
                _viewModel.InitializeAppliancesWithCurrentSettings();
            };

            disconnectAppliancesAction.Activated += (sender, e) =>
            {
                _viewModel.DisposeAppliances();
            };
        }

        #region IMainViewModelDialogService

        /// <summary>
        /// Shows a message and waits for the user's response
        /// </summary>
        /// <param name="parent">The parent window of the message dialog.</param>
        /// <param name="dialogType">The type of the dialog to show.</param>
        /// <param name="message">The main message to show.</param>
        /// <param name="secondaryMessage">The secondary message to show.</param>
        /// <returns>The option selected by the user.</returns>
        public DialogResult ShowMessageAndWaitForResponse (DialogType dialogType,
            string message, string secondaryMessage)
        {
            return MessageDialogBuilder
                .ShowMessageAndWaitForResponse(this, dialogType, message, secondaryMessage);
        }

        /// <summary>
        /// Shows and adjusts the application settings.
        /// </summary>
        /// <param name="applicationSettingsVM">The viewmodel of the application settings.</param>
        /// <returns>The option selected by the user.</returns>
        public DialogResult ShowAndAdjustApplicationSettings (IApplicationSettingsViewModel applicationSettingsVM)
        {
            return ApplicationSettingsView.Create(applicationSettingsVM, this, true)
                .ShowAndWaitForResponse();
        }

        #endregion

        /// <summary>
        /// Performs actions whenever the appliance viewmodel collection has changed.
        /// </summary>
        private void ApplianceVMCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<Widget> newWidgets = new Widget[0];
            IEnumerable<Widget> oldWidgets = new Widget[0];

            // Currently only Add und Reset are supported (others aren't used yet).
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    newWidgets = CreateApplianceViews(e.NewItems.OfType<IApplianceViewModel>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // To make "Remove" work, we have to determine the view
                    // instances according to the viewmodel instances. As this
                    // isn't used yet, it's not yet implemented.
                    //oldWidgets = ???;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    oldWidgets = applianceContainer.AllChildren.OfType<Widget>();
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                default:
                    break;
            }

            oldWidgets.ForEach (widget => applianceContainer.Remove (widget));
            newWidgets.ForEach (widget => applianceContainer.Add (widget));
        }

        /// <summary>
        /// Creates views for the specified appliance viewmodels.
        /// </summary>
        protected abstract IEnumerable<ApplianceViewBase> CreateApplianceViews(IEnumerable<IApplianceViewModel> applianceVMs);

        /// <summary>
        /// Performs actions whenever a request to close the window has been made.
        /// </summary>
        private void Deleted(object sender, DeleteEventArgs a)
        {
            var acceptCloseFunction = _closeRequestHandler;
            a.RetVal = acceptCloseFunction != null ? !acceptCloseFunction() : true;
        }
    }
}

