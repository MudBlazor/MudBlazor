// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.AspNetCore.Components;

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
        private const int LegendBox = 18;
        // the minimum padding between cells and line length on legend labels
        private const int CellPadding = 5;
        // the line length on legend labels
        private const int LegendLineLength = 5;
        // the heatmap outside padding
        private const int HeatMapPadding = 15;
        private const int LegendFontSize = 12;
        // approximate width of YAxis Labels
        private const int LabelFontSize = 10;
        private const double AverageCharWidthMultiplier = 0.6;
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
        private int SeriesLength => _series.Max(s => s.Data.Length);
        // The number of rows visible
        private int RowCount => _series.Where(s => s.Visible).Count();
        // the amount of pixels a legend extends horizontally when it's on left/right
        private int _legendLabelsYAxis = 0;
        // the amount of pixels a legend extends vertically when it's on the top/bottom
        private int _legendLabelsXAxis = 0;
        // Calculate the actual width of the heatmap cells area
        private double HeatmapWidth => BoundWidth - _horizontalStartSpace - _horizontalEndSpace;
        // Calculate the actual height of the heatmap cells area
        private double HeatmapHeight => BoundHeight - _verticalStartSpace - _verticalEndSpace;

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        private ChartOptions? _options;

        private List<ChartSeries> _series = [];
        private List<(string value, string color)> _legends = [];
        private List<HeatMapCell> _heatmapCells = [];

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudChartParent != null)
            {
                if (_options == null || _options != MudChartParent.ChartOptions)
                {
                    _options = MudChartParent.ChartOptions;
                    _colorPalette = _options.ChartPalette ?? _colorPalette;
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
            _heatmapCells.Clear();
            _minValue = 0;
            _maxValue = 1;

            // # of rows
            var rows = _series.Count;
            // cols should be the max number of data[] in all series
            var cols = _series.Max(series => series.Data.Length);

            var cellWidth = BoundWidth / cols;
            var cellHeight = BoundHeight / rows;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var value = GetDataValue(row, col); // Method to retrieve the value for each cell
                    _heatmapCells.Add(new HeatMapCell
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
            _yAxisLabelWidth = (_series?.Max(x => x.Name.Length) ?? 1) * LabelFontSize * AverageCharWidthMultiplier;
            var defaultCharsWidth = 5 * LegendFontSize * AverageCharWidthMultiplier;
            _legendLabelsYAxis = (int)Math.Ceiling((_options?.ShowLabels ?? false) 
                ? (defaultCharsWidth + LegendLineLength) 
                : 0);
            _legendLabelsXAxis = (_options?.ShowLabels ?? false)
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
                _verticalStartSpace += CellPadding + LegendFontSize + CellPadding;
            }
            if (_options?.XAxisLabelPosition == XAxisLabelPosition.Bottom)
            {
                _verticalEndSpace += CellPadding + LegendFontSize + CellPadding;
            }
            // Make Room for Legend (if Any)
            if (_options?.ShowLegend ?? false)
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
                    default:
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
                _legends.Add((value.ToString("F2", CultureInfo.InvariantCulture), colors[i]));
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
            if (col < 0 || col >= _series[row].Data.Length)
            {
                return null;
            }
            return _series[row].Data[col];
        }

        private double? GetNeighborValue(int row, int col)
        {
            return GetDataValue(row, col);
        }

        private string GetColorForValue(double? value)
        {
            if (value == null)
            {
                return "#fff"; // Default color for missing data
            }

            // Find the closest matching color in the legends
            var normalizedValue = Math.Clamp((value.Value - _minValue) / (_maxValue - _minValue), 0, 1);
            var legendIndex = (int)Math.Floor(normalizedValue * (_legends.Count - 1));
            return _legends[Math.Clamp(legendIndex, 0, _legends.Count - 1)].color;
        }

        #region Color Methods
        private string[] GetEqualizedColorPalette(int shadeCount)
        {
            // Equalizes between 1 and 5 user colors supplied
            var baseColors = _colorPalette;
            var colorCount = baseColors.Length;

            var interpolatedColors = new string[shadeCount];
            for (var i = 0; i < shadeCount; i++)
            {
                var t = i / (double)(shadeCount - 1); // Normalized between 0 and 1

                if (colorCount == 1)
                {
                    // When there's only one color, vary the alpha or lightness
                    // we don't allow a 0 here instead moving it to a .1 lightness at the minimum
                    var tValue = t == 0 ? .1 : t;
                    var color = AdjustAlpha(baseColors[0], tValue);
                    interpolatedColors[i] = color;
                }
                else
                {
                    // For multiple colors, interpolate as before
                    var colorIndex = (int)Math.Floor(t * (colorCount - 1));
                    var nextColorIndex = Math.Min(colorIndex + 1, colorCount - 1);

                    var color = InterpolateColor(baseColors[colorIndex], baseColors[nextColorIndex], t);
                    interpolatedColors[i] = color;
                }
            }
            return interpolatedColors;
        }

        private static string AdjustAlpha(string color, double alpha)
        {
            (var r, var g, var b) = ParseColor(color);
            return $"rgba({r}, {g}, {b}, {alpha.ToString("F2", CultureInfo.InvariantCulture)})";
        }

        private static string InterpolateColor(string colorStart, string colorEnd, double t)
        {
            (var r1, var g1, var b1) = ParseColor(colorStart);
            (var r2, var g2, var b2) = ParseColor(colorEnd);

            var r = (int)(r1 + (r2 - r1) * t);
            var g = (int)(g1 + (g2 - g1) * t);
            var b = (int)(b1 + (b2 - b1) * t);

            return $"rgb({r}, {g}, {b})";
        }

        private static (int, int, int) ParseColor(string color)
        {
            if (color.StartsWith('#'))
            {
                return HexToRgb(color);
            }
            else if (color.StartsWith("rgba") || color.StartsWith("rgb"))
            {
                return RgbaToRgb(color);
            }
            throw new FormatException($"Unsupported color format: {color}");
        }

        private static (int, int, int) HexToRgb(string hex)
        {
            hex = hex.TrimStart('#');
            var r = Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);
            return (r, g, b);
        }

        private static (int, int, int) RgbaToRgb(string rgba)
        {
            var values = rgba.TrimStart("rgba(".ToCharArray()).TrimEnd(')').Split(',');
            var r = int.Parse(values[0]);
            var g = int.Parse(values[1]);
            var b = int.Parse(values[2]);
            return (r, g, b);
        }

        #endregion

        #region Razor Assist Methods
        private string FormatValueForDisplay(double? value)
        {
            if (value == null)
                return string.Empty;

            var formatString = _options?.ValueFormatString ?? "G";
            // Format the value and truncate to 5 characters or less
            var formattedValue = value.Value.ToString(formatString, CultureInfo.InvariantCulture);

            return formattedValue.Length > 5 ? formattedValue.Substring(0, 5) : formattedValue;
        }

        private string FormatValueForDisplay(string? strValue)
        {
            var value = double.TryParse(strValue, out var parsedValue) ? parsedValue : (double?)null;
            return FormatValueForDisplay(value);
        }

        private (double x, double y) GetLegendPosition()
        {
            var x = _legendPosition switch
            {
                Position.Top or Position.Bottom =>
                    (_horizontalStartSpace + HeatmapWidth + _horizontalEndSpace) / 2,
                Position.Right =>
                    _horizontalStartSpace + HeatmapWidth + HeatMapPadding + _legendLabelsYAxis,
                Position.Left =>
                    HeatMapPadding,
                _ => 0
            };

            var y = _legendPosition switch
            {
                Position.Right or Position.Left =>
                    (_verticalStartSpace + HeatmapHeight + _verticalEndSpace) / 2,
                Position.Bottom =>
                    _verticalStartSpace + HeatmapHeight + HeatMapPadding,
                Position.Top =>
                    HeatMapPadding,
                _ => 0
            };

            return (x, y);
        }
        #endregion
    }
}
