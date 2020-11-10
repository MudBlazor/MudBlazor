using MudBlazor.Utilities;

namespace MudBlazor.Theme.Defaults

{
    public class Palette
    {
        public string Black { get; set; } = Colors.Shades.Black;
        public string White { get; set; } = Colors.Shades.White;
        public string Primary { get; set; } = "#594AE2";
        public string Secondary { get; set; } = Colors.Pink.Accent2;
        public string Tertiary { get; set; } = Colors.Teal.Accent3;
        public string Info { get; set; } = Colors.Blue.Default;
        public string Success { get; set; } = Colors.Green.Accent4;
        public string Warning { get; set; } = Colors.Orange.Default;
        public string Error { get; set; } = Colors.Red.Default;
        public string Dark { get; set; } = Colors.Grey.Darken4;
        public string TextPrimary { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.87);
        public string TextSecondary { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.54);
        public string TextDisabled { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.38);
        public string ActionDefault { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.54);
        public string ActionDisabled { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.26);
        public string ActionDisabledBackground { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.12);
        public string Background { get; set; } = Colors.Shades.White;
        public string Surface { get; set; } = Colors.Shades.White;
        public string DrawerBackground { get; set; } = Colors.Shades.White;
        public string DrawerText { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.87);
        public string AppbarBackground { get; set; } = "#594AE2";
        public string AppbarText { get; set; } = Colors.Shades.White;
        public string LinesDefault { get; set; } = Colors.Grey.Lighten1;
        public string LinesInputs { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.12);
        public string Divider { get; set; } = Colors.Grey.Lighten2;
        public string DividerLight { get; set; } = ColorManager.ToRgbaFromHex(Colors.Shades.Black, 0.8);

        public double HoverOpacity { get; set; } = 0.04;
    }
}
