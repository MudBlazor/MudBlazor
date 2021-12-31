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

        public MudColor Black { get; set; } = "#272c34";
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
        public MudColor TextSecondary { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.54).ToString(MudColorOutputFormats.RGBA);
        public MudColor TextDisabled { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.38).ToString(MudColorOutputFormats.RGBA);
        public MudColor ActionDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.54).ToString(MudColorOutputFormats.RGBA);
        public MudColor ActionDisabled { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.26).ToString(MudColorOutputFormats.RGBA);
        public MudColor ActionDisabledBackground { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);
        public MudColor Background { get; set; } = Colors.Shades.White;
        public MudColor BackgroundGrey { get; set; } = Colors.Grey.Lighten4;
        public MudColor Surface { get; set; } = Colors.Shades.White;
        public MudColor DrawerBackground { get; set; } = Colors.Shades.White;
        public MudColor DrawerText { get; set; } = Colors.Grey.Darken3;
        public MudColor DrawerIcon { get; set; } = Colors.Grey.Darken2;
        public MudColor AppbarBackground { get; set; } = "#594AE2";
        public MudColor AppbarText { get; set; } = Colors.Shades.White;
        public MudColor LinesDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);
        public MudColor LinesInputs { get; set; } = Colors.Grey.Lighten1;
        public MudColor TableLines { get; set; } = new MudColor(Colors.Grey.Lighten2).SetAlpha(1.0).ToString(MudColorOutputFormats.RGBA);
        public MudColor TableStriped { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.02).ToString(MudColorOutputFormats.RGBA);
        public MudColor TableHover { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.04).ToString(MudColorOutputFormats.RGBA);
        public MudColor Divider { get; set; } = Colors.Grey.Lighten2;
        public MudColor DividerLight { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.8).ToString(MudColorOutputFormats.RGBA);

        public string PrimaryDarken
        {
            get => (_primaryDarken ??= Primary.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _primaryDarken = value;
        }
        public string PrimaryLighten
        {
            get => (_primaryLighten ??= Primary.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _primaryLighten = value;
        }
        public string SecondaryDarken
        {
            get => (_secondaryDarken ??= Secondary.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _secondaryDarken = value;
        }
        public string SecondaryLighten
        {
            get => (_secondaryLighten ??= Secondary.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _secondaryLighten = value;
        }
        public string TertiaryDarken
        {
            get => (_tertiaryDarken ??= Tertiary.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _tertiaryDarken = value;
        }
        public string TertiaryLighten
        {
            get => (_tertiaryLighten ??= Tertiary.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _tertiaryLighten = value;
        }
        public string InfoDarken
        {
            get => (_infoDarken ??= Info.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _infoDarken = value;
        }
        public string InfoLighten
        {
            get => (_infoLighten ??= Info.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _infoLighten = value;
        }
        public string SuccessDarken
        {
            get => (_successDarken ??= Success.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _successDarken = value;
        }
        public string SuccessLighten
        {
            get => (_successLighten ??= Success.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _successLighten = value;
        }
        public string WarningDarken
        {
            get => (_warningDarken ??= Warning.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _warningDarken = value;
        }
        public string WarningLighten
        {
            get => (_warningLighten ??= Warning.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _warningLighten = value;
        }
        public string ErrorDarken
        {
            get => (_errorDarken ??= Error.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _errorDarken = value;
        }
        public string ErrorLighten
        {
            get => (_errorLighten ??= Error.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _errorLighten = value;
        }
        public string DarkDarken
        {
            get => (_darkDarken ??= Dark.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _darkDarken = value;
        }
        public string DarkLighten
        {
            get => (_darkLighten ??= Dark.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _darkLighten = value;
        }

        public double HoverOpacity { get; set; } = 0.06;

        public string GrayDefault { get; set; } = Colors.Grey.Default;
        public string GrayLight { get; set; } = Colors.Grey.Lighten1;
        public string GrayLighter { get; set; } = Colors.Grey.Lighten2;
        public string GrayDark { get; set; } = Colors.Grey.Darken1;
        public string GrayDarker { get; set; } = Colors.Grey.Darken2;

        public string OverlayDark { get; set; } = new MudColor("#212121").SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA);
        public string OverlayLight { get; set; } = new MudColor(Colors.Shades.White).SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA);

        internal static Palette ConvertToDarkTheme(Palette palette)
        {
            palette.Primary = "#776be7";
            palette.Black = "#27272f";
            palette.Background = "#32333d";
            palette.BackgroundGrey = "#27272f";
            palette.Surface = "#373740";
            palette.DrawerBackground = "#27272f";
            palette.DrawerText = "rgba(255,255,255, 0.50)";
            palette.DrawerIcon = "rgba(255,255,255, 0.50)";
            palette.AppbarBackground = "#27272f";
            palette.AppbarText = "rgba(255,255,255, 0.70)";
            palette.TextPrimary = "rgba(255,255,255, 0.70)";
            palette.TextSecondary = "rgba(255,255,255, 0.50)";
            palette.ActionDefault = "#adadb1";
            palette.ActionDisabled = "rgba(255,255,255, 0.26)";
            palette.ActionDisabledBackground = "rgba(255,255,255, 0.12)";
            palette.Divider = "rgba(255,255,255, 0.12)";
            palette.DividerLight = "rgba(255,255,255, 0.06)";
            palette.TableLines = "rgba(255,255,255, 0.12)";
            palette.LinesDefault = "rgba(255,255,255, 0.12)";
            palette.LinesInputs = "rgba(255,255,255, 0.3)";
            palette.TextDisabled = "rgba(255,255,255, 0.2)";
            palette.Info = "#3299ff";
            palette.Success = "#0bba83";
            palette.Warning = "#ffa800";
            palette.Error = "#f64e62";
            palette.Dark = "#27272f";
            return palette;
        }
    }
}
