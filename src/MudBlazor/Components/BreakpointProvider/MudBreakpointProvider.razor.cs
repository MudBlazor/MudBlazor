// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a cascading parameter which exposes the window's current breakpoint (xs, sm, md, lg, xl).
    /// </summary>
    public partial class MudBreakpointProvider : IBrowserViewportObserver, IAsyncDisposable
    {
        /// <summary>
        /// The current breakpoint.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Breakpoint.Always"/>.
        /// </remarks>
        public Breakpoint Breakpoint { get; private set; } = Breakpoint.Always;

        /// <summary>
        /// Occurs when <see cref="Breakpoint"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<Breakpoint> OnBreakpointChanged { get; set; }

        [Inject]
        protected IBrowserViewportService BrowserViewportService { get; set; } = null!;

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.BreakpointProvider.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
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
            Breakpoint = browserViewportEventArgs.Breakpoint;
            await OnBreakpointChanged.InvokeAsync(browserViewportEventArgs.Breakpoint);
            await InvokeAsync(StateHasChanged);
        }
    }
}
