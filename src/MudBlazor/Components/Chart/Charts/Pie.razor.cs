using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    public class PieBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<SvgCircle> Circles = new List<SvgCircle>();
        public List<SvgLegend> Legends = new List<SvgLegend>();

        protected override void OnInitialized()
        {
            double counterClockwiseOffset = 25;
            double totalPercent = 0;
            double offset = counterClockwiseOffset;

            int counter = 0;
            foreach (double data in GetNormalizedData())
            {
                double percent = data * 100;
                double reversePercent = 100 - percent;
                offset = 100 - totalPercent + counterClockwiseOffset;
                totalPercent = totalPercent + percent;

                var circle = new SvgCircle()
                {
                    Index = counter,
                    CX = 10,
                    CY = 10,
                    Radius = 5,
                    StrokeDashArray = $"calc({percent.ToString(CultureInfo.InvariantCulture)} * 31.4 / 100) 31.4",
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
