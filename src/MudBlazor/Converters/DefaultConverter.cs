// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using MudBlazor.Utilities.Converter.Dispatcher;
using static MudBlazor.DefaultConverter;

namespace MudBlazor;

/// <summary>
/// Default reversible converter that converts between <typeparamref name="T"/> and <see cref="string"/>.
/// </summary>
/// <typeparam name="T">The target CLR type the converter handles.</typeparam>
/// <remarks>
/// This converter composes many built-in converters (numbers, dates, guid, boolean, char, BigInteger, etc.).
/// It implements <see cref="ICultureAwareConverter"/> so that when used as
/// a component converter (for example via a Mud form component) the host can automatically supply the
/// <see cref="Culture"/> and <see cref="Format"/> delegates.
/// </remarks>
public sealed class DefaultConverter<T> : IReversibleConverter<T?, string?>, ICultureAwareConverter
{
    private readonly IReversibleConverter<T?, string?> _dispatcher;

    /// <inheritdoc />
    public Func<string?> Format { get; set; } = () => null;

    /// <inheritdoc />
    public Func<CultureInfo> Culture { get; set; } = () => CultureInfo.InvariantCulture;

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultConverter{T}"/> and registers the built-in converters with a reversible dispatcher.
    /// </summary>
    public DefaultConverter()
    {
        var builder = ReversibleTypeDispatcher.Create<T?, string?>(DispatcherRegistrationPolicy.FirstWins);

        AddBuiltInConverter(builder);
        AddEnumConverters(builder);
        AddParsableConverters(builder);
        // Make sure this is the last converter added, so it runs only if no other converter can handle the type.
        // This ensures we don't accidentally bypass a more specific converter with FirstWins.
        builder.Add(new ToStringFallbackConverter<T>());

        _dispatcher = builder.Build();
    }

