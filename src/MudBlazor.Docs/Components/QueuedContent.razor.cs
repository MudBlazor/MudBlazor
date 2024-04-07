// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Components
{
    public partial class QueuedContent : ComponentBase, IAsyncDisposable
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Inject] IRenderQueueService RenderQueue { get; set; }

        private bool AllowRender { get; set; }
        public bool IsDisposed { get; set; }
        public bool IsRendered { get; set; }
        public EventCallback Rendered { get; set; }
        public EventCallback Disposed { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RenderQueue.Enqueue(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (IsDisposed)
                return;
            if (AllowRender && !IsRendered)
            {
                IsRendered = true;
                await Rendered.InvokeAsync(this);
            }
        }

        public async ValueTask RenderAsync()
        {
            if (IsDisposed)
                return;
            AllowRender = true;
            await InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            IsDisposed = true;
            if (!AllowRender)
                await Disposed.InvokeAsync(this);
            GC.SuppressFinalize(this);
        }
    }
}
