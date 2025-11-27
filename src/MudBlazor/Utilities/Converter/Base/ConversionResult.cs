using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public readonly struct ConversionResult<T>
{
    public T? Value { get; }

    public Exception? ExceptionError { get; }

    /// <summary>
    /// A localizable string key or message token.
    /// Example: "Converter_ConversionFailed"
    /// </summary>
    public string? ErrorMessageKey { get; }

    /// <summary>
    /// Optional formatting arguments (e.g. ["int", "double", "bad input"])
    /// </summary>
    public object[] ErrorMessageArgs { get; }

    [MemberNotNullWhen(false, nameof(ExceptionError))]
    public bool Success => ExceptionError is null;

    public ConversionResult(T? value) => (Value, ExceptionError, ErrorMessageKey, ErrorMessageArgs) = (value, null, null, []);

    public ConversionResult(Exception ex) => (Value, ExceptionError, ErrorMessageKey, ErrorMessageArgs) = (default, ex, null, []);

    public ConversionResult(Exception ex, string? errorKey = null, params object[] errorArgs) => (Value, ExceptionError, ErrorMessageKey, ErrorMessageArgs) = (default, ex, errorKey, errorArgs);
}
