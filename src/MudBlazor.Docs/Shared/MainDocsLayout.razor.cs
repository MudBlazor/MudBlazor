// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Microsoft.AspNetCore.Components;
namespace MudBlazor.Docs.Shared;

public partial class MainDocsLayout : LayoutComponentBase
{
    [CascadingParameter] private MainLayout MainData { get; set; }
    
    protected override void OnInitialized()
    {
        MainData.SetBaseTheme(Theme.DocsTheme());
    }
}
