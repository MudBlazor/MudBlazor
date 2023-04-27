// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
#nullable enable
    public class GridState<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public ICollection<SortDefinition<T>> SortDefinitions { get; set; } = new List<SortDefinition<T>>();

        public ICollection<FilterDefinition<T>> FilterDefinitions { get; set; } = new List<FilterDefinition<T>>();
    }

    public class GridData<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public int TotalItems { get; set; }
    }
}
