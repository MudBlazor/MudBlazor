using MudBlazor.Charts;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Options for the <see cref="HeatMap"/> chart.
/// </summary>
public class HeatMapChartOptions : AxisChartOptions
{
    /// <summary>
    /// Enables smooth color transitions for heatmap cells and removes all padding between cells in a <see cref="ChartType.HeatMap"/>
    /// Defaults to <c>false</c>
    /// </summary>
    public bool EnableSmoothGradient { get; set; } = false;

    /// <summary>
    /// The position of the X axis labels as either top or bottom in a <see cref="ChartType.HeatMap"/>.
    /// Defaults to <see cref="XAxisLabelPosition.Bottom"/>
    /// </summary>
    public XAxisLabelPosition XAxisLabelPosition { get; set; } = XAxisLabelPosition.Bottom;

    /// <summary>
    /// The position of the Y axis labels as either left or right in a <see cref="ChartType.HeatMap"/>.
    /// Defaults to <see cref="YAxisLabelPosition.Left"/>
    /// </summary>
    public YAxisLabelPosition YAxisLabelPosition { get; set; } = YAxisLabelPosition.Left;
    
    /// <summary>
    /// Enables labels for every box in a <see cref="ChartType.HeatMap"/>
    /// Defaults to <c>true</c>
    /// </summary>
    public bool ShowLabels { get; set; } = true;

    /// <summary>
    /// Enables label values for the legend boxes in a <see cref="ChartType.HeatMap"/>
    /// Defaults to <c>false</c>
    /// </summary>
    public bool ShowLegendLabels { get; set; } = false;

    /// <summary>
    /// The format applied to labels for every box in a <see cref="ChartType.HeatMap"/>
    /// Defaults to <c>"F2"</c>
    /// </summary>
    public string ValueFormatString { get; set; } = "F2";
    
    internal List<MudHeatMapCell> MudHeatMapCells { get; set; } = [];

    internal void AddCell(MudHeatMapCell cell)
    {
        MudHeatMapCells.Add(cell);
    }
}
