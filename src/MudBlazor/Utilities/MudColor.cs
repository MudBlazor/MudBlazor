//// Copyright (c) Steven Coco
//// https://stackoverflow.com/questions/4087581/creating-a-c-sharp-color-from-hsl-values/4087601#4087601
//// Stripped and adapted by Meinrad Recheis and Benjamin Kappel for MudBlazor

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MudBlazor.Extensions;

namespace MudBlazor.Utilities
{
#nullable enable
    /// <summary>
    /// Specifies different output formats for <seealso cref="MudColor"/>.
    /// </summary>
    public enum MudColorOutputFormats
    {
        /// <summary>
        /// Output will be starting with a # and include r,g and b but no alpha values. Example #ab2a3d
        /// </summary>
        Hex,
        /// <summary>
        /// Output will be starting with a # and include r,g and b and alpha values. Example #ab2a3dff
        /// </summary>
        HexA,
        /// <summary>
        /// Will output css like output for value. Example rgb(12,15,40)
        /// </summary>
        RGB,
        /// <summary>
        /// Will output css like output for value with alpha. Example rgba(12,15,40,0.42)
        /// </summary>
        RGBA,
        /// <summary>
        /// Will output the color elements without any decorator and without alpha. Example 12,15,26
        /// </summary>
        ColorElements
    }

    /// <summary>
    /// Represents a color with methods to manipulate color values.
    /// </summary>
    [Serializable]
    public class MudColor : ISerializable, IEquatable<MudColor>
    {
        private const double Epsilon = 0.000000000000001;
        private readonly byte[] _valuesAsByte;

        /// <summary>
        /// Gets the hexadecimal representation of the color.
        /// </summary>
        [JsonIgnore]
        public string Value => $"#{R:x2}{G:x2}{B:x2}{A:x2}";

        /// <summary>
        /// Gets the 32-bit unsigned integer representation of the color.
        /// </summary>
        [JsonIgnore]
        public uint UInt32 => (uint)(R << 24 | G << 16 | B << 8 | A);

        /// <summary>
        /// Gets the red component value of the color.
        /// </summary>
        [JsonInclude]
        public byte R => _valuesAsByte[0];

        /// <summary>
        /// Gets the green component value of the color.
        /// </summary>
        [JsonInclude]
        public byte G => _valuesAsByte[1];

        /// <summary>
        /// Gets the blue component value of the color.
        /// </summary>
        [JsonInclude]
        public byte B => _valuesAsByte[2];

        /// <summary>
        /// Gets the alpha component value of the color.
        /// </summary>
        [JsonInclude]
        public byte A => _valuesAsByte[3];

        /// <summary>
        /// Gets the alpha component value as a percentage (0.0 to 1.0) of the color.
        /// </summary>
        [JsonIgnore]
        public double APercentage => Math.Round(A / 255.0, 2);

        /// <summary>
        /// Gets the hue component value of the color.
        /// </summary>
        [JsonIgnore]
        public double H { get; }

        /// <summary>
        /// Gets the lightness component value of the color.
        /// </summary>
        [JsonIgnore]
        public double L { get; }

        /// <summary>
        /// Gets the saturation component value of the color.
        /// </summary>
        [JsonIgnore]
        public double S { get; }

        /// <summary>
        /// Deserialization constructor for <see cref="MudColor"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>> containing the serialized data.</param>
        /// <param name="context">The <see cref="StreamingContext"/>>.</param>
        protected MudColor(SerializationInfo info, StreamingContext context) :
            this(info.GetByte(nameof(R)), info.GetByte(nameof(G)), info.GetByte(nameof(B)), info.GetByte(nameof(A)))
        {
        }

