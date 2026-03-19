// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

public partial class ChartData<T>
{
    public ChartData(DateTime dateTime, T value) => Points = [new ChartPoint<T>(dateTime, value)];

    public ChartData((DateTime dateTime, T value) timeValue) =>
        Points = [new ChartPoint<T>(timeValue.dateTime, timeValue.value)];

    public ChartData(IReadOnlyList<(DateTime dateTime, T value)> timeValues) =>
        Points = [.. timeValues.Select(tv => new ChartPoint<T>(tv.dateTime, tv.value))];

    public static implicit operator ChartData<T>((DateTime dateTime, T value)[] timeValues) => new(timeValues);
    public static implicit operator ChartData<T>(List<(DateTime dateTime, T value)> timeValues) => new(timeValues);
    public static implicit operator ChartData<T>(TimeValue<T> timeValue) => new((timeValue.DateTime, timeValue.Value));
    public static implicit operator ChartData<T>(TimeValue<T>[] values) => new(values.Select(tv => (tv.DateTime, tv.Value)).ToArray());
    public static implicit operator ChartData<T>(List<TimeValue<T>> values) => new(values.Select(tv => (tv.DateTime, tv.Value)).ToArray());
}
