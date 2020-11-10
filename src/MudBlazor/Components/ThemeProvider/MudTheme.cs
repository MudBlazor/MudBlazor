using System;
using System.Text;
using MudBlazor.Utilities;
using System.Globalization;
using MudColor = System.Drawing.Color;
using System.Runtime.InteropServices;

namespace MudBlazor
{
    public class MudThemeOld : DefaultMudTheme
    {
        protected virtual void GenerateTheme(StringBuilder theme)
        {
            //Core Layout Surfaces
            theme.AppendLine($"--mud-theme-background: {Color_Background};");
            theme.AppendLine($"--mud-theme-background-grey: {Color_Background_Grey};");

            theme.AppendLine($"--mud-theme-surface: {Color_Surface};");
            theme.AppendLine($"--mud-theme-on-surface: {Color_On_Surface};");

            theme.AppendLine($"--mud-theme-appbar: {Color_AppBar};");
            theme.AppendLine($"--mud-theme-on-appbar: {Color_On_AppBar};");

            theme.AppendLine($"--mud-theme-drawer: {Color_Drawer};");
            theme.AppendLine($"--mud-theme-on-drawer: {Color_On_Drawer};");


            //Text Only
            theme.AppendLine($"--mud-theme-text-default: {Color_Text_Default};");


            //Theme Main Colors
            theme.AppendLine($"--mud-theme-default: {BuildColor(Color_Default, ColorOption.RgbA, 0.54)};");
            theme.AppendLine($"--mud-theme-on-default: {BuildColor(Color_On_Default, ColorOption.RgbA, 0.87)};");
            theme.AppendLine($"--mud-theme-default-lighten: {BuildColor(Color_Default, ColorOption.RgbA, 0.1)};");
            theme.AppendLine($"--mud-theme-default-darken: {BuildColor(Color_Default, ColorOption.RgbA, 0.2)};");
            theme.AppendLine($"--mud-theme-default-hover: {BuildColor(Color_Default, ColorOption.RgbA, 0.04)};");

            theme.AppendLine($"--mud-theme-primary: {BuildColor(Color_Primary,ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-on-primary: {Color_On_Primary};");
            theme.AppendLine($"--mud-theme-primary-lighten: {BuildColor(Color_Primary,ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-primary-darken: {BuildColor(Color_Primary, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-primary-hover: {BuildColor(Color_Primary, ColorOption.RgbA, 0.06)};");

            theme.AppendLine($"--mud-theme-secondary: {BuildColor(Color_Secondary, ColorOption.Rgb)};");
            theme.AppendLine($"--mud-theme-on-secondary: {Color_On_Secondary};");
            theme.AppendLine($"--mud-theme-secondary-lighten: {BuildColor(Color_Secondary, ColorOption.Lighten)};");
            theme.AppendLine($"--mud-theme-secondary-darken: {BuildColor(Color_Secondary, ColorOption.Darken)};");
            theme.AppendLine($"--mud-theme-secondary-hover: {BuildColor(Color_Secondary, ColorOption.RgbA, 0.06)};");


            //Theme Alert & Notification Colors
            theme.AppendLine($"--mud-theme-info: {Color_Info};");
            theme.AppendLine($"--mud-theme-on-info: {Color_On_Alert};");
            theme.AppendLine($"--mud-theme-success: {Color_Success};");
            theme.AppendLine($"--mud-theme-on-success: {Color_On_Alert};");
            theme.AppendLine($"--mud-theme-warning: {Color_Warning};");
            theme.AppendLine($"--mud-theme-on-warning: {Color_On_Alert};");
            theme.AppendLine($"--mud-theme-danger: {Color_Danger};");
            theme.AppendLine($"--mud-theme-on-danger: {Color_On_Alert};");
            theme.AppendLine($"--mud-theme-dark: {Color_Dark};");
            theme.AppendLine($"--mud-theme-on-dark: {Color_On_Alert};");
            theme.AppendLine($"--mud-theme-on-alert: {Color_On_Alert};");


            //Theme Border Colors & Radius, Todo remove or make less of them?
            theme.AppendLine($"--mud-theme-border-color-default: {Color_Border_Default};");
            theme.AppendLine($"--mud-theme-border-color-outlines: {Color_Border_Outlines};");
            theme.AppendLine($"--mud-theme-border-color-inputs: {Color_Border_Inputs};");
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
