// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace MudBlazor.Components.NumericField
{
    public class NumericBoundariesConverter<T> : Converter<string>
    {
        public Func<T, T> BoundariesFunc { get; set; }

        public NumericBoundariesConverter(Func<T, T> boundariesFunc)
        {
            BoundariesFunc = boundariesFunc;
            SetFunc = (value) => value;
            GetFunc = OnGet;
        }

        private string OnGet(string value)
        {
            try
            {
                // sbyte
                if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
                {
                    if (sbyte.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                }
                // byte
                else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
                {
                    if (byte.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // short
                else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
                {
                    if (short.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // ushort
                else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
                {
                    if (ushort.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // int
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                {
                    if (int.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // uint
                else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
                {
                    if (uint.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // long
                else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                {
                    if (long.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // ulong
                else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                {
                    if (ulong.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // float
                else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
                {
                    if (float.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // double
                else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                {
                    if (double.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                // decimal
                else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                {
                    if (decimal.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return CheckBoundaries((T)(object)parsedValue);
                    UpdateGetError("Not a valid number");
                }
                else
                {
                    UpdateGetError($"Conversion to type {typeof(T)} not implemented");
                }
            }
            catch (Exception e)
            {
                UpdateGetError("Conversion error: " + e.Message);
            }

            return null;
        }

        private string CheckBoundaries(T value) => Convert.ToString(BoundariesFunc.Invoke(value), Culture);
    }
}
