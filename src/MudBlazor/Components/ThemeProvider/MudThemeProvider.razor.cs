using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudColor = System.Drawing.Color;

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

        // private string Breakpoint = "mud-breakpoint";
        private readonly string _palette = "mud-palette";
        private readonly string _elevation = "mud-elevation";
        private readonly string _typography = "mud-typography";
        private readonly string _layoutProperties = "mud";
        private readonly string _zindex = "mud-zindex";

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
            theme.AppendLine($"--{_palette}-black: {Theme.Palette.Black};");
            theme.AppendLine($"--{_palette}-white: {Theme.Palette.White};");

            theme.AppendLine($"--{_palette}-primary: {Theme.Palette.Primary};");
            theme.AppendLine($"--{_palette}-primary-text: {Theme.Palette.PrimaryContrastText};");
            theme.AppendLine($"--{_palette}-primary-darken: {ColorRgbDarken(Theme.Palette.Primary)};");
            theme.AppendLine($"--{_palette}-primary-lighten: {ColorRgbLighten(Theme.Palette.Primary)};");
            theme.AppendLine($"--{_palette}-primary-hover: {ColorRgba(Theme.Palette.Primary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-secondary: {Theme.Palette.Secondary};");
            theme.AppendLine($"--{_palette}-secondary-text: {Theme.Palette.SecondaryContrastText};");
            theme.AppendLine($"--{_palette}-secondary-darken: {ColorRgbDarken(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{_palette}-secondary-lighten: {ColorRgbLighten(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{_palette}-secondary-hover: {ColorRgba(Theme.Palette.Secondary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-tertiary: {Theme.Palette.Tertiary};");
            theme.AppendLine($"--{_palette}-tertiary-text: {Theme.Palette.TertiaryContrastText};");
            theme.AppendLine($"--{_palette}-tertiary-darken: {ColorRgbDarken(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{_palette}-tertiary-lighten: {ColorRgbLighten(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{_palette}-tertiary-hover: {ColorRgba(Theme.Palette.Tertiary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-info: {Theme.Palette.Info};");
            theme.AppendLine($"--{_palette}-info-text: {Theme.Palette.InfoContrastText};");
            theme.AppendLine($"--{_palette}-info-darken: {ColorRgbDarken(Theme.Palette.Info)};");
            theme.AppendLine($"--{_palette}-info-lighten: {ColorRgbLighten(Theme.Palette.Info)};");
            theme.AppendLine($"--{_palette}-info-hover: {ColorRgba(Theme.Palette.Info, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-success: {Theme.Palette.Success};");
            theme.AppendLine($"--{_palette}-success-text: {Theme.Palette.SuccessContrastText};");
            theme.AppendLine($"--{_palette}-success-darken: {ColorRgbDarken(Theme.Palette.Success)};");
            theme.AppendLine($"--{_palette}-success-lighten: {ColorRgbLighten(Theme.Palette.Success)};");
            theme.AppendLine($"--{_palette}-success-hover: {ColorRgba(Theme.Palette.Success, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-warning: {Theme.Palette.Warning};");
            theme.AppendLine($"--{_palette}-warning-text: {Theme.Palette.WarningContrastText};");
            theme.AppendLine($"--{_palette}-warning-darken: {ColorRgbDarken(Theme.Palette.Warning)};");
            theme.AppendLine($"--{_palette}-warning-lighten: {ColorRgbLighten(Theme.Palette.Warning)};");
            theme.AppendLine($"--{_palette}-warning-hover: {ColorRgba(Theme.Palette.Warning, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-error: {Theme.Palette.Error};");
            theme.AppendLine($"--{_palette}-error-text: {Theme.Palette.ErrorContrastText};");
            theme.AppendLine($"--{_palette}-error-darken: {ColorRgbDarken(Theme.Palette.Error)};");
            theme.AppendLine($"--{_palette}-error-lighten: {ColorRgbLighten(Theme.Palette.Error)};");
            theme.AppendLine($"--{_palette}-error-hover: {ColorRgba(Theme.Palette.Error, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-dark: {Theme.Palette.Dark};");
            theme.AppendLine($"--{_palette}-dark-text: {Theme.Palette.DarkContrastText};");
            theme.AppendLine($"--{_palette}-dark-darken: {ColorRgbDarken(Theme.Palette.Dark)};");
            theme.AppendLine($"--{_palette}-dark-lighten: {ColorRgbLighten(Theme.Palette.Dark)};");
            theme.AppendLine($"--{_palette}-dark-hover: {ColorRgba(Theme.Palette.Dark, Theme.Palette.HoverOpacity)};");

            theme.AppendLine($"--{_palette}-text-primary: {Theme.Palette.TextPrimary};");
            theme.AppendLine($"--{_palette}-text-secondary: {Theme.Palette.TextSecondary};");
            theme.AppendLine($"--{_palette}-text-disabled: {Theme.Palette.TextDisabled};");

            theme.AppendLine($"--{_palette}-action-default: {Theme.Palette.ActionDefault};");
            theme.AppendLine($"--{_palette}-action-default-hover: {ColorRgba(Colors.Shades.Black, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{_palette}-action-disabled: {Theme.Palette.ActionDisabled};");
            theme.AppendLine($"--{_palette}-action-disabled-background: {Theme.Palette.ActionDisabledBackground};");

            theme.AppendLine($"--{_palette}-surface: {Theme.Palette.Surface};");
            theme.AppendLine($"--{_palette}-background: {Theme.Palette.Background};");
            theme.AppendLine($"--{_palette}-background-grey: {Theme.Palette.BackgroundGrey};");
            theme.AppendLine($"--{_palette}-drawer-background: {Theme.Palette.DrawerBackground};");
            theme.AppendLine($"--{_palette}-drawer-text: {Theme.Palette.DrawerText};");
            theme.AppendLine($"--{_palette}-drawer-icon: {Theme.Palette.DrawerIcon};");
            theme.AppendLine($"--{_palette}-appbar-background: {Theme.Palette.AppbarBackground};");
            theme.AppendLine($"--{_palette}-appbar-text: {Theme.Palette.AppbarText};");

            theme.AppendLine($"--{_palette}-lines-default: {Theme.Palette.LinesDefault};");
            theme.AppendLine($"--{_palette}-lines-inputs: {Theme.Palette.LinesInputs};");

            theme.AppendLine($"--{_palette}-divider: {Theme.Palette.Divider};");
            theme.AppendLine($"--{_palette}-divider-light: {Theme.Palette.DividerLight};");

            theme.AppendLine($"--{_palette}-grey-default: {Theme.Palette.GrayDefault};");
            theme.AppendLine($"--{_palette}-grey-light: {Theme.Palette.GrayLight};");
            theme.AppendLine($"--{_palette}-grey-lighter: {Theme.Palette.GrayLighter};");
            theme.AppendLine($"--{_palette}-grey-dark: {Theme.Palette.GrayDark};");
            theme.AppendLine($"--{_palette}-grey-darker: {Theme.Palette.GrayDarker};");

            theme.AppendLine($"--{_palette}-overlay-dark: {Theme.Palette.OverlayDark};");
            theme.AppendLine($"--{_palette}-overlay-light: {Theme.Palette.OverlayLight};");

            //Elevations
            theme.AppendLine($"--{_elevation}-0: {Theme.Shadows.Elevation.GetValue(0)};");
            theme.AppendLine($"--{_elevation}-1: {Theme.Shadows.Elevation.GetValue(1)};");
            theme.AppendLine($"--{_elevation}-2: {Theme.Shadows.Elevation.GetValue(2)};");
            theme.AppendLine($"--{_elevation}-3: {Theme.Shadows.Elevation.GetValue(3)};");
            theme.AppendLine($"--{_elevation}-4: {Theme.Shadows.Elevation.GetValue(4)};");
            theme.AppendLine($"--{_elevation}-5: {Theme.Shadows.Elevation.GetValue(5)};");
            theme.AppendLine($"--{_elevation}-6: {Theme.Shadows.Elevation.GetValue(6)};");
            theme.AppendLine($"--{_elevation}-7: {Theme.Shadows.Elevation.GetValue(7)};");
            theme.AppendLine($"--{_elevation}-8: {Theme.Shadows.Elevation.GetValue(8)};");
            theme.AppendLine($"--{_elevation}-9: {Theme.Shadows.Elevation.GetValue(9)};");
            theme.AppendLine($"--{_elevation}-10: {Theme.Shadows.Elevation.GetValue(10)};");
            theme.AppendLine($"--{_elevation}-11: {Theme.Shadows.Elevation.GetValue(11)};");
            theme.AppendLine($"--{_elevation}-12: {Theme.Shadows.Elevation.GetValue(12)};");
            theme.AppendLine($"--{_elevation}-13: {Theme.Shadows.Elevation.GetValue(13)};");
            theme.AppendLine($"--{_elevation}-14: {Theme.Shadows.Elevation.GetValue(14)};");
            theme.AppendLine($"--{_elevation}-15: {Theme.Shadows.Elevation.GetValue(15)};");
            theme.AppendLine($"--{_elevation}-16: {Theme.Shadows.Elevation.GetValue(16)};");
            theme.AppendLine($"--{_elevation}-17: {Theme.Shadows.Elevation.GetValue(17)};");
            theme.AppendLine($"--{_elevation}-18: {Theme.Shadows.Elevation.GetValue(18)};");
            theme.AppendLine($"--{_elevation}-19: {Theme.Shadows.Elevation.GetValue(19)};");
            theme.AppendLine($"--{_elevation}-20: {Theme.Shadows.Elevation.GetValue(20)};");
            theme.AppendLine($"--{_elevation}-21: {Theme.Shadows.Elevation.GetValue(21)};");
            theme.AppendLine($"--{_elevation}-22: {Theme.Shadows.Elevation.GetValue(22)};");
            theme.AppendLine($"--{_elevation}-23: {Theme.Shadows.Elevation.GetValue(23)};");
            theme.AppendLine($"--{_elevation}-24: {Theme.Shadows.Elevation.GetValue(24)};");
            theme.AppendLine($"--{_elevation}-25: {Theme.Shadows.Elevation.GetValue(25)};");

            //Layout Properties
            theme.AppendLine($"--{_layoutProperties}-default-borderradius: {Theme.LayoutProperties.DefaultBorderRadius};");
            theme.AppendLine($"--{_layoutProperties}-drawer-width: {Theme.LayoutProperties.DrawerWidth};");
            theme.AppendLine($"--{_layoutProperties}-appbar-min-height: {Theme.LayoutProperties.AppbarMinHeight};");

            //Breakpoint
            //theme.AppendLine($"--{Breakpoint}-xs: {Theme.Breakpoints.xs};");
            //theme.AppendLine($"--{Breakpoint}-sm: {Theme.Breakpoints.sm};");
            //theme.AppendLine($"--{Breakpoint}-md: {Theme.Breakpoints.md};");
            //theme.AppendLine($"--{Breakpoint}-lg: {Theme.Breakpoints.lg};");
            //theme.AppendLine($"--{Breakpoint}-xl: {Theme.Breakpoints.xl};");

            //Typography
            theme.AppendLine($"--{_typography}-default-family: '{string.Join("','", Theme.Typography.Default.FontFamily)}';");
            theme.AppendLine($"--{_typography}-default-size: {Theme.Typography.Default.FontSize};");
            theme.AppendLine($"--{_typography}-default-weight: {Theme.Typography.Default.FontWeight};");
            theme.AppendLine($"--{_typography}-default-lineheight: {Theme.Typography.Default.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-default-letterspacing: {Theme.Typography.Default.LetterSpacing};");

            theme.AppendLine($"--{_typography}-h1-family: '{string.Join("','", Theme.Typography.H1.FontFamily)}';");
            theme.AppendLine($"--{_typography}-h1-size: {Theme.Typography.H1.FontSize};");
            theme.AppendLine($"--{_typography}-h1-weight: {Theme.Typography.H1.FontWeight};");
            theme.AppendLine($"--{_typography}-h1-lineheight: {Theme.Typography.H1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-h1-letterspacing: {Theme.Typography.H1.LetterSpacing};");

            theme.AppendLine($"--{_typography}-h2-family: '{string.Join("','", Theme.Typography.H2.FontFamily)}';");
            theme.AppendLine($"--{_typography}-h2-size: {Theme.Typography.H2.FontSize};");
            theme.AppendLine($"--{_typography}-h2-weight: {Theme.Typography.H2.FontWeight};");
            theme.AppendLine($"--{_typography}-h2-lineheight: {Theme.Typography.H2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-h2-letterspacing: {Theme.Typography.H2.LetterSpacing};");

            theme.AppendLine($"--{_typography}-h3-family: '{string.Join("','", Theme.Typography.H3.FontFamily)}';");
            theme.AppendLine($"--{_typography}-h3-size: {Theme.Typography.H3.FontSize};");
            theme.AppendLine($"--{_typography}-h3-weight: {Theme.Typography.H3.FontWeight};");
            theme.AppendLine($"--{_typography}-h3-lineheight: {Theme.Typography.H3.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-h3-letterspacing: {Theme.Typography.H3.LetterSpacing};");

            theme.AppendLine($"--{_typography}-h4-family: '{string.Join("','", Theme.Typography.H4.FontFamily)}';");
            theme.AppendLine($"--{_typography}-h4-size: {Theme.Typography.H4.FontSize};");
            theme.AppendLine($"--{_typography}-h4-weight: {Theme.Typography.H4.FontWeight};");
            theme.AppendLine($"--{_typography}-h4-lineheight: {Theme.Typography.H4.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-h4-letterspacing: {Theme.Typography.H4.LetterSpacing};");

            theme.AppendLine($"--{_typography}-h5-family: '{string.Join("','", Theme.Typography.H5.FontFamily)}';");
            theme.AppendLine($"--{_typography}-h5-size: {Theme.Typography.H5.FontSize};");
            theme.AppendLine($"--{_typography}-h5-weight: {Theme.Typography.H5.FontWeight};");
            theme.AppendLine($"--{_typography}-h5-lineheight: {Theme.Typography.H5.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-h5-letterspacing: {Theme.Typography.H5.LetterSpacing};");

            theme.AppendLine($"--{_typography}-h6-family: '{string.Join("','", Theme.Typography.H6.FontFamily)}';");
            theme.AppendLine($"--{_typography}-h6-size: {Theme.Typography.H6.FontSize};");
            theme.AppendLine($"--{_typography}-h6-weight: {Theme.Typography.H6.FontWeight};");
            theme.AppendLine($"--{_typography}-h6-lineheight: {Theme.Typography.H6.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-h6-letterspacing: {Theme.Typography.H6.LetterSpacing};");

            theme.AppendLine($"--{_typography}-subtitle1-family: '{string.Join("','", Theme.Typography.Subtitle1.FontFamily)}';");
            theme.AppendLine($"--{_typography}-subtitle1-size: {Theme.Typography.Subtitle1.FontSize};");
            theme.AppendLine($"--{_typography}-subtitle1-weight: {Theme.Typography.Subtitle1.FontWeight};");
            theme.AppendLine($"--{_typography}-subtitle1-lineheight: {Theme.Typography.Subtitle1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-subtitle1-letterspacing: {Theme.Typography.Subtitle1.LetterSpacing};");

            theme.AppendLine($"--{_typography}-subtitle2-family: '{string.Join("','", Theme.Typography.Subtitle2.FontFamily)}';");
            theme.AppendLine($"--{_typography}-subtitle2-size: {Theme.Typography.Subtitle2.FontSize};");
            theme.AppendLine($"--{_typography}-subtitle2-weight: {Theme.Typography.Subtitle2.FontWeight};");
            theme.AppendLine($"--{_typography}-subtitle2-lineheight: {Theme.Typography.Subtitle2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-subtitle2-letterspacing: {Theme.Typography.Subtitle2.LetterSpacing};");

            theme.AppendLine($"--{_typography}-body1-family: '{string.Join("','", Theme.Typography.Body1.FontFamily)}';");
            theme.AppendLine($"--{_typography}-body1-size: {Theme.Typography.Body1.FontSize};");
            theme.AppendLine($"--{_typography}-body1-weight: {Theme.Typography.Body1.FontWeight};");
            theme.AppendLine($"--{_typography}-body1-lineheight: {Theme.Typography.Body1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-body1-letterspacing: {Theme.Typography.Body1.LetterSpacing};");

            theme.AppendLine($"--{_typography}-body2-family: '{string.Join("','", Theme.Typography.Body2.FontFamily)}';");
            theme.AppendLine($"--{_typography}-body2-size: {Theme.Typography.Body2.FontSize};");
            theme.AppendLine($"--{_typography}-body2-weight: {Theme.Typography.Body2.FontWeight};");
            theme.AppendLine($"--{_typography}-body2-lineheight: {Theme.Typography.Body2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-body2-letterspacing: {Theme.Typography.Body2.LetterSpacing};");

            theme.AppendLine($"--{_typography}-button-family: '{string.Join("','", Theme.Typography.Button.FontFamily)}';");
            theme.AppendLine($"--{_typography}-button-size: {Theme.Typography.Button.FontSize};");
            theme.AppendLine($"--{_typography}-button-weight: {Theme.Typography.Button.FontWeight};");
            theme.AppendLine($"--{_typography}-button-lineheight: {Theme.Typography.Button.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-button-letterspacing: {Theme.Typography.Button.LetterSpacing};");

            theme.AppendLine($"--{_typography}-caption-family: '{string.Join("','", Theme.Typography.Caption.FontFamily)}';");
            theme.AppendLine($"--{_typography}-caption-size: {Theme.Typography.Caption.FontSize};");
            theme.AppendLine($"--{_typography}-caption-weight: {Theme.Typography.Caption.FontWeight};");
            theme.AppendLine($"--{_typography}-caption-lineheight: {Theme.Typography.Caption.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-caption-letterspacing: {Theme.Typography.Caption.LetterSpacing};");

            theme.AppendLine($"--{_typography}-overline-family: '{string.Join("','", Theme.Typography.Overline.FontFamily)}';");
            theme.AppendLine($"--{_typography}-overline-size: {Theme.Typography.Overline.FontSize};");
            theme.AppendLine($"--{_typography}-overline-weight: {Theme.Typography.Overline.FontWeight};");
            theme.AppendLine($"--{_typography}-overline-lineheight: {Theme.Typography.Overline.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{_typography}-overline-letterspacing: {Theme.Typography.Overline.LetterSpacing};");


            //Z-Index
            theme.AppendLine($"--{_zindex}-drawer: {Theme.ZIndex.Drawer};");
            theme.AppendLine($"--{_zindex}-appbar: {Theme.ZIndex.AppBar};");
            theme.AppendLine($"--{_zindex}-dialog: {Theme.ZIndex.Dialog};");
            theme.AppendLine($"--{_zindex}-popover: {Theme.ZIndex.Popover};");
            theme.AppendLine($"--{_zindex}-snackbar: {Theme.ZIndex.Snackbar};");
            theme.AppendLine($"--{_zindex}-tooltip: {Theme.ZIndex.Tooltip};");
        }
    }
}
