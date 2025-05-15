// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Runtime.CompilerServices;

namespace MudBlazor;

[CollectionBuilder(typeof(ChartDataSet), nameof(Create))]
public class ChartDataSet : IEquatable<ChartDataSet>
{
    public ChartDataSet() { }

    public ChartDataSet(double[] doubles) => Data = doubles;

    /// <summary>
    /// The legend label for this data set.
    /// </summary>
    public string Label { get; set; } = string.Empty;

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


    public static implicit operator ChartDataSet(double[] values) => new() { Data = values };
    public static ChartDataSet Create(ReadOnlySpan<double> values) => new(values.ToArray());
    public IEnumerator<double> GetEnumerator() => Data.GetEnumerator();

    public bool Equals(ChartDataSet? other)
    {
        if (other is null) return false;

        return Label == other.Label &&
               Data.Values.SequenceEqual(other.Data.Values);
    }

    public override bool Equals(object? obj) => Equals(obj as ChartDataSet);

    public override int GetHashCode() => HashCode.Combine(Label, string.Join(",", Data.Values));
}

public static class ChartDataSetExtensions
{
    public static List<ChartDataSet> AsList(this ChartDataSet dataSet)
    {
        return [dataSet];
    }

    public static List<ChartDataSet> AsChartDataSet(this double[] dataSet)
    {
        return new ChartDataSet(dataSet).AsList();
    }
}
