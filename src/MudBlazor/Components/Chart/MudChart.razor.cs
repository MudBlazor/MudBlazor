﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudChartBase : MudComponentBase
    {
        [Parameter] public double[] InputData { get; set; } = Array.Empty<double>();

        [Parameter] public string[] InputLabels { get; set; } = Array.Empty<string>();

        [Parameter] public string[] XAxisLabels { get; set; } = Array.Empty<string>();

        [Parameter] public List<ChartSeries> ChartSeries { get; set; } = new();

        [Parameter] public ChartOptions ChartOptions { get; set; } = new();

        protected string Classname =>
        new CssBuilder("mud-chart")
           .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [CascadingParameter]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The Type of the chart.
        /// </summary>
        [Parameter] public ChartType ChartType { get; set; }

        /// <summary>
        /// The Width of the chart, end with % or px.
        /// </summary>
        [Parameter] public string Width { get; set; } = "80%";

        /// <summary>
        /// The Height of the chart, end with % or px.
        /// </summary>
        [Parameter] public string Height { get; set; } = "80%";

        /// <summary>
        /// The placement direction of the legend if used.
        /// </summary>
        [Parameter] public Position LegendPosition { get; set; } = Position.Bottom;

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
        /// Selected index of a portion of the chart.
        /// </summary>
        [Parameter]
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
        /// Selected index of a portion of the chart.
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

    }

    public enum ChartType
    {
        Donut,
        Line,
        Pie,
        Bar
    }
}
