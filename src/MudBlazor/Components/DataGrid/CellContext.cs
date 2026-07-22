// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    /// <summary>
    /// Cell state and actions passed to a <see cref="MudDataGrid{T}"/> cell template, exposing the row item, selection state, and editing commands.
    /// </summary>
    /// <typeparam name="T">The type of item displayed in the cell.</typeparam>
    public class CellContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly HashSet<T> _selection;
        private readonly T _sourceItem;

        internal HashSet<T> OpenHierarchies { get; }

        /// <summary>
        /// The item displayed in the cell.
        /// </summary>
        /// <remarks>
        /// In <see cref="DataGridEditMode.Inline"/> mode, when the row is being edited,
        /// this returns the editing copy of the item. Otherwise, it returns the source item.
        /// </remarks>
        public T Item { get; set; }

        /// <summary>
        /// The behaviors which can be performed in the cell.
        /// </summary>
        public CellActions Actions { get; }

        /// <summary>
        /// Indicates if the cell is currently selected.
        /// </summary>
        public bool Selected => _selection.Contains(_sourceItem);

        /// <summary>
        /// Indicates if the cell is currently in an open hierarchy.
        /// </summary>
        public bool Open => OpenHierarchies.Contains(_sourceItem);

        /// <summary>
        /// Indicates if the row containing this cell is currently being edited in inline mode.
        /// </summary>
        /// <remarks>
        /// This property is only relevant when <see cref="MudDataGrid{T}.EditMode"/> is set to
        /// <see cref="DataGridEditMode.Inline"/>. Use this to conditionally render edit controls
        /// in a <c>CellTemplate</c>.
        /// </remarks>
        public bool IsEditing => _dataGrid.IsEditingItem(_sourceItem);

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataGrid">The data grid which owns this context.</param>
        /// <param name="item">The item displayed in the cell.</param>
        public CellContext(MudDataGrid<T> dataGrid, T item)
            : this(dataGrid, item, item)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataGrid">The data grid which owns this context.</param>
        /// <param name="item">The item displayed in the cell (may be the editing copy in inline mode).</param>
        /// <param name="sourceItem">The original source item from the data collection.</param>
        public CellContext(MudDataGrid<T> dataGrid, T item, T sourceItem)
        {
            _dataGrid = dataGrid;
            _selection = dataGrid.Selection;
            _sourceItem = sourceItem;
            OpenHierarchies = dataGrid._openHierarchies;
            Item = item;
            Actions = new CellActions
            {
                SetSelectedItemAsync = x => dataGrid.SetSelectedItemAsync(x, sourceItem),
                StartEditingItemAsync = () => dataGrid.SetEditingItemAsync(sourceItem),
                CancelEditingItemAsync = () => dataGrid.CancelEditingItemAsync(),
                CommitEditingItemAsync = () => dataGrid.CommitInlineEditAsync(),
                ToggleHierarchyVisibilityForItemAsync = () => dataGrid.ToggleHierarchyVisibilityAsync(sourceItem),
                GetGroupIcon = (expanded, rightToLeft) => dataGrid.GetGroupIcon(expanded, rightToLeft),
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
            public required Func<bool, Task> SetSelectedItemAsync { get; init; }

            /// <summary>
            /// The function which begins editing.
            /// </summary>
            public required Func<Task> StartEditingItemAsync { get; init; }

            /// <summary>
            /// The function which ends editing.
            /// </summary>
            public required Func<Task> CancelEditingItemAsync { get; init; }

            /// <summary>
            /// The function which commits inline edits and exits edit mode.
            /// </summary>
            /// <remarks>
            /// This action is only relevant when <see cref="MudDataGrid{T}.EditMode"/> is set to
            /// <see cref="DataGridEditMode.Inline"/>. It validates, persists changes to the source item,
            /// and exits edit mode.
            /// </remarks>
            public Func<Task> CommitEditingItemAsync { get; init; } = () => Task.CompletedTask;

            /// <summary>
            /// The function which toggles hierarchy visibility.
            /// </summary>
            public required Func<Task> ToggleHierarchyVisibilityForItemAsync { get; init; }

            /// <summary>
            /// The function which retrieves the GroupIcon.
            /// </summary>
            public Func<bool, bool, string>? GetGroupIcon { get; init; }
        }
    }
}
