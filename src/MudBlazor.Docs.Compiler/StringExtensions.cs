using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Compiler
{
    public static partial class StringExtensions
    {

        public static string ToLfLineEndings(this string self)
        {
            if (self == null)
                return null;
            return NewLineRegularExpression().Replace(self, "\n");
        }

        [GeneratedRegex(@"\r?\n")]
        private static partial Regex NewLineRegularExpression();
    }
}
