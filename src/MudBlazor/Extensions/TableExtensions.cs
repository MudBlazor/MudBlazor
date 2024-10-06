using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MudBlazor
{
#nullable enable
    public static class TableExtensions
    {
        public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(this IEnumerable<TSource> source, SortDirection direction, Func<TSource, TKey> keySelector)
        {
            if (direction == SortDirection.Descending)
            {
                return source.OrderByDescending(keySelector);
            }

            return source.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source, SortDirection direction, Expression<Func<TSource, TKey>> keySelector)
        {
            if (direction == SortDirection.Descending)
            {
                return source.OrderByDescending(keySelector);
            }

            return source.OrderBy(keySelector);
        }

        /// <summary>
        /// Disabled the edit button if edit row switching is blocked and the provided item is not being edited
        /// </summary>
        public static bool EditButtonDisabled<T>(this TableContext? context, T item) => (context?.Table?.IsEditRowSwitchingBlocked ?? false) && context?.Table._editingItem != null && !ReferenceEquals(context?.Table._editingItem, item);
    }
}
