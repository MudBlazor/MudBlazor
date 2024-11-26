//// Copyright (c) Steven Coco
//// https://stackoverflow.com/questions/4087581/creating-a-c-sharp-color-from-hsl-values/4087601#4087601
//// Stripped and adapted by Meinrad Recheis and Benjamin Kappel for MudBlazor

using System.Diagnostics.CodeAnalysis;
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
    public partial class MudColor : ISerializable, IEquatable<MudColor>, IParsable<MudColor>, IFormattable
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
        public byte R => _valuesAsByte[0];

        /// <summary>
        /// Gets the green component value of the color.
        /// </summary>
        public byte G => _valuesAsByte[1];

        /// <summary>
        /// Gets the blue component value of the color.
        /// </summary>
        public byte B => _valuesAsByte[2];

        /// <summary>
        /// Gets the alpha component value of the color.
        /// </summary>
        public byte A => _valuesAsByte[3];

        /// <summary>
        /// Gets the alpha component value as a percentage (0.0 to 1.0) of the color.
        /// </summary>
        [JsonIgnore]
        public double APercentage => NormalizeAlpha(A);

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

            var (r, g, b) = HslToRgb(h, s, l);
            _valuesAsByte[0] = r;
            _valuesAsByte[1] = g;
            _valuesAsByte[2] = b;
            _valuesAsByte[3] = (byte)a;

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

            var (h, s, l) = RgbToHsl(r, g, b);
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
            : this(r, g, b, (byte)(alpha * 255.0).EnsureRange(255))
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

            var (r, g, b, a) = ParseStringColorCore(value);
            var (h, s, l) = RgbToHsl(r, g, b);
            _valuesAsByte = [r, g, b, a];
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

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);

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

        /// <inheritdoc />
        /// <remarks>
        /// The following formats are available:
        /// <list type="bullet">
        /// <item>
        /// <term>rgb</term>
        /// <description>Outputs the color in the format "rgb(r,g,b)".</description>
        /// </item>
        /// <item>
        /// <term>rgba</term>
        /// <description>Outputs the color in the format "rgba(r,g,b,a)".</description>
        /// </item>
        /// <item>
        /// <term>hex</term>
        /// <description>Outputs the color in the hexadecimal format "#rrggbb".</description>
        /// </item>
        /// <item>
        /// <term>hexa</term>
        /// <description>Outputs the color in the hexadecimal format with alpha "#rrggbbaa".</description>
        /// </item>
        /// <item>
        /// <term>colorelements</term>
        /// <description>Outputs the color elements without any decorator "r,g,b".</description>
        /// </item>
        /// </list>
        /// </remarks>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                // Default to parameterless ToString behaviour, otherwise it will break and return incorrect format _ => Value
                // if the framework has choice between ToString(string) and ToString(string, IFormatProvider) it will choose the latter.
                return ToString();
            }

            return format.ToLowerInvariant() switch
            {
                "rgb" => ToString(MudColorOutputFormats.RGB),
                "rgba" => ToString(MudColorOutputFormats.RGBA),
                "hex" => ToString(MudColorOutputFormats.Hex),
                "hexa" => ToString(MudColorOutputFormats.HexA),
                "colorelements" => ToString(MudColorOutputFormats.ColorElements),
                _ => Value
            };
        }

        /// <summary>
        /// Deconstructs the <see cref="MudColor"/> into its red, green, and blue components.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <param name="b">The blue component value (0 to 255).</param>
        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstructs the <see cref="MudColor"/> into its red, green, blue, and alpha components.
        /// </summary>
        /// <param name="r">The red component value (0 to 255).</param>
        /// <param name="g">The green component value (0 to 255).</param>
        /// <param name="b">The blue component value (0 to 255).</param>
        /// <param name="a">The alpha component value (0 to 255).</param>
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Determines whether two <see cref="MudColor"/> instances are not equal.
        /// </summary>
        /// <param name="lhs">The first <see cref="MudColor"/> instance to compare.</param>
        /// <param name="rhs">The second <see cref="MudColor"/> instance to compare.</param>
        /// <returns>True if the instances are not equal; otherwise, false.</returns>
        public static bool operator !=(MudColor? lhs, MudColor? rhs) => !(lhs == rhs);

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

        /// <summary>
        /// Converts a string representation of a color to a <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="input">The string representation of the color.</param>
        /// <returns>A new <see cref="MudColor"/> instance representing the color.</returns>
        public static implicit operator MudColor(string input) => Parse(input);

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

        /// <summary>
        /// Parses a string representation of a color to a <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="s">The string representation of the color.</param>
        /// <param name="provider">An optional format provider.</param>
        /// <remarks>
        /// The color can be represented in various formats, including hexadecimal (with or without alpha), RGB, and RGBA.
        /// Examples of valid color strings:
        /// - Hexadecimal format: "#ab2a3d", "#ab2a3dff"
        /// - RGB format: "rgb(12,15,40)"
        /// - RGBA format: "rgba(12,15,40,0.42)"
        /// </remarks>
        /// <returns>A new <see cref="MudColor"/> instance representing the color.</returns>
        /// <exception cref="ArgumentException">Thrown when the input string is null, empty or invalid color format.</exception>
        public static MudColor Parse(string s, IFormatProvider? provider = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(s);
            var (r, g, b, a) = ParseStringColorCore(s);

            return new MudColor(r, g, b, a);
        }

        /// <summary>
        /// Tries to parse a string representation of a color to a <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="s">The string representation of the color.</param>
        /// <param name="provider">An optional format provider.</param>
        /// <param name="result">When this method returns, contains the <see cref="MudColor"/> instance equivalent to the color contained in <paramref name="s"/>, if the conversion succeeded, or <c>null</c> if the conversion failed.</param>
        /// <remarks>
        /// The color can be represented in various formats, including hexadecimal (with or without alpha), RGB, and RGBA.
        /// Examples of valid color strings:
        /// - Hexadecimal format: "#ab2a3d", "#ab2a3dff"
        /// - RGB format: "rgb(12,15,40)"
        /// - RGBA format: "rgba(12,15,40,0.42)"
        /// </remarks>
        /// <returns><c>true</c> if the string was successfully parsed; otherwise, <c>false</c>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MudColor result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = null;
                return false;
            }

            try
            {
                var mudColor = Parse(s, provider);
                result = mudColor;
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to parse a string representation of a color to a <see cref="MudColor"/> instance.
        /// </summary>
        /// <param name="s">The string representation of the color.</param>
        /// <param name="result">When this method returns, contains the <see cref="MudColor"/> instance equivalent to the color contained in <paramref name="s"/>, if the conversion succeeded, or <c>null</c> if the conversion failed.</param>
        /// <remarks>
        /// The color can be represented in various formats, including hexadecimal (with or without alpha), RGB, and RGBA.
        /// Examples of valid color strings:
        /// - Hexadecimal format: "#ab2a3d", "#ab2a3dff"
        /// - RGB format: "rgb(12,15,40)"
        /// - RGBA format: "rgba(12,15,40,0.42)"
        /// </remarks>
        /// <returns><c>true</c> if the string was successfully parsed; otherwise, <c>false</c>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out MudColor result) => TryParse(s, null, out result);

        private static double NormalizeAlpha(byte a, int digit = 2) => Math.Round(a / 255.0, digit);

        private static double NormalizeAlpha(int a, int digit = 2) => Math.Round(a / 255.0, digit);

        private static byte GetByteFromValuePart(string input, int index) => byte.Parse(new string(new[] { input[index], input[index + 1] }), NumberStyles.HexNumber);

        private static (byte r, byte g, byte b) HslToRgb(double h, double s, double l)
        {
            // Achromatic (gray scale)
            if (Math.Abs(s) < Epsilon)
            {
                var gray = (byte)Math.Max(0, Math.Min(255, (int)Math.Ceiling(l * 255d)));
                return (gray, gray, gray);
            }

            var hNormalized = h / 360d;
            var t2 = l <= 0.5d
                ? l * (1.0d + s)
                : l + s - l * s;
            var t1 = 2.0d * l - t2;

            var tr = HueToRgb(t1, t2, hNormalized + 1.0d / 3.0d);
            var tg = HueToRgb(t1, t2, hNormalized);
            var tb = HueToRgb(t1, t2, hNormalized - 1.0d / 3.0d);

            var r = ((int)Math.Round(tr * 255d)).EnsureRangeToByte();
            var g = ((int)Math.Round(tg * 255d)).EnsureRangeToByte();
            var b = ((int)Math.Round(tb * 255d)).EnsureRangeToByte();

            return (r, g, b);
        }

        private static double HueToRgb(double t1, double t2, double hueNormalized)
        {
            if (hueNormalized < 0.0d) hueNormalized += 1.0d;
            if (hueNormalized > 1.0d) hueNormalized -= 1.0d;
            return hueNormalized switch
            {
                < 1.0d / 6.0d => t1 + (t2 - t1) * 6.0d * hueNormalized,
                < 1.0d / 2.0d => t2,
                < 2.0d / 3.0d => t1 + (t2 - t1) * (2.0d / 3.0d - hueNormalized) * 6.0d,
                _ => t1
            };
        }

        private static (byte r, byte g, byte b, byte a) ParseStringColorCore(string value)
        {
            value = value.Trim().ToLowerInvariant();

            return value switch
            {
                _ when value.StartsWith("rgba") => ParseStringRgbaToRgba(value),
                _ when value.StartsWith("rgb") => ParseStringRgbToRgba(value),
                _ => ParseStringHexToRgba(value),
            };
        }

        private static (byte r, byte g, byte b, byte a) ParseStringRgbaToRgba(string value)
        {
            var parts = SplitInputIntoParts(value);
            if (parts.Length != 4)
            {
                throw new ArgumentException("Invalid color format.");
            }

            var r = byte.Parse(parts[0], CultureInfo.InvariantCulture);
            var g = byte.Parse(parts[1], CultureInfo.InvariantCulture);
            var b = byte.Parse(parts[2], CultureInfo.InvariantCulture);
            var a = (byte)Math.Max(0, Math.Min(255, 255 * double.Parse(parts[3], CultureInfo.InvariantCulture)));

            return (r, g, b, a);
        }

        private static (byte r, byte g, byte b, byte a) ParseStringRgbToRgba(string value)
        {
            var parts = SplitInputIntoParts(value);
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid color format.");
            }

            var r = byte.Parse(parts[0], CultureInfo.InvariantCulture);
            var g = byte.Parse(parts[1], CultureInfo.InvariantCulture);
            var b = byte.Parse(parts[2], CultureInfo.InvariantCulture);
            return (r, g, b, byte.MaxValue);
        }

        private static (byte r, byte g, byte b, byte a) ParseStringHexToRgba(string value)
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
            return (GetByteFromValuePart(value, 0), GetByteFromValuePart(value, 2), GetByteFromValuePart(value, 4), GetByteFromValuePart(value, 6));
        }

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

        private static (double h, double s, double l) RgbToHsl(byte r, byte g, byte b)
        {
            var h = 0D;
            var s = 0D;

            // normalize red, green, blue values
            var rNormalized = r / 255D;
            var gNormalized = g / 255D;
            var bNormalized = b / 255D;

            var max = Math.Max(rNormalized, Math.Max(gNormalized, bNormalized));
            var min = Math.Min(rNormalized, Math.Min(gNormalized, bNormalized));

            // hue
            if (Math.Abs(max - min) < Epsilon)
            {
                h = 0D; // undefined
            }
            else if ((Math.Abs(max - rNormalized) < Epsilon) && (gNormalized >= bNormalized))
            {
                h = (60D * (gNormalized - bNormalized)) / (max - min);
            }
            else if ((Math.Abs(max - rNormalized) < Epsilon) && (gNormalized < bNormalized))
            {
                h = ((60D * (gNormalized - bNormalized)) / (max - min)) + 360D;
            }
            else if (Math.Abs(max - gNormalized) < Epsilon)
            {
                h = ((60D * (bNormalized - rNormalized)) / (max - min)) + 120D;
            }
            else if (Math.Abs(max - bNormalized) < Epsilon)
            {
                h = ((60D * (rNormalized - gNormalized)) / (max - min)) + 240D;
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
