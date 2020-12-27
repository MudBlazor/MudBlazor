using Microsoft.AspNetCore.Components;

using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class MainLayout:LayoutComponentBase
    {

        bool _drawerOpen = false;
        bool _rightToLeft = false;
        NavigationFooterLink _previous;
        NavigationFooterLink _next;
        NavigationSection? _section =null;
        NavMenu _navMenuRef;

        [Inject] IDocsNavigationService DocsService { get; set; }
        [Inject]  NavigationManager NavigationManager { get; set; }
        
        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        void RightToLeftToggle()
        {
            _rightToLeft = !_rightToLeft;
        }

        protected override void OnInitialized()
        {
            currentTheme = defaultTheme;
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

        #region Theme        

        void DarkMode()
        {
            if (currentTheme == defaultTheme)
            {
                currentTheme = darkTheme;
            }
            else
            {
                currentTheme = defaultTheme;
            }
        }

        MudTheme currentTheme = new MudTheme();
        readonly MudTheme defaultTheme = new MudTheme()
        {
            Palette = new Palette()
            {
                Black = "#272c34"
            }
        };
        readonly MudTheme darkTheme = new MudTheme()
        {
            Palette = new Palette()
            {
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#27272f",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            }
        };

        #endregion
    }
}
