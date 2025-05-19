// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudBlazor
{
#nullable enable
    public static class DataGridExtensions
    {
        public static IEnumerable<T> OrderBySortDefinitions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this IEnumerable<T> source, GridState<T> state)
            => OrderBySortDefinitions(source, state.SortDefinitions);

        public static IEnumerable<T> OrderBySortDefinitions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this IEnumerable<T> source, GridStateVirtualize<T> state)
            => OrderBySortDefinitions(source, state.SortDefinitions);

        public static IEnumerable<T> OrderBySortDefinitions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this IEnumerable<T> source, ICollection<SortDefinition<T>> sortDefinitions)
            => OrderBySortDefinitionsInternal(source, sortDefinitions, sortDefinitions.Count);

        public static IEnumerable<T> OrderBySortDefinitions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this IEnumerable<T> source, IReadOnlyCollection<SortDefinition<T>> sortDefinitions)
            => OrderBySortDefinitionsInternal(source, sortDefinitions, sortDefinitions.Count);

        private static IEnumerable<T> OrderBySortDefinitionsInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(IEnumerable<T> source, IEnumerable<SortDefinition<T>> sortDefinitions, int sortDefinitionsCount)
        {
            //avoid multiple enumeration
            var sourceArray = source as T[] ?? source.ToArray();

            if (sourceArray.Length == 0)
            {
                return sourceArray;
            }

            if (sortDefinitionsCount == 0)
            {
                return sourceArray;
            }

            IOrderedEnumerable<T>? orderedEnumerable = null;

            foreach (var sortDefinition in sortDefinitions)
            {
                if (orderedEnumerable is null)
                {
                    orderedEnumerable = sortDefinition.Descending ? sourceArray.OrderByDescending(sortDefinition.SortFunc, sortDefinition.Comparer)
                        : sourceArray.OrderBy(sortDefinition.SortFunc, sortDefinition.Comparer);
                }
                else
                {
                    orderedEnumerable = sortDefinition.Descending ? orderedEnumerable.ThenByDescending(sortDefinition.SortFunc, sortDefinition.Comparer)
                        : orderedEnumerable.ThenBy(sortDefinition.SortFunc, sortDefinition.Comparer);
                }
            }

            return orderedEnumerable ?? source;
        }

        public static Column<T>? GetColumnByPropertyName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this MudDataGrid<T> dataGrid, string propertyName)
        {
            return dataGrid.RenderedColumns.FirstOrDefault(x => x.PropertyName == propertyName);
        }
    }
}
