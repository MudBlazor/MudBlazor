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
                component = type.ToString().Replace("MudBlazor.Mud", "").ToLowerInvariant();
            string href = $"/api/{component}";
            return href;
        }

        public static string GetComponentLinkFor(Type type)
        {
            if (!SpecialCaseComponents.TryGetValue(type, out var component))
                component = type.ToString().Replace("MudBlazor.Mud", "").ToLowerInvariant();
            if (ComponentLinkTranslation.ContainsKey(component))
                component = ComponentLinkTranslation[component];
            string href = $"/components/{component}";
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
            if (InverseSpecialCase.TryGetValue(component, out var type))
                return type;
            var assembly = typeof(MudComponentBase).Assembly;
            var lookup = new Dictionary<string, Type>(); 
            foreach(var x in assembly.GetTypes())
                lookup[x.Name.ToLowerInvariant()]=x;            
            var type_lower = $"mud{component}".ToLowerInvariant();
            if (!lookup.TryGetValue(type_lower, out type))
                return null;
            return type;
        }

        static Dictionary<Type, string> SpecialCaseComponents = new Dictionary<Type, string>()
        {
            [typeof(MudTable<T>)] = "table",
            [typeof(MudTextField<T>)] = "textfield",
            [typeof(MudSelect<T>)] = "select",
            [typeof(MudInput<T>)] = "input",
            [typeof(MudAutocomplete<T>)] = "autocomplete",
            [typeof(MudSlider<T>)]="slider",
            [typeof(MudCheckBox<T>)] = "checkbox",
            [typeof(MudSwitch<T>)] = "switch",
            [typeof(MudFab)] = "buttonfab",
            [typeof(MudIcon)] = "icons",
            [typeof(MudProgressCircular)] = "progress",
            [typeof(MudText)]= "typography",
        };

        // this is the inversion of above lookup
        private static Dictionary<string, Type> InverseSpecialCase =
            SpecialCaseComponents.ToDictionary(pair => pair.Value, pair => pair.Key);

        private static Dictionary<string, string> ComponentLinkTranslation = new Dictionary<string, string>()
        {
            ["icon"]="icons",
            ["chip"]="chips",
        };

    }
}
