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
        private const double HorizontalStartSpace = 30.0;
        private const double HorizontalEndSpace = 30.0;
        private const double VerticalStartSpace = 25.0;
        private const double VerticalEndSpace = 25.0;
        private const int PaddingAll = 5;
        private const int MinSize = 8;
        private double _minValue = 0.0;
        private double _maxValue = 1.0;
        private string[] colorPalette = ["#587934"];
        private int SeriesLength => _series.Max(s => s.Data.Length);
        private int RowCount => _series.Where(s => s.Visible).Count();

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
                    //colorPalette = _options.ChartPalette ?? colorPalette;
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
            _heatmapCells = [];

            var rows = _series.Count; // # of rows
            // cols should be the max number of data[] in all series
            var cols = _series.Max(series => series.Data.Length);

            double cellWidth = 650 / cols;
            double cellHeight = 350 / rows;

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
        }

        private double? GetDataValue(int row, int col)
        {
            // need to ensure column index exists in case there is no data for a column in a series
            if (col >= _series[row].Data.Length)
            {
                return null;
            }
            return _series[row].Data[col];
        }

        private string GetColorForValue(double? value)
        {
            if (value == null)
            {
                return "#fff"; // Default color for missing data
            }
            _legends.Clear();
            var colors = GetEqualizedColorPalette(5); // Always generate 5 shades

            // Determine index based on normalized value
            var normalizedValue = Math.Clamp((value.Value - _minValue) / (_maxValue - _minValue), 0, 1);
            var index = (int)Math.Floor(normalizedValue * (colors.Length - 1));
            return colors[Math.Clamp(index, 0, colors.Length - 1)];
        }

        private string[] GetEqualizedColorPalette(int shadeCount)
        {
            string[] baseColors = colorPalette;
            var colorCount = baseColors.Length;

            var interpolatedColors = new string[shadeCount];
            if (_legends.Count == 0) // if legend doesn't exist, create it
            {
                for (var i = 0; i < shadeCount; i++)
                {
                    var t = i / (double)(shadeCount - 1); // Normalized between 0 and 1

                    if (colorCount == 1)
                    {
                        // When there's only one color, vary the alpha or lightness
                        var color = AdjustAlpha(baseColors[0], t == 0 ? .1 : t);
                        interpolatedColors[i] = color;
                        _legends.Add((value: (_minValue + t * (_maxValue - _minValue)).ToString("F2", CultureInfo.InvariantCulture), color: color));
                    }
                    else
                    {
                        // For multiple colors, interpolate as before
                        var colorIndex = (int)Math.Floor(t * (colorCount - 1));
                        var nextColorIndex = Math.Min(colorIndex + 1, colorCount - 1);

                        var color = InterpolateColor(baseColors[colorIndex], baseColors[nextColorIndex], t);
                        interpolatedColors[i] = color;
                        _legends.Add((value: (_minValue + t * (_maxValue - _minValue)).ToString("F2", CultureInfo.InvariantCulture), color: color));
                    }
                }
            }
            return interpolatedColors;
        }

        private static string AdjustAlpha(string color, double alpha)
        {
            (var r, var g, var b) = ParseColor(color);
            var adjustedAlpha = (int)(alpha * 255);
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
            if (color.StartsWith("#"))
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

    }
}
