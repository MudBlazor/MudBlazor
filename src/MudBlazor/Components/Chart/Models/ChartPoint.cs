// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Numerics;
using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Represents the data point in a chart series, with optional X value and required Y value.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ChartPoint<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    /// <summary>
    /// The X value of the data point.
    /// </summary>
    public object? X { get; set; } = null;
    /// <summary>
    /// The Y value of the data point.
    /// </summary>
    public T Y { get; set; }

    public ChartPoint(T y) => Y = y;

    public ChartPoint(object? x, T y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator ChartPoint<T>((SankeyLink x, T y) value) => new(value.x, value.y);
    public static implicit operator ChartPoint<T>((DateTime x, T y) value) => new(value.x, value.y);
    public static implicit operator ChartPoint<T>((T x, T y) value) => new(value.x, value.y);
    public static implicit operator ChartPoint<T>(T y) => new(y);
}
