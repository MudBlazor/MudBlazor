using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    public class DonutBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<SvgCircle> Circles = new List<SvgCircle>();
        public List<SvgLegend> Legends = new List<SvgLegend>();

        protected string ParentWidth => MudChartParent?.Width;
        protected string ParentHeight => MudChartParent?.Height;

        protected override void OnParametersSet()
        {
            Circles.Clear();
            Legends.Clear();
            double counterClockwiseOffset = 25;
            double totalPercent = 0;
            double offset = counterClockwiseOffset;

            int counter = 0;
            foreach (double data in GetNormalizedData())
            {
                double percent = data*100;
                double reversePercent = 100 - percent;
                offset = 100 - totalPercent + counterClockwiseOffset;
                totalPercent = totalPercent + percent;

                var circle = new SvgCircle()
                {
                    Index = counter,
                    CX = 20,
                    CY = 20,
                    Radius = 15.91549430918954,
                    StrokeDashArray = $"{ToS(percent)} {ToS(reversePercent)}",
                    StrokeDashOffset = offset
                };
                Circles.Add(circle);


                string labels = "";
                if (counter < InputLabels.Length)
                {
                    labels = InputLabels[counter];
                }
                SvgLegend Legend = new SvgLegend()
                {
                    Index = counter,
                    Labels = labels,
                    Data = data.ToString()
                };
                Legends.Add(Legend);

                counter += 1;
            }
        }

    }
}
