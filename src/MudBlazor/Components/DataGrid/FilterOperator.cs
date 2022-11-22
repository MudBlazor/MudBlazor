// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace MudBlazor
{
    public static class FilterOperator
    {
        public static class String
        {
            public const string Contains = "contains";
            public const string NotContains = "not contains";
            public const string Equal = "equals";
            public const string NotEqual = "not equals";
            public const string StartsWith = "starts with";
            public const string EndsWith = "ends with";
            public const string Empty = "is empty";
            public const string NotEmpty = "is not empty";

            internal static string[] Values = GetFields(typeof(String));
        }

        public static class Number
        {
            public const string Equal = "=";
            public const string NotEqual = "!=";
            public const string GreaterThan = ">";
            public const string GreaterThanOrEqual = ">=";
            public const string LessThan = "<";
            public const string LessThanOrEqual = "<=";
            public const string Empty = "is empty";
            public const string NotEmpty = "is not empty";

            internal static string[] Values = GetFields(typeof(Number));
        }

        public static class Enum
        {
            public const string Is = "is";
            public const string IsNot = "is not";

            internal static string[] Values = GetFields(typeof(Enum));
        }

        public static class Boolean
        {
            public const string Is = "is";

            internal static string[] Values = GetFields(typeof(Boolean));
        }

        public static class DateTime
        {
            public const string Is = "is";
            public const string IsNot = "is not";
            public const string After = "is after";
            public const string OnOrAfter = "is on or after";
            public const string Before = "is before";
            public const string OnOrBefore = "is on or before";
            public const string Empty = "is empty";
            public const string NotEmpty = "is not empty";

            internal static string[] Values = GetFields(typeof(DateTime));
        }

        public static class Guid
        {
            public const string Equal = "equals";
            public const string NotEqual = "not equals";

            internal static string[] Values = GetFields(typeof(Guid));
        }

        internal static string[] GetOperatorByDataType(Type type)
        {
            if (type == typeof(string))
            {
                return String.Values;
            }
            if (IsNumber(type))
            {
                return Number.Values;
            }
            if (IsEnum(type))
            {
                return Enum.Values;
            }
            if (type == typeof(bool))
            {
                return Boolean.Values;
            }
            if (type == typeof(System.DateTime))
            {
                return DateTime.Values;
            }
            if (type == typeof(System.Guid))
            {
                return Guid.Values;
            }

            // default
            return new string[] { };
        }

        internal static string[] GetFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type type)
        {
            var fields = new List<string>();

            foreach (var field in type.GetFields().Where(fi => fi.IsLiteral))
            {
                var value = (string?)field.GetValue(null);
                if (value is not null)
                {
                    fields.Add(value);
                }
            }

            return fields.ToArray();
        }

        internal static readonly HashSet<Type> NumericTypes = new()
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

        internal static bool IsNumber(Type? type)
        {
            return type is not null && NumericTypes.Contains(type);
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

            if (type == typeof(System.DateTime))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(System.DateTime);
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

            if (type == typeof(System.Guid))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is not null && underlyingType == typeof(System.Guid);
        }
    }
}
