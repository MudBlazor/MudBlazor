using System;
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

        public static bool IsSubclassOfGeneric(this Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
