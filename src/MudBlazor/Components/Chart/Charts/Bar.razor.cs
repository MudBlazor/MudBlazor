using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as rectangular bars.
    /// </summary>
    /// <seealso cref="Donut{T}"/>
    /// <seealso cref="Line{T}"/>
    /// <seealso cref="Pie{T}"/>
    /// <seealso cref="StackedBar{T}"/>
    /// <seealso cref="TimeSeries{T}"/>
    partial class Bar<T> : MudAxisChartBase<T, BarChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
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

            if (ChartReference is IMudAxisChart<T> axisChart)
            {
                axisChart.OverlayChart = this;
                axisChart.OverlayContent = this.Chart;
            }

            base.OnInitialized();
        }

        public override void RebuildChart()
        {
            // shared plot points should be initialized before generating overlay charts
            if (IsOverlayChart && SharedData is null) return;

            Series = (ChartContainer != null && ChartReference is MudChart<T>)
                ? ChartContainer.ChartSeries
                : ChartSeries;

            GeneratePlotArea(out var gridYUnits, out var lowestHorizontalLine, out var numHorizontalLines, out var numVerticalLines, out var horizontalSpace, out var verticalSpace);

            if (!IsOverlayChart)
            {
                // If this is not an overlay chart, we generate the shared plot points if an overlay exists
                SharedData = OverlayChart is IMudAxisChart<T> ? new AxisGridData<T>(lowestHorizontalLine, numHorizontalLines, gridYUnits, _boundWidth, _boundHeight) : null;
            }
            else
            {
                // If this is an overlay chart, we use the shared plot points from the main chart
                var area = SharedData!.Value;

                lowestHorizontalLine = SharedData.Value.LowestHorizontalLine;
                gridYUnits = SharedData.Value.YAxisTicks;

                _boundWidth = area.BoundWidth;
                _boundHeight = area.BoundHeight;
            }

            GenerateBars(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace, numVerticalLines);
            GenerateLegends();

            if (OverlayChart is IMudAxisChart<T> overlay)
            {
                overlay.SharedData = SharedData;
                overlay.RebuildChart();
                StateHasChanged();
            }
        }

        private void GeneratePlotArea(out T gridYUnits, out int lowestHorizontalLine, out int numHorizontalLines, out int numVerticalLines, out double horizontalSpace, out double verticalSpace)
        {
            SetBounds();
            ComputeUnitsAndNumberOfLines(out gridYUnits, out numHorizontalLines, out lowestHorizontalLine, out numVerticalLines);

            var horizontalLines = IsOverlayChart ? SharedData!.Value.HorizontalLineCount - 1 : numHorizontalLines;

            horizontalSpace = _boundWidth - HorizontalStartSpace - HorizontalEndSpace;
            verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, horizontalLines);
            var tickWidth = horizontalSpace / numVerticalLines;

            ComputeBarDimensions(tickWidth);
            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, horizontalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out T gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            var yAxisTicks = ChartOptions?.YAxisTicks;
            if (yAxisTicks.HasValue && yAxisTicks.Value > 0)
                gridYUnits = T.CreateSaturating(yAxisTicks.Value);
            else
                gridYUnits = T.CreateSaturating(20);

            var allValues = Series.SelectMany(series => series.Data.Values);

            if (allValues.Any())
            {
                var minY = allValues.Min();
                var maxY = ChartOptions?.YAxisSuggestedMax is null
                    ? allValues.Max()
                    : T.Max(T.CreateSaturating(ChartOptions.YAxisSuggestedMax.Value), allValues.Max());

                var minYDivided = minY / gridYUnits;
                var maxYDivided = maxY / gridYUnits;

                lowestHorizontalLine = Math.Min((int)Math.Floor(double.CreateSaturating(minY / gridYUnits)), 0);
                var highestHorizontalLine = Math.Max((int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits)), 0);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // Safeguard against too many gridlines
                var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 20;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= T.CreateSaturating(2);
                    minYDivided = minY / gridYUnits;
                    maxYDivided = maxY / gridYUnits;

                    lowestHorizontalLine = Math.Min((int)Math.Floor(double.CreateSaturating(minY / gridYUnits)), 0);
                    highestHorizontalLine = Math.Max((int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits)), 0);

                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }

                numVerticalLines = Series.Max(series => series.Data.Values.Count);
            }
            else
            {
                numHorizontalLines = 1;
                lowestHorizontalLine = 0;
                numVerticalLines = 1;
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

        private void GenerateBars(int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace, int numVerticalLines)
        {
            _bars.Clear();

            var barGroupPositions = CalculateBarGroupPositions(horizontalSpace, numVerticalLines);

            for (var i = 0; i < Series.Count; i++)
            {
                var series = Series[i];
                var data = series.Data;

                for (var j = 0; j < data.Values.Count && j < barGroupPositions.Length; j++)
                {
                    var dataValue = data.GetValue(j);

                    var groupStartX = barGroupPositions[j] - (_barGroupWidth / 2);
                    var gridValueX = groupStartX + (i * (_barWidth + _barGap)) + (_barWidth / 2);

                    var gridValueY = _boundHeight - VerticalStartSpace + (lowestHorizontalLine * verticalSpace);
                    var barHeight = (double.CreateSaturating((dataValue / gridYUnits)) - lowestHorizontalLine) * verticalSpace;
                    var gridValue = _boundHeight - VerticalStartSpace - double.CreateSaturating(barHeight);

                    var bar = new SvgPath()
                    {
                        Index = i,
                        Data = $"M {ToS(gridValueX)} {ToS(gridValueY)} L {ToS(gridValueX)} {ToS(gridValue)}",
                        LabelXValue = ChartLabels.Length > j ? ChartLabels[j] : string.Empty,
                        LabelYValue = dataValue.ToString(series.TooltipYValueFormat, null),
                        LabelX = gridValueX,
                        LabelY = dataValue <= T.Zero ? gridValueY : gridValue
                    };
                    _bars.Add(bar);
                }
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
        private void OnBarMouseOver(MouseEventArgs _, SvgPath bar)
        {
            _hoveredBar = bar;

            if (IsOverlayChart && ChartReference is IMudStateHasChanged chart)
                chart.StateHasChanged();
        }

        private void OnBarMouseOut()
        {
            _hoveredBar = null;

            if (IsOverlayChart && ChartReference is IMudStateHasChanged chart)
                chart.StateHasChanged();
        }
    }
}
