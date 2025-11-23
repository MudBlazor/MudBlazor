// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Interop;
using MudBlazor.Utilities.Debounce;

#nullable enable
namespace MudBlazor.Charts;

/// <summary>
/// Represents a base class for radial charts.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
/// <typeparam name="TOptions">The type of options for the chart.</typeparam>
public abstract class MudRadialChartBase<T, TOptions> : MudChartBase<T, TOptions>, IDisposable
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    where TOptions : IRadialChartOptions
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The chart, if any, containing this component.
    /// </summary>
    [CascadingParameter]
    public MudChart<T>? MudChartParent { get; set; }

    private const double BoundWidthDefault = 280;
    private const double BoundHeightDefault = 280;
    private const int DebounceIntervalMs = 200;

    private readonly DotNetObjectReference<MudRadialChartBase<T, TOptions>> _dotNetObjectReference;
    private readonly DebounceDispatcher _debouncer = new(DebounceIntervalMs);

    private ElementSize? _elementSize;

    /// <summary>
    /// The width of the chart's bounds.
    /// </summary>
    protected double _boundWidth = 280;

    /// <summary>
    /// The height of the chart's bounds.
    /// </summary>
    protected double _boundHeight = 280;

    /// <summary>
    /// The SVG paths for the chart segments.
    /// </summary>
    internal List<SvgPath> _paths = [];

    /// <summary>
    /// The legends for the chart.
    /// </summary>
    internal List<SvgLegend> _legends = [];

    /// <summary>
    /// The currently hovered SVG path segment.
    /// </summary>
    internal SvgPath? _hoveredSegment;

    /// <summary>
    /// The indices of hidden data series.
    /// </summary>
    protected HashSet<int> HiddenIndices { get; set; } = [];

    /// <summary>
    /// The radius of the radial chart.
    /// </summary>
    protected double Radius => Math.Round(Math.Min(_boundWidth, _boundHeight) / 2);

    /// <summary>
    /// Initializes a new instance of the <see cref="MudRadialChartBase{T, TOptions}"/> class.
    /// </summary>
    [DynamicDependency(nameof(OnElementSizeChanged))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElementSize))]
    protected MudRadialChartBase()
    {
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    /// <summary>
    /// Called when the component's parameters are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        _paths.Clear();
        _legends.Clear();
        HiddenIndices.Clear();
        _hoveredSegment = null;

        if (MatchBoundsToSize && _elementSize is null) return;

        if (ChartSeries == null || ChartSeries.Count == 0)
            return;

        RebuildChart();
    }

    /// <summary>
    /// Sets the element reference and observes its size.
    /// </summary>
    /// <param name="elementRef">The element reference.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task SetElementReference(ElementReference elementRef)
    {
        var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, elementRef);

        OnElementSizeChanged(elementSize);
    }

    /// <summary>
    /// Gets the labels for the chart.
    /// </summary>
    /// <returns>An array of chart labels.</returns>
    protected string[] GetChartLabels()
    {
        return ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet
            ? ChartSeries.Select(ds => ds.Name).ToArray()
            : ChartLabels ?? [];
    }

    /// <summary>
    /// Aggregates the series data based on the specified aggregation option.
    /// </summary>
    /// <param name="aggregation">The aggregation option.</param>
    /// <returns>An array of aggregated data.</returns>
    protected T[] AggregateSeriesData(AggregationOption aggregation)
    {
        if (aggregation == AggregationOption.None || ChartSeries is null || ChartSeries.Count == 0 || !ChartSeries.Any(x => x.Visible))
            return [];

        var maxCategoryLength = ChartOptions!.AggregationOption == AggregationOption.GroupByLabel
                ? GetMaxCategoryLengthForLabelGrouping()
                : ChartSeries.Count;

        var aggregated = new T[maxCategoryLength];

        return aggregation switch
        {
            AggregationOption.GroupByLabel => AggregateByLabel(aggregated),
            AggregationOption.GroupByDataSet => AggregateByDataSet(aggregated),
            _ => throw new ArgumentOutOfRangeException(nameof(aggregation), $"Unsupported aggregation: {aggregation}")
        };
    }

    private int GetMaxCategoryLengthForLabelGrouping()
    {
        if (ChartLabels.Length > 0)
            return ChartLabels.Length;

        return ChartSeries.Where(x => x.Data?.Values != null).DefaultIfEmpty()
                          .Max(x => x?.Data?.Values.Count ?? 0);
    }

    private T[] AggregateByLabel(T[] aggregated)
    {
        foreach (var series in ChartSeries.Where(s => s.Visible))
        {
            var values = series.Data?.Values ?? [];

            for (var i = 0; i < values.Count; i++)
            {
                if (!HiddenIndices.Contains(i) && i < aggregated.Length)
                    aggregated[i] += values[i];
            }
        }

        return aggregated;
    }

    private T[] AggregateByDataSet(T[] aggregated)
    {
        var chartSeries = ChartSeries.Take(aggregated.Length);

        foreach (var (series, index) in chartSeries.Select((s, i) => (s, i)))
        {
            if (!series.Visible) continue;

            aggregated[index] = series.Data?.Values.SumGeneric() ?? T.Zero;
        }

        return aggregated;
    }

    /// <summary>
    /// Builds the legends for the chart.
    /// </summary>
    /// <param name="chartLabels">The labels for the chart.</param>
    protected void BuildLegends(string[] chartLabels)
    {
        for (var i = 0; i < chartLabels.Length; i++)
        {
            var label = chartLabels[i];

            if (string.IsNullOrWhiteSpace(label))
                continue;

            _legends.Add(new SvgLegend
            {
                Index = i,
                Labels = label,
                Visible = ChartOptions!.AggregationOption == AggregationOption.GroupByLabel
                    ? !HiddenIndices.Contains(i)
                    : ChartSeries[i].Visible,
                OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
            });
        }
    }

    /// <summary>
    /// Sets the bounds of the chart.
    /// </summary>
    protected void SetBounds()
    {
        _boundWidth = BoundWidthDefault;
        _boundHeight = BoundHeightDefault;

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

    /// <summary>
    /// Scales the input data to the range between 0 and 1
    /// </summary>
    protected T[] GetNormalizedData()
    {
        if (ChartSeries is null || ChartSeries.Count == 0)
            return [];

        var data = AggregateSeriesData(ChartOptions!.AggregationOption);
        var total = data.SumGeneric();

        if (total == T.Zero)
            return data;

        return data.Select(x => T.Abs(x) / total).ToArray();
    }

    /// <summary>
    /// Handles the visibility change of a legend.
    /// </summary>
    /// <param name="legend">The legend whose visibility changed.</param>
    protected void HandleLegendVisibilityChanged(SvgLegend legend)
    {
        if (legend.Visible)
            HiddenIndices.Remove(legend.Index);
        else
            HiddenIndices.Add(legend.Index);

        if (ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet)
            ChartSeries[legend.Index].Visible = legend.Visible;

        RebuildChart();
    }

    /// <summary>
    /// Represents the coordinates of a segment in a radial chart.
    /// </summary>
    protected readonly struct SegmentCoordinates
    {
        /// <summary>
        /// The starting X coordinate.
        /// </summary>
        public double StartX { get; init; }
        /// <summary>
        /// The starting Y coordinate.
        /// </summary>
        public double StartY { get; init; }
        /// <summary>
        /// The middle X coordinate.
        /// </summary>
        public double MidX { get; init; }
        /// <summary>
        /// The middle Y coordinate.
        /// </summary>
        public double MidY { get; init; }
        /// <summary>
        /// The ending X coordinate.
        /// </summary>
        public double EndX { get; init; }
        /// <summary>
        /// The ending Y coordinate.
        /// </summary>
        public double EndY { get; init; }
        /// <summary>
        /// The large arc flag for SVG path.
        /// </summary>
        public int LargeArcFlag { get; init; }
    }

    /// <summary>
    /// Handles the mouse over event for a segment.
    /// </summary>
    /// <param name="args">The mouse event arguments.</param>
    /// <param name="segment">The hovered segment path.</param>
    internal virtual void OnSegmentMouseOver(MouseEventArgs args, SvgPath segment) => _hoveredSegment = segment;

    /// <summary>
    /// Handles the mouse out event for a segment.
    /// </summary>
    internal virtual void OnSegmentMouseOut() => _hoveredSegment = null;

    /// <summary>
    /// Called when the element size changes.
    /// </summary>
    /// <param name="elementSize">The new element size.</param>
    [JSInvokable]
    public void OnElementSizeChanged(ElementSize elementSize)
    {
        if (elementSize == null || elementSize.Timestamp <= _elementSize?.Timestamp)
            return;

        _elementSize = elementSize;

        if (!MatchBoundsToSize)
            return;

        var minDimension = Math.Min(_elementSize.Width, _elementSize.Height);
        _boundWidth = minDimension;
        _boundHeight = minDimension;

        _debouncer.DebounceAfterFirstExecuteAsync(async () =>
        {
            await InvokeAsync(() =>
            {
                RebuildChart();
                StateHasChanged();
            });
        }).CatchAndLog();
    }

    /// <summary>
    /// Releases the resources used by the component.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the component and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _debouncer?.Cancel();
        }

        _dotNetObjectReference.Dispose();
    }
}
