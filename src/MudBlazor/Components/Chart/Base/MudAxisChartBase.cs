using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities.Debounce;

#nullable enable
namespace MudBlazor.Charts;

/// <summary>
/// Serves as the base class for axis-based charts, providing core functionality for rendering and managing chart
/// elements such as axes, grids, and legends.
/// </summary>
/// <remarks>
/// Provides foundational features for axis-based charts, including support for shared data
/// regions, overlay charts, and dynamic resizing.
/// </remarks>
/// <typeparam name="TOptions">The type of chart options used to configure the chart's behavior and appearance.</typeparam>
public abstract class MudAxisChartBase<TOptions> : MudChartBase<TOptions>, IMudAxisChart, IDisposable where TOptions : IAxisChartOptions
{
    [Inject]
    private IJSRuntime JsRuntime { get; init; } = null!;

    /// <summary>
    /// Gets the rectangle that defines the plot area of the chart.
    /// </summary>
    /// <remarks>
    /// The plot area is the region of the chart where data is visualized, excluding axes, labels, and other chart elements.
    /// Values are shared between the overlay and the main chart.
    /// </remarks>
    [CascadingParameter]
    public AxisGridData? SharedData { get; set; }

    /// <summary>
    /// The chart, if any, containing this component.
    /// </summary>
    [CascadingParameter]
    public MudChart? ChartContainer { get; set; }

    public IMudChart? OverlayChart { get; set; }
    public bool IsOverlayChart => ChartReference is IMudAxisChart;

    protected List<ChartSeries> Series { get; set; } = [];

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
    public override string[] LegendPalette => [.. (ChartOptions?.ChartPalette ?? []), .. OverlayChart?.LegendPalette ?? []];
    public abstract RenderFragment? OverlayContent { get; set; }

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

        if (MatchBoundsToSize is true && _elementSize is null) return;

        RebuildChart();
    }

    protected async Task SetElementReference(ElementReference elementRef)
    {
        var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, elementRef);

        OnElementSizeChanged(elementSize);
    }

    protected void AxisChanged()
    {
        _ = _debouncer.DebounceAfterFirstExecuteAsync(async () =>
        {
            await InvokeAsync(() =>
            {
                RebuildChart();
                StateHasChanged();
            });
        });
    }

    protected void SetBounds()
    {
        if (ChartReference is IMudAxisChart chart && chart.SharedData is { } data)
        {
            _boundWidth = data.BoundWidth;
            _boundHeight = data.BoundHeight;
            return;
        }

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
                && double.TryParse(Width.AsSpan(0, Width.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out var width)
                && double.TryParse(Height.AsSpan(0, Height.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out var height))
            {
                _boundWidth = width;
                _boundHeight = height;
            }
        }
    }

    protected void GenerateLegends()
    {
        Legends.Clear();
        for (var i = 0; i < Series.Count; i++)
        {
            var series = Series[i];
            var legend = new SvgLegend()
            {
                Index = i,
                Labels = series.Name,
                Visible = series.Visible,
                OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
            };
            Legends.Add(legend);
        }

        if (OverlayChart is IMudAxisChart overlay)
        {
            for (var i = 0; i < overlay.ChartSeries.Count; i++)
            {
                var series = overlay.ChartSeries[i];
                var legend = new SvgLegend()
                {
                    Index = Series.Count + i,
                    Labels = series.Name,
                    Visible = series.Visible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, args => HandleOverlayChartLegendVisibility(overlay, i, args.Visible))
                };
                Legends.Add(legend);
            }
        }
    }

    [JSInvokable]
    public void OnElementSizeChanged(ElementSize elementSize)
    {
        if (elementSize is null || elementSize.Timestamp <= _elementSize?.Timestamp)
            return;

        _elementSize = new ElementSize()
        {
            Height = elementSize.Height,
            Width = Math.Max(0, elementSize.Width - 50),
            Timestamp = elementSize.Timestamp
        };

        if (ChartContainer?.MatchBoundsToSize is not true)
            return;

        if (Math.Abs(_boundWidth - _elementSize.Width) < Epsilon &&
            Math.Abs(_boundHeight - _elementSize.Height) < Epsilon)
        {
            return;
        }

        // Debounce the chart update logic
        _ = _debouncer.DebounceAfterFirstExecuteAsync(async () =>
        {
            await InvokeAsync(() =>
            {
                RebuildChart();
                StateHasChanged();
            });
        });
    }

    protected virtual void HandleLegendVisibilityChanged(SvgLegend legend)
    {
        var series = Series[legend.Index];
        series.Visible = legend.Visible;
        RebuildChart();
    }

    protected void HandleOverlayChartLegendVisibility(IMudChart overlayChart, int index, bool isVisible)
    {
        if (overlayChart?.ChartSeries != null && index >= 0 && index < overlayChart.ChartSeries.Count)
        {
            overlayChart.ChartSeries[index].Visible = isVisible;
            overlayChart.RebuildChart();
            StateHasChanged();
        }
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
