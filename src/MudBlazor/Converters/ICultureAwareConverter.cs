// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor;

/// <summary>
/// Provides culture and format providers for converters that need culture-aware formatting or parsing.
/// </summary>
/// <remarks>
/// This interface is intended primarily for converters that are used by components via
/// <see cref="MudFormComponent{T, U}.Converter"/>. When a converter implementing this interface is supplied to
/// a Mud form component, the component will automatically provide the <see cref="Culture"/> and
/// <see cref="Format"/> delegates at runtime.
/// 
/// If you plan to use a converter outside of Blazor components (for example in plain services or library code),
/// prefer not to implement this interface. Instead, provide culture/format information explicitly to the converter
/// (for example via constructor parameters or required properties)
/// 
/// Both members are delegates so callers can supply dynamic providers (for example a component-local culture or a runtime
/// format string). Implementations should invoke these delegates at conversion time to obtain the current <see cref="CultureInfo"/>
/// and format string. When <see cref="Format"/> returns <c>null</c>, the implementation should use the default formatting behavior.
/// </remarks>
public interface ICultureAwareConverter
{
    /// <summary>
    /// A function that returns the <see cref="CultureInfo"/> to use when converting or formatting values.
    /// </summary>
    /// <remarks>
    /// This delegate is invoked at conversion time. When the converter is used as a component converter (via
    /// <see cref="MudFormComponent{T, U}.Converter"/>), the host component will supply this delegate automatically.
    /// Implementations should not cache the returned value permanently unless they have a reason to ignore runtime changes.
    /// </remarks>
    Func<CultureInfo> Culture { get; set; }

    /// <summary>
    /// A function that provides an optional format string to use when formatting values.
    /// </summary>
    /// <remarks>
    /// The delegate is invoked at conversion/formatting time and may return <c>null</c> to indicate that the default
    /// formatting rules should be applied. Typical uses include custom date/time or numeric format strings supplied by the consumer.
    /// When the converter is used as a component converter (via <see cref="MudFormComponent{T, U}.Converter"/>), the host component will
    /// supply this delegate automatically.
    /// </remarks>
    Func<string?> Format { get; set; }
}
