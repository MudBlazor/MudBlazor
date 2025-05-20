// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor;

public class ChartPoint
{
    public object? X { get; set; } = null;
    public double Y { get; set; }

    public ChartPoint() { }

    public ChartPoint(double y) => Y = y;

    public ChartPoint(object x, double y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator ChartPoint((DateTime x, double y) value) => new(value.x, value.y);
    public static implicit operator ChartPoint((double x, double y) value) => new(value.x, value.y);
    public static implicit operator ChartPoint(double y) => new(y);
}
