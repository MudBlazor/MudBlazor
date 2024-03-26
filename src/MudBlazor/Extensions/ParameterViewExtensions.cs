using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    internal static class ParameterViewExtensions
    {
        public static bool Contains<T>(this ParameterView view, string parameterName)
        {
            return view.TryGetValue<T>(parameterName, out _);
        }

        /// <summary>
        /// Checks if a parameter changed.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
        /// <returns><c>true</c> if the parameter value has changed, <c>false</c> otherwise.</returns>
        public static bool HasParameterChanged<T>(this ParameterView parameters, string parameterName, T parameterValue, IEqualityComparer<T>? comparer = null)
        {
            return HasParameterChanged(parameters, parameterName, parameterValue, out _, comparer);
        }

        /// <summary>
        /// Checks if a parameter changed.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="value">Receives the value, if any.</param>
        /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
        /// <returns><c>true</c> if the parameter value has changed, <c>false</c> otherwise.</returns>
        public static bool HasParameterChanged<T>(this ParameterView parameters, string parameterName, T parameterValue, [MaybeNullWhen(false)] out T value, IEqualityComparer<T>? comparer = null)
        {
            if (parameters.TryGetValue(parameterName, out value))
            {
                return !comparer?.Equals(value, parameterValue) ?? !EqualityComparer<T>.Default.Equals(value, parameterValue);
            }

            return false;
        }
    }
}
