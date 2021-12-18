using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class BaseMudThemeProvider : ComponentBase
    {
        [Parameter] public MudTheme Theme { get; set; }

        /// <summary>
        ///  If true, will not apply MudBlazor styled scrollbar and use browser default. 
        /// </summary>
        [Parameter] public bool DefaultScrollbar { get; set; }

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

        public string BuildMudBlazorScrollbar()
        {
            var scrollbar = new StringBuilder();
            scrollbar.AppendLine("<style>");
            scrollbar.AppendLine("::-webkit-scrollbar {width: 8px;height: 8px;z-index: 1;}");
            scrollbar.AppendLine("::-webkit-scrollbar-track {background: transparent;}");
            scrollbar.AppendLine("::-webkit-scrollbar-thumb {background: #c4c4c4;border-radius: 1px;}");
            scrollbar.AppendLine("::-webkit-scrollbar-thumb:hover {background: #a6a6a6;}");
            //Firefox
            scrollbar.AppendLine("html, body * {scrollbar-color: #c4c4c4 transparent;scrollbar-width: thin;}");
            scrollbar.AppendLine("</style>");
            return scrollbar.ToString();
        }

        // private const string Breakpoint = "mud-breakpoint";
        private const string Palette = "mud-palette";
        private const string Elevation = "mud-elevation";
        private const string Typography = "mud-typography";
        private const string LayoutProperties = "mud";
        private const string Zindex = "mud-zindex";

        protected virtual void GenerateTheme(StringBuilder theme)
        {
            //Palette
            theme.AppendLine($"--{Palette}-black: {Theme.CurrentPalette.Black};");
            theme.AppendLine($"--{Palette}-white: {Theme.CurrentPalette.White};");

            theme.AppendLine($"--{Palette}-primary: {Theme.CurrentPalette.Primary};");
            theme.AppendLine($"--{Palette}-primary-rgb: {Theme.CurrentPalette.Primary.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-primary-text: {Theme.CurrentPalette.PrimaryContrastText};");
            theme.AppendLine($"--{Palette}-primary-darken: {Theme.CurrentPalette.PrimaryDarken};");
            theme.AppendLine($"--{Palette}-primary-lighten: {Theme.CurrentPalette.PrimaryLighten};");
            theme.AppendLine($"--{Palette}-primary-hover: { Theme.CurrentPalette.Primary.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-secondary: {Theme.CurrentPalette.Secondary};");
            theme.AppendLine($"--{Palette}-secondary-rgb: {Theme.CurrentPalette.Secondary.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-secondary-text: {Theme.CurrentPalette.SecondaryContrastText};");
            theme.AppendLine($"--{Palette}-secondary-darken: {Theme.CurrentPalette.SecondaryDarken};");
            theme.AppendLine($"--{Palette}-secondary-lighten: {Theme.CurrentPalette.SecondaryLighten};");
            theme.AppendLine($"--{Palette}-secondary-hover: { Theme.CurrentPalette.Secondary.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-tertiary: {Theme.CurrentPalette.Tertiary};");
            theme.AppendLine($"--{Palette}-tertiary-rgb: {Theme.CurrentPalette.Tertiary.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-tertiary-text: {Theme.CurrentPalette.TertiaryContrastText};");
            theme.AppendLine($"--{Palette}-tertiary-darken: {Theme.CurrentPalette.TertiaryDarken};");
            theme.AppendLine($"--{Palette}-tertiary-lighten: {Theme.CurrentPalette.TertiaryLighten};");
            theme.AppendLine($"--{Palette}-tertiary-hover: { Theme.CurrentPalette.Tertiary.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-info: {Theme.CurrentPalette.Info};");
            theme.AppendLine($"--{Palette}-info-rgb: {Theme.CurrentPalette.Info.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-info-text: {Theme.CurrentPalette.InfoContrastText};");
            theme.AppendLine($"--{Palette}-info-darken: {Theme.CurrentPalette.InfoDarken};");
            theme.AppendLine($"--{Palette}-info-lighten: {Theme.CurrentPalette.InfoLighten};");
            theme.AppendLine($"--{Palette}-info-hover: { Theme.CurrentPalette.Info.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-success: {Theme.CurrentPalette.Success};");
            theme.AppendLine($"--{Palette}-success-rgb: {Theme.CurrentPalette.Success.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-success-text: {Theme.CurrentPalette.SuccessContrastText};");
            theme.AppendLine($"--{Palette}-success-darken: {Theme.CurrentPalette.SuccessDarken};");
            theme.AppendLine($"--{Palette}-success-lighten: {Theme.CurrentPalette.SuccessLighten};");
            theme.AppendLine($"--{Palette}-success-hover: { Theme.CurrentPalette.Success.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-warning: {Theme.CurrentPalette.Warning};");
            theme.AppendLine($"--{Palette}-warning-rgb: {Theme.CurrentPalette.Warning.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-warning-text: {Theme.CurrentPalette.WarningContrastText};");
            theme.AppendLine($"--{Palette}-warning-darken: {Theme.CurrentPalette.WarningDarken};");
            theme.AppendLine($"--{Palette}-warning-lighten: {Theme.CurrentPalette.WarningLighten};");
            theme.AppendLine($"--{Palette}-warning-hover: { Theme.CurrentPalette.Warning.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-error: {Theme.CurrentPalette.Error};");
            theme.AppendLine($"--{Palette}-error-rgb: {Theme.CurrentPalette.Error.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-error-text: {Theme.CurrentPalette.ErrorContrastText};");
            theme.AppendLine($"--{Palette}-error-darken: {Theme.CurrentPalette.ErrorDarken};");
            theme.AppendLine($"--{Palette}-error-lighten: {Theme.CurrentPalette.ErrorLighten};");
            theme.AppendLine($"--{Palette}-error-hover: { Theme.CurrentPalette.Error.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-dark: {Theme.CurrentPalette.Dark};");
            theme.AppendLine($"--{Palette}-dark-rgb: {Theme.CurrentPalette.Dark.ToString(MudColorOutputFormats.ColorElements)};");
            theme.AppendLine($"--{Palette}-dark-text: {Theme.CurrentPalette.DarkContrastText};");
            theme.AppendLine($"--{Palette}-dark-darken: {Theme.CurrentPalette.DarkDarken};");
            theme.AppendLine($"--{Palette}-dark-lighten: {Theme.CurrentPalette.DarkLighten};");
            theme.AppendLine($"--{Palette}-dark-hover: { Theme.CurrentPalette.Dark.SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");

            theme.AppendLine($"--{Palette}-text-primary: {Theme.CurrentPalette.TextPrimary};");
            theme.AppendLine($"--{Palette}-text-secondary: {Theme.CurrentPalette.TextSecondary};");
            theme.AppendLine($"--{Palette}-text-disabled: {Theme.CurrentPalette.TextDisabled};");

            theme.AppendLine($"--{Palette}-action-default: {Theme.CurrentPalette.ActionDefault};");
            theme.AppendLine($"--{Palette}-action-default-hover: { new MudColor(Colors.Shades.Black).SetAlpha(Theme.CurrentPalette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
            theme.AppendLine($"--{Palette}-action-disabled: {Theme.CurrentPalette.ActionDisabled};");
            theme.AppendLine($"--{Palette}-action-disabled-background: {Theme.CurrentPalette.ActionDisabledBackground};");

            theme.AppendLine($"--{Palette}-surface: {Theme.CurrentPalette.Surface};");
            theme.AppendLine($"--{Palette}-background: {Theme.CurrentPalette.Background};");
            theme.AppendLine($"--{Palette}-background-grey: {Theme.CurrentPalette.BackgroundGrey};");
            theme.AppendLine($"--{Palette}-drawer-background: {Theme.CurrentPalette.DrawerBackground};");
            theme.AppendLine($"--{Palette}-drawer-text: {Theme.CurrentPalette.DrawerText};");
            theme.AppendLine($"--{Palette}-drawer-icon: {Theme.CurrentPalette.DrawerIcon};");
            theme.AppendLine($"--{Palette}-appbar-background: {Theme.CurrentPalette.AppbarBackground};");
            theme.AppendLine($"--{Palette}-appbar-text: {Theme.CurrentPalette.AppbarText};");

            theme.AppendLine($"--{Palette}-lines-default: {Theme.CurrentPalette.LinesDefault};");
            theme.AppendLine($"--{Palette}-lines-inputs: {Theme.CurrentPalette.LinesInputs};");

            theme.AppendLine($"--{Palette}-table-lines: {Theme.CurrentPalette.TableLines};");
            theme.AppendLine($"--{Palette}-table-striped: {Theme.CurrentPalette.TableStriped};");
            theme.AppendLine($"--{Palette}-table-hover: {Theme.CurrentPalette.TableHover};");

            theme.AppendLine($"--{Palette}-divider: {Theme.CurrentPalette.Divider};");
            theme.AppendLine($"--{Palette}-divider-light: {Theme.CurrentPalette.DividerLight};");

            theme.AppendLine($"--{Palette}-grey-default: {Theme.CurrentPalette.GrayDefault};");
            theme.AppendLine($"--{Palette}-grey-light: {Theme.CurrentPalette.GrayLight};");
            theme.AppendLine($"--{Palette}-grey-lighter: {Theme.CurrentPalette.GrayLighter};");
            theme.AppendLine($"--{Palette}-grey-dark: {Theme.CurrentPalette.GrayDark};");
            theme.AppendLine($"--{Palette}-grey-darker: {Theme.CurrentPalette.GrayDarker};");

            theme.AppendLine($"--{Palette}-overlay-dark: {Theme.CurrentPalette.OverlayDark};");
            theme.AppendLine($"--{Palette}-overlay-light: {Theme.CurrentPalette.OverlayLight};");

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
            theme.AppendLine($"--{LayoutProperties}-drawer-width-left: {Theme.LayoutProperties.DrawerWidthLeft};");
            theme.AppendLine($"--{LayoutProperties}-drawer-width-right: {Theme.LayoutProperties.DrawerWidthRight};");
            theme.AppendLine($"--{LayoutProperties}-drawer-width-mini-left: {Theme.LayoutProperties.DrawerMiniWidthLeft};");
            theme.AppendLine($"--{LayoutProperties}-drawer-width-mini-right: {Theme.LayoutProperties.DrawerMiniWidthRight};");
            theme.AppendLine($"--{LayoutProperties}-appbar-height: {Theme.LayoutProperties.AppbarHeight};");

            //Breakpoint
            //theme.AppendLine($"--{Breakpoint}-xs: {Theme.Breakpoints.xs};");
            //theme.AppendLine($"--{Breakpoint}-sm: {Theme.Breakpoints.sm};");
            //theme.AppendLine($"--{Breakpoint}-md: {Theme.Breakpoints.md};");
            //theme.AppendLine($"--{Breakpoint}-lg: {Theme.Breakpoints.lg};");
            //theme.AppendLine($"--{Breakpoint}-xl: {Theme.Breakpoints.xl};");
            //theme.AppendLine($"--{Breakpoint}-xxl: {Theme.Breakpoints.xxl};");

            //Typography
            theme.AppendLine($"--{Typography}-default-family: '{string.Join("','", Theme.Typography.Default.FontFamily)}';");
            theme.AppendLine($"--{Typography}-default-size: {Theme.Typography.Default.FontSize};");
            theme.AppendLine($"--{Typography}-default-weight: {Theme.Typography.Default.FontWeight};");
            theme.AppendLine($"--{Typography}-default-lineheight: {Theme.Typography.Default.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-default-letterspacing: {Theme.Typography.Default.LetterSpacing};");
            theme.AppendLine($"--{Typography}-default-text-transform: {Theme.Typography.Default.TextTransform};");

            theme.AppendLine($"--{Typography}-h1-family: '{string.Join("','", (Theme.Typography.H1.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.H1.FontFamily))}';");
            theme.AppendLine($"--{Typography}-h1-size: {Theme.Typography.H1.FontSize};");
            theme.AppendLine($"--{Typography}-h1-weight: {Theme.Typography.H1.FontWeight};");
            theme.AppendLine($"--{Typography}-h1-lineheight: {Theme.Typography.H1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h1-letterspacing: {Theme.Typography.H1.LetterSpacing};");
            theme.AppendLine($"--{Typography}-h1-text-transform: {Theme.Typography.H1.TextTransform};");

            theme.AppendLine($"--{Typography}-h2-family: '{string.Join("','", (Theme.Typography.H2.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.H2.FontFamily))}';");
            theme.AppendLine($"--{Typography}-h2-size: {Theme.Typography.H2.FontSize};");
            theme.AppendLine($"--{Typography}-h2-weight: {Theme.Typography.H2.FontWeight};");
            theme.AppendLine($"--{Typography}-h2-lineheight: {Theme.Typography.H2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h2-letterspacing: {Theme.Typography.H2.LetterSpacing};");
            theme.AppendLine($"--{Typography}-h2-text-transform: {Theme.Typography.H2.TextTransform};");

            theme.AppendLine($"--{Typography}-h3-family: '{string.Join("','", (Theme.Typography.H3.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.H3.FontFamily))}';");
            theme.AppendLine($"--{Typography}-h3-size: {Theme.Typography.H3.FontSize};");
            theme.AppendLine($"--{Typography}-h3-weight: {Theme.Typography.H3.FontWeight};");
            theme.AppendLine($"--{Typography}-h3-lineheight: {Theme.Typography.H3.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h3-letterspacing: {Theme.Typography.H3.LetterSpacing};");
            theme.AppendLine($"--{Typography}-h3-text-transform: {Theme.Typography.H3.TextTransform};");

            theme.AppendLine($"--{Typography}-h4-family: '{string.Join("','", (Theme.Typography.H4.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.H4.FontFamily))}';");
            theme.AppendLine($"--{Typography}-h4-size: {Theme.Typography.H4.FontSize};");
            theme.AppendLine($"--{Typography}-h4-weight: {Theme.Typography.H4.FontWeight};");
            theme.AppendLine($"--{Typography}-h4-lineheight: {Theme.Typography.H4.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h4-letterspacing: {Theme.Typography.H4.LetterSpacing};");
            theme.AppendLine($"--{Typography}-h4-text-transform: {Theme.Typography.H4.TextTransform};");

            theme.AppendLine($"--{Typography}-h5-family: '{string.Join("','", (Theme.Typography.H5.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.H5.FontFamily))}';");
            theme.AppendLine($"--{Typography}-h5-size: {Theme.Typography.H5.FontSize};");
            theme.AppendLine($"--{Typography}-h5-weight: {Theme.Typography.H5.FontWeight};");
            theme.AppendLine($"--{Typography}-h5-lineheight: {Theme.Typography.H5.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h5-letterspacing: {Theme.Typography.H5.LetterSpacing};");
            theme.AppendLine($"--{Typography}-h5-text-transform: {Theme.Typography.H5.TextTransform};");

            theme.AppendLine($"--{Typography}-h6-family: '{string.Join("','", (Theme.Typography.H6.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.H6.FontFamily))}';");
            theme.AppendLine($"--{Typography}-h6-size: {Theme.Typography.H6.FontSize};");
            theme.AppendLine($"--{Typography}-h6-weight: {Theme.Typography.H6.FontWeight};");
            theme.AppendLine($"--{Typography}-h6-lineheight: {Theme.Typography.H6.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-h6-letterspacing: {Theme.Typography.H6.LetterSpacing};");
            theme.AppendLine($"--{Typography}-h6-text-transform: {Theme.Typography.H6.TextTransform};");

            theme.AppendLine($"--{Typography}-subtitle1-family: '{string.Join("','", (Theme.Typography.Subtitle1.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Subtitle1.FontFamily))}';");
            theme.AppendLine($"--{Typography}-subtitle1-size: {Theme.Typography.Subtitle1.FontSize};");
            theme.AppendLine($"--{Typography}-subtitle1-weight: {Theme.Typography.Subtitle1.FontWeight};");
            theme.AppendLine($"--{Typography}-subtitle1-lineheight: {Theme.Typography.Subtitle1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-subtitle1-letterspacing: {Theme.Typography.Subtitle1.LetterSpacing};");
            theme.AppendLine($"--{Typography}-subtitle1-text-transform: {Theme.Typography.Subtitle1.TextTransform};");

            theme.AppendLine($"--{Typography}-subtitle2-family: '{string.Join("','", (Theme.Typography.Subtitle2.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Subtitle2.FontFamily))}';");
            theme.AppendLine($"--{Typography}-subtitle2-size: {Theme.Typography.Subtitle2.FontSize};");
            theme.AppendLine($"--{Typography}-subtitle2-weight: {Theme.Typography.Subtitle2.FontWeight};");
            theme.AppendLine($"--{Typography}-subtitle2-lineheight: {Theme.Typography.Subtitle2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-subtitle2-letterspacing: {Theme.Typography.Subtitle2.LetterSpacing};");
            theme.AppendLine($"--{Typography}-subtitle2-text-transform: {Theme.Typography.Subtitle2.TextTransform};");

            theme.AppendLine($"--{Typography}-body1-family: '{string.Join("','", (Theme.Typography.Body1.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Body1.FontFamily))}';");
            theme.AppendLine($"--{Typography}-body1-size: {Theme.Typography.Body1.FontSize};");
            theme.AppendLine($"--{Typography}-body1-weight: {Theme.Typography.Body1.FontWeight};");
            theme.AppendLine($"--{Typography}-body1-lineheight: {Theme.Typography.Body1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-body1-letterspacing: {Theme.Typography.Body1.LetterSpacing};");
            theme.AppendLine($"--{Typography}-body1-text-transform: {Theme.Typography.Body1.TextTransform};");

            theme.AppendLine($"--{Typography}-body2-family: '{string.Join("','", (Theme.Typography.Body2.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Body2.FontFamily))}';");
            theme.AppendLine($"--{Typography}-body2-size: {Theme.Typography.Body2.FontSize};");
            theme.AppendLine($"--{Typography}-body2-weight: {Theme.Typography.Body2.FontWeight};");
            theme.AppendLine($"--{Typography}-body2-lineheight: {Theme.Typography.Body2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-body2-letterspacing: {Theme.Typography.Body2.LetterSpacing};");
            theme.AppendLine($"--{Typography}-body2-text-transform: {Theme.Typography.Body2.TextTransform};");

            theme.AppendLine($"--{Typography}-button-family: '{string.Join("','", (Theme.Typography.Button.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Button.FontFamily))}';");
            theme.AppendLine($"--{Typography}-button-size: {Theme.Typography.Button.FontSize};");
            theme.AppendLine($"--{Typography}-button-weight: {Theme.Typography.Button.FontWeight};");
            theme.AppendLine($"--{Typography}-button-lineheight: {Theme.Typography.Button.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-button-letterspacing: {Theme.Typography.Button.LetterSpacing};");
            theme.AppendLine($"--{Typography}-button-text-transform: {Theme.Typography.Button.TextTransform};");

            theme.AppendLine($"--{Typography}-caption-family: '{string.Join("','", (Theme.Typography.Caption.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Caption.FontFamily))}';");
            theme.AppendLine($"--{Typography}-caption-size: {Theme.Typography.Caption.FontSize};");
            theme.AppendLine($"--{Typography}-caption-weight: {Theme.Typography.Caption.FontWeight};");
            theme.AppendLine($"--{Typography}-caption-lineheight: {Theme.Typography.Caption.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-caption-letterspacing: {Theme.Typography.Caption.LetterSpacing};");
            theme.AppendLine($"--{Typography}-caption-text-transform: {Theme.Typography.Caption.TextTransform};");

            theme.AppendLine($"--{Typography}-overline-family: '{string.Join("','", (Theme.Typography.Overline.FontFamily == null ? Theme.Typography.Default.FontFamily : Theme.Typography.Overline.FontFamily))}';");
            theme.AppendLine($"--{Typography}-overline-size: {Theme.Typography.Overline.FontSize};");
            theme.AppendLine($"--{Typography}-overline-weight: {Theme.Typography.Overline.FontWeight};");
            theme.AppendLine($"--{Typography}-overline-lineheight: {Theme.Typography.Overline.LineHeight.ToString(CultureInfo.InvariantCulture)};");
            theme.AppendLine($"--{Typography}-overline-letterspacing: {Theme.Typography.Overline.LetterSpacing};");
            theme.AppendLine($"--{Typography}-overline-text-transform: {Theme.Typography.Overline.TextTransform};");


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
