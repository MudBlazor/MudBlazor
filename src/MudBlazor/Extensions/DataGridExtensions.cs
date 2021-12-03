// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
    public static class DataGridExtensions
    {
        public static IEnumerable<T> OrderByDirection<T>(this IEnumerable<T> source, GridState<T> state)
        {
            if (state.SortDirection == SortDirection.None || state.SortBy == null)
                return source;

            if (state.SortDirection == SortDirection.Descending)
                return source.OrderByDescending(state.SortBy);
            return source.OrderBy(state.SortBy);
        }

    }
}
