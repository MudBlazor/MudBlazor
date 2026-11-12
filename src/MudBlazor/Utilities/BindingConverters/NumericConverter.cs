using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    internal static class Num
    {
        // From .NET 11 on, converting a double or float to decimal gives the exact binary value, so 0.1 becomes 0.1000000000000000055511151231.
        // Earlier runtimes kept 15 significant digits of a double and 7 of a float, and going through text keeps that on every runtime.
        public static decimal ToDecimal(double d)
        {
            if (!double.IsFinite(d))
                throw new OverflowException();
            return decimal.Parse(d.ToString("G15", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(float f)
        {
            if (!float.IsFinite(f))
                throw new OverflowException();
            return decimal.Parse(f.ToString("G7", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(object value) => value switch
        {
            double d => ToDecimal(d),
            float f => ToDecimal(f),
            _ => Convert.ToDecimal(value)
        };

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
                return (T)(object)ToDecimal(d.Value);
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
                return (T)(object)ToDecimal(d.Value);
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
