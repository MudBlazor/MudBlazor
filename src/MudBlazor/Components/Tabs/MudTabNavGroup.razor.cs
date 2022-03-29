// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudTabNavGroup : MudTabPanel
{

    [Parameter]
    [Category(CategoryTypes.NavMenu.Behavior)]
    public string Title { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.General.ClickAction)]
    public string LinkBase
    {
        get => _linkBase;
        set
        {
            _linkBase = value.TrimStart('/');
        }
    }

    [Parameter] 
    [Category("Behavior")] 
    public MouseEvent ActivationEvent { get; set; }
    
    private readonly List<MudTabNavLink> _panels = new();
    private string _linkBase;
    public IEnumerable<MudTabNavLink> Panels => _panels;
    
    internal void AddPanel(MudTabNavLink tabPanel)
    {
        _panels.Add(tabPanel);
        StateHasChanged();
    }

    internal void RemovePanel(MudTabNavLink tabPanel)
    {
        _panels.Remove(tabPanel);
        StateHasChanged();
    }
}
