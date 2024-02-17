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

    private NavMenu _navMenuRef;
    private bool _drawerOpen = true;
    private bool _topMenuOpen = false;
    protected override void OnInitialized()
    {
        LayoutService.SetBaseTheme(Theme.DocsTheme());
    }

    protected override void OnAfterRender(bool firstRender)
    {
        //refresh nav menu because no parameters change in nav menu but internal data does
        _navMenuRef?.Refresh();
    }

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void OpenTopMenu()
    {
        _topMenuOpen = true;
    }

    private void OnDrawerOpenChanged(bool value)
    {
        _topMenuOpen = false;
        _drawerOpen = value;
        StateHasChanged();
    }

}
