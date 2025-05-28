using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;

#nullable enable
namespace MudBlazor
{
    public abstract class MudCategoryAxisChartBase : MudCategoryChartBase, IDisposable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        private const double Epsilon = 1e-6;
        protected const double BoundWidthDefault = 650.0;
        protected const double BoundHeightDefault = 350.0;
        protected const double HorizontalStartSpaceBuffer = 10.0;
        protected double HorizontalStartSpace => Math.Max(HorizontalStartSpaceBuffer + Math.Ceiling(_yAxisLabelSize?.Width ?? 0), 30);
        protected const double HorizontalEndSpace = 30.0;
        protected const double VerticalStartSpaceBuffer = 10.0;
        protected double VerticalStartSpace => Math.Max(VerticalStartSpaceBuffer + (_xAxisLabelSize?.Height ?? 0), 30);
        protected const double VerticalEndSpace = 25.0;
        protected double XAxisLabelOffset => Math.Ceiling(_xAxisLabelSize?.Height ?? 20) / 2;

        protected double _boundWidth = 650.0;
        protected double _boundHeight = 350.0;
        private ElementSize? _elementSize;
        private ElementSize? _yAxisLabelSize;
        private ElementSize? _xAxisLabelSize;

        private readonly DotNetObjectReference<MudCategoryAxisChartBase> _dotNetObjectReference;
        protected ElementReference _elementReference;
        protected ElementReference? _xAxisGroupElementReference;
        protected ElementReference? _yAxisGroupElementReference;

        [DynamicDependency(nameof(OnElementSizeChanged))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ElementSize))]
        protected MudCategoryAxisChartBase()
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, _elementReference);

                OnElementSizeChanged(elementSize);
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

            if (MudChartParent != null && (MudChartParent.AxisChartOptions.MatchBoundsToSize)) // backwards compatibilitly to the mudchartparent approach
            {
                if (_elementSize != null)
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

            if (!AxisChartOptions.MatchBoundsToSize)
            {
                return;
            }

            if (Math.Abs(_boundWidth - _elementSize.Width) < Epsilon &&
                Math.Abs(_boundHeight - _elementSize.Height) < Epsilon)
            {
                return;
            }

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
}
