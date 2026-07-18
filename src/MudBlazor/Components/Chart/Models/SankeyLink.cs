// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

/// <summary>
/// A link in a <see cref="Sankey{T}" /> diagram, naming the source and target <see cref="SankeyNode" /> it connects.
/// </summary>
/// <param name="Source">The source <see cref="SankeyNode"/>.</param>
/// <param name="Target">The target <see cref="SankeyNode"/>.</param>
public record struct SankeyLink(string Source, string Target);
