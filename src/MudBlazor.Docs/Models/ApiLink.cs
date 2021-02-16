using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Models
{
    public static class ApiLink
    {
        public static string GetApiLinkFor(Type type)
        {
            if (!s_specialCaseComponents.TryGetValue(type, out var component))
                component = type.ToString().Replace("MudBlazor.Mud", "").ToLowerInvariant();
            var href = $"/api/{component}";
            return href;
        }

        public static string GetComponentLinkFor(Type type)
        {
            if (!s_specialCaseComponents.TryGetValue(type, out var component))
                component = type.ToString().Replace("MudBlazor.Mud", "").ToLowerInvariant();
            if (s_componentLinkTranslation.ContainsKey(component))
                component = s_componentLinkTranslation[component];
            var href = $"/components/{component}";
            return href;
        }

        /// <summary>
        /// Converts a lowercase component name from an URL into the C# Type name.
        /// Examples: 
        ///   table --> MudTable<T>
        ///   button  MudButton
        ///   appbar  AppBar
        /// </summary>
        public static Type GetTypeFromComponentLink(string component)
        {
            if (string.IsNullOrEmpty(component))
                return null;
            if (s_inverseSpecialCase.TryGetValue(component, out var type))
                return type;
            var assembly = typeof(MudComponentBase).Assembly;
            var lookup = new Dictionary<string, Type>();
            foreach (var x in assembly.GetTypes())
                lookup[x.Name.ToLowerInvariant()] = x;
            var type_lower = $"mud{component}".ToLowerInvariant();
            if (!lookup.TryGetValue(type_lower, out type))
                return null;
            return type;
        }

        private static Dictionary<Type, string> s_specialCaseComponents =
            new Dictionary<Type, string>()
            {
                [typeof(MudTable<T>)] = "table",
                [typeof(MudTextField<T>)] = "textfield",
                [typeof(MudNumericField<T>)] = "numericfield",
                [typeof(MudSelect<T>)] = "select",
                [typeof(MudInput<T>)] = "input",
                [typeof(MudAutocomplete<T>)] = "autocomplete",
                [typeof(MudSlider<T>)] = "slider",
                [typeof(MudCheckBox<T>)] = "checkbox",
                [typeof(MudSwitch<T>)] = "switch",
                [typeof(MudTreeView<T>)] = "treeview",
                [typeof(MudFab)] = "buttonfab",
                [typeof(MudIcon)] = "icons",
                [typeof(MudProgressCircular)] = "progress",
                [typeof(MudText)] = "typography",
            };

        // this is the inversion of above lookup
        private static Dictionary<string, Type> s_inverseSpecialCase =
            s_specialCaseComponents.ToDictionary(pair => pair.Value, pair => pair.Key);

        private static Dictionary<string, string> s_componentLinkTranslation =
            new Dictionary<string, string>()
            {
                ["icon"] = "icons",
                ["chip"] = "chips",
            };

    }
}
