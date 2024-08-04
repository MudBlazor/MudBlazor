// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

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
    /// Determines if an array has any elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="array">The array to check.</param>
    /// <returns>true if the array has any elements; otherwise, false.</returns>
    internal static bool Any<T>(this T[] array)
    {
        return array.Length > 0;
    }

    /// <summary>
    /// Determines whether the given element exists in the array.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="array">The array to check.</param>
    /// <param name="element">The element to check for.</param>
    /// <returns>true if the element exists in the array; otherwise, false.</returns>
    internal static bool Contains<T>(this T[] array, T element)
    {
        return Array.IndexOf(array, element) != -1;
    }

    /// <summary>
    /// Returns the first element in an array that matches the specified predicate, or the default value if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="array">The array to search.</param>
    /// <param name="predicate">The predicate to apply to each element.</param>
    /// <returns>The first element that matches the predicate, or the default value if no such element is found.</returns>
    internal static T? FirstOrDefault<T>(this T[] array, Predicate<T> predicate)
    {
        return Array.Find(array, predicate);
    }

    /// <summary>
    /// Returns the first element in an array, or the default value if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="array">The array to search.</param>
    /// <returns>The first element, or the default value if no element is found.</returns>
    internal static T? FirstOrDefault<T>(this T[] array)
    {
        return array.Length > 0 ? array[0] : default;
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

    /// <summary>
    /// Determines if a list has any elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the list.</typeparam>
    /// <param name="list">The list to check.</param>
    /// <returns>true if the list has any elements; otherwise, false.</returns>
    internal static bool Any<T>(this List<T> list)
    {
        return list.Count > 0;
    }

    /// <summary>
    /// Returns the first element in a list that matches the specified predicate, or the default value if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the list.</typeparam>
    /// <param name="list">The list to search.</param>
    /// <param name="predicate">The predicate to apply to each element.</param>
    /// <returns>The first element that matches the predicate, or the default value if no such element is found.</returns>
    internal static T? FirstOrDefault<T>(this List<T> list, Predicate<T> predicate)
    {
        return list.Find(predicate);
    }

    /// <summary>
    /// Returns the first element in a list, or the default value if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the list.</typeparam>
    /// <param name="list">The list to search.</param>
    /// <returns>The first element, or the default value if no element is found.</returns>
    internal static T? FirstOrDefault<T>(this List<T> list)
    {
        return list.Count > 0 ? list[0] : default;
    }
}
