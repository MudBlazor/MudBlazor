using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{

    public partial class MudHidden : MudComponentBase, IAsyncDisposable
    {
        private Breakpoint _currentBreakpoint = Breakpoint.None;
        private bool _serviceIsReady = false;
        private Guid _breakpointServiceSubscriptionId;

        [Inject] public IBreakpointService BreakpointService { get; set; }

        [CascadingParameter] public Breakpoint CurrentBreakpointFromProvider { get; set; } = Breakpoint.None;

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

        private bool _isHidden = true;

        /// <summary>
        /// True if the component is not visible (two-way bindable)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (_isHidden != value)
                {
                    _isHidden = value;
                    IsHiddenChanged.InvokeAsync(_isHidden);

                }
            }
        }

        /// <summary>
        /// Fires when the breakpoint changes visibility of the component
        /// </summary>
        [Parameter] public EventCallback<bool> IsHiddenChanged { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Hidden.Behavior)]
        public RenderFragment ChildContent { get; set; }

        protected void Update(Breakpoint currentBreakpoint)
        {
            if (CurrentBreakpointFromProvider != Breakpoint.None)
            {
                currentBreakpoint = CurrentBreakpointFromProvider;
            }
            else if (_serviceIsReady == false) { return; }

            if (currentBreakpoint == Breakpoint.None) { return; }

            _currentBreakpoint = currentBreakpoint;

            var hidden = BreakpointService.IsMediaSize(Breakpoint, currentBreakpoint);
            if (Invert == true)
            {
                hidden = !hidden;
            }

            IsHidden = hidden;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Update(_currentBreakpoint);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender == true)
            {
                if (CurrentBreakpointFromProvider == Breakpoint.None)
                {
                    var attachResult = await BreakpointService.SubscribeAsync((x) =>
                    {
                        Update(x);
                        InvokeAsync(StateHasChanged);
                    });

                    _serviceIsReady = true;
                    _breakpointServiceSubscriptionId = attachResult.SubscriptionId;
                    Update(attachResult.Breakpoint);
                    StateHasChanged();
                }
                else
                {
                    _serviceIsReady = true;
                }
            }
        }

        public async ValueTask DisposeAsync() => await BreakpointService.UnsubscribeAsync(_breakpointServiceSubscriptionId);
    }
}
