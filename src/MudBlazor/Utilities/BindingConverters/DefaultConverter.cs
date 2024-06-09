using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace MudBlazor
{
    /// <summary>
    /// A universal T to string binding converter
    /// </summary>
    public class DefaultConverter<T> : Converter<T>
    {
        /// <summary>
        /// A static global delegate used if no converter is found. 
        /// </summary>
        public static Func<T, string> GlobalGetFunc;
        /// <summary>
        /// A static global delegate used if no converter is found.
        /// </summary>
        public static Func<string, T> GlobalSetFunc;

        public DefaultConverter()
        {
            SetFunc = ConvertToString;
            GetFunc = ConvertFromString;
        }

        public string DefaultTimeSpanFormat { get; set; } = "c";

        protected virtual T ConvertFromString(string value)
        {
            try
            {
                // string
                if (typeof(T) == typeof(string))
                    return (T)(object)value;

                // this is important, or otherwise all the TryParse down there might fail.
                if (string.IsNullOrEmpty(value))
                    return default(T);
                // char
                else if (typeof(T) == typeof(char) || typeof(T) == typeof(char?))
                {
                    return (T)(object)value[0];
                }
                // bool
                else if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                {
                    var lowerValue = value.ToLowerInvariant();
                    if (lowerValue is "true" or "on")
                        return (T)(object)true;
                    if (lowerValue is "false" or "off")
                        return (T)(object)false;
                    UpdateGetError("Not a valid boolean");
                }
                // sbyte
                else if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
                {
                    if (sbyte.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // byte
                else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
                {
                    if (byte.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // short
                else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
                {
                    if (short.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // ushort
                else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
                {
                    if (ushort.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // int
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                {
                    if (int.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // uint
                else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
                {
                    if (uint.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // long
                else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                {
                    if (long.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // ulong
                else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                {
                    if (ulong.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // float
                else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
                {
                    if (float.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // double
                else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                {
                    if (double.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // decimal
                else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                {
                    if (decimal.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid number");
                }
                // guid
                else if (typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
                {
                    if (Guid.TryParse(value, out var parsedValue))
                        return (T)(object)parsedValue;
                    UpdateGetError("Not a valid GUID");
                }
                // enum
                else if (IsNullableEnum(typeof(T)))
                {
                    var enum_type = Nullable.GetUnderlyingType(typeof(T));
                    if (Enum.TryParse(enum_type, value, out var parsedValue))
                        return (T)parsedValue;
                    UpdateGetError("Not a value of " + enum_type.Name);
                }
                else if (typeof(T).IsEnum)
                {
                    if (Enum.TryParse(typeof(T), value, out var parsedValue))
                        return (T)parsedValue;
                    UpdateGetError("Not a value of " + typeof(T).Name);
                }
                // datetime
                else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                {
                    try
                    {
                        return (T)(object)DateTime.ParseExact(value, Format ?? Culture.DateTimeFormat.ShortDatePattern, Culture);
                    }
                    catch (FormatException)
                    {
                        UpdateGetError("Not a valid date time");
                    }
                }
                // timespan
                else if (typeof(T) == typeof(TimeSpan) || typeof(T) == typeof(TimeSpan?))
                {
                    try
                    {
                        return (T)(object)TimeSpan.ParseExact(value, Format ?? DefaultTimeSpanFormat, Culture);
                    }
                    catch (Exception e) when (e is FormatException or OverflowException)
                    {
                        UpdateGetError("Not a valid time span");
                    }
                }
                else if (GlobalSetFunc != null)
                {
                    try
                    {
                        return GlobalSetFunc(value);
                    }
                    catch (Exception)
                    {
                        UpdateGetError($"Not a valid {typeof(T).Name}");
                    }
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

            return default(T);
        }

        protected virtual string ConvertToString(T arg)
        {
            if (arg == null)
                return null; // <-- this catches all nullable values which are null. no nullchecks necessary below!
            try
            {
                // string
                if (typeof(T) == typeof(string))
                    return (string)(object)arg;
                // char
                if (typeof(T) == typeof(char))
                    return ((char)(object)arg).ToString(Culture);
                if (typeof(T) == typeof(char?))
                    return ((char?)(object)arg).Value.ToString(Culture);
                // bool
                if (typeof(T) == typeof(bool))
                    return ((bool)(object)arg).ToString(CultureInfo.InvariantCulture);
                if (typeof(T) == typeof(bool?))
                    return ((bool?)(object)arg).Value.ToString(CultureInfo.InvariantCulture);
                // sbyte
                if (typeof(T) == typeof(sbyte))
                    return ((sbyte)(object)arg).ToString(Format, Culture);
                if (typeof(T) == typeof(sbyte?))
                    return ((sbyte?)(object)arg).Value.ToString(Format, Culture);
                // byte
                if (typeof(T) == typeof(byte))
                    return ((byte)(object)arg).ToString(Format, Culture);
                if (typeof(T) == typeof(byte?))
                    return ((byte?)(object)arg).Value.ToString(Format, Culture);
                // short
                if (typeof(T) == typeof(short))
                    return ((short)(object)arg).ToString(Format, Culture);
                if (typeof(T) == typeof(short?))
                    return ((short?)(object)arg).Value.ToString(Format, Culture);
                // ushort
                if (typeof(T) == typeof(ushort))
                    return ((ushort)(object)arg).ToString(Format, Culture);
                if (typeof(T) == typeof(ushort?))
                    return ((ushort?)(object)arg).Value.ToString(Format, Culture);
                // int
                else if (typeof(T) == typeof(int))
                    return ((int)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(int?))
                    return ((int?)(object)arg).Value.ToString(Format, Culture);
                // uint
                else if (typeof(T) == typeof(uint))
                    return ((uint)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(uint?))
                    return ((uint?)(object)arg).Value.ToString(Format, Culture);
                // long
                else if (typeof(T) == typeof(long))
                    return ((long)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(long?))
                    return ((long?)(object)arg).Value.ToString(Format, Culture);
                // ulong
                else if (typeof(T) == typeof(ulong))
                    return ((ulong)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(ulong?))
                    return ((ulong?)(object)arg).Value.ToString(Format, Culture);
                // float
                else if (typeof(T) == typeof(float))
                    return ((float)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(float?))
                    return ((float?)(object)arg).Value.ToString(Format, Culture);
                // double
                else if (typeof(T) == typeof(double))
                    return ((double)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(double?))
                    return ((double?)(object)arg).Value.ToString(Format, Culture);
                // decimal
                else if (typeof(T) == typeof(decimal))
                    return ((decimal)(object)arg).ToString(Format, Culture);
                else if (typeof(T) == typeof(decimal?))
                    return ((decimal?)(object)arg).Value.ToString(Format, Culture);
                // guid
                else if (typeof(T) == typeof(Guid))
                {
                    var value = (Guid)(object)arg;
                    return value.ToString();
                }
                else if (typeof(T) == typeof(Guid?))
                {
                    var value = (Guid?)(object)arg;
                    return value.Value.ToString();
                }
                // enum
                else if (IsNullableEnum(typeof(T)))
                {
                    var value = (Enum)(object)arg;
                    return value.ToString();
                }
                else if (typeof(T).IsEnum)
                {
                    var value = (Enum)(object)arg;
                    return value.ToString();
                }
                // datetime
                else if (typeof(T) == typeof(DateTime))
                {
                    var value = (DateTime)(object)arg;
                    return value.ToString(Format ?? Culture.DateTimeFormat.ShortDatePattern, Culture);
                }
                else if (typeof(T) == typeof(DateTime?))
                {
                    var value = (DateTime?)(object)arg;
                    return value.Value.ToString(Format ?? Culture.DateTimeFormat.ShortDatePattern, Culture);
                }
                // timespan
                else if (typeof(T) == typeof(TimeSpan))
                {
                    var value = (TimeSpan)(object)arg;
                    return value.ToString(Format ?? DefaultTimeSpanFormat, Culture);
                }
                else if (typeof(T) == typeof(TimeSpan?))
                {
                    var value = (TimeSpan?)(object)arg;
                    return value.Value.ToString(Format ?? DefaultTimeSpanFormat, Culture);
                }
                else if (GlobalGetFunc != null)
                {
                    return GlobalGetFunc(arg);
                }
                return arg.ToString();
            }
            catch (FormatException e)
            {
                UpdateSetError("Conversion error: " + e.Message);
                return null;
            }
        }

        public static bool IsNullableEnum(Type t)
        {
            var u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }
    }
}
