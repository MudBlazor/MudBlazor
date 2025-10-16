// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections;
using System.Numerics;

namespace MudBlazor;

public partial class ChartData<T> : IEnumerable<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public ChartData() { }

    public ChartData(T value) => Points = [new ChartPoint<T>(null, value)];

    public ChartData(IReadOnlyList<T> values) => Points = [.. values.Select(v => new ChartPoint<T>(null, v))];

    public IReadOnlyList<ChartPoint<T>> Points { get; } = [];

    public IReadOnlyList<T> Values => [.. Points.Select(p => p.Y)];

    public ChartPoint<T> this[int index] => Points[index];

    public T GetValue(int index) => Points[index].Y;

    public int Count => Points.Count;

    public static implicit operator ChartData<T>(T value) => new(value);
    public static implicit operator ChartData<T>(T[] values) => new(values);
    public static implicit operator ChartData<T>(List<T> values) => new(values);

    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
