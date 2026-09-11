// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.CodeAnalysis;

namespace MudBlazor.UnitTests.Analyzers.Internal;

#nullable enable
/// <summary>
/// The wording each MUD0002 migration hint has to carry, shared by the synthetic-operation tests and the
/// tests that compile real Razor.
/// </summary>
/// <remarks>
/// The fragments name the replacement parameter and both boolean outcomes, so a mapping that pointed at the
/// wrong replacement or dropped one direction of the inversion would fail rather than silently pass.
/// </remarks>
internal static class MigrationHintExpectations
{
    internal const string GenericMessagePrefix = "Illegal Attribute '";

    /// <summary>
    /// Text that only ever appears in a migration hint, used to assert that a diagnostic carries none.
    /// </summary>
    internal const string HintMarker = "was removed in v";

    internal static readonly string[] Link =
    [
        "'Link' was removed in v7",
        "put the URL in 'Href' instead",
        "https://github.com/MudBlazor/MudBlazor/discussions/12658"
    ];

    internal static readonly string[] DisableRipple =
    [
        "'DisableRipple' was removed in v7 and replaced by 'Ripple'",
        "inverts the value",
        "DisableRipple=\"true\" becomes Ripple=\"false\"",
        "DisableRipple=\"false\" becomes Ripple=\"true\"",
        "DisableRipple=\"@expression\" becomes Ripple=\"@(!expression)\"",
        "https://github.com/MudBlazor/MudBlazor/discussions/12658"
    ];

    internal static readonly string[] AutoGrow =
    [
        "'AutoGrow' was removed in v9 and replaced by 'Sizing'",
        "AutoGrow=\"true\" becomes Sizing=\"InputSizing.Auto\"",
        "AutoGrow=\"false\" becomes Sizing=\"InputSizing.Fixed\"",
        "AutoGrow=\"@expression\" becomes Sizing=\"@(expression ? InputSizing.Auto : InputSizing.Fixed)\"",
        "Lines, MaxLines, and masked input still need a separate look",
        "https://mudblazor.com/features/analyzers#migration-hints"
    ];

    /// <summary>
    /// Asserts that <paramref name="diagnostic"/> keeps the generic MUD0002 wording and then appends every
    /// fragment of the expected hint.
    /// </summary>
    internal static void ShouldCarryHint(this Diagnostic diagnostic, string attributeName, string tagName, string[] expectedHint)
    {
        var message = diagnostic.GetMessage();

        message.Should().StartWith($"{GenericMessagePrefix}{attributeName}' on '{tagName}' using pattern '",
            "the migration hint is appended to the existing message, not a replacement for it");

        foreach (var fragment in expectedHint)
        {
            message.Should().Contain(fragment);
        }
    }

    /// <summary>
    /// Asserts that <paramref name="diagnostic"/> is the unchanged generic warning.
    /// </summary>
    internal static void ShouldCarryNoHint(this Diagnostic diagnostic, string attributeName, string tagName)
    {
        var message = diagnostic.GetMessage();

        message.Should().StartWith($"{GenericMessagePrefix}{attributeName}' on '{tagName}' using pattern '");
        message.Should().NotContain(HintMarker);
    }

    /// <summary>
    /// Returns the single diagnostic raised for <paramref name="attributeName"/> on <paramref name="tagName"/>.
    /// </summary>
    internal static Diagnostic Single(this IEnumerable<Diagnostic> diagnostics, string attributeName, string tagName)
    {
        var prefix = $"{GenericMessagePrefix}{attributeName}' on '{tagName}' using pattern '";
        var matches = diagnostics.Where(x => x.GetMessage().StartsWith(prefix, StringComparison.Ordinal)).ToList();

        matches.Should().ContainSingle($"exactly one diagnostic is expected for '{attributeName}' on '{tagName}'");

        return matches[0];
    }

    /// <summary>
    /// Returns every diagnostic raised for <paramref name="attributeName"/> on <paramref name="tagName"/>.
    /// </summary>
    internal static IReadOnlyList<Diagnostic> All(this IEnumerable<Diagnostic> diagnostics, string attributeName, string tagName)
    {
        var prefix = $"{GenericMessagePrefix}{attributeName}' on '{tagName}' using pattern '";

        return diagnostics.Where(x => x.GetMessage().StartsWith(prefix, StringComparison.Ordinal)).ToList();
    }

    /// <summary>
    /// Splits diagnostics that share an attribute and tag name into the explained and unexplained ones, which
    /// is how a shadow type sharing a real component's display name is told apart.
    /// </summary>
    internal static IReadOnlyList<Diagnostic> WithHint(this IEnumerable<Diagnostic> diagnostics) =>
        diagnostics.Where(x => x.GetMessage().Contains(HintMarker)).ToList();

    /// <inheritdoc cref="WithHint"/>
    internal static IReadOnlyList<Diagnostic> WithoutHint(this IEnumerable<Diagnostic> diagnostics) =>
        diagnostics.Where(x => !x.GetMessage().Contains(HintMarker)).ToList();
}
#nullable restore
