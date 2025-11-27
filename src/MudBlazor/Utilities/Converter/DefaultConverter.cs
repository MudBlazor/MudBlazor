// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using MudBlazor.Resources;
using MudBlazor.Utilities.Converter.Base;
using MudBlazor.Utilities.Converter.Dispatcher;

namespace MudBlazor.Utilities.Converter;

#nullable enable
public sealed class DefaultConverter<T> : IReversibleConverter<T?, string?>
{
    private readonly IReversibleConverter<T?, string?> _dispatcher;

    public Func<string?>? Format { get;  }

    public Func<CultureInfo> Culture { get; }

    public DefaultConverter(Func<CultureInfo>? culture = null, Func<string?>? format = null)
    {
        Format = format;
        Culture = culture ?? (() => CultureInfo.InvariantCulture);

        _dispatcher = ReversibleTypeDispatcher.Create<T?, string?>()
            .Add(StringIdentityConverter.Instance)
            .Add<char>(CharConverter.Instance)
            .Add<char?>(CharConverter.Instance)
            .Add<bool>(BoolConverter.Instance)
            .Add<bool?>(BoolConverter.Instance)
            .Add(new NumberConverter<sbyte>(Culture, Format))
            .Add(new NullableNumberConverter<sbyte>(Culture, Format))
            .Add(new NumberConverter<byte>(Culture, Format))
            .Add(new NullableNumberConverter<byte>(Culture, Format))
            .Add(new NumberConverter<short>(Culture, Format))
            .Add(new NullableNumberConverter<short>(Culture, Format))
            .Add(new NumberConverter<ushort>(Culture, Format))
            .Add(new NullableNumberConverter<ushort>(Culture, Format))
            .Add(new NumberConverter<int>(Culture, Format))
            .Add(new NullableNumberConverter<int>(Culture, Format))
            .Add(new NumberConverter<uint>(Culture, Format))
            .Add(new NullableNumberConverter<uint>(Culture, Format))
            .Add(new NumberConverter<long>(Culture, Format))
            .Add(new NullableNumberConverter<long>(Culture, Format))
            .Add(new NumberConverter<ulong>(Culture, Format))
            .Add(new NullableNumberConverter<ulong>(Culture, Format))
            .Add(new NumberConverter<float>(Culture, Format))
            .Add(new NullableNumberConverter<float>(Culture, Format))
            .Add(new NumberConverter<double>(Culture, Format))
            .Add(new NullableNumberConverter<double>(Culture, Format))
            .Add(new NumberConverter<decimal>(Culture, Format))
            .Add(new NullableNumberConverter<decimal>(Culture, Format))
            .Add<Guid>(StrictGuidStringConverter.Instance)
            .Add<Guid?>(StrictGuidStringConverter.Instance)
            //.Add(DateTimeConverter.Create(this)!)
            //.Add(TimeSpanConverter.Create(this)!)
            //.Add(ObjectConverter.Create(this))
            .Build();
    }

    public string? Convert(T? input)
    {
        return _dispatcher.Convert(input);
    }

    public T? ConvertBack(string? output)
    {
        return _dispatcher.ConvertBack(output);
    }

    public sealed class StrictGuidStringConverter : IReversibleConverter<Guid, string?>, IReversibleConverter<Guid?, string?>
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

    public sealed class CharConverter : IReversibleConverter<char, string?>, IReversibleConverter<char?, string?>
    {
        public string? Convert(char input) => input.ToString();
        public string? Convert(char? input) => input?.ToString();

        public char ConvertBack(string? s) => string.IsNullOrEmpty(s) ? '\0' : s[0];

        char? IReversibleConverter<char?, string?>.ConvertBack(string? output)
        {
            return ConvertBack(output);
        }

        public static readonly CharConverter Instance = new();
    }

    public sealed class BoolConverter : IReversibleConverter<bool?, string?>, IReversibleConverter<bool, string?>
    {
        public string? Convert(bool input) => input ? "on" : "off";

        public string? Convert(bool? input) => input switch
        {
            true => "on",
            false => "off",
            null => null
        };

        public bool ConvertBack(string? value) =>
            value?.ToLowerInvariant() switch
            {
                "true" or "1" or "on" => true,
                _ => false
            };

        bool? IReversibleConverter<bool?, string?>.ConvertBack(string? value) =>
            value?.ToLowerInvariant() switch
            {
                "true" or "1" or "on" => true,
                "false" or "0" or "off" => false,
                _ => null
            };

        public static readonly BoolConverter Instance = new();
    }

    public sealed class StringIdentityConverter : IReversibleConverter<string?, string?>
    {
        public string? Convert(string? input) => input;

        public string? ConvertBack(string? input) => input;

        public static readonly StringIdentityConverter Instance = new();
    }

    public sealed class NullableNumberConverter<TNumber> : IReversibleConverter<TNumber?, string?> where TNumber : struct, INumber<TNumber>
    {
        public Func<string?>? Format { get; }

        public Func<CultureInfo> Culture { get; }

        public NullableNumberConverter(Func<CultureInfo> culture, Func<string?>? format)
        {
            Culture = culture;
            Format = format;
            
        }

        public string? Convert(TNumber? input)
        {
            var culture = Culture.Invoke();

            return input?.ToString(Format?.Invoke(), culture);
        }

        public TNumber? ConvertBack(string? output)
        {
            var culture = Culture.Invoke();

            if (TNumber.TryParse(output, NumberStyles.Any, culture, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidNumber);
        }
    }

    public sealed class NumberConverter<TNumber> : IReversibleConverter<TNumber, string?> where TNumber : INumber<TNumber>
    {
        public Func<string?>? Format { get; }

        public Func<CultureInfo> Culture { get; }

        public NumberConverter(Func<CultureInfo> culture, Func<string?>? format)
        {
            Culture = culture;
            Format = format;

        }

        public string? Convert(TNumber input)
        {
            var culture = Culture.Invoke();

            return input.ToString(Format?.Invoke(), culture);
        }

        public TNumber ConvertBack(string? output)
        {
            var culture = Culture.Invoke();

            if (TNumber.TryParse(output, NumberStyles.Any, culture, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidNumber);
        }
    }


    //public sealed class DateTimeConverter : IReversibleConverter<DateTime, string>
    //{
    //    private readonly DefaultConverter<T> _parent;
    //    private DateTimeConverter(DefaultConverter<T> parent) => _parent = parent;

    //    public static DateTimeConverter Create(DefaultConverter<T> parent) => new(parent);

    //    public string Convert(DateTime dt) =>
    //        dt.ToString(_parent.Format ?? _parent.Culture.DateTimeFormat.ShortDatePattern, _parent.Culture);

    //    public DateTime ConvertBack(string s) =>
    //        DateTime.ParseExact(s, _parent.Format ?? _parent.Culture.DateTimeFormat.ShortDatePattern, _parent.Culture);
    //}

    //public sealed class TimeSpanConverter : IReversibleConverter<TimeSpan, string>
    //{
    //    private readonly DefaultConverter<T> _parent;
    //    private TimeSpanConverter(DefaultConverter<T> parent) => _parent = parent;

    //    public static TimeSpanConverter Create(DefaultConverter<T> parent) => new(parent);

    //    public string Convert(TimeSpan ts) =>
    //        ts.ToString(_parent.Format ?? "c", _parent.Culture);

    //    public TimeSpan ConvertBack(string s) =>
    //        TimeSpan.ParseExact(s, _parent.Format ?? "c", _parent.Culture);
    //}

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
