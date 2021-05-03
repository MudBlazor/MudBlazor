using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class MudChartBase : MudComponentBase
    {
        [Parameter] public double[] InputData { get; set; } = Array.Empty<double>();

        [Parameter] public string[] InputLabels { get; set; } = Array.Empty<string>();

        [Parameter] public string[] XAxisLabels { get; set; } = Array.Empty<string>();

        [Parameter] public List<ChartSeries> ChartSeries { get; set; } = new List<ChartSeries>();

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
        [Parameter] public Position LegendPosition { get; set; } = Position.Bottom;

        /// <summary>
        /// Selected index of a portion of the chart.
        /// </summary>     
        [Parameter] public int SelectedIndex { get; set; }

        /// <summary>
        /// Selected index of a portion of the chart.
        /// </summary>    
        [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }

        /// <summary>
        /// EventCallback selected portion of a chart based on the selected index. 
        /// </summary>
        protected double[] GetNormalizedData()
        {
            if (InputData == null)
                return Array.Empty<double>();
            var total = InputData.Sum();
            return InputData.Select(x => Math.Abs(x) / total).ToArray();
        }

        protected async Task UpdateSelectedIndex(int index)
        {
            SelectedIndex = index;
            await SelectedIndexChanged.InvokeAsync(SelectedIndex);
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
        Pie,
        Bar
    }
}
