// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public class CellContext<T>
    {
        internal HashSet<T> selection;
        internal HashSet<T> openHierarchies;
        public T Item { get; set; }
        public CellActions Actions { get; internal set; }
        public bool IsSelected
        {
            get
            {
                if (selection != null)
                {
                    return selection.Contains(Item);
                }

                return false;
            }
        }

        public CellContext(MudDataGrid<T> dataGrid, T item)
        {
            selection = dataGrid.Selection;
            openHierarchies = dataGrid._openHierarchies;
            Item = item;
            Actions = new CellContext<T>.CellActions
            {
                SetSelectedItem = async (x) => await dataGrid.SetSelectedItemAsync(x, item),
                StartEditingItem = async () => await dataGrid.SetEditingItemAsync(item),
                CancelEditingItem = async () => await dataGrid.CancelEditingItemAsync(),
                ToggleHierarchyVisibilityForItem = async () => await dataGrid.ToggleHierarchyVisibilityAsync(item),
            };
        }

        public class CellActions
        {
            public Action<bool> SetSelectedItem { get; internal set; }
            public Action StartEditingItem { get; internal set; }
            public Action CancelEditingItem { get; internal set; }
            public Action ToggleHierarchyVisibilityForItem { get; internal set; }
        }
    }
}
