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

        public List<SvgPath> Paths = new List<SvgPath>();
        public List<SvgLegend> Legends = new List<SvgLegend>();

        private double radius = 1;
        private void GetCoordinatesForPercent(double percent, out double x, out double y)
        {
            x = radius * Math.Cos(2 * Math.PI * percent);
            y = radius * Math.Sin(2 * Math.PI * percent);
        }

        protected override void OnInitialized()
        {
            double x, y;
            double px = 0, py = 0;
            double totalPercent = 0;
            var radius = this.radius;
            var ndata = GetNormalizedData();
            for (int icounter = 0; icounter < ndata.Length; icounter++)
            {
                double percent = ndata[icounter];

                totalPercent = totalPercent + percent;
                x = this.radius * Math.Cos(2 * Math.PI * totalPercent);
                y = this.radius * Math.Sin(2 * Math.PI * totalPercent);
                SvgPath path = null;
                if (icounter == 0)
                {
                    path = new SvgPath()
                    {
                        Index = icounter,
                        Data = $"M {ToS(radius)} 0 A {ToS(radius)} {ToS(radius)} 0 0 1 {ToS(x)} {ToS(y)} L 0 0"
                    };
                }
                else
                {
                    if (percent > 0.5)
                    {
                        path = new SvgPath()
                        {
                            Index = icounter,
                            Data = $"M {ToS(px)} {ToS(py)} A {ToS(radius)} {ToS(radius)} 0 1 1 {ToS(x)} {ToS(y)} L 0 0"
                        };
                    }
                    else
                    {
                        path = new SvgPath()
                        {
                            Index = icounter,
                            Data = $"M {ToS(px)} {ToS(py)} A {ToS(radius)} {ToS(radius)} 0 0 1 {ToS(x)} {ToS(y)} L 0 0"
                        };
                    }
                }
                Paths.Add(path);
                px = x; py = y;
            }

            int counter = 0;
            foreach (double data in ndata)
            {
                var percent = data * 100;
                string labels = "";
                if (counter < InputLabels.Length)
                {
                    labels = InputLabels[counter];
                }
                SvgLegend Legend = new SvgLegend()
                {
                    Index = counter,
                    Labels = labels,
                    Data = ToS(percent)
                };
                Legends.Add(Legend);
                counter += 1;
            }
        }
    }
}
