using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.Chart.Models;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class MudTimeSeriesChartBase : MudChartBase
    {
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<TimeSeriesChartSeries> ChartSeries { get; set; } = new();

        [Parameter]
        public TimeSpan TimeLabelSpacing { get; set; } = TimeSpan.FromMinutes(5);
    }
}
