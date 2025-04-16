// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Runtime.CompilerServices;

namespace MudBlazor.Components.Chart;

[CollectionBuilder(typeof(ChartData), nameof(Create))]
public class ChartData
{
    private double[] _values = [];

    public ChartData() { }

    public ChartData(double value) => _values = [value];

    public ChartData(double[] values) => _values = values ?? [];

    public double[] Values => _values;

    public double this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    public static implicit operator ChartData(double value) => new(value);

    public static implicit operator ChartData(double[] values) => new(values);

    public static ChartData Create(ReadOnlySpan<double> values) => new(values.ToArray());
    public IEnumerator<double> GetEnumerator() => _values.AsEnumerable().GetEnumerator();
}
