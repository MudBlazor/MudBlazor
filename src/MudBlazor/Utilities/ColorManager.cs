using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using MudColor = System.Drawing.Color;


namespace MudBlazor.Utilities
{
    public class ColorManager
    {
        public static MudColor FromHex(string hex)
        {
            FromHex(hex, out var a, out var r, out var g, out var b);

            return MudColor.FromArgb(a, r, g, b);
        }

        // amount is between 0.0 and 1.0
        public static MudColor ColorLighten(MudColor rgbColor, double amount)
        {
            var hsl = ColorTransformation.RgBtoHsl(rgbColor);
            hsl.L = Math.Max(0, Math.Min(1, hsl.L + amount));
            return ColorTransformation.HsLtoRgb(hsl, rgbColor.A);
        }

        // amount is between 0.0 and 1.0
        public static MudColor ColorDarken(MudColor rgbColor, double amount)
        {
            var hsl = ColorTransformation.RgBtoHsl(rgbColor);
            hsl.L = Math.Max(0, Math.Min(1, hsl.L - amount));
            return ColorTransformation.HsLtoRgb(hsl, rgbColor.A);
        }

        public static void FromHex(string hex, out byte a, out byte r, out byte g, out byte b)
        {
            hex = ToRgbaHex(hex);
            if (hex == null || !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var packedValue))
            {
                throw new ArgumentException("Hexadecimal string is not in the correct format.", nameof(hex));
            }

            a = (byte)(packedValue >> 0);
            r = (byte)(packedValue >> 24);
            g = (byte)(packedValue >> 16);
            b = (byte)(packedValue >> 8);
        }

        public static string ToRgbaFromHex(string hex, double alpha)
        {
            MudColor Color = FromHex(hex);
            return $"rgba({Color.R},{Color.G},{Color.B}, {alpha.ToString(CultureInfo.InvariantCulture)})";
        }


        private static string ToRgbaHex(string hex)
        {
            hex = hex.StartsWith("#") ? hex.Substring(1) : hex;

            if (hex.Length == 8)
            {
                return hex;
            }

            if (hex.Length == 6)
            {
                return hex + "FF";
            }

            if (hex.Length < 3 || hex.Length > 4)
            {
                return null;
            }

            string red = char.ToString(hex[0]);
            string green = char.ToString(hex[1]);
            string blue = char.ToString(hex[2]);
            string alpha = hex.Length == 3 ? "F" : char.ToString(hex[3]);

            return string.Concat(red, red, green, green, blue, blue, alpha, alpha);
        }
    }
}
