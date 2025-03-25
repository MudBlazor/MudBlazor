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
        protected const double HorizontalStartSpace = 30.0;
        protected const double HorizontalEndSpace = 30.0;
        protected const double VerticalStartSpace = 25.0;
        protected const double VerticalEndSpace = 25.0;

        protected const double BoundWidthDefault = 650.0;
        protected const double BoundHeightDefault = 350.0;
        protected double _boundWidth = 650.0;
        protected double _boundHeight = 350.0;
        private ElementSize _elementSize = new() { Width = BoundWidthDefault, Height = BoundHeightDefault };

        private readonly DotNetObjectReference<MudCategoryAxisChartBase> _dotNetObjectReference;
        protected ElementReference _elementReference = new();

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
                _elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, _elementReference);

                OnElementSizeChanged(_elementSize);
            }
        }

        protected virtual void SetBounds()
        {
            _boundWidth = BoundWidthDefault;
            _boundHeight = BoundHeightDefault;

#pragma warning disable CS0618
            if (MudChartParent != null && AxisChartOptions.MatchBoundsToSize)
            {
                _boundWidth = _elementSize.Width;
                _boundHeight = _elementSize.Height;
            }
#pragma warning restore CS0618
        }

        [JSInvokable]
        public void OnElementSizeChanged(ElementSize elementSize)
        {
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
