// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Docs
{
    public class Theme
    {
        public static MudTheme LandingPageTheme()
        {
            var theme = new MudTheme()
            {
                PaletteLight = LandingPageLightPalette,
                PaletteDark = LandingPageDarkPalette,
                Shadows = LandingPageShadows,
                LayoutProperties = LandingPageLayoutProperties,
                ZIndex = new ZIndex(),
                Typography = LandingPageTypography
            };
            return theme;
        }

        public static MudTheme DocsTheme()
        {
            var theme = new MudTheme()
            {
                PaletteLight = DocsLightPalette,
                PaletteDark = DocsDarkPalette,
                LayoutProperties = new LayoutProperties()
            };
            return theme;
        }



        #region Docs

        private static readonly PaletteLight DocsLightPalette = new()
        {
            Black = "#110e2d",
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,0.8)",
            DrawerBackground = "#ffffff",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
        };

        private static readonly PaletteDark DocsDarkPalette = new()
        {
            Primary = "#7e6fff",
            Surface = "#1e1e2d",
            Background = "#1a1a27",
            BackgroundGray = "#151521",
            AppbarText = "#92929f",
            AppbarBackground = "rgba(26,26,39,0.8)",
            DrawerBackground = "#1a1a27",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextPrimary = "#b2b0bf",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#3dcb6c",
            Warning = "#ffb545",
            Error = "#ff3f5f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80",
        };
        #endregion
        #region LandingPage

        private static readonly LayoutProperties LandingPageLayoutProperties = new()
        {
            DefaultBorderRadius = "6px"
        };
        private static readonly Typography LandingPageTypography = new()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Public Sans", "Roboto", "Arial", "sans-serif" },
                LetterSpacing = "normal"
            },
            H1 = new H1()
            {
                FontSize = "4rem",
                FontWeight = 700,
            },
            H3 = new H3()
            {
                FontSize = "3rem",
                FontWeight = 600,
                LineHeight = 1.8,
            },
            H4 = new H4()
            {
                FontSize = "1.8rem",
                FontWeight = 700,
            },
            H5 = new H5()
            {
                FontSize = "1.8rem",
                FontWeight = 700,
                LineHeight = 2,
            },
            H6 = new H6()
            {
                FontSize = "1.125rem",
                FontWeight = 700,
                LineHeight = 2,
            },
            Subtitle1 = new Subtitle1()
            {
                FontSize = "1.1rem",
                FontWeight = 500
            },
            Subtitle2 = new Subtitle2()
            {
                FontSize = "1rem",
                FontWeight = 600,
                LineHeight = 1.8,
            },
            Body1 = new Body1()
            {
                FontSize = "1rem",
                FontWeight = 400
            },
            Button = new Button()
            {
                TextTransform = "none"
            }
        };
        private static readonly PaletteLight LandingPageLightPalette = new()
        {
            AppbarText = "#424242",
            AppbarBackground = "rgba(0,0,0,0)",
            BackgroundGray = "#F9FAFC",
            TextSecondary = "#425466",
            Dark = "#110E2D",
            DarkLighten = "#1A1643",
            GrayDefault = "#4B5563",
            GrayLight = "#9CA3AF",
            GrayLighter = "#adbdccff",
        };
        private static readonly PaletteDark LandingPageDarkPalette = new()
        {
            AppbarText = "#92929f",
            AppbarBackground = "rgba(0,0,0,0)",
            BackgroundGray = "#1a1a27",
            Surface = "#1e1e2d",
            Background = "#151521",
            Dark = "#111019",
            DarkLighten = "#1A1643",
            TextPrimary = "#ffffff",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            ActionDefault = "#92929f",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            DrawerBackground = "#151521",
            OverlayLight = "#1e1e2d80",
            Divider = "#5c5c6a",
        };

        private static readonly Shadow LandingPageShadows = new()
        {
            Elevation = new string[]
            {
            "none",
            "0 2px 4px -1px rgba(6, 24, 44, 0.2)",
            "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)",
            "0 30px 60px rgba(0,0,0,0.12)",
            "0 6px 12px -2px rgba(50,50,93,0.25),0 3px 7px -3px rgba(0,0,0,0.3)",
            "0 50px 100px -20px rgba(50,50,93,0.25),0 30px 60px -30px rgba(0,0,0,0.3)",
            "0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12)",
            "0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12)",
            "0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12)",
            "0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12)",
            "0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12)",
            "0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12)",
            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12)",
            "0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12)",
            "0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12)",
            "0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12)",
            "0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12)",
            "0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12)",
            "0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12)",
            "0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12)",
            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12)",
            "0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12)",
            "0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12)",
            "0 50px 100px -20px rgba(50, 50, 93, 0.25), 0 30px 60px -30px rgba(0, 0, 0, 0.30)",
            "2.8px 2.8px 2.2px rgba(0, 0, 0, 0.02),6.7px 6.7px 5.3px rgba(0, 0, 0, 0.028),12.5px 12.5px 10px rgba(0, 0, 0, 0.035),22.3px 22.3px 17.9px rgba(0, 0, 0, 0.042),41.8px 41.8px 33.4px rgba(0, 0, 0, 0.05),100px 100px 80px rgba(0, 0, 0, 0.07)",
            "0px 0px 20px 0px rgba(0, 0, 0, 0.05)"
            }
        };
        #endregion
    }
}
