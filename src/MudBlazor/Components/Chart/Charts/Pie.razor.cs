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
            string[] inputData = InputData.Split(',');
            string[] inputLabels = InputLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            double x, y;
            double px = 0, py = 0;
            double TotalPercent = 0;
            string Radius = PieRadius.ToString();
            for (int icounter = 0; icounter < inputData.Length; icounter++)
            {
                double data = 0;
                bool isDouble2 = double.TryParse(inputData[icounter], out data);
                double Percent = data / 100;

                TotalPercent = TotalPercent + Percent;
                x = PieRadius * Math.Cos(2 * Math.PI * TotalPercent);
                y = PieRadius * Math.Sin(2 * Math.PI * TotalPercent);
                SvgPath Path = null;
                if (icounter == 0)
                {
                    Path = new SvgPath()
                    {
                        Index = icounter,
                        Data = $"M {Radius.ToString(CultureInfo.InvariantCulture)} 0 A {Radius.ToString(CultureInfo.InvariantCulture)} {Radius.ToString(CultureInfo.InvariantCulture)} 0 0 1 {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} L 0 0"
                    };
                }
                else
                {
                    if (Percent > 0.5)
                    {
                        Path = new SvgPath()
                        {
                            Index = icounter,
                            Data = $"M {px.ToString(CultureInfo.InvariantCulture)} {py.ToString(CultureInfo.InvariantCulture)} A {Radius} {Radius.ToString(CultureInfo.InvariantCulture)} 0 1 1 {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} L 0 0"
                        };
                    }
                    else
                    {
                        Path = new SvgPath()
                        {
                            Index = icounter,
                            Data = $"M {px.ToString(CultureInfo.InvariantCulture)} {py.ToString(CultureInfo.InvariantCulture)} A {Radius.ToString(CultureInfo.InvariantCulture)} {Radius.ToString(CultureInfo.InvariantCulture)} 0 0 1 {x.ToString(CultureInfo.InvariantCulture)} {y.ToString(CultureInfo.InvariantCulture)} L 0 0"
                        };
                    }
                }
                Paths.Add(Path);
                px = x; py = y;
            }

            int Counter = 0;
            foreach (string dataString in inputData)
            {
                double Data = double.Parse(dataString);
                string Labels = "";
                if (Counter < inputLabels.Length)
                {
                    Labels = inputLabels[Counter];
                }
                SvgLegend Legend = new SvgLegend()
                {
                    Index = Counter,
                    Labels = Labels,
                    Data = Data.ToString()
                };
                Legends.Add(Legend);
                Counter += 1;
            }
        }
    }
}
