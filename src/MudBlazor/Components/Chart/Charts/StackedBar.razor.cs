using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    partial class StackedBar : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        private List<SvgPath> _horizontalLines = new List<SvgPath>();
        private List<SvgText> _horizontalValues = new List<SvgText>();

        private List<SvgPath> _verticalLines = new List<SvgPath>();
        private List<SvgText> _verticalValues = new List<SvgText>();

        private List<SvgLegend> _legends = new List<SvgLegend>();
        private List<ChartSeries> _series = new List<ChartSeries>();

        private List<SvgPath> _bars = new List<SvgPath>();

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


            //Bars
            var colorcounter = 0;
            double barsPerSeries = 0;
            foreach (var item in _series)
            {
                double gridValueX = horizontalStartSpace + barsPerSeries;
                double gridValueY = boundHeight - verticalStartSpace;

                foreach (var dataLine in item.Data)
                {
                    var dataValue = ((double)dataLine) * verticalSpace / gridYUnits;
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

                var legend = new SvgLegend()
                {
                    Index = colorcounter,
                    Labels = item.Name
                };
                colorcounter++;
                _legends.Add(legend);

            }
            _bars = SortBarsDescending();
        }
        private List<SvgPath> SortBarsDescending()
        {
            var groupedData = _bars.GroupBy(x => double.Parse(x.Data.Split(' ')[1])).ToList();
            _bars.Clear();
            foreach (var group in groupedData.ToList()) 
            {
                var test2 = group.OrderByDescending(x => double.Parse(x.Data.Split(' ')[5])).ToList();
                var count = 1;
                var minusValue = 0;
                foreach (var bar in group.Reverse()) 
                {
                    minusValue = 0;
                    if (bar == group.First())
                        continue;
                    SvgPath data = new SvgPath();
                    data.Index = test2[count].Index;

                    if (count == 1)
                        _bars.Add(bar);

                    if (count == 1)
                        minusValue = int.Parse(test2[count].Data.Split(' ')[2]) - int.Parse(test2[count - 1].Data.Split(' ')[5]);
                    else                    
                        minusValue = int.Parse(test2[count - 1].Data.Split(' ')[2]) - int.Parse(_bars.Last().Data.Split(' ')[5]);
                                          
                    var splittedBar = test2[count].Data.Split(' ');

                    splittedBar[5] = (int.Parse(splittedBar[5]) - minusValue).ToString();

                    var replaceYValue = _bars.Last().Data.Split(' ')[5];

                    splittedBar[2] = replaceYValue;

                    foreach (var t in splittedBar)
                        if (t == "M")
                            data.Data = t;
                        else
                            data.Data = data.Data + " " + t;

                    _bars.Add(data);
                    
                    count++;
                }
                _bars.Reverse();
            }
               
            return _bars;
        }
    }
}
