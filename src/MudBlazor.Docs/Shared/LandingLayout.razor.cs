// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase
    {
        [Inject] protected LayoutService LayoutService { get; set; }

        private bool _drawerOpen = false;

        protected override void OnInitialized()
        {
            LayoutService.SetBaseTheme(Theme.LandingPageTheme());

            base.OnInitialized();
        }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

    }
}
