using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Docs.Extensions
{
    public static class TableExtension
    {
        public static IOrderedEnumerable<TSource> OrderByDirection<TSource, TKey>(this IEnumerable<TSource> source, SortDirection direction, Func<TSource, TKey> keySelector)
        {
            if (direction == SortDirection.Descending)
                return source.OrderByDescending(keySelector);
            return source.OrderBy(keySelector);
        }
    }
}
