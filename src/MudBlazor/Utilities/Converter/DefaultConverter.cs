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
            //.Add(ObjectConverter.Create(this))
            .Build();
    }

    public string? Convert(T? input)
    {
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
            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidGUID);
        }

        public string Convert(Guid value) => value.ToString();

        public string? Convert(Guid? value) => value is null ? null : value.ToString();

        Guid? IReversibleConverter<Guid?, string?>.ConvertBack(string? output) => ConvertBack(output);

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
        public string Convert(bool input) => input ? "on" : "off";

        public string? Convert(bool? input) => input switch
        {
            true => "on",
            false => "off",
            null => null
        };

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
            var culture1 = culture.Invoke();

            return input?.ToString(format?.Invoke(), culture1);
        }

        public TNumber? ConvertBack(string? input)
        {
            var culture1 = culture.Invoke();

            if (TNumber.TryParse(input, NumberStyles.Any, culture1, out var result))
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
            return input.ToString(format.Invoke(), culture.Invoke());
        }

        public TNumber ConvertBack(string? input)
        {
            if (TNumber.TryParse(input, NumberStyles.Any, culture.Invoke(), out var result))
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
            var currentCulture = culture.Invoke();
            if (DateTime.TryParseExact(input, format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidDateTime);
        }

        DateTime? IReversibleConverter<DateTime?, string?>.ConvertBack(string? input)
        {
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
            if (TimeSpan.TryParseExact(input, format.Invoke() ?? DefaultTimeSpanFormat, culture.Invoke(), out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidTimeSpan);
        }

        TimeSpan? IReversibleConverter<TimeSpan?, string?>.ConvertBack(string? input)
        {
            return ConvertBack(input);
        }
    }

    //public sealed class ObjectConverter : IReversibleConverter<object?, string?>
    //{
    //    public Func<string?>? Format { get; }

    //    public Func<CultureInfo> Culture { get; }

    //    public ObjectConverter(Func<CultureInfo> culture, Func<string?>? format)
    //    {
    //        Culture = culture;
    //        Format = format;
    //    }

    //    public string? Convert(object? input)
    //    {
    //        return input switch
    //        {
    //            null => null,
    //            DateTime dt => DateTimeConverter.Create(_parent).Convert(dt),
    //            TimeSpan ts => TimeSpanConverter.Create(_parent).Convert(ts),
    //            int i => new NumberConverter<int>(Culture, Format).Convert(i),
    //            double d => new NumberConverter<double>(Culture, Format).Convert(d),
    //            string s => s,
    //            _ => input.ToString()
    //        };
    //    }

    //    public object? ConvertBack(string? output)
    //    {
    //        if (output == null) return null;

    //        // Try numeric
    //        if (int.TryParse(output, NumberStyles.Any, _parent.Culture, out var i)) return i;
    //        if (double.TryParse(output, NumberStyles.Any, _parent.Culture, out var d)) return d;
    //        // Try DateTime / TimeSpan
    //        if (DateTime.TryParseExact(output, _parent.Format ?? _parent.Culture.DateTimeFormat.ShortDatePattern, _parent.Culture, DateTimeStyles.None, out var dt))
    //            return dt;
    //        if (TimeSpan.TryParseExact(output, _parent.Format ?? "c", _parent.Culture, out var ts))
    //            return ts;

    //        return output;
    //    }
    //}
}
