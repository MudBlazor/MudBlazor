using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.Chart;

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

        private const double BarStroke = 8;
        private const double BarGap = 10;
        private double BarGroupWidth => (_series.Count - 1) * BarGap + BarStroke; // number of gaps of 10 + the stroke width

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
                    X = x + (BarGroupWidth / 2),
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
                    var dataValue = data.Values[j];
                    var gridValueX = barGroupPositions[j] + (BarStroke / 2) + (i * BarGap);
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
                    Labels = series.Label
                };
                _legends.Add(legend);
            }
        }

        private double[] CalculateBarGroupPositions(double horizontalSpace, int numVerticalLines)
        {
            var numGroups = _series.Count;

            if (numGroups == 0) return [];

            var positions = new double[numVerticalLines];

            var totalGroupWidth = numGroups * BarGroupWidth;
            var groupSpacing = Math.Max(BarGroupWidth, CalculateSpaceWidth(numVerticalLines));
            var centerOffset = BarGroupWidth / 2;
            var centerX = HorizontalStartSpace + ((horizontalSpace - totalGroupWidth) / 2) + centerOffset;

            switch (ChartOptions!.Justify)
            {
                case Justify.FlexStart:
                    for (var i = 0; i < numVerticalLines; i++)
                    {
                        positions[i] = HorizontalStartSpace + centerOffset + (i * groupSpacing);
                    }
                    break;

                case Justify.FlexEnd:
                    var totalSpacing = groupSpacing * (numVerticalLines - 1);
                    var endStartX = _boundWidth - HorizontalStartSpace - HorizontalEndSpace - BarGroupWidth - totalSpacing;
                    for (var i = 0; i < numVerticalLines; i++)
                    {
                        positions[i] = endStartX + centerOffset + (i * groupSpacing);
                    }
                    break;

                case Justify.Center:
                    var halfTotalSpacing = groupSpacing * (numVerticalLines / 2);
                    var centerStartX = centerX - halfTotalSpacing;
                    for (var i = 0; i < numVerticalLines; i++)
                    {
                        positions[i] = centerStartX + (i * groupSpacing);
                    }
                    break;

                case Justify.SpaceBetween:
                    var spacing = (horizontalSpace - BarGroupWidth - HorizontalEndSpace) / (numVerticalLines - 1);
                    for (var i = 0; i < numVerticalLines; i++)
                    {
                        positions[i] = numVerticalLines == 1 ? centerX : HorizontalStartSpace + centerOffset + (i * spacing);
                    }
                    break;

                case Justify.SpaceAround:
                    var unitSpaceAround = horizontalSpace / (numVerticalLines * 2);
                    for (var i = 0; i < numVerticalLines; i++)
                    {
                        positions[i] = numVerticalLines == 1 ? centerX : HorizontalStartSpace + unitSpaceAround + (i * (unitSpaceAround * 2));
                    }
                    break;

                case Justify.SpaceEvenly:
                    var unitSpaceEvenly = horizontalSpace / (numVerticalLines + 1);
                    for (var i = 0; i < numVerticalLines; i++)
                    {
                        positions[i] = numVerticalLines == 1 ? centerX : HorizontalStartSpace + unitSpaceEvenly * (i + 1);
                    }
                    break;
            }

            return positions;
        }

        private int CalculateSpaceWidth(int groupCount)
        {
            var spaceCount = groupCount - 1;
            var remainingWidth = _boundWidth - HorizontalStartSpace - HorizontalEndSpace - (BarGroupWidth * groupCount);
            var spaceWidth = remainingWidth * ChartOptions!.SpacingRatio;
            var spaceBetweenGroups = spaceCount > 0 ? spaceWidth / spaceCount : 0;

            return (int)spaceBetweenGroups;
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
