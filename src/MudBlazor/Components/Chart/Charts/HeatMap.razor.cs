// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor.Charts
{
    partial class HeatMap : MudCategoryChartBase
    {
        private const double BoundWidth = 650.0;

        private const double BoundHeight = 350.0;

        private Position _legendPosition = Position.Bottom;

        // the minimum size a cell can shrink to (height and width)
        private const int CellMinSize = 8;

        // the width and height of a legend color box
        private const double LegendBox = 18;

        // the minimum padding between cells and line length on legend labels
        private const int CellPadding = 5;

        // the line length on legend labels
        private const int LegendLineLength = 5;

        // the heatmap outside padding
        private const int HeatMapPadding = 15;

        private const double AverageCharWidthMultiplier = 0.6;

        private const int LegendFontSize = 10;

        private double _dynamicFontSize = 8;

        private double _yAxisLabelWidth = 0;

        // padding or legend area for each side of the heatmap
        private double _horizontalStartSpace = HeatMapPadding;

        private double _horizontalEndSpace = HeatMapPadding;

        private double _verticalStartSpace = HeatMapPadding;

        private double _verticalEndSpace = HeatMapPadding;

        // the minimum value in all series
        private double _minValue = 0.0;

        // the maximum value in all series
        private double _maxValue = 1.0;

        private string[] _colorPalette = ["#587934"];

        // The maximum number of cells in a series
        private int SeriesLength => _series.Any() ? _series.Where(s => s.Data != null).Max(s => s.Data.Length) : 0;

        // The number of rows visible
        private int RowCount => _series.Any() ? _series.Count(s => s.Visible) : 0;

        // the amount of pixels a legend extends horizontally when it's on left/right
        private int _legendLabelsYAxis = 0;

        // the amount of pixels a legend extends vertically when it's on the top/bottom
        private int _legendLabelsXAxis = 0;

        // Calculate the actual width of the heatmap cells area
        private double HeatmapWidth => BoundWidth - _horizontalStartSpace - _horizontalEndSpace;

        // Calculate the actual height of the heatmap cells area
        private double HeatmapHeight => BoundHeight - _verticalStartSpace - _verticalEndSpace;

        private ChartOptions? _options;

        private List<ChartSeries> _series = [];

        private List<(double value, string color)> _legends = [];

        private List<HeatMapCell> _heatMapCells = [];

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudChartParent != null)
            {
                if (_options == null || _options != MudChartParent.ChartOptions)
                {
                    _options = MudChartParent.ChartOptions;
                    _colorPalette = _options.ChartPalette.Any() ? _options.ChartPalette : _colorPalette;
                    _legendPosition = MudChartParent.LegendPosition switch
                    {
                        Position.Center => Position.Bottom,
                        Position.Start => Position.Left,
                        Position.End => Position.Right,
                        _ => MudChartParent.LegendPosition
                    };
                }
                if (_series.Count == 0 ||
                    (MudChartParent.ChartSeries.Count > 0 &&
                    _series != MudChartParent.ChartSeries))
                {
                    _series.Clear();
                    _series = MudChartParent.ChartSeries;
                }
            }

            InitializeHeatmap();
        }

        private void InitializeHeatmap()
        {
            // Populate _heatmapCells based on data, e.g., matrix of values
            _heatMapCells.Clear();
            _minValue = 0;
            _maxValue = 1;

            // # of rows
            var rows = _series.Count;
            // cols should be the max number of data[] in all series
            var cols = SeriesLength;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var value = GetDataValue(row, col); // Method to retrieve the value for each cell
                    _heatMapCells.Add(new HeatMapCell
                    {
                        Row = row,
                        Column = col,
                        Value = value,
                    });
                    if (value != null)
                    {
                        _minValue = Math.Min(_minValue, value.Value);
                        _maxValue = Math.Max(_maxValue, value.Value);
                    }
                }
            }
            CalculateAreas();
            BuildLegends();
        }

        private void CalculateAreas()
        {
            // Defaults each side gets some space around the heatmap
            _verticalStartSpace = _verticalEndSpace = _horizontalStartSpace = _horizontalEndSpace = HeatMapPadding;
            var estimatedCellWidth = Math.Max(CellMinSize, (BoundWidth - (6 * HeatMapPadding) - CellPadding) / Math.Max(1, SeriesLength));
            var estimatedCellHeight = (BoundHeight - (6 * HeatMapPadding)) / Math.Max(1, RowCount);
            _dynamicFontSize = CalculateFontSize(estimatedCellWidth, estimatedCellHeight, 8) - 2;

            // Calculate Y-axis label width based on dynamic font size
            _yAxisLabelWidth = (_series.Any() ? _series?.Max(x => x.Name.Length) ?? 1 : 1) * _dynamicFontSize * AverageCharWidthMultiplier;

            var defaultCharsWidth = 5 * LegendFontSize * AverageCharWidthMultiplier;
            _legendLabelsYAxis = (int)Math.Ceiling(_options is { ShowLegendLabels: true }
                ? (defaultCharsWidth + LegendLineLength) : 0);
            _legendLabelsXAxis = _options is { ShowLegendLabels: true }
                ? (LegendFontSize + LegendLineLength)
                : 0;

            // make room for X and Y Axis Labels
            if (_options?.YAxisLabelPosition == YAxisLabelPosition.Left)
            {
                _horizontalStartSpace += CellPadding + _yAxisLabelWidth + CellPadding;
            }
            if (_options?.YAxisLabelPosition == YAxisLabelPosition.Right)
            {
                _horizontalEndSpace += CellPadding + _yAxisLabelWidth + CellPadding;
            }
            if (_options?.XAxisLabelPosition == XAxisLabelPosition.Top)
            {
                _verticalStartSpace += CellPadding + _dynamicFontSize + CellPadding;
            }
            if (_options?.XAxisLabelPosition == XAxisLabelPosition.Bottom)
            {
                _verticalEndSpace += CellPadding + _dynamicFontSize + CellPadding;
            }
            // Make Room for Legend (if Any)
            if (_options is { ShowLegend: true })
            {
                switch (_legendPosition)
                {
                    case Position.Bottom:
                        _verticalEndSpace += CellPadding + _legendLabelsXAxis + LegendBox + CellPadding;
                        break;
                    case Position.Top:
                        _verticalStartSpace += CellPadding + _legendLabelsXAxis + LegendBox + CellPadding;
                        break;
                    case Position.Left:
                        _horizontalStartSpace += CellPadding + _legendLabelsYAxis + LegendBox + CellPadding;
                        break;
                    case Position.Right:
                        _horizontalEndSpace += CellPadding + _legendLabelsYAxis + LegendBox + CellPadding;
                        break;
                }
            }
        }

        private void BuildLegends()
        {
            _legends.Clear();
            var colors = GetEqualizedColorPalette(5); // Always generate 5 shades

            for (var i = 0; i < colors.Length; i++)
            {
                var t = i / (double)(colors.Length - 1);
                var value = _minValue + t * (_maxValue - _minValue);
                _legends.Add((value, colors[i].ToString(MudColorOutputFormats.RGB)));
            }
        }

        private double? GetDataValue(int row, int col)
        {
            // need to ensure row index exists in case there is no data for a row in a series
            if (row < 0 || row >= _series.Count)
            {
                return null;
            }
            // need to ensure column index exists in case there is no data for a column in a series
            if (col < 0 || _series[row].Data == null || col >= _series[row].Data.Length)
            {
                return null;
            }
            return _series[row].Data[col];
        }

        private string GetColorForValue(double? value)
        {
            if (value is null)
            {
                return "#fff"; // Default color for missing data
            }

            // Find the closest matching color in the legends
            var normalizedValue = Math.Clamp((value.Value - _minValue) / (_maxValue - _minValue), 0, 1);
            var legendIndex = (int)Math.Floor(normalizedValue * (_legends.Count - 1));
            return _legends[Math.Clamp(legendIndex, 0, _legends.Count - 1)].color;
        }

        private MudColor[] GetEqualizedColorPalette(int shadeCount)
        {
            var baseColors = _colorPalette.Select(x => new MudColor(x)).ToArray();
            var colorCount = baseColors.Length;

            if (colorCount == 1)
            {
                return MudColor.GenerateTintShadePalette(baseColors[0]).ToArray();
            }
            else if (colorCount != 5)
            {
                return MudColor.GenerateMultiGradientPalette(baseColors, shadeCount).ToArray();
            }
            else
            {
                return baseColors;
            }
        }

        private string FormatValueForDisplay(double? value)
        {
            if (value == null)
                return string.Empty;

            var formatString = _options?.ValueFormatString ?? "G";
            // Format the value and truncate to 5 characters or fewer
            var formattedValue = value.Value.ToString(formatString, CultureInfo.InvariantCulture);

            return formattedValue.Length > 5 ? formattedValue.Substring(0, 5) : formattedValue;
        }

        private static double CalculateFontSize(double cellWidth, double cellHeight, int defaultSize)
        {
            var minDimension = Math.Min(cellWidth, cellHeight);
            return Math.Max(defaultSize, minDimension * 0.4);
        }

        private (double x, double y) GetLegendPosition()
        {
            // Determine the horizontal position based on the legend's position.
            var x = _legendPosition switch
            {
                Position.Top or Position.Bottom => _horizontalStartSpace + (HeatmapWidth / 2),
                Position.Right => GetRightPosition(),
                Position.Left => GetLeftPosition(),
                _ => 0
            };

            // Determine the vertical position based on the legend's position.
            var y = _legendPosition switch
            {
                Position.Right or Position.Left => _verticalStartSpace + (HeatmapHeight / 2),
                Position.Bottom => GetBottomPosition(),
                Position.Top => GetTopPosition(),
                _ => 0
            };

            return (x, y);

            // Calculates the horizontal position for the legend when it is placed on the right.
            double GetRightPosition() =>
                _horizontalStartSpace + HeatmapWidth + HeatMapPadding + CellPadding +
                (_options?.YAxisLabelPosition == YAxisLabelPosition.Right ? _yAxisLabelWidth : 0);

            // Calculates the horizontal position for the legend when it is placed on the left.
            double GetLeftPosition() =>
                _horizontalStartSpace - HeatMapPadding - LegendBox - CellPadding -
                (_options?.YAxisLabelPosition == YAxisLabelPosition.Left ? _yAxisLabelWidth : 0);

            // Calculates the vertical position for the legend when it is placed at the bottom.
            double GetBottomPosition() =>
                _verticalStartSpace + HeatmapHeight + LegendBox + CellPadding +
                (_options?.XAxisLabelPosition == XAxisLabelPosition.Bottom ? _dynamicFontSize + CellPadding : 0);

            // Calculates the vertical position for the legend when it is placed at the top.
            double GetTopPosition() =>
                _verticalStartSpace - CellPadding - LegendBox - CellPadding -
                (_options?.XAxisLabelPosition == XAxisLabelPosition.Top ? _dynamicFontSize + CellPadding : 0);
        }
    }
}
