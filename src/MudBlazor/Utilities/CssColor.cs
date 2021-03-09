// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Utilities
{
    public class CssColor
    {
        private Byte[] _valuesAsByte;

        public String Value { get; init; }

        public Byte R => _valuesAsByte[0];
        public Byte G => _valuesAsByte[1];
        public Byte B => _valuesAsByte[2];
        public Byte A => _valuesAsByte[3];

        public CssColor(String value)
        {
            value = value.Trim().ToLower();

            if (value.StartsWith("rgba") == true)
            {
                string[] parts = SplitInputIntoParts(value);
                if (parts.Length != 4)
                {
                    throw new ArgumentException("invalid color format");
                }

                _valuesAsByte = new byte[]
                {
                    Byte.Parse(parts[0]),
                    Byte.Parse(parts[1]),
                    Byte.Parse(parts[2]),
                    Byte.Parse(parts[3]),
                };

                Value = $"{R:x2}{G:x2}{B:x2}{A:x2}";
            }
            else if (value.StartsWith("rgb") == true)
            {
                string[] parts = SplitInputIntoParts(value);
                if (parts.Length != 3)
                {
                    throw new ArgumentException("invalid color format");
                }
                _valuesAsByte = new byte[]
                {
                    Byte.Parse(parts[0]),
                    Byte.Parse(parts[1]),
                    Byte.Parse(parts[2]),
                    255
                };

                Value = $"{R:x2}{G:x2}{B:x2}{A:x2}";
            }
            else
            {

                if (value.StartsWith("#"))
                {
                    value = value.Substring(1);
                }

                switch (value.Length)
                {
                    case 3:
                        value = new string(new Char[8] { value[0], value[0], value[1], value[1], value[2], value[2], 'F', 'F' });
                        break;
                    case 4:
                        value = new string(new Char[8] { value[0], value[0], value[1], value[1], value[2], value[2], value[3], value[3] });
                        break;
                    case 6:
                        value += "FF";
                        break;
                    case 8:
                        break;
                    default:
                        throw new ArgumentException("not a valid color", nameof(value));
                }

                Value = value;

                _valuesAsByte = new Byte[]
                {
                    GetByteFromValuePart(0),
                    GetByteFromValuePart(2),
                    GetByteFromValuePart(4),
                    GetByteFromValuePart(6),
                };
            }
        }

        private static String[] SplitInputIntoParts(String value)
        {
            Int32 startIndex = value.IndexOf('(');
            Int32 lastIndex = value.LastIndexOf(')');
            String subString = value[(startIndex + 1)..lastIndex];
            String[] parts = subString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return parts;
        }

        private Byte GetByteFromValuePart(Int32 index) => Byte.Parse(new String(new Char[] { Value[index], Value[index + 1] }), System.Globalization.NumberStyles.HexNumber);

        public static implicit operator CssColor(String input) => new CssColor(input);
        public static explicit operator String(CssColor color) => color == null ? String.Empty : color.Value;

        public override string ToString() => Value;
    }
}