    /// <summary>
    /// Registers the built-in converter for <typeparamref name="T"/>, if there is one.
    /// </summary>
    /// <remarks>
    /// The dispatcher resolves a single handler for <c>typeof(T)</c> when it is built and drops every other registration,
    /// so registering the whole table would build about forty converters and their culture and format closures per instance to keep one.
    /// Do NOT pass Culture or Format directly (<c>new NumberConverter&lt;int&gt;(Culture, Format)</c>): the dispatcher captures the
    /// field values at registration time, while <c>() =&gt; Culture()</c> reads the latest property value on every conversion.
    /// </remarks>
    private void AddBuiltInConverter(IReversibleDispatcherBuilder<T?, string?> builder)
    {
        var targetType = typeof(T);

        if (targetType == typeof(string))
        {
            builder.Add(StringConverter.Instance);
        }
        else if (targetType == typeof(char))
        {
            builder.Add<char>(CharConverter.Instance);
        }
        else if (targetType == typeof(char?))
        {
            builder.Add<char?>(CharConverter.Instance);
        }
        else if (targetType == typeof(bool))
        {
            builder.Add<bool>(DefaultConverter.BoolConverter.Instance);
        }
        else if (targetType == typeof(bool?))
        {
            builder.Add<bool?>(DefaultConverter.BoolConverter.Instance);
        }
        else if (targetType == typeof(Guid))
        {
            builder.Add<Guid>(new GuidConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(Guid?))
        {
            builder.Add<Guid?>(new GuidConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(sbyte))
        {
            builder.Add(new NumberConverter<sbyte>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(sbyte?))
        {
            builder.Add(new NullableNumberConverter<sbyte>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(byte))
        {
            builder.Add(new NumberConverter<byte>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(byte?))
        {
            builder.Add(new NullableNumberConverter<byte>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(short))
        {
            builder.Add(new NumberConverter<short>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(short?))
        {
            builder.Add(new NullableNumberConverter<short>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(ushort))
        {
            builder.Add(new NumberConverter<ushort>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(ushort?))
        {
            builder.Add(new NullableNumberConverter<ushort>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(int))
        {
            builder.Add(new NumberConverter<int>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(int?))
        {
            builder.Add(new NullableNumberConverter<int>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(uint))
        {
            builder.Add(new NumberConverter<uint>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(uint?))
        {
            builder.Add(new NullableNumberConverter<uint>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(long))
        {
            builder.Add(new NumberConverter<long>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(long?))
        {
            builder.Add(new NullableNumberConverter<long>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(ulong))
        {
            builder.Add(new NumberConverter<ulong>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(ulong?))
        {
            builder.Add(new NullableNumberConverter<ulong>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(float))
        {
            builder.Add(new NumberConverter<float>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(float?))
        {
            builder.Add(new NullableNumberConverter<float>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(double))
        {
            builder.Add(new NumberConverter<double>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(double?))
        {
            builder.Add(new NullableNumberConverter<double>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(decimal))
        {
            builder.Add(new NumberConverter<decimal>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(decimal?))
        {
            builder.Add(new NullableNumberConverter<decimal>(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(BigInteger))
        {
            builder.Add<BigInteger>(new BigIntegerConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(BigInteger?))
        {
            builder.Add<BigInteger?>(new BigIntegerConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(DateTime))
        {
            builder.Add<DateTime>(new DateTimeConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(DateTime?))
        {
            builder.Add<DateTime?>(new DateTimeConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(DateTimeOffset))
        {
            builder.Add<DateTimeOffset>(new DateTimeOffsetConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(DateTimeOffset?))
        {
            builder.Add<DateTimeOffset?>(new DateTimeOffsetConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(DateOnly))
        {
            builder.Add<DateOnly>(new DateOnlyConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(DateOnly?))
        {
            builder.Add<DateOnly?>(new DateOnlyConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(TimeOnly))
        {
            builder.Add<TimeOnly>(new TimeOnlyConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(TimeOnly?))
        {
            builder.Add<TimeOnly?>(new TimeOnlyConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(TimeSpan))
        {
            builder.Add<TimeSpan>(new DefaultConverter.TimeSpanConverter(() => Culture(), () => Format()));
        }
        else if (targetType == typeof(TimeSpan?))
        {
            builder.Add<TimeSpan?>(new DefaultConverter.TimeSpanConverter(() => Culture(), () => Format()));
        }
    }

    /// <inheritdoc />
    public string? Convert(T? input) => _dispatcher.Convert(input);

    /// <inheritdoc />
    public T? ConvertBack(string? input) => _dispatcher.ConvertBack(input);

    // TODO: Consider adding DynamicallyAccessedMembers attribute in future as DefaultConverter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.Interfaces)]T>, affects MudBaseInput, MudBaseDatePicker, MudFileUpload, MudColorPicker + 3rd party libraries.
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2090", // Missing DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
        Justification = "Not 200% safe without annotation, but considering if type is supplied by the user, it should work. Suppressed for backward compatibility.")]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2091", // Missing DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
        Justification = "Not 200% safe without annotation, but considering if type is supplied by the user, it should work. Suppressed for backward compatibility.")]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2087", // Missing DynamicallyAccessedMemberTypes.Interfaces
        Justification = "Not 200% safe without annotation, but considering if type is supplied by the user, it should work. Suppressed for backward compatibility.")]
    private void AddParsableConverters(IReversibleDispatcherBuilder<T?, string?> builder)
    {
        var targetType = typeof(T);

        var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlyingType is not null && ImplementsIParsable(nullableUnderlyingType))
        {
            var nullableConverterType = typeof(NullableParsableConverter<>).MakeGenericType(nullableUnderlyingType);
            var nullableConverter = Activator.CreateInstance(nullableConverterType, (Func<CultureInfo>)(() => Culture()), (Func<string?>)(() => Format()));
            if (nullableConverter is not null)
            {
                builder.AddDynamic(targetType, nullableConverter);
            }
        }

        if (ImplementsIParsable(targetType))
        {
            var converterType = typeof(ParsableConverter<>).MakeGenericType(targetType);
            var converter = Activator.CreateInstance(converterType, (Func<CultureInfo>)(() => Culture()), (Func<string?>)(() => Format()));
            if (converter is not null)
            {
                builder.AddDynamic(targetType, converter);
            }
        }
    }

    private static bool ImplementsIParsable([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type type)
    {
        return type
            .GetInterfaces()
            .Any(x => x.IsGenericType
                      && x.GetGenericTypeDefinition() == typeof(IParsable<>)
                      && x.GenericTypeArguments[0] == type);
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2090", // Missing DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
        Justification = "Not 200% safe without annotation, but considering if type is supplied by the user, it should work. Suppressed for backward compatibility.")]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2091", // Missing DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
        Justification = "Not 200% safe without annotation, but considering if type is supplied by the user, it should work. Suppressed for backward compatibility.")]
    private static void AddEnumConverters(IReversibleDispatcherBuilder<T?, string?> builder)
    {
        var targetType = typeof(T);

        var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
        if (nullableUnderlyingType?.IsEnum is true)
        {
            var nullableEnumConverterType = typeof(NullableEnumConverter<>).MakeGenericType(nullableUnderlyingType);
            var nullableEnumConverter = Activator.CreateInstance(nullableEnumConverterType);
            if (nullableEnumConverter is not null)
            {
                builder.AddDynamic(targetType, nullableEnumConverter);
            }

            return;
        }

        if (targetType.IsEnum)
        {
            var enumConverterType = typeof(EnumConverter<>).MakeGenericType(targetType);
            var enumConverter = Activator.CreateInstance(enumConverterType);
            if (enumConverter is not null)
            {
                builder.AddDynamic(targetType, enumConverter);
            }
        }
    }
}
