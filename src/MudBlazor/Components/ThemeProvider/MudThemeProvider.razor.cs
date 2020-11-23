using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudColor = System.Drawing.Color;
using System.Text;
using System.Globalization;

namespace MudBlazor
{
    public class BaseMudThemeProvider : ComponentBase
    {
        [Parameter] public MudTheme Theme { get; set; }

        protected override void OnInitialized()
        {
            if (Theme == null)
            {
                MudTheme _theme = new MudTheme();
                Theme = _theme;
            }
        }

        public string BuildTheme()
        {
            var theme = new StringBuilder();
            theme.AppendLine("<style>");
            theme.Append(":root");
            theme.AppendLine("{");
            GenerateTheme(theme);
            theme.AppendLine("}");
            theme.AppendLine("</style>");
            return theme.ToString();
        }

        private string Breakpoint = "mud-breakpoint";
        private string Palette = "mud-palette";
        private string Elevation = "mud-elevation";
        private string LayoutProperties = "mud";
        private string Zindex = "mud-zindex";

        public static string ColorRgbDarken(string hex)
        {
            MudColor Color = ColorManager.FromHex(hex);
            Color = ColorManager.ColorDarken(Color, 0.075);
            return $"rgb({Color.R},{Color.G},{Color.B})";
        }
        public static string ColorRgbLighten(string hex)
        {
            MudColor Color = ColorManager.FromHex(hex);
            Color = ColorManager.ColorLighten(Color, 0.075);
            return $"rgb({Color.R},{Color.G},{Color.B})";
        }

        public static string ColorRgb(string hex)
        {
            MudColor Color = ColorManager.FromHex(hex);
            return $"rgb({Color.R},{Color.G},{Color.B})";
        }

        public static string ColorRgba(string hex, double alpha)
        {
            MudColor Color = ColorManager.FromHex(hex);
            return $"rgba({Color.R},{Color.G},{Color.B}, {alpha.ToString(CultureInfo.InvariantCulture)})";
        }

