// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        internal static string[] GetFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            List<string> fields = new List<string>();

            foreach (var field in type.GetFields().Where(fi => fi.IsLiteral))
            {
                fields.Add((string)field.GetValue(null));
            }

            return fields.ToArray();
        }

        internal static readonly HashSet<Type> NumericTypes = new HashSet<Type>
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

        internal static bool IsNumber([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            return NumericTypes.Contains(type);
        }

        internal static bool IsEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            if (null == type)
                return false;

            if (type.IsEnum)
                return true;

            Type u = Nullable.GetUnderlyingType(type);
            return (u != null) && u.IsEnum;
        }

        internal static bool IsDateTime([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            if (type == typeof(System.DateTime))
                return true;

            Type u = Nullable.GetUnderlyingType(type);
            return (u != null) && u == typeof(System.DateTime);
        }

        internal static bool IsBoolean([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
        {
            if (type == typeof(bool))
                return true;

            Type u = Nullable.GetUnderlyingType(type);
            return (u != null) && u == typeof(bool);
        }

        internal static bool IsGuid(Type type)
        {
            if (type == typeof(System.Guid))
                return true;

            Type u = Nullable.GetUnderlyingType(type);
            return (u != null) && u == typeof(System.Guid);
        }
    }
}
