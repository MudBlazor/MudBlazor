// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the current state of a header in a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The kind of item being managed.</typeparam>
    public class HeaderContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        /// <summary>
        /// The items to apply to the header.
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
        /// The behaviors which are allowed for this header.
        /// </summary>
        public HeaderActions Actions { get; }

        /// <summary>
        /// Indicates whether all items are currently selected.
        /// </summary>
        public bool? IsAllSelected
        {
            get
            {
                if (_dataGrid.Selection is not null && (Items?.Any() ?? false))
                {
                    if (_dataGrid.Selection.Count == Items.Count())
                    {
                        return true;
                    }

                    if (_dataGrid.Selection.Count == 0)
                    {
                        return false;
                    }

                    return null;
                }

                return false;
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataGrid">The <see cref="MudDataGrid{T}"/> which contains the header.</param>
        public HeaderContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new HeaderActions
            {
                SetSelectAllAsync = x => _dataGrid.SetSelectAllAsync(x ?? false),
            };

        }

        /// <summary>
        /// Represents the behaviors allowed for a <see cref="MudDataGrid{T}"/> header.
        /// </summary>
        public class HeaderActions
        {
            /// <summary>
            /// The function which selects all items.
            /// </summary>
            public required Func<bool?, Task> SetSelectAllAsync { get; init; }
        }
    }
}
