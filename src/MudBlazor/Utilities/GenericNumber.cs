using System;

namespace MudBlazor.Utilities
{

    /// <summary>
    /// This structure allow a generic to be nullable, for example when used inside a generic class that doesn't restrict the type to structures, bypassing an invalid declaration of <see cref="Nullable{T}"/>.
    /// It also includes methods to easily convert to and from numeric values.
    /// </summary>
    public struct GenericNumber<T>
    {
        public T Value { get; set; }
        public bool HasValue { get; init; }

        public GenericNumber(T value)
        {
            HasValue = true;
            Value = value;
        }

        #region Conversion
        #region Conversion with fallback
        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public sbyte ToSByte(sbyte fallback = default(sbyte)) => HasValue ? (sbyte)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public byte ToByte(byte fallback = default(byte)) => HasValue ? (byte)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public short ToInt16(short fallback = default(short)) => HasValue ? (short)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public ushort ToUInt16(ushort fallback = default(ushort)) => HasValue ? (ushort)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public int ToInt32(int fallback = default(int)) => HasValue ? (int)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public uint ToUInt32(uint fallback = default(uint)) => HasValue ? (uint)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public long ToInt64(long fallback = default(long)) => HasValue ? (long)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public ulong ToUInt64(ulong fallback = default(ulong)) => HasValue ? (ulong)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public float ToSingle(float fallback = default(float)) => HasValue ? (float)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public double ToDouble(double fallback = default(double)) => HasValue ? (double)(object)Value : fallback;

        /// <summary>
        /// Returns the value of this instance, if it has no value it returns the provided <paramref name="fallback"/> or the default for the type.
        /// </summary>
        /// <param name="fallback">Fallback value in case this instance is null.</param>
        public decimal ToDecimal(decimal fallback = default(decimal)) => HasValue ? (decimal)(object)Value : fallback;
        #endregion

        #region cast
        public static implicit operator GenericNumber<T>(sbyte i) => new((T)(object)i);
        public static explicit operator sbyte(GenericNumber<T> i) => i.HasValue ? (sbyte)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(byte i) => new((T)(object)i);
        public static explicit operator byte(GenericNumber<T> i) => i.HasValue ? (byte)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(short i) => new((T)(object)i);
        public static explicit operator short(GenericNumber<T> i) => i.HasValue ? (short)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(ushort i) => new((T)(object)i);
        public static explicit operator ushort(GenericNumber<T> i) => i.HasValue ? (ushort)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(int i) => new((T)(object)i);
        public static explicit operator int(GenericNumber<T> i) => i.HasValue ? (int)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(uint i) => new((T)(object)i);
        public static explicit operator uint(GenericNumber<T> i) => i.HasValue ? (uint)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(long i) => new((T)(object)i);
        public static explicit operator long(GenericNumber<T> i) => i.HasValue ? (long)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(ulong i) => new((T)(object)i);
        public static explicit operator ulong(GenericNumber<T> i) => i.HasValue ? (ulong)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(float i) => new((T)(object)i);
        public static explicit operator float(GenericNumber<T> i) => i.HasValue ? (float)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(double i) => new((T)(object)i);
        public static explicit operator double(GenericNumber<T> i) => i.HasValue ? (double)(object)i.Value : throw new NullReferenceException();

        public static implicit operator GenericNumber<T>(decimal i) => new((T)(object)i);
        public static explicit operator decimal(GenericNumber<T> i) => i.HasValue ? (decimal)(object)i.Value : throw new NullReferenceException();
        #endregion
        #endregion
    }
}
