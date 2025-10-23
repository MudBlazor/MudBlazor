using MudBlazor.Charts;

namespace MudBlazor;

#nullable enable
public class SankeyChartOptions : DefaultChartOptions
{
    /// <summary>
    /// A collection of nodes to be rendered in the chart, allows overriding default node definitions.
    /// </summary>
    public IReadOnlyCollection<SankeyNode> NodeOverrides { get; set; } = [];

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
    /// Show labels for every node in the <see cref="ChartType.Sankey"/>
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>
    /// </remarks>
    public bool ShowLabels { get; set; } = true;

    /// <summary>
    /// Whether to show the values of the nodes within their respective labels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// Requires <see cref="ShowLabels"/> to be set to <c>true</c>.
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

    public static implicit operator SankeyChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
