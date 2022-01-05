using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services.UserPreferences;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] private IUserPreferencesService UserPreferencesService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        
        internal bool _rightToLeft;
        internal bool _isDarkMode;
        private UserPreferences _userPreferences;
        private MudThemeProvider _mudThemeProvider;
        private MudTheme _currentTheme;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ApplyUserPreferences();
                StateHasChanged();
            }
        }

        private async Task ApplyUserPreferences()
        {
            _userPreferences = await UserPreferencesService.LoadUserPreferences();
            if (_userPreferences != null)
            {
                _isDarkMode = _userPreferences.DarkTheme;
                _rightToLeft = _userPreferences.RightToLeft;
            }
            else
            {
                _isDarkMode = await _mudThemeProvider.GetSystemPreference();
                _userPreferences = new UserPreferences {DarkTheme = _isDarkMode};
                await UserPreferencesService.SaveUserPreferences(_userPreferences);
            }
        }
        
        internal async Task DarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _userPreferences.DarkTheme = _isDarkMode;
            await UserPreferencesService.SaveUserPreferences(_userPreferences);
            StateHasChanged();
        }
        
        internal async Task RightToLeft()
        {
            _rightToLeft = !_rightToLeft;
            _userPreferences.RightToLeft = _rightToLeft;
            await UserPreferencesService.SaveUserPreferences(_userPreferences);
            StateHasChanged();
        }
        
        internal void SetBaseTheme(MudTheme theme)
        {
            _currentTheme = theme;
            StateHasChanged();
        }

        internal DocsBasePage GetDocsBasePage()
        {
            if (NavigationManager.Uri.Contains("/api/") || NavigationManager.Uri.Contains("/components/"))
            {
                return DocsBasePage.Docs;
            }
            else if (NavigationManager.Uri.Contains("/getting-started/"))
            {
                return DocsBasePage.GettingStarted;
            }
            else if (NavigationManager.Uri.Contains("/mud/"))
            {
                return DocsBasePage.DiscoverMore;
            }
            else
            {
                return DocsBasePage.None;
            }
        }
    }
}
