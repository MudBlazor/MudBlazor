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
        // Do NOT pass Culture or Format directly: new NumberConverter<sbyte>(Culture, Format)
        // The dispatcher caches method delegates and captures the converter's field values at registration time.
        // Using () => Culture() and () => Format() ensures the converters always read the latest property values.
        // We could make Add a factory Func<IConverter> overload, but that would create instance on each conversion attempt which is less performant than current the trick.
        var builder = ReversibleTypeDispatcher.Create<T?, string?>(DispatcherRegistrationPolicy.FirstWins)
            .Add(StringConverter.Instance)
            .Add<char>(CharConverter.Instance)
            .Add<char?>(CharConverter.Instance)
            .Add<bool>(DefaultConverter.BoolConverter.Instance)
            .Add<bool?>(DefaultConverter.BoolConverter.Instance)
            .Add<Guid>(new GuidConverter(() => Culture(), () => Format()))
            .Add<Guid?>(new GuidConverter(() => Culture(), () => Format()))
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
            .Add<TimeSpan>(new DefaultConverter.TimeSpanConverter(() => Culture(), () => Format()))
            .Add<TimeSpan?>(new DefaultConverter.TimeSpanConverter(() => Culture(), () => Format()));
        // Let's not use that for now and see if we really need it
        //.Add(new ObjectConverter(() => Culture(), () => Format()))

        AddEnumConverters(builder);
        AddParsableConverters(builder);
        // Make sure this is the last converter added, so it runs only if no other converter can handle the type.
        // This ensures we don't accidentally bypass a more specific converter with FirstWins.
        builder.Add(new ToStringFallbackConverter<T>());

        _dispatcher = builder.Build();
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
