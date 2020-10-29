using System;
using System.Collections.Generic;
using System.Linq;
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
            string href = $"/api/{component}";
            return href;
        }

        public static string GetComponentLinkFor(Type type)
        {
            if (!SpecialCaseComponents.TryGetValue(type, out var component))
                component = type.ToString().Replace("MudBlazor.Mud", "").ToLowerInvariant(); //.ToKebabCase();
            if (ComponentLinkTranslation.ContainsKey(component))
                component = ComponentLinkTranslation[component];
            string href = $"/components/{component}";
            return href;
        }

        /// <summary>
        /// Converts "table" into typeof(MudTable<T>) or "button" into typeof(MudButton)
        /// </summary>
        public static Type GetTypeFromComponentLink(string component)
        {
            if (InverseSpecialCase.TryGetValue(component, out var type))
                return type;
            var type_name = $"MudBlazor.Mud{component.ToPascalCase()}";
            var assembly = typeof(MudComponentBase).Assembly;
            type = assembly.GetType(type_name);
            return type;
        }

        static Dictionary<Type, string> SpecialCaseComponents = new Dictionary<Type, string>()
        {
            [typeof(MudTable<T>)] = "table",
        };

        private static Dictionary<string, string> ComponentLinkTranslation = new Dictionary<string, string>()
        {
            ["icon"]="icons",
            ["chip"]="chips",
        };

        private static Dictionary<string, Type> InverseSpecialCase =
            SpecialCaseComponents.ToDictionary(pair => pair.Value, pair => pair.Key);
    }
}
