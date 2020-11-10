using System;
using System.Globalization;

namespace MudBlazor
{
    
    /// <summary>
    /// A universal T to double binding converter
    /// </summary>
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
                UpdateGetError("Conversion error: "+e.Message);
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
                else if (typeof(T) == typeof(int) )
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
                UpdateSetError("Conversion error: "+e.Message);
                return double.NaN;
            }
        }


        #region --> Floating Point comparison

        const double MinNormal = 2.2250738585072014E-308d;

        public static bool AreEqual(double a, double b, double epsilon = MinNormal)
        {
            // Copyright (c) Michael Borgwardt
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        #endregion
    }
}