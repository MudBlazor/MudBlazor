using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    internal static class ParameterViewExtensions
    {
        public static bool Contains<T>(this ParameterView view, string parameterName)
        {
            return view.TryGetValue<T>(parameterName, out var _);
        }
    }
}
