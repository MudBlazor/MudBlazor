// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;
#nullable enable

public partial class ChartData<T>
{
    public ChartData(SankeyLink link, T weight) => Points = [new ChartPoint<T>(link, weight)];

    public ChartData(IReadOnlyList<(SankeyLink link, T weight)> edges) =>
        Points = [.. edges.Select(edge => new ChartPoint<T>(edge.link, edge.weight))];

    public ChartData((string source, string target, T weight) edge) =>
        Points = [new ChartPoint<T>(new SankeyLink(edge.source, edge.target), edge.weight)];

    public ChartData(IReadOnlyList<(string source, string target, T value)> edges) =>
        Points = [.. edges.Select(edge => new ChartPoint<T>(new SankeyLink(edge.source, edge.target), edge.value))];

    public static implicit operator ChartData<T>((SankeyLink link, T weight) edges) => new([edges]);
    public static implicit operator ChartData<T>((SankeyLink link, T weight)[] edges) => new(edges);
    public static implicit operator ChartData<T>(List<(SankeyLink link, T weight)> edges) => new(edges);
    public static implicit operator ChartData<T>(SankeyEdge<T> edge) => new((edge.Source, edge.Target, edge.Weight));
    public static implicit operator ChartData<T>(SankeyEdge<T>[] edges) => new(edges.Select(edge => (edge.Source, edge.Target, edge.Weight)).ToArray());
    public static implicit operator ChartData<T>(List<SankeyEdge<T>> edges) => new(edges.Select(edge => (edge.Source, edge.Target, edge.Weight)).ToArray());
}
