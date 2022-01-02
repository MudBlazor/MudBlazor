// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase
    {
        [CascadingParameter] private MainLayout MainData { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await MainData.SetBaseTheme(Theme.DocsTheme());
        }
    }
}
