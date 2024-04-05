// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace MudBlazor
{
#nullable enable
    public static class DataGridExtensions
    {
        public static IEnumerable<T> OrderBySortDefinitions<T>(this IEnumerable<T> source, GridState<T> state)
            => OrderBySortDefinitions(source, state.SortDefinitions);

        public static IEnumerable<T> OrderBySortDefinitions<T>(this IEnumerable<T> source, ICollection<SortDefinition<T>> sortDefinitions)
        {
            //avoid multiple enumeration
            var sourceArray = source as T[] ?? source.ToArray();

            if (sourceArray.Length == 0)
            {
                return sourceArray;
            }

            if (sortDefinitions.Count == 0)
            {
                return sourceArray;
            }

            IOrderedEnumerable<T>? orderedEnumerable = null;

            foreach (var sortDefinition in sortDefinitions)
            {
                if (orderedEnumerable is null)
                {
                    orderedEnumerable = sortDefinition.Descending ? sourceArray.OrderByDescending(sortDefinition.SortFunc)
                        : sourceArray.OrderBy(sortDefinition.SortFunc);
                }
                else
                {
                    orderedEnumerable = sortDefinition.Descending ? orderedEnumerable.ThenByDescending(sortDefinition.SortFunc)
                        : orderedEnumerable.ThenBy(sortDefinition.SortFunc);
                }
            }

            return orderedEnumerable ?? source;
        }

        public static Column<T>? GetColumnByPropertyName<T>(this MudDataGrid<T> dataGrid, string propertyName)
        {
            return dataGrid.RenderedColumns.FirstOrDefault(x => x.PropertyName == propertyName);
        }
    }
}
