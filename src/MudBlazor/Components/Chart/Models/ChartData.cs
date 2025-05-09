// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Runtime.CompilerServices;

namespace MudBlazor;

[CollectionBuilder(typeof(ChartData), nameof(Create))]
public class ChartData
{
    private List<ChartPoint> _points = [];

    public ChartData() { }

    public ChartData(double value) => _points = [new ChartPoint(0, value)];

    public ChartData(double[] values)
    {
        _points = new List<ChartPoint>(values.Length);

        for (var i = 0; i < values.Length; i++)
        {
            _points.Add(new ChartPoint(0, values[i]));
        }
    }

    public ChartData(IEnumerable<ChartPoint> points) => _points = [.. points];

    public ChartData(IEnumerable<(DateTime dateTime, double value)> timeValues) =>
        _points = [.. timeValues.Select(tv => new ChartPoint(tv.dateTime, tv.value))];

    public double[] Values => [.. _points.Select(p => p.Y)];

    public IReadOnlyList<ChartPoint> Points => _points;

    public double this[int index]
    {
        get => _points[index].Y;
        set => _points[index].Y = value;
    }

    public static implicit operator ChartData(double value) => new(value);
    public static implicit operator ChartData(double[] values) => new(values);
    public static implicit operator ChartData((DateTime dateTime, double value)[] timeValues) => new(timeValues);
    public static implicit operator ChartData(List<(DateTime dateTime, double value)> timeValues) => new(timeValues);


    public static ChartData Create(ReadOnlySpan<double> values) => new(values.ToArray());
    public IEnumerator<double> GetEnumerator() => _points.Select(p => p.Y).GetEnumerator();
}
