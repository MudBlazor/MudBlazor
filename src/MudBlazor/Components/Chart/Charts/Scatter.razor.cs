using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;
using MudBlazor.Components.Chart;
using MudBlazor.Components.Chart.Interpolation;

namespace MudBlazor.Charts
{
    partial class Scatter : MudChartBase
    {
        private const int MaxHorizontalGridLines = 100;

        [CascadingParameter] public MudChart MudChartParent { get; set; }

        private List<SvgPath> _horizontalLines = new();
        private List<SvgText> _horizontalValues = new();

        private List<SvgPath> _verticalLines = new();
        private List<SvgText> _verticalValues = new();

        private List<SvgLegend> _legends = new();
        private List<XYChartSeries> _series = new();

        private List<SvgCircle> _chartDots = new();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _horizontalLines.Clear();
            _verticalLines.Clear();
            _horizontalValues.Clear();
            _verticalValues.Clear();
            _legends.Clear();
            _chartDots.Clear();

            if (MudChartParent != null)
                _series = MudChartParent.XYChartSeries;

            var maxY = 0.0;
            var minY = 0.0;
            var maxX = 0.0;
            var minX = 0.0;
            var numYValues = 0;
            var numXValues = 0;
            foreach (var item in _series)
            {
                if (numYValues < item.YData.Length)
                {
                    numYValues = item.YData.Length;
                }
                if (numXValues < item.XData.Length)
                {
                    numXValues = item.XData.Length;
                }
                foreach (int i in item.YData)
                {
                    if (maxY < i)
                    {
                        maxY = i;
                    }
                    if (minY > i)
                    {
                        minY = i;
                    }
                }
                foreach (int i in item.XData)
                {
                    if (maxX < i)
                    {
                        maxX = i;
                    }
                    if (minX > i)
                    {
                        minX = i;
                    }
                }
            }

            var boundHeight = 350.0;
            var boundWidth = 650.0;

            double gridYUnits = MudChartParent?.XYChartOptions.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;
            int maxYTicks = MudChartParent?.XYChartOptions.MaxNumYAxisTicks ?? 100;

            double gridXUnits = MudChartParent?.XYChartOptions.XAxisTicks ?? 20;
            if (gridXUnits <= 0)
                gridXUnits = 20;
            int maxXTicks = MudChartParent?.XYChartOptions.MaxNumXAxisTicks ?? 100;


            var numVerticalLines = ((int)(maxX / gridXUnits)) + 1;

            var numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;

            // this is a safeguard against millions of gridlines which might arise with very high values
            while (numHorizontalLines > maxYTicks)
            {
                gridYUnits *= 2;
                numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;
            }
            while (numVerticalLines > maxXTicks)
            {
                gridXUnits *= 2;
                numVerticalLines = ((int)(maxX / gridXUnits)) + 1;
            }
       
            var verticalStartSpace = 25.0;
            var horizontalStartSpace = 30.0;
            var verticalEndSpace = 25.0;
            var horizontalEndSpace = 30.0;

            var verticalSpace = (boundHeight - verticalStartSpace - verticalEndSpace) / (numHorizontalLines);
            var horizontalSpace = (boundWidth - horizontalStartSpace - horizontalEndSpace) / (numVerticalLines);

            //Horizontal Grid Lines
            var y = verticalStartSpace;
            double offsetY = Math.Ceiling(minY / gridYUnits) * gridYUnits;
            double startGridY = offsetY;
            for (var counter = 0; counter <= numHorizontalLines; counter++)
            {
                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(horizontalStartSpace)} {ToS((boundHeight - y))} L {ToS((boundWidth - horizontalEndSpace))} {ToS((boundHeight - y))}"
                };
                _horizontalLines.Add(line);

                var lineValue = new SvgText() { X = (horizontalStartSpace - 15), Y = (boundHeight - y + 5), Value = ToS(startGridY, MudChartParent?.XYChartOptions.YAxisFormat) };
                _horizontalValues.Add(lineValue);

                startGridY += gridYUnits;
                y += verticalSpace;
            }

            //Vertical Grid Lines
            var x = horizontalStartSpace;
            double offsetX = Math.Ceiling(minX / gridXUnits) * gridXUnits;
            double startGridX = offsetX;
            for (var counter = 0; counter <= numVerticalLines; counter++)
            {
                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(x)} {ToS(verticalStartSpace)} L {ToS(x)} {ToS((boundHeight - verticalEndSpace))}"
                };
                _verticalLines.Add(line);

                var lineValue = new SvgText() { X = (x + 5), Y = (boundHeight - 10), Value = ToS(startGridX, MudChartParent?.XYChartOptions.XAxisFormat) };
                _verticalValues.Add(lineValue);

                startGridX += gridXUnits;
                x += horizontalSpace;
            }

            //Chart Scatter
            ValueTuple<string, IEnumerable<(double, double)>> XYChartSeriesToGridSeries(XYChartSeries series, double offsetY, double offsetX)
            {
                var GridValues = series.XData.Zip(
                    series.YData,
                    (xValue, yValue) => ConvertToGridValues(xValue, yValue, offsetY, offsetX)
                );
                (string name, IEnumerable<(double, double)> gridValues) gridSeries = (series.Name, GridValues);
                return gridSeries;
            }

            ValueTuple<double, double> ConvertToGridValues(double xValue, double yValue, double offsetY, double offsetX)
            {
                (double xGridValue, double yGridValue) gridValues = (
                    horizontalStartSpace + (( xValue - offsetX) * horizontalSpace / gridXUnits),
                    (boundHeight - verticalStartSpace) - ((yValue - offsetY) * verticalSpace / gridYUnits)
                );
                return gridValues;
            }

            var series = _series.Select((xyChartSeries) => XYChartSeriesToGridSeries(xyChartSeries, offsetY, offsetY));

            var colorcounter = 0;
            foreach ((string name, IEnumerable<(double xGridValue, double yGridValue)> data) in series)
            {
                foreach ((double xGridValue, double yGridValue) in data)
                {
                    var circle = new SvgCircle()
                    {
                        Index = colorcounter,
                        CX = xGridValue,
                        CY = yGridValue,
                        Radius = 5,
                        StrokeDashArray = "",
                        StrokeDashOffset = 0.0
                    };
                    _chartDots.Add(circle);
                }
                var legend = new SvgLegend()
                {
                    Index = colorcounter,
                    Labels = name
                };
                colorcounter++;
                _legends.Add(legend);
            }
        }
    }
}
