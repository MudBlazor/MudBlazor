// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public class GridState<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public Func<T, object> SortBy { get; set; }

        public SortDirection SortDirection { get; set; }
    }

    public class GridData<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }

    }
}
