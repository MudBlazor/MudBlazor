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
            double counterClockwiseOffset = 25;
            double totalPercent = 0;
            double offset;

            var counter = 0;
            foreach (var data in GetNormalizedData())
            {
                var percent = data * 100;
                var reversePercent = 100 - percent;
                offset = 100 - totalPercent + counterClockwiseOffset;
                totalPercent += percent;

                var circle = new SvgCircle()
                {
                    Index = counter,
                    CX = 20,
                    CY = 20,
                    Radius = 15.91549430918954,
                    StrokeDashArray = $"{ToS(percent)} {ToS(reversePercent)}",
                    StrokeDashOffset = offset
                };
                _circles.Add(circle);


                var labels = "";
                if (counter < InputLabels.Length)
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

                counter += 1;
            }
        }

    }
}
