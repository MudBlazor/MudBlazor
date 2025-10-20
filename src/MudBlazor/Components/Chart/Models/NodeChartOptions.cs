using MudBlazor.Charts;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents options which customize the display of node-based charts (<see cref="Sankey"/>).
/// </summary>
/// <remarks>
/// Note that the more general options are available in <see cref="ChartOptions"/>.
/// </remarks>
public class NodeChartOptions
{
    /// <summary>
    /// The width of nodes in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>10</c>.
    /// </remarks>
    public double NodeWidth { get; set; } = 10;

    /// <summary>
    /// The minimal vertical spacing between nodes in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>12</c>.
    /// </remarks>
    public double MinVerticalSpacing { get; set; } = 12;

    /// <summary>
    /// The opacity of edges.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.5</c>.
    /// </remarks>
    public double EdgeOpacity { get; set; } = 0.5;

    /// <summary>
    /// Whether to show the values of the nodes within their respective labels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// Requires <see cref="ChartOptions.ShowLabels"/> to be set to <c>true</c>.
    /// </remarks>
    public bool ShowNodeValues { get; set; } = true;

    /// <summary>
    /// The font size of all labels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"0.75rem"</c>.
    /// </remarks>
    public string LabelFontSize { get; set; } = "0.75rem";

    /// <summary>
    /// Whether to constantly show the labels of the edges.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool ShowEdgeLabels { get; set; } = false;

    /// <summary>
    /// Whether to highlight nodes and edges on hover.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool HighlightOnHover { get; set; } = true;

    /// <summary>
    /// The color used to highlight nodes and edges on hover.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>var(--mud-palette-text-primary)</c>.
    /// </remarks>
    public string HighlightColor { get; set; } = "var(--mud-palette-text-primary)";
}
