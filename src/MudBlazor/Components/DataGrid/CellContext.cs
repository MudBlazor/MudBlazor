// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
#nullable enable
    public class CellContext<T>
    {
        private readonly HashSet<T> _selection;

        internal HashSet<T> OpenHierarchies { get; }

        public T Item { get; set; }

        public CellActions Actions { get; }

        public bool IsSelected
        {
            get
            {
                return _selection.Contains(Item);
            }
        }

        public CellContext(MudDataGrid<T> dataGrid, T item)
        {
            _selection = dataGrid.Selection;
            OpenHierarchies = dataGrid._openHierarchies;
            Item = item;
            Actions = new CellActions
            {
                SetSelectedItem = async (x) => await dataGrid.SetSelectedItemAsync(x, item),
                StartEditingItem = async () => await dataGrid.SetEditingItemAsync(item),
                CancelEditingItem = async () => await dataGrid.CancelEditingItemAsync(),
                ToggleHierarchyVisibilityForItem = async () => await dataGrid.ToggleHierarchyVisibilityAsync(item),
            };
        }

        public class CellActions
        {
            public Action<bool>? SetSelectedItem { get; internal set; }
            public Action? StartEditingItem { get; internal set; }
            public Action? CancelEditingItem { get; internal set; }
            public Action? ToggleHierarchyVisibilityForItem { get; internal set; }
        }
    }
}
