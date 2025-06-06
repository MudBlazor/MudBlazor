// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Data;
using System.Numerics;

namespace MudBlazor;

public interface IChartSeries
{
    string Name { get; }
    bool Visible { get; }
}

public sealed class ChartSeries<T> : IChartSeries, IEquatable<ChartSeries<T>> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public ChartSeries() { }

    public ChartSeries(IReadOnlyList<T> values) => Data = values.ToArray();

    /// <summary>
    /// The legend label for this data set.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The values to display on the chart.
    /// </summary>
    public ChartData<T> Data { get; set; } = new();

    /// <summary>
    /// Displays this data set in the chart.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Tooltip title format for the data set. Supported tags are {{SERIES_NAME}}, {{X_VALUE}} and {{Y_VALUE}}.
    /// </summary>
    public string? TooltipTitleFormat { get; set; }

    /// <summary>
    /// Tooltip subtitle format for the data set. Supported tags are {{SERIES_NAME}}, {{X_VALUE}} and {{Y_VALUE}}.
    /// </summary>
    public string? TooltipSubtitleFormat { get; set; }

    /// <summary>
    /// Tooltip YValue format for the series. It is used to format the {{Y_VALUE}} tag.
    /// </summary>
    public string? TooltipYValueFormat { get; set; }

    public bool Equals(ChartSeries<T>? other)
    {
        if (other is null || other.Data is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Data?.Values?.Count != other.Data.Values.Count) return false;

        return Name == other.Name &&
               Data.Values.SequenceEqual(other.Data.Values);
    }

    public override bool Equals(object? obj) => Equals(obj as ChartSeries<T>);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Name);

        if (Data?.Values != null)
        {
            hashCode.Add(Data.Values.Count);

            for (var i = 0; i < Math.Min(10, Data.Values.Count); i++)
            {
                hashCode.Add(Data.Values[i]);
            }
        }

        return hashCode.ToHashCode();
    }

    public static implicit operator ChartSeries<T>(T[] values) => new() { Data = values };
}

public static class ChartDataSetExtensions
{
    public static List<ChartSeries<T>> AsList<T>(this ChartSeries<T> dataSet) where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        return [dataSet];
    }

    public static List<ChartSeries<T>> AsChartDataSet<T>(this T[] dataSet) where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        return new ChartSeries<T>(dataSet).AsList();
    }
}
