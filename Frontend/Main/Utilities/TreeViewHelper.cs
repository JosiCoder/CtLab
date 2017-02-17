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
using Gtk;

namespace CtLab.Frontend.ViewModels
{
    /// <summary>
    /// Provides a treeview item that in turn can have child items and can
    /// manage a user-provided object.
    /// </summary>
    /// <typeparam name="TUserObject">The type of the user object managed by this instance.</typeparam>
    public class TreeItem<TUserObject>
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="id">The identifier used internally.</param>
        /// <param name="caption">The caption intended to be used on user interfaces.</param>
        /// <param name="userObject">The user object managed by this instance.</param>
        /// <param name="children">Optional child items.</param>
        public TreeItem (string id, string caption,
            TUserObject userObject, IEnumerable<TreeItem<TUserObject>> children = null)
        {
            Id = id;
            Caption = caption;
            UserObject = userObject;
            Children = children;
        }

        public string Id;
        public string Caption;
        public TUserObject UserObject;
        public IEnumerable<TreeItem<TUserObject>> Children;
    }

    /// <summary>
    /// Provides utilities for handling Gtk# treestores and treeviews.
    /// </summary>
    public static class TreeViewHelper
    {
        /// <summary>
        /// Adds the specified items to a treestore.
        /// </summary>
        /// <param name="treeStore">The treestore to add the items to.</param>
        /// <param name="storeItem">The item to add the specified items to as children.</param>
        /// <param name="items">The items to add.</param>
        /// <typeparam name="TUserObject">The type of the user object managed by the items.</typeparam>
        public static void AddItems<TUserObject>(this TreeStore treeStore, TreeIter? storeItem, IEnumerable<TreeItem<TUserObject>> items)
        {
            foreach (var item in items)
            {
                var createdStoreItem = storeItem.HasValue
                    ? treeStore.AppendValues (storeItem.Value, item.Id, item.Caption)
                    : treeStore.AppendValues (item.Id, item.Caption);
                if (item.Children != null)
                {
                    treeStore.AddItems (createdStoreItem, item.Children);
                }
            }
        }

        /// <summary>
        /// Finds the an item belonging to a path within the specified items.
        /// </summary>
        /// <param name="items">The items to search the item within.</param>
        /// <param name="itemPathIndices">The path indices of the item to search for.</param>
        /// <returns>The item found or <c>null</c>.</returns>
        /// <typeparam name="TUserObject">The type of the user object managed by the items.</typeparam>
        public static TreeItem<TUserObject> FindItem<TUserObject>(this IEnumerable<TreeItem<TUserObject>> items, IEnumerable<int> itemPathIndices)
        {
            items = items != null ? items : new TreeItem<TUserObject>[0];
            itemPathIndices = itemPathIndices != null ? itemPathIndices : new int[0];

            if (!itemPathIndices.Any ())
            {
                return null;
            }

            var currentItemPathIndex = itemPathIndices.First ();
            var currentItem = items.Skip (currentItemPathIndex).FirstOrDefault();
            var remainingIndices = itemPathIndices.Skip (1);
            if (remainingIndices.Any ())
            {
                return currentItem.Children.FindItem (remainingIndices);
            }
            else
            {
                return currentItem;
            }
        }
    }
}

