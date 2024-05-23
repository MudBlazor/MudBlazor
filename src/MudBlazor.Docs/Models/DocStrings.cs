using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Models
{
    // this is needed for the api docs 
    public static partial class DocStrings
    {
        /* To speed up the method, run it in this way:
         *   string saveTypename = DocStrings.GetSaveTypename(type);  // calculate it only once
         *   DocStrings.GetMemberDescription(saveTypename, member);
         */
        public static string GetMemberDescription(string saveTypename, MemberInfo member)
        {
            string name;

            if (member is PropertyInfo property)
                name = saveTypename + "_" + property.Name;
            else if (member is MethodInfo method)
                name = saveTypename + "_method_" + GetSaveMethodIdentifier(method);
            else
                throw new Exception("Implemented only for properties and methods.");

            var field = typeof(DocStrings).GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
            if (field == null)
                return null;
            return (string)field.GetValue(null);
        }

        public static string GetSaveTypename(Type t) => PeriodRegularExpression().Replace(t.ConvertToCSharpSource(), "_").Replace("<T>", "").TrimEnd('_');

        private static string GetSaveMethodIdentifier(MethodInfo method) => LetterDigitOrUnderscoreRegularExpression().Replace(method.ToString().Replace("MudBlazor.Docs.Models.T", "T"), "_");  // method signature

        [GeneratedRegex(@"[\.]")]
        private static partial Regex PeriodRegularExpression();

        [GeneratedRegex("[^A-Za-z0-9_]")]
        private static partial Regex LetterDigitOrUnderscoreRegularExpression();
    }
}
