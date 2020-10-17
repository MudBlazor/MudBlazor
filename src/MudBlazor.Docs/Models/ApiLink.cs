using System;
using System.Collections.Generic;
using System.Text;
using MudBlazor.Docs.Extensions;

namespace MudBlazor.Docs.Models
{
    public static class ApiLink
    {
        public static string GetApiLinkFor(Type type)
        {
            if (!SpecialCaseComponents.TryGetValue(type, out var component))
                component = type.ToString().Replace("MudBlazor.Mud", "").ToKebabCase();
            string href = $"api/{component}";
            return href;
        }

        public static string GetComponentLinkFor(Type type)
        {
            if (!SpecialCaseComponents.TryGetValue(type, out var component))
                component = type.ToString().Replace("MudBlazor.Mud", "").ToKebabCase();
            string href = $"components/{component}";
            return href;
        }

        static Dictionary<Type, string> SpecialCaseComponents = new Dictionary<Type, string>()
        {
            [typeof(MudTable<T>)] = "table",
        };
    }
}
