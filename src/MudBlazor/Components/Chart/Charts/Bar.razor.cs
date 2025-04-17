using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.Chart;
using MudBlazor.Extensions;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as rectangular bars.
    /// </summary>
    /// <seealso cref="Donut"/>
    /// <seealso cref="Line"/>
    /// <seealso cref="Pie"/>
    /// <seealso cref="StackedBar"/>
    /// <seealso cref="TimeSeries"/>
    partial class Bar : MudAxisChartBase<BarChartOptions>
    {
        private readonly List<SvgPath> _horizontalLines = [];
        private readonly List<SvgText> _horizontalValues = [];

        private readonly List<SvgPath> _verticalLines = [];
        private readonly List<SvgText> _verticalValues = [];

        private readonly List<SvgLegend> _legends = [];
        private List<ChartDataSet> _series = [];

        private readonly List<SvgPath> _bars = [];
        private SvgPath? _hoveredBar;

        private double _barGroupWidth;
        private double _barWidth;
        private double _barGap;

        private const double MinBarWidth = 4;

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            RebuildChart();
        }

        protected override void RebuildChart()
        {
            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            SetBounds();
            ComputeUnitsAndNumberOfLines(out var gridYUnits, out var numHorizontalLines, out var lowestHorizontalLine, out var numVerticalLines);

            var horizontalSpace = _boundWidth - HorizontalStartSpace - HorizontalEndSpace;
            var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);
            var tickWidth = horizontalSpace / numVerticalLines;

            ComputeBarDimensions(tickWidth - HorizontalStartSpace - HorizontalEndSpace);
            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, horizontalSpace);
            GenerateBars(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace, numVerticalLines);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridYUnits = ChartOptions?.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            if (_series.SelectMany(series => series.Data.Values).Any())
            {
                var minY = _series.SelectMany(series => series.Data.Values).Min();
                var maxY = _series.SelectMany(series => series.Data.Values).Max();
                lowestHorizontalLine = Math.Min((int)Math.Floor(minY / gridYUnits), 0);
                var highestHorizontalLine = Math.Max((int)Math.Ceiling(maxY / gridYUnits), 0);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 20;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= 2;
                    lowestHorizontalLine = Math.Min((int)Math.Floor(minY / gridYUnits), 0);
                    highestHorizontalLine = Math.Max((int)Math.Ceiling(maxY / gridYUnits), 0);
                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }

                numVerticalLines = _series.Max(series => series.Data.Values.Length);
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
                var y = VerticalStartSpace + (i * verticalSpace);
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(HorizontalStartSpace)} {ToS(_boundHeight - y)} L {ToS(_boundWidth - HorizontalEndSpace)} {ToS(_boundHeight - y)}"
                };
                _horizontalLines.Add(line);

                var startGridY = (lowestHorizontalLine + i) * gridYUnits;
                var lineValue = new SvgText()
                {
                    X = HorizontalStartSpace - 10,
                    Y = _boundHeight - y + 5,
                    Value = ToS(startGridY, ChartOptions?.YAxisFormat)
                };
                _horizontalValues.Add(lineValue);
            }
        }

        private void GenerateVerticalGridLines(int numVerticalLines, double horizontalSpace)
        {
            _verticalLines.Clear();
            _verticalValues.Clear();

            var spaces = _series.Count - 1;
            var leftShift = spaces switch
            {
                0 or 2 => _barWidth / 2,
                1 => 0,
                _ => _barWidth * ((spaces - 1) / 2.0)
            };

            var barGroupPositions = CalculateBarGroupPositions(horizontalSpace, numVerticalLines);

            for (var i = 0; i < numVerticalLines; i++)
            {
                var x = barGroupPositions[i];
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                _verticalLines.Add(line);

                var xLabels = i < ChartOptions!.XAxisLabels.Length ? ChartOptions!.XAxisLabels[i] : "";
                var lineValue = new SvgText()
                {
                    X = x + (_barGroupWidth / 2) - ((_barGap * spaces) / 2) - leftShift,
                    Y = _boundHeight - 10,
                    Value = xLabels
                };
                _verticalValues.Add(lineValue);
            }
        }

        private void GenerateBars(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace, int numVerticalLines)
        {
            _legends.Clear();
            _bars.Clear();

            var barGroupPositions = CalculateBarGroupPositions(horizontalSpace, numVerticalLines);

            for (var i = 0; i < _series.Count; i++)
            {
                var series = _series[i];
                var data = series.Data;

                for (var j = 0; j < data.Values.Length && j < barGroupPositions.Length; j++)
                {
                    var dataValue = data[j];

                    var groupStartX = barGroupPositions[j] - (_barGroupWidth / 2);
                    var gridValueX = groupStartX + (i * (_barWidth + _barGap)) + (_barWidth / 2);

                    var gridValueY = _boundHeight - VerticalStartSpace + (lowestHorizontalLine * verticalSpace);
                    var barHeight = ((dataValue / gridYUnits) - lowestHorizontalLine) * verticalSpace;
                    var gridValue = _boundHeight - VerticalStartSpace - barHeight;

                    var bar = new SvgPath()
                    {
                        Index = i,
                        Data = $"M {ToS(gridValueX)} {ToS(gridValueY)} L {ToS(gridValueX)} {ToS(gridValue)}",
                        LabelXValue = ChartOptions!.XAxisLabels.Length > j ? ChartOptions!.XAxisLabels[j] : string.Empty,
                        LabelYValue = dataValue.ToString(series.TooltipYValueFormat),
                        LabelX = gridValueX,
                        LabelY = gridValue
                    };
                    _bars.Add(bar);
                }

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = series.Label,
                    Visible = series.Visible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                _legends.Add(legend);
            }
        }

        private void HandleLegendVisibilityChanged(SvgLegend legend)
        {
            var series = _series[legend.Index];
            series.Visible = legend.Visible;
            RebuildChart();
        }

        private double[] CalculateBarGroupPositions(double horizontalSpace, int columnsPerDataSet)
        {
            var dataSetCount = _series.Count;

            if (dataSetCount == 0) return [];

            var positions = new double[columnsPerDataSet];
            var spaceBetweenGroups = Math.Max(dataSetCount == 1 ? 0 : _barGroupWidth, CalculateSpaceWidth(horizontalSpace, columnsPerDataSet));
            var centerOffset = _barGroupWidth / 2;
            var barGapOffset = _barGap / 2;
            var spacingOffset = spaceBetweenGroups / 2;
            var spacingRatioOffset = ChartOptions!.SeriesSpacingRatio / 2;
            var totalSpaces = dataSetCount - 1;
            var gapsPerDataSet = columnsPerDataSet - 1;
            var startingPoint = centerOffset;

            switch (ChartOptions.Justify)
            {
                case Justify.FlexStart:
                    startingPoint += HorizontalStartSpace + HorizontalEndSpace;

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + i * (spaceBetweenGroups + _barWidth + barGapOffset * spacingRatioOffset);
                    }
                    break;

                case Justify.FlexEnd:
                    startingPoint = horizontalSpace - (dataSetCount * _barGroupWidth + gapsPerDataSet * (spaceBetweenGroups + _barWidth)) + totalSpaces * barGapOffset;

                    centerOffset = dataSetCount switch
                    {
                        <= 2 => centerOffset + totalSpaces * barGapOffset,
                        3 => _barGroupWidth * totalSpaces,
                        _ => (_barGroupWidth * totalSpaces) + (_barWidth * (totalSpaces - 2) * 0.5)
                    };

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + centerOffset + i * (spaceBetweenGroups + _barWidth - ((_barGap - _barWidth) / 64));
                    }
                    break;

                case Justify.Center:
                    var barWidthOffset = _barWidth / 2;

                    startingPoint = HorizontalStartSpace + (horizontalSpace - dataSetCount * _barGroupWidth) / 2;

                    centerOffset = dataSetCount switch
                    {
                        <= 2 => _barGroupWidth / 2,
                        3 => _barGroupWidth + barWidthOffset,
                        _ => ((_barGroupWidth + _barWidth) * (totalSpaces / 2.0)) - barWidthOffset
                    };
                    var leftShift = gapsPerDataSet * (barWidthOffset + spacingOffset);

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + centerOffset - leftShift + totalSpaces * barGapOffset + i * (spaceBetweenGroups + _barWidth + barGapOffset * spacingRatioOffset);
                    }
                    break;

                case Justify.SpaceBetween:
                    if (dataSetCount == 1 && columnsPerDataSet == 1)
                    {
                        positions[0] = HorizontalStartSpace + centerOffset + (horizontalSpace - _barGroupWidth) / 2;
                        return positions;
                    }

                    var spacing = (horizontalSpace - _barGroupWidth) / gapsPerDataSet - (dataSetCount == 1 ? 0 : (_barWidth / gapsPerDataSet));

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + HorizontalStartSpace + i * spacing;
                    }
                    break;

                case Justify.SpaceAround:
                    var spaceAround = horizontalSpace / (columnsPerDataSet * 2);

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = HorizontalStartSpace + spaceAround + i * (spaceAround * 2);
                    }
                    break;

                case Justify.SpaceEvenly:
                    var evenSpace = horizontalSpace / (columnsPerDataSet + 1);

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = HorizontalStartSpace + evenSpace * (i + 1);
                    }
                    break;
            }

            return positions;
        }

        private int CalculateSpaceWidth(double horizontalSpace, int groupCount)
        {
            if (groupCount <= 1) return 0;

            var spaceCount = groupCount - 1;
            var remainingWidth = horizontalSpace - HorizontalStartSpace - HorizontalEndSpace - ((_barGroupWidth + (_barWidth / 2)) * groupCount);
            var spaceWidth = remainingWidth * ChartOptions!.SeriesSpacingRatio.EnsureRange(0.01, 1.0);
            var spaceBetweenGroups = spaceCount > 0 ? spaceWidth / spaceCount : 0;

            return (int)Math.Max(0, spaceBetweenGroups);
        }

        private void ComputeBarDimensions(double tickWidth)
        {
            var seriesCount = _series.Count;

            var fixedWidth = ChartOptions?.FixedBarWidth;

            if (fixedWidth.HasValue)
            {
                _barWidth = fixedWidth.Value;
                _barGap = _barWidth * 0.25;
                _barGroupWidth = (seriesCount * _barWidth) + ((seriesCount - 1) * _barGap);
                return;
            }

            var groupWidthRatio = ChartOptions!.BarWidthRatio.EnsureRange(0.01, 1.0);
            var totalGapRatio = seriesCount > 1 ? ChartOptions!.BarSpacingRatio * (seriesCount - 1) : 1;
            var barWidthRelative = 1.0 / (seriesCount + totalGapRatio);
            var groupWidthRelative = tickWidth * groupWidthRatio;

            _barWidth = Math.Max(MinBarWidth, groupWidthRelative * barWidthRelative);
            _barGap = seriesCount > 1 ? groupWidthRelative * barWidthRelative * ChartOptions!.BarSpacingRatio : 0;
            _barGroupWidth = Math.Max(MinBarWidth * seriesCount - 2, groupWidthRelative - _barWidth);
        }


        private void OnBarMouseOver(MouseEventArgs _, SvgPath bar)
        {
            _hoveredBar = bar;
        }

        private void OnBarMouseOut(MouseEventArgs _)
        {
            _hoveredBar = null;
        }
    }
}
