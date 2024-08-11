using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.Chart.Models;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudTimeSeriesChartBase : MudChartBase
    {
        /// <summary>
        /// The series of values to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<TimeSeriesChartSeries> ChartSeries { get; set; } = new();

        /// <summary>
        /// A way to have minimum spacing between timestamp labels, default of 5 minutes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public TimeSpan TimeLabelSpacing { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// A way to specify datetime formats for timestamp labels, default of HH:mm.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string TimeLabelFormat { get; set; } = "HH:mm";
    }
}
