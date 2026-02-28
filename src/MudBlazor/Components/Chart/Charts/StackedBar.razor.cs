using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.Justification.StackedBars;

namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as portions of vertical rectangles.
    /// </summary>
    /// <seealso cref="Bar{T}"/>
    /// <seealso cref="Donut{T}"/>
    /// <seealso cref="Line{T}"/>
    /// <seealso cref="Pie{T}"/>
    /// <seealso cref="TimeSeries{T}"/>
    partial class StackedBar<T> : MudAxisChartBase<T, StackedBarChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        public override RenderFragment? OverlayContent { get; set; }

        private const double BarOverlapAmountFix = 0.5; // used to trigger slight overlap so the bars don't have gaps due to floating point rounding

        private readonly List<SvgPath> _bars = [];
        private double _barWidth;
        private double _barWidthStroke;
        private SvgPath? _hoveredBar;

        private const double MinBarWidth = 6;

        protected override void OnInitialized()
        {
            ChartType = ChartType.StackedBar;
            ChartOptions ??= new StackedBarChartOptions();

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

            // ensure the stacked bar width ratio is within the valid range
            ChartOptions!.BarWidthRatio = ChartOptions.BarWidthRatio.EnsureRange(0.01, 1);

            GeneratePlotArea(out var lowestHorizontalLine, out var gridYUnits, out var numHorizontalLines, out var horizontalSpace, out var verticalSpace);

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

            GenerateStackedBars(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
            GenerateLegends();
            RenderOverlay();
        }

        private void GeneratePlotArea(out int lowestHorizontalLine, out T gridYUnits, out int numHorizontalLines, out double horizontalSpace, out double verticalSpace)
        {
            SetBounds();
            ComputeStackedUnitsAndNumberOfLines(out lowestHorizontalLine, out gridYUnits, out numHorizontalLines, out var numVerticalLines);

            var horizontalLines = IsOverlayChart ? SharedData!.Value.HorizontalLineCount - 1 : numHorizontalLines;

            horizontalSpace = _boundWidth - HorizontalStartSpace - HorizontalEndSpace;
            verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, horizontalLines);

            // If this is an overlay chart, we do not generate the grid lines
            if (IsOverlayChart) return;

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, horizontalSpace);
        }

        /// <summary>
        /// Computes the grid units and the number of grid lines needed for the stacked bar chart.
        /// </summary>
        private void ComputeStackedUnitsAndNumberOfLines(out int lowestHorizontalLine, out T gridYUnits, out int numHorizontalLines, out int numVerticalLines)
        {
            var yAxisTicks = ChartOptions?.YAxisTicks;

            gridYUnits = T.CreateSaturating(yAxisTicks.HasValue && yAxisTicks.Value > 0 ? yAxisTicks.Value : 20);
            numVerticalLines = Series.Count == 0 ? 0 : Series.Max(series => series.Data.Values.Count);

            CalculateStrokeWidth(numVerticalLines);

            var (stackedPos, stackedNeg) = ComputeStackedColumnTotals(numVerticalLines);
            var (maxY, minY) = GetYAxisExtremes(stackedPos, stackedNeg);

            numHorizontalLines = StackedBar<T>.CalculateNumHorizontalLines(gridYUnits, maxY, minY, out lowestHorizontalLine);

            ClampNumHorizontalLines(ref gridYUnits, ref numHorizontalLines, ref lowestHorizontalLine, maxY, minY);
        }

        private (T[] stackedPositive, T[] stackedNegative) ComputeStackedColumnTotals(int columnCount)
        {
            var posTotals = new T[columnCount];
            var negTotals = new T[columnCount];

            for (var j = 0; j < columnCount; j++)
            {
                foreach (var seriesData in Series.Select(x => x.Data))
                {
                    if (j >= seriesData.Values.Count)
                        continue;

                    var value = seriesData[j].Y;

                    if (value < T.Zero)
                        negTotals[j] += value;
                    else
                        posTotals[j] += value;
                }
            }

            return (posTotals, negTotals);
        }

        private (T maxY, T minY) GetYAxisExtremes(T[] stackedPositiveTotals, T[] stackedNegativeTotals)
        {
            var maxY = stackedPositiveTotals.Length == 0 ? T.Zero : stackedPositiveTotals.Max();

            if (ChartOptions?.YAxisSuggestedMax is { } suggestedMax)
                maxY = T.Max(T.CreateSaturating(suggestedMax), maxY);

            var minY = stackedNegativeTotals.Length == 0 ? T.Zero : stackedNegativeTotals.Min();

            return (maxY, minY);
        }

        private static int CalculateNumHorizontalLines(T gridYUnits, T maxY, T minY, out int lowestLine)
        {
            var highestLine = Math.Max((int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits)), 0);
            lowestLine = Math.Min((int)Math.Floor(double.CreateSaturating(minY / gridYUnits)), 0);
            return highestLine - lowestLine + 1;
        }

        private void ClampNumHorizontalLines(ref T gridYUnits, ref int numLines, ref int lowestLine, T maxY, T minY)
        {
            var maxTicks = ChartOptions?.MaxNumYAxisTicks ?? 20;
            while (numLines > maxTicks)
            {
                gridYUnits *= T.CreateSaturating(2);
                numLines = CalculateNumHorizontalLines(gridYUnits, maxY, minY, out lowestLine);
            }
        }

        private void CalculateStrokeWidth(int numVerticalLines)
        {
            if (ChartOptions?.FixedBarWidth is not null)
            {
                _barWidthStroke = _barWidth = ChartOptions.FixedBarWidth.Value;
                ChartOptions!.BarWidthRatio = 1;
                return;
            }

            var barWidth = Math.Round((_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / (numVerticalLines > 1 ? numVerticalLines : 1), 1);

            _barWidthStroke = _barWidth = Math.Max(MinBarWidth, barWidth * ChartOptions!.BarWidthRatio);

            if (ChartOptions!.BarWidthRatio >= 0.9999)
            {
                // Optimisation to remove gaps between bars due to floating point rounding causing gaps to be visible between bars.
                // This givs a very slight overlap which isn't visible without purposeful inspection and zooming.
                _barWidthStroke += BarOverlapAmountFix;
            }
            else
            {
                var roundedBarWidth = Math.Round(_barWidth, 0);
                if (roundedBarWidth * numVerticalLines < (_boundWidth - HorizontalStartSpace - HorizontalEndSpace))
                {
                    _barWidthStroke = _barWidth = roundedBarWidth;
                }
            }
        }

        /// <summary>
        /// Generates the vertical grid lines and corresponding X-axis labels.
        /// </summary>
        private void GenerateVerticalGridLines(int numVerticalLines, double horizontalSpace)
        {
            VerticalLines.Clear();
            VerticalValues.Clear();

            var maxSeriesLength = Series.Count != 0 ? Series.Max(series => series.Data.Values.Count) : 0;
            var barPositions = CalculateBarGroupPositions(horizontalSpace, maxSeriesLength);

            for (var j = 0; j < numVerticalLines; j++)
            {
                var x = barPositions.Length == 0 ? 0 : barPositions[j];

                var line = new SvgPath()
                {
                    Index = j,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                VerticalLines.Add(line);

                var label = j < ChartLabels.Length ? ChartLabels[j] : "";
                var text = new SvgText()
                {
                    X = x,
                    Y = _boundHeight - XAxisLabelOffset,
                    Value = label,
                };
                VerticalValues.Add(text);
            }
        }

        /// <summary>
        /// Generates the stacked bars by drawing each segment on top of the previous one.
        /// </summary>
        private void GenerateStackedBars(int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
        {
            _bars.Clear();

            var maxSeriesLength = Series.Count != 0 ? Series.Max(series => series.Data.Values.Count) : 0;
            var barPositions = CalculateBarGroupPositions(horizontalSpace, maxSeriesLength);

            for (var dataIndex = 0; dataIndex < maxSeriesLength; dataIndex++)
            {
                var x = barPositions[dataIndex];
                var baseY = _boundHeight - VerticalStartSpace + (lowestHorizontalLine * verticalSpace);
                var positiveStack = baseY;
                var negativeStack = baseY;

                foreach (var (series, seriesIndex) in Series.Select((s, i) => (s, i)))
                {
                    if (dataIndex >= series.Data.Values.Count)
                        continue;

                    var dataValue = series.Visible ? series.Data[dataIndex].Y : T.Zero;

                    if (dataValue == T.Zero && !ChartOptions!.ShowZeroValues)
                        continue;

                    var segmentHeight = dataValue / T.CreateSaturating(gridYUnits) * T.CreateSaturating(verticalSpace);
                    var isNegative = dataValue < T.Zero;

                    var yStart = isNegative ? negativeStack : positiveStack;
                    var yEnd = yStart - double.CreateSaturating(segmentHeight);

                    _bars.Add(new SvgPath
                    {
                        Index = seriesIndex,
                        Data = $"M {ToS(x)} {ToS(yStart)} L {ToS(x)} {ToS(isNegative ? yEnd + BarOverlapAmountFix : yEnd - BarOverlapAmountFix)}",
                        LabelXValue = ChartLabels.Length > dataIndex ? ChartLabels[dataIndex] : string.Empty,
                        LabelYValue = dataValue.ToString(series.TooltipYValueFormat, null),
                        LabelX = x,
                        LabelY = isNegative ? yStart : yEnd
                    });

                    if (isNegative)
                        negativeStack = yEnd;
                    else
                        positiveStack = yEnd;
                }
            }
        }

        private double[] CalculateBarGroupPositions(double horizontalSpace, int maxColumns)
        {
            if (Series.Count == 0) return [];

            var context = new StackedBarContext
            {
                BarWidth = _barWidth,
                MaxColumns = maxColumns,
                HorizontalSpace = horizontalSpace,
                HorizontalStartSpace = HorizontalStartSpace,
                HorizontalEndSpace = HorizontalEndSpace,
                SpaceBetweenBars = CalculateSpaceWidth(horizontalSpace, maxColumns),
            };

            var strategy = StackedBarStrategyFactory.GetStrategy(ChartOptions!.Justify);

            return strategy.CalculatePositions(context);
        }

        private int CalculateSpaceWidth(double horizontalSpace, int maxColumns)
        {
            if (maxColumns <= 1) return 0;

            var spaceCount = maxColumns - 1;
            var remainingWidth = horizontalSpace - (_barWidth * maxColumns);
            var spaceWidth = remainingWidth * ChartOptions!.SeriesSpacingRatio.EnsureRange(0.0, 1.0);
            var spaceBetweenBars = spaceWidth / spaceCount;

            return (int)Math.Max(0, spaceBetweenBars);
        }

        private void OnBarMouseOver(SvgPath bar)
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
