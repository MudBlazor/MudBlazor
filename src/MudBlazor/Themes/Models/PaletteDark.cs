// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a dark color palette.
    /// </summary>
    public record class PaletteDark : Palette
    {
        public PaletteDark()
        {
            Black = "#27272f";
            Primary = "#776be7";
            Info = "#3299ff";
            Success = "#0bba83";
            Warning = "#ffa800";
            Error = "#f64e62";
            Dark = "#27272f";
            TextPrimary = "rgba(255,255,255, 0.70)";
            TextSecondary = "rgba(255,255,255, 0.50)";
            TextDisabled = "rgba(255,255,255, 0.2)";
            ActionDefault = "#adadb1";
            ActionDisabled = "rgba(255,255,255, 0.26)";
            ActionDisabledBackground = "rgba(255,255,255, 0.12)";
            Background = "#32333d";
            BackgroundGray = "#27272f";
            Surface = "#373740";
            DrawerBackground = "#27272f";
            DrawerText = "rgba(255,255,255, 0.50)";
            DrawerIcon = "rgba(255,255,255, 0.50)";
            AppbarBackground = "#27272f";
            AppbarText = "rgba(255,255,255, 0.70)";
            LinesDefault = "rgba(255,255,255, 0.12)";
            LinesInputs = "rgba(255,255,255, 0.3)";
            TableLines = "rgba(255,255,255, 0.12)";
            TableStriped = "rgba(255,255,255, 0.2)";
            Divider = "rgba(255,255,255, 0.12)";
            DividerLight = "rgba(255,255,255, 0.06)";
            Skeleton = "rgba(255,255,255, 0.11)";
        }
    }
}
