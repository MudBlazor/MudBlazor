// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public partial class ChartData<T>
{
    /// <summary>
    /// Creates a data series with a single (X, Y) coordinate pair for use in a scatter plot.
    /// </summary>
    public ChartData((T x, T y) point) =>
        Points = [new ChartPoint<T>(point.x, point.y)];

    /// <summary>
    /// Creates a data series from a collection of (X, Y) coordinate pairs for use in a scatter plot.
    /// </summary>
    public ChartData(IReadOnlyList<(T x, T y)> points) =>
        Points = [.. points.Select(p => new ChartPoint<T>(p.x, p.y))];

    public static implicit operator ChartData<T>((T x, T y)[] points) => new(points);
    public static implicit operator ChartData<T>(List<(T x, T y)> points) => new(points);
}
