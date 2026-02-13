// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Debounce;


namespace MudBlazor.Charts
{
    partial class HeatMap<T> : MudChartBase<T, HeatMapChartOptions>, IDisposable where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        internal record CellDimension(double Width, double Height, int Padding);

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        private readonly DotNetObjectReference<HeatMap<T>> _dotNetObjectReference;

        protected ElementReference _elementReference;

        private ElementSize? _elementSize;

        protected const double Epsilon = 1e-6;

        private readonly List<HeatMapCell<T>> _heatMapCells = [];

        private const double BoundWidth = 800.0;

        private const double BoundHeight = 350.0;

        internal Position _legendPosition = Position.Bottom;

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
        private readonly List<SvgLegend> _toggleLegend = [];

        private double _boundWidth = BoundWidth;

        private double _boundHeight = BoundHeight;

        private double _dynamicFontSize = 8;

        private double _yAxisLabelWidth = 0;

        // padding or legend area for each side of the heatmap
        private double _horizontalStartSpace = HeatMapPadding;

        private double _horizontalEndSpace = HeatMapPadding;

        private double _verticalStartSpace = HeatMapPadding;

        private double _verticalEndSpace = HeatMapPadding;

        // the minimum value in all series
        internal T _minValue = T.MaxValue;

        // the maximum value in all series
        internal T _maxValue = T.MinValue;

        private string[] _colorPalette = ["#587934"];

        // The maximum number of cells in a series
        private int SeriesLength => _series.Select(s => s.Data?.Values.Count ?? 0).DefaultIfEmpty(0).Max();

        // The number of rows visible
        private int RowCount => _series.Count > 0 ? _series.Count(s => s.Visible) : 0;

        // the amount of pixels a legend extends horizontally when it's on left/right
        private int _legendLabelsYAxis = 0;

        // the amount of pixels a legend extends vertically when it's on the top/bottom
        private int _legendLabelsXAxis = 0;

        // Calculate the actual width of the heatmap cells area
        private double HeatmapWidth => _boundWidth - _horizontalStartSpace - _horizontalEndSpace;

        // Calculate the actual height of the heatmap cells area
        private double HeatmapHeight => _boundHeight - _verticalStartSpace - _verticalEndSpace;

        private HeatMapChartOptions? _options;

        private List<ChartSeries<T>> _series = [];

        private readonly List<(T value, string color)> _legends = [];

        internal List<MudHeatMapCell<T>> _customHeatMapCells = [];

        private CellDimension _cellDimension = new(0, 0, 0);

        private HeatMapCell<T>? _hoveredCell;

        private (T value, string color)? _hoveredLegend;

        private PointF _hoveredLegendPosition;

        private readonly DebounceDispatcher _debouncer = new(DebounceIntervalMs, leading: true);

        private const int DebounceIntervalMs = 150;

        private string HoveredStylename =>
            new StyleBuilder()
                .AddStyle("overflow", "visible", _hoveredCell is not null || _hoveredLegend is not null)
                .Build();

        /// <summary>
        /// The currently selected <see cref="HeatMapCell{T}"/>.
        /// </summary>
        public (int Row, int Column) SelectedCell { get; set; }

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart<T>? MudChartParent { get; set; }

