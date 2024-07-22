// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor;

internal static class CollectionExtensions
{
    /// <summary>
    /// Determines whether any element of an array satisfies a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="array">The array to apply the predicate to.</param>
    /// <param name="predicate">The predicate to apply to each element.</param>
    /// <returns>true if any elements match the predicate; otherwise, false.</returns>
    internal static bool Any<T>(this T[] array, Predicate<T> predicate)
    {
        return Array.Exists(array, predicate);
    }

    /// <summary>
    /// Determines whether any element of a list satisfies a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the list.</typeparam>
    /// <param name="list">The list to apply the predicate to.</param>
    /// <param name="predicate">The predicate to apply to each element.</param>
    /// <returns>true if any elements match the predicate; otherwise, false.</returns>
    internal static bool Any<T>(this List<T> list, Predicate<T> predicate)
    {
        return list.FindIndex(predicate) != -1;
    }
}
