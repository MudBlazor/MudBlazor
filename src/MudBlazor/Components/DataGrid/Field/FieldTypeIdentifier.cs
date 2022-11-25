// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MudBlazor
{
    internal class FieldTypeIdentifier
    {
        private static readonly HashSet<Type> _numericTypes = new()
        {
            typeof(int),
            typeof(double),
            typeof(decimal),
            typeof(long),
            typeof(short),
            typeof(sbyte),
            typeof(byte),
            typeof(ulong),
            typeof(ushort),
            typeof(uint),
            typeof(float),
            typeof(BigInteger),
            typeof(int?),
            typeof(double?),
            typeof(decimal?),
            typeof(long?),
            typeof(short?),
            typeof(sbyte?),
            typeof(byte?),
            typeof(ulong?),
            typeof(ushort?),
            typeof(uint?),
            typeof(float?),
            typeof(BigInteger?),
        };

        internal static bool IsString(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(string))
                return true;

            return false;
        }

        internal static bool IsNumber(Type? type)
        {
            return type is not null && _numericTypes.Contains(type);
        }

        internal static bool IsEnum(Type? type)
        {
            if (type is null)
                return false;

            if (type.IsEnum)
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is { IsEnum: true };
        }

        internal static bool IsDateTime(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(DateTime))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(DateTime);
        }

        internal static bool IsBoolean(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(bool))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(bool);
        }

        internal static bool IsGuid(Type? type)
        {
            if (type is null)
                return false;

            if (type == typeof(Guid))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(Guid);
        }
    }
}
