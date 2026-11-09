// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace MudBlazor.Extensions;

/// <summary>
/// Provides internal helper extensions for working with enumerable sequences.
/// </summary>
internal static class EnumerableExtensions
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> into an <see cref="IReadOnlyCollection{T}"/> 
    /// with minimal overhead. 
    /// </summary>
    /// <remarks>
    /// This method attempts to avoid unnecessary allocations by using fast paths:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// If <paramref name="source"/> already implements <see cref="IReadOnlyCollection{T}"/>,
    /// it is returned directly.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If <paramref name="source"/> implements <see cref="ICollection{T}"/>, it is wrapped in a lightweight
    /// read-only adapter without copying the underlying data.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If <paramref name="source"/> is <c>null</c>, an empty read-only collection is returned.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Otherwise, the sequence is enumerated once and materialized into a <see cref="List{T}"/>.
    /// </description>
    /// </item>
    /// </list>
    /// This ensures callers always receive an <see cref="IReadOnlyCollection{T}"/> while avoiding 
    /// unnecessary enumeration or allocation whenever possible.
    /// </remarks>
    /// <param name="source">The sequence to convert to a read-only collection.</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>
    /// An <see cref="IReadOnlyCollection{T}"/> representing the contents of <paramref name="source"/>.
    /// </returns>
    public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T>? source)
    {
        return source switch
        {
            IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection,
            ICollection<T> collection => new CollectionWrapper<T>(collection),
            null => Array.Empty<T>(),
            _ => source.ToList()
        };
    }

    private sealed class CollectionWrapper<T>(ICollection<T> inner) : IReadOnlyCollection<T>
    {
        public int Count => inner.Count;
        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
