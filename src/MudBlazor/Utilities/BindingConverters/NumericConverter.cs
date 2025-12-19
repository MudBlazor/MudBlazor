using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    internal static class Num
    {
        public static T? To<T>(double? d)
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
            if (typeof(T) == typeof(decimal) && (decimal?)d >= decimal.MinValue && decimal.MaxValue >= (decimal?)d)
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
            if (typeof(T) == typeof(decimal?) && (decimal?)d >= decimal.MinValue && decimal.MaxValue >= (decimal?)d)
                return (T)(object)Convert.ToDecimal(d);
            return default;
        }
        public static double? From<T>(T? v)
        {
            if (typeof(T) == typeof(sbyte))
                return Convert.ToDouble((sbyte?)(object?)v);
            if (typeof(T) == typeof(byte))
                return Convert.ToDouble((byte?)(object?)v);
            if (typeof(T) == typeof(short))
                return Convert.ToDouble((short?)(object?)v);
            if (typeof(T) == typeof(ushort))
                return Convert.ToDouble((ushort?)(object?)v);
            if (typeof(T) == typeof(int))
                return Convert.ToDouble((int?)(object?)v);
            if (typeof(T) == typeof(uint))
                return Convert.ToDouble((uint?)(object?)v);
            if (typeof(T) == typeof(long))
                return Convert.ToDouble((long?)(object?)v);
            if (typeof(T) == typeof(ulong))
                return Convert.ToDouble((ulong?)(object?)v);
            if (typeof(T) == typeof(float))
                return Convert.ToDouble((float?)(object?)v);
            if (typeof(T) == typeof(double))
                return Convert.ToDouble((double?)(object?)v);
            if (typeof(T) == typeof(decimal))
                return Convert.ToDouble((decimal?)(object?)v);
            if (typeof(T) == typeof(sbyte?))
                return Convert.ToDouble((sbyte?)(object?)v);
            if (typeof(T) == typeof(byte?))
                return Convert.ToDouble((byte?)(object?)v);
            if (typeof(T) == typeof(short?))
                return Convert.ToDouble((short?)(object?)v);
            if (typeof(T) == typeof(ushort?))
                return Convert.ToDouble((ushort?)(object?)v);
            if (typeof(T) == typeof(int?))
                return Convert.ToDouble((int?)(object?)v);
            if (typeof(T) == typeof(uint?))
                return Convert.ToDouble((uint?)(object?)v);
            if (typeof(T) == typeof(long?))
                return Convert.ToDouble((long?)(object?)v);
            if (typeof(T) == typeof(ulong?))
                return Convert.ToDouble((ulong?)(object?)v);
            if (typeof(T) == typeof(float?))
                return Convert.ToDouble((float?)(object?)v);
            if (typeof(T) == typeof(double?))
                return Convert.ToDouble((double?)(object?)v);
            if (typeof(T) == typeof(decimal?))
                return Convert.ToDouble((decimal?)(object?)v);
            return default;
        }
    }
}