        protected virtual void GenerateTheme(StringBuilder theme)
        {
            //Palette
            theme.AppendLine($"--{Palette}-black: {Theme.Palette.Black};");
            theme.AppendLine($"--{Palette}-white: {Theme.Palette.White};");

            theme.AppendLine($"--{Palette}-primary: {Theme.Palette.Primary};");
            theme.AppendLine($"--{Palette}-primary-text: {Theme.Palette.PrimaryContrastText};");
            theme.AppendLine($"--{Palette}-primary-darken: {ColorRgbDarken(Theme.Palette.Primary)};");
            theme.AppendLine($"--{Palette}-primary-lighten: {ColorRgbLighten(Theme.Palette.Primary)};");
            theme.AppendLine($"--{Palette}-primary-hover: {ColorRgba(Theme.Palette.Primary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-secondary: {Theme.Palette.Secondary};");
            theme.AppendLine($"--{Palette}-secondary-text: {Theme.Palette.SecondaryContrastText};");
            theme.AppendLine($"--{Palette}-secondary-darken: {ColorRgbDarken(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{Palette}-secondary-lighten: {ColorRgbLighten(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{Palette}-secondary-hover: {ColorRgba(Theme.Palette.Secondary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-tertiary: {Theme.Palette.Tertiary};");
            theme.AppendLine($"--{Palette}-tertiary-text: {Theme.Palette.TertiaryContrastText};");
            theme.AppendLine($"--{Palette}-tertiary-darken: {ColorRgbDarken(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{Palette}-tertiary-lighten: {ColorRgbLighten(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{Palette}-tertiary-hover: {ColorRgba(Theme.Palette.Tertiary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-info: {Theme.Palette.Info};");
            theme.AppendLine($"--{Palette}-info-text: {Theme.Palette.InfoContrastText};");
            theme.AppendLine($"--{Palette}-info-darken: {ColorRgbDarken(Theme.Palette.Info)};");
            theme.AppendLine($"--{Palette}-info-lighten: {ColorRgbLighten(Theme.Palette.Info)};");
            theme.AppendLine($"--{Palette}-info-hover: {ColorRgba(Theme.Palette.Info, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-success: {Theme.Palette.Success};");
            theme.AppendLine($"--{Palette}-success-text: {Theme.Palette.SuccessContrastText};");
            theme.AppendLine($"--{Palette}-success-darken: {ColorRgbDarken(Theme.Palette.Success)};");
            theme.AppendLine($"--{Palette}-success-lighten: {ColorRgbLighten(Theme.Palette.Success)};");
            theme.AppendLine($"--{Palette}-success-hover: {ColorRgba(Theme.Palette.Success, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-warning: {Theme.Palette.Warning};");
            theme.AppendLine($"--{Palette}-warning-text: {Theme.Palette.WarningContrastText};");
            theme.AppendLine($"--{Palette}-warning-darken: {ColorRgbDarken(Theme.Palette.Warning)};");
            theme.AppendLine($"--{Palette}-warning-lighten: {ColorRgbLighten(Theme.Palette.Warning)};");
            theme.AppendLine($"--{Palette}-warning-hover: {ColorRgba(Theme.Palette.Warning, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-error: {Theme.Palette.Error};");
            theme.AppendLine($"--{Palette}-error-text: {Theme.Palette.ErrorContrastText};");
            theme.AppendLine($"--{Palette}-error-darken: {ColorRgbDarken(Theme.Palette.Error)};");
            theme.AppendLine($"--{Palette}-error-lighten: {ColorRgbLighten(Theme.Palette.Error)};");
            theme.AppendLine($"--{Palette}-error-hover: {ColorRgba(Theme.Palette.Error, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-dark: {Theme.Palette.Dark};");
            theme.AppendLine($"--{Palette}-dark-text: {Theme.Palette.DarkContrastText};");
            theme.AppendLine($"--{Palette}-dark-darken: {ColorRgbDarken(Theme.Palette.Dark)};");
            theme.AppendLine($"--{Palette}-dark-lighten: {ColorRgbLighten(Theme.Palette.Dark)};");
            theme.AppendLine($"--{Palette}-dark-hover: {ColorRgba(Theme.Palette.Dark, Theme.Palette.HoverOpacity)};");

            theme.AppendLine($"--{Palette}-text-primary: {Theme.Palette.TextPrimary};");
            theme.AppendLine($"--{Palette}-text-secondary: {Theme.Palette.TextSecondary};");
            theme.AppendLine($"--{Palette}-text-disabled: {Theme.Palette.TextDisabled};");

            theme.AppendLine($"--{Palette}-action-default: {Theme.Palette.ActionDefault};");
            theme.AppendLine($"--{Palette}-action-default-hover: {ColorRgba(Colors.Shades.Black, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-action-disabled: {Theme.Palette.ActionDisabled};");
            theme.AppendLine($"--{Palette}-action-disabled-background: {Theme.Palette.ActionDisabledBackground};");

            theme.AppendLine($"--{Palette}-surface: {Theme.Palette.Surface};");
            theme.AppendLine($"--{Palette}-background: {Theme.Palette.Background};");
            theme.AppendLine($"--{Palette}-background-grey: {Theme.Palette.BackgroundGrey};");
            theme.AppendLine($"--{Palette}-drawer-background: {Theme.Palette.DrawerBackground};");
            theme.AppendLine($"--{Palette}-drawer-text: {Theme.Palette.DrawerText};");
            theme.AppendLine($"--{Palette}-drawer-icon: {Theme.Palette.DrawerIcon};");
            theme.AppendLine($"--{Palette}-appbar-background: {Theme.Palette.AppbarBackground};");
            theme.AppendLine($"--{Palette}-appbar-text: {Theme.Palette.AppbarText};");

            theme.AppendLine($"--{Palette}-lines-default: {Theme.Palette.LinesDefault};");
            theme.AppendLine($"--{Palette}-lines-inputs: {Theme.Palette.LinesInputs};");

            theme.AppendLine($"--{Palette}-divider: {Theme.Palette.Divider};");
            theme.AppendLine($"--{Palette}-divider-light: {Theme.Palette.DividerLight};");

            theme.AppendLine($"--{Palette}-grey-default: {Theme.Palette.GrayDefault};");
            theme.AppendLine($"--{Palette}-grey-light: {Theme.Palette.GrayLight};");
            theme.AppendLine($"--{Palette}-grey-lighter: {Theme.Palette.GrayLighter};");
            theme.AppendLine($"--{Palette}-grey-dark: {Theme.Palette.GrayDark};");
            theme.AppendLine($"--{Palette}-grey-darker: {Theme.Palette.GrayDarker};");

            //Elevations
            theme.AppendLine($"--{Elevation}-0: {Theme.Shadows.Elevation.GetValue(0)};");
            theme.AppendLine($"--{Elevation}-1: {Theme.Shadows.Elevation.GetValue(1)};");
            theme.AppendLine($"--{Elevation}-2: {Theme.Shadows.Elevation.GetValue(2)};");
            theme.AppendLine($"--{Elevation}-3: {Theme.Shadows.Elevation.GetValue(3)};");
            theme.AppendLine($"--{Elevation}-4: {Theme.Shadows.Elevation.GetValue(4)};");
            theme.AppendLine($"--{Elevation}-5: {Theme.Shadows.Elevation.GetValue(5)};");
            theme.AppendLine($"--{Elevation}-6: {Theme.Shadows.Elevation.GetValue(6)};");
            theme.AppendLine($"--{Elevation}-7: {Theme.Shadows.Elevation.GetValue(7)};");
            theme.AppendLine($"--{Elevation}-8: {Theme.Shadows.Elevation.GetValue(8)};");
            theme.AppendLine($"--{Elevation}-9: {Theme.Shadows.Elevation.GetValue(9)};");
            theme.AppendLine($"--{Elevation}-10: {Theme.Shadows.Elevation.GetValue(10)};");
            theme.AppendLine($"--{Elevation}-11: {Theme.Shadows.Elevation.GetValue(11)};");
            theme.AppendLine($"--{Elevation}-12: {Theme.Shadows.Elevation.GetValue(12)};");
            theme.AppendLine($"--{Elevation}-13: {Theme.Shadows.Elevation.GetValue(13)};");
            theme.AppendLine($"--{Elevation}-14: {Theme.Shadows.Elevation.GetValue(14)};");
            theme.AppendLine($"--{Elevation}-15: {Theme.Shadows.Elevation.GetValue(15)};");
            theme.AppendLine($"--{Elevation}-16: {Theme.Shadows.Elevation.GetValue(16)};");
            theme.AppendLine($"--{Elevation}-17: {Theme.Shadows.Elevation.GetValue(17)};");
            theme.AppendLine($"--{Elevation}-18: {Theme.Shadows.Elevation.GetValue(18)};");
            theme.AppendLine($"--{Elevation}-19: {Theme.Shadows.Elevation.GetValue(19)};");
            theme.AppendLine($"--{Elevation}-20: {Theme.Shadows.Elevation.GetValue(20)};");
            theme.AppendLine($"--{Elevation}-21: {Theme.Shadows.Elevation.GetValue(21)};");
            theme.AppendLine($"--{Elevation}-22: {Theme.Shadows.Elevation.GetValue(22)};");
            theme.AppendLine($"--{Elevation}-23: {Theme.Shadows.Elevation.GetValue(23)};");
            theme.AppendLine($"--{Elevation}-24: {Theme.Shadows.Elevation.GetValue(24)};");
            theme.AppendLine($"--{Elevation}-25: {Theme.Shadows.Elevation.GetValue(25)};");

            //Layout Properties
            theme.AppendLine($"--{LayoutProperties}-default-borderradius: {Theme.LayoutProperties.DefaultBorderRadius};");
            theme.AppendLine($"--{LayoutProperties}-drawer-width: {Theme.LayoutProperties.DrawerWidth};");
            theme.AppendLine($"--{LayoutProperties}-appbar-min-height: {Theme.LayoutProperties.AppbarMinHeight};");

            //Breakpoint
            //theme.AppendLine($"--{Breakpoint}-xs: {Theme.Breakpoints.xs};");
            //theme.AppendLine($"--{Breakpoint}-sm: {Theme.Breakpoints.sm};");
            //theme.AppendLine($"--{Breakpoint}-md: {Theme.Breakpoints.md};");
            //theme.AppendLine($"--{Breakpoint}-lg: {Theme.Breakpoints.lg};");
            //theme.AppendLine($"--{Breakpoint}-xl: {Theme.Breakpoints.xl};");

            //Z-Index
            theme.AppendLine($"--{Zindex}-drawer: {Theme.ZIndex.Drawer};");
            theme.AppendLine($"--{Zindex}-appbar: {Theme.ZIndex.AppBar};");
            theme.AppendLine($"--{Zindex}-dialog: {Theme.ZIndex.Dialog};");
            theme.AppendLine($"--{Zindex}-popover: {Theme.ZIndex.Popover};");
            theme.AppendLine($"--{Zindex}-snackbar: {Theme.ZIndex.Snackbar};");
            theme.AppendLine($"--{Zindex}-tooltip: {Theme.ZIndex.Tooltip};");
        }
    }
}
