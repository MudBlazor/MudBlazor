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
    [Obsolete("Renamed to XAxisLabelRotation. This will be removed in a future major version.", false)]
    public int LabelRotation { get => XAxisLabelRotation; set => XAxisLabelRotation = value; }

    /// <summary>
    /// Rotation angle to rotate the labels in degrees.
    /// </summary>
    public int XAxisLabelRotation { get; set; }

    /// <summary>
    /// Extra height to fit XAxis rotated labels.
    /// </summary>
    [Obsolete("No longer required, labels are now calculated automatically. This will be removed in a future major version.", false)]
    public int LabelExtraHeight { get; set; }

    /// <summary>
    /// The ratio of the width of the bars to the space between them.
    /// </summary>
    public double StackedBarWidthRatio { get; set; } = 0.5;
}
