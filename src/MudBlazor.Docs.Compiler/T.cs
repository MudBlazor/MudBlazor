using System;
using System.Globalization;
using System.Numerics;

namespace MudBlazor.UnitTests
{
    public class T : INumber<T>
    {
        int IComparable.CompareTo(object obj) => 0;

        int IComparable<T>.CompareTo(T other) => 0;

        bool IEquatable<T>.Equals(T other) => false;

        string IFormattable.ToString(string format, IFormatProvider formatProvider) => string.Empty;

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
        {
            charsWritten = 0;
            return false;
        }

        static T IParsable<T>.Parse(string s, IFormatProvider provider) => new();

        static bool IParsable<T>.TryParse(string s, IFormatProvider provider, out T result)
        {
            result = new T();
            return false;
        }

        static T ISpanParsable<T>.Parse(ReadOnlySpan<char> s, IFormatProvider provider) => new();

        static bool ISpanParsable<T>.TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out T result)
        {
            result = new T();
            return false;
        }

        static T IAdditionOperators<T, T, T>.operator +(T left, T right) => new();

        static T IAdditiveIdentity<T, T>.AdditiveIdentity => new();

        static bool IEqualityOperators<T, T, bool>.operator ==(T left, T right) => false;

        static bool IEqualityOperators<T, T, bool>.operator !=(T left, T right) => false;

        static bool IComparisonOperators<T, T, bool>.operator >(T left, T right) => false;

        static bool IComparisonOperators<T, T, bool>.operator >=(T left, T right) => false;

        static bool IComparisonOperators<T, T, bool>.operator <(T left, T right) => false;

        static bool IComparisonOperators<T, T, bool>.operator <=(T left, T right) => false;

        static T IDecrementOperators<T>.operator --(T value) => new();

        static T IDivisionOperators<T, T, T>.operator /(T left, T right) => new();

        static T IIncrementOperators<T>.operator ++(T value) => new();

        static T IModulusOperators<T, T, T>.operator %(T left, T right) => new();

        static T IMultiplicativeIdentity<T, T>.MultiplicativeIdentity => new();

        static T IMultiplyOperators<T, T, T>.operator *(T left, T right) => new();

        static T ISubtractionOperators<T, T, T>.operator -(T left, T right) => new();

        static T IUnaryNegationOperators<T, T>.operator -(T value) => new();

        static T IUnaryPlusOperators<T, T>.operator +(T value) => new();

        static T INumberBase<T>.Abs(T value) => new();

        static bool INumberBase<T>.IsCanonical(T value) => false;

        static bool INumberBase<T>.IsComplexNumber(T value) => false;

        static bool INumberBase<T>.IsEvenInteger(T value) => false;

        static bool INumberBase<T>.IsFinite(T value) => false;

        static bool INumberBase<T>.IsImaginaryNumber(T value) => false;

        static bool INumberBase<T>.IsInfinity(T value) => false;

        static bool INumberBase<T>.IsInteger(T value) => false;

        static bool INumberBase<T>.IsNaN(T value) => false;

        static bool INumberBase<T>.IsNegative(T value) => false;

        static bool INumberBase<T>.IsNegativeInfinity(T value) => false;

        static bool INumberBase<T>.IsNormal(T value) => false;

        static bool INumberBase<T>.IsOddInteger(T value) => false;

        static bool INumberBase<T>.IsPositive(T value) => false;

        static bool INumberBase<T>.IsPositiveInfinity(T value) => false;

        static bool INumberBase<T>.IsRealNumber(T value) => false;

        static bool INumberBase<T>.IsSubnormal(T value) => false;

        static bool INumberBase<T>.IsZero(T value) => false;

        static T INumberBase<T>.MaxMagnitude(T x, T y) => new();

        static T INumberBase<T>.MaxMagnitudeNumber(T x, T y) => new();

        static T INumberBase<T>.MinMagnitude(T x, T y) => new();

        static T INumberBase<T>.MinMagnitudeNumber(T x, T y) => new();

        static T INumberBase<T>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => new();

