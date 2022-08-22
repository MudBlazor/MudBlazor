using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Utilities
{
    [ExcludeFromCodeCoverage]
    [Obsolete("Use MudColor instead.", true)]
    public class ColorManager
    {
        public static System.Drawing.Color FromHex(string hex)
        {
            FromHex(hex, out var a, out var r, out var g, out var b);

            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        // amount is between 0.0 and 1.0
        public static System.Drawing.Color ColorLighten(System.Drawing.Color rgbColor, double amount)
        {
            var hsl = ColorTransformation.RgBtoHsl(rgbColor);
            hsl.L = Math.Max(0, Math.Min(1, hsl.L + amount));
            return ColorTransformation.HsLtoRgb(hsl, rgbColor.A);
        }

        // amount is between 0.0 and 1.0
        public static System.Drawing.Color ColorDarken(System.Drawing.Color rgbColor, double amount)
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
            var color = FromHex(hex);
            return $"rgba({color.R},{color.G},{color.B}, {alpha.ToString(CultureInfo.InvariantCulture)})";
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

            if (hex.Length is < 3 or > 4)
            {
                return null;
            }

            var red = char.ToString(hex[0]);
            var green = char.ToString(hex[1]);
            var blue = char.ToString(hex[2]);
            var alpha = hex.Length == 3 ? "F" : char.ToString(hex[3]);

            return string.Concat(red, red, green, green, blue, blue, alpha, alpha);
        }

        public static string ColorRgbDarken(string hex)
        {
            var color = FromHex(hex);
            color = ColorDarken(color, 0.075);
            return $"rgb({color.R},{color.G},{color.B})";
        }
        public static string ColorRgbLighten(string hex)
        {
            var color = FromHex(hex);
            color = ColorLighten(color, 0.075);
            return $"rgb({color.R},{color.G},{color.B})";
        }

        public static string ColorRgb(string hex)
        {
            var color = FromHex(hex);
            return $"rgb({color.R},{color.G},{color.B})";
        }

        public static string ColorRgbElements(string hex)
        {
            var color = FromHex(hex);
            return $"{color.R},{color.G},{color.B}";
        }

        public static string ColorRgba(string hex, double alpha)
        {
            var color = FromHex(hex);
            return $"rgba({color.R},{color.G},{color.B}, {alpha.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
