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

            theme.AppendLine($"--mud-theme-text-default: {BuildColor(Color_Text_Default, ColorOption.RgbA, 0.87)};");
            theme.AppendLine($"--mud-theme-text-secondary: {BuildColor(Color_Text_Default, ColorOption.RgbA, 0.54)};");

            theme.AppendLine($"--mud-theme-color-default: {BuildColor(Color_Default, ColorOption.RgbA, 0.54)};");
            theme.AppendLine($"--mud-theme-color-on-default: {BuildColor(Color_OnDefault, ColorOption.RgbA, 0.87)};");
            theme.AppendLine($"--mud-theme-color-default-lighten: {BuildColor(Color_Default, ColorOption.RgbA, 0.1)};");
            theme.AppendLine($"--mud-theme-color-default-darken: {BuildColor(Color_Default, ColorOption.RgbA, 0.2)};");
            theme.AppendLine($"--mud-theme-color-default-hover: {BuildColor(Color_Default, ColorOption.RgbA, 0.04)};");

            
            theme.AppendLine($"--mud-theme-color-primary: {BuildColor(Color_Primary,ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-color-on-primary: {Color_OnPrimary};");
            theme.AppendLine($"--mud-theme-color-primary-lighten: {BuildColor(Color_Primary,ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-color-primary-darken: {BuildColor(Color_Primary, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-color-primary-hover: {BuildColor(Color_Primary, ColorOption.RgbA, 0.06)};");

            
            theme.AppendLine($"--mud-theme-color-secondary: {BuildColor(Color_Secondary, ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-color-on-secondary: {Color_OnSecondary};");
            theme.AppendLine($"--mud-theme-color-secondary-lighten: {BuildColor(Color_Secondary, ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-color-secondary-darken: {BuildColor(Color_Secondary, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-color-secondary-hover: {BuildColor(Color_Secondary, ColorOption.RgbA, 0.06)};");

            
            
            

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

        public string BuildColor(string Hex, ColorOption ColorOption, [Optional] double alpha, [Optional] int changevalue)
        {
            string CssVar = "";
            int ChangeValue = 25;

            if(changevalue != 0)
            {
                ChangeValue = changevalue;
            }

            MudColor Color = ColorManager.FromHex(Hex);

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
                    Color = ColorManager.ColorLighten(Color, ChangeValue);
                    CssVar = $"rgb({Color.R},{Color.G},{Color.B})";
                    break;
                case ColorOption.Darken:
                    Color = ColorManager.ColorDarken(Color, ChangeValue);
                    CssVar = $"rgb({Color.R},{Color.G},{Color.B})";
                    break;
            }
            return CssVar;
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
