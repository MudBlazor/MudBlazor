using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
/// <summary>
/// Represents the result of a conversion attempt performed by a converter.
/// </summary>
/// <typeparam name="T">The target type produced by the conversion.</typeparam>
/// <remarks>
/// Instances of <see cref="ConversionResult{T}"/> encode either a successful conversion (the <see cref="Value"/> is set
/// and <see cref="Success"/> is <c>true</c>) or a failed conversion (an <see cref="ExceptionError"/> and optional
/// error metadata are provided and <see cref="Success"/> is <c>false</c>).
/// The struct is immutable and intended to be a lightweight carrier of both value and error information for converter APIs.
/// </remarks>
public readonly struct ConversionResult<T>
{
    /// <summary>
    /// The converted value when the conversion succeeded; otherwise <c>default</c>.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// When the conversion failed, this contains the exception that describes the failure; otherwise <c>null</c>.
    /// </summary>
    public Exception? ExceptionError { get; }

    /// <summary>
    /// A localizable string key or message token identifying the error.
    /// Example: "Converter_ConversionFailed".
    /// This is intended for use by UI layers that want to map converter failures to localized messages.
    /// </summary>
    public string? ErrorMessageKey { get; }

    /// <summary>
    /// Optional formatting arguments for <see cref="ErrorMessageKey"/> (for example: <c>["int", "double", "bad input"]</c>).
    /// If no arguments are provided this will be an empty array.
    /// </summary>
    public object[] ErrorMessageArgs { get; }

    /// <summary>
    /// Indicates whether the conversion succeeded.
    /// Returns <c>true</c> when <see cref="ExceptionError"/> is <c>null</c>, otherwise <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(ExceptionError))]
    public bool Success => ExceptionError is null;

    /// <summary>
    /// Creates a successful conversion result containing the provided <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The successfully converted value.</param>
    public ConversionResult(T? value) => (Value, ExceptionError, ErrorMessageKey, ErrorMessageArgs) = (value, null, null, []);

    /// <summary>
    /// Creates a failed conversion result that contains the provided exception.
    /// </summary>
    /// <param name="ex">The exception describing the conversion failure.</param>
    public ConversionResult(Exception ex) => (Value, ExceptionError, ErrorMessageKey, ErrorMessageArgs) = (default, ex, null, []);

    /// <summary>
    /// Creates a failed conversion result with the provided exception and optional localization metadata.
    /// </summary>
    /// <param name="ex">The exception describing the conversion failure.</param>
    /// <param name="errorKey">An optional localizable error message key (or token).</param>
    /// <param name="errorArgs">Optional format arguments for the error message key.</param>
    public ConversionResult(Exception ex, string errorKey, params object[] errorArgs) => (Value, ExceptionError, ErrorMessageKey, ErrorMessageArgs) = (default, ex, errorKey, errorArgs);
}
