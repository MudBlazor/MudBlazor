using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.Models;

namespace MudBlazor.Charts
{
    public class DonutBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<ChartSegment> Segments = new List<ChartSegment>();
        public List<ChartLegend> Legends = new List<ChartLegend>();

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

                ChartSegment Segment = new ChartSegment()
                {
                    Index = Counter,
                    Cx = 21.ToString(CultureInfo.InvariantCulture),
                    Cy = 21.ToString(CultureInfo.InvariantCulture),
                    R = 15.915.ToString(CultureInfo.InvariantCulture),
                    StrokeDashArray = $"{Percent.ToString(CultureInfo.InvariantCulture)} {ReversePercent.ToString(CultureInfo.InvariantCulture)}",
                    StrokeDashOffset = Offset.ToString()
                };
                Segments.Add(Segment);


                string Labels = "";
                if (Counter < inputLabels.Length)
                {
                    Labels = inputLabels[Counter];
                }
                ChartLegend Legend = new ChartLegend()
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
