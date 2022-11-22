// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudBlazor
{
    public class FilterContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
    {
        internal MudDataGrid<T>? _dataGrid;
        internal HeaderCell<T>? _headerCell;
        public IEnumerable<T> Items
        {
            get => _dataGrid?.Items ?? Enumerable.Empty<T>();
        }
        public List<FilterDefinition<T>> FilterDefinitions
        {
            get => _dataGrid?.FilterDefinitions ?? new List<FilterDefinition<T>>();
        }
        internal FilterDefinition<T>? FilterDefinition { get; set; }
        public FilterActions? Actions { get; internal set; }

        public FilterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FilterActions
            {
                ApplyFilter = x => _headerCell?.ApplyFilter(x),
                ApplyFilters = x => _headerCell?.ApplyFilters(x),
                ClearFilter = x => _headerCell?.ClearFilter(x),
                ClearFilters = x => _headerCell?.ClearFilters(x),
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
