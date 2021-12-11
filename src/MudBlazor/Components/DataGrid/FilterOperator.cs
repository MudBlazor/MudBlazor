// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MudBlazor
{
    public static class FilterOperator
    {
        public static class String
        {
            public const string Contains = "contains";
            public const string Equal = "equals";
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
            if (type.IsEnum)
            {
                return Enum.Values;
            }
            if (type == typeof(bool))
            {
                return Boolean.Values;
            }

            // default
            return new string[] { };
        }

        internal static string[] GetFields(Type type)
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
            typeof(BigInteger)
        };

        internal static bool IsNumber(Type type)
        {
            return NumericTypes.Contains(type);
        }
    }
}
