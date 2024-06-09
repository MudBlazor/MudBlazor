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
    /// Represents the current state of a footer in a <see cref="MudDataGrid{T}"/>.
    /// </summary>
    /// <typeparam name="T">The kind of item being managed.</typeparam>
    public class FooterContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

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
        /// Indicates whether all values are currently selected.
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
        /// <param name="dataGrid">The <see cref="MudDataGrid{T}"/> containing this footer.</param>
        public FooterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FooterActions
            {
                SetSelectAllAsync = x => _dataGrid.SetSelectAllAsync(x ?? false),
            };
        }

        /// <summary>
        /// Represents the actions which can be performed on the footer of <see cref="MudDataGrid{T}"/> columns.
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
