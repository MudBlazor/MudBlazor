// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using MudBlazor.Resources;
using MudBlazor.Utilities.Converter.Base;
using MudBlazor.Utilities.Converter.Dispatcher;

namespace MudBlazor.Utilities.Converter;

#nullable enable
public sealed class DefaultConverter<T> : IReversibleConverter<T?, string?>, ICultureAwareConverter
{
    private readonly IReversibleConverter<T?, string?> _dispatcher;

    public Func<string?> Format { get; set; } = () => null;

    public Func<CultureInfo> Culture { get; set; } = () => CultureInfo.InvariantCulture;

    public DefaultConverter()
    {
        // Do NOT pass Culture or Format directly.
        // The dispatcher caches method delegates and captures the converter's field values at registration time.
        // Using () => Culture() and () => Format() ensures the converters always read the latest property values.
        _dispatcher = ReversibleTypeDispatcher.Create<T?, string?>()
            .Add(StringIdentityConverter.Instance)
            .Add<char>(CharConverter.Instance)
            .Add<char?>(CharConverter.Instance)
            .Add<bool>(BoolConverter.Instance)
            .Add<bool?>(BoolConverter.Instance)
            .Add(new NumberConverter<sbyte>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<sbyte>(() => Culture(), () => Format()))
            .Add(new NumberConverter<byte>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<byte>(() => Culture(), () => Format()))
            .Add(new NumberConverter<short>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<short>(() => Culture(), () => Format()))
            .Add(new NumberConverter<ushort>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<ushort>(() => Culture(), () => Format()))
            .Add(new NumberConverter<int>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<int>(() => Culture(), () => Format()))
            .Add(new NumberConverter<uint>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<uint>(() => Culture(), () => Format()))
            .Add(new NumberConverter<long>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<long>(() => Culture(), () => Format()))
            .Add(new NumberConverter<ulong>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<ulong>(() => Culture(), () => Format()))
            .Add(new NumberConverter<float>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<float>(() => Culture(), () => Format()))
            .Add(new NumberConverter<double>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<double>(() => Culture(), () => Format()))
            .Add(new NumberConverter<decimal>(() => Culture(), () => Format()))
            .Add(new NullableNumberConverter<decimal>(() => Culture(), () => Format()))
            .Add<Guid>(StrictGuidStringConverter.Instance)
            .Add<Guid?>(StrictGuidStringConverter.Instance)
            .Add<DateTime>(new DateTimeConverter(() => Culture(), () => Format()))
            .Add<DateTime?>(new DateTimeConverter(() => Culture(), () => Format()))
            .Add<TimeSpan>(new TimeSpanConverter(() => Culture(), () => Format()))
            .Add<TimeSpan?>(new TimeSpanConverter(() => Culture(), () => Format()))
            //.Add(new ObjectConverter(() => Culture(), () => Format()))
            .Build();
    }

    public string? Convert(T? input)
    {
        //if (input is null)
        //{
        //    return null;
        //}

        // Special handling for enums
        if (IsNullableEnum(typeof(T), out _))
        {
            var value = input as Enum;
            return value?.ToString();
        }

        if (typeof(T).IsEnum)
        {
            var value = input as Enum;
            return value?.ToString();
        }

        var result = _dispatcher.TryConvert(input);
        // If conversion failed, fallback to ToString() implementation of the T
        return result.Success ? result.Value : input?.ToString();
    }

    public T? ConvertBack(string? input)
    {
        // Special handling for enums
        if (IsNullableEnum(typeof(T), out var enumType))
        {
            if (Enum.TryParse(enumType, input, out var result))
            {
                return (T)result;
            }

            throw new ConversionException(LanguageResource.Converter_NotValueOf, [enumType.Name]);
        }

        if (typeof(T).IsEnum)
        {
            if (Enum.TryParse(typeof(T), input, out var result))
            {
                return (T)result;
            }

            throw new ConversionException(LanguageResource.Converter_NotValueOf, [typeof(T).Name]);
        }

        return _dispatcher.ConvertBack(input);
    }

