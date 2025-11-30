// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using MudBlazor.Resources;
using MudBlazor.Utilities.Converter.Base;
using MudBlazor.Utilities.Converter.Dispatcher;
using static MudBlazor.Utilities.Converter.DefaultConverter;

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
            .Add(StringConverter.Instance)
            .Add<char>(CharConverter.Instance)
            .Add<char?>(CharConverter.Instance)
            .Add<bool>(BoolConverter.Instance)
            .Add<bool?>(BoolConverter.Instance)
            .Add<Guid>(GuidConverter.Instance)
            .Add<Guid?>(GuidConverter.Instance)
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
            .Add<BigInteger>(new BigIntegerConverter(() => Culture(), () => Format()))
            .Add<BigInteger?>(new BigIntegerConverter(() => Culture(), () => Format()))
            .Add<DateTime>(new DateTimeConverter(() => Culture(), () => Format()))
            .Add<DateTime?>(new DateTimeConverter(() => Culture(), () => Format()))
            .Add<DateTimeOffset>(new DateTimeOffsetConverter(() => Culture(), () => Format()))
            .Add<DateTimeOffset?>(new DateTimeOffsetConverter(() => Culture(), () => Format()))
            .Add<DateOnly>(new DateOnlyConverter(() => Culture(), () => Format()))
            .Add<DateOnly?>(new DateOnlyConverter(() => Culture(), () => Format()))
            .Add<TimeOnly>(new TimeOnlyConverter(() => Culture(), () => Format()))
            .Add<TimeOnly?>(new TimeOnlyConverter(() => Culture(), () => Format()))
            .Add<TimeSpan>(new TimeSpanConverter(() => Culture(), () => Format()))
            .Add<TimeSpan?>(new TimeSpanConverter(() => Culture(), () => Format()))
            //.Add(new ObjectConverter(() => Culture(), () => Format()))
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
    //        return output;
    //    }
    //}
}
