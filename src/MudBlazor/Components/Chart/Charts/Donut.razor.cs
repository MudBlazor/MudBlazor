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

        protected override void OnInitialized()
        {
            double CounterClockwiseOffset = 25;
            double TotalPercent = 0;
            double Offset = CounterClockwiseOffset;

            string[] inputData = InputData.Split(',');
            string[] inputLabels = InputLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int Counter = 0;
            foreach (string dataString in inputData)
            {
                double Data = 0;
                bool isDouble2 = double.TryParse(dataString, out Data);

                double Percent = Data;
                double ReversePercent = 100 - Percent;
                Offset = 100 - TotalPercent + CounterClockwiseOffset;
                TotalPercent = TotalPercent + Percent;

                SvgCircle Circle = new SvgCircle()
                {
                    Index = Counter,
                    CX = 21,
                    CY = 21,
                    Radius = 15.915,
                    StrokeDashArray = $"{Percent.ToString(CultureInfo.InvariantCulture)} {ReversePercent.ToString(CultureInfo.InvariantCulture)}",
                    StrokeDashOffset = Offset
                };
                Circles.Add(Circle);


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
