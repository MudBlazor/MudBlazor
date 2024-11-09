// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        private ChartOptions? _options;

        private List<SvgPath> _horizontalLines = [];
        private List<SvgText> _horizontalValues = [];

        private List<SvgPath> _verticalLines = [];
        private List<SvgText> _verticalValues = [];

        private List<SvgLegend> _legends = [];
        private List<ChartSeries> _series = [];

        private List<HeatMapCell> _heatmapCells = [];
        private int _minValue = 0;
        private int _maxValue = 0;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudChartParent != null)
            {
                _options = MudChartParent.ChartOptions;
                _series = MudChartParent.ChartSeries;
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

            if (_options?.EnableSmoothGradient ?? false)
            {
                // set _options.ChartPalette values for index 0 and 1 if they don't exist
                if (_options.ChartPalette == null || _options.ChartPalette.Length < 2)
                {
                    _options.ChartPalette = new string[2]
                    {
                        "#ADD8E6",
                        "#FF4500"
                    };
                }
                // Apply gradient based on value range (e.g., from 0 to 100)
                double normalizedValue = Math.Clamp((value.Value - _minValue) / (_maxValue - _minValue), 0, 1);
                return InterpolateColor(_options.ChartPalette[0], _options.ChartPalette[1], normalizedValue);
            }

            // Default color mapping
            return value < 50 ? "#ADD8E6" : "#FF4500";
        }

        private string InterpolateColor(string colorStart, string colorEnd, double t)
        {
            // Interpolate between colorStart and colorEnd based on t (0 to 1)
            // Use RGB channel interpolation logic here
            // Example: linear interpolation for each RGB component
            var r = (int)(colorStart[0] + (colorEnd[0] - colorStart[0]) * t);
            var g = (int)(colorStart[1] + (colorEnd[1] - colorStart[1]) * t);
            var b = (int)(colorStart[2] + (colorEnd[2] - colorStart[2]) * t);
            return $"rgb({r}, {g}, {b})";
        }

    }
}
