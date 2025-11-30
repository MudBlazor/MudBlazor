using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    //[ExcludeFromCodeCoverage]
    //internal static class Num
    //{
    //    private static readonly Dictionary<Type, Func<double, object>> _toConverters =
    //        new()
    //        {
    //            [typeof(sbyte)] = d => (sbyte)d,
    //            [typeof(byte)] = d => (byte)d,
    //            [typeof(short)] = d => (short)d,
    //            [typeof(ushort)] = d => (ushort)d,
    //            [typeof(int)] = d => (int)d,
    //            [typeof(uint)] = d => (uint)d,
    //            [typeof(long)] = d => (long)d,
    //            [typeof(ulong)] = d => (ulong)d,
    //            [typeof(float)] = d => (float)d,
    //            [typeof(double)] = d => d,
    //            [typeof(decimal)] = d => (decimal)d,

    //            // Nullable types
    //            [typeof(sbyte?)] = d => (sbyte)d,
    //            [typeof(byte?)] = d => (byte)d,
    //            [typeof(short?)] = d => (short)d,
    //            [typeof(ushort?)] = d => (ushort)d,
    //            [typeof(int?)] = d => (int)d,
    //            [typeof(uint?)] = d => (uint)d,
    //            [typeof(long?)] = d => (long)d,
    //            [typeof(ulong?)] = d => (ulong)d,
    //            [typeof(float?)] = d => (float)d,
    //            [typeof(double?)] = d => d,
    //            [typeof(decimal?)] = d => (decimal)d,
    //        };

    //    public static T To<T>(double d)
    //    {
    //        if (_toConverters.TryGetValue(typeof(T), out var f))
    //            return (T)f(d);

    //        return default!;
    //    }

    //    private static readonly Dictionary<Type, Func<object, double>> _fromConverters =
    //        new()
    //        {
    //            [typeof(sbyte)] = v => (sbyte)v,
    //            [typeof(byte)] = v => (byte)v,
    //            [typeof(short)] = v => (short)v,
    //            [typeof(ushort)] = v => (ushort)v,
    //            [typeof(int)] = v => (int)v,
    //            [typeof(uint)] = v => (uint)v,
    //            [typeof(long)] = v => (long)v,
    //            [typeof(ulong)] = v => (ulong)v,
    //            [typeof(float)] = v => (float)v,
    //            [typeof(double)] = v => (double)v,
    //            [typeof(decimal)] = v => (double)(decimal)v,

    //            // Nullable types
    //            [typeof(sbyte?)] = v => (double)(sbyte?)v,
    //            [typeof(byte?)] = v => (double)(byte?)v,
    //            [typeof(short?)] = v => (double)(short?)v,
    //            [typeof(ushort?)] = v => (double)(ushort?)v,
    //            [typeof(int?)] = v => (double)(int?)v,
    //            [typeof(uint?)] = v => (double)(uint?)v,
    //            [typeof(long?)] = v => (double)(long?)v,
    //            [typeof(ulong?)] = v => (double)(ulong?)v,
    //            [typeof(float?)] = v => (double)(float?)v,
    //            [typeof(double?)] = v => (double)(double?)v,
    //            [typeof(decimal?)] = v => (double)(decimal?)v,
    //        };

    //    public static double From<T>(T v)
    //    {
    //        if (_fromConverters.TryGetValue(typeof(T), out var f))
    //            return f(v!);

    //        return 0;
    //    }
    //}

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
