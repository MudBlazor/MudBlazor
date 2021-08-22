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
    public partial class BreakpointProvider : IAsyncDisposable
    {
        public Breakpoint Breakpoint { get; private set; } = Breakpoint.None;

        [Parameter] public EventCallback<Breakpoint> OnBreakpointChanged { get; set; }

        [Inject] public IBreakpointListenerService Service { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender == true)
            {
               Breakpoint =  await Service.Attach(SetBreakpointCallback, new ResizeOptions());
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

        public bool IsMediaSize(Breakpoint breakpoint) => Service.IsMediaSize(breakpoint, Breakpoint);

        public async ValueTask DisposeAsync() => await Service.DisposeAsync();
    }
}
