// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services.UserPreferences;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase
    {
        [Inject] private IUserPreferencesService UserPreferencesService { get; set; }
        private MudTheme _currentTheme;
        private bool _isDarkMode;
        private UserPreferences _userPreferences;
        private MudThemeProvider _mudThemeProvider;
        
        protected override void OnInitialized()
        {
            _currentTheme = Theme.LandingPageTheme();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadUserPreferences();
            }
        }

        private async Task DarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _userPreferences.DarkTheme = _isDarkMode;
            await UserPreferencesService.SaveUserPreferences(_userPreferences);
        }
        
        private async Task LoadUserPreferences()
        {
            _userPreferences = await UserPreferencesService.LoadUserPreferences();
            if (_userPreferences != null)
            {
                _isDarkMode = _userPreferences.DarkTheme;
                StateHasChanged();
            }
            else
            {
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                StateHasChanged();
                _userPreferences = new UserPreferences {DarkTheme = _isDarkMode};
                await UserPreferencesService.SaveUserPreferences(_userPreferences);
            }
        }
    }
}
