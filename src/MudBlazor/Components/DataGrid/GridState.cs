// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor
{
    public class GridState<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public ICollection<SortDefinition<T>> SortDefinitions { get; set; }

        public ICollection<FilterDefinition<T>> FilterDefinitions { get; set; }
    }

    public class GridData<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
    }
}
