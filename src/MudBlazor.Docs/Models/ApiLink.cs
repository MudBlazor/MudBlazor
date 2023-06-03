using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using System.Reflection;

namespace MudBlazor.Docs.Models
{
    public static class ApiLink
    {
        public static string GetClassLinkFor(Type type)
        {
            return $"class/{new string(type.Name.TakeWhile(c => c != '`').ToArray()).ToLowerInvariant()}"; //.ToString().Replace("MudBlazor.", "") 
        }
        public static string GetApiLinkFor(Type type)
        {
            return $"api/{GetComponentName(type)}";
        }

        public static string GetComponentLinkFor(Type type)
        {
            return $"components/{GetComponentName(type)}";
        }

        /// <summary>
        /// Converts a lowercase component name from an URL into the C# Type name.
        /// Examples: 
        ///   table --> <see cref="MudTable{T}"/>
        ///   button  <see cref="MudButton"/>
        ///   appbar  <see cref="MudAppBar"/>
        /// </summary>
        public static Type GetTypeFromComponentLink(string component)
        {
            if (component.Contains('#') == true)
            {
                component = component.Substring(0, component.IndexOf('#'));
            }

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
        private static string GetComponentName(Type type)
        {
            if (!s_specialCaseComponents.TryGetValue(type, out var component))
            {
                component = new string(type.ToString().Replace("MudBlazor.Mud", "").Replace("MudBlazor.", "").TakeWhile(c => c != '`').ToArray())
                    .ToLowerInvariant();
            }

            return component;
        }

        private static Dictionary<Type, string> s_specialCaseComponents =
            new()
            {
                [typeof(MudFab)] = "buttonfab",
                [typeof(MudIcon)] = "icons",
                [typeof(MudProgressCircular)] = "progress",
                [typeof(MudText)] = "typography",
                [typeof(MudSnackbarProvider)] = "snackbar",
                [typeof(Bar)] = "barchart",
                [typeof(Donut)] = "donutchart",
                [typeof(Line)] = "linechart",
                [typeof(Pie)] = "piechart",
                [typeof(MudChip)] = "chips",
                [typeof(ChartOptions)] = "options",
                [typeof(TemplateColumn<T>)] = "datagridtemplatecolumn",
                [typeof(PropertyColumn<T,TProperty>)] = "datagridpropertycolumn",
                [typeof(SelectColumn<T>)] = "datagridselectcolumn",
            };

        // this is the inversion of above lookup
        private static Dictionary<string, Type> s_inverseSpecialCase =
            s_specialCaseComponents.ToDictionary(pair => pair.Value, pair => pair.Key);

        /// <summary>
        /// Converts a lowercase Class name from an URL into the C# Type name.
        /// Examples: 
        ///   table --> <see cref="MudTable{T}"/>
        ///   button  <see cref="MudButton"/>
        ///   appbar  <see cref="MudAppBar"/>
        /// </summary>
        public static Type GetTypeFromClassLink(string className)
        {
            if (className.Contains('#') == true)
            {
                className = className.Substring(0, className.IndexOf('#'));
            }

            if (string.IsNullOrEmpty(className))
                return null;
            if (s_inverseSpecialCase.TryGetValue(className, out var type))
                return type;


            var assembly = typeof(MudComponentBase).Assembly;
            foreach (var x in assembly.GetTypes())
            {
                if (new string(x.Name.ToLowerInvariant().TakeWhile(c => c != '`').ToArray()) == $"{className}".ToLowerInvariant())
                {
                    if (x.Name.Contains('`'))
                    {
                        return x.GetGenericTypeDefinition();//.MakeGenericType(x.GetGenericArguments());// GenericTypeParameters);
                    }
                    else if (x.Name.ToLowerInvariant() == $"{className}".ToLowerInvariant())
                    {
                        return x;
                    }
                }
            }

            return null;
        }
    }
}
