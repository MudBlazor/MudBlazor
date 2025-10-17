using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Options for the <see cref="Line"/> chart.
/// </summary>
public class LineChartOptions : AxisChartOptions
{
    /// <summary>
    /// The technique used to smooth lines.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="InterpolationOption.Straight"/>.  Only takes effect when the <see cref="MudChart"/> type is <see cref="ChartType.Line"/>.
    /// </remarks>
    public InterpolationOption InterpolationOption { get; set; } = InterpolationOption.Straight;

    /// <summary>
    /// The width of lines, in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>3</c> for three pixels.  Only takes effect when the <see cref="MudChart"/> type is <see cref="ChartType.Line"/>.
    /// </remarks>
    public double LineStrokeWidth { get; set; } = 3;
    
    /// <summary>
    /// Allows series to be hidden when <see cref="ChartType"/> is <see cref="ChartType.Line"/>.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, checkboxes are displayed which can toggle visibility of each line.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public bool CanHideSeries { get; set; } = false;
}
