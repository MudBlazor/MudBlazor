﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a base class for designing <see cref="MudChart"/> components.
    /// </summary>
    public abstract class MudChartBase : MudComponentBase
    {
        /// <summary>
        /// Gets or sets the data to be displayed.
        /// </summary>
        /// <remarks>
        /// The values in this array should align with labels in the <see cref="InputLabels"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public double[] InputData { get; set; } = Array.Empty<double>();

        /// <summary>
        /// Gets or sets the labels describing data values.
        /// </summary>
        /// <remarks>
        /// The values in this array should align with data values in the <see cref="InputData"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] InputLabels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the labels applied to the horizontal axis.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the series of values to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<ChartSeries> ChartSeries { get; set; } = new();

        /// <summary>
        /// Gets or sets display options applied to the chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public ChartOptions ChartOptions { get; set; } = new();

        /// <summary>
        /// Gets or sets any custom graphics within this chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public RenderFragment CustomGraphics { get; set; }

        protected string Classname => new CssBuilder("mud-chart")
            .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Gets or sets whether text is displayed Right-to-Left (RTL).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, text will display property for RTL languages such as Arabic, Hebrew, and Persian.
        /// </remarks>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets the type of chart to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public ChartType ChartType { get; set; }

        /// <summary>
        /// Gets or sets the width of the chart, as a CSS style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>80%</c>.  Values can be a percentage or pixel width such as <c>200px</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public string Width { get; set; } = "80%";

        /// <summary>
        /// Gets or sets the height of the chart, as a CSS style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>80%</c>.  Values can be a percentage or pixel width such as <c>200px</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public string Height { get; set; } = "80%";

        /// <summary>
        /// Gets or sets the location of series labels.
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
        /// Gets or sets the currently selected data point.
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

        /// <summary>
        /// Occurs when the <see cref="SelectedIndex"/> has changed.
        /// </summary>
        [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }

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

        protected string ToS(double d, string format = null)
        {
            if (string.IsNullOrEmpty(format))
                return d.ToString(CultureInfo.InvariantCulture);

            return d.ToString(format);
        }

        /// <summary>
        /// Gets or sets whether lines can be hidden when <see cref="ChartType"/> is <see cref="ChartType.Line"/>.
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
        StackedBar
    }
}
