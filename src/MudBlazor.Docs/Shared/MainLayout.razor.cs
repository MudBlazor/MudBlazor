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

        #region Theme   
        private void DarkMode()
        {
            _theme.IsDarkMode = !_theme.IsDarkMode;
        }

        private MudTheme _theme = new();
        #endregion
    }
}
