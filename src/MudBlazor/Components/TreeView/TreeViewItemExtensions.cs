using System;
using System.Collections.Generic;

namespace MudBlazor
{
    public static class TreeViewItemExtensions
    {
        public static bool TryFindTreeViewItemByValue<TValue>(this IEnumerable<MudTreeViewItem<TValue>> items, TValue value, out MudTreeViewItem<TValue> result, IEqualityComparer<TValue> equalityComparer = null)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            equalityComparer ??= EqualityComparer<TValue>.Default;

            foreach (var item in items)
            {
                if (equalityComparer.Equals(item.Value, value))
                {
                    result = item;
                    return true;
                }
            }

            foreach (var item in items)
            {
                if (item.TryFindTreeViewItemByValue(value, out result, equalityComparer))
                    return true;
            }

            result = default;
            return false;
        }
    }
}
