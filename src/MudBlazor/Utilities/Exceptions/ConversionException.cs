// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Exceptions;

/// <summary>
/// Exception type used to represent failures during value conversion performed by converters.
/// </summary>
/// <remarks>
/// <see cref="ConversionException"/> carries a localization-friendly error key (<see cref="ErrorMessageKey"/>)
/// and optional formatting arguments (<see cref="ErrorMessageArgs"/>) in addition to the inner <see cref="Exception"/>.
/// Callers (for example UI layers) can use <see cref="ErrorMessageKey"/> together with <see cref="ErrorMessageArgs"/>
/// to present a localized, formatted message to the user. The <see cref="Exception.Message"/> property
/// contains the <c>key</c> supplied to the constructor for convenience.
/// </remarks>
public class ConversionException : Exception
{
    /// <summary>
    /// A localizable string key or message token identifying the conversion error.
    /// </summary>
    public string ErrorMessageKey { get; }

    /// <summary>
    /// Optional formatting arguments for <see cref="ErrorMessageKey"/>.
    /// </summary>
    public object[] ErrorMessageArgs { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ConversionException"/>.
    /// </summary>
    /// <param name="key">A localization key or message token describing the error. This value cannot be <c>null</c>.</param>
    /// <param name="arguments">Optional formatting arguments for the error message.</param>
    /// <param name="inner">An optional inner exception describing the underlying cause.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    public ConversionException(string key, object[]? arguments = null, Exception? inner = null)
        : base(key, inner)
    {
        ErrorMessageKey = key ?? throw new ArgumentNullException(nameof(key));
        ErrorMessageArgs = arguments ?? [];
    }
}
