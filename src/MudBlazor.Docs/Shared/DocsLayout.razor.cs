// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared;

public partial class DocsLayout : LayoutComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IDocsJsApiService DocsJsApi { get; set; }

    private NavMenu _navMenuRef;
    private bool _drawerOpen = true;
    private bool _topMenuOpen = false;
    protected override void OnInitialized()
    {
        LayoutService.SetBaseTheme(Theme.DocsTheme());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _navMenuRef?.Refresh();
        await DocsJsApi.ScrollToActiveNavLinkAsync();
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
}
