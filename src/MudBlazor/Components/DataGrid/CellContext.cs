// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the current state of a cell in a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item displayed in the cell.</typeparam>
    public class CellContext<T>
    {
        private readonly HashSet<T> _selection;

        internal HashSet<T> OpenHierarchies { get; }

        /// <summary>
        /// The item displayed in the cell.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// The behaviors which can be performed in the cell.
        /// </summary>
        public CellActions Actions { get; }

        /// <summary>
        /// Indicates if the cell is currently selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                return _selection.Contains(Item);
            }
        }

        /// <summary>
        /// Indicates if the cell is currently in an open hierarchy.
        /// </summary>
        public bool Open
        {
            get
            {
                return OpenHierarchies.Contains(Item);
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataGrid">The data grid which owns this context.</param>
        /// <param name="item">The item displayed in the cell.</param>
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

        /// <summary>
        /// Represents behaviors which can be performed on a cell.
        /// </summary>
        public class CellActions
        {
            /// <summary>
            /// The function which selects the cell.
            /// </summary>
            public Func<bool, Task> SetSelectedItemAsync { get; init; } = null!;

            /// <summary>
            /// The function which begins editing.
            /// </summary>
            public Func<Task> StartEditingItemAsync { get; init; } = null!;

            /// <summary>
            /// The function which ends editing.
            /// </summary>
            public Func<Task> CancelEditingItemAsync { get; init; } = null!;

            /// <summary>
            /// The function which toggles hierarchy visibility.
            /// </summary>
            public Func<Task> ToggleHierarchyVisibilityForItemAsync { get; init; } = null!;
        }
    }
}
