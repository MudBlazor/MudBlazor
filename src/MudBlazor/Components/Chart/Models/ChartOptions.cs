// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

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
        /// Values in this property are standard .NET format strings, such as those passed into the <c>ToString()</c> method.  For a list of common formats, see: <see href="https://learn.microsoft.com/dotnet/standard/base-types/formatting-types" />
        /// </remarks>
        public string? YAxisFormat { get; set; }

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
        [
            Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3,
            Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3, Colors.Teal.Lighten1, Colors.Amber.Lighten1,
            Colors.Orange.Lighten1, Colors.Red.Lighten1, Colors.DeepPurple.Lighten1, Colors.Green.Lighten1, Colors.LightBlue.Lighten1,
            Colors.Amber.Darken2, Colors.Orange.Darken2, Colors.Red.Darken2, Colors.DeepPurple.Darken2, Colors.Gray.Darken2
        ];

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
        /// Enables smooth color transitions for heatmap cells and removes all padding between cells in a <see cref="ChartType.HeatMap"/>
        /// Defaults to false
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
        /// Enables tooltips for values in a <see cref="ChartType.HeatMap"/>
        /// Defaults to true
        /// </summary>
        public bool ShowToolTips { get; set; } = true;

        /// <summary>
        /// Enables labels for every box in a <see cref="ChartType.HeatMap"/>
        /// Defaults to true
        /// </summary>
        public bool ShowLabels { get; set; } = true;

        /// <summary>
        /// Enables label values for the legend boxes in a <see cref="ChartType.HeatMap"/>
        /// Defaults to false
        /// </summary>
        /// 
        public bool ShowLegendLabels { get; set; } = false;

        /// <summary>
        /// The format applied to labels for every box in a <see cref="ChartType.HeatMap"/>
        /// Defaults to "F2"
        /// </summary>
        public string ValueFormatString { get; set; } = "F2";
    }
}
