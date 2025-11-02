// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase, IAsyncDisposable
    {
        [Inject] protected LayoutService LayoutService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        private bool _drawerOpen = false;
        private Appbar _appbarRef;
        private DotNetObjectReference<LandingLayout> _dotNetRef;
        private IJSObjectReference _jsModule;

        protected override void OnInitialized()
        {
            LayoutService.SetBaseTheme(Theme.LandingPageTheme());

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/MudBlazor.Docs/JS/keyboard-shortcuts.js");
                await _jsModule.InvokeVoidAsync("registerSearchShortcut", _dotNetRef);
            }
        }

        [JSInvokable]
        public async Task OnSearchShortcut()
        {
            if (_appbarRef != null)
            {
                await _appbarRef.ActivateSearchAsync();
            }
        }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        public async ValueTask DisposeAsync()
        {
            if (_jsModule != null)
            {
                await _jsModule.InvokeVoidAsync("unregisterSearchShortcut");
                await _jsModule.DisposeAsync();
            }
            _dotNetRef?.Dispose();
        }
    }
}