        /// <summary>
        /// Constructs a default instance of <see cref="MudColor"/> with default values (black with full opacity).
        /// </summary>
        public MudColor()
        {
            _valuesAsByte = new byte[4];
            _valuesAsByte[0] = 0;
            _valuesAsByte[1] = 0;
            _valuesAsByte[2] = 0;
            _valuesAsByte[3] = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified hue, saturation, lightness, and alpha values.
        /// </summary>
        /// <param name="h">The hue component value (0 to 360).</param>
        /// <param name="s">The saturation component value (0.0 to 1.0).</param>
        /// <param name="l">The lightness component value (0.0 to 1.0).</param>
        /// <param name="a">The alpha component value (0 to 1.0).</param>
        public MudColor(double h, double s, double l, double a)
            : this(h, s, l, (int)(a * 255.0).EnsureRange(255))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified hue, saturation, lightness, and alpha values.
        /// </summary>
        /// <param name="h">The hue component value (0 to 360).</param>
        /// <param name="s">The saturation component value (0.0 to 1.0).</param>
        /// <param name="l">The lightness component value (0.0 to 1.0).</param>
        /// <param name="a">The alpha component value (0 to 255).</param>
        public MudColor(double h, double s, double l, int a)
        {
            _valuesAsByte = new byte[4];

            h = Math.Round(h.EnsureRange(360), 0);
            s = Math.Round(s.EnsureRange(1), 2);
            l = Math.Round(l.EnsureRange(1), 2);
            a = a.EnsureRange(255);

            // achromatic argb (gray scale)
            if (Math.Abs(s) < Epsilon)
            {
                _valuesAsByte[0] = (byte)Math.Max(0, Math.Min(255, (int)Math.Ceiling(l * 255D)));
                _valuesAsByte[1] = (byte)Math.Max(0, Math.Min(255, (int)Math.Ceiling(l * 255D)));
                _valuesAsByte[2] = (byte)Math.Max(0, Math.Min(255, (int)Math.Ceiling(l * 255D)));
                _valuesAsByte[3] = (byte)a;
            }
            else
            {

                var q = l < .5D
                        ? l * (1D + s)
                        : (l + s) - (l * s);
                var p = (2D * l) - q;

                var hk = h / 360D;
                var t = new double[3];
                t[0] = hk + (1D / 3D); // Tr
                t[1] = hk; // Tb
                t[2] = hk - (1D / 3D); // Tg

                for (var i = 0; i < 3; i++)
                {
                    if (t[i] < 0D)
                        t[i] += 1D;
                    if (t[i] > 1D)
                        t[i] -= 1D;

                    if ((t[i] * 6D) < 1D)
                        t[i] = p + ((q - p) * 6D * t[i]);
                    else if ((t[i] * 2D) < 1)
                        t[i] = q;
                    else if ((t[i] * 3D) < 2)
                        t[i] = p + ((q - p) * ((2D / 3D) - t[i]) * 6D);
                    else
                        t[i] = p;
                }

                _valuesAsByte[0] = ((int)Math.Round(t[0] * 255D)).EnsureRangeToByte();
                _valuesAsByte[1] = ((int)Math.Round(t[1] * 255D)).EnsureRangeToByte();
                _valuesAsByte[2] = ((int)Math.Round(t[2] * 255D)).EnsureRangeToByte();
                _valuesAsByte[3] = (byte)a;
            }

            H = Math.Round(h, 0);
            S = Math.Round(s, 2);
            L = Math.Round(l, 2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified red, green, blue, and alpha values.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <param name="b">The blue component value (0 to 255).</param>
        /// <param name="a">The alpha component value (0 to 255).</param>
        [JsonConstructor]
        public MudColor(byte r, byte g, byte b, byte a)
        {
            _valuesAsByte = new byte[4];

            _valuesAsByte[0] = r;
            _valuesAsByte[1] = g;
            _valuesAsByte[2] = b;
            _valuesAsByte[3] = a;

            var (h, s, l) = CalculateHsl();
            H = h;
            S = s;
            L = l;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified color.
        /// </summary>
        /// <param name="rgba">the four bytes of this 32-bit unsigned integer contain the red, green, blue and alpha components</param>
        public MudColor(uint rgba)
            : this(r: (byte)(rgba >> 24), g: (byte)(rgba >> 16), b: (byte)(rgba >> 8), a: (byte)rgba)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified red, green, blue, and alpha values, copying the hue value from the provided color.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <param name="b">The blue component value (0 to 255).</param>
        /// <param name="color">The existing color to copy the hue value from.</param>
        public MudColor(byte r, byte g, byte b, MudColor color) : this(r, g, b, color.A)
        {
            H = color.H;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified RGB values and alpha component.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <param name="b">The blue component value (0 to 255).</param>
        /// <param name="alpha">The alpha component value (0.0 to 1.0).</param>
        public MudColor(int r, int g, int b, double alpha)
            : this(r, g, b, (byte)((alpha * 255.0).EnsureRange(255)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified RGB values and alpha component.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <param name="b">The blue component value (0 to 255).</param>
        /// <param name="alpha">The alpha component value (0 to 255).</param>
        public MudColor(int r, int g, int b, int alpha)
            : this((byte)r.EnsureRange(255), (byte)g.EnsureRange(255), (byte)b.EnsureRange(255), (byte)alpha.EnsureRange(255))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MudColor"/> class with the specified string representation of a color.
        /// </summary>
        /// <param name="value">The string representation of a color.</param>
        /// <remarks>
        /// The color can be represented in various formats, including hexadecimal (with or without alpha), RGB, and RGBA.
        /// Examples of valid color strings:
        /// - Hexadecimal format: "#ab2a3d", "#ab2a3dff"
        /// - RGB format: "rgb(12,15,40)"
        /// - RGBA format: "rgba(12,15,40,0.42)"
        /// </remarks>
        public MudColor(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);

            value = value.Trim().ToLowerInvariant();

            if (value.StartsWith("rgba"))
            {
                var parts = SplitInputIntoParts(value);
                if (parts.Length != 4)
                {
                    throw new ArgumentException("Invalid color format.");
                }

                _valuesAsByte = new[]
                {
                    byte.Parse(parts[0], CultureInfo.InvariantCulture),
                    byte.Parse(parts[1], CultureInfo.InvariantCulture),
                    byte.Parse(parts[2], CultureInfo.InvariantCulture),
                    (byte)Math.Max(0, Math.Min(255, 255 * double.Parse(parts[3], CultureInfo.InvariantCulture))),
                };
            }
            else if (value.StartsWith("rgb"))
            {
                var parts = SplitInputIntoParts(value);
                if (parts.Length != 3)
                {
                    throw new ArgumentException("Invalid color format.");
                }

                _valuesAsByte = new[]
                {
                    byte.Parse(parts[0], CultureInfo.InvariantCulture),
                    byte.Parse(parts[1], CultureInfo.InvariantCulture),
                    byte.Parse(parts[2], CultureInfo.InvariantCulture),
                    byte.MaxValue
                };
            }
            else
            {

                if (value.StartsWith('#'))
                {
                    value = value.Substring(1);
                }

                switch (value.Length)
                {
                    case 3:
                        value = new string(new[] { value[0], value[0], value[1], value[1], value[2], value[2], 'F', 'F' });
                        break;
                    case 4:
                        value = new string(new[] { value[0], value[0], value[1], value[1], value[2], value[2], value[3], value[3] });
                        break;
                    case 6:
                        value += "FF";
                        break;
                    case 8:
                        break;
                    default:
                        throw new ArgumentException(@"Not a valid color.", nameof(value));
                }

                _valuesAsByte = new[]
                {
                    GetByteFromValuePart(value,0),
                    GetByteFromValuePart(value,2),
                    GetByteFromValuePart(value,4),
                    GetByteFromValuePart(value,6),
                };
            }

            var (h, s, l) = CalculateHsl();
            H = h;
            S = s;
            L = l;
        }

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified hue value while keeping the saturation, lightness, and alpha values unchanged.
        /// </summary>
        /// <param name="h">The hue component value (0 to 360).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified hue value.</returns>
        public MudColor SetH(double h) => new(h, S, L, A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified saturation value while keeping the hue, lightness, and alpha values unchanged.
        /// </summary>
        /// <param name="s">The saturation component value (0.0 to 1.0).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified saturation value.</returns>
        public MudColor SetS(double s) => new(H, s, L, A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified lightness value while keeping the hue, saturation, and alpha values unchanged.
        /// </summary>
        /// <param name="l">The lightness component value (0.0 to 1.0).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified lightness value.</returns>
        public MudColor SetL(double l) => new(H, S, l, A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified red component value while keeping the green, blue, and alpha values unchanged.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified red component value.</returns>
        public MudColor SetR(int r) => new(r, G, B, A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified green component value while keeping the red, blue, and alpha values unchanged.
        /// </summary>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified green component value.</returns>
        public MudColor SetG(int g) => new(R, g, B, A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified blue component value while keeping the red, green, and alpha values unchanged.
        /// </summary>
        /// <param name="b">The blue component value (0 to 255).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified blue component value.</returns>
        public MudColor SetB(int b) => new(R, G, b, A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified alpha value while keeping the red, green, blue values unchanged.
        /// </summary>
        /// <param name="a">The alpha component value (0 to 255).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified alpha component value.</returns>
        public MudColor SetAlpha(int a) => new(R, G, B, a);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance with the specified alpha value while keeping the red, green, blue values unchanged.
        /// </summary>
        /// <param name="a">The alpha component value (0.0 to 1.0).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the specified alpha component value.</returns>
        public MudColor SetAlpha(double a) => new(R, G, B, a);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance by adjusting the lightness component value by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to adjust the lightness by (-1.0 to 1.0).</param>
        /// <returns>A new <see cref="MudColor"/> instance with the adjusted lightness.</returns>
        public MudColor ChangeLightness(double amount) => new(H, S, Math.Max(0, Math.Min(1, L + amount)), A);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance by lightening the color.
        /// </summary>
        /// <param name="amount">The amount to lighten the color by.</param>
        /// <returns>A new <see cref="MudColor"/> instance that is lighter than the original color.</returns>
        public MudColor ColorLighten(double amount) => ChangeLightness(+amount);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance by darkening the color.
        /// </summary>
        /// <param name="amount">The amount to darken the color by.</param>
        /// <returns>A new <see cref="MudColor"/> instance that is darker than the original color.</returns>
        public MudColor ColorDarken(double amount) => ChangeLightness(-amount);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance by lightening the color using the RGB algorithm.
        /// </summary>
        /// <returns>A new <see cref="MudColor"/> instance that is lighter than the original color.</returns>
        public MudColor ColorRgbLighten() => ColorLighten(0.075);

        /// <summary>
        /// Creates a new <see cref="MudColor"/> instance by darkening the color using the RGB algorithm.
        /// </summary>
        /// <returns>A new <see cref="MudColor"/> instance that is darker than the original color.</returns>
        public MudColor ColorRgbDarken() => ColorDarken(0.075);

        /// <summary>
        /// Checks whether the HSL (Hue, Saturation, lightness) values of this <see cref="MudColor"/> instance have changed compared to another <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="MudColor"/> instance to compare HSL values with.</param>
        /// <returns>True if the HSL values have changed; otherwise, false.</returns>
        public bool HslChanged(MudColor value)
        {
            var comparer = DoubleEpsilonEqualityComparer.Default;

            return !comparer.Equals(H, value.H) || !comparer.Equals(S, value.S) || !comparer.Equals(L, value.L);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is MudColor color && Equals(color);

        /// <summary>
        /// Determines whether this <see cref="MudColor"/> instance is equal to another <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="MudColor"/> instance to compare.</param>
        /// <returns>True if the instances are equal; otherwise, false.</returns>
        public bool Equals(MudColor? other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return
                _valuesAsByte[0] == other._valuesAsByte[0] &&
                _valuesAsByte[1] == other._valuesAsByte[1] &&
                _valuesAsByte[2] == other._valuesAsByte[2] &&
                _valuesAsByte[3] == other._valuesAsByte[3];
        }

        /// <summary>
        /// Determines whether two <see cref="MudColor"/> instances are equal.
        /// </summary>
        /// <param name="lhs">The first <see cref="MudColor"/> instance to compare.</param>
        /// <param name="rhs">The second <see cref="MudColor"/> instance to compare.</param>
        /// <returns>True if the instances are equal; otherwise, false.</returns>
        public static bool operator ==(MudColor? lhs, MudColor? rhs)
        {
            if (lhs is null && rhs is null)
            {
                return true;
            }

            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is null || rhs is null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <inheritdoc />
        public override int GetHashCode() => _valuesAsByte[0] + _valuesAsByte[1] + _valuesAsByte[2] + _valuesAsByte[3];

        /// <inheritdoc />
        public override string ToString() => ToString(MudColorOutputFormats.RGBA);

        /// <summary>
        /// Returns the string representation of the color in the specified format.
        /// </summary>
        /// <param name="format">The format to represent the color.</param>
        /// <returns>A string representing the color.</returns>
        public string ToString(MudColorOutputFormats format) => format switch
        {
            MudColorOutputFormats.Hex => Value.Substring(0, 7),
            MudColorOutputFormats.HexA => Value,
            MudColorOutputFormats.RGB => $"rgb({R},{G},{B})",
            MudColorOutputFormats.RGBA => $"rgba({R},{G},{B},{(A / 255.0).ToString(CultureInfo.InvariantCulture)})",
            MudColorOutputFormats.ColorElements => $"{R},{G},{B}",
            _ => Value,
        };

        /// <summary>
        /// Determines whether two <see cref="MudColor"/> instances are not equal.
        /// </summary>
        /// <param name="lhs">The first <see cref="MudColor"/> instance to compare.</param>
        /// <param name="rhs">The second <see cref="MudColor"/> instance to compare.</param>
        /// <returns>True if the instances are not equal; otherwise, false.</returns>
        public static bool operator !=(MudColor lhs, MudColor rhs) => !(lhs == rhs);

        /// <summary>
        /// Converts a string representation of a color to a <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="input">The string representation of the color.</param>
        /// <returns>A new <see cref="MudColor"/> instance representing the color.</returns>
        public static implicit operator MudColor(string input) => new(input);

        /// <summary>
        /// Converts a <see cref="MudColor"/> instance to its string representation.
        /// </summary>
        /// <param name="color">The MudColor instance to convert.</param>
        /// <returns>The string representation of the color.</returns>
        public static explicit operator string(MudColor? color) => color == null ? string.Empty : color.Value;

        /// <summary>
        /// Converts a <see cref="MudColor"/> instance to a 32-bit unsigned integer.
        /// </summary>
        /// <param name="mudColor">The MudColor instance to convert.</param>
        /// <returns>The 32-bit unsigned integer representation of the color.</returns>
        public static explicit operator uint(MudColor mudColor) => mudColor.UInt32;

        private byte GetByteFromValuePart(string input, int index) => byte.Parse(new string(new[] { input[index], input[index + 1] }), NumberStyles.HexNumber);

        private static string[] SplitInputIntoParts(string value)
        {
            var startIndex = value.IndexOf('(');
            var lastIndex = value.LastIndexOf(')');
            var subString = value[(startIndex + 1)..lastIndex];
            var parts = subString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            return parts;
        }

        private (double h, double s, double l) CalculateHsl()
        {
            var h = 0D;
            var s = 0D;

            // normalize red, green, blue values
            var r = R / 255D;
            var g = G / 255D;
            var b = B / 255D;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            // hue
            if (Math.Abs(max - min) < Epsilon)
            {
                h = 0D; // undefined
            }
            else if ((Math.Abs(max - r) < Epsilon) && (g >= b))
            {
                h = (60D * (g - b)) / (max - min);
            }
            else if ((Math.Abs(max - r) < Epsilon) && (g < b))
            {
                h = ((60D * (g - b)) / (max - min)) + 360D;
            }
            else if (Math.Abs(max - g) < Epsilon)
            {
                h = ((60D * (b - r)) / (max - min)) + 120D;
            }
            else if (Math.Abs(max - b) < Epsilon)
            {
                h = ((60D * (r - g)) / (max - min)) + 240D;
            }

            // lightness
            var l = (max + min) / 2D;

            // saturation
            if (Math.Abs(l) < Epsilon || (Math.Abs(max - min) < Epsilon))
            {
                s = 0D;
            }
            else if ((0D < l) && (l <= .5D))
            {
                s = (max - min) / (max + min);
            }
            else if (l > .5D)
            {
                s = (max - min) / (2D - (max + min)); //(max-min > 0)?
            }

            return (Math.Round(h.EnsureRange(360), 0), Math.Round(s.EnsureRange(1), 2), Math.Round(l.EnsureRange(1), 2));
        }

        /// <inheritdoc />
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(R), R);
            info.AddValue(nameof(G), G);
            info.AddValue(nameof(B), B);
            info.AddValue(nameof(A), A);
        }
    }
}
