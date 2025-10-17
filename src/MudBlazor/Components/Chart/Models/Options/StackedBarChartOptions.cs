using MudBlazor.Charts;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Options for the <see cref="StackedBar"/> chart.
/// </summary>
public class StackedBarChartOptions : ChartOptions
{
    /// <summary>
    /// The ratio of the width of the bars to the space between them.
    /// </summary>
    public double StackedBarWidthRatio { get; set; } = 0.5;
}