        static T INumberBase<T>.Parse(string s, NumberStyles style, IFormatProvider provider) => new();

        static bool INumberBase<T>.TryConvertFromChecked<TOther>(TOther value, out T result)
        {
            result = new T();
            return false;
        }

        static bool INumberBase<T>.TryConvertFromSaturating<TOther>(TOther value, out T result)
        {
            result = new T();
            return false;
        }

        static bool INumberBase<T>.TryConvertFromTruncating<TOther>(TOther value, out T result)
        {
            result = new T();
            return false;
        }

        static bool INumberBase<T>.TryConvertToChecked<TOther>(T value, out TOther result)
        {
            result = default;
            return false;
        }

        static bool INumberBase<T>.TryConvertToSaturating<TOther>(T value, out TOther result)
        {
            result = default;
            return false;
        }

        static bool INumberBase<T>.TryConvertToTruncating<TOther>(T value, out TOther result)
        {
            result = default;
            return false;
        }

        static bool INumberBase<T>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out T result)
        {
            result = new T();
            return false;
        }

        static bool INumberBase<T>.TryParse(string s, NumberStyles style, IFormatProvider provider, out T result)
        {
            result = new T();
            return false;
        }

        static T INumberBase<T>.One => new();

        static int INumberBase<T>.Radix => 0;

        static T INumberBase<T>.Zero => new();

        static T INumberBase<T>.CreateChecked<TOther>(TOther value) => new();

        static T INumberBase<T>.CreateTruncating<TOther>(TOther value) => new();

        static T INumberBase<T>.CreateSaturating<TOther>(TOther value) => new();

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 0;
    }

    public struct TStruct : INumber<TStruct>
    {
        int IComparable.CompareTo(object obj) => 0;

        int IComparable<TStruct>.CompareTo(TStruct other) => 0;

        bool IEquatable<TStruct>.Equals(TStruct other) => false;

        string IFormattable.ToString(string format, IFormatProvider formatProvider) => string.Empty;

        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
        {
            charsWritten = 0;
            return false;
        }

        static TStruct IParsable<TStruct>.Parse(string s, IFormatProvider provider) => new();

        static bool IParsable<TStruct>.TryParse(string s, IFormatProvider provider, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static TStruct ISpanParsable<TStruct>.Parse(ReadOnlySpan<char> s, IFormatProvider provider) => new();

        static bool ISpanParsable<TStruct>.TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static TStruct IAdditionOperators<TStruct, TStruct, TStruct>.operator +(TStruct left, TStruct right) => new();

        static TStruct IAdditiveIdentity<TStruct, TStruct>.AdditiveIdentity => new();

        static bool IEqualityOperators<TStruct, TStruct, bool>.operator ==(TStruct left, TStruct right) => false;

        static bool IEqualityOperators<TStruct, TStruct, bool>.operator !=(TStruct left, TStruct right) => false;

        static bool IComparisonOperators<TStruct, TStruct, bool>.operator >(TStruct left, TStruct right) => false;

        static bool IComparisonOperators<TStruct, TStruct, bool>.operator >=(TStruct left, TStruct right) => false;

        static bool IComparisonOperators<TStruct, TStruct, bool>.operator <(TStruct left, TStruct right) => false;

        static bool IComparisonOperators<TStruct, TStruct, bool>.operator <=(TStruct left, TStruct right) => false;

        static TStruct IDecrementOperators<TStruct>.operator --(TStruct value) => new();

        static TStruct IDivisionOperators<TStruct, TStruct, TStruct>.operator /(TStruct left, TStruct right) => new();

        static TStruct IIncrementOperators<TStruct>.operator ++(TStruct value) => new();

        static TStruct IModulusOperators<TStruct, TStruct, TStruct>.operator %(TStruct left, TStruct right) => new();

        static TStruct IMultiplicativeIdentity<TStruct, TStruct>.MultiplicativeIdentity => new();

        static TStruct IMultiplyOperators<TStruct, TStruct, TStruct>.operator *(TStruct left, TStruct right) => new();

        static TStruct ISubtractionOperators<TStruct, TStruct, TStruct>.operator -(TStruct left, TStruct right) => new();

        static TStruct IUnaryNegationOperators<TStruct, TStruct>.operator -(TStruct value) => new();

        static TStruct IUnaryPlusOperators<TStruct, TStruct>.operator +(TStruct value) => new();

        static TStruct INumberBase<TStruct>.Abs(TStruct value) => new();

        static bool INumberBase<TStruct>.IsCanonical(TStruct value) => false;

        static bool INumberBase<TStruct>.IsComplexNumber(TStruct value) => false;

        static bool INumberBase<TStruct>.IsEvenInteger(TStruct value) => false;

        static bool INumberBase<TStruct>.IsFinite(TStruct value) => false;

        static bool INumberBase<TStruct>.IsImaginaryNumber(TStruct value) => false;

        static bool INumberBase<TStruct>.IsInfinity(TStruct value) => false;

        static bool INumberBase<TStruct>.IsInteger(TStruct value) => false;

        static bool INumberBase<TStruct>.IsNaN(TStruct value) => false;

        static bool INumberBase<TStruct>.IsNegative(TStruct value) => false;

        static bool INumberBase<TStruct>.IsNegativeInfinity(TStruct value) => false;

        static bool INumberBase<TStruct>.IsNormal(TStruct value) => false;

        static bool INumberBase<TStruct>.IsOddInteger(TStruct value) => false;

        static bool INumberBase<TStruct>.IsPositive(TStruct value) => false;

        static bool INumberBase<TStruct>.IsPositiveInfinity(TStruct value) => false;

        static bool INumberBase<TStruct>.IsRealNumber(TStruct value) => false;

        static bool INumberBase<TStruct>.IsSubnormal(TStruct value) => false;

        static bool INumberBase<TStruct>.IsZero(TStruct value) => false;

        static TStruct INumberBase<TStruct>.MaxMagnitude(TStruct x, TStruct y) => new();

        static TStruct INumberBase<TStruct>.MaxMagnitudeNumber(TStruct x, TStruct y) => new();

        static TStruct INumberBase<TStruct>.MinMagnitude(TStruct x, TStruct y) => new();

        static TStruct INumberBase<TStruct>.MinMagnitudeNumber(TStruct x, TStruct y) => new();

        static TStruct INumberBase<TStruct>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => new();

        static TStruct INumberBase<TStruct>.Parse(string s, NumberStyles style, IFormatProvider provider) => new();

        static bool INumberBase<TStruct>.TryConvertFromChecked<TOther>(TOther value, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static bool INumberBase<TStruct>.TryConvertFromSaturating<TOther>(TOther value, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static bool INumberBase<TStruct>.TryConvertFromTruncating<TOther>(TOther value, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static bool INumberBase<TStruct>.TryConvertToChecked<TOther>(TStruct value, out TOther result)
        {
            result = default;
            return false;
        }

        static bool INumberBase<TStruct>.TryConvertToSaturating<TOther>(TStruct value, out TOther result)
        {
            result = default;
            return false;
        }

        static bool INumberBase<TStruct>.TryConvertToTruncating<TOther>(TStruct value, out TOther result)
        {
            result = default;
            return false;
        }

        static bool INumberBase<TStruct>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static bool INumberBase<TStruct>.TryParse(string s, NumberStyles style, IFormatProvider provider, out TStruct result)
        {
            result = new TStruct();
            return false;
        }

        static TStruct INumberBase<TStruct>.One => new();

        static int INumberBase<TStruct>.Radix => 0;

        static TStruct INumberBase<TStruct>.Zero => new();

        static TStruct INumberBase<TStruct>.CreateChecked<TOther>(TOther value) => new();

        static TStruct INumberBase<TStruct>.CreateTruncating<TOther>(TOther value) => new();

        static TStruct INumberBase<TStruct>.CreateSaturating<TOther>(TOther value) => new();

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 0;
    }

    public class U { }
}
