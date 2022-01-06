// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared;

public partial class Drawer : IDisposable
{
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Inject] protected LayoutService LayoutService { get; set; }

    private bool _topMenuOpen = false;
    
    private void OpenTopMenu()
    {
        _topMenuOpen = true;
    }
    
    protected override void OnInitialized()
    {
        LayoutService.CloseDrawerRequested += LayoutServiceOnCloseDrawerRequested;
        LayoutService.OpenDrawerRequested += LayoutServiceOnOpenDrawerRequested;
        
        base.OnInitialized();
    }

    private void LayoutServiceOnOpenDrawerRequested(object sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void LayoutServiceOnCloseDrawerRequested(object sender, EventArgs e)
    {
        if (_topMenuOpen == true)
        {
            _topMenuOpen = false;
        }
        
        StateHasChanged();
    }


    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            LayoutService.CloseDrawerRequested -= LayoutServiceOnCloseDrawerRequested;
            LayoutService.OpenDrawerRequested -= LayoutServiceOnOpenDrawerRequested;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
