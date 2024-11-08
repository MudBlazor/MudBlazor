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

        private List<SvgPath> _horizontalLines = [];
        private List<SvgText> _horizontalValues = [];

        private List<SvgPath> _verticalLines = [];
        private List<SvgText> _verticalValues = [];

        private List<SvgLegend> _legends = [];
        private List<ChartSeries> _series = [];

        private List<HeatMapCell> _heatmapCells = [];

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            InitializeHeatmap();
        }

        private void InitializeHeatmap()
        {
            // Populate _heatmapCells based on data, e.g., matrix of values
            _heatmapCells = [];

            var rows = 10; // Adjust according to data
            var cols = 10; // Adjust according to data
            double cellWidth = 650 / cols;
            double cellHeight = 350 / rows;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var value = GetDataValue(row, col); // Method to retrieve the value for each cell
                    _heatmapCells.Add(new HeatMapCell
                    {
                        X = col * cellWidth,
                        Y = row * cellHeight,
                        Width = cellWidth,
                        Height = cellHeight,
                        Value = value
                    });
                }
            }
        }

        private string GetColorForValue(double value)
        {
            // Map the value to a color based on intensity. You might want to use a gradient for this.
            // For example, lower values could be light blue, and higher values could be dark red.
            return value < 0.5 ? "#ADD8E6" : "#FF4500"; // Example color mapping logic
        }

        private double GetDataValue(int row, int col)
        {
            // Replace this with the actual logic to get data values
            return new Random().NextDouble(); // Placeholder for demonstration
        }
    }
}
