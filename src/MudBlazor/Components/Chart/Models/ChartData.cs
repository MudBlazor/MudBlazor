// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections;
using System.Numerics;

namespace MudBlazor;

/// <summary>
/// Data values used for a chart series.
/// </summary>
/// <remarks>
/// Includes X and Y values based on the chart type.
/// X values are optional and can be null.
/// </remarks>
/// <typeparam name="T">The data type of tye Y value</typeparam>
public partial class ChartData<T> : IEnumerable<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public ChartData() { }

    /// <summary>
    /// Create a data series with a single Y value.
    /// </summary>
    /// <param name="value">The Y value of the chart series</param>
    public ChartData(T value) => Points = [new ChartPoint<T>(null, value)];

    /// <summary>
    /// Create a data series with multiple Y values.
    /// </summary>
    /// <param name="values">The Y values of the chart series</param>
    public ChartData(IReadOnlyList<T> values) => Points = [.. values.Select(v => new ChartPoint<T>(null, v))];

    /// <summary>
    /// A list of data points in the chart series.
    /// </summary>
    public IReadOnlyList<ChartPoint<T>> Points { get; } = [];

    /// <summary>
    /// A list of Y values in the chart series.
    /// </summary>
    public IReadOnlyList<T> Values => [.. Points.Select(p => p.Y)];

    /// <summary>
    /// The data point at the specified index.
    /// </summary>
    /// <param name="index"></param>
    public ChartPoint<T> this[int index] => Points[index];

    /// <summary>
    /// The Y value at the specified index.
    /// </summary>
    /// <param name="index"></param>
    public T GetValue(int index) => Points[index].Y;

    /// <summary>
    /// The number of data points in the chart series.
    /// </summary>
    public int Count => Points.Count;

    public static implicit operator ChartData<T>(T value) => new(value);
    public static implicit operator ChartData<T>(T[] values) => new(values);
    public static implicit operator ChartData<T>(List<T> values) => new(values);

    ///<inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
