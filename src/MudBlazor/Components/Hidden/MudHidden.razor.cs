using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{

    public partial class MudHidden : MudComponentBase
    {

        [Inject] IResizeListenerService ResizeListener { get; set; }

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
        [Parameter] public bool IsHidden
        {
            get => _is_hidden;
            set
            {
                if (_is_hidden == value)
                    return;
                _is_hidden = value;
                StateHasChanged();
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

        private bool _is_hidden=true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ResizeListener.OnResized += OnResized;
                Update();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task Update()
        {
            var hidden = await ResizeListener.IsMediaSize(Breakpoint);
            if (Invert)
                hidden = !hidden;
            if (hidden == _is_hidden)
                return;
            _is_hidden = hidden;
            InvokeAsync(StateHasChanged);
            IsHiddenChanged.InvokeAsync(_is_hidden);
        }

        private void OnResized(object sender, BrowserWindowSize size)
        {
            Update();
        }
    }
}
