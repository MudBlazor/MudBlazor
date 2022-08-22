// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
    public static class DataGridExtensions
    {
        public static IEnumerable<T> OrderBySortDefinitions<T>(this IEnumerable<T> source, GridState<T> state)
            => OrderBySortDefinitions(source, state?.SortDefinitions);

        public static IEnumerable<T> OrderBySortDefinitions<T>(this IEnumerable<T> source, ICollection<SortDefinition<T>> sortDefinitions)
        {
            if (null == source || !source.Any())
                return source;

            if (null == sortDefinitions || 0 == sortDefinitions.Count)
                return source;

            IOrderedEnumerable<T> orderedEnumerable = null;

            foreach (var sortDefinition in sortDefinitions)
            {
                if (null == orderedEnumerable)
                    orderedEnumerable = sortDefinition.Descending ? source.OrderByDescending(sortDefinition.SortFunc)
                        : source.OrderBy(sortDefinition.SortFunc);
                else
                    orderedEnumerable = sortDefinition.Descending ? orderedEnumerable.ThenByDescending(sortDefinition.SortFunc)
                        : orderedEnumerable.ThenBy(sortDefinition.SortFunc);
            }

            return orderedEnumerable ?? source;
        }
    }
}
