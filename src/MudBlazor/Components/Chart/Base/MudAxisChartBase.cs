using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities.Debounce;

#nullable enable
namespace MudBlazor.Charts;

public abstract class MudAxisChartBase<TOptions> : MudChartBase<TOptions>, IDisposable where TOptions : IAxisChartOptions
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The chart, if any, containing this component.
    /// </summary>
    [CascadingParameter]
    public MudChart? MudChartParent { get; set; }

    protected List<ChartDataSet> Series { get; set; } = [];

    protected readonly List<SvgPath> HorizontalLines = [];
    protected readonly List<SvgText> HorizontalValues = [];

    protected readonly List<SvgPath> VerticalLines = [];
    protected readonly List<SvgText> VerticalValues = [];

    protected readonly List<SvgLegend> Legends = [];

    protected const double Epsilon = 1e-6;
    protected const double BoundWidthDefault = 700.0;
    protected const double BoundHeightDefault = 350.0;
    protected const double HorizontalStartSpaceBuffer = 10.0;
    protected double HorizontalStartSpace => Math.Max(HorizontalStartSpaceBuffer + Math.Ceiling(_yAxisLabelSize?.Width ?? 0), 30);
    protected const double HorizontalEndSpace = 30.0;
    protected const double VerticalStartSpaceBuffer = 10.0;
    protected double VerticalStartSpace => Math.Max(VerticalStartSpaceBuffer + (_xAxisLabelSize?.Height ?? 0), 30);
    protected const double VerticalEndSpace = 25.0;
    protected double XAxisLabelOffset => Math.Ceiling(_xAxisLabelSize?.Height ?? 20) / 2;

    protected double _boundWidth = BoundWidthDefault;
    protected double _boundHeight = BoundHeightDefault;
    private ElementSize? _elementSize;
    protected ElementSize? _yAxisLabelSize;
    protected ElementSize? _xAxisLabelSize;

    private readonly DotNetObjectReference<MudAxisChartBase<TOptions>> _dotNetObjectReference;
    protected ElementReference _elementReference;
    protected ElementReference? _xAxisGroupElementReference;
    protected ElementReference? _yAxisGroupElementReference;

    private readonly DebounceDispatcher _debouncer = new(DebounceIntervalMs);
    private const int DebounceIntervalMs = 200;

    [DynamicDependency(nameof(OnElementSizeChanged))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElementSize))]
    protected MudAxisChartBase()
    {
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        RebuildChart();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, _elementReference);

            OnElementSizeChanged(elementSize);
            return;
        }

        var yAxisLabelSize = _yAxisGroupElementReference != null ? await JsRuntime.InvokeAsync<ElementSize>("mudGetSvgBBox", _yAxisGroupElementReference) : null;
        var xAxisLabelSize = _xAxisGroupElementReference != null ? await JsRuntime.InvokeAsync<ElementSize>("mudGetSvgBBox", _xAxisGroupElementReference) : null;

        var axisChanged = false;
        var comparer = new DoubleEpsilonEqualityComparer(0.01);
        if (yAxisLabelSize != null && (_yAxisLabelSize == null || !comparer.Equals(yAxisLabelSize.Width, _yAxisLabelSize.Width)))
        {
            _yAxisLabelSize = yAxisLabelSize;
            axisChanged = true;
        }

        if (xAxisLabelSize != null && (_xAxisLabelSize == null || !comparer.Equals(xAxisLabelSize.Height, _xAxisLabelSize.Height)))
        {
            _xAxisLabelSize = xAxisLabelSize;
            axisChanged = true;
        }

        // maybe there should be some kind of cancellation token here to prevent multiple rebuilds when the invokeasync takes time in server mode and subsequent renders have started to take place
        if (axisChanged)
        {
            RebuildChart();
            StateHasChanged();
        }
    }

    protected void SetBounds()
    {
        _boundWidth = BoundWidthDefault;
        _boundHeight = BoundHeightDefault;

        if (MudChartParent is not null && MudChartParent.MatchBoundsToSize) // backwards compatibilitly to the mudchartparent approach
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

    [JSInvokable]
    public void OnElementSizeChanged(ElementSize elementSize)
    {
        if (elementSize == null || elementSize.Timestamp <= _elementSize?.Timestamp)
            return;

        _elementSize = elementSize;

        if (MudChartParent?.MatchBoundsToSize is not true)
            return;

        if (Math.Abs(_boundWidth - _elementSize.Width) < Epsilon &&
            Math.Abs(_boundHeight - _elementSize.Height) < Epsilon)
        {
            return;
        }

        // Debounce the chart update logic
        _ = _debouncer.DebounceAsync(async () =>
        {
            await InvokeAsync(() =>
            {
                RebuildChart();
                StateHasChanged();
            });
        });
    }

    protected abstract void RebuildChart();

    protected virtual void HandleLegendVisibilityChanged(SvgLegend legend)
    {
        var series = Series[legend.Index];
        series.Visible = legend.Visible;
        RebuildChart();
    }

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
