// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Centralizes the date-type dispatch shared by <see cref="MudBaseDatePicker{T}"/> and <see cref="DateRange{T}"/>.
/// Exposes the underlying CLR type, a support check, and the bidirectional conversions between
/// <typeparamref name="T"/> and <see cref="DateTime"/>.
/// </summary>
/// <typeparam name="T">The date type to support. Permitted: <see cref="DateTime"/>, <see cref="DateOnly"/>,
/// <see cref="DateTimeOffset"/>, and their nullable variants.</typeparam>
internal static class PickerTypeSupport<T>
{
    /// <summary>
    /// The underlying value type — <typeparamref name="T"/> with the <see cref="Nullable{T}"/> wrapper stripped.
    /// </summary>
    public static readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> when <typeparamref name="T"/> is not supported.
    /// The thrown message mentions analyzer MUD0003 so users searching the runtime error find the build-time check.
    /// </summary>
    public static void EnsureSupported()
    {
        if (UnderlyingType != typeof(DateTime) &&
            UnderlyingType != typeof(DateOnly) &&
            UnderlyingType != typeof(DateTimeOffset))
        {
            throw Unsupported();
        }
    }

    /// <summary>
    /// Convert a <typeparamref name="T"/> value into a <see cref="DateTime"/> for internal calendar math.
    /// </summary>
    public static DateTime? ToDateTime(T? value) => value switch
    {
        null => null,
        DateTime dt => dt,
        DateOnly d => d.ToDateTime(TimeOnly.MinValue),
        DateTimeOffset dto => dto.DateTime,
        _ => throw Unsupported()
    };

    /// <summary>
    /// Convert a <see cref="DateTime"/> back to <typeparamref name="T"/>. The supplied
    /// <paramref name="offset"/> is applied when the underlying type is <see cref="DateTimeOffset"/>.
    /// </summary>
    public static T? FromDateTime(DateTime? value, TimeSpan offset)
    {
        if (value is null) return default;
        if (UnderlyingType == typeof(DateTime)) return (T)(object)value.Value;
        if (UnderlyingType == typeof(DateOnly)) return (T)(object)DateOnly.FromDateTime(value.Value);
        if (UnderlyingType == typeof(DateTimeOffset)) return (T)(object)new DateTimeOffset(value.Value, offset);
        throw Unsupported();
    }

    private static InvalidOperationException Unsupported() => new(
        $"Unsupported date type T = {typeof(T)}. Use DateTime, DateOnly, or DateTimeOffset (or their nullable variants). See analyzer MUD0003.");
}
