using MudBlazor.Utilities;

namespace MudBlazor
{
    public class Palette
    {
        private string _primaryDarken = null;
        private string _primaryLighten = null;
        private string _secondaryDarken = null;
        private string _secondaryLighten = null;
        private string _tertiaryDarken = null;
        private string _tertiaryLighten = null;
        private string _infoDarken = null;
        private string _infoLighten = null;
        private string _successDarken = null;
        private string _successLighten = null;
        private string _warningDarken = null;
        private string _warningLighten = null;
        private string _errorDarken = null;
        private string _errorLighten = null;
        private string _darkDarken = null;
        private string _darkLighten = null;

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
        public string TableLines { get; set; } = ColorManager.ToRgbaFromHex(Colors.Grey.Lighten2, 1);
        public string TableStriped { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.02);
        public string TableHover { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.04);
        public string Divider { get; set; } = Colors.Grey.Lighten2;
        public string DividerLight { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.8);

        public string PrimaryDarken
        {
            get => _primaryDarken ??= ColorManager.ColorRgbDarken(Primary);
            set => _primaryDarken = value;
        }
        public string PrimaryLighten
        {
            get => _primaryLighten ??= ColorManager.ColorRgbLighten(Primary);
            set => _primaryLighten = value;
        }
        public string SecondaryDarken
        {
            get => _secondaryDarken ??= ColorManager.ColorRgbDarken(Secondary);
            set => _secondaryDarken = value;
        }
        public string SecondaryLighten
        {
            get => _secondaryLighten ??= ColorManager.ColorRgbLighten(Secondary);
            set => _secondaryLighten = value;
        }
        public string TertiaryDarken
        {
            get => _tertiaryDarken ??= ColorManager.ColorRgbDarken(Tertiary);
            set => _tertiaryDarken = value;
        }
        public string TertiaryLighten
        {
            get => _tertiaryLighten ??= ColorManager.ColorRgbLighten(Tertiary);
            set => _tertiaryLighten = value;
        }
        public string InfoDarken
        {
            get => _infoDarken ??= ColorManager.ColorRgbDarken(Info);
            set => _infoDarken = value;
        }
        public string InfoLighten
        {
            get => _infoLighten ??= ColorManager.ColorRgbLighten(Info);
            set => _infoLighten = value;
        }
        public string SuccessDarken
        {
            get => _successDarken ??= ColorManager.ColorRgbDarken(Success);
            set => _successDarken = value;
        }
        public string SuccessLighten
        {
            get => _successLighten ??= ColorManager.ColorRgbLighten(Success);
            set => _successLighten = value;
        }
        public string WarningDarken
        {
            get => _warningDarken ??= ColorManager.ColorRgbDarken(Warning);
            set => _warningDarken = value;
        }
        public string WarningLighten
        {
            get => _warningLighten ??= ColorManager.ColorRgbLighten(Warning);
            set => _warningLighten = value;
        }
        public string ErrorDarken
        {
            get => _errorDarken ??= ColorManager.ColorRgbDarken(Error);
            set => _errorDarken = value;
        }
        public string ErrorLighten
        {
            get => _errorLighten ??= ColorManager.ColorRgbLighten(Error);
            set => _errorLighten = value;
        }
        public string DarkDarken
        {
            get => _darkDarken ??= ColorManager.ColorRgbDarken(Dark);
            set => _darkDarken = value;
        }
        public string DarkLighten
        {
            get => _darkLighten ??= ColorManager.ColorRgbLighten(Dark);
            set => _darkLighten = value;
        }

        public double HoverOpacity { get; set; } = 0.06;

        public string GrayDefault { get; set; } = Colors.Grey.Default;
        public string GrayLight { get; set; } = Colors.Grey.Lighten1;
        public string GrayLighter { get; set; } = Colors.Grey.Lighten2;
        public string GrayDark { get; set; } = Colors.Grey.Darken1;
        public string GrayDarker { get; set; } = Colors.Grey.Darken2;

        public string OverlayDark { get; set; } = ColorManager.ToRgbaFromHex("#212121", 0.5);
        public string OverlayLight { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.White, 0.5);
    }
}
