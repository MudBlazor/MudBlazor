// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared;

public partial class DocsMainLayout : LayoutComponentBase
{
    [Inject] private LayoutService LayoutService { get; set; }
    protected override void OnInitialized()
    {
        LayoutService.SetDrawer(true);
        LayoutService.SetBaseTheme(Theme.DocsTheme());
    }
    

}
