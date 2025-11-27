using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public readonly struct ConversionResult<T>
{
    public T? Value { get; }

    public Exception? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success => Error is null;

    public ConversionResult(T? value) => (Value, Error) = (value, null);

    public ConversionResult(Exception ex) => (Value, Error) = (default, ex);
}
