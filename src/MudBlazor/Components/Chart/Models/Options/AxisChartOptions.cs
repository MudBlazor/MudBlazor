#nullable enable

namespace MudBlazor;

/// <summary>
/// Base options class for all charts using a x/y coordinate system.
/// </summary>
public abstract class AxisChartOptions : ChartOptions
{
    /// <summary>
    /// Rotation angle to rotate the labels in degrees.
    /// </summary>
    public int XAxisLabelRotation { get; set; }
    
    /// <summary>
    /// The spacing between vertical tick marks.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>20</c>.
    /// </remarks>
    public int YAxisTicks { get; set; } = 20;

    /// <summary>
    /// The maximum allowed number of vertical tick marks.
    /// </summary>
    /// <remarks>
    /// If the number of ticks calculated exceeds this value, the tick marks will automatically be thinned out.
    /// </remarks>
    public int MaxNumYAxisTicks { get; set; } = 20;

    /// <summary>
    /// The format applied to numbers on the vertical axis.
    /// </summary>
    /// <remarks>
    /// Values in this property are standard .NET format strings, such as those passed into the <c>ToString()</c> method.  For a list of common formats, see: <see href="https://learn.microsoft.com/dotnet/standard/base-types/formatting-types" />
    /// </remarks>
    public string? YAxisFormat { get; set; }

    /// <summary>
    /// Custom formatting function for vertical axis values.
    /// If set, this function will be used to convert Y-axis values to strings for display purposes.
    /// If not provided, <see cref="YAxisFormat"/> will be used instead.
    /// </summary>
    public Func<double, string>? YAxisToStringFunc { get; set; }

    /// <summary>
    /// Shows vertical axis lines.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool YAxisLines { get; set; } = true;

    /// <summary>
    /// Shows horizontal axis lines.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool XAxisLines { get; set; }

    /// <summary>
    /// Shows zero point on vertical axis.
    /// Only takes effect when the <see cref="MudChart"/> type is <see cref="ChartType.Line"/> or <see cref="MudTimeSeriesChartBase" /> is used.
    /// <remarks>
    /// Defaults to <c>false</c>
    /// </remarks>
    /// </summary>
    public bool YAxisRequireZeroPoint { get; set; }
}
