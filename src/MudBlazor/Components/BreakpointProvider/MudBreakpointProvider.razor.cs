// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;

namespace MudBlazor
{
    public partial class MudBreakpointProvider : IAsyncDisposable
    {
        private Guid _breakPointListenerSubscriptionId;

        public Breakpoint Breakpoint { get; private set; } = Breakpoint.Always;

        [Parameter] public EventCallback<Breakpoint> OnBreakpointChanged { get; set; }

        [Inject] public IBreakpointService Service { get; set; }

        [Parameter]
        [Category(CategoryTypes.BreakpointProvider.Behavior)]
        public RenderFragment ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender == true)
            {
                var attachResult = await Service.Subscribe(SetBreakpointCallback);
                _breakPointListenerSubscriptionId = attachResult.SubscriptionId;
                Breakpoint = attachResult.Breakpoint;
                await OnBreakpointChanged.InvokeAsync(Breakpoint);
                StateHasChanged();
            }
        }

        private void SetBreakpointCallback(Breakpoint breakpoint)
        {
            InvokeAsync(() =>
            {
                Breakpoint = breakpoint;
                OnBreakpointChanged.InvokeAsync(breakpoint);
                StateHasChanged();
            }).AndForget();
        }

        public async ValueTask DisposeAsync() => await Service.Unsubscribe(_breakPointListenerSubscriptionId);
    }
}
