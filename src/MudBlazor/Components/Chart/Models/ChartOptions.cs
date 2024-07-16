using System.Diagnostics;

namespace MudBlazor
{
    /// <summary>
    /// Represents options which customize the display of a <see cref="MudChart"/>.
    /// </summary>
    /// <remarks>
    /// This class is typically used to control display features of a chart such as: colors, the number of horizontal and vertical ticks, and line smoothing options.
    /// </remarks>
    public class ChartOptions
    {
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
        /// Values in this property are standard .NET format strings, such as those passed into the <c>ToString()</c> method.  For a list of common formats, see: <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types" />
        /// </remarks>
        public string YAxisFormat { get; set; }

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
        public bool YAxisRequireZeroPoint { get; set; }

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
        {
            Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3,
            Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3, Colors.Teal.Lighten1, Colors.Amber.Lighten1,
            Colors.Orange.Lighten1, Colors.Red.Lighten1, Colors.DeepPurple.Lighten1, Colors.Green.Lighten1, Colors.LightBlue.Lighten1,
            Colors.Amber.Darken2, Colors.Orange.Darken2, Colors.Red.Darken2, Colors.DeepPurple.Darken2, Colors.Gray.Darken2
        };

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
    }

    /// <summary>
    /// Indicates the technique used to smooth lines connecting values in a <see cref="MudChart"/>.
    /// </summary>
    public enum InterpolationOption
    {
        /// <summary>
        /// Lines are smoothed to pass through each data point as a natural spline.
        /// </summary>
        NaturalSpline,

        /// <summary>
        /// Lines are smoothed to pass through each data point as an end-point spline.
        /// </summary>
        EndSlope,

        /// <summary>
        /// Lines are smoothed to pass through each data point as a periodic spline.
        /// </summary>
        Periodic,

        /// <summary>
        /// Lines are straight from one point to the next.
        /// </summary>
        Straight
    }
}
