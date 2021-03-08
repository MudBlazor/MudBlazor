using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    partial class Line : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        private List<SvgPath> _horizontalLines = new List<SvgPath>();
        private List<SvgText> _horizontalValues = new List<SvgText>();

        private List<SvgPath> _verticalLines = new List<SvgPath>();
        private List<SvgText> _verticalValues = new List<SvgText>();

        private List<SvgLegend> _legends = new List<SvgLegend>();
        private List<ChartSeries> _series = new List<ChartSeries>();

        private List<SvgPath> _chartLines = new List<SvgPath>();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _horizontalLines.Clear();
            _verticalLines.Clear();
            _horizontalValues.Clear();
            _verticalValues.Clear();
            _legends.Clear();
            _chartLines.Clear();

            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            var maxY = 0.0;
            var numValues = 0;
            var numXLabels = XAxisLabels.Length;
            foreach (var item in _series)
            {
                if (numValues < item.Data.Length)
                {
                    numValues = item.Data.Length;
                }
                foreach (int i in item.Data)
                {
                    if (maxY < i)
                    {
                        maxY = i;
                    }
                }
            }

            var boundHeight = 350.0;
            var boundWidth = 650.0;

            double gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            double gridXUnits = 30;

            var numVerticalLines = numValues - 1;

            var numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;

            var verticalStartSpace = 25.0;
            var horizontalStartSpace = 30.0;
            var verticalEndSpace = 25.0;
            var horizontalEndSpace = 30.0;

            var verticalSpace = (boundHeight - verticalStartSpace - verticalEndSpace) / (numHorizontalLines);
            var horizontalSpace = (boundWidth - horizontalStartSpace - horizontalEndSpace) / (numVerticalLines);

            //Horizontal Grid Lines
            var y = verticalStartSpace;
            double startGridY = 0;
            for (var counter = 0; counter <= numHorizontalLines; counter++)
            {
                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(horizontalStartSpace)} {ToS((boundHeight - y))} L {ToS((boundWidth - horizontalEndSpace))} {ToS((boundHeight - y))}"
                };
                _horizontalLines.Add(line);

                var lineValue = new SvgText() { X = (horizontalStartSpace - 10), Y = (boundHeight - y + 5), Value = ToS(startGridY) };
                _horizontalValues.Add(lineValue);

                startGridY += gridYUnits;
                y += verticalSpace;
            }

            //Vertical Grid Lines
            var x = horizontalStartSpace;
            double startGridX = 0;
            for (var counter = 0; counter <= numVerticalLines; counter++)
            {

                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(x)} {ToS((boundHeight - verticalStartSpace))} L {ToS(x)} {ToS(verticalEndSpace)}"
                };
                _verticalLines.Add(line);

                var xLabels = "";
                if (counter < numXLabels)
                {
                    xLabels = XAxisLabels[counter];
                }

                var lineValue = new SvgText() { X = x, Y = boundHeight - 2, Value = xLabels };
                _verticalValues.Add(lineValue);

                startGridX += gridXUnits;
                x += horizontalSpace;
            }


            //Chart Lines
            var colorcounter = 0;
            foreach (var item in _series)
            {
                var chartLine = "";
                double gridValueX = 0;
                double gridValueY = 0;
                var firstTime = true;

                foreach (var dataLine in item.Data)
                {
                    if (firstTime)
                    {
                        chartLine += "M ";
                        firstTime = false;
                        gridValueX = horizontalStartSpace;
                        gridValueY = verticalStartSpace;
                    }
                    else
                    {
                        chartLine += " L ";
                        gridValueX += horizontalSpace;
                        gridValueY = verticalStartSpace;
                    }

                    var gridValue = ((double)dataLine) * verticalSpace / gridYUnits;
                    gridValueY = boundHeight - (gridValueY + gridValue);
                    chartLine = chartLine + ToS(gridValueX) + " " + ToS(gridValueY);
                }

                var line = new SvgPath()
                {
                    Index = colorcounter,
                    Data = chartLine
                };

                var legend = new SvgLegend()
                {
                    Index = colorcounter,
                    Labels = item.Name
                };
                colorcounter++;
                _chartLines.Add(line);
                _legends.Add(legend);
            }
        }
    }
}
