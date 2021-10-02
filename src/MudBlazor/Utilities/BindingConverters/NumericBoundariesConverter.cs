// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace MudBlazor
{
    /// <summary>
    /// This converter is used by MudNumericInput to enforce the Min/Max boundaries on the textbox.
    /// It's a string => string converter, the number it's only to enforce the limits.
    /// </summary>
    /// <typeparam name="T">Must be a numeric type</typeparam>
    public class NumericBoundariesConverter<T> : Converter<string>
    {
#nullable enable
        public Func<T, T> EvaluationFunc { get; set; }
        public Func<string, string> FilterFunc { get; set; }
#nullable disable

        public NumericBoundariesConverter(Func<T, T> evaluationFunc)
        {
            EvaluationFunc = evaluationFunc;
            SetFunc = (value) => value; //Set doesn't do anything, awaits the Get to commit changes (IE: typing the decimal separator shouldn't trigger changes)
            GetFunc = OnGet;
        }

        private string OnGet(string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;
            var value = FilterFunc == null ? text : FilterFunc(text);
            try
            {
                // sbyte
                if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
                {
                    if (sbyte.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((sbyte)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                }
                // byte
                else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
                {
                    if (byte.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((byte)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // short
                else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
                {
                    if (short.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((short)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // ushort
                else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
                {
                    if (ushort.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((ushort)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // int
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                {
                    if (int.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((int)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // uint
                else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
                {
                    if (uint.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((uint)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // long
                else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                {
                    if (long.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((long)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // ulong
                else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                {
                    if (ulong.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return ((ulong)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // float
                else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
                {
                    if (float.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return ((float)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // double
                else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                {
                    if (double.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return ((double)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
                    UpdateGetError("Not a valid number");
                }
                // decimal
                else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                {
                    if (decimal.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return ((decimal)(object)CheckBoundaries((T)(object)parsedValue)).ToString(Format, Culture);
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


        private T CheckBoundaries(T value) => EvaluationFunc.Invoke(value);
    }
}
