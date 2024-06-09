using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as portions of vertical rectangles.
    /// </summary>
    partial class StackedBar : MudCategoryChartBase
    {
        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart MudChartParent { get; set; }

        private List<SvgPath> _horizontalLines = new();
        private List<SvgText> _horizontalValues = new();

        private List<SvgPath> _verticalLines = new();
        private List<SvgText> _verticalValues = new();

        private List<SvgLegend> _legends = new();
        private List<ChartSeries> _series = new();

        private List<SvgPath> _bars = new();

        /// <inheritdoc />
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

            var maxY = 0.0;
            var numXLabels = XAxisLabels.Length;
            var numValues = _series.Any() ? _series.Max(x => x.Data.Length) : 0;
            var barTopValues = new double[numValues];
            foreach (var item in _series)
            {
                var dataNumber = 0;
                foreach (int i in item.Data)
                {
                    barTopValues[dataNumber] += i;
                    dataNumber++;
                }
            }
            maxY = barTopValues.Any() ? barTopValues.Max() : 0;

            var boundHeight = 350.0;
            var boundWidth = 650.0;

            double gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            double gridXUnits = 30;

            var numVerticalLines = numValues;

            var numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;

            var verticalStartSpace = 25.0;
            var horizontalStartSpace = 35.0;
            var verticalEndSpace = 25.0;
            var horizontalEndSpace = 30.0;

            var verticalSpace = (boundHeight - verticalStartSpace - verticalEndSpace) / numHorizontalLines;
            var horizontalSpace = (boundWidth - horizontalStartSpace - horizontalEndSpace) / numVerticalLines;

            //Horizontal Grid Lines
            var y = verticalStartSpace;
            double startGridY = 0;
            for (var counter = 0; counter <= numHorizontalLines; counter++)
            {
                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(horizontalStartSpace)} {ToS(boundHeight - y)} L {ToS(boundWidth - horizontalEndSpace)} {ToS(boundHeight - y)}"
                };
                _horizontalLines.Add(line);

                var lineValue = new SvgText() { X = horizontalStartSpace, Y = (boundHeight - y + 5), Value = ToS(startGridY, MudChartParent?.ChartOptions.YAxisFormat) };
                _horizontalValues.Add(lineValue);

                startGridY += gridYUnits;
                y += verticalSpace;
            }

            //Vertical Grid Lines
            var x = horizontalStartSpace + 24;
            double startGridX = 0;
            for (var counter = 0; counter <= numVerticalLines; counter++)
            {

                var line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {ToS(x)} {ToS(boundHeight - verticalStartSpace)} L {ToS(x)} {ToS(verticalEndSpace)}"
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
            double barsPerSeries = 0;
            double[] barValuesOffset = null;
            foreach (var item in _series)
            {
                var gridValueX = horizontalStartSpace + 24;

                if (barValuesOffset == null)
                {
                    barValuesOffset = new double[item.Data.Length];
                    for (var i = 0; i < item.Data.Length; i++)
                    {
                        barValuesOffset[i] = boundHeight - verticalStartSpace;
                    }
                }

                var dataNumber = 0;
                foreach (var dataLine in item.Data)
                {
                    var dataValue = dataLine * verticalSpace / gridYUnits;
                    var dataGridValueY = barValuesOffset[dataNumber];
                    var dataGridValue = dataGridValueY - dataValue;
                    var bar = $"M {ToS(gridValueX)} {ToS(dataGridValueY)} L {ToS(gridValueX)} {ToS(dataGridValue)}";

                    gridValueX += horizontalSpace;
                    barValuesOffset[dataNumber] = dataGridValue;

                    var line = new SvgPath()
                    {
                        Index = colorcounter,
                        Data = bar
                    };
                    _bars.Add(line);
                    dataNumber++;
                }

                barsPerSeries += 10;

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
