// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Interfaces;

namespace MudBlazor;

public partial class MudTabNavLink : MudTabPanel
{
    private bool _disposed;

    [Parameter]
    [Category(CategoryTypes.General.ClickAction)]
    public string Href { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Parent?.SetPanelRef(PanelRef);
        }
    }

    protected override void OnInitialized()
    {
        // NOTE: we must not throw here because we need the component to be able to live for the API docs to be able to infer default values
        //if (Parent == null)
        //    throw new ArgumentNullException(nameof(Parent), "TabPanel must exist within a Tabs component");
        // base.OnInitialized();


        Parent?.AddPanel(this);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        await (Parent?.RemovePanel(this) ?? Task.CompletedTask);
    }
}
