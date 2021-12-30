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
        [Inject] private IUserPreferencesService UserPreferencesService { get; set; }
        
        internal bool _rightToLeft;
        internal bool _isDarkMode;
        private UserPreferences _userPreferences;
        private MudThemeProvider _mudThemeProvider;
        private MudTheme _currentTheme;
        
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
                _isDarkMode = _userPreferences.DarkTheme;
                _rightToLeft = _userPreferences.RightToLeft;
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
        
        internal async Task DarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _userPreferences.DarkTheme = _isDarkMode;
            await UserPreferencesService.SaveUserPreferences(_userPreferences);
        }
        
        internal async Task RightToLeft()
        {
            _rightToLeft = !_rightToLeft;
            _userPreferences.RightToLeft = _rightToLeft;
            await UserPreferencesService.SaveUserPreferences(_userPreferences);
        }
        
        internal void SetBaseTheme(MudTheme theme)
        {
            _currentTheme = theme;
            StateHasChanged();
        }
    }
}
