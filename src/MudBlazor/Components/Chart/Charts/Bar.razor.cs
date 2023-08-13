﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    partial class Bar : MudChartBase
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

        private List<SvgPath> _bars = new();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            ComputeUnitsAndNumberOfLines(out double gridXUnits, out double gridYUnits, out int numHorizontalLines, out int numVerticalLines);                    

            var horizontalSpace = (BoundWidth - HorizontalStartSpace - HorizontalEndSpace) / (numVerticalLines - 1);
            var verticalSpace = (BoundHeight - VerticalStartSpace - VerticalEndSpace) / (numHorizontalLines - 1); 

            GenerateHorizontalGridLines(numHorizontalLines, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, gridXUnits, horizontalSpace);
            GenerateBars(gridYUnits, horizontalSpace, verticalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridXUnits, out double gridYUnits, out int numHorizontalLines, out int numVerticalLines)
        {
            gridXUnits = 30;

            gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;
                
            if (_series.SelectMany(series => series.Data).Any())
            {
                var maxY = _series.SelectMany(series => series.Data).Max();
                numHorizontalLines = (int)Math.Ceiling(maxY / gridYUnits) + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                int maxYTicks = MudChartParent?.ChartOptions.MaxNumYAxisTicks ?? 100;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= 2;
                    numHorizontalLines = (int)Math.Ceiling(maxY / gridYUnits) + 1;
                }    

                numVerticalLines = _series.Max(series => series.Data.Length);  
            }
            else
            {
                numHorizontalLines = 1;
                numVerticalLines = 1;
            }
        }

        private void GenerateHorizontalGridLines(int numHorizontalLines, double gridYUnits, double verticalSpace)
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

                var startGridY = i * gridYUnits;
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

        private void GenerateBars(double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            _legends.Clear();
            _bars.Clear();

            for (var i = 0; i < _series.Count; i++)
            {
                var data = _series[i].Data;  

                for (var j = 0; j < data.Length; j++)
                {
                    var gridValueX = HorizontalStartSpace + i * 10 + j * horizontalSpace;
                    var gridValueY = BoundHeight - VerticalStartSpace;
                    var dataValue = data[j] * verticalSpace / gridYUnits;
                    var gridValue = gridValueY - dataValue;

                    var bar = new SvgPath()
                    {
                        Index = i,
                        Data = $"M {ToS(gridValueX)} {ToS(gridValueY)} L {ToS(gridValueX)} {ToS(gridValue)}"
                    };
                    _bars.Add(bar);
                }

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
