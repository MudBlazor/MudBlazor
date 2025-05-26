using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        public static new ChartType ChartType => ChartType.Bar;

        public override RenderFragment? OverlayContent { get; set; }

        private readonly List<SvgPath> _bars = [];
        private SvgPath? _hoveredBar;

        private double _barGroupWidth;
        private double _barWidth;
        private double _barGap;

        private const double MinBarWidth = 6;

        protected override void OnInitialized()
        {
            ChartOptions ??= new BarChartOptions();
            base.OnInitialized();
        }

        public override void RebuildChart()
        {
            Series = (ChartContainer != null && ChartReference is MudChart)
                ? ChartContainer.ChartSeries
                : ChartSeries;

            GeneratePlotArea(out var gridYUnits, out var lowestHorizontalLine, out var numVerticalLines, out var horizontalSpace, out var verticalSpace);

            PlotArea = new PlotArea(horizontalSpace, verticalSpace, lowestHorizontalLine, numVerticalLines, gridYUnits);

            GenerateBars(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace, numVerticalLines);
        }

        private void GeneratePlotArea(out double gridYUnits, out int lowestHorizontalLine, out int numVerticalLines, out double horizontalSpace, out double verticalSpace)
        {
            SetBounds();
            ComputeUnitsAndNumberOfLines(out gridYUnits, out var numHorizontalLines, out lowestHorizontalLine, out numVerticalLines);

            horizontalSpace = _boundWidth - HorizontalStartSpace - HorizontalEndSpace;
            verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);
            var tickWidth = horizontalSpace / numVerticalLines;

            ComputeBarDimensions(tickWidth);
            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, horizontalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridYUnits = ChartOptions?.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            var allValues = Series.SelectMany(series => series.Data.Values);

            if (allValues.Any())
            {
                var minY = allValues.Min();
                var maxY = ChartOptions?.YAxisSuggestedMax is null
                    ? allValues.Max()
                    : Math.Max(ChartOptions.YAxisSuggestedMax.Value, allValues.Max());

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

                numVerticalLines = Series.Max(series => series.Data.Values.Length);
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
            HorizontalLines.Clear();
            HorizontalValues.Clear();

            for (var i = 0; i < numHorizontalLines; i++)
            {
                var y = VerticalStartSpace + (i * verticalSpace);
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(HorizontalStartSpace)} {ToS(_boundHeight - y)} L {ToS(_boundWidth - HorizontalEndSpace)} {ToS(_boundHeight - y)}"
                };
                HorizontalLines.Add(line);

                var startGridY = (lowestHorizontalLine + i) * gridYUnits;
                var lineValue = new SvgText()
                {
                    X = HorizontalStartSpace - 10,
                    Y = _boundHeight - y + 5,
                    Value = ToS(startGridY, ChartOptions?.YAxisFormat)
                };
                HorizontalValues.Add(lineValue);
            }
        }

        private void GenerateVerticalGridLines(int numVerticalLines, double horizontalSpace)
        {
            VerticalLines.Clear();
            VerticalValues.Clear();

            var spaces = Series.Count - 1;
            var leftShift = spaces switch
            {
                0 or 2 => _barWidth / 2,
                1 => 0,
                _ => _barWidth * ((spaces - 1) / 2.0)
            };

            var barGroupPositions = CalculateBarGroupPositions(horizontalSpace, numVerticalLines);

            for (var i = 0; i < numVerticalLines; i++)
            {
                var x = barGroupPositions.Length == 0 ? 0 : barGroupPositions[i];
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                VerticalLines.Add(line);

                var xLabels = i < ChartLabels.Length ? ChartLabels[i] : "";
                var lineValue = new SvgText()
                {
                    X = x + (_barGroupWidth / 2) - ((_barGap * spaces) / 2) - leftShift,
                    Y = _boundHeight - 10,
                    Value = xLabels
                };
                VerticalValues.Add(lineValue);
            }
        }

        private void GenerateBars(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace, int numVerticalLines)
        {
            Legends.Clear();
            _bars.Clear();

            var barGroupPositions = CalculateBarGroupPositions(horizontalSpace, numVerticalLines);

            for (var i = 0; i < Series.Count; i++)
            {
                var series = Series[i];
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
                        LabelXValue = ChartLabels.Length > j ? ChartLabels[j] : string.Empty,
                        LabelYValue = dataValue.ToString(series.TooltipYValueFormat),
                        LabelX = gridValueX,
                        LabelY = dataValue <= 0 ? gridValueY : gridValue
                    };
                    _bars.Add(bar);
                }

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = series.Name,
                    Visible = series.Visible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                Legends.Add(legend);
            }
        }

        private double[] CalculateBarGroupPositions(double horizontalSpace, int columnsPerDataSet)
        {
            var dataSetCount = Series.Count;

            if (dataSetCount == 0) return [];

            var positions = new double[columnsPerDataSet];
            var spaceBetweenGroups = Math.Max(dataSetCount == 1 ? 0 : _barGroupWidth, CalculateSpaceWidth(horizontalSpace, columnsPerDataSet));
            var centerOffset = _barGroupWidth / 2;
            var barWidthOffset = _barWidth / 2;
            var barGapOffset = _barGap / 2;
            var spacingOffset = spaceBetweenGroups / 2;
            var spacingRatioOffset = ChartOptions!.SeriesSpacingRatio / 2;
            var spacesPerGroup = dataSetCount - 1;
            var gapsPerDataSet = columnsPerDataSet - 1;
            var startingPoint = centerOffset;
            var availableSpace = horizontalSpace - ((_barWidth * dataSetCount * columnsPerDataSet) + (_barGap * spacesPerGroup * columnsPerDataSet));

            switch (ChartOptions.Justify)
            {
                case Justify.FlexStart:
                    startingPoint += HorizontalStartSpace;

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + i * (spaceBetweenGroups + _barWidth + barGapOffset * spacingRatioOffset);
                    }
                    break;

                case Justify.FlexEnd:
                    startingPoint = horizontalSpace + HorizontalEndSpace - (dataSetCount * _barGroupWidth + gapsPerDataSet * (spaceBetweenGroups + _barWidth)) + spacesPerGroup * barGapOffset;

                    centerOffset = dataSetCount switch
                    {
                        <= 2 => centerOffset + spacesPerGroup * barGapOffset,
                        3 => _barGroupWidth * spacesPerGroup,
                        _ => (_barGroupWidth * spacesPerGroup) + (_barWidth * (spacesPerGroup - 2) * 0.5)
                    };

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + centerOffset + i * (spaceBetweenGroups + _barWidth - ((_barGap - _barWidth) / 64));
                    }
                    break;

                case Justify.Center:
                    startingPoint = HorizontalStartSpace + (horizontalSpace - dataSetCount * _barGroupWidth) / 2;

                    centerOffset = dataSetCount switch
                    {
                        <= 2 => _barGroupWidth / 2,
                        3 => _barGroupWidth + barWidthOffset,
                        _ => ((_barGroupWidth + _barWidth) * (spacesPerGroup / 2.0)) - barWidthOffset
                    };
                    var leftShift = gapsPerDataSet * (barWidthOffset + spacingOffset);

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + centerOffset - leftShift + spacesPerGroup * barGapOffset + i * (spaceBetweenGroups + _barWidth + barGapOffset * spacingRatioOffset);
                    }
                    break;

                case Justify.SpaceBetween:
                    if (columnsPerDataSet == 1)
                    {
                        positions[0] = startingPoint + HorizontalStartSpace + (horizontalSpace - (_barWidth * dataSetCount) - (_barGap * spacesPerGroup)) / 2;
                        return positions;
                    }

                    var spaceBetween = availableSpace / Math.Max(1, gapsPerDataSet);

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = startingPoint + HorizontalStartSpace + i * ((_barWidth * dataSetCount) + (_barGap * spacesPerGroup) + spaceBetween);
                    }
                    break;

                case Justify.SpaceAround:
                    var spaceAround = horizontalSpace / (columnsPerDataSet * 2);
                    var offset = HorizontalStartSpace + spaceAround - (dataSetCount == 1 ? 0 : barWidthOffset);

                    for (var i = 0; i < columnsPerDataSet; i++)
                    {
                        positions[i] = offset + i * spaceAround * 2;
                    }
                    break;

                case Justify.SpaceEvenly:
                    var evenSpace = availableSpace / (columnsPerDataSet + 1);

                    positions[0] = startingPoint += HorizontalStartSpace + evenSpace;

                    for (var i = 1; i < columnsPerDataSet; i++)
                    {
                        positions[i] = positions[i - 1] + evenSpace + (_barWidth * dataSetCount) + (_barGap * spacesPerGroup);
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
            var seriesCount = Series.Count;

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
        private void OnBarMouseOver(MouseEventArgs _, SvgPath bar) => _hoveredBar = bar;

        private void OnBarMouseOut() => _hoveredBar = null;
    }
}
