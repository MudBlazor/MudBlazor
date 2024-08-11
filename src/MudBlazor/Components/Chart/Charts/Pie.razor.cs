using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays values as a percentage of a circle.
    /// </summary>
    partial class Pie : MudCategoryChartBase
    {
        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart MudChartParent { get; set; }

        private List<SvgPath> _paths = new();
        private List<SvgLegend> _legends = new();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _paths.Clear();
            _legends.Clear();

            var ndata = GetNormalizedData();
            double cumulativeRadians = 0;
            for (var i = 0; i < ndata.Length; i++)
            {
                var data = ndata[i];
                var startx = Math.Cos(cumulativeRadians);
                var starty = Math.Sin(cumulativeRadians);
                cumulativeRadians += 2 * Math.PI * data;
                var endx = Math.Cos(cumulativeRadians);
                var endy = Math.Sin(cumulativeRadians);
                var largeArcFlag = data > 0.5 ? 1 : 0;
                var path = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(startx)} {ToS(starty)} A 1 1 0 {ToS(largeArcFlag)} 1 {ToS(endx)} {ToS(endy)} L 0 0"
                };
                _paths.Add(path);
            }

            for (var i = 0; i < ndata.Length; i++)
            {
                var percent = ndata[i] * 100;
                var labels = i < InputLabels.Length ? InputLabels[i] : "";
                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = labels,
                    Data = ToS(Math.Round(percent, 1))
                };
                _legends.Add(legend);
            }
        }
    }
}
