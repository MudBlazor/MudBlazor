// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Services;

#nullable enable

/// <summary>
/// Calculates how closely a target string matches a search query.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Returns a score from 0 to 100 indicating how well <paramref name="target"/> matches <paramref name="query"/>.
    /// </summary>
    int GetScore(string target, string query);
}
