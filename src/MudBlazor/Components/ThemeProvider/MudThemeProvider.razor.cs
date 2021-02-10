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
        private const string Color = "mud-color";

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
            theme.AppendLine($"--{LayoutProperties}-drawer-width: {Theme.LayoutProperties.DrawerWidth};");
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
            
            //Color
            theme.AppendLine($"--{Color}-red-default: {Colors.Red.Default};");
            theme.AppendLine($"--{Color}-red-lighten5: {Colors.Red.Lighten5};");
            theme.AppendLine($"--{Color}-red-lighten4: {Colors.Red.Lighten4};");
            theme.AppendLine($"--{Color}-red-lighten3: {Colors.Red.Lighten3};");
            theme.AppendLine($"--{Color}-red-lighten2: {Colors.Red.Lighten2};");
            theme.AppendLine($"--{Color}-red-lighten1: {Colors.Red.Lighten1};");
            theme.AppendLine($"--{Color}-red-darken1: {Colors.Red.Darken1};");
            theme.AppendLine($"--{Color}-red-darken2: {Colors.Red.Darken2};");
            theme.AppendLine($"--{Color}-red-darken3: {Colors.Red.Darken3};");
            theme.AppendLine($"--{Color}-red-darken4: {Colors.Red.Darken4};");
            theme.AppendLine($"--{Color}-red-accent1: {Colors.Red.Accent1};");
            theme.AppendLine($"--{Color}-red-accent2: {Colors.Red.Accent2};");
            theme.AppendLine($"--{Color}-red-accent3: {Colors.Red.Accent3};");
            theme.AppendLine($"--{Color}-red-accent4: {Colors.Red.Accent4};");
            
            theme.AppendLine($"--{Color}-pink-default: {Colors.Pink.Default};");
            theme.AppendLine($"--{Color}-pink-lighten5: {Colors.Pink.Lighten5};");
            theme.AppendLine($"--{Color}-pink-lighten4: {Colors.Pink.Lighten4};");
            theme.AppendLine($"--{Color}-pink-lighten3: {Colors.Pink.Lighten3};");
            theme.AppendLine($"--{Color}-pink-lighten2: {Colors.Pink.Lighten2};");
            theme.AppendLine($"--{Color}-pink-lighten1: {Colors.Pink.Lighten1};");
            theme.AppendLine($"--{Color}-pink-darken1: {Colors.Pink.Darken1};");
            theme.AppendLine($"--{Color}-pink-darken2: {Colors.Pink.Darken2};");
            theme.AppendLine($"--{Color}-pink-darken3: {Colors.Pink.Darken3};");
            theme.AppendLine($"--{Color}-pink-darken4: {Colors.Pink.Darken4};");
            theme.AppendLine($"--{Color}-pink-accent1: {Colors.Pink.Accent1};");
            theme.AppendLine($"--{Color}-pink-accent2: {Colors.Pink.Accent2};");
            theme.AppendLine($"--{Color}-pink-accent3: {Colors.Pink.Accent3};");
            theme.AppendLine($"--{Color}-pink-accent4: {Colors.Pink.Accent4};");

            theme.AppendLine($"--{Color}-purple-default: {Colors.Purple.Default};");
            theme.AppendLine($"--{Color}-purple-lighten5: {Colors.Purple.Lighten5};");
            theme.AppendLine($"--{Color}-purple-lighten4: {Colors.Purple.Lighten4};");
            theme.AppendLine($"--{Color}-purple-lighten3: {Colors.Purple.Lighten3};");
            theme.AppendLine($"--{Color}-purple-lighten2: {Colors.Purple.Lighten2};");
            theme.AppendLine($"--{Color}-purple-lighten1: {Colors.Purple.Lighten1};");
            theme.AppendLine($"--{Color}-purple-darken1: {Colors.Purple.Darken1};");
            theme.AppendLine($"--{Color}-purple-darken2: {Colors.Purple.Darken2};");
            theme.AppendLine($"--{Color}-purple-darken3: {Colors.Purple.Darken3};");
            theme.AppendLine($"--{Color}-purple-darken4: {Colors.Purple.Darken4};");
            theme.AppendLine($"--{Color}-purple-accent1: {Colors.Purple.Accent1};");
            theme.AppendLine($"--{Color}-purple-accent2: {Colors.Purple.Accent2};");
            theme.AppendLine($"--{Color}-purple-accent3: {Colors.Purple.Accent3};");
            theme.AppendLine($"--{Color}-purple-accent4: {Colors.Purple.Accent4};");
            
            theme.AppendLine($"--{Color}-deep-purple-default: {Colors.DeepPurple.Default};");
            theme.AppendLine($"--{Color}-deep-purple-lighten5: {Colors.DeepPurple.Lighten5};");
            theme.AppendLine($"--{Color}-deep-purple-lighten4: {Colors.DeepPurple.Lighten4};");
            theme.AppendLine($"--{Color}-deep-purple-lighten3: {Colors.DeepPurple.Lighten3};");
            theme.AppendLine($"--{Color}-deep-purple-lighten2: {Colors.DeepPurple.Lighten2};");
            theme.AppendLine($"--{Color}-deep-purple-lighten1: {Colors.DeepPurple.Lighten1};");
            theme.AppendLine($"--{Color}-deep-purple-darken1: {Colors.DeepPurple.Darken1};");
            theme.AppendLine($"--{Color}-deep-purple-darken2: {Colors.DeepPurple.Darken2};");
            theme.AppendLine($"--{Color}-deep-purple-darken3: {Colors.DeepPurple.Darken3};");
            theme.AppendLine($"--{Color}-deep-purple-darken4: {Colors.DeepPurple.Darken4};");
            theme.AppendLine($"--{Color}-deep-purple-accent1: {Colors.DeepPurple.Accent1};");
            theme.AppendLine($"--{Color}-deep-purple-accent2: {Colors.DeepPurple.Accent2};");
            theme.AppendLine($"--{Color}-deep-purple-accent3: {Colors.DeepPurple.Accent3};");
            theme.AppendLine($"--{Color}-deep-purple-accent4: {Colors.DeepPurple.Accent4};");
                    
            theme.AppendLine($"--{Color}-indigo-default: {Colors.Indigo.Default};");
            theme.AppendLine($"--{Color}-indigo-lighten5: {Colors.Indigo.Lighten5};");
            theme.AppendLine($"--{Color}-indigo-lighten4: {Colors.Indigo.Lighten4};");
            theme.AppendLine($"--{Color}-indigo-lighten3: {Colors.Indigo.Lighten3};");
            theme.AppendLine($"--{Color}-indigo-lighten2: {Colors.Indigo.Lighten2};");
            theme.AppendLine($"--{Color}-indigo-lighten1: {Colors.Indigo.Lighten1};");
            theme.AppendLine($"--{Color}-indigo-darken1: {Colors.Indigo.Darken1};");
            theme.AppendLine($"--{Color}-indigo-darken2: {Colors.Indigo.Darken2};");
            theme.AppendLine($"--{Color}-indigo-darken3: {Colors.Indigo.Darken3};");
            theme.AppendLine($"--{Color}-indigo-darken4: {Colors.Indigo.Darken4};");
            theme.AppendLine($"--{Color}-indigo-accent1: {Colors.Indigo.Accent1};");
            theme.AppendLine($"--{Color}-indigo-accent2: {Colors.Indigo.Accent2};");
            theme.AppendLine($"--{Color}-indigo-accent3: {Colors.Indigo.Accent3};");
            theme.AppendLine($"--{Color}-indigo-accent4: {Colors.Indigo.Accent4};");
                             
            theme.AppendLine($"--{Color}-blue-default: {Colors.Blue.Default};");
            theme.AppendLine($"--{Color}-blue-lighten5: {Colors.Blue.Lighten5};");
            theme.AppendLine($"--{Color}-blue-lighten4: {Colors.Blue.Lighten4};");
            theme.AppendLine($"--{Color}-blue-lighten3: {Colors.Blue.Lighten3};");
            theme.AppendLine($"--{Color}-blue-lighten2: {Colors.Blue.Lighten2};");
            theme.AppendLine($"--{Color}-blue-lighten1: {Colors.Blue.Lighten1};");
            theme.AppendLine($"--{Color}-blue-darken1: {Colors.Blue.Darken1};");
            theme.AppendLine($"--{Color}-blue-darken2: {Colors.Blue.Darken2};");
            theme.AppendLine($"--{Color}-blue-darken3: {Colors.Blue.Darken3};");
            theme.AppendLine($"--{Color}-blue-darken4: {Colors.Blue.Darken4};");
            theme.AppendLine($"--{Color}-blue-accent1: {Colors.Blue.Accent1};");
            theme.AppendLine($"--{Color}-blue-accent2: {Colors.Blue.Accent2};");
            theme.AppendLine($"--{Color}-blue-accent3: {Colors.Blue.Accent3};");
            theme.AppendLine($"--{Color}-blue-accent4: {Colors.Blue.Accent4};");
            
            theme.AppendLine($"--{Color}-light-blue-default: {Colors.LightBlue.Default};");
            theme.AppendLine($"--{Color}-light-blue-lighten5: {Colors.LightBlue.Lighten5};");
            theme.AppendLine($"--{Color}-light-blue-lighten4: {Colors.LightBlue.Lighten4};");
            theme.AppendLine($"--{Color}-light-blue-lighten3: {Colors.LightBlue.Lighten3};");
            theme.AppendLine($"--{Color}-light-blue-lighten2: {Colors.LightBlue.Lighten2};");
            theme.AppendLine($"--{Color}-light-blue-lighten1: {Colors.LightBlue.Lighten1};");
            theme.AppendLine($"--{Color}-light-blue-darken1: {Colors.LightBlue.Darken1};");
            theme.AppendLine($"--{Color}-light-blue-darken2: {Colors.LightBlue.Darken2};");
            theme.AppendLine($"--{Color}-light-blue-darken3: {Colors.LightBlue.Darken3};");
            theme.AppendLine($"--{Color}-light-blue-darken4: {Colors.LightBlue.Darken4};");
            theme.AppendLine($"--{Color}-light-blue-accent1: {Colors.LightBlue.Accent1};");
            theme.AppendLine($"--{Color}-light-blue-accent2: {Colors.LightBlue.Accent2};");
            theme.AppendLine($"--{Color}-light-blue-accent3: {Colors.LightBlue.Accent3};");
            theme.AppendLine($"--{Color}-light-blue-accent4: {Colors.LightBlue.Accent4};");
            
            theme.AppendLine($"--{Color}-cyan-default: {Colors.Cyan.Default};");
            theme.AppendLine($"--{Color}-cyan-lighten5: {Colors.Cyan.Lighten5};");
            theme.AppendLine($"--{Color}-cyan-lighten4: {Colors.Cyan.Lighten4};");
            theme.AppendLine($"--{Color}-cyan-lighten3: {Colors.Cyan.Lighten3};");
            theme.AppendLine($"--{Color}-cyan-lighten2: {Colors.Cyan.Lighten2};");
            theme.AppendLine($"--{Color}-cyan-lighten1: {Colors.Cyan.Lighten1};");
            theme.AppendLine($"--{Color}-cyan-darken1: {Colors.Cyan.Darken1};");
            theme.AppendLine($"--{Color}-cyan-darken2: {Colors.Cyan.Darken2};");
            theme.AppendLine($"--{Color}-cyan-darken3: {Colors.Cyan.Darken3};");
            theme.AppendLine($"--{Color}-cyan-darken4: {Colors.Cyan.Darken4};");
            theme.AppendLine($"--{Color}-cyan-accent1: {Colors.Cyan.Accent1};");
            theme.AppendLine($"--{Color}-cyan-accent2: {Colors.Cyan.Accent2};");
            theme.AppendLine($"--{Color}-cyan-accent3: {Colors.Cyan.Accent3};");
            theme.AppendLine($"--{Color}-cyan-accent4: {Colors.Cyan.Accent4};");
            
            theme.AppendLine($"--{Color}-teal-default: {Colors.Teal.Default};");
            theme.AppendLine($"--{Color}-teal-lighten5: {Colors.Teal.Lighten5};");
            theme.AppendLine($"--{Color}-teal-lighten4: {Colors.Teal.Lighten4};");
            theme.AppendLine($"--{Color}-teal-lighten3: {Colors.Teal.Lighten3};");
            theme.AppendLine($"--{Color}-teal-lighten2: {Colors.Teal.Lighten2};");
            theme.AppendLine($"--{Color}-teal-lighten1: {Colors.Teal.Lighten1};");
            theme.AppendLine($"--{Color}-teal-darken1: {Colors.Teal.Darken1};");
            theme.AppendLine($"--{Color}-teal-darken2: {Colors.Teal.Darken2};");
            theme.AppendLine($"--{Color}-teal-darken3: {Colors.Teal.Darken3};");
            theme.AppendLine($"--{Color}-teal-darken4: {Colors.Teal.Darken4};");
            theme.AppendLine($"--{Color}-teal-accent1: {Colors.Teal.Accent1};");
            theme.AppendLine($"--{Color}-teal-accent2: {Colors.Teal.Accent2};");
            theme.AppendLine($"--{Color}-teal-accent3: {Colors.Teal.Accent3};");
            theme.AppendLine($"--{Color}-teal-accent4: {Colors.Teal.Accent4};");
            
            theme.AppendLine($"--{Color}-green-default: {Colors.Green.Default};");
            theme.AppendLine($"--{Color}-green-lighten5: {Colors.Green.Lighten5};");
            theme.AppendLine($"--{Color}-green-lighten4: {Colors.Green.Lighten4};");
            theme.AppendLine($"--{Color}-green-lighten3: {Colors.Green.Lighten3};");
            theme.AppendLine($"--{Color}-green-lighten2: {Colors.Green.Lighten2};");
            theme.AppendLine($"--{Color}-green-lighten1: {Colors.Green.Lighten1};");
            theme.AppendLine($"--{Color}-green-darken1: {Colors.Green.Darken1};");
            theme.AppendLine($"--{Color}-green-darken2: {Colors.Green.Darken2};");
            theme.AppendLine($"--{Color}-green-darken3: {Colors.Green.Darken3};");
            theme.AppendLine($"--{Color}-green-darken4: {Colors.Green.Darken4};");
            theme.AppendLine($"--{Color}-green-accent1: {Colors.Green.Accent1};");
            theme.AppendLine($"--{Color}-green-accent2: {Colors.Green.Accent2};");
            theme.AppendLine($"--{Color}-green-accent3: {Colors.Green.Accent3};");
            theme.AppendLine($"--{Color}-green-accent4: {Colors.Green.Accent4};");
            
            theme.AppendLine($"--{Color}-light-green-default: {Colors.LightGreen.Default};");
            theme.AppendLine($"--{Color}-light-green-lighten5: {Colors.LightGreen.Lighten5};");
            theme.AppendLine($"--{Color}-light-green-lighten4: {Colors.LightGreen.Lighten4};");
            theme.AppendLine($"--{Color}-light-green-lighten3: {Colors.LightGreen.Lighten3};");
            theme.AppendLine($"--{Color}-light-green-lighten2: {Colors.LightGreen.Lighten2};");
            theme.AppendLine($"--{Color}-light-green-lighten1: {Colors.LightGreen.Lighten1};");
            theme.AppendLine($"--{Color}-light-green-darken1: {Colors.LightGreen.Darken1};");
            theme.AppendLine($"--{Color}-light-green-darken2: {Colors.LightGreen.Darken2};");
            theme.AppendLine($"--{Color}-light-green-darken3: {Colors.LightGreen.Darken3};");
            theme.AppendLine($"--{Color}-light-green-darken4: {Colors.LightGreen.Darken4};");
            theme.AppendLine($"--{Color}-light-green-accent1: {Colors.LightGreen.Accent1};");
            theme.AppendLine($"--{Color}-light-green-accent2: {Colors.LightGreen.Accent2};");
            theme.AppendLine($"--{Color}-light-green-accent3: {Colors.LightGreen.Accent3};");
            theme.AppendLine($"--{Color}-light-green-accent4: {Colors.LightGreen.Accent4};");

            theme.AppendLine($"--{Color}-lime-default: {Colors.Lime.Default};");
            theme.AppendLine($"--{Color}-lime-lighten5: {Colors.Lime.Lighten5};");
            theme.AppendLine($"--{Color}-lime-lighten4: {Colors.Lime.Lighten4};");
            theme.AppendLine($"--{Color}-lime-lighten3: {Colors.Lime.Lighten3};");
            theme.AppendLine($"--{Color}-lime-lighten2: {Colors.Lime.Lighten2};");
            theme.AppendLine($"--{Color}-lime-lighten1: {Colors.Lime.Lighten1};");
            theme.AppendLine($"--{Color}-lime-darken1: {Colors.Lime.Darken1};");
            theme.AppendLine($"--{Color}-lime-darken2: {Colors.Lime.Darken2};");
            theme.AppendLine($"--{Color}-lime-darken3: {Colors.Lime.Darken3};");
            theme.AppendLine($"--{Color}-lime-darken4: {Colors.Lime.Darken4};");
            theme.AppendLine($"--{Color}-lime-accent1: {Colors.Lime.Accent1};");
            theme.AppendLine($"--{Color}-lime-accent2: {Colors.Lime.Accent2};");
            theme.AppendLine($"--{Color}-lime-accent3: {Colors.Lime.Accent3};");
            theme.AppendLine($"--{Color}-lime-accent4: {Colors.Lime.Accent4};");
            
            theme.AppendLine($"--{Color}-yellow-default: {Colors.Yellow.Default};");
            theme.AppendLine($"--{Color}-yellow-lighten5: {Colors.Yellow.Lighten5};");
            theme.AppendLine($"--{Color}-yellow-lighten4: {Colors.Yellow.Lighten4};");
            theme.AppendLine($"--{Color}-yellow-lighten3: {Colors.Yellow.Lighten3};");
            theme.AppendLine($"--{Color}-yellow-lighten2: {Colors.Yellow.Lighten2};");
            theme.AppendLine($"--{Color}-yellow-lighten1: {Colors.Yellow.Lighten1};");
            theme.AppendLine($"--{Color}-yellow-darken1: {Colors.Yellow.Darken1};");
            theme.AppendLine($"--{Color}-yellow-darken2: {Colors.Yellow.Darken2};");
            theme.AppendLine($"--{Color}-yellow-darken3: {Colors.Yellow.Darken3};");
            theme.AppendLine($"--{Color}-yellow-darken4: {Colors.Yellow.Darken4};");
            theme.AppendLine($"--{Color}-yellow-accent1: {Colors.Yellow.Accent1};");
            theme.AppendLine($"--{Color}-yellow-accent2: {Colors.Yellow.Accent2};");
            theme.AppendLine($"--{Color}-yellow-accent3: {Colors.Yellow.Accent3};");
            theme.AppendLine($"--{Color}-yellow-accent4: {Colors.Yellow.Accent4};");
            
            theme.AppendLine($"--{Color}-amber-default: {Colors.Amber.Default};");
            theme.AppendLine($"--{Color}-amber-lighten5: {Colors.Amber.Lighten5};");
            theme.AppendLine($"--{Color}-amber-lighten4: {Colors.Amber.Lighten4};");
            theme.AppendLine($"--{Color}-amber-lighten3: {Colors.Amber.Lighten3};");
            theme.AppendLine($"--{Color}-amber-lighten2: {Colors.Amber.Lighten2};");
            theme.AppendLine($"--{Color}-amber-lighten1: {Colors.Amber.Lighten1};");
            theme.AppendLine($"--{Color}-amber-darken1: {Colors.Amber.Darken1};");
            theme.AppendLine($"--{Color}-amber-darken2: {Colors.Amber.Darken2};");
            theme.AppendLine($"--{Color}-amber-darken3: {Colors.Amber.Darken3};");
            theme.AppendLine($"--{Color}-amber-darken4: {Colors.Amber.Darken4};");
            theme.AppendLine($"--{Color}-amber-accent1: {Colors.Amber.Accent1};");
            theme.AppendLine($"--{Color}-amber-accent2: {Colors.Amber.Accent2};");
            theme.AppendLine($"--{Color}-amber-accent3: {Colors.Amber.Accent3};");
            theme.AppendLine($"--{Color}-amber-accent4: {Colors.Amber.Accent4};");
            
            theme.AppendLine($"--{Color}-orange-default: {Colors.Orange.Default};");
            theme.AppendLine($"--{Color}-orange-lighten5: {Colors.Orange.Lighten5};");
            theme.AppendLine($"--{Color}-orange-lighten4: {Colors.Orange.Lighten4};");
            theme.AppendLine($"--{Color}-orange-lighten3: {Colors.Orange.Lighten3};");
            theme.AppendLine($"--{Color}-orange-lighten2: {Colors.Orange.Lighten2};");
            theme.AppendLine($"--{Color}-orange-lighten1: {Colors.Orange.Lighten1};");
            theme.AppendLine($"--{Color}-orange-darken1: {Colors.Orange.Darken1};");
            theme.AppendLine($"--{Color}-orange-darken2: {Colors.Orange.Darken2};");
            theme.AppendLine($"--{Color}-orange-darken3: {Colors.Orange.Darken3};");
            theme.AppendLine($"--{Color}-orange-darken4: {Colors.Orange.Darken4};");
            theme.AppendLine($"--{Color}-orange-accent1: {Colors.Orange.Accent1};");
            theme.AppendLine($"--{Color}-orange-accent2: {Colors.Orange.Accent2};");
            theme.AppendLine($"--{Color}-orange-accent3: {Colors.Orange.Accent3};");
            theme.AppendLine($"--{Color}-orange-accent4: {Colors.Orange.Accent4};");
            
            theme.AppendLine($"--{Color}-deep-orange-default: {Colors.DeepOrange.Default};");
            theme.AppendLine($"--{Color}-deep-orange-lighten5: {Colors.DeepOrange.Lighten5};");
            theme.AppendLine($"--{Color}-deep-orange-lighten4: {Colors.DeepOrange.Lighten4};");
            theme.AppendLine($"--{Color}-deep-orange-lighten3: {Colors.DeepOrange.Lighten3};");
            theme.AppendLine($"--{Color}-deep-orange-lighten2: {Colors.DeepOrange.Lighten2};");
            theme.AppendLine($"--{Color}-deep-orange-lighten1: {Colors.DeepOrange.Lighten1};");
            theme.AppendLine($"--{Color}-deep-orange-darken1: {Colors.DeepOrange.Darken1};");
            theme.AppendLine($"--{Color}-deep-orange-darken2: {Colors.DeepOrange.Darken2};");
            theme.AppendLine($"--{Color}-deep-orange-darken3: {Colors.DeepOrange.Darken3};");
            theme.AppendLine($"--{Color}-deep-orange-darken4: {Colors.DeepOrange.Darken4};");
            theme.AppendLine($"--{Color}-deep-orange-accent1: {Colors.DeepOrange.Accent1};");
            theme.AppendLine($"--{Color}-deep-orange-accent2: {Colors.DeepOrange.Accent2};");
            theme.AppendLine($"--{Color}-deep-orange-accent3: {Colors.DeepOrange.Accent3};");
            theme.AppendLine($"--{Color}-deep-orange-accent4: {Colors.DeepOrange.Accent4};");
            
            theme.AppendLine($"--{Color}-brown-default: {Colors.Brown.Default};");
            theme.AppendLine($"--{Color}-brown-lighten5: {Colors.Brown.Lighten5};");
            theme.AppendLine($"--{Color}-brown-lighten4: {Colors.Brown.Lighten4};");
            theme.AppendLine($"--{Color}-brown-lighten3: {Colors.Brown.Lighten3};");
            theme.AppendLine($"--{Color}-brown-lighten2: {Colors.Brown.Lighten2};");
            theme.AppendLine($"--{Color}-brown-lighten1: {Colors.Brown.Lighten1};");
            theme.AppendLine($"--{Color}-brown-darken1: {Colors.Brown.Darken1};");
            theme.AppendLine($"--{Color}-brown-darken2: {Colors.Brown.Darken2};");
            theme.AppendLine($"--{Color}-brown-darken3: {Colors.Brown.Darken3};");
            theme.AppendLine($"--{Color}-brown-darken4: {Colors.Brown.Darken4};");

            theme.AppendLine($"--{Color}-blue-grey-default: {Colors.BlueGrey.Default};");
            theme.AppendLine($"--{Color}-blue-grey-lighten5: {Colors.BlueGrey.Lighten5};");
            theme.AppendLine($"--{Color}-blue-grey-lighten4: {Colors.BlueGrey.Lighten4};");
            theme.AppendLine($"--{Color}-blue-grey-lighten3: {Colors.BlueGrey.Lighten3};");
            theme.AppendLine($"--{Color}-blue-grey-lighten2: {Colors.BlueGrey.Lighten2};");
            theme.AppendLine($"--{Color}-blue-grey-lighten1: {Colors.BlueGrey.Lighten1};");
            theme.AppendLine($"--{Color}-blue-grey-darken1: {Colors.BlueGrey.Darken1};");
            theme.AppendLine($"--{Color}-blue-grey-darken2: {Colors.BlueGrey.Darken2};");
            theme.AppendLine($"--{Color}-blue-grey-darken3: {Colors.BlueGrey.Darken3};");
            theme.AppendLine($"--{Color}-blue-grey-darken4: {Colors.BlueGrey.Darken4};");

            theme.AppendLine($"--{Color}-grey-default: {Colors.Grey.Default};");
            theme.AppendLine($"--{Color}-grey-lighten5: {Colors.Grey.Lighten5};");
            theme.AppendLine($"--{Color}-grey-lighten4: {Colors.Grey.Lighten4};");
            theme.AppendLine($"--{Color}-grey-lighten3: {Colors.Grey.Lighten3};");
            theme.AppendLine($"--{Color}-grey-lighten2: {Colors.Grey.Lighten2};");
            theme.AppendLine($"--{Color}-grey-lighten1: {Colors.Grey.Lighten1};");
            theme.AppendLine($"--{Color}-grey-darken1: {Colors.Grey.Darken1};");
            theme.AppendLine($"--{Color}-grey-darken2: {Colors.Grey.Darken2};");
            theme.AppendLine($"--{Color}-grey-darken3: {Colors.Grey.Darken3};");
            theme.AppendLine($"--{Color}-grey-darken4: {Colors.Grey.Darken4};");

            theme.AppendLine($"--{Color}-shades-black: {Colors.Shades.Black};");
            theme.AppendLine($"--{Color}-shades-white: {Colors.Shades.White};");
            theme.AppendLine($"--{Color}-shades-transparent: {Colors.Shades.Transparent};");
        }
    }
}
