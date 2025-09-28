#nullable enable
namespace MudBlazor;

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
    /// Whether to constantly show the labels of the edges.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool ShowEdgeLabels { get; set; } = false;
}
