// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared;

public partial class DocsLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject] private LayoutService LayoutService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }

    private NavMenu _navMenuRef;
    private Appbar _appbarRef;
    private bool _drawerOpen = true;
    private bool _topMenuOpen = false;
    private DotNetObjectReference<DocsLayout> _dotNetRef;
    private IJSObjectReference _jsModule;

    protected override void OnInitialized()
    {
        LayoutService.SetBaseTheme(Theme.DocsTheme());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //refresh nav menu because no parameters change in nav menu but internal data does
        _navMenuRef?.Refresh();

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

    private void OnDrawerOpenChanged(bool value)
    {
        _topMenuOpen = false;
        _drawerOpen = value;
        StateHasChanged();
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
