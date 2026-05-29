using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;

namespace MudBlazor.Docs.Extensions
{
    public static partial class StringExtensions
    {
        public static string ToKebabCase(this string source)
        {
            if (source is null)
                return null;
            if (source.Length == 0)
                return string.Empty;
            var builder = new StringBuilder();
            for (var i = 0; i < source.Length; i++)
            {
                if (char.IsLower(source[i])) // if current char is already lowercase
                {
                    builder.Append(source[i]);
                }
                else if (i == 0) // if current char is the first char
                {
                    builder.Append(char.ToLowerInvariant(source[i]));
                }
                else if (char.IsLower(source[i - 1]) && char.IsLetter(source[i])) // if current char is upper and previous char is lower
                {
                    builder.Append('-');
                    builder.Append(char.ToLowerInvariant(source[i]));
                }
                else if (i + 1 == source.Length || char.IsUpper(source[i + 1])) // if current char is upper and next char doesn't exist or is upper
                {
                    builder.Append(char.ToLowerInvariant(source[i]));
                }
                else if (char.IsUpper(source[i]) && char.IsLetter(source[i + 1]) && char.IsLower(source[i + 1])) // if current char is upper and next char is lower
                {
                    builder.Append('-');
                    builder.Append(char.ToLowerInvariant(source[i]));
                }
                else
                {
                    builder.Append(char.ToLowerInvariant(source[i]));
                }
            }
            return builder.ToString();
        }

        public static string ToPascalCase(this string source)
        {
            if (source is null)
                return null;
            if (source.Length == 0)
                return string.Empty;
            var tokens = TokenRegularExpression().Split(source);
            return string.Join("", tokens.Select(x => x.Capitalize()));
        }

        public static string Capitalize(this string str)
        {
            if (str == null)
                return null;
            if (char.IsUpper(str[0]))
                return str;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }

        public static string ToCompressedEncodedUrl(this string code)
        {
            string urlEncodedBase64compressedCode;
            byte[] bytes;

            using (var uncompressed = new MemoryStream(Encoding.UTF8.GetBytes(code)))
            using (var compressed = new MemoryStream())
            using (var compressor = new DeflateStream(compressed, CompressionMode.Compress))
            {
                uncompressed.CopyTo(compressor);
                compressor.Close();
                bytes = compressed.ToArray();
                urlEncodedBase64compressedCode = WebEncoders.Base64UrlEncode(bytes);

                return urlEncodedBase64compressedCode;
            }
        }

        [GeneratedRegex(@"[-_ ]")]
        private static partial Regex TokenRegularExpression();
    }
}
