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
        public static MudColor ColorLighten(MudColor rgbColor)
        {
            byte R = (byte)(rgbColor.R - 25);
            byte G = (byte)(rgbColor.G - 25);
            byte B = (byte)(rgbColor.B - 25);
            return MudColor.FromArgb(rgbColor.A, R, G, B);
        }
        public static MudColor ColorDarken(MudColor rgbColor)
        {
            byte R = (byte)(rgbColor.R - 25);
            byte G = (byte)(rgbColor.G - 25);
            byte B = (byte)(rgbColor.B - 25);
            return MudColor.FromArgb(rgbColor.A, R, G, B);
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
