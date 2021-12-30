// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.UserPreferences;

namespace MudBlazor.Docs.Shared
{

    public partial class DocsLayout : LayoutComponentBase
    {
        [CascadingParameter] private MainLayout MainData { get; set; }
        private bool _drawerOpen = true;
        private NavMenu _navMenuRef;
        
        protected override void OnInitialized()
        {
            MainData.SetBaseTheme(Theme.DocsTheme());
        }
        
        protected override void OnAfterRender(bool firstRender)
        {
            //refresh nav menu because no parameters change in nav menu
            //but internal data does
            _navMenuRef.Refresh();
        }
    }
}
