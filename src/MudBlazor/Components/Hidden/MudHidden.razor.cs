using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A component which conditionally renders content depending on the screen size.
    /// </summary>
    /// <remarks>
    /// This component uses JavaScript to listen for browser window size changes.  If you want a solution using only CSS, you can use the <see href="https://mudblazor.com/features/display#class-reference">responsive display classes</see>.
    /// </remarks>
    public partial class MudHidden : MudComponentBase, IBrowserViewportObserver, IAsyncDisposable
    {
        private readonly ParameterState<bool> _hiddenState;
        private bool _serviceIsReady = false;
        private Breakpoint _currentBreakpoint = Breakpoint.None;

        [Inject]
        protected IBrowserViewportService BrowserViewportService { get; set; } = null!;

        /// <summary>
        /// The current breakpoint.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Breakpoint.None"/>.
        /// </remarks>
        [CascadingParameter]
        public Breakpoint CurrentBreakpointFromProvider { get; set; } = Breakpoint.None;

        /// <summary>
        /// The breakpoint at which component is not rendered, when <see cref="Invert"/> is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// When <see cref="Invert"/> is <c>true</c>, this property controls when the content is shown.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public Breakpoint Breakpoint { get; set; }

        /// <summary>
        /// Causes the <see cref="Breakpoint"/> to control when content is displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public bool Invert { get; set; }

        /// <summary>
        /// Hides the content within this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Occurs when <see cref="Hidden"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> HiddenChanged { get; set; }

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        public MudHidden()
        {
            using var registerScope = CreateRegisterScope();
            _hiddenState = registerScope.RegisterParameter<bool>(nameof(Hidden))
                .WithParameter(() => Hidden)
                .WithEventCallback(() => HiddenChanged);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await UpdateAsync(_currentBreakpoint);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (CurrentBreakpointFromProvider == Breakpoint.None)
                {
                    _serviceIsReady = true;
                    await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
                }
                else
                {
                    _serviceIsReady = true;
                }
            }
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (IsJSRuntimeAvailable)
            {
                await BrowserViewportService.UnsubscribeAsync(this);
            }
        }

        Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

        async Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
        {
            await UpdateAsync(browserViewportEventArgs.Breakpoint);
            await InvokeAsync(StateHasChanged);
        }

        protected async Task UpdateAsync(Breakpoint currentBreakpoint)
        {
            if (CurrentBreakpointFromProvider != Breakpoint.None)
            {
                currentBreakpoint = CurrentBreakpointFromProvider;
            }
            else
            {
                if (!_serviceIsReady)
                {
                    return;
                }
            }

            if (currentBreakpoint == Breakpoint.None)
            {
                return;
            }

            _currentBreakpoint = currentBreakpoint;

            var hidden = await BrowserViewportService.IsBreakpointWithinReferenceSizeAsync(Breakpoint, currentBreakpoint);
            if (Invert)
            {
                hidden = !hidden;
            }

            await _hiddenState.SetValueAsync(hidden);
        }
    }
}
