using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Models
{
    // this is needed for the api docs 
    public static partial class DocStrings
    {
        public static string GetMemberDescription(Type t, MemberInfo member)
        {
            var name = GetSaveTypename(t);

            if (member is PropertyInfo property)
                name += "_" + property.Name;
            else if (member is MethodInfo method)
                name += "_method_" + GetSaveMethodIdentifier(method);
            else
                throw new Exception("Implemented only for properties and methods.");

            var field = typeof(DocStrings).GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
            if (field == null)
                return null;
            return (string)field.GetValue(null);
        }

        private static string GetSaveTypename(Type t) => Regex.Replace(t.ConvertToCSharpSource(), @"[\.]", "_").Replace("<T>", "").TrimEnd('_');

        private static string GetSaveMethodIdentifier(MethodInfo method) => Regex.Replace(method.ToString().Replace("MudBlazor.Docs.Models.T", "T"), "[^A-Za-z0-9_]", "_");  // method signature
    }
}
