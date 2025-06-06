// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections;
using System.Numerics;
using MudBlazor.Charts;

namespace MudBlazor;

public class ChartData<T> : IEnumerable<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    private readonly IReadOnlyList<ChartPoint<T>> _points = [];

    public ChartData() { }

    public ChartData(T value) => _points = [new ChartPoint<T>(null, value)];

    public ChartData(IReadOnlyList<T> values) => _points = [.. values.Select(v => new ChartPoint<T>(null, v))];

    public ChartData((DateTime dateTime, T value) timeValue) =>
        _points = [new ChartPoint<T>(timeValue.dateTime, timeValue.value)];

    public ChartData(IReadOnlyList<(DateTime dateTime, T value)> timeValues) =>
        _points = [.. timeValues.Select(tv => new ChartPoint<T>(tv.dateTime, tv.value))];

    public IReadOnlyList<ChartPoint<T>> Points => _points;

    public IReadOnlyList<T> Values => [.. _points.Select(p => p.Y)];

    public ChartPoint<T> this[int index] => _points[index];

    public T GetValue(int index) => _points[index].Y;

    public int Count => _points.Count;

    public static implicit operator ChartData<T>(T value) => new(value);
    public static implicit operator ChartData<T>(T[] values) => new(values);
    public static implicit operator ChartData<T>(List<T> values) => new(values);
    public static implicit operator ChartData<T>((DateTime dateTime, T value)[] timeValues) => new(timeValues);
    public static implicit operator ChartData<T>(List<(DateTime dateTime, T value)> timeValues) => new(timeValues);
    public static implicit operator ChartData<T>(TimeValue<T> timeValue) => new((timeValue.DateTime, timeValue.Value));
    public static implicit operator ChartData<T>(TimeValue<T>[] values) => new(values.Select(tv => (tv.DateTime, tv.Value)).ToArray());
    public static implicit operator ChartData<T>(List<TimeValue<T>> values) => new(values.Select(tv => (tv.DateTime, tv.Value)).ToArray());

    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
