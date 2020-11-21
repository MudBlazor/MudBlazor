using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    public class LineBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<SvgPath> HorizontalLines = new List<SvgPath>();
        public List<SvgText> HorizontalValues = new List<SvgText>();

        public List<SvgPath> VerticalLines = new List<SvgPath>();
        public List<SvgText> VerticalValues = new List<SvgText>();

        public List<SvgLegend> Legends = new List<SvgLegend>();
        public List<ChartSeries> Series = new List<ChartSeries>();

        public List<SvgPath> ChartLines = new List<SvgPath>();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            HorizontalLines.Clear();
            VerticalLines.Clear();
            HorizontalValues.Clear();
            VerticalValues.Clear();
            Legends.Clear();
            ChartLines.Clear();

            if (MudChartParent!=null)
                Series = MudChartParent.ChartSeries;

            double maxY = 0.0;
            int numValues = 0;
            int numXLabels = XAxisLabels.Length;
            foreach (var item in Series)
            {
                if(numValues < item.Data.Length)
                {
                    numValues = item.Data.Length;
                }
                foreach (int i in item.Data)
                {
                    if(maxY < i)
                    {
                        maxY = i;
                    }
                }
            }

            double boundHeight = 350.0;
            double boundWidth = 650.0;

            double gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            double gridXUnits = 30;

            int numVerticalLines = numValues - 1;

            int numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;

            double verticalStartSpace = 25.0;
            double horizontalStartSpace = 30.0;
            double verticalEndSpace = 25.0;
            double horizontalEndSpace = 30.0;

            double verticalSpace = (boundHeight - verticalStartSpace - verticalEndSpace) / (numHorizontalLines);
            double horizontalSpace = (boundWidth - horizontalStartSpace - horizontalEndSpace) / (numVerticalLines);

            //Horizontal Grid Lines
            double y = verticalStartSpace;
            double startGridY = 0;
            for (int counter = 0; counter <= numHorizontalLines; counter++)
            {
                SvgPath Line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(horizontalStartSpace)} {ToS((boundHeight - y))} L {ToS((boundWidth - horizontalEndSpace))} {ToS((boundHeight - y))}"
                };
                HorizontalLines.Add(Line);

                SvgText LineValue = new SvgText() { X = (horizontalStartSpace - 10), Y = (boundHeight - y + 5), Value = ToS(startGridY) };
                HorizontalValues.Add(LineValue);

                startGridY = startGridY + gridYUnits;
                y = y + verticalSpace;
            }

            //Vertical Grid Lines
            double x = horizontalStartSpace;
            double startGridX = 0;
            for (int counter = 0; counter <= numVerticalLines; counter++)
            {

                SvgPath Line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(x)} {ToS((boundHeight - verticalStartSpace))} L {ToS(x)} {ToS(verticalEndSpace)}"
                };
                VerticalLines.Add(Line);

                string xLabels = "";
                if(counter < numXLabels)
                {
                    xLabels = XAxisLabels[counter];
                }

                SvgText LineValue = new SvgText() { X = x, Y = boundHeight -2, Value = xLabels };
                VerticalValues.Add(LineValue);

                startGridX = startGridX + gridXUnits;
                x = x + horizontalSpace;
            }


            //Chart Lines
            int colorcounter = 0;
            foreach (var item in Series)
            {
                string chartLine = "";
                double gridValueX = 0;
                double gridValueY = 0;
                bool firstTime = true;

                foreach(var line in item.Data)
                {
                    if (firstTime)
                    {
                        chartLine = chartLine + "M ";
                        firstTime = false;
                        gridValueX = horizontalStartSpace;
                        gridValueY = verticalStartSpace;
                        double gridValue = ((double)line) * verticalSpace / gridYUnits;
                        gridValueY = boundHeight - (gridValueY + gridValue);
                        chartLine = chartLine + ToS(gridValueX) + " " + ToS(gridValueY);
                    }
                    else
                    {
                        chartLine = chartLine + " L ";
                        gridValueX = gridValueX + horizontalSpace;
                        gridValueY = verticalStartSpace;

                        double gridValue = ((double)line) * verticalSpace / gridYUnits;
                        gridValueY = boundHeight - (gridValueY + gridValue);
                        chartLine = chartLine + ToS(gridValueX) + " " + ToS(gridValueY);
                    }
                }
                SvgPath Line = new SvgPath()
                {
                    Index = colorcounter,
                    Data = chartLine
                };
                SvgLegend Legend = new SvgLegend()
                {
                    Index = colorcounter,
                    Labels = item.Name
                };
                colorcounter = colorcounter + 1;
                ChartLines.Add(Line);
                Legends.Add(Legend);
            }
        }
    }
}
