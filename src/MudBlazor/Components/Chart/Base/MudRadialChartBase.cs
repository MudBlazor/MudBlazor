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
    protected double _boundWidth = 280;
    protected double _boundHeight = 280;

    internal List<SvgPath> _paths = [];
    internal List<SvgLegend> _legends = [];
    internal SvgPath? _hoveredSegment;

    protected HashSet<int> HiddenIndices { get; set; } = [];
    protected double Radius => Math.Round(Math.Min(_boundWidth, _boundHeight) / 2);

    [DynamicDependency(nameof(OnElementSizeChanged))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElementSize))]
    protected MudRadialChartBase()
    {
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

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

    protected async Task SetElementReference(ElementReference elementRef)
    {
        var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, elementRef);

        OnElementSizeChanged(elementSize);
    }

    protected string[] GetChartLabels()
    {
        return ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet
            ? ChartSeries.Select(ds => ds.Name).ToArray()
            : ChartLabels ?? [];
    }

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

    protected readonly struct SegmentCoordinates
    {
        public double StartX { get; init; }
        public double StartY { get; init; }
        public double MidX { get; init; }
        public double MidY { get; init; }
        public double EndX { get; init; }
        public double EndY { get; init; }
        public int LargeArcFlag { get; init; }
    }

    internal virtual void OnSegmentMouseOver(MouseEventArgs args, SvgPath segment) => _hoveredSegment = segment;

    internal virtual void OnSegmentMouseOut() => _hoveredSegment = null;

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

        _ = _debouncer.DebounceAfterFirstExecuteAsync(async () =>
        {
            await InvokeAsync(() =>
            {
                RebuildChart();
                StateHasChanged();
            });
        });
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _debouncer?.Cancel();
        }

        _dotNetObjectReference.Dispose();
    }
}