        [DynamicDependency(nameof(OnElementSizeChanged))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElementSize))]
        public HeatMap()
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            UpdateLegendPosition(LegendPosition);
            UpdateChartSeries(ChartSeries);

            if (ChartOptions is not null)
            {
                UpdateChartOptions(ChartOptions);
            }

            UpdateHeatMapCells(MudHeatMapCells);

            RebuildChart();
        }

        protected override void OnInitialized()
        {
            ChartType = ChartType.HeatMap;
            ChartOptions ??= new HeatMapChartOptions();
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, _elementReference);
                OnElementSizeChanged(elementSize);
            }
        }

        private void UpdateLegendPosition(Position position)
        {
            _legendPosition = position switch
            {
                Position.Center => Position.Bottom,
                Position.Start => Position.Left,
                Position.End => Position.Right,
                _ => position
            };
        }

        private void UpdateChartOptions(HeatMapChartOptions chartOptions)
        {
            if (_options == null || _options != chartOptions)
            {
                _options = chartOptions;
                _colorPalette = _options.ChartPalette.Length > 0 ? _options.ChartPalette : _colorPalette;
            }
        }

        private void UpdateChartSeries(List<ChartSeries<T>> chartSeriesList)
        {
            var hasUpdatedList = chartSeriesList.Count > 0 && _series != chartSeriesList;

            if (_series.Count == 0 || hasUpdatedList)
            {
                _series.Clear();
                _series = chartSeriesList;
            }
        }

        private void UpdateHeatMapCells(List<MudHeatMapCell<T>> mudHeatMapCellsList)
        {
            var hasUpdatedList = mudHeatMapCellsList.Count > 0 && _customHeatMapCells != mudHeatMapCellsList;

            if (_customHeatMapCells.Count == 0 || hasUpdatedList)
            {
                _customHeatMapCells.Clear();
                _customHeatMapCells = mudHeatMapCellsList;
            }

            var padding = _options is { EnableSmoothGradient: true } ? 0 : CellPadding;

            if (RowCount == 0 || SeriesLength == 0)
            {
                _cellDimension = new CellDimension(CellMinSize, CellMinSize, padding);
                return;
            }

            var cellHeight = Math.Max(CellMinSize, (_boundHeight - _verticalStartSpace - _verticalEndSpace - (padding * (RowCount - 1))) / RowCount);
            var cellWidth = Math.Max(CellMinSize, (_boundWidth - _horizontalStartSpace - _horizontalEndSpace - padding * (SeriesLength - 1)) / SeriesLength);

            _cellDimension = new CellDimension(cellWidth, cellHeight, padding);
        }

        public override void RebuildChart()
        {
            SetBounds();
            // Populate _heatmapCells based on data, e.g., matrix of values
            _heatMapCells.Clear();
            _toggleLegend.Clear();
            _minValue = T.MaxValue;
            _maxValue = T.MinValue;

            var hasValues = false;
            // # of rows
            var rows = _series.Count;
            // cols should be the max number of data[] in all series
            var cols = SeriesLength;

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var mudHeatMapOverride = _customHeatMapCells.FirstOrDefault(x => x.Row == row && x.Column == col);
                    var value = mudHeatMapOverride?.Value ?? GetDataValue(row, col); // Method to retrieve the value for each cell                    
                    _heatMapCells.Add(new HeatMapCell<T>
                    {
                        Row = row,
                        Column = col,
                        Value = value,
                        CustomFragment = mudHeatMapOverride?.ChildContent,
                        Width = mudHeatMapOverride?.Width,
                        Height = mudHeatMapOverride?.Height,
                        MudColor = mudHeatMapOverride?.MudColor,
                    });
                    if (value.HasValue)
                    {
                        _minValue = T.Min(_minValue, value.Value);
                        _maxValue = T.Max(_maxValue, value.Value);
                        hasValues = true;
                    }
                }

                if (CanHideSeries)
                {
                    var legend = new SvgLegend()
                    {
                        Index = row,
                        Labels = _series[row].Name,
                        Visible = _series[row].Visible,
                        OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                    };
                    _toggleLegend.Add(legend);
                }
            }

            var overrideMinValue = _customHeatMapCells.LastOrDefault(x => x.MinValue.HasValue)?.MinValue;
            var overrideMaxValue = _customHeatMapCells.LastOrDefault(x => x.MaxValue.HasValue)?.MaxValue;

            _minValue = overrideMinValue ?? (hasValues ? _minValue : T.Zero);
            _maxValue = overrideMaxValue ?? (hasValues ? _maxValue : T.One);

            CalculateAreas();
            BuildLegends();
            UpdateHeatMapCells(MudHeatMapCells);
            StateHasChanged();
        }

        private void HandleLegendVisibilityChanged(SvgLegend legend)
        {
            var series = _series[legend.Index];
            series.Visible = legend.Visible;
            RebuildChart();
        }

        private void CalculateAreas()
        {
            // Defaults each side gets some space around the heatmap
            _verticalStartSpace = _verticalEndSpace = _horizontalStartSpace = _horizontalEndSpace = HeatMapPadding;
            var estimatedCellWidth = Math.Max(CellMinSize, (_boundWidth - (6 * HeatMapPadding) - CellPadding) / Math.Max(1, SeriesLength));
            var estimatedCellHeight = (_boundHeight - (6 * HeatMapPadding)) / Math.Max(1, RowCount);
            _dynamicFontSize = CalculateFontSize(estimatedCellWidth, estimatedCellHeight, 8);

            // Calculate Y-axis label width based on dynamic font size
            _yAxisLabelWidth = (_series.Count > 0 ? _series?.Max(x => x.Name.Length) ?? 1 : 1) * _dynamicFontSize * AverageCharWidthMultiplier;

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
                var value = _minValue + T.CreateSaturating(t) * (_maxValue - _minValue);
                _legends.Add((value, colors[i].ToString(MudColorOutputFormats.RGB)));
            }
        }

        private T? GetDataValue(int row, int col)
        {
            // need to ensure row index exists in case there is no data for a row in a series
            if (row < 0 || row >= _series.Count)
            {
                return null;
            }
            // need to ensure column index exists in case there is no data for a column in a series
            if (col < 0 || _series[row].Data == null || col >= _series[row].Data.Values.Count)
            {
                return null;
            }
            return _series[row].Data[col].Y;
        }

        private string GetColorForValue(T? value)
        {
            if (value is null)
            {
                return "#fff"; // Default color for missing data
            }

            var range = _maxValue - _minValue;
            var offset = value.Value - _minValue;

            // Find the closest matching color in the legends
            var normalizedValue = Math.Clamp(double.CreateSaturating(offset / range), 0.0, 1.0);
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

        private string FormatValueForDisplay(T? value)
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

            return Math.Max(defaultSize, 2 * Math.Sqrt(minDimension));
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
                _horizontalStartSpace + HeatmapWidth + HeatMapPadding +
                (_options?.YAxisLabelPosition == YAxisLabelPosition.Right ? _yAxisLabelWidth + CellPadding : 0);

            // Calculates the horizontal position for the legend when it is placed on the left.
            double GetLeftPosition() =>
                _horizontalStartSpace - HeatMapPadding - LegendBox -
                (_options?.YAxisLabelPosition == YAxisLabelPosition.Left ? _yAxisLabelWidth + CellPadding : 0);

            // Calculates the vertical position for the legend when it is placed at the bottom.
            double GetBottomPosition() =>
                _verticalStartSpace + HeatmapHeight + HeatMapPadding + CellPadding + CellPadding +
                (_options?.XAxisLabelPosition == XAxisLabelPosition.Bottom ? _dynamicFontSize : 0);

            // Calculates the vertical position for the legend when it is placed at the top.
            double GetTopPosition() =>
                _verticalStartSpace - CellPadding - LegendBox -
                (_options?.XAxisLabelPosition == XAxisLabelPosition.Top ? _dynamicFontSize + CellPadding : 0);
        }

        internal List<MudHeatMapCell<T>> MudHeatMapCells { get; set; } = [];

        internal void AddCell(MudHeatMapCell<T> cell)
        {
            MudHeatMapCells.Add(cell);

            DebouncedRebuild();
        }

        private void SetBounds()
        {
            _boundWidth = BoundWidth;
            _boundHeight = BoundHeight;

            if (MatchBoundsToSize)
            {
                if (_elementSize is not null)
                {
                    _boundWidth = _elementSize.Width;
                    _boundHeight = _elementSize.Height;
                }
                else if (Width.EndsWith("px")
                    && Height.EndsWith("px")
                    && double.TryParse(Width.AsSpan(0, Width.Length - 2), out var width)
                    && double.TryParse(Height.AsSpan(0, Height.Length - 2), out var height))
                {
                    _boundWidth = width;
                    _boundHeight = height;
                }
            }
        }

        [JSInvokable]
        public void OnElementSizeChanged(ElementSize elementSize)
        {
            if (elementSize == null || elementSize.Timestamp <= _elementSize?.Timestamp)
                return;

            _elementSize = elementSize;

            if (!MatchBoundsToSize)
                return;

            if (Math.Abs(_boundWidth - _elementSize.Width) < Epsilon &&
                Math.Abs(_boundHeight - _elementSize.Height) < Epsilon)
            {
                return;
            }

            DebouncedRebuild();
        }

        private void DebouncedRebuild()
        {
            _debouncer.DebounceAsync(async () =>
            {
                await InvokeAsync(() =>
                {
                    RebuildChart();
                });
            }).CatchAndLog();
        }

        private void OnCellMouseOver(MouseEventArgs _, HeatMapCell<T>? cell)
        {
            _hoveredCell = cell;
        }

        private void OnCellMouseOut(MouseEventArgs _)
        {
            _hoveredCell = null;
        }

        private void OnLegendMouseOver(MouseEventArgs _, (T value, string color) legend, PointF position)
        {
            _hoveredLegend = legend;
            _hoveredLegendPosition = position;
        }

        private void OnLegendMouseOut()
        {
            _hoveredLegend = null;
        }

        internal async Task SetSelectedCellAsync(int row, int column)
        {
            SelectedCell = (row, column);

            await SetSelectedIndexAsync(row);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            _debouncer.Dispose();
            _dotNetObjectReference?.Dispose();
        }
    }
}
