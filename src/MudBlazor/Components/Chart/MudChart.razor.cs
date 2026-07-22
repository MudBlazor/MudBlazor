using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a graphic display of data values in a line, bar, stacked bar, pie, or donut shape.
    /// </summary>
    public partial class MudChart
    {
    }

    /// <summary>
    /// Represents a base class for designing category <see cref="MudChart"/> components.
    /// </summary>
    public abstract class MudCategoryChartBase : MudChartBase
    {
        /// <summary>
        /// The data to be displayed.
        /// </summary>
        /// <remarks>
        /// Applies to <c>Pie</c> and <c>Donut</c> charts.  The number of values in this array should be the same as the number of labels in the <see cref="InputLabels"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public double[] InputData { get; set; } = Array.Empty<double>();

        /// <summary>
        /// The labels describing data values.
        /// </summary>
        /// <remarks>
        /// Applies to <c>Pie</c> and <c>Donut</c> charts.  The number of labels in this array is typically the same as the number of values in the <see cref="InputData"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] InputLabels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The labels applied to the horizontal axis.
        /// </summary>
        /// <remarks>
        /// Applies to <c>Line</c>, <c>Bar</c>, and <c>StackedBar</c> charts.  The number of values in this array is typically equal to the number of values in the <see cref="ChartSeries.Data"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// The series of values to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<ChartSeries> ChartSeries { get; set; } = new();

        /// <summary>
        /// Scales the input data to the range between 0 and 1
        /// </summary>
        protected double[] GetNormalizedData()
        {
            if (InputData == null)
                return Array.Empty<double>();
            var total = InputData.Sum();
            return InputData.Select(x => Math.Abs(x) / total).ToArray();
        }
    }

    /// <summary>
    /// Shared a base class for designing category <see cref="MudChart"/> and <see cref="MudTimeSeriesChart"/> components.
    /// </summary>
    public abstract class MudChartBase : MudComponentBase
    {
        /// <summary>
        /// The display options applied to the chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public ChartOptions ChartOptions { get; set; } = new();

        /// <summary>
        /// The custom graphics within this chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public RenderFragment CustomGraphics { get; set; }

        protected string Classname => new CssBuilder("mud-chart")
            .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        [CascadingParameter(Name = "RightToLeft")]
        [Category(CategoryTypes.Chart.Behavior)]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The type of chart to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public ChartType ChartType { get; set; }

        /// <summary>
        /// The width of the chart, as a CSS style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>80%</c>.  Values can be a percentage or pixel width such as <c>200px</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public string Width { get; set; } = "80%";

        /// <summary>
        /// The height of the chart, as a CSS style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>80%</c>.  Values can be a percentage or pixel width such as <c>200px</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public string Height { get; set; } = "80%";

        /// <summary>
        /// The location of series labels.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Position.Bottom"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public Position LegendPosition { get; set; } = Position.Bottom;

        private Position ConvertLegendPosition(Position position)
        {
            return position switch
            {
                Position.Start => RightToLeft ? Position.Right : Position.Left,
                Position.End => RightToLeft ? Position.Left : Position.Right,
                _ => position
            };
        }

        private int _selectedIndex;

        /// <summary>
        /// The currently selected data point.
        /// </summary>
        /// <remarks>
        /// When this property changes, the <see cref="SelectedIndexChanged"/> event occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    SelectedIndexChanged.InvokeAsync(value);
                }
            }
        }

        internal void SetSelectedIndex(int index)
        {
            SelectedIndex = index;
        }

        /// <summary>
        /// Occurs when the <see cref="SelectedIndex"/> has changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        protected string ToS(double d, string format = null)
        {
            if (string.IsNullOrEmpty(format))
                return d.ToString(CultureInfo.InvariantCulture);

            return d.ToString(format);
        }

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

    /// <summary>
    /// Indicates the type of chart to display.
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// Data is displayed as a hollow circle.
        /// </summary>
        Donut,
        /// <summary>
        /// Data is displayed as connecting lines.
        /// </summary>
        Line,
        /// <summary>
        /// Data is displayed as a portion of a circle.
        /// </summary>
        Pie,
        /// <summary>
        /// Data is displayed as rectangles.
        /// </summary>
        Bar,
        /// <summary>
        /// Data is displayed as connected rectangles.
        /// </summary>
        StackedBar,
        /// <summary>
        /// Data is displayed as connecting lines or as areas.
        /// </summary>
        Timeseries
    }
}
