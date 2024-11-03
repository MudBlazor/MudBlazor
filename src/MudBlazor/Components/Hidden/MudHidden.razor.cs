using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

namespace MudBlazor
{
#nullable enable
    public partial class MudHidden : MudComponentBase, IBrowserViewportObserver, IAsyncDisposable
    {
        private readonly ParameterState<bool> _hiddenState;
        private bool _serviceIsReady = false;
        private Breakpoint _currentBreakpoint = Breakpoint.None;

        [Inject]
        protected IBrowserViewportService BrowserViewportService { get; set; } = null!;

        [CascadingParameter]
        public Breakpoint CurrentBreakpointFromProvider { get; set; } = Breakpoint.None;

        /// <summary>
        /// The screen size(s) depending on which the ChildContent should not be rendered (or should be, if Invert is true)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public Breakpoint Breakpoint { get; set; }

        /// <summary>
        /// Inverts the Breakpoint, so that the ChildContent is only rendered when the breakpoint matches the screen size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public bool Invert { get; set; }

        /// <summary>
        /// True if the component is hidden (two-way bindable)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Fires when the breakpoint changes visibility of the component
        /// </summary>
        [Parameter]
        public EventCallback<bool> HiddenChanged { get; set; }

        /// <summary>
        /// Child content of component.
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