    private static bool IsNullableEnum(Type type, [NotNullWhen(true)] out Type? result)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType?.IsEnum is true)
        {
            result = underlyingType;
            return true;
        }

        result = null!;
        return false;
    }

    private sealed class StrictGuidStringConverter : IReversibleConverter<Guid, string?>, IReversibleConverter<Guid?, string?>
    {
        public Guid ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Guid.Empty;
            }

            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidGUID);
        }

        public string Convert(Guid value) => value.ToString();

        public string? Convert(Guid? value) => value is null ? null : value.ToString();

        Guid? IReversibleConverter<Guid?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }

        public static StrictGuidStringConverter Instance { get; } = new();
    }

    private sealed class CharConverter : IReversibleConverter<char, string?>, IReversibleConverter<char?, string?>
    {
        public string Convert(char input) => input.ToString();

        public string? Convert(char? input) => input?.ToString();

        public char ConvertBack(string? input) => string.IsNullOrEmpty(input) ? '\0' : input[0];

        char? IReversibleConverter<char?, string?>.ConvertBack(string? input)
        {
            return ConvertBack(input);
        }

        public static readonly CharConverter Instance = new();
    }

    private sealed class BoolConverter : IReversibleConverter<bool?, string?>, IReversibleConverter<bool, string?>
    {
        //public string Convert(bool input) => input ? "on" : "off";

        //public string? Convert(bool? input) => input switch
        //{
        //    true => "on",
        //    false => "off",
        //    null => null
        //};

        public string Convert(bool input) => input.ToString(CultureInfo.InvariantCulture);

        public string? Convert(bool? input) => input?.ToString(CultureInfo.InvariantCulture);

        public bool ConvertBack(string? input) =>
            input?.ToLowerInvariant() switch
            {
                "true" or "1" or "on" => true,
                _ => false
            };

        bool? IReversibleConverter<bool?, string?>.ConvertBack(string? input) =>
            input?.ToLowerInvariant() switch
            {
                "true" or "1" or "on" => true,
                "false" or "0" or "off" => false,
                _ => null
            };

        public static readonly BoolConverter Instance = new();
    }

    private sealed class StringIdentityConverter : IReversibleConverter<string?, string?>
    {
        public string? Convert(string? input) => input;

        public string? ConvertBack(string? input) => input;

        public static readonly StringIdentityConverter Instance = new();
    }

    private sealed class NullableNumberConverter<TNumber>(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<TNumber?, string?>
        where TNumber : struct, INumber<TNumber>
    {
        public string? Convert(TNumber? input)
        {
            var currentCulture = culture.Invoke();
            var currentFormat = format.Invoke();
            var result = input?.ToString(currentFormat, currentCulture);

            return result;
        }

        public TNumber? ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var currentCulture = culture.Invoke();

            if (TNumber.TryParse(input, NumberStyles.Any, currentCulture, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidNumber);
        }
    }

    private sealed class NumberConverter<TNumber>(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<TNumber, string?>
        where TNumber : INumber<TNumber>
    {
        public string Convert(TNumber input)
        {
            var currentCulture = culture.Invoke();
            var currentFormat = format.Invoke();

            return input.ToString(currentFormat, currentCulture);
        }

        public TNumber ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return TNumber.Zero;
            }

            var currentCulture = culture.Invoke();

            if (TNumber.TryParse(input, NumberStyles.Any, currentCulture, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidNumber);
        }
    }

    private sealed class DateTimeConverter(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<DateTime, string?>, IReversibleConverter<DateTime?, string?>
    {
        public string Convert(DateTime dateTime)
        {
            var currentFormat = format.Invoke();
            return dateTime.ToString(currentFormat, culture.Invoke());
        }

        public string? Convert(DateTime? input) => input?.ToString(format.Invoke(), culture.Invoke());

        public DateTime ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            var currentCulture = culture.Invoke();
            if (DateTime.TryParseExact(input, format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidDateTime);
        }

        DateTime? IReversibleConverter<DateTime?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }

    private sealed class TimeSpanConverter(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<TimeSpan, string?>, IReversibleConverter<TimeSpan?, string?>
    {
        private const string DefaultTimeSpanFormat = "c";

        public string Convert(TimeSpan timeSpan) => timeSpan.ToString(format.Invoke() ?? DefaultTimeSpanFormat, culture.Invoke());

        public string? Convert(TimeSpan? timeSpan) => timeSpan?.ToString(format.Invoke() ?? DefaultTimeSpanFormat, culture.Invoke());

        public TimeSpan ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return TimeSpan.Zero;
            }

            if (TimeSpan.TryParseExact(input, format.Invoke() ?? DefaultTimeSpanFormat, culture.Invoke(), out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidTimeSpan);
        }

        TimeSpan? IReversibleConverter<TimeSpan?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }

    //public sealed class ObjectConverter(Func<CultureInfo> culture, Func<string?> format)
    //    : IReversibleConverter<object?, string?>
    //{
    //    public string? Convert(object? input)
    //    {
    //        return input switch
    //        {
    //            null => null,
    //            Guid guid => StrictGuidStringConverter.Instance.Convert(guid),
    //            DateTime dateTime => new DateTimeConverter(() => culture(), () => format()).Convert(dateTime),
    //            TimeSpan timeSpan => new TimeSpanConverter(() => culture(), () => format()).Convert(timeSpan),
    //            int i => new NumberConverter<int>(() => culture(), () => format()).Convert(i),
    //            uint u => new NumberConverter<uint>(() => culture(), () => format()).Convert(u),
    //            long l => new NumberConverter<long>(() => culture(), () => format()).Convert(l),
    //            ulong ul => new NumberConverter<ulong>(() => culture(), () => format()).Convert(ul),
    //            float f => new NumberConverter<float>(() => culture(), () => format()).Convert(f),
    //            decimal m => new NumberConverter<decimal>(() => culture(), () => format()).Convert(m),
    //            double d => new NumberConverter<double>(() => culture(), () => format()).Convert(d),
    //            byte b => new NumberConverter<byte>(() => culture(), () => format()).Convert(b),
    //            sbyte sb => new NumberConverter<sbyte>(() => culture(), () => format()).Convert(sb),
    //            short sh => new NumberConverter<short>(() => culture(), () => format()).Convert(sh),
    //            ushort ush => new NumberConverter<ushort>(() => culture(), () => format()).Convert(ush),
    //            bool bo => BoolConverter.Instance.Convert(bo),
    //            char c => CharConverter.Instance.Convert(c),
    //            string s => s,
    //            _ => input.ToString()
    //        };
    //    }

    //    public object? ConvertBack(string? output)
    //    {
    //        //// Try numeric
    //        //if (int.TryParse(output, NumberStyles.Any, _parent.Culture, out var i)) return i;
    //        //if (double.TryParse(output, NumberStyles.Any, _parent.Culture, out var d)) return d;
    //        //// Try DateTime / TimeSpan
    //        //if (DateTime.TryParseExact(output, _parent.Format ?? _parent.Culture.DateTimeFormat.ShortDatePattern, _parent.Culture, DateTimeStyles.None, out var dt))
    //        //    return dt;
    //        //if (TimeSpan.TryParseExact(output, _parent.Format ?? "c", _parent.Culture, out var ts))
    //        //    return ts;

    //        return output;
    //    }
    //}
}
