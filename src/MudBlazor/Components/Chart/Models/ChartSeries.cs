// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections;
using System.Runtime.CompilerServices;

namespace MudBlazor;

[CollectionBuilder(typeof(ChartSeries), nameof(Create))]
public sealed class ChartSeries : IEquatable<ChartSeries>, IEnumerable<double>
{
    public ChartSeries() { }

    public ChartSeries(double[] doubles) => Data = doubles;

    /// <summary>
    /// The legend label for this data set.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The values to display on the chart.
    /// </summary>
    public ChartData Data { get; set; } = new();

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


    public static implicit operator ChartSeries(double[] values) => new() { Data = values };
    public static ChartSeries Create(ReadOnlySpan<double> values) => new(values.ToArray());
    public IEnumerator<double> GetEnumerator() => Data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(ChartSeries? other)
    {
        if (other is null) return false;

        return Name == other.Name &&
               Data.Values.SequenceEqual(other.Data.Values);
    }

    public override bool Equals(object? obj) => Equals(obj as ChartSeries);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Name);
        if (Data?.Values != null)
        {
            foreach (var value in Data.Values)
            {
                hashCode.Add(value);
            }
        }
        return hashCode.ToHashCode();
    }
}

public static class ChartDataSetExtensions
{
    public static List<ChartSeries> AsList(this ChartSeries dataSet)
    {
        return [dataSet];
    }

    public static List<ChartSeries> AsChartDataSet(this double[] dataSet)
    {
        return new ChartSeries(dataSet).AsList();
    }
}
