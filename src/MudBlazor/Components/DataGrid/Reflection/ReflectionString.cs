// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MudBlazor
{
    internal class ReflectionString
    {
        internal const DynamicallyAccessedMemberTypes MethodTrimmingAccess = DynamicallyAccessedMemberTypes.PublicMethods;
        internal const string TrimMethodName = "Trim";
        internal const string ContainsMethodName = "Contains";
        internal const string EqualsMethodName = "Equals";
        internal const string StartsWithMethodName = "StartsWith";
        internal const string EndsWithMethodName = "EndsWith";

        internal static MethodInfo GetTrimMethodInfo([DynamicallyAccessedMembers(MethodTrimmingAccess)] Type type)
        {
            var trimMethodInfo = type.GetMethod(TrimMethodName, Type.EmptyTypes);
            if (trimMethodInfo is null)
            {
                throw new InvalidOperationException($"Method {TrimMethodName} not found for type {type}.");
            }

            return trimMethodInfo;
        }

        internal static MethodInfo GetContainsMethodInfo([DynamicallyAccessedMembers(MethodTrimmingAccess)] Type type, Type[] types)
        {
            var containsMethodInfo = type.GetMethod(ContainsMethodName, types);
            if (containsMethodInfo is null)
            {
                throw new InvalidOperationException($"Method {ContainsMethodName} not found for type {type}.");
            }

            return containsMethodInfo;
        }

        internal static MethodInfo GetEqualsMethodInfo([DynamicallyAccessedMembers(MethodTrimmingAccess)] Type type, Type[] types)
        {
            var containsMethodInfo = type.GetMethod(EqualsMethodName, types);
            if (containsMethodInfo is null)
            {
                throw new InvalidOperationException($"Method {EqualsMethodName} not found for type {type}.");
            }

            return containsMethodInfo;
        }

        internal static MethodInfo GetStartsWithMethodInfo([DynamicallyAccessedMembers(MethodTrimmingAccess)] Type type, Type[] types)
        {
            var containsMethodInfo = type.GetMethod(StartsWithMethodName, types);
            if (containsMethodInfo is null)
            {
                throw new InvalidOperationException($"Method {StartsWithMethodName} not found for type {type}.");
            }

            return containsMethodInfo;
        }

        internal static MethodInfo GetEndsWithMethodInfo([DynamicallyAccessedMembers(MethodTrimmingAccess)] Type type, Type[] types)
        {
            var containsMethodInfo = type.GetMethod(EndsWithMethodName, types);
            if (containsMethodInfo is null)
            {
                throw new InvalidOperationException($"Method {EndsWithMethodName} not found for type {type}.");
            }

            return containsMethodInfo;
        }
    }
}
