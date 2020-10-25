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

        private double PieRadius = 1;
        private void GetCoordinatesForPercent(double percent, out double x, out double y)
        {
            x = PieRadius * Math.Cos(2 * Math.PI * percent);
            y = PieRadius * Math.Sin(2 * Math.PI * percent);
        }

        protected override void OnInitialized()
        {
            double x, y;
            double px = 0, py = 0;
            double totalPercent = 0;
            string radius = PieRadius.ToString();
            for (int icounter = 0; icounter < InputData.Length; icounter++)
            {
                double data = InputData[icounter];
                double percent = data / 100;

                totalPercent = totalPercent + percent;
                x = PieRadius * Math.Cos(2 * Math.PI * totalPercent);
                y = PieRadius * Math.Sin(2 * Math.PI * totalPercent);
                SvgPath path = null;
                if (icounter == 0)
                {
                    path = new SvgPath()
                    {
                        Index = icounter,
                        Data = $"M {radius.ToString(CultureInfo.InvariantCulture)} 0 A {radius.ToString(CultureInfo.InvariantCulture)} {radius.ToString(CultureInfo.InvariantCulture)} 0 0 1 {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} L 0 0"
                    };
                }
                else
                {
                    if (percent > 0.5)
                    {
                        path = new SvgPath()
                        {
                            Index = icounter,
                            Data = $"M {px.ToString(CultureInfo.InvariantCulture)} {py.ToString(CultureInfo.InvariantCulture)} A {radius} {radius.ToString(CultureInfo.InvariantCulture)} 0 1 1 {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} L 0 0"
                        };
                    }
                    else
                    {
                        path = new SvgPath()
                        {
                            Index = icounter,
                            Data = $"M {px.ToString(CultureInfo.InvariantCulture)} {py.ToString(CultureInfo.InvariantCulture)} A {radius.ToString(CultureInfo.InvariantCulture)} {radius.ToString(CultureInfo.InvariantCulture)} 0 0 1 {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} L 0 0"
                        };
                    }
                }
                Paths.Add(path);
                px = x; py = y;
            }

            int counter = 0;
            foreach (double data in InputData)
            {
                string labels = "";
                if (counter < InputLabels.Length)
                {
                    labels = InputLabels[counter];
                }
                SvgLegend Legend = new SvgLegend()
                {
                    Index = counter,
                    Labels = labels,
                    Data = data.ToString(CultureInfo.InvariantCulture)
                };
                Legends.Add(Legend);
                counter += 1;
            }
        }
    }
}
