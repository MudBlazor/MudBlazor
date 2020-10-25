using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.Models;

namespace MudBlazor.Charts
{
    public class LineBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<ChartSegment> Segments = new List<ChartSegment>();
        public List<ChartLegend> Legends = new List<ChartLegend>();
        public List<ChartSeries> Series = new List<ChartSeries>();

        protected override void OnInitialized()
        {

            string[] inputLabels = InputLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] xAxisinputLabels = XAxisLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            

            int numLines = 0;
            foreach (var item in ChartSeries)
            {

            }
        }
    }
}
