using System;
using System.Text;
using MudBlazor.Utilities;
using System.Globalization;
using MudColor = System.Drawing.Color;
using System.Runtime.InteropServices;

namespace MudBlazor
{
    public class MudTheme : DefaultMudTheme
    {
        protected virtual void GenerateTheme(StringBuilder theme)
        {
            theme.AppendLine($"--mud-theme-on-surface: {Color_OnSurface};");
            theme.AppendLine($"--mud-theme-surface: {Color_Surface};");
            theme.AppendLine($"--mud-theme-background: {Color_Background};");

            theme.AppendLine($"--mud-theme-text-color-default: {BuildColor(Color_Text_Default, ColorOption.RgbA, 0.87)};");

            theme.AppendLine($"--mud-theme-on-default: {Color_Default};");
            theme.AppendLine($"--mud-theme-color-default: {BuildColor(Color_Default, ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-color-default-lighten: {BuildColor(Color_Default, ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-color-default-darken: {BuildColor(Color_Default, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-color-default-hover: {BuildColor(Color_Text_Default, ColorOption.RgbA, 0.05)};");

            theme.AppendLine($"--mud-theme-on-primary: {Color_OnPrimary};");
            theme.AppendLine($"--mud-theme-color-primary: {BuildColor(Color_Primary,ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-color-primary-lighten: {BuildColor(Color_Primary,ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-color-primary-darken: {BuildColor(Color_Primary, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-color-primary-hover: {BuildColor(Color_Primary, ColorOption.RgbA, 0.05)};");

            theme.AppendLine($"--mud-theme-on-secondary: {Color_OnPrimary};");
            theme.AppendLine($"--mud-theme-color-secondary: {BuildColor(Color_Secondary, ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-color-secondary-lighten: {BuildColor(Color_Secondary, ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-color-secondary-darken: {BuildColor(Color_Secondary, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-color-secondary-hover: {BuildColor(Color_Secondary, ColorOption.RgbA, 0.05)};");

            theme.AppendLine($"--mud-theme-color-info: {Color_Info};");
            theme.AppendLine($"--mud-theme-color-success: {Color_Success};");
            theme.AppendLine($"--mud-theme-color-warning: {Color_Warning};");
            theme.AppendLine($"--mud-theme-color-danger: {Color_Danger};");
            theme.AppendLine($"--mud-theme-color-dark: {Color_Dark};");

            theme.AppendLine($"--mud-theme-border-radius-default: {BorderRadius_Default};");
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
        public string BuildColor(string Hex, ColorOption ColorOption, [Optional] double alpha)
        {
            string CssVar = "";
            MudColor Color = HexToRgb(Hex);

            switch (ColorOption)
            {
                case ColorOption.Rgb:
                    CssVar = $"rgb({Color.R},{Color.G},{Color.B})";
                    break;
                case ColorOption.RgbA:
                    string Alpha = alpha.ToString("G", CultureInfo.InvariantCulture);
                    CssVar = $"rgba({Color.R},{Color.G},{Color.B}, {Alpha})";
                    break;
                case ColorOption.Lighten:
                    Color = Lighten(Color);
                    CssVar = $"rgb({Color.R},{Color.G},{Color.B})";
                    break;
                case ColorOption.Darken:
                    Color = Darken(Color);
                    CssVar = $"rgb({Color.R},{Color.G},{Color.B})";
                    break;
            }
            return CssVar;
        }

        public MudColor HexToRgb(string hex)
        {
            return ColorManager.FromHex(hex);
        }

        public MudColor Lighten(MudColor color)
        {
            return ColorManager.ColorLighten(color);
        }
        public MudColor Darken(MudColor color)
        {
            return ColorManager.ColorDarken(color);
        }

        public enum ColorOption
        {
            Rgb,
            RgbA,
            Lighten,
            Darken
        }
    }
}
