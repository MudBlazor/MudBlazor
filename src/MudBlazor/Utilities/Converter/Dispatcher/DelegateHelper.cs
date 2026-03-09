// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MudBlazor.Utilities.Converter.Dispatcher;

internal static class DelegateHelper
{
    public static Delegate CreateForwardDelegate<TIn, TOut>(Type specificType, object converter)
        => CreateDelegate(
            converter,
            typeof(IConverter<,>).MakeGenericType(specificType, typeof(TOut)),
            nameof(IConverter<TIn, TOut>.Convert),
            typeof(Func<,>).MakeGenericType(specificType, typeof(TOut)),
            $"{nameof(IConverter<TIn, TOut>.Convert)}({specificType})");

    public static Delegate CreateBackwardDelegate<TIn, TOut>(Type specificType, object converter)
        => CreateDelegate(
            converter,
            typeof(IReversibleConverter<,>).MakeGenericType(specificType, typeof(TOut)),
            nameof(IReversibleConverter<TIn, TOut>.ConvertBack),
            typeof(Func<,>).MakeGenericType(typeof(TOut), specificType),
            $"{nameof(IReversibleConverter<TIn, TOut>.ConvertBack)}({typeof(TOut)})");

    private static Delegate CreateDelegate(
        object converter,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.Interfaces)] Type interfaceType,
        string methodName,
        Type delegateType,
        string missingMethodDisplay)
    {
        var converterType = converter.GetType();
        if (!interfaceType.IsAssignableFrom(converterType))
        {
            throw new InvalidOperationException($"Converter type {converterType.FullName} does not implement {missingMethodDisplay}");
        }

        var method = interfaceType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Cannot be null since we already verified the interface is implemented
        return method!.CreateDelegate(delegateType, converter);
    }
}
