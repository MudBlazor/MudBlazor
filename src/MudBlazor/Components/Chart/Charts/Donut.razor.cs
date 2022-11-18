using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    partial class Donut : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        private List<SvgCircle> _circles = new();
        private List<SvgLegend> _legends = new();

        protected string ParentWidth => MudChartParent?.Width;
        protected string ParentHeight => MudChartParent?.Height;

        protected override void OnParametersSet()
        {
            _circles.Clear();
            _legends.Clear();
            double counterClockwiseOffset = 25d;
            decimal totalPercent = 0m;
            double offset;

            var counter = 0;
            foreach (var data in GetNormalizedData())
            {
                var percent = data * 100;
                var reversePercent = 100 - percent;
                offset = 100 - totalPercent + counterClockwiseOffset;
                totalPercent += percent;

                var circle = new SvgCircle
                {
                    Index = counter,
                    CX = 21d,
                    CY = 21d,
                    Radius = 15.91549430918954d,
                    StrokeDashArray = $"{ToS(percent)} {ToS(reversePercent)}",
                    StrokeDashOffset = offset
                };
                _circles.Add(circle);

                var labels = string.Empty;
                if (counter < InputLabels.Count)
                {
                    labels = InputLabels[counter];
                }
                var legend = new SvgLegend()
                {
                    Index = counter,
                    Labels = labels,
                    Data = data.ToString()
                };
                _legends.Add(legend);

                counter++;
            }
        }
    }
}
