using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudBlazor.Charts;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class MudChartBase : MudComponentBase
    {
        [Parameter] public double[] InputData { get; set; }

        [Parameter] public string[] InputLabels { get; set; }

        [Parameter] public string[] XAxisLabels { get; set; }

        [Parameter] public List<ChartSeries> ChartSeries { get; set; }

        [Parameter] public ChartOptions ChartOptions { get; set; } = new ChartOptions();

        protected string Classname =>
        new CssBuilder("mud-chart")
           .AddClass($"mud-chart-legend-{LegendPosition.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

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
        /// The placment direction of the legend if used.
        /// </summary>
        [Parameter] public LegendPosition LegendPosition { get; set; } = LegendPosition.Bottom;

        /// <summary>
        /// Scales the input data to the range between 0 and 1
        /// </summary>
        protected double[] GetNormalizedData()
        {
            var total = InputData.Sum();
            return InputData.Select(x => Math.Abs(x) / total).ToArray();
        }

        protected string ToS(double d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }
    }

    public enum ChartType
    {
        Donut,
        Line,
        Pie
    }
}
