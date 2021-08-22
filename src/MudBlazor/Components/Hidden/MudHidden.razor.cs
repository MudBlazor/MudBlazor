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

        [Inject] public IBreakpointListenerService BreakpointListenerService {  get; set; }

        /// <summary>
        /// The screen size(s) depending on which the ChildContent should not be rendered (or should be, if Invert is true)
        /// </summary>
        [Parameter] public Breakpoint Breakpoint { get; set; }

        /// <summary>
        /// Inverts the Breakpoint, so that the ChildContent is only rendered when the breakpoint matches the screen size.
        /// </summary>
        [Parameter] public bool Invert { get; set; }

        /// <summary>
        /// True if the component is not visible (two-way bindable)
        /// </summary>
        [Parameter]
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
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _isHidden = true;

        protected void Update(Breakpoint currentBreakpoint)
        {
            if (_serviceIsReady == false) { return; }

            _currentBreakpoint = currentBreakpoint;

            var hidden = BreakpointListenerService.IsMediaSize(Breakpoint, currentBreakpoint);
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
                var result = await BreakpointListenerService.Attach((x) => {
                    Update(x);
                    InvokeAsync(StateHasChanged);
                    }, new ResizeOptions());
                _serviceIsReady = true;
                Update(result);
                StateHasChanged();
                
            }
        }

        public async ValueTask DisposeAsync() => await BreakpointListenerService.DisposeAsync();
    }
}
