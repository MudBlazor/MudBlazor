using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MudBlazor
{

    /// <summary>
    /// <para>A universal T to double binding converter</para>
    /// <para>
    /// Note: currently not in use. Should we ever use it, remove
    /// the  [ExcludeFromCodeCoverage] attribute
    /// </para>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NumericConverter<T> : Converter<T, double>
    {

        public NumericConverter()
        {
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private T OnGet(double value)
        {
            try
            {
                // double
                if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
                    return (T)(object)value;
                // string
                else if (typeof(T) == typeof(string))
                    return (T)(object)value.ToString(Culture);
                // sbyte
                else if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
                    return (T)(object)Convert.ToSByte(value);
                // byte
                else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
                    return (T)(object)Convert.ToByte(value);
                // short
                else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
                    return (T)(object)Convert.ToInt16(value);
                // ushort
                else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
                    return (T)(object)Convert.ToUInt16(value);
                // int
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                    return (T)(object)Convert.ToInt32(value);
                // uint
                else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
                    return (T)(object)Convert.ToUInt32(value);
                // long
                else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                    return (T)(object)Convert.ToInt64(value);
                // ulong
                else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                    return (T)(object)Convert.ToUInt64(value);
                // float
                else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
                    return (T)(object)Convert.ToSingle(value);
                // decimal
                else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                    return (T)(object)Convert.ToDecimal(value);
                else
                {
                    UpdateGetError($"Conversion to type {typeof(T)} not implemented");
                }
            }
            catch (Exception e)
            {
                UpdateGetError("Conversion error: " + e.Message);
                return default(T);
            }
            return default(T);
        }

        private double OnSet(T arg)
        {
            if (arg == null)
                return double.NaN; // <-- this catches all nullable values which are null. no nullchecks necessary below!
            try
            {
                // double
                if (typeof(T) == typeof(double))
                    return (double)(object)arg;
                else if (typeof(T) == typeof(double?))
                    return ((double?)(object)arg).Value;
                // string
                if (typeof(T) == typeof(string))
                    return double.Parse((string)(object)arg, NumberStyles.Any, Culture);
                // sbyte
                if (typeof(T) == typeof(sbyte))
                    return Convert.ToDouble((sbyte)(object)arg);
                if (typeof(T) == typeof(sbyte?))
                    return Convert.ToDouble(((sbyte?)(object)arg).Value);
                // byte
                if (typeof(T) == typeof(byte))
                    return Convert.ToDouble((byte)(object)arg);
                if (typeof(T) == typeof(byte?))
                    return Convert.ToDouble(((byte?)(object)arg).Value);
                // short
                if (typeof(T) == typeof(short))
                    return Convert.ToDouble((short)(object)arg);
                if (typeof(T) == typeof(short?))
                    return Convert.ToDouble(((short?)(object)arg).Value);
                // ushort
                if (typeof(T) == typeof(ushort))
                    return Convert.ToDouble((ushort)(object)arg);
                if (typeof(T) == typeof(ushort?))
                    return Convert.ToDouble(((ushort?)(object)arg).Value);
                // int
                else if (typeof(T) == typeof(int))
                    return Convert.ToDouble((int)(object)arg);
                else if (typeof(T) == typeof(int?))
                    return Convert.ToDouble(((int?)(object)arg).Value);
                // uint
                else if (typeof(T) == typeof(uint))
                    return Convert.ToDouble((uint)(object)arg);
                else if (typeof(T) == typeof(uint?))
                    return Convert.ToDouble(((uint?)(object)arg).Value);
                // long
                else if (typeof(T) == typeof(long))
                    return Convert.ToDouble((long)(object)arg);
                else if (typeof(T) == typeof(long?))
                    return Convert.ToDouble(((long?)(object)arg).Value);
                // ulong
                else if (typeof(T) == typeof(ulong))
                    return Convert.ToDouble((ulong)(object)arg);
                else if (typeof(T) == typeof(ulong?))
                    return Convert.ToDouble(((ulong?)(object)arg).Value);
                // float
                else if (typeof(T) == typeof(float))
                    return Convert.ToDouble((float)(object)arg);
                else if (typeof(T) == typeof(float?))
                    return Convert.ToDouble(((float?)(object)arg).Value);
                // decimal
                else if (typeof(T) == typeof(decimal))
                    return Convert.ToDouble((decimal)(object)arg);
                else if (typeof(T) == typeof(decimal?))
                    return Convert.ToDouble(((decimal?)(object)arg).Value);
                else
                {
                    UpdateSetError("Unable to convert to double from type " + typeof(T).Name);
                    return double.NaN;
                }
            }
            catch (FormatException e)
            {
                UpdateSetError("Conversion error: " + e.Message);
                return double.NaN;
            }
        }
    }

    [ExcludeFromCodeCoverage]
    internal static class Num
    {
        public static T To<T>(double d)
        {
            if (typeof(T) == typeof(sbyte) && d >= sbyte.MinValue && sbyte.MaxValue >= d)
                return (T)(object)Convert.ToSByte(d);
            if (typeof(T) == typeof(byte) && d >= byte.MinValue && byte.MaxValue >= d)
                return (T)(object)Convert.ToByte(d);
            if (typeof(T) == typeof(short) && d >= short.MinValue && short.MaxValue >= d)
                return (T)(object)Convert.ToInt16(d);
            if (typeof(T) == typeof(ushort) && d >= ushort.MinValue && ushort.MaxValue >= d)
                return (T)(object)Convert.ToUInt16(d);
            if (typeof(T) == typeof(int) && d >= int.MinValue && int.MaxValue >= d)
                return (T)(object)Convert.ToInt32(d);
            if (typeof(T) == typeof(uint) && d >= uint.MinValue && uint.MaxValue >= d)
                return (T)(object)Convert.ToUInt32(d);
            if (typeof(T) == typeof(long) && d >= long.MinValue && long.MaxValue >= d)
                return (T)(object)Convert.ToInt64(d);
            if (typeof(T) == typeof(ulong) && d >= ulong.MinValue && ulong.MaxValue >= d)
                return (T)(object)Convert.ToUInt64(d);
            if (typeof(T) == typeof(float) && d >= float.MinValue && float.MaxValue >= d)
                return (T)(object)Convert.ToSingle(d);
            if (typeof(T) == typeof(double) && d >= double.MinValue && double.MaxValue >= d)
                return (T)(object)Convert.ToDouble(d);
            if (typeof(T) == typeof(decimal) && (decimal)d >= decimal.MinValue && decimal.MaxValue >= (decimal)d)
                return (T)(object)Convert.ToDecimal(d);
            if (typeof(T) == typeof(sbyte?) && d >= sbyte.MinValue && sbyte.MaxValue >= d)
                return (T)(object)Convert.ToSByte(d);
            if (typeof(T) == typeof(byte?) && d >= byte.MinValue && byte.MaxValue >= d)
                return (T)(object)Convert.ToByte(d);
            if (typeof(T) == typeof(short?) && d >= short.MinValue && short.MaxValue >= d)
                return (T)(object)Convert.ToInt16(d);
            if (typeof(T) == typeof(ushort?) && d >= ushort.MinValue && ushort.MaxValue >= d)
                return (T)(object)Convert.ToUInt16(d);
            if (typeof(T) == typeof(int?) && d >= int.MinValue && int.MaxValue >= d)
                return (T)(object)Convert.ToInt32(d);
            if (typeof(T) == typeof(uint?) && d >= uint.MinValue && uint.MaxValue >= d)
                return (T)(object)Convert.ToUInt32(d);
            if (typeof(T) == typeof(long?) && d >= long.MinValue && long.MaxValue >= d)
                return (T)(object)Convert.ToInt64(d);
            if (typeof(T) == typeof(ulong?) && d >= ulong.MinValue && ulong.MaxValue >= d)
                return (T)(object)Convert.ToUInt64(d);
            if (typeof(T) == typeof(float?) && d >= float.MinValue && float.MaxValue >= d)
                return (T)(object)Convert.ToSingle(d);
            if (typeof(T) == typeof(double?) && d >= double.MinValue && double.MaxValue >= d)
                return (T)(object)Convert.ToDouble(d);
            if (typeof(T) == typeof(decimal?) && (decimal)d >= decimal.MinValue && decimal.MaxValue >= (decimal)d)
                return (T)(object)Convert.ToDecimal(d);
            return default;
        }
        public static double From<T>(T v)
        {
            if (typeof(T) == typeof(sbyte))
                return Convert.ToDouble((sbyte)(object)v);
            if (typeof(T) == typeof(byte))
                return Convert.ToDouble((byte)(object)v);
            if (typeof(T) == typeof(short))
                return Convert.ToDouble((short)(object)v);
            if (typeof(T) == typeof(ushort))
                return Convert.ToDouble((ushort)(object)v);
            if (typeof(T) == typeof(int))
                return Convert.ToDouble((int)(object)v);
            if (typeof(T) == typeof(uint))
                return Convert.ToDouble((uint)(object)v);
            if (typeof(T) == typeof(long))
                return Convert.ToDouble((long)(object)v);
            if (typeof(T) == typeof(ulong))
                return Convert.ToDouble((ulong)(object)v);
            if (typeof(T) == typeof(float))
                return Convert.ToDouble((float)(object)v);
            if (typeof(T) == typeof(double))
                return Convert.ToDouble((double)(object)v);
            if (typeof(T) == typeof(decimal))
                return Convert.ToDouble((decimal)(object)v);
            if (typeof(T) == typeof(sbyte?))
                return Convert.ToDouble((sbyte?)(object)v);
            if (typeof(T) == typeof(byte?))
                return Convert.ToDouble((byte?)(object)v);
            if (typeof(T) == typeof(short?))
                return Convert.ToDouble((short?)(object)v);
            if (typeof(T) == typeof(ushort?))
                return Convert.ToDouble((ushort?)(object)v);
            if (typeof(T) == typeof(int?))
                return Convert.ToDouble((int?)(object)v);
            if (typeof(T) == typeof(uint?))
                return Convert.ToDouble((uint?)(object)v);
            if (typeof(T) == typeof(long?))
                return Convert.ToDouble((long?)(object)v);
            if (typeof(T) == typeof(ulong?))
                return Convert.ToDouble((ulong?)(object)v);
            if (typeof(T) == typeof(float?))
                return Convert.ToDouble((float?)(object)v);
            if (typeof(T) == typeof(double?))
                return Convert.ToDouble((double?)(object)v);
            if (typeof(T) == typeof(decimal?))
                return Convert.ToDouble((decimal?)(object)v);
            return default;
        }
    }
}
