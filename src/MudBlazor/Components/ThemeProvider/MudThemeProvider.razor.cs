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
                var theme = new MudTheme();
                Theme = theme;
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

        // private const string Breakpoint = "mud-breakpoint";
        private const string Palette = "mud-palette";
        private const string Elevation = "mud-elevation";
        private const string Typography = "mud-typography";
        private const string LayoutProperties = "mud";
        private const string Zindex = "mud-zindex";

        public static string ColorRgbDarken(string hex)
        {
            var color = ColorManager.FromHex(hex);
            color = ColorManager.ColorDarken(color, 0.075);
            return $"rgb({color.R},{color.G},{color.B})";
        }
        public static string ColorRgbLighten(string hex)
        {
            var color = ColorManager.FromHex(hex);
            color = ColorManager.ColorLighten(color, 0.075);
            return $"rgb({color.R},{color.G},{color.B})";
        }

        public static string ColorRgb(string hex)
        {
            var color = ColorManager.FromHex(hex);
            return $"rgb({color.R},{color.G},{color.B})";
        }

        public static string ColorRgbElements(string hex)
        {
            var color = ColorManager.FromHex(hex);
            return $"{color.R},{color.G},{color.B}";
        }

        public static string ColorRgba(string hex, double alpha)
        {
            var color = ColorManager.FromHex(hex);
            return $"rgba({color.R},{color.G},{color.B}, {alpha.ToString(CultureInfo.InvariantCulture)})";
        }

        protected virtual void GenerateTheme(StringBuilder theme)
        {
            //Palette
            theme.AppendLine($"--{Palette}-black: {Theme.Palette.Black};");
            theme.AppendLine($"--{Palette}-white: {Theme.Palette.White};");

            theme.AppendLine($"--{Palette}-primary: {Theme.Palette.Primary};");
            theme.AppendLine($"--{Palette}-primary-rgb: {ColorRgbElements(Theme.Palette.Primary)};");
            theme.AppendLine($"--{Palette}-primary-text: {Theme.Palette.PrimaryContrastText};");
            theme.AppendLine($"--{Palette}-primary-darken: {ColorRgbDarken(Theme.Palette.Primary)};");
            theme.AppendLine($"--{Palette}-primary-lighten: {ColorRgbLighten(Theme.Palette.Primary)};");
            theme.AppendLine($"--{Palette}-primary-hover: {ColorRgba(Theme.Palette.Primary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-secondary: {Theme.Palette.Secondary};");
            theme.AppendLine($"--{Palette}-secondary-rgb: {ColorRgbElements(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{Palette}-secondary-text: {Theme.Palette.SecondaryContrastText};");
            theme.AppendLine($"--{Palette}-secondary-darken: {ColorRgbDarken(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{Palette}-secondary-lighten: {ColorRgbLighten(Theme.Palette.Secondary)};");
            theme.AppendLine($"--{Palette}-secondary-hover: {ColorRgba(Theme.Palette.Secondary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-tertiary: {Theme.Palette.Tertiary};");
            theme.AppendLine($"--{Palette}-tertiary-rgb: {ColorRgbElements(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{Palette}-tertiary-text: {Theme.Palette.TertiaryContrastText};");
            theme.AppendLine($"--{Palette}-tertiary-darken: {ColorRgbDarken(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{Palette}-tertiary-lighten: {ColorRgbLighten(Theme.Palette.Tertiary)};");
            theme.AppendLine($"--{Palette}-tertiary-hover: {ColorRgba(Theme.Palette.Tertiary, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-info: {Theme.Palette.Info};");
            theme.AppendLine($"--{Palette}-info-rgb: {ColorRgbElements(Theme.Palette.Info)};");
            theme.AppendLine($"--{Palette}-info-text: {Theme.Palette.InfoContrastText};");
            theme.AppendLine($"--{Palette}-info-darken: {ColorRgbDarken(Theme.Palette.Info)};");
            theme.AppendLine($"--{Palette}-info-lighten: {ColorRgbLighten(Theme.Palette.Info)};");
            theme.AppendLine($"--{Palette}-info-hover: {ColorRgba(Theme.Palette.Info, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-success: {Theme.Palette.Success};");
            theme.AppendLine($"--{Palette}-success-rgb: {ColorRgbElements(Theme.Palette.Success)};");
            theme.AppendLine($"--{Palette}-success-text: {Theme.Palette.SuccessContrastText};");
            theme.AppendLine($"--{Palette}-success-darken: {ColorRgbDarken(Theme.Palette.Success)};");
            theme.AppendLine($"--{Palette}-success-lighten: {ColorRgbLighten(Theme.Palette.Success)};");
            theme.AppendLine($"--{Palette}-success-hover: {ColorRgba(Theme.Palette.Success, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-warning: {Theme.Palette.Warning};");
            theme.AppendLine($"--{Palette}-warning-rgb: {ColorRgbElements(Theme.Palette.Warning)};");
            theme.AppendLine($"--{Palette}-warning-text: {Theme.Palette.WarningContrastText};");
            theme.AppendLine($"--{Palette}-warning-darken: {ColorRgbDarken(Theme.Palette.Warning)};");
            theme.AppendLine($"--{Palette}-warning-lighten: {ColorRgbLighten(Theme.Palette.Warning)};");
            theme.AppendLine($"--{Palette}-warning-hover: {ColorRgba(Theme.Palette.Warning, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-error: {Theme.Palette.Error};");
            theme.AppendLine($"--{Palette}-error-rgb: {ColorRgbElements(Theme.Palette.Error)};");
            theme.AppendLine($"--{Palette}-error-text: {Theme.Palette.ErrorContrastText};");
            theme.AppendLine($"--{Palette}-error-darken: {ColorRgbDarken(Theme.Palette.Error)};");
            theme.AppendLine($"--{Palette}-error-lighten: {ColorRgbLighten(Theme.Palette.Error)};");
            theme.AppendLine($"--{Palette}-error-hover: {ColorRgba(Theme.Palette.Error, Theme.Palette.HoverOpacity)};");
            theme.AppendLine($"--{Palette}-dark: {Theme.Palette.Dark};");
            theme.AppendLine($"--{Palette}-dark-rgb: {ColorRgbElements(Theme.Palette.Dark)};");
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

            theme.AppendLine($"--{Palette}-table-lines: {Theme.Palette.TableLines};");
            theme.AppendLine($"--{Palette}-table-striped: {Theme.Palette.TableStriped};");
            theme.AppendLine($"--{Palette}-table-hover: {Theme.Palette.TableHover};");

            theme.AppendLine($"--{Palette}-divider: {Theme.Palette.Divider};");
            theme.AppendLine($"--{Palette}-divider-light: {Theme.Palette.DividerLight};");

            theme.AppendLine($"--{Palette}-grey-default: {Theme.Palette.GrayDefault};");
            theme.AppendLine($"--{Palette}-grey-light: {Theme.Palette.GrayLight};");
            theme.AppendLine($"--{Palette}-grey-lighter: {Theme.Palette.GrayLighter};");
            theme.AppendLine($"--{Palette}-grey-dark: {Theme.Palette.GrayDark};");
            theme.AppendLine($"--{Palette}-grey-darker: {Theme.Palette.GrayDarker};");

            theme.AppendLine($"--{Palette}-overlay-dark: {Theme.Palette.OverlayDark};");
            theme.AppendLine($"--{Palette}-overlay-light: {Theme.Palette.OverlayLight};");

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
#pragma warning disable CS0612 // Type or member is obsolete
            if (!string.IsNullOrEmpty(Theme.LayoutProperties.DrawerWidth))
            {
                theme.AppendLine($"--{LayoutProperties}-drawer-width-left: {Theme.LayoutProperties.DrawerWidth};");
                theme.AppendLine($"--{LayoutProperties}-drawer-width-right: {Theme.LayoutProperties.DrawerWidth};");
            }
#pragma warning restore CS0612 // Type or member is obsolete
            else
            {
                theme.AppendLine($"--{LayoutProperties}-drawer-width-left: {Theme.LayoutProperties.DrawerWidthLeft};");
                theme.AppendLine($"--{LayoutProperties}-drawer-width-right: {Theme.LayoutProperties.DrawerWidthRight};");
            }
            theme.AppendLine($"--{LayoutProperties}-appbar-min-height: {Theme.LayoutProperties.AppbarMinHeight};");

            //Breakpoint
            //theme.AppendLine($"--{Breakpoint}-xs: {Theme.Breakpoints.xs};");
            //theme.AppendLine($"--{Breakpoint}-sm: {Theme.Breakpoints.sm};");
            //theme.AppendLine($"--{Breakpoint}-md: {Theme.Breakpoints.md};");
            //theme.AppendLine($"--{Breakpoint}-lg: {Theme.Breakpoints.lg};");
            //theme.AppendLine($"--{Breakpoint}-xl: {Theme.Breakpoints.xl};");

            //Typography
            theme.AppendLine($"--{Typography}-default-family: '{string.Join("','", Theme.Typography.Default.FontFamily)}';");
            theme.AppendLine($"--{Typography}-default-size: {Theme.Typography.Default.FontSize};");
            theme.AppendLine($"--{Typography}-default-weight: {Theme.Typography.Default.FontWeight};");
            theme.AppendLine($"--{Typography}-default-lineheight: {Theme.Typography.Default.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-default-letterspacing: {Theme.Typography.Default.LetterSpacing};");

            theme.AppendLine($"--{Typography}-h1-family: '{string.Join("','", Theme.Typography.H1.FontFamily)}';");
            theme.AppendLine($"--{Typography}-h1-size: {Theme.Typography.H1.FontSize};");
            theme.AppendLine($"--{Typography}-h1-weight: {Theme.Typography.H1.FontWeight};");
            theme.AppendLine($"--{Typography}-h1-lineheight: {Theme.Typography.H1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h1-letterspacing: {Theme.Typography.H1.LetterSpacing};");

            theme.AppendLine($"--{Typography}-h2-family: '{string.Join("','", Theme.Typography.H2.FontFamily)}';");
            theme.AppendLine($"--{Typography}-h2-size: {Theme.Typography.H2.FontSize};");
            theme.AppendLine($"--{Typography}-h2-weight: {Theme.Typography.H2.FontWeight};");
            theme.AppendLine($"--{Typography}-h2-lineheight: {Theme.Typography.H2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h2-letterspacing: {Theme.Typography.H2.LetterSpacing};");

            theme.AppendLine($"--{Typography}-h3-family: '{string.Join("','", Theme.Typography.H3.FontFamily)}';");
            theme.AppendLine($"--{Typography}-h3-size: {Theme.Typography.H3.FontSize};");
            theme.AppendLine($"--{Typography}-h3-weight: {Theme.Typography.H3.FontWeight};");
            theme.AppendLine($"--{Typography}-h3-lineheight: {Theme.Typography.H3.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h3-letterspacing: {Theme.Typography.H3.LetterSpacing};");

            theme.AppendLine($"--{Typography}-h4-family: '{string.Join("','", Theme.Typography.H4.FontFamily)}';");
            theme.AppendLine($"--{Typography}-h4-size: {Theme.Typography.H4.FontSize};");
            theme.AppendLine($"--{Typography}-h4-weight: {Theme.Typography.H4.FontWeight};");
            theme.AppendLine($"--{Typography}-h4-lineheight: {Theme.Typography.H4.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h4-letterspacing: {Theme.Typography.H4.LetterSpacing};");

            theme.AppendLine($"--{Typography}-h5-family: '{string.Join("','", Theme.Typography.H5.FontFamily)}';");
            theme.AppendLine($"--{Typography}-h5-size: {Theme.Typography.H5.FontSize};");
            theme.AppendLine($"--{Typography}-h5-weight: {Theme.Typography.H5.FontWeight};");
            theme.AppendLine($"--{Typography}-h5-lineheight: {Theme.Typography.H5.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h5-letterspacing: {Theme.Typography.H5.LetterSpacing};");

            theme.AppendLine($"--{Typography}-h6-family: '{string.Join("','", Theme.Typography.H6.FontFamily)}';");
            theme.AppendLine($"--{Typography}-h6-size: {Theme.Typography.H6.FontSize};");
            theme.AppendLine($"--{Typography}-h6-weight: {Theme.Typography.H6.FontWeight};");
            theme.AppendLine($"--{Typography}-h6-lineheight: {Theme.Typography.H6.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h6-letterspacing: {Theme.Typography.H6.LetterSpacing};");

            theme.AppendLine($"--{Typography}-subtitle1-family: '{string.Join("','", Theme.Typography.Subtitle1.FontFamily)}';");
            theme.AppendLine($"--{Typography}-subtitle1-size: {Theme.Typography.Subtitle1.FontSize};");
            theme.AppendLine($"--{Typography}-subtitle1-weight: {Theme.Typography.Subtitle1.FontWeight};");
            theme.AppendLine($"--{Typography}-subtitle1-lineheight: {Theme.Typography.Subtitle1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-subtitle1-letterspacing: {Theme.Typography.Subtitle1.LetterSpacing};");

            theme.AppendLine($"--{Typography}-subtitle2-family: '{string.Join("','", Theme.Typography.Subtitle2.FontFamily)}';");
            theme.AppendLine($"--{Typography}-subtitle2-size: {Theme.Typography.Subtitle2.FontSize};");
            theme.AppendLine($"--{Typography}-subtitle2-weight: {Theme.Typography.Subtitle2.FontWeight};");
            theme.AppendLine($"--{Typography}-subtitle2-lineheight: {Theme.Typography.Subtitle2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-subtitle2-letterspacing: {Theme.Typography.Subtitle2.LetterSpacing};");

            theme.AppendLine($"--{Typography}-body1-family: '{string.Join("','", Theme.Typography.Body1.FontFamily)}';");
            theme.AppendLine($"--{Typography}-body1-size: {Theme.Typography.Body1.FontSize};");
            theme.AppendLine($"--{Typography}-body1-weight: {Theme.Typography.Body1.FontWeight};");
            theme.AppendLine($"--{Typography}-body1-lineheight: {Theme.Typography.Body1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-body1-letterspacing: {Theme.Typography.Body1.LetterSpacing};");

            theme.AppendLine($"--{Typography}-body2-family: '{string.Join("','", Theme.Typography.Body2.FontFamily)}';");
            theme.AppendLine($"--{Typography}-body2-size: {Theme.Typography.Body2.FontSize};");
            theme.AppendLine($"--{Typography}-body2-weight: {Theme.Typography.Body2.FontWeight};");
            theme.AppendLine($"--{Typography}-body2-lineheight: {Theme.Typography.Body2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-body2-letterspacing: {Theme.Typography.Body2.LetterSpacing};");

            theme.AppendLine($"--{Typography}-button-family: '{string.Join("','", Theme.Typography.Button.FontFamily)}';");
            theme.AppendLine($"--{Typography}-button-size: {Theme.Typography.Button.FontSize};");
            theme.AppendLine($"--{Typography}-button-weight: {Theme.Typography.Button.FontWeight};");
            theme.AppendLine($"--{Typography}-button-lineheight: {Theme.Typography.Button.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-button-letterspacing: {Theme.Typography.Button.LetterSpacing};");

            theme.AppendLine($"--{Typography}-caption-family: '{string.Join("','", Theme.Typography.Caption.FontFamily)}';");
            theme.AppendLine($"--{Typography}-caption-size: {Theme.Typography.Caption.FontSize};");
            theme.AppendLine($"--{Typography}-caption-weight: {Theme.Typography.Caption.FontWeight};");
            theme.AppendLine($"--{Typography}-caption-lineheight: {Theme.Typography.Caption.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-caption-letterspacing: {Theme.Typography.Caption.LetterSpacing};");

            theme.AppendLine($"--{Typography}-overline-family: '{string.Join("','", Theme.Typography.Overline.FontFamily)}';");
            theme.AppendLine($"--{Typography}-overline-size: {Theme.Typography.Overline.FontSize};");
            theme.AppendLine($"--{Typography}-overline-weight: {Theme.Typography.Overline.FontWeight};");
            theme.AppendLine($"--{Typography}-overline-lineheight: {Theme.Typography.Overline.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-overline-letterspacing: {Theme.Typography.Overline.LetterSpacing};");


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
