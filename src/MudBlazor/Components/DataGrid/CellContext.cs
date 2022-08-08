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
            Item = item;
            Actions = new CellContext<T>.CellActions
            {
                SetSelectedItem = async (x) => await dataGrid.SetSelectedItemAsync(x, this.Item),
                StartEditingItem = async () => await dataGrid.SetEditingItemAsync(this.Item),
                CancelEditingItem = async () => await dataGrid.CancelEditingItemAsync(),
                CommitItemChanges = async () => await dataGrid.CommitItemChangesAsync(this.Item),
            };
        }

        public class CellActions
        {
            public Action<bool> SetSelectedItem { get; internal set; }
            public Action StartEditingItem { get; internal set; }
            public Action CancelEditingItem { get; internal set; }
            public Action CommitItemChanges { get; internal set; }
        }
    }
}
