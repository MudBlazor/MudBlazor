using System;
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
        private NavigationFooterLink _previous;
        private NavigationFooterLink _next;
        private NavigationSection? _section = null;
        private NavMenu _navMenuRef;

        [Inject] private IDocsNavigationService DocsService { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject] private IApiLinkService ApiLinkService { get; set; }

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

        protected override void OnParametersSet()
        {
            _previous = DocsService.Previous;
            _next = DocsService.Next;
            _section = DocsService.Section;
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
                return Task.FromResult<IEnumerable<ApiLinkServiceEntry>>(Array.Empty<ApiLinkServiceEntry>());
            return ApiLinkService.Search(text);
        }

        private void OnSearchResult(ApiLinkServiceEntry entry)
        {
            NavigationManager.NavigateTo(entry.Link);
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

        private MudTheme _currentTheme = new MudTheme();
        private readonly MudTheme _defaultTheme =
            new MudTheme()
            {
                Palette = new Palette()
                {
                    Black = "#272c34"
                }
            };
        private readonly MudTheme _darkTheme =
            new MudTheme()
            {
                Palette = new Palette()
                {
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
                    TextDisabled = "rgba(255,255,255, 0.2)"
                }
            };

        #endregion
    }
}
