// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
#nullable enable
    public class FilterContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        internal FilterDefinition<T>? FilterDefinition { get; set; }

        internal HeaderCell<T>? HeaderCell { get; set; }

        public IEnumerable<T> Items
        {
            get
            {
                return _dataGrid.Items;
            }
        }

        public List<FilterDefinition<T>> FilterDefinitions
        {
            get
            {
                return _dataGrid.FilterDefinitions;
            }
        }

        public FilterActions Actions { get; }

        public FilterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FilterActions
            {
                ApplyFilter = x => HeaderCell?.ApplyFilter(x),
                ApplyFilters = x => HeaderCell?.ApplyFilters(x),
                ClearFilter = x => HeaderCell?.ClearFilter(x),
                ClearFilters = x => HeaderCell?.ClearFilters(x),
            };
        }

        public class FilterActions
        {
            public Action<FilterDefinition<T>>? ApplyFilter { get; internal set; }
            public Action<IEnumerable<FilterDefinition<T>>>? ApplyFilters { get; internal set; }
            public Action<FilterDefinition<T>>? ClearFilter { get; internal set; }
            public Action<IEnumerable<FilterDefinition<T>>>? ClearFilters { get; internal set; }
        }
    }
}
