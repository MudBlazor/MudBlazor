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
        internal MudDataGrid<T>? DataGrid { get; set; }
        internal HeaderCell<T>? HeaderCell { get; set; }
        public IEnumerable<T> Items
        {
            get => DataGrid?.Items ?? Enumerable.Empty<T>();
        }
        public List<FilterDefinition<T>> FilterDefinitions
        {
            get => DataGrid?.FilterDefinitions ?? new List<FilterDefinition<T>>();
        }
        internal FilterDefinition<T>? FilterDefinition { get; set; }
        public FilterActions? Actions { get; internal set; }

        public FilterContext(MudDataGrid<T> dataGrid)
        {
            DataGrid = dataGrid;
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
