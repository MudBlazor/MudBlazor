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
        private bool _drawerOpen = true;
        private NavMenu _navMenuRef;
        private bool _isDarkMode;
        private MudTheme _currentTheme;
        private UserPreferences _userPreferences;
        private MudThemeProvider _mudThemeProvider;

        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject] private IApiLinkService ApiLinkService { get; set; }

        [Inject] private IUserPreferencesService UserPreferencesService { get; set; }

        MudAutocomplete<ApiLinkServiceEntry> _searchAutocomplete;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //refresh nav menu because no parameters change in nav menu
            //but internal data does
            _navMenuRef.Refresh();
        }

        private Task<IEnumerable<ApiLinkServiceEntry>> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // the user just clicked the autocomplete open, show the most popular pages as search result according to our analytics data
                // ordered by popularity
                return Task.FromResult<IEnumerable<ApiLinkServiceEntry>>(new[]
                {
                    new ApiLinkServiceEntry
                    {
                        Title = "Installation",
                        Link = "getting-started/installation",
                        SubTitle = "Getting started with MudBlazor fast and easy."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Wireframes",
                        Link = "getting-started/wireframes",
                        SubTitle =
                            "These small templates can be copied directly or just be used for inspiration."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Table",
                        Link = "components/table",
                        ComponentType = typeof(MudTable<T>),
                        SubTitle = "A sortable, filterable table with multiselection and pagination."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Grid",
                        Link = "components/grid",
                        ComponentType = typeof(MudGrid),
                        SubTitle =
                            "The grid component helps keeping layout consistent across various screen resolutions and sizes."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Button",
                        Link = "components/button",
                        ComponentType = typeof(MudGrid),
                        SubTitle = "A Material Design button for triggering an action or navigating to a link."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Card",
                        Link = "components/card",
                        ComponentType = typeof(MudCard),
                        SubTitle = "Cards can contain actions, text, or media like images or graphics."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Dialog",
                        Link = "components/dialog",
                        ComponentType = typeof(MudDialog),
                        SubTitle =
                            "A dialog will overlay your current app content, providing the user with either information, a choice, or other tasks."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "App Bar",
                        Link = "components/appbar",
                        ComponentType = typeof(MudAppBar),
                        SubTitle = "App bar is used to display actions, branding, navigation and screen titles."
                    },
                    new ApiLinkServiceEntry
                    {
                        Title = "Navigation Menu",
                        Link = "components/navmenu",
                        ComponentType = typeof(MudNavMenu),
                        SubTitle = "Nav menu provides a tree-like menu linking to the content on your site."
                    },
                });
            }

            return ApiLinkService.Search(text);
        }

        private async void OnSearchResult(ApiLinkServiceEntry entry)
        {
            NavigationManager.NavigateTo(entry.Link);
            await Task.Delay(1000);
            await _searchAutocomplete.Clear();
        }

        protected override void OnInitialized()
        {
            _currentTheme = Theme.DocsTheme();
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
