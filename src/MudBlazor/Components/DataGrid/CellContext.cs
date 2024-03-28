// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public bool IsOpened
        {
            get
            {
                return OpenHierarchies.Contains(Item);
            }
        }

        public CellContext(MudDataGrid<T> dataGrid, T item)
        {
            _selection = dataGrid.Selection;
            OpenHierarchies = dataGrid._openHierarchies;
            Item = item;
            Actions = new CellActions
            {
                SetSelectedItemAsync = x => dataGrid.SetSelectedItemAsync(x, item),
                StartEditingItemAsync = () => dataGrid.SetEditingItemAsync(item),
                CancelEditingItemAsync = () => dataGrid.CancelEditingItemAsync(),
                ToggleHierarchyVisibilityForItemAsync = () => dataGrid.ToggleHierarchyVisibilityAsync(item),
            };
        }

        public class CellActions
        {
            public Func<bool, Task> SetSelectedItemAsync { get; init; } = null!;
            public Func<Task> StartEditingItemAsync { get; init; } = null!;
            public Func<Task> CancelEditingItemAsync { get; init; } = null!;
            public Func<Task> ToggleHierarchyVisibilityForItemAsync { get; init; } = null!;
        }
    }
}
