using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.Chart;
using MudBlazor.Extensions;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as portions of vertical rectangles.
    /// </summary>
    /// <seealso cref="Bar"/>
    /// <seealso cref="Donut"/>
    /// <seealso cref="Line"/>
    /// <seealso cref="Pie"/>
    /// <seealso cref="TimeSeries"/>
    partial class StackedBar : MudAxisChartBase<StackedBarChartOptions>
    {
        private const double BarOverlapAmountFix = 0.5; // used to trigger slight overlap so the bars don't have gaps due to floating point rounding

        private readonly List<SvgPath> _horizontalLines = [];
        private readonly List<SvgText> _horizontalValues = [];

        private readonly List<SvgPath> _verticalLines = [];
        private readonly List<SvgText> _verticalValues = [];

        private readonly List<SvgLegend> _legends = [];
        private List<ChartDataSet> _series = [];

        private readonly List<SvgPath> _bars = [];
        private double _barWidth;
        private double _barWidthStroke;
        private SvgPath? _hoveredBar;

        private const double MinBarWidth = 6;

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

            // ensure the stacked bar width ratio is within the valid range
            ChartOptions!.BarWidthRatio = ChartOptions.BarWidthRatio.EnsureRange(0.01, 1);

            SetBounds();
            ComputeStackedUnitsAndNumberOfLines(out var _, out var gridYUnits, out var numHorizontalLines, out var numVerticalLines);

            // Calculate spacing – note the horizontal space is computed so that the vertical grid lines line up
            var horizontalSpace = _boundWidth - HorizontalStartSpace - HorizontalEndSpace;
            var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / (numHorizontalLines > 1 ? (numHorizontalLines) : 1);

            GenerateHorizontalGridLines(numHorizontalLines, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, horizontalSpace);
            GenerateStackedBars(gridYUnits, horizontalSpace, verticalSpace);
            GenerateLegends();
        }

        /// <summary>
        /// Computes the grid units and the number of grid lines needed for the stacked bar chart.
        /// </summary>
        private void ComputeStackedUnitsAndNumberOfLines(
            out double gridXUnits,
            out double gridYUnits,
            out int numHorizontalLines,
            out int numVerticalLines)
        {
            gridXUnits = 30;
            gridYUnits = ChartOptions?.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            // Determine the number of columns (i.e. vertical grid lines)
            numVerticalLines = _series.Count != 0 ? _series.Max(series => series.Data.Values.Length) : 0;

            CalculateStrokeWidth(numVerticalLines);

            // Compute the stacked total for each column
            var stackedTotals = new double[numVerticalLines];
            for (var j = 0; j < numVerticalLines; j++)
            {
                foreach (var series in _series)
                {
                    if (j < series.Data.Values.Length)
                        stackedTotals[j] += series.Data[j];
                }
            }
            var maxY = stackedTotals.Length != 0 ? stackedTotals.Max() : 0;
            numHorizontalLines = (int)(maxY / gridYUnits) + 1;

            // this is a safeguard against millions of gridlines which might arise with very high values
            var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 20;
            while (numHorizontalLines > maxYTicks)
            {
                gridYUnits *= 2;
                var lowestHorizontalLine = Math.Min((int)Math.Floor(0 / gridYUnits), 0);
                var highestHorizontalLine = Math.Max((int)Math.Ceiling(maxY / gridYUnits), 0);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
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

            var barWidth = Math.Round((_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / (numVerticalLines > 1 ? (numVerticalLines) : 1), 1);

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
        /// Generates the horizontal grid lines and corresponding value labels.
        /// </summary>
        private void GenerateHorizontalGridLines(int numHorizontalLines, double gridYUnits, double verticalSpace)
        {
            _horizontalLines.Clear();
            _horizontalValues.Clear();

            for (var i = 0; i <= numHorizontalLines; i++)
            {
                var y = VerticalStartSpace + (i * verticalSpace);
                var lineValue = i * gridYUnits;

                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(HorizontalStartSpace)} {ToS(_boundHeight - y)} L {ToS(_boundWidth - HorizontalEndSpace)} {ToS(_boundHeight - y)}"
                };
                _horizontalLines.Add(line);

                var text = new SvgText()
                {
                    X = HorizontalStartSpace - 10,
                    Y = _boundHeight - y + 5,
                    Value = ToS(lineValue, ChartOptions?.YAxisFormat)
                };
                _horizontalValues.Add(text);
            }
        }

        /// <summary>
        /// Generates the vertical grid lines and corresponding X-axis labels.
        /// </summary>
        private void GenerateVerticalGridLines(int numVerticalLines, double horizontalSpace)
        {
            _verticalLines.Clear();
            _verticalValues.Clear();

            var startPadding = (_barWidth / 2) + (horizontalSpace * (1 - ChartOptions!.BarWidthRatio) / 2);
            var maxSeriesLength = _series.Count != 0 ? _series.Max(series => series.Data.Values.Length) : 0;
            var barPositions = CalculateBarGroupPositions(horizontalSpace, maxSeriesLength);

            for (var j = 0; j < numVerticalLines; j++)
            {
                var x = barPositions[j];

                var line = new SvgPath()
                {
                    Index = j,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                _verticalLines.Add(line);

                var label = j < ChartLabels.Length ? ChartLabels[j] : "";
                var text = new SvgText()
                {
                    X = x,
                    Y = _boundHeight - XAxisLabelOffset,
                    Value = label,
                };
                _verticalValues.Add(text);
            }
        }

        /// <summary>
        /// Generates the stacked bars by drawing each segment on top of the previous one.
        /// </summary>
        private void GenerateStackedBars(double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            _bars.Clear();

            var startPadding = (_barWidth / 2) + (horizontalSpace * (1 - ChartOptions!.BarWidthRatio) / 2);

            // For each series, stack the bars in each column
            var maxSeriesLength = _series.Count != 0 ? _series.Max(series => series.Data.Values.Length) : 0;
            var barPositions = CalculateBarGroupPositions(horizontalSpace, maxSeriesLength);

            for (var j = 0; j < maxSeriesLength; j++)
            {
                var x = barPositions[j];

                var yStart = _boundHeight - VerticalStartSpace;
                for (var i = 0; i < _series.Count; i++)
                {
                    var series = _series[i];
                    // Ensure the series has data for this index
                    if (j >= series.Data.Values.Length)
                    {
                        continue;
                    }

                    var dataValue = series.Visible ? series.Data[j] : 0;
                    var segmentHeight = (dataValue / gridYUnits) * verticalSpace;

                    var yEnd = yStart - segmentHeight;

                    var bar = new SvgPath()
                    {
                        Index = i,
                        Data = $"M {ToS(x)} {ToS(yStart)} L {ToS(x)} {ToS(yEnd - BarOverlapAmountFix)}",
                        LabelXValue = ChartLabels.Length > j ? ChartLabels[j] : string.Empty,
                        LabelYValue = dataValue.ToString(series.TooltipYValueFormat),
                        LabelX = x,
                        LabelY = yEnd
                    };
                    _bars.Add(bar);

                    // Update the offset for the next series at the same vertical
                    yStart = yEnd;
                }
            }
        }

        private double[] CalculateBarGroupPositions(double horizontalSpace, int maxColumns)
        {
            if (_series.Count == 0) return [];

            var positions = new double[maxColumns];
            var spaceBetweenBars = CalculateSpaceWidth(horizontalSpace, maxColumns);
            var centerOffset = _barWidth / 2;
            var totalSpaces = maxColumns - 1;
            var startingPoint = centerOffset;

            switch (ChartOptions!.Justify)
            {
                case Justify.FlexStart:
                    startingPoint += HorizontalStartSpace;

                    for (var i = 0; i < maxColumns; i++)
                    {
                        positions[i] = startingPoint + i * (spaceBetweenBars + _barWidth);
                    }
                    break;

                case Justify.FlexEnd:
                    startingPoint += horizontalSpace + HorizontalEndSpace - ((maxColumns * _barWidth) + (spaceBetweenBars * totalSpaces));

                    for (var i = 0; i < maxColumns; i++)
                    {
                        positions[i] = startingPoint + i * (spaceBetweenBars + _barWidth);
                    }
                    break;

                case Justify.Center:
                    startingPoint += HorizontalStartSpace + (horizontalSpace - (maxColumns * _barWidth) - (spaceBetweenBars * totalSpaces)) / 2;

                    for (var i = 0; i < maxColumns; i++)
                    {
                        positions[i] = startingPoint + i * (spaceBetweenBars + _barWidth);
                    }
                    break;

                case Justify.SpaceBetween:
                    if (maxColumns == 1)
                    {
                        positions[0] = HorizontalStartSpace + centerOffset + (horizontalSpace - _barWidth) / 2;
                        return positions;
                    }

                    var totalBarWidth = maxColumns * _barWidth;
                    var spaceBetween = (horizontalSpace - totalBarWidth) / (maxColumns - 1);

                    for (var i = 0; i < maxColumns; i++)
                    {
                        positions[i] = startingPoint + HorizontalStartSpace + i * (_barWidth + spaceBetween);
                    }
                    break;

                case Justify.SpaceAround:
                    var spaceAround = horizontalSpace / (maxColumns * 2);

                    for (var i = 0; i < maxColumns; i++)
                    {
                        positions[i] = HorizontalStartSpace + spaceAround + i * (spaceAround * 2);
                    }
                    break;

                case Justify.SpaceEvenly:
                    var contentSpace = maxColumns * _barWidth;
                    var remainingSpace = horizontalSpace - contentSpace;
                    var evenSpace = remainingSpace / (maxColumns + 1);
                    
                    positions[0] = startingPoint += HorizontalStartSpace + evenSpace;

                    for (var i = 1; i < maxColumns; i++)
                    {
                        positions[i] = positions[i - 1] + _barWidth + evenSpace;
                    }
                    break;
            }

            return positions;
        }

        private int CalculateSpaceWidth(double horizontalSpace, int maxColumns)
        {
            if (maxColumns <= 1) return 0;

            var spaceCount = maxColumns - 1;
            var remainingWidth = horizontalSpace - HorizontalStartSpace - (_barWidth * maxColumns);
            var spaceWidth = remainingWidth * ChartOptions!.SeriesSpacingRatio.EnsureRange(0.0, 1.0);
            var spaceBetweenBars = spaceCount > 0 ? spaceWidth / spaceCount : 0;

            return (int)Math.Max(0, spaceBetweenBars);
        }

        /// <summary>
        /// Generates legends for each data series.
        /// </summary>
        private void GenerateLegends()
        {
            _legends.Clear();
            for (var i = 0; i < _series.Count; i++)
            {
                var series = _series[i];
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
