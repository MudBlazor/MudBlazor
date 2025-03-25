using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
    partial class StackedBar : MudCategoryAxisChartBase
    {
        private const double BarOverlapAmountFix = 0.5; // used to trigger slight overlap so the bars don't have gaps due to floating point rounding

        private List<SvgPath> _horizontalLines = [];
        private List<SvgText> _horizontalValues = [];

        private List<SvgPath> _verticalLines = [];
        private List<SvgText> _verticalValues = [];

        private List<SvgLegend> _legends = [];
        private List<ChartSeries> _series = [];

        private List<SvgPath> _bars = [];
        private double _barWidth;
        private double _barWidthStroke;
        private SvgPath? _hoveredBar;

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
            AxisChartOptions.StackedBarWidthRatio = AxisChartOptions.StackedBarWidthRatio.EnsureRange(0.1, 1);

            SetBounds();
            ComputeStackedUnitsAndNumberOfLines(out var _, out var gridYUnits, out var numHorizontalLines, out var numVerticalLines);

            // Calculate spacing – note the horizontal space is computed so that the vertical grid lines line up
            double horizontalSpace = Math.Round((_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / (numVerticalLines > 1 ? (numVerticalLines) : 1), 1);
            double verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace - AxisChartOptions.LabelExtraHeight) / (numHorizontalLines > 1 ? (numHorizontalLines) : 1);

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
            gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            // Determine the number of columns (i.e. vertical grid lines)
            numVerticalLines = _series.Any() ? _series.Max(series => series.Data.Length) : 0;

            _barWidthStroke = _barWidth = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / (numVerticalLines > 1 ? (numVerticalLines) : 1) * AxisChartOptions.StackedBarWidthRatio;

            if (AxisChartOptions.StackedBarWidthRatio >= 0.9999)
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

            // Compute the stacked total for each column
            double[] stackedTotals = new double[numVerticalLines];
            for (int j = 0; j < numVerticalLines; j++)
            {
                foreach (var series in _series)
                {
                    if (j < series.Data.Length)
                        stackedTotals[j] += series.Data[j];
                }
            }
            var maxY = stackedTotals.Any() ? stackedTotals.Max() : 0;
            numHorizontalLines = (int)(maxY / gridYUnits) + 1;
        }

        /// <summary>
        /// Generates the horizontal grid lines and corresponding value labels.
        /// </summary>
        private void GenerateHorizontalGridLines(int numHorizontalLines, double gridYUnits, double verticalSpace)
        {
            _horizontalLines.Clear();
            _horizontalValues.Clear();

            for (int i = 0; i <= numHorizontalLines; i++)
            {
                double y = VerticalStartSpace + (i * verticalSpace);
                double lineValue = i * gridYUnits;

                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(HorizontalStartSpace)} {ToS(_boundHeight - AxisChartOptions.LabelExtraHeight - y)} L {ToS(_boundWidth - HorizontalEndSpace)} {ToS(_boundHeight - AxisChartOptions.LabelExtraHeight - y)}"
                };
                _horizontalLines.Add(line);

                var text = new SvgText()
                {
                    X = HorizontalStartSpace - 10,
                    Y = _boundHeight - AxisChartOptions.LabelExtraHeight - y + 5,
                    Value = ToS(lineValue, MudChartParent?.ChartOptions.YAxisFormat)
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

            var startPadding = (_barWidth / 2) + (horizontalSpace * (1 - AxisChartOptions.StackedBarWidthRatio) / 2);

            for (int j = 0; j <= numVerticalLines; j++)
            {
                double x = HorizontalStartSpace + startPadding + (j * horizontalSpace);

                var line = new SvgPath()
                {
                    Index = j,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                _verticalLines.Add(line);

                string label = j < XAxisLabels.Length ? XAxisLabels[j] : "";
                var text = new SvgText()
                {
                    X = x,
                    Y = _boundHeight - (AxisChartOptions.LabelExtraHeight / 2) - 10,
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

            var startPadding = (_barWidth / 2) + (horizontalSpace * (1 - AxisChartOptions.StackedBarWidthRatio) / 2);

            // For each series, stack the bars in each column
            var maxSeriesLength = _series.Any() ? _series.Max(series => series.Data.Length) : 0;

            for (int j = 0; j < maxSeriesLength; j++)
            {
                double x = HorizontalStartSpace + startPadding + (j * horizontalSpace);

                var yStart = _boundHeight - VerticalStartSpace - AxisChartOptions.LabelExtraHeight;
                for (int i = 0; i < _series.Count; i++)
                {
                    var series = _series[i];
                    // Ensure the series has data for this index
                    if (j >= series.Data.Length)
                    {
                        continue;
                    }

                    double dataValue = series.Data[j];
                    double segmentHeight = (dataValue / gridYUnits) * verticalSpace;

                    double yEnd = yStart - segmentHeight;

                    var bar = new SvgPath()
                    {
                        Index = i,
                        Data = $"M {ToS(x)} {ToS(yStart)} L {ToS(x)} {ToS(yEnd - BarOverlapAmountFix)}",
                        LabelXValue = XAxisLabels.Length > j ? XAxisLabels[j] : string.Empty,
                        LabelYValue = dataValue.ToString(),
                        LabelX = x,
                        LabelY = yEnd
                    };
                    _bars.Add(bar);

                    // Update the offset for the next series at the same vertical
                    yStart = yEnd;
                }
            }
        }

        /// <summary>
        /// Generates legends for each data series.
        /// </summary>
        private void GenerateLegends()
        {
            _legends.Clear();
            for (int i = 0; i < _series.Count; i++)
            {
                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = _series[i].Name
                };
                _legends.Add(legend);
            }
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
