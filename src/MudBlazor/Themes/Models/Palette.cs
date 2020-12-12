using MudBlazor.Utilities;

namespace MudBlazor

{
    public class Palette
    {
        public string Black { get; set; } = Colors.Shades.Black;
        public string White { get; set; } = Colors.Shades.White;
        public string Primary { get; set; } = "#594AE2";
        public string PrimaryContrastText { get; set; } = Colors.Shades.White;
        public string Secondary { get; set; } = Colors.Pink.Accent2;
        public string SecondaryContrastText { get; set; } = Colors.Shades.White;
        public string Tertiary { get; set; } = "#1EC8A5";
        public string TertiaryContrastText { get; set; } = Colors.Shades.White;
        public string Info { get; set; } = Colors.Blue.Default;
        public string InfoContrastText { get; set; } = Colors.Shades.White;
        public string Success { get; set; } = Colors.Green.Accent4;
        public string SuccessContrastText { get; set; } = Colors.Shades.White;
        public string Warning { get; set; } = Colors.Orange.Default;
        public string WarningContrastText { get; set; } = Colors.Shades.White;
        public string Error { get; set; } = Colors.Red.Default;
        public string ErrorContrastText { get; set; } = Colors.Shades.White;
        public string Dark { get; set; } = Colors.Grey.Darken3;
        public string DarkContrastText { get; set; } = Colors.Shades.White;
        public string TextPrimary { get; set; } = Colors.Grey.Darken3;
        public string TextSecondary { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.54);
        public string TextDisabled { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.38);
        public string ActionDefault { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.54);
        public string ActionDisabled { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.26);
        public string ActionDisabledBackground { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.12);
        public string Background { get; set; } = Colors.Shades.White;
        public string BackgroundGrey { get; set; } = Colors.Grey.Lighten4;
        public string Surface { get; set; } = Colors.Shades.White;
        public string DrawerBackground { get; set; } = Colors.Shades.White;
        public string DrawerText { get; set; } = Colors.Grey.Darken3;
        public string DrawerIcon { get; set; } = Colors.Grey.Darken2;
        public string AppbarBackground { get; set; } = "#594AE2";
        public string AppbarText { get; set; } = Colors.Shades.White;
        public string LinesDefault { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.12);
        public string LinesInputs { get; set; } = Colors.Grey.Lighten1;
        public string Divider { get; set; } = Colors.Grey.Lighten2;
        public string DividerLight { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.8);

        public double HoverOpacity { get; set; } = 0.06;

        public string GrayDefault { get; set; } = Colors.Grey.Default;
        public string GrayLight { get; set; } = Colors.Grey.Lighten1;
        public string GrayLighter { get; set; } = Colors.Grey.Lighten2;
        public string GrayDark { get; set; } = Colors.Grey.Darken1;
        public string GrayDarker { get; set; } = Colors.Grey.Darken2;

        public string OverlayDark { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.5);
        public string OverlayLight { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.White, 0.5);
    }
}
