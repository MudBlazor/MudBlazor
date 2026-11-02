// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Numerics;

namespace MudBlazor.Charts;

/// <summary>
/// A directed edge in a <see cref="Sankey{T}" /> diagram, connecting a source node to a target node with a weight that sets the flow size.
/// </summary>
/// <remarks>
/// The edge is defined by its source node, target node, and a weight value that determines the size of the edge.
/// The weight typically represents the flow or quantity being transferred between the nodes.
/// </remarks>
/// <typeparam name="T">The type of the value associated with the link.</typeparam>
/// <param name="Source">The name of the source <see cref="SankeyNode"/>.</param>
/// <param name="Target">The name of the target <see cref="SankeyNode"/>.</param>
/// <param name="Weight">The weight (size) of this edge</param>
public record SankeyEdge<T>(string Source, string Target, T Weight) where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable;
