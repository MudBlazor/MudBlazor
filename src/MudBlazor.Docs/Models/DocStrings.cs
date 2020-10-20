using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Models
{
    // this is needed for the api docs
    public static partial class DocStrings
    {
        public static string GetPropertyDescription(Type t, string property)
        {
            var name = $"{GetSaveTypename(t)}_{property}";
            var field = typeof(DocStrings).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .FirstOrDefault(f => f.Name == name);
            if (field == null)
                return null;
            return (string)field.GetValue(null);
        }

        public static string GetSaveTypename(Type t) => Regex.Replace(t.ConvertToCSharpSource(), @"[\.<>]", "_");
    }
}