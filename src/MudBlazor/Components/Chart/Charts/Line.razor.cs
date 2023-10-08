using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;
using MudBlazor.Components.Chart;
using MudBlazor.Components.Chart.Interpolation;

namespace MudBlazor.Charts
{
    partial class Line : MudChartBase
    {
        private const double BoundWidth = 650.0;
        private const double BoundHeight = 350.0;
        private const double HorizontalStartSpace = 30.0;
        private const double HorizontalEndSpace = 30.0;
        private const double VerticalStartSpace = 25.0;
        private const double VerticalEndSpace = 25.0;

        [CascadingParameter] public MudChart MudChartParent { get; set; }

        private List<SvgPath> _horizontalLines = new();
        private List<SvgText> _horizontalValues = new();

        private List<SvgPath> _verticalLines = new();
        private List<SvgText> _verticalValues = new();

        private List<SvgLegend> _legends = new();
        private List<ChartSeries> _series = new();

        private List<SvgPath> _chartLines = new();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries; 
            
            ComputeUnitsAndNumberOfLines(out double gridXUnits, out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines);                    

            var horizontalSpace = (BoundWidth - HorizontalStartSpace - HorizontalEndSpace) / (numVerticalLines - 1);
            var verticalSpace = (BoundHeight - VerticalStartSpace - VerticalEndSpace) / (numHorizontalLines - 1); 

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, gridXUnits, horizontalSpace);
            GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridXUnits, out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridXUnits = 30;

            gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;
                
            if (_series.SelectMany(series => series.Data).Any())
            {
                var minY = _series.SelectMany(series => series.Data).Min();
                var maxY = _series.SelectMany(series => series.Data).Max();
                lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                var highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);                
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                int maxYTicks = MudChartParent?.ChartOptions.MaxNumYAxisTicks ?? 100;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= 2;
                    lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                    highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }    

                numVerticalLines = _series.Max(series => series.Data.Length);  
            }
            else
            {
                numHorizontalLines = 1;
                lowestHorizontalLine = 0;
                numVerticalLines = 1;
            }
        }

        private void GenerateHorizontalGridLines(int numHorizontalLines, int lowestHorizontalLine, double gridYUnits, double verticalSpace)
        {
            _horizontalLines.Clear();
            _horizontalValues.Clear();

            for (var i = 0; i < numHorizontalLines; i++)
            {
                var y = VerticalStartSpace + i * verticalSpace;                
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(HorizontalStartSpace)} {ToS((BoundHeight - y))} L {ToS((BoundWidth - HorizontalEndSpace))} {ToS((BoundHeight - y))}"
                };
                _horizontalLines.Add(line);

                var startGridY = (lowestHorizontalLine + i) * gridYUnits;
                var lineValue = new SvgText() 
                { 
                    X = HorizontalStartSpace - 10, 
                    Y = BoundHeight - y + 5, 
                    Value = ToS(startGridY, MudChartParent?.ChartOptions.YAxisFormat) 
                };
                _horizontalValues.Add(lineValue);
            }
        }

        private void GenerateVerticalGridLines(int numVerticalLines, double gridXUnits, double horizontalSpace)
        {
            _verticalLines.Clear();
            _verticalValues.Clear();

            for (var i = 0; i < numVerticalLines; i++)
            {
                var x = HorizontalStartSpace + i * horizontalSpace;
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(x)} {ToS((BoundHeight - VerticalStartSpace))} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                _verticalLines.Add(line);

                var xLabels = i < XAxisLabels.Length ? XAxisLabels[i] : "";
                var lineValue = new SvgText()
                { 
                    X = x, 
                    Y = BoundHeight - 2, 
                    Value = xLabels
                };
                _verticalValues.Add(lineValue);
            }
        }

        private void GenerateChartLines(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            _legends.Clear();
            _chartLines.Clear();

            for (var i = 0; i < _series.Count; i++)
            {                
                StringBuilder chartLine = new StringBuilder();
                            
                var data = _series[i].Data;   
                
                (double x, double y) GetXYForDataPoint(int index)
                {
                    var x = HorizontalStartSpace + index * horizontalSpace;
                    var gridValue = (data[index] / gridYUnits - lowestHorizontalLine) * verticalSpace;
                    var y = BoundHeight - VerticalStartSpace - gridValue;
                    return (x, y);
                }             

                bool interpolationEnabled = MudChartParent != null && MudChartParent.ChartOptions.InterpolationOption != InterpolationOption.Straight;
                if (interpolationEnabled)
                {
                    double[] XValues = new double[data.Length];
                    double[] YValues = new double[data.Length];                
                    for (var j = 0; j < data.Length; j++)
                        (XValues[j], YValues[j]) = GetXYForDataPoint(j);

                    ILineInterpolator interpolator = MudChartParent?.ChartOptions.InterpolationOption switch {
                        InterpolationOption.NaturalSpline => new NaturalSpline(XValues, YValues),
                        InterpolationOption.EndSlope      => new EndSlopeSpline(XValues, YValues),
                        InterpolationOption.Periodic      => new PeriodicSpline(XValues, YValues),
                        _                                 => throw new NotImplementedException("Interpolation option not implemented yet")
                    };

                    horizontalSpace = (BoundWidth - HorizontalStartSpace - HorizontalEndSpace) / interpolator.InterpolatedXs.Length;

                    for (var j = 0; j < interpolator.InterpolatedYs.Length; j++)
                    {
                        if (j == 0)
                            chartLine.Append("M ");
                        else
                            chartLine.Append(" L ");

                        var x = HorizontalStartSpace + j * horizontalSpace;
                        var y = interpolator.InterpolatedYs[j];                        
                        chartLine.Append(ToS(x));
                        chartLine.Append(' ');
                        chartLine.Append(ToS(y));
                    }
                }
                else
                {
                    for (var j = 0; j < data.Length; j++)
                    {
                        if (j == 0)
                            chartLine.Append("M ");
                        else
                            chartLine.Append(" L ");

                        var (x, y) = GetXYForDataPoint(j);
                        chartLine.Append(ToS(x));
                        chartLine.Append(' ');
                        chartLine.Append(ToS(y));
                    }
                }

                var line = new SvgPath()
                {
                    Index = i,
                    Data = chartLine.ToString()
                };
                _chartLines.Add(line);

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = _series[i].Name
                };
                _legends.Add(legend);
            }
        }
    }
}
