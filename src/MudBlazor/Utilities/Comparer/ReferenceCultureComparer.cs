// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.CompilerServices;

namespace MudBlazor.Utilities.Comparer;

/// <summary>
/// Provides a reference-based equality comparison for <see cref="CultureInfo"/> objects.
/// </summary>
/// <remarks>
/// This comparer treats two <see cref="CultureInfo"/> instances as equal only if they are the
/// exact same object reference. It ignores all culture properties, including culture name,
/// formatting settings, and user customizations.
///
/// This is useful when using <see cref="CultureInfo"/> as a dictionary key or in a set,
/// and you want distinct <see cref="CultureInfo"/> instances—even those representing the
/// same culture—to be treated as different.
///
/// The hash code is generated using <see cref="RuntimeHelpers.GetHashCode(object)"/>, ensuring
/// identity-based hashing rather than content-based hashing.
/// </remarks>
internal sealed class ReferenceCultureComparer : IEqualityComparer<CultureInfo>
{
    /// <inheritdoc />
    public bool Equals(CultureInfo? x, CultureInfo? y)
        => ReferenceEquals(x, y);

    /// <inheritdoc />
    public int GetHashCode(CultureInfo obj)
        => RuntimeHelpers.GetHashCode(obj);

    /// <summary>
    /// Gets the default instance of <see cref="ReferenceCultureComparer"/>.
    /// </summary>
    public static readonly ReferenceCultureComparer Default = new();
}
