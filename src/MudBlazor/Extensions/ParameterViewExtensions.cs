using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;

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
            return parameters.HasParameterChanged(parameterName, parameterValue, out _, comparer);
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

        public static EffectiveParameterResult<TParameter1, TParameter2> ResolveEffectiveParameter<TParameter1, TParameter2>(this ParameterView parameterView, ParameterState<TParameter1> parameterState1, ParameterState<TParameter2> parameterState2, string dominantParameterName)
        {
            var parameterState1Internal = (ParameterStateInternal<TParameter1>)parameterState1;
            var parameterState2Internal = (ParameterStateInternal<TParameter2>)parameterState2;
            var parameterState1Comparer = parameterState1Internal.ExtractComparer(parameterView);
            var parameterState2Comparer = parameterState2Internal.ExtractComparer(parameterView);
            var hasParameter1Changed = parameterView.HasParameterChanged(parameterState1Internal.Metadata.ParameterName, parameterState1.RenderValue, out var parameter1Value, parameterState1Comparer);
            var hasParameter2Changed = parameterView.HasParameterChanged(parameterState2Internal.Metadata.ParameterName, parameterState2Internal.RenderValue, out var parameter2Value, parameterState2Comparer);

            if (!hasParameter1Changed && !hasParameter2Changed)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.None();
            }

            if (hasParameter1Changed && hasParameter2Changed)
            {
                if (dominantParameterName == parameterState1Internal.Metadata.ParameterName)
                    return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(
                        parameterState1Internal.Metadata.ParameterName,
                        parameter1Value);

                if (dominantParameterName == parameterState2Internal.Metadata.ParameterName)
                    return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(
                        parameterState2Internal.Metadata.ParameterName,
                        parameter2Value);

                throw new ArgumentException($"Unknown dominant parameter '{dominantParameterName}'.");
            }

            if (hasParameter1Changed)
            {
                return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter1(
                    parameterState1Internal.Metadata.ParameterName,
                    parameter1Value);
            }

            return EffectiveParameterResult<TParameter1, TParameter2>.FromParameter2(
                parameterState2Internal.Metadata.ParameterName,
                parameter2Value);
        }
    }
}
