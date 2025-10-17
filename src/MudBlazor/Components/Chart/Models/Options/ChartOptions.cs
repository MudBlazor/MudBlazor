#nullable enable

namespace MudBlazor;

/// <summary>
/// Base options to customize the display of any <see cref="MudChart"/>.
/// </summary>
/// <remarks>
/// This class contains only options which are used by all chart types.
/// Chart-specific options are defined in their respective option-classes.
/// </remarks>
public class ChartOptions
{
    /// <summary>
    /// Make the chart fill the parent.
    /// </summary>
    public bool MatchBoundsToSize { get; set; }
    
    /// <summary>
    /// Shows the chart series legend.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool ShowLegend { get; set; } = true;

    /// <summary>
    /// The list of colors applied to series values.
    /// </summary>
    /// <remarks>
    /// Defaults to an array of <c>20</c> colors.
    /// </remarks>
    public string[] ChartPalette { get; set; } =
    [
        Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3,
        Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3, Colors.Teal.Lighten1, Colors.Amber.Lighten1,
        Colors.Orange.Lighten1, Colors.Red.Lighten1, Colors.DeepPurple.Lighten1, Colors.Green.Lighten1, Colors.LightBlue.Lighten1,
        Colors.Amber.Darken2, Colors.Orange.Darken2, Colors.Red.Darken2, Colors.DeepPurple.Darken2, Colors.Gray.Darken2
    ];

    /// <summary>
    /// Enables tooltips for values
    /// Defaults to <c>true</c>
    /// </summary>
    public bool ShowToolTips { get; set; } = true;

    public string DefaultDataMarkerTooltipTitleFormat { get; set; } = "{{Y_VALUE}} - {{X_VALUE}}";
}
