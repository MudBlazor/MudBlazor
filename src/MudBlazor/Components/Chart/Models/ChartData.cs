// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections;
using System.Runtime.CompilerServices;
using MudBlazor.Charts;

namespace MudBlazor;

[CollectionBuilder(typeof(ChartData), nameof(Create))]
public class ChartData : IEnumerable<double>
{
    private List<ChartPoint> _points = [];

    public ChartData() { }

    public ChartData(double value) => _points = [new ChartPoint(0, value)];

    public ChartData(IEnumerable<double> values) => _points = [.. values.Select(v => new ChartPoint(0, v))];

    public ChartData(TimeSeries.DataPoint point) => _points = [new ChartPoint(point.DateTime, point.Value)];

    public ChartData(IEnumerable<TimeSeries.DataPoint> points) =>
        _points = [.. points.Select(p => new ChartPoint(p.DateTime, p.Value))];

    public ChartData(IEnumerable<ChartPoint> points) => _points = [.. points];

    public ChartData((DateTime dateTime, double value) timeValue) =>
        _points = [new ChartPoint(timeValue.dateTime, timeValue.value)];

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
    public static implicit operator ChartData(TimeSeries.DataPoint dataPoint) => new(dataPoint);
    public static implicit operator ChartData(List<TimeSeries.DataPoint> dataPoints) => new(dataPoints);
    public static implicit operator ChartData((DateTime dateTime, double value)[] timeValues) => new(timeValues);
    public static implicit operator ChartData(List<(DateTime dateTime, double value)> timeValues) => new(timeValues);


    public static ChartData Create(ReadOnlySpan<double> values) => new(values.ToArray());
    public IEnumerator<double> GetEnumerator() => _points.Select(p => p.Y).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
