#nullable enable
namespace MudBlazor;

public class AxisChartOptions
{
    /// <summary>
    /// Make the chart fill the parent
    /// </summary>
    public bool MatchBoundsToSize { get; set; }

    /// <summary>
    /// Rotation angle to rotate the labels in degrees.
    /// </summary>
    public int LabelRotation { get; set; }

    /// <summary>
    /// Extra height to fit rotated labels.
    /// </summary>
    public int LabelExtraHeight { get; set; }

    /// <summary>
    /// The ratio of the width of the bars to the space between them.
    /// </summary>
    public double StackedBarWidthRatio { get; set; } = 0.5;
}
