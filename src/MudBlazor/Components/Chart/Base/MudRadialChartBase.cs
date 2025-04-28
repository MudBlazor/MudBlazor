// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;

#nullable enable
namespace MudBlazor.Components.Chart;

public abstract partial class MudRadialChartBase<TOptions> : MudChartBase<TOptions>, IDisposable where TOptions : IChartOptions
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The chart, if any, containing this component.
    /// </summary>
    [CascadingParameter]
    public MudChart? MudChartParent { get; set; }

    private readonly DotNetObjectReference<MudRadialChartBase<TOptions>> _dotNetObjectReference;
    private ElementSize? _elementSize;
    private ElementReference _elementReference;
    private double _boundWidth = 280;
    private double _boundHeight = 280;
    internal List<SvgPath> _paths = [];
    internal List<SvgLegend> _legends = [];
    private SvgPath? _hoveredSegment;
    protected (double x, double y, string label, double value)? _hoveredDot = null;
    protected int? _selectedSeriesIndex = null;
    protected double CalculatedRadius => Math.Round(Math.Min(_boundWidth, _boundHeight) / 2);
    protected abstract string ChartClass { get; }

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
        _hoveredSegment = null;
        _hoveredDot = null;
        _selectedSeriesIndex = null;

        if (ChartSeries == null || ChartSeries.Count == 0)
            return;

        RebuildChart();
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
    protected double[] AggregateSeriesData()
    {
        if (ChartSeries == null || ChartSeries.Count == 0)
            return [];

        var categories = ChartSeries.Max(series => series.Data.Values.Length);
        var aggregated = new double[categories];

        foreach (var series in ChartSeries)
        {
            for (var i = 0; i < series.Data.Values.Length; i++)
            {
                aggregated[i] += series.Data.Values[i];
            }
        }

        return aggregated;
    }

    internal void OnSegmentMouseOver(MouseEventArgs _, SvgPath segment)
    {
        _hoveredSegment = segment;
    }

    internal void OnSegmentMouseOut(MouseEventArgs _)
    {
        _hoveredSegment = null;
    }

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

        RebuildChart();
        StateHasChanged();
    }

    protected abstract void RebuildChart();

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
