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
                await LoadUserPreferences();
            }
        }

        private async Task LoadUserPreferences()
        {
            _userPreferences = await UserPreferencesService.LoadUserPreferences();
            if (_userPreferences != null)
            {
                _rightToLeft = _userPreferences.RightToLeft;
                StateHasChanged();
            }
            else
            {
                _userPreferences = new UserPreferences();
            }
        }

    }
}
