using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        private bool _drawerOpen = false;
        private bool _rightToLeft = false;
        private NavMenu _navMenuRef;

        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject] private IApiLinkService ApiLinkService { get; set; }

        MudAutocomplete<ApiLinkServiceEntry> _searchAutocomplete;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void RightToLeftToggle()
        {
            _rightToLeft = !_rightToLeft;
        }

        protected override void OnInitialized()
        {
            _currentTheme = _defaultTheme;
            //if not home page, the navbar starts open
            if (!NavigationManager.IsHomePage())
            {
                _drawerOpen = true;
            }
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
                return Task.FromResult<IEnumerable<ApiLinkServiceEntry>>(new[] {
                    new ApiLinkServiceEntry{ Title = "Installation", Link="getting-started/installation", SubTitle="Getting started with MudBlazor fast and easy." },
                    new ApiLinkServiceEntry{ Title = "Wireframes", Link="getting-started/wireframes", SubTitle="These small templates can be copied directly or just be used for inspiration." },
                    new ApiLinkServiceEntry{ Title = "Table", Link ="components/table", ComponentType=typeof(MudTable<T>), SubTitle = "A sortable, filterable table with multiselection and pagination." },
                    new ApiLinkServiceEntry{ Title = "Grid", Link ="components/grid", ComponentType=typeof(MudGrid), SubTitle = "The grid component helps keeping layout consistent across various screen resolutions and sizes." },
                    new ApiLinkServiceEntry{ Title = "Button", Link ="components/button", ComponentType=typeof(MudGrid), SubTitle = "A Material Design button for triggering an action or navigating to a link." },
                    new ApiLinkServiceEntry{ Title = "Card", Link ="components/card", ComponentType=typeof(MudCard), SubTitle = "Cards can contain actions, text, or media like images or graphics." },
                    new ApiLinkServiceEntry{ Title = "Dialog", Link ="components/dialog", ComponentType=typeof(MudDialog), SubTitle = "A dialog will overlay your current app content, providing the user with either information, a choice, or other tasks." },
                    new ApiLinkServiceEntry{ Title = "App Bar", Link ="components/appbar", ComponentType=typeof(MudAppBar), SubTitle = "App bar is used to display actions, branding, navigation and screen titles." },
                    new ApiLinkServiceEntry{ Title = "Navigation Menu", Link ="components/navmenu", ComponentType=typeof(MudNavMenu), SubTitle = "Nav menu provides a tree-like menu linking to the content on your site." },
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

        private void OnSwipe(SwipeDirection direction)
        {
            if (direction == SwipeDirection.LeftToRight && !_drawerOpen)
            {
                _drawerOpen = true;
                StateHasChanged();
            }
            else if (direction == SwipeDirection.RightToLeft && _drawerOpen)
            {
                _drawerOpen = false;
                StateHasChanged();
            }
        }

        #region Theme        

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

        private void DarkMode()
        {
            if (_currentTheme == _defaultTheme)
            {
                _currentTheme = _darkTheme;
            }
            else
            {
                _currentTheme = _defaultTheme;
            }
        }

        private MudTheme _currentTheme = new();
        private readonly MudTheme _defaultTheme =
            new()
            {
                Palette = new Palette()
                {
                    Black = "#272c34"
                }
            };
        private readonly MudTheme _darkTheme =
            new()
            {
                Palette = new Palette()
                {
                    Primary = "#776be7",
                    Black = "#27272f",
                    Background = "#32333d",
                    BackgroundGrey = "#27272f",
                    Surface = "#373740",
                    DrawerBackground = "#27272f",
                    DrawerText = "rgba(255,255,255, 0.50)",
                    DrawerIcon = "rgba(255,255,255, 0.50)",
                    AppbarBackground = "#27272f",
                    AppbarText = "rgba(255,255,255, 0.70)",
                    TextPrimary = "rgba(255,255,255, 0.70)",
                    TextSecondary = "rgba(255,255,255, 0.50)",
                    ActionDefault = "#adadb1",
                    ActionDisabled = "rgba(255,255,255, 0.26)",
                    ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                    Divider = "rgba(255,255,255, 0.12)",
                    DividerLight = "rgba(255,255,255, 0.06)",
                    TableLines = "rgba(255,255,255, 0.12)",
                    LinesDefault = "rgba(255,255,255, 0.12)",
                    LinesInputs = "rgba(255,255,255, 0.3)",
                    TextDisabled = "rgba(255,255,255, 0.2)",
                    Info = "#3299ff",
                    Success = "#0bba83",
                    Warning = "#ffa800",
                    Error = "#f64e62",
                    Dark = "#27272f"
                }
            };

        #endregion
    }
}
