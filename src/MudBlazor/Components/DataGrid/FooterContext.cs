// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor
{

    /// <summary>
    /// Footer state and actions passed to a <see cref="MudDataGrid{T}"/> footer template, exposing the displayed items and select-all command.
    /// </summary>
    /// <typeparam name="T">The kind of item being managed.</typeparam>
    public class FooterContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        /// <summary>
        /// Supplies the rows of the group this footer belongs to, or <c>null</c> when the footer applies to the whole grid.
        /// </summary>
        internal Func<IEnumerable<T>?> GroupItemsFunc { private get; init; } = static () => null;

        /// <summary>
        /// The items which apply to the footer.
        /// </summary>
        public IEnumerable<T> Items
        {
            get
            {
                return _dataGrid.HasServerData
                    ? _dataGrid.ServerItems
                    : _dataGrid.FilteredItems;
            }
        }

        /// <summary>
        /// The behaviors which can be performed on this footer.
        /// </summary>
        public FooterActions Actions { get; }

        /// <summary>
        /// Indicates whether the data grid supports multiple selection.
        /// </summary>
        public bool IsMultiSelection => _dataGrid.MultiSelection;

        /// <summary>
        /// Indicates whether all values are currently selected.
        /// </summary>
        public bool? IsAllSelected
        {
            get
            {
                if (GroupItemsFunc() is { } groupItems)
                {
                    return _dataGrid.GetGroupSelectionState(groupItems);
                }

                if (_dataGrid.Selection is not null && (Items?.Any() ?? false))
                {
                    if (_dataGrid.Selection.Count == 0)
                    {
                        return false;
                    }

                    return _dataGrid.Selection.Count == _dataGrid.GetSelectableItems().Count() ? true : null;
                }

                return false;
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataGrid">The <see cref="MudDataGrid{T}"/> containing this footer.</param>
        public FooterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FooterActions
            {
                SetSelectAllAsync = x => GroupItemsFunc() is { } groupItems
                    ? _dataGrid.SetGroupSelectAllAsync(x ?? false, groupItems)
                    : _dataGrid.SetSelectAllAsync(x ?? false),
            };
        }

        /// <summary>
        /// Select-all delegate for a <see cref="MudDataGrid{T}"/> footer, exposed through <see cref="FooterContext{T}"/>.
        /// </summary>
        public class FooterActions
        {
            /// <summary>
            /// The function which selects all values.
            /// </summary>
            public required Func<bool?, Task> SetSelectAllAsync { get; init; }
        }
    }
}
