using System;
using System.Collections.Generic;
using System.Globalization;
using MudBlazor.Charts.Models;

namespace MudBlazor.Charts
{
    public class LineBase : MudChartBase
    {
        public List<ChartSegment> Segments = new List<ChartSegment>();
        public List<ChartLegend> Legends = new List<ChartLegend>();

        protected override void OnInitialized()
        {
            
        }
    }
}
