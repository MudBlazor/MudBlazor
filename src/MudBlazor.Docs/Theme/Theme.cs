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
    public static class Theme
    {
        public static MudTheme LandingPageLightTheme { get; set; } =
            new()
            {
                Palette = new Palette()
                {
                    AppbarText = "rgb(33, 43, 54)",
                    AppbarBackground = "#FFFFFF"
                },
                Typography = new Typography()
                {

                }
            };
        public static MudTheme DocsLightTheme { get; set; } =
            new()
            {
                Palette = new Palette()
                {
                    Black = "#272c34"
                }
            };
        public static MudTheme DocsDarkTheme { get; set; } =
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
    }
}
