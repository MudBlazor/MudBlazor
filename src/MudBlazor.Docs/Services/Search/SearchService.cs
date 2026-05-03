// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FuzzySharp;

namespace MudBlazor.Docs.Services;

#nullable enable

/// <inheritdoc cref="ISearchService"/>
public class SearchService : ISearchService
{
    /// <inheritdoc />
    public int GetScore(string target, string query) => Fuzz.WeightedRatio(target, query);
}
