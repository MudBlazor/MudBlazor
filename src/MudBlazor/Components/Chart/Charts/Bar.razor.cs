using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    partial class Bar : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        private List<SvgPath> _horizontalLines = new();
        private List<SvgText> _horizontalValues = new();

        private List<SvgPath> _verticalLines = new();
        private List<SvgText> _verticalValues = new();

        private List<SvgLegend> _legends = new();
        private List<ChartSeries> _series = new();

        private List<SvgPath> _bars = new();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _horizontalLines.Clear();
            _verticalLines.Clear();
            _horizontalValues.Clear();
            _verticalValues.Clear();
            _legends.Clear();
            _bars.Clear();

            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            var maxY = 0.0m;
            var numValues = 0;
            var numXLabels = XAxisLabels.Count;
            foreach (var item in _series)
            {
                if (numValues < item.Data.Count)
                {
                    numValues = item.Data.Count;
                }
                foreach (int i in item.Data)
                {
                    if (maxY < i)
                    {
                        maxY = i;
                    }
                }
            }

            var boundHeight = 350.0m;
            var boundWidth = 650.0m;

            decimal gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20m;
            decimal gridXUnits = 30m;

            var numVerticalLines = numValues - 1;

            var numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;

            var verticalStartSpace = 25.0m;
            var horizontalStartSpace = 30.0m;
            var verticalEndSpace = 25.0m;
            var horizontalEndSpace = 30.0m;

            var verticalSpace = (boundHeight - verticalStartSpace - verticalEndSpace) / (numHorizontalLines);
            var horizontalSpace = (boundWidth - horizontalStartSpace - horizontalEndSpace) / (numVerticalLines);

            //Horizontal Grid Lines
            var y = verticalStartSpace;
            decimal startGridY = decimal.Zero;
            for (var counter = 0; counter <= numHorizontalLines; counter++)
            {
                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(horizontalStartSpace)} {ToS((boundHeight - y))} L {ToS((boundWidth - horizontalEndSpace))} {ToS((boundHeight - y))}"
                };
                _horizontalLines.Add(line);

                var lineValue = new SvgText() { X = (horizontalStartSpace - 10), Y = (boundHeight - y + 5), Value = ToS(startGridY, MudChartParent?.ChartOptions.YAxisFormat) };
                _horizontalValues.Add(lineValue);

                startGridY += gridYUnits;
                y += verticalSpace;
            }

            //Vertical Grid Lines
            var x = horizontalStartSpace;
            decimal startGridX = decimal.Zero;
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


            //Bars
            var colorcounter = 0;
            decimal barsPerSeries = decimal.Zero;
            foreach (var item in _series)
            {
                decimal gridValueX = horizontalStartSpace + barsPerSeries;
                decimal gridValueY = boundHeight - verticalStartSpace;

                foreach (var dataLine in item.Data)
                {
                    var dataValue = dataLine * verticalSpace / gridYUnits;
                    var gridValue = gridValueY - dataValue;
                    var bar = $"M {ToS(gridValueX)} {ToS(gridValueY)} L {ToS(gridValueX)} {ToS(gridValue)}";

                    gridValueX += horizontalSpace;

                    var line = new SvgPath()
                    {
                        Index = colorcounter,
                        Data = bar
                    };
                    _bars.Add(line);
                }

                barsPerSeries += 10m;

                var legend = new SvgLegend()
                {
                    Index = colorcounter,
                    Labels = item.Name
                };
                colorcounter++;
                _legends.Add(legend);
            }
        }
    }
}
