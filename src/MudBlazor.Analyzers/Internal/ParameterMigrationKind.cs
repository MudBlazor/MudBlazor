// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers.Internal;

/// <summary>
/// How a removed parameter's value carries over to its replacement, which decides the wording of the hint.
/// </summary>
internal enum ParameterMigrationKind
{
    /// <summary>
    /// The replacement takes the same value under a new name.
    /// </summary>
    Rename,

    /// <summary>
    /// The replacement is a boolean that takes the negated value.
    /// </summary>
    Inversion,

    /// <summary>
    /// An inversion whose replacement also changed its default, so leaving it unset no longer behaves like leaving the old parameter unset.
    /// </summary>
    InversionWithNewDefault,

    /// <summary>
    /// An inversion whose replacement is a nullable boolean that falls back to a parent component's setting when unset.
    /// </summary>
    InheritedInversion,

    /// <summary>
    /// One half of a two-way binding pair that was renamed together, so the <c>@bind-</c> spelling changes too.
    /// </summary>
    Binding,

    /// <summary>
    /// A starting value replaced by a parameter that holds the live state.
    /// </summary>
    InitialState,

    /// <summary>
    /// The value's type, unit, or signature changes, so the hint spells out the conversion.
    /// </summary>
    Manual,
}
