using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    internal static class ParameterViewExtensions
    {
        public static bool Contains<T>(this ParameterView view, string parameterName)
        {
            return view.TryGetValue<T>(parameterName, out var _);
        }

        public static bool HasChanged<T>(this ParameterView view, T parameter, string parameterName)
        {
            view.TryGetValue<T>(parameterName, out var newValue);
            return !Equals(parameter, newValue);
        }

        public static T GetValue<T>(this ParameterView view, string parameterName)
        {
            var contains = view.TryGetValue<T>(parameterName, out var value);
            return contains ? value : default(T);
        }
    }
}
