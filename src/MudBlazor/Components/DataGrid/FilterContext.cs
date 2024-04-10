﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor
{
#nullable enable
    public class FilterContext<T>
    {
        private readonly MudDataGrid<T> _dataGrid;

        internal IFilterDefinition<T>? FilterDefinition { get; set; }

        internal HeaderCell<T>? HeaderCell { get; set; }

        public IEnumerable<T> Items => _dataGrid.Items;

        public List<IFilterDefinition<T>> FilterDefinitions => _dataGrid.FilterDefinitions;

        public FilterActions Actions { get; }

        public FilterContext(MudDataGrid<T> dataGrid)
        {
            _dataGrid = dataGrid;
            Actions = new FilterActions
            {
                ApplyFilterAsync = async x => await (HeaderCell?.ApplyFilterAsync(x) ?? Task.CompletedTask),
                ApplyFiltersAsync = async x => await (HeaderCell?.ApplyFiltersAsync(x) ?? Task.CompletedTask),
                ClearFilterAsync = async x => await (HeaderCell?.ClearFilterAsync(x) ?? Task.CompletedTask),
                ClearFiltersAsync = async x => await (HeaderCell?.ClearFiltersAsync(x) ?? Task.CompletedTask),
            };
        }

        public class FilterActions
        {
            public required Func<IFilterDefinition<T>, Task> ApplyFilterAsync { get; init; }

            public required Func<IEnumerable<IFilterDefinition<T>>, Task> ApplyFiltersAsync { get; init; }

            public required Func<IFilterDefinition<T>, Task> ClearFilterAsync { get; init; }

            public required Func<IEnumerable<IFilterDefinition<T>>, Task> ClearFiltersAsync { get; init; }
        }
    }
}
