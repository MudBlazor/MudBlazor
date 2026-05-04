// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// Provides full-text search over named items and keyword indexes.
/// </summary>
internal interface ISearchService
{
    /// <summary>
    /// Returns a score from 0 to 100 indicating how well <paramref name="target"/> matches <paramref name="query"/>.
    /// </summary>
    int GetScore(string target, string query);

    /// <summary>
    /// Searches a keyword index and returns matching items ordered by relevance.
    /// Multiple keywords can map to the same item; the highest score per item wins.
    /// </summary>
    /// <remarks>
    /// Keywords in <paramref name="index"/> must already be lower-cased.
    /// </remarks>
    IReadOnlyList<T> Search<T>(IEnumerable<KeyValuePair<string, T>> index, string query) where T : notnull;

    /// <summary>
    /// Searches a collection by primary name and optional secondary field, returning
    /// matching items ordered by relevance then by name.
    /// </summary>
    IReadOnlyList<T> Search<T>(IEnumerable<T> items, Func<T, string> getName, Func<T, string?> getSecondary, string query);
}
