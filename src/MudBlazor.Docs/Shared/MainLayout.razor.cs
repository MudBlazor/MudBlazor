using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.UserPreferences;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        private bool _rightToLeft;
        
        private UserPreferences _userPreferences;
        
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IUserPreferencesService UserPreferencesService { get; set; }

        

        private async Task RightToLeftToggle()
        {
            _rightToLeft = !_rightToLeft;
            _userPreferences.RightToLeft = _rightToLeft;
            await UserPreferencesService.SaveUserPreferences(_userPreferences);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _userPreferences = await UserPreferencesService.LoadUserPreferences();
                _rightToLeft = _userPreferences.RightToLeft;
                StateHasChanged();
            }
        }
        
        private void SwitchToServer()
        {
            NavigationManager.NavigateTo(NavigationManager.Uri.Replace("wasm/", string.Empty), forceLoad: true);
        }

        private void SwitchToWasm()
        {
            NavigationManager.NavigateTo(NavigationManager.Uri.Replace(
                NavigationManager.BaseUri,
                NavigationManager.BaseUri + "wasm/" + NavigationManager.ToBaseRelativePath(NavigationManager.BaseUri))
                , forceLoad: true);
        }

        private bool Wasm => NavigationManager.Uri.Contains("wasm");

        
    }
}
