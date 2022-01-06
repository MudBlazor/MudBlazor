// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase, IDisposable
    {
        [Inject] protected LayoutService LayoutService { get; set; }
        
        protected override void OnInitialized()
        {
            LayoutService.SetBaseTheme(Theme.LandingPageTheme());
            
            LayoutService.CloseDrawerRequested += LayoutServiceOnCloseDrawerRequested;
            LayoutService.OpenDrawerRequested += LayoutServiceOnOpenDrawerRequested;
        
            base.OnInitialized();
        }
        
        private void LayoutServiceOnOpenDrawerRequested(object sender, EventArgs e) => StateHasChanged();
        private void LayoutServiceOnCloseDrawerRequested(object sender, EventArgs e) => StateHasChanged();


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
}
