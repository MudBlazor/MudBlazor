using System;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a palette of colors used in the application.
    /// </summary>
    [Obsolete("This property will be abstract in a future update. Use PaletteLight, PaletteDark or your own implementation.", false)]
    public class Palette
    {
        private MudColor? _primaryDarken;
        private MudColor? _primaryLighten;
        private MudColor? _secondaryDarken;
        private MudColor? _secondaryLighten;
        private MudColor? _tertiaryDarken;
        private MudColor? _tertiaryLighten;
        private MudColor? _infoDarken;
        private MudColor? _infoLighten;
        private MudColor? _successDarken;
        private MudColor? _successLighten;
        private MudColor? _warningDarken;
        private MudColor? _warningLighten;
        private MudColor? _errorDarken;
        private MudColor? _errorLighten;
        private MudColor? _darkDarken;
        private MudColor? _darkLighten;

        /// <summary>
        /// Gets or sets the black color.
        /// </summary>
        public virtual MudColor Black { get; set; } = "#272c34";

        /// <summary>
        /// Gets or sets the white color.
        /// </summary>
        public virtual MudColor White { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        public virtual MudColor Primary { get; set; } = "#594AE2";

        /// <summary>
        /// Gets or sets the contrast text color for the primary color.
        /// </summary>
        public virtual MudColor PrimaryContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the secondary color.
        /// </summary>
        public virtual MudColor Secondary { get; set; } = Colors.Pink.Accent2;

        /// <summary>
        /// Gets or sets the contrast text color for the secondary color.
        /// </summary>
        public virtual MudColor SecondaryContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the tertiary color.
        /// </summary>
        public virtual MudColor Tertiary { get; set; } = "#1EC8A5";

        /// <summary>
        /// Gets or sets the contrast text color for the tertiary color.
        /// </summary>
        public virtual MudColor TertiaryContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the info color.
        /// </summary>
        public virtual MudColor Info { get; set; } = Colors.Blue.Default;

        /// <summary>
        /// Gets or sets the contrast text color for the info color.
        /// </summary>
        public virtual MudColor InfoContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the success color.
        /// </summary>
        public virtual MudColor Success { get; set; } = Colors.Green.Accent4;

        /// <summary>
        /// Gets or sets the contrast text color for the success color.
        /// </summary>
        public virtual MudColor SuccessContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the warning color.
        /// </summary>
        public virtual MudColor Warning { get; set; } = Colors.Orange.Default;

        /// <summary>
        /// Gets or sets the contrast text color for the warning color.
        /// </summary>
        public virtual MudColor WarningContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the error color.
        /// </summary>
        public virtual MudColor Error { get; set; } = Colors.Red.Default;

        /// <summary>
        /// Gets or sets the contrast text color for the error color.
        /// </summary>
        public virtual MudColor ErrorContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the dark color.
        /// </summary>
        public virtual MudColor Dark { get; set; } = Colors.Grey.Darken3;

        /// <summary>
        /// Gets or sets the contrast text color for the dark color.
        /// </summary>
        public virtual MudColor DarkContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the primary text color.
        /// </summary>
        public virtual MudColor TextPrimary { get; set; } = Colors.Grey.Darken3;

        /// <summary>
        /// Gets or sets the secondary text color.
        /// </summary>
        public virtual MudColor TextSecondary { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.54).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the disabled text color.
        /// </summary>
        public virtual MudColor TextDisabled { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.38).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the default action color.
        /// </summary>
        public virtual MudColor ActionDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.54).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the disabled action color.
        /// </summary>
        public virtual MudColor ActionDisabled { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.26).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the background color for disabled actions.
        /// </summary>
        public virtual MudColor ActionDisabledBackground { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);


        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public virtual MudColor Background { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the grey background color.
        /// </summary>
        public virtual MudColor BackgroundGrey { get; set; } = Colors.Grey.Lighten4;

        /// <summary>
        /// Gets or sets the surface color.
        /// </summary>
        public virtual MudColor Surface { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the drawer background color.
        /// </summary>
        public virtual MudColor DrawerBackground { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the drawer text color.
        /// </summary>
        public virtual MudColor DrawerText { get; set; } = Colors.Grey.Darken3;

        /// <summary>
        /// Gets or sets the drawer icon color.
        /// </summary>
        public virtual MudColor DrawerIcon { get; set; } = Colors.Grey.Darken2;

        /// <summary>
        /// Gets or sets the appbar background color.
        /// </summary>
        public virtual MudColor AppbarBackground { get; set; } = "#594AE2";

        /// <summary>
        /// Gets or sets the appbar text color.
        /// </summary>
        public virtual MudColor AppbarText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// Gets or sets the default color for lines.
        /// </summary>
        public virtual MudColor LinesDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the color for input lines.
        /// </summary>
        public virtual MudColor LinesInputs { get; set; } = Colors.Grey.Lighten1;

        /// <summary>
        /// Gets or sets the color for table lines.
        /// </summary>
        public virtual MudColor TableLines { get; set; } = new MudColor(Colors.Grey.Lighten2).SetAlpha(1.0).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the color for striped rows in a table.
        /// </summary>
        public virtual MudColor TableStriped { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.02).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the color for table rows on hover.
        /// </summary>
        public virtual MudColor TableHover { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.04).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the color for dividers.
        /// </summary>
        public virtual MudColor Divider { get; set; } = Colors.Grey.Lighten2;

        /// <summary>
        /// Gets or sets the light color for dividers.
        /// </summary>
        public virtual MudColor DividerLight { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.8).ToString(MudColorOutputFormats.RGBA);
        
        /// <summary>
        /// Gets or sets the default color for chips.
        /// </summary>
        public virtual MudColor ChipDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.08).ToString(MudColorOutputFormats.RGBA);
        
        /// <summary>
        /// Gets or sets the default hover color for chips.
        /// </summary>
        public virtual MudColor ChipDefaultHover { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the darkened value of the primary color.
        /// </summary>
        public string PrimaryDarken
        {
            get => (_primaryDarken ??= Primary.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _primaryDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the primary color.
        /// </summary>
        public string PrimaryLighten
        {
            get => (_primaryLighten ??= Primary.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _primaryLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the secondary color.
        /// </summary>
        public string SecondaryDarken
        {
            get => (_secondaryDarken ??= Secondary.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _secondaryDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the secondary color.
        /// </summary>
        public string SecondaryLighten
        {
            get => (_secondaryLighten ??= Secondary.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _secondaryLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the tertiary color.
        /// </summary>
        public string TertiaryDarken
        {
            get => (_tertiaryDarken ??= Tertiary.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _tertiaryDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the tertiary color.
        /// </summary>
        public string TertiaryLighten
        {
            get => (_tertiaryLighten ??= Tertiary.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _tertiaryLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the info color.
        /// </summary>
        public string InfoDarken
        {
            get => (_infoDarken ??= Info.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _infoDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the info color.
        /// </summary>
        public string InfoLighten
        {
            get => (_infoLighten ??= Info.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _infoLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the success color.
        /// </summary>
        public string SuccessDarken
        {
            get => (_successDarken ??= Success.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _successDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the success color.
        /// </summary>
        public string SuccessLighten
        {
            get => (_successLighten ??= Success.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _successLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the warning color.
        /// </summary>
        public string WarningDarken
        {
            get => (_warningDarken ??= Warning.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _warningDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the warning color.
        /// </summary>
        public string WarningLighten
        {
            get => (_warningLighten ??= Warning.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _warningLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the error color.
        /// </summary>
        public string ErrorDarken
        {
            get => (_errorDarken ??= Error.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _errorDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the error color.
        /// </summary>
        public string ErrorLighten
        {
            get => (_errorLighten ??= Error.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _errorLighten = value;
        }

        /// <summary>
        /// Gets or sets the darkened value of the dark color.
        /// </summary>
        public string DarkDarken
        {
            get => (_darkDarken ??= Dark.ColorRgbDarken()).ToString(MudColorOutputFormats.RGB);
            set => _darkDarken = value;
        }

        /// <summary>
        /// Gets or sets the lightened value of the dark color.
        /// </summary>
        public string DarkLighten
        {
            get => (_darkLighten ??= Dark.ColorRgbLighten()).ToString(MudColorOutputFormats.RGB);
            set => _darkLighten = value;
        }

        /// <summary>
        /// Gets or sets the opacity value for hover effect.
        /// </summary>
        public double HoverOpacity { get; set; } = 0.06;

        /// <summary>
        /// Gets or sets the default gray color.
        /// </summary>
        public string GrayDefault { get; set; } = Colors.Grey.Default;

        /// <summary>
        /// Gets or sets the lightened gray color.
        /// </summary>
        public string GrayLight { get; set; } = Colors.Grey.Lighten1;

        /// <summary>
        /// Gets or sets the further lightened gray color.
        /// </summary>
        public string GrayLighter { get; set; } = Colors.Grey.Lighten2;

        /// <summary>
        /// Gets or sets the darkened gray color.
        /// </summary>
        public string GrayDark { get; set; } = Colors.Grey.Darken1;

        /// <summary>
        /// Gets or sets the further darkened gray color.
        /// </summary>
        public string GrayDarker { get; set; } = Colors.Grey.Darken2;

        /// <summary>
        /// Gets or sets the dark overlay color.
        /// </summary>
        public string OverlayDark { get; set; } = new MudColor("#212121").SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Gets or sets the light overlay color.
        /// </summary>
        public string OverlayLight { get; set; } = new MudColor(Colors.Shades.White).SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA);
    }
}
