using System;
using System.Collections.Generic;
using System.Linq;
using MudBlazor.Charts;

namespace MudBlazor.Docs.Models
{
#nullable enable
    public static class ApiLink
    {
        public static string GetApiLinkFor(Type type)
        {
            return $"api/{type.Name}";
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
        public static Type? GetTypeFromComponentLink(string component)
        {
            if (string.IsNullOrEmpty(component))
            {
                return null;
            }
            if (component.Contains('#'))
            {
                component = component[..component.IndexOf('#')];
            }
            if (InverseSpecialCase.TryGetValue(component, out var type))
            {
                return type;
            }

            var assembly = typeof(MudComponentBase).Assembly;
            foreach (var componentType in assembly.GetTypes())
            {
                var typeNameWithoutGenericInfo = new string(componentType.Name.ToLowerInvariant().TakeWhile(c => c != '`').ToArray());
                if (typeNameWithoutGenericInfo.Equals($"mud{component}", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (componentType.Name.Contains('`'))
                    {
                        return componentType.MakeGenericType(typeof(T));
                    }

                    if (string.Equals(componentType.Name, $"mud{component}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return componentType;
                    }
                }
            }

            return null;
        }

        private static string GetComponentName(Type type)
        {
            if (!SpecialCaseComponents.TryGetValue(type, out var component))
            {
                component = new string(type
                        .ToString()
                        .Replace("MudBlazor.Mud", "")
                        .TakeWhile(c => c != '`')
                        .ToArray())
                    .ToLowerInvariant();
            }

            return component;
        }

        private static readonly Dictionary<Type, string> SpecialCaseComponents =
            new()
            {
                [typeof(MudFab)] = "buttonfab",
                [typeof(MudIcon)] = "icons",
                [typeof(MudProgressCircular)] = "progress",
                [typeof(MudText)] = "typography",
                [typeof(MudSnackbarProvider)] = "snackbar",
                [typeof(Bar)] = "barchart",
                [typeof(StackedBar)] = "stackedbarchart",
                [typeof(Donut)] = "donutchart",
                [typeof(Line)] = "linechart",
                [typeof(TimeSeries)] = "timeserieschart",
                [typeof(Pie)] = "piechart",
                [typeof(MudChip<T>)] = "chips",
                [typeof(ChartOptions)] = "options"
            };

        // this is the inversion of above lookup
        private static readonly Dictionary<string, Type> InverseSpecialCase =
            SpecialCaseComponents.ToDictionary(pair => pair.Value, pair => pair.Key);
    }
}
