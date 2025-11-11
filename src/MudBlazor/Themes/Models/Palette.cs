// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a palette of colors used throughout the application.
    /// </summary>
    public record class Palette
    {
        private string? _primaryDarken;
        private string? _primaryLighten;
        private string? _secondaryDarken;
        private string? _secondaryLighten;
        private string? _tertiaryDarken;
        private string? _tertiaryLighten;
        private string? _infoDarken;
        private string? _infoLighten;
        private string? _successDarken;
        private string? _successLighten;
        private string? _warningDarken;
        private string? _warningLighten;
        private string? _errorDarken;
        private string? _errorLighten;
        private string? _darkDarken;
        private string? _darkLighten;

        /// <summary>
        /// The black color.
        /// </summary>
        public MudColor Black { get; set; } = "#272c34";

        /// <summary>
        /// The white color.
        /// </summary>
        public MudColor White { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The primary color.
        /// </summary>
        public MudColor Primary { get; set; } = "#594AE2";

        /// <summary>
        /// The contrast text color for the primary color.
        /// </summary>
        public MudColor PrimaryContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The secondary color.
        /// </summary>
        public MudColor Secondary { get; set; } = Colors.Pink.Accent2;

        /// <summary>
        /// The contrast text color for the secondary color.
        /// </summary>
        public MudColor SecondaryContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The tertiary color.
        /// </summary>
        public MudColor Tertiary { get; set; } = "#1EC8A5";

        /// <summary>
        /// The contrast text color for the tertiary color.
        /// </summary>
        public MudColor TertiaryContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The info color.
        /// </summary>
        public MudColor Info { get; set; } = Colors.Blue.Default;

        /// <summary>
        /// The contrast text color for the info color.
        /// </summary>
        public MudColor InfoContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The success color.
        /// </summary>
        public MudColor Success { get; set; } = Colors.Green.Accent4;

        /// <summary>
        /// The contrast text color for the success color.
        /// </summary>
        public MudColor SuccessContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The warning color.
        /// </summary>
        public MudColor Warning { get; set; } = Colors.Orange.Default;

        /// <summary>
        /// The contrast text color for the warning color.
        /// </summary>
        public MudColor WarningContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The error color.
        /// </summary>
        public MudColor Error { get; set; } = Colors.Red.Default;

        /// <summary>
        /// The contrast text color for the error color.
        /// </summary>
        public MudColor ErrorContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The dark color.
        /// </summary>
        public MudColor Dark { get; set; } = Colors.Gray.Darken3;

        /// <summary>
        /// The contrast text color for the dark color.
        /// </summary>
        public MudColor DarkContrastText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The primary text color.
        /// </summary>
        public MudColor TextPrimary { get; set; } = Colors.Gray.Darken3;

        /// <summary>
        /// The secondary text color.
        /// </summary>
        public MudColor TextSecondary { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.54).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The disabled text color.
        /// </summary>
        public MudColor TextDisabled { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.38).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The default action color.
        /// </summary>
        public MudColor ActionDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.54).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The disabled action color.
        /// </summary>
        public MudColor ActionDisabled { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.26).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The background color for disabled actions.
        /// </summary>
        public MudColor ActionDisabledBackground { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The background color.
        /// </summary>
        public MudColor Background { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The gray background color.
        /// </summary>
        public MudColor BackgroundGray { get; set; } = Colors.Gray.Lighten4;

        /// <summary>
        /// The surface color.
        /// </summary>
        public MudColor Surface { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The drawer background color.
        /// </summary>
        public MudColor DrawerBackground { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The drawer text color.
        /// </summary>
        public MudColor DrawerText { get; set; } = Colors.Gray.Darken3;

        /// <summary>
        /// The drawer icon color.
        /// </summary>
        public MudColor DrawerIcon { get; set; } = Colors.Gray.Darken2;

        /// <summary>
        /// The appbar background color.
        /// </summary>
        public MudColor AppbarBackground { get; set; } = "#594AE2";

        /// <summary>
        /// The appbar text color.
        /// </summary>
        public MudColor AppbarText { get; set; } = Colors.Shades.White;

        /// <summary>
        /// The default color for lines.
        /// </summary>
        public MudColor LinesDefault { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.12).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The color for input lines.
        /// </summary>
        public MudColor LinesInputs { get; set; } = Colors.Gray.Lighten1;

        /// <summary>
        /// The color for table lines.
        /// </summary>
        public MudColor TableLines { get; set; } = new MudColor(Colors.Gray.Lighten2).SetAlpha(1.0).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The color for striped rows in a table.
        /// </summary>
        public MudColor TableStriped { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.02).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The color for table rows on hover.
        /// </summary>
        public MudColor TableHover { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.04).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The color for dividers.
        /// </summary>
        public MudColor Divider { get; set; } = Colors.Gray.Lighten2;

        /// <summary>
        /// The light color for dividers.
        /// </summary>
        public MudColor DividerLight { get; set; } = new MudColor(Colors.Shades.Black).SetAlpha(0.8).ToString(MudColorOutputFormats.RGBA);


        /// <summary>
        /// The color for skeletons.
        /// </summary>
        public MudColor Skeleton { get; set; } = new MudColor("rgba(0, 0, 0, 0.11)").ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The darkened value of the primary color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string PrimaryDarken
        {
            get => _primaryDarken ??= Primary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _primaryDarken = value;
        }

        /// <summary>
        /// The lightened value of the primary color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string PrimaryLighten
        {
            get => _primaryLighten ??= Primary.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _primaryLighten = value;
        }

        /// <summary>
        /// The darkened value of the secondary color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string SecondaryDarken
        {
            get => _secondaryDarken ??= Secondary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _secondaryDarken = value;
        }

        /// <summary>
        /// The lightened value of the secondary color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string SecondaryLighten
        {
            get => _secondaryLighten ??= Secondary.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _secondaryLighten = value;
        }

        /// <summary>
        /// The darkened value of the tertiary color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string TertiaryDarken
        {
            get => _tertiaryDarken ??= Tertiary.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _tertiaryDarken = value;
        }

        /// <summary>
        /// The lightened value of the tertiary color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string TertiaryLighten
        {
            get => _tertiaryLighten ??= Tertiary.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _tertiaryLighten = value;
        }

        /// <summary>
        /// The darkened value of the info color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string InfoDarken
        {
            get => _infoDarken ??= Info.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _infoDarken = value;
        }

        /// <summary>
        /// The lightened value of the info color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string InfoLighten
        {
            get => _infoLighten ??= Info.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _infoLighten = value;
        }

        /// <summary>
        /// The darkened value of the success color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string SuccessDarken
        {
            get => _successDarken ??= Success.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _successDarken = value;
        }

        /// <summary>
        /// The lightened value of the success color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string SuccessLighten
        {
            get => _successLighten ??= Success.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _successLighten = value;
        }

        /// <summary>
        /// The darkened value of the warning color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string WarningDarken
        {
            get => _warningDarken ??= Warning.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _warningDarken = value;
        }

        /// <summary>
        /// The lightened value of the warning color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string WarningLighten
        {
            get => _warningLighten ??= Warning.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _warningLighten = value;
        }

        /// <summary>
        /// The darkened value of the error color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string ErrorDarken
        {
            get => _errorDarken ??= Error.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _errorDarken = value;
        }

        /// <summary>
        /// The lightened value of the error color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string ErrorLighten
        {
            get => _errorLighten ??= Error.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _errorLighten = value;
        }

        /// <summary>
        /// The darkened value of the dark color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbDarken"/> if not set.
        /// </summary>
        public string DarkDarken
        {
            get => _darkDarken ??= Dark.ColorRgbDarken().ToString(MudColorOutputFormats.RGB);
            set => _darkDarken = value;
        }

        /// <summary>
        /// The lightened value of the dark color.<br/>
        /// This is calculated using <see cref="MudColor.ColorRgbLighten"/> if not set.
        /// </summary>
        public string DarkLighten
        {
            get => _darkLighten ??= Dark.ColorRgbLighten().ToString(MudColorOutputFormats.RGB);
            set => _darkLighten = value;
        }

        /// <summary>
        /// The opacity value for most borders.
        /// </summary>
        public double BorderOpacity { get; set; } = 1.0;

        /// <summary>
        /// The opacity value for hover effect.
        /// </summary>
        public double HoverOpacity { get; set; } = 0.06;

        /// <summary>
        /// The opacity for the ripple effect.
        /// </summary>
        public double RippleOpacity { get; set; } = 0.1;

        /// <summary>
        /// The opacity for the ripple effect on specific elements like filled buttons.
        /// </summary>
        public double RippleOpacitySecondary { get; set; } = 0.2;

        /// <summary>
        /// The default gray color.
        /// </summary>
        public string GrayDefault { get; set; } = Colors.Gray.Default;

        /// <summary>
        /// The lightened gray color.
        /// </summary>
        public string GrayLight { get; set; } = Colors.Gray.Lighten1;

        /// <summary>
        /// The further lightened gray color.
        /// </summary>
        public string GrayLighter { get; set; } = Colors.Gray.Lighten2;

        /// <summary>
        /// The darkened gray color.
        /// </summary>
        public string GrayDark { get; set; } = Colors.Gray.Darken1;

        /// <summary>
        /// The further darkened gray color.
        /// </summary>
        public string GrayDarker { get; set; } = Colors.Gray.Darken2;

        /// <summary>
        /// The dark overlay color.
        /// </summary>
        public string OverlayDark { get; set; } = new MudColor("#212121").SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// The light overlay color.
        /// </summary>
        public string OverlayLight { get; set; } = new MudColor(Colors.Shades.White).SetAlpha(0.5).ToString(MudColorOutputFormats.RGBA);
    }
}
