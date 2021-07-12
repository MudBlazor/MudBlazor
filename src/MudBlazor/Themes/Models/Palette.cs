using MudBlazor.Utilities;

namespace MudBlazor
{
    public class Palette
    {
        private MudColor _primaryDarken = null;
        private MudColor _primaryLighten = null;
        private MudColor _secondaryDarken = null;
        private MudColor _secondaryLighten = null;
        private MudColor _tertiaryDarken = null;
        private MudColor _tertiaryLighten = null;
        private MudColor _infoDarken = null;
        private MudColor _infoLighten = null;
        private MudColor _successDarken = null;
        private MudColor _successLighten = null;
        private MudColor _warningDarken = null;
        private MudColor _warningLighten = null;
        private MudColor _errorDarken = null;
        private MudColor _errorLighten = null;
        private MudColor _darkDarken = null;
        private MudColor _darkLighten = null;

        public MudColor Black { get; set; } = Colors.Shades.Black;
        public MudColor White { get; set; } = Colors.Shades.White;
        public MudColor Primary { get; set; } = "#594AE2";
        public MudColor PrimaryContrastText { get; set; } = Colors.Shades.White;
        public MudColor Secondary { get; set; } = Colors.Pink.Accent2;
        public MudColor SecondaryContrastText { get; set; } = Colors.Shades.White;
        public MudColor Tertiary { get; set; } = "#1EC8A5";
        public MudColor TertiaryContrastText { get; set; } = Colors.Shades.White;
        public MudColor Info { get; set; } = Colors.Blue.Default;
        public MudColor InfoContrastText { get; set; } = Colors.Shades.White;
        public MudColor Success { get; set; } = Colors.Green.Accent4;
        public MudColor SuccessContrastText { get; set; } = Colors.Shades.White;
        public MudColor Warning { get; set; } = Colors.Orange.Default;
        public MudColor WarningContrastText { get; set; } = Colors.Shades.White;
        public MudColor Error { get; set; } = Colors.Red.Default;
        public MudColor ErrorContrastText { get; set; } = Colors.Shades.White;
        public MudColor Dark { get; set; } = Colors.Grey.Darken3;
        public MudColor DarkContrastText { get; set; } = Colors.Shades.White;
        public MudColor TextPrimary { get; set; } = Colors.Grey.Darken3;
        public MudColor TextSecondary { get; set; } = new MudColor(Colors.Shades.Black, 0.54).ToRGBA();
        public MudColor TextDisabled { get; set; } = new MudColor(Colors.Shades.Black, 0.38).ToRGBA();
        public MudColor ActionDefault { get; set; } = new MudColor(Colors.Shades.Black, 0.54).ToRGBA();
        public MudColor ActionDisabled { get; set; } = new MudColor(Colors.Shades.Black, 0.26).ToRGBA();
        public MudColor ActionDisabledBackground { get; set; } = new MudColor(Colors.Shades.Black, 0.12).ToRGBA();
        public MudColor Background { get; set; } = Colors.Shades.White;
        public MudColor BackgroundGrey { get; set; } = Colors.Grey.Lighten4;
        public MudColor Surface { get; set; } = Colors.Shades.White;
        public MudColor DrawerBackground { get; set; } = Colors.Shades.White;
        public MudColor DrawerText { get; set; } = Colors.Grey.Darken3;
        public MudColor DrawerIcon { get; set; } = Colors.Grey.Darken2;
        public MudColor AppbarBackground { get; set; } = "#594AE2";
        public MudColor AppbarText { get; set; } = Colors.Shades.White;
        public MudColor LinesDefault { get; set; } = new MudColor(Colors.Shades.Black, 0.12).ToRGBA();
        public MudColor LinesInputs { get; set; } = Colors.Grey.Lighten1;
        public MudColor TableLines { get; set; } = new MudColor(Colors.Grey.Lighten2, 1).ToRGBA();
        public MudColor TableStriped { get; set; } = new MudColor(Colors.Shades.Black, 0.02).ToRGBA();
        public MudColor TableHover { get; set; } = new MudColor(Colors.Shades.Black, 0.04).ToRGBA();
        public MudColor Divider { get; set; } = Colors.Grey.Lighten2;
        public MudColor DividerLight { get; set; } = new MudColor(Colors.Shades.Black, 0.8).ToRGBA();

        public string PrimaryDarken
        {
            get => (_primaryDarken ??= Primary.ColorRgbDarken()).ToRGB();
            set => _primaryDarken = value;
        }
        public string PrimaryLighten
        {
            get => (_primaryLighten ??= Primary.ColorRgbLighten()).ToRGB();
            set => _primaryLighten = value;
        }
        public string SecondaryDarken
        {
            get => (_secondaryDarken ??= Secondary.ColorRgbDarken()).ToRGB();
            set => _secondaryDarken = value;
        }
        public string SecondaryLighten
        {
            get => (_secondaryLighten ??= Secondary.ColorRgbLighten()).ToRGB();
            set => _secondaryLighten = value;
        }
        public string TertiaryDarken
        {
            get => (_tertiaryDarken ??= Tertiary.ColorRgbDarken()).ToRGB();
            set => _tertiaryDarken = value;
        }
        public string TertiaryLighten
        {
            get => (_tertiaryLighten ??= Tertiary.ColorRgbLighten()).ToRGB();
            set => _tertiaryLighten = value;
        }
        public string InfoDarken
        {
            get => (_infoDarken ??= Info.ColorRgbDarken()).ToRGB();
            set => _infoDarken = value;
        }
        public string InfoLighten
        {
            get => (_infoLighten ??= Info.ColorRgbLighten()).ToRGB();
            set => _infoLighten = value;
        }
        public string SuccessDarken
        {
            get => (_successDarken ??= Success.ColorRgbDarken()).ToRGB();
            set => _successDarken = value;
        }
        public string SuccessLighten
        {
            get => (_successLighten ??= Success.ColorRgbLighten()).ToRGB();
            set => _successLighten = value;
        }
        public string WarningDarken
        {
            get => (_warningDarken ??= Warning.ColorRgbDarken()).ToRGB();
            set => _warningDarken = value;
        }
        public string WarningLighten
        {
            get => (_warningLighten ??= Warning.ColorRgbLighten()).ToRGB();
            set => _warningLighten = value;
        }
        public string ErrorDarken
        {
            get => (_errorDarken ??= Error.ColorRgbDarken()).ToRGB();
            set => _errorDarken = value;
        }
        public string ErrorLighten
        {
            get => (_errorLighten ??= Error.ColorRgbLighten()).ToRGB();
            set => _errorLighten = value;
        }
        public string DarkDarken
        {
            get => (_darkDarken ??= Dark.ColorRgbDarken()).ToRGB();
            set => _darkDarken = value;
        }
        public string DarkLighten
        {
            get => (_darkLighten ??= Dark.ColorRgbLighten()).ToRGB();
            set => _darkLighten = value;
        }

        public double HoverOpacity { get; set; } = 0.06;

        public string GrayDefault { get; set; } = Colors.Grey.Default;
        public string GrayLight { get; set; } = Colors.Grey.Lighten1;
        public string GrayLighter { get; set; } = Colors.Grey.Lighten2;
        public string GrayDark { get; set; } = Colors.Grey.Darken1;
        public string GrayDarker { get; set; } = Colors.Grey.Darken2;

        public string OverlayDark { get; set; } = new MudColor("#212121", 0.5).ToRGBA();
        public string OverlayLight { get; set; } = new MudColor(Colors.Shades.White, 0.5).ToRGBA();
    }
}
