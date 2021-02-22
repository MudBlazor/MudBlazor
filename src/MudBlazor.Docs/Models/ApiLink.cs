﻿using System;
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
                component = new string(type.ToString().Replace("MudBlazor.Mud", "").TakeWhile(c => c != '`').ToArray()).ToLowerInvariant();
            var href = $"/api/{component}";
            return href;
        }

        public static string GetComponentLinkFor(Type type)
        {
            if (!s_specialCaseComponents.TryGetValue(type, out var component))
                component = new string(type.ToString().Replace("MudBlazor.Mud", "").TakeWhile(c => c != '`').ToArray()).ToLowerInvariant();
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
            foreach (var x in assembly.GetTypes())
            {
                if (new string(x.Name.ToLowerInvariant().TakeWhile(c => c != '`').ToArray()) == $"mud{component}".ToLowerInvariant())
                {
                    if (x.Name.Contains('`'))
                    {
                        return x.MakeGenericType(typeof(T));
                    }
                    else if (x.Name.ToLowerInvariant() == $"mud{component}".ToLowerInvariant())
                    {
                        return x;
                    }
                }
            }

            return null;
        }

        private static Dictionary<Type, string> s_specialCaseComponents =
            new Dictionary<Type, string>()
            {
                [typeof(MudFab)] = "buttonfab",
                [typeof(MudIcon)] = "icons",
                [typeof(MudProgressCircular)] = "progress",
                [typeof(MudText)] = "typography",
                [typeof(MudSnackbarProvider)] = "snackbar",
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
