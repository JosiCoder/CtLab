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
using System.Linq;
using System.ComponentModel;
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
using PB = Praeclarum.Bind;
using System.Collections.Specialized;
using CtLab.Frontend.ViewModels;

namespace CtLab.Frontend
{
    /// <summary>
    /// Provides the Gtk# view of an appliance.
    /// </summary>
    public class ApplicationSettingsView : Gtk.Dialog
    {
        private readonly IApplicationSettingsViewModel _viewModel;
        private readonly IEnumerable<TreeItem<Gtk.Container>> _treeItems;

        // Categories tree, details generics
        [UI] Gtk.TreeView categoriesTreeView;
        [UI] Gtk.TreeStore categoriesTreeStore;
        [UI] Gtk.Label detailsCategoryLabel;
        // EnvironmentConnection
        [UI] Gtk.Container connectionDetailsContainer;
        [UI] Gtk.ComboBox serialPortComboBox;
        [UI] Gtk.Label serialPortHintLabel;
        [UI] Gtk.ListStore serialPortListStore;
        //[UI] Gtk.SpinButton channelSpinbutton;
        [UI] Gtk.Adjustment channelAdjustment;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        /// <param name="parent">The parent window of the message dialog.</param>
        /// <param name="modal">A value indicating whether to show the dialog in a modal way.</param>
        public static ApplicationSettingsView Create(IApplicationSettingsViewModel viewModel,
            Window parent, bool modal)
        {
            var builder = new Builder (null, "ApplicationSettingsView.glade", null);
            var view = new ApplicationSettingsView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
            view.Configure (parent, true, dialog => true);
            return view;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private ApplicationSettingsView(IApplicationSettingsViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            serialPortListStore.Clear ();
            _viewModel.PortNames.ForEach (portName => serialPortListStore.AppendValues (portName));

            // --- Create the categories hierarchy.

            _treeItems = new []
            {
                new TreeItem<Gtk.Container> ("Environment", "Environment", null,
                    new []
                    {
                        new TreeItem<Gtk.Container> ("EnvironmentConnection", "Connection",
                            connectionDetailsContainer),
                    }),
//                new TreeItem<Gtk.Container> ("Cat2ID", "Cat 2 Caption", null,
//                    new []
//                    {
//                        new TreeItem<Gtk.Container> ("Cat2.1ID", "Cat 2.1 Caption", null),
//                        new TreeItem<Gtk.Container> ("Cat2.2ID", "Cat 2.2 Caption", null),
//                    }),
            };

            categoriesTreeStore.AddItems (null, _treeItems);

            categoriesTreeView.ExpandAll ();

            // === Create sub-views. ===

            // === Register event handlers. ===

            categoriesTreeView.Selection.Changed += CategoriesTreeView_SelectionChangedHandler;

            _viewModel.PropertyChanged += (sender, e) =>       
            {
                UpdateSerialPortHintLabel();
            };

            // === Create value converters. ===

            var channelAdjustmentValueConverter = new ValueConverterViewModel<int, double>(
                val => val,
                val => (int)val);
            
            // === Create bindings. ===

            PB.Binding.Create (() => serialPortComboBox.ActiveId == _viewModel.CurrentPortName);
            // Bind the channel spin button's value to the current value.
            PB.Binding.Create (() => channelAdjustment.Value == channelAdjustmentValueConverter.DerivedValue);
            PB.Binding.Create (() => channelAdjustmentValueConverter.OriginalValue == viewModel.CurrentChannel);

            // === Final initialization. ===

            UpdateSerialPortHintLabel ();
        }

        /// <summary>
        /// Updates the serial port hint label.
        /// </summary>
        private void UpdateSerialPortHintLabel()
        {
            var currentPortName = _viewModel.CurrentPortName;
            serialPortHintLabel.Text =
                (string.IsNullOrEmpty(currentPortName) || _viewModel.PortNames.Contains(currentPortName))
                ? "" : string.Format("Current port \"{0}\" is not available.", currentPortName);
        }

        /// <summary>
        /// Performs actions whenever the treeview selection has changed.
        /// </summary>
        private void CategoriesTreeView_SelectionChangedHandler (object sender, EventArgs e)
        {
            detailsCategoryLabel.Text = "";
            SetDetailsContainerVisibility(_treeItems, false);

            var selectedRowPaths = categoriesTreeView.Selection.GetSelectedRows();
            if (selectedRowPaths != null && selectedRowPaths.Any())
            {
                var treeItem = _treeItems.FindItem(selectedRowPaths.First().Indices);
                if (treeItem != null)
                {
                    detailsCategoryLabel.Text = treeItem.Caption;
                    var detailsContainer = treeItem.UserObject;
                    if (detailsContainer != null)
                    {
                        detailsContainer.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the visibility of the details containers for the specified items
        /// and their sub-items.
        /// </summary>
        /// <param name="items">The items to set the details container visibility for.</param>
        /// <param name="visible">The visibility to set.</param>
        private static void SetDetailsContainerVisibility(IEnumerable<TreeItem<Gtk.Container>> items,
            bool visible)
        {
            foreach (var item in items)
            {
                var userObject = item.UserObject;
                if (userObject != null)
                {
                    userObject.Visible = visible;
                }
                
                var children = item.Children;
                if (children != null)
                {
                    SetDetailsContainerVisibility (children, visible);
                }
            }
        }
    }
}