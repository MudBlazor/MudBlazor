// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Extensions;

internal static class IReadOnlyListExtensions
{
    public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(predicate);

        for (var i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
                return i;
        }

        return -1;
    }
}
