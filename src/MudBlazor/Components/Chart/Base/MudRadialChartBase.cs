// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities.Debounce;

#nullable enable
namespace MudBlazor.Charts;

public abstract class MudRadialChartBase<TOptions> : MudChartBase<TOptions>, IDisposable where TOptions : IRadialChartOptions
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The chart, if any, containing this component.
    /// </summary>
    [CascadingParameter]
    public MudChart? MudChartParent { get; set; }

    private const double BoundWidthDefault = 280;
    private const double BoundHeightDefault = 280;
    private const int DebounceIntervalMs = 200;

    private readonly DotNetObjectReference<MudRadialChartBase<TOptions>> _dotNetObjectReference;
    private readonly DebounceDispatcher _debouncer = new(DebounceIntervalMs);

    private ElementSize? _elementSize;
    private double _boundWidth = 280;
    private double _boundHeight = 280;

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

        if (MatchBoundsToSize is true && _elementSize is null) return;

        if (ChartSeries == null || ChartSeries.Count == 0)
            return;

        RebuildChart();
    }

    protected async Task SetElementReference(ElementReference elementRef)
    {
        var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, elementRef);

        OnElementSizeChanged(elementSize);
    }

    protected double[] AggregateSeriesData(AggregationOption aggregation)
    {
        if (aggregation == AggregationOption.None || ChartSeries is null || !ChartSeries.Any(x => x.Visible))
            return [];

        var maxCategoryLength = ChartOptions!.AggregationOption == AggregationOption.GroupByLabel
                ? ChartLabels.Length == 0
                    ? ChartSeries.Count > 0 ? ChartSeries.Where(x => x.Data?.Values != null).DefaultIfEmpty().Max(x => x?.Data?.Values.Length ?? 0) : 0
                    : ChartLabels.Length
                : ChartSeries.Count;
        var aggregated = new double[maxCategoryLength];

        switch (aggregation)
        {
            case AggregationOption.GroupByLabel:
                foreach (var series in ChartSeries)
                {
                    if (!series.Visible)
                        continue;

                    var values = series.Data?.Values ?? [];

                    for (var i = 0; i < values.Length; i++)
                    {
                        if (HiddenIndices.Contains(i))
                            continue;

                        aggregated[i] += values[i];
                    }
                }
                break;

            case AggregationOption.GroupByDataSet:
                var index = -1;

                foreach (var series in ChartSeries)
                {
                    index++;

                    if (!series.Visible)
                        continue;

                    if (index >= aggregated.Length)
                        break;

                    aggregated[index] = series.Data?.Values?.Sum() ?? 0;
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(aggregation), $"Unsupported aggregation: {aggregation}");
        }

        return aggregated;
    }

    protected void SetBounds()
    {
        _boundWidth = BoundWidthDefault;
        _boundHeight = BoundHeightDefault;

        if (MudChartParent?.MatchBoundsToSize is true)
        {
            if (_elementSize is not null)
            {
                _boundWidth = _elementSize.Width;
                _boundHeight = _elementSize.Height;
            }
            else if (MudChartParent.Width.EndsWith("px")
                && MudChartParent.Height.EndsWith("px")
                && double.TryParse(MudChartParent.Width.AsSpan(0, MudChartParent.Width.Length - 2), out var width)
                && double.TryParse(MudChartParent.Height.AsSpan(0, MudChartParent.Height.Length - 2), out var height))
            {
                _boundWidth = width;
                _boundHeight = height;
            }
        }
    }

    /// <summary>
    /// Scales the input data to the range between 0 and 1
    /// </summary>
    protected double[] GetNormalizedData()
    {
        if (ChartSeries is null || ChartSeries.Count == 0)
            return [];

        var data = AggregateSeriesData(ChartOptions!.AggregationOption);
        var total = data.Sum();

        if (total == 0)
            return data;

        return data.Select(x => Math.Abs(x) / total).ToArray();
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

    internal void OnSegmentMouseOver(MouseEventArgs args, SvgPath segment) => _hoveredSegment = segment;

    internal void OnSegmentMouseOut() => _hoveredSegment = null;

    [JSInvokable]
    public void OnElementSizeChanged(ElementSize elementSize)
    {
        if (elementSize == null || elementSize.Timestamp <= _elementSize?.Timestamp)
            return;

        _elementSize = elementSize;

        if (MudChartParent?.MatchBoundsToSize is not true)
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

    public abstract void RebuildChart();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _dotNetObjectReference.Dispose();
    }
}
