// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;
using ComponentDescriptor = MudBlazorAnalyzer::MudBlazor.Analyzers.Internal.ComponentDescriptor;
using ParameterMigration = MudBlazorAnalyzer::MudBlazor.Analyzers.Internal.ParameterMigration;
using ParameterMigrationKind = MudBlazorAnalyzer::MudBlazor.Analyzers.Internal.ParameterMigrationKind;
using ParameterMigrationResolver = MudBlazorAnalyzer::MudBlazor.Analyzers.Internal.ParameterMigrationResolver;

#nullable enable
/// <summary>
/// Checks every migration hint in the analyzer's catalog against the real MudBlazor assembly, so an API removal, a reintroduced name, or a hint pointing somewhere that no longer exists fails here instead of shipping wrong advice.
/// </summary>
[TestFixture]
public class ParameterMigrationCatalogTests
{
    private static readonly Compilation _compilation = AnalyzerCompilationFactory.CreateCompilation(string.Empty);

    private static INamedTypeSymbol MudComponentBase => _compilation.GetTypeByMetadataName("MudBlazor.MudComponentBase")!;

    private static INamedTypeSymbol ParameterAttribute => _compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.ParameterAttribute")!;

    /// <summary>
    /// Every entry names a component that exists and that MUD0002 actually inspects.
    /// </summary>
    [Test]
    public void EveryComponentResolvesAndIsInspectedByMud0002()
    {
        var failures = ParameterMigration.All
            .Where(x => Resolve(x) is not { } component || !InheritsFrom(component, MudComponentBase))
            .Select(Describe);

        failures.Should().BeEmpty("a hint on a missing or non-MudComponentBase type can never be shown");
    }

    /// <summary>
    /// The removed name is no longer a parameter on the component or on anything deriving from it.
    /// </summary>
    [Test]
    public void EveryRemovedParameterIsGoneFromItsComponentAndDescendants()
    {
        var failures = ParameterMigration.All
            .SelectMany(entry => Reached(entry).Select(component => (entry, component)))
            .Where(x => FindParameter(x.component, x.entry.ParameterName, StringComparison.OrdinalIgnoreCase) is not null)
            .Select(x => $"{Describe(x.entry)} is still a parameter on {x.component.Name}");

        failures.Should().BeEmpty("a real parameter always wins over a hint, so the entry would be dead or wrong");
    }

    /// <summary>
    /// The replacement is a current, non-obsolete parameter on the component and on every type the hint reaches.
    /// </summary>
    [Test]
    public void EveryReplacementIsACurrentParameterWhereverTheHintApplies()
    {
        var failures = ParameterMigration.All
            .SelectMany(entry => Reached(entry).Select(component => (entry, component)))
            .Select(x => (x.entry, x.component, replacement: FindParameter(x.component, x.entry.ReplacementParameterName, StringComparison.Ordinal)))
            .Where(x => x.replacement is null || x.replacement.GetAttributes().Any(a => a.AttributeClass?.Name == nameof(ObsoleteAttribute)))
            .Select(x => $"{Describe(x.entry)} points at a missing or obsolete '{x.entry.ReplacementParameterName}' on {x.component.Name}");

        failures.Should().BeEmpty("the hint must send the reader to a parameter they can use today");
    }

    /// <summary>
    /// Inversion hints only ever map onto booleans, and the ones that tell the reader to leave the value unset map onto a nullable boolean that can inherit.
    /// </summary>
    [Test]
    public void InversionsMapOntoBooleans()
    {
        var failures = ParameterMigration.All
            .Where(x => x.Kind is ParameterMigrationKind.Inversion or ParameterMigrationKind.InversionWithNewDefault or ParameterMigrationKind.InheritedInversion)
            .SelectMany(entry => Reached(entry).Select(component => (entry, type: FindParameter(component, entry.ReplacementParameterName, StringComparison.Ordinal)?.Type)))
            .Where(x => x.entry.Kind is ParameterMigrationKind.InheritedInversion ? !IsNullableBoolean(x.type) : x.type?.SpecialType is not SpecialType.System_Boolean)
            .Select(x => $"{Describe(x.entry)} maps onto {x.type?.ToDisplayString() ?? "nothing"}");

        failures.Should().BeEmpty();
    }

    /// <summary>
    /// Hints that describe the new default say what a freshly constructed component really does.
    /// </summary>
    [Test]
    public void DefaultsMatchWhatTheHintsSay()
    {
        var failures = new List<string>();

        foreach (var entry in ParameterMigration.All)
        {
            var expected = entry.Kind switch
            {
                ParameterMigrationKind.InversionWithNewDefault => (object?)false,
                ParameterMigrationKind.InheritedInversion => null,
                _ => "unchecked",
            };

            if (expected is "unchecked")
                continue;

            var actual = GetDefault(entry.ComponentMetadataName, entry.ReplacementParameterName);
            if (!Equals(actual, expected))
                failures.Add($"{Describe(entry)} defaults to {actual ?? "null"}, not {expected ?? "null"}");
        }

        failures.Should().BeEmpty("'needs Ripple=\"true\" to keep its old behavior' and 'leave it unset to follow the parent' are claims about the default");
    }

    /// <summary>
    /// Two-way binding hints come in value and callback pairs whose replacements form a real binding today.
    /// </summary>
    [Test]
    public void BindingHintsComeInPairsThatStillBind()
    {
        var failures = new List<string>();
        var bindings = ParameterMigration.All.Where(x => x.Kind is ParameterMigrationKind.Binding).ToList();

        foreach (var value in bindings.Where(x => !x.ParameterName.EndsWith("Changed", StringComparison.Ordinal)))
        {
            var callback = bindings.SingleOrDefault(x => x.ComponentMetadataName == value.ComponentMetadataName && x.ParameterName == $"{value.ParameterName}Changed");
            if (callback is null || callback.ReplacementParameterName != $"{value.ReplacementParameterName}Changed")
            {
                failures.Add($"{Describe(value)} has no matching callback entry");
                continue;
            }

            var component = Resolve(value)!;
            var valueType = FindParameter(component, value.ReplacementParameterName, StringComparison.Ordinal)?.Type;
            var callbackType = FindParameter(component, callback.ReplacementParameterName, StringComparison.Ordinal)?.Type as INamedTypeSymbol;

            if (valueType is null || callbackType is not { Name: "EventCallback", TypeArguments.Length: 1 } || !SymbolEqualityComparer.Default.Equals(callbackType.TypeArguments[0], valueType))
                failures.Add($"{Describe(value)} does not pair with an EventCallback of the same type");
        }

        failures.AddRange(bindings
            .Where(x => x.ParameterName.EndsWith("Changed", StringComparison.Ordinal))
            .Where(x => !bindings.Any(v => v.ComponentMetadataName == x.ComponentMetadataName && $"{v.ParameterName}Changed" == x.ParameterName))
            .Select(x => $"{Describe(x)} has no matching value entry"));

        failures.Should().BeEmpty("the hint tells the reader to rewrite @bind, which only works when both halves exist");
    }

    /// <summary>
    /// Initial-state hints point at a boolean the component also raises a change for, so the @bind form the hint mentions exists.
    /// </summary>
    [Test]
    public void InitialStateHintsPointAtBindableState()
    {
        var failures = ParameterMigration.All
            .Where(x => x.Kind is ParameterMigrationKind.InitialState)
            .Where(x => Resolve(x) is not { } component
                || FindParameter(component, x.ReplacementParameterName, StringComparison.Ordinal)?.Type.SpecialType is not SpecialType.System_Boolean
                || FindParameter(component, $"{x.ReplacementParameterName}Changed", StringComparison.Ordinal) is null)
            .Select(Describe);

        failures.Should().BeEmpty();
    }

    /// <summary>
    /// Every hint resolves from resources into text that names the removed parameter and its replacement and ends with a link.
    /// </summary>
    [Test]
    public void EveryHintResolvesToTextNamingBothParameters()
    {
        var failures = new List<string>();

        foreach (var entry in ParameterMigration.All)
        {
            var text = entry.Hint.ToString(CultureInfo.InvariantCulture);

            if (!text.StartsWith($" '{entry.ParameterName}' was removed in v", StringComparison.Ordinal)
                || !text.Contains($"'{entry.ReplacementParameterName}'", StringComparison.Ordinal)
                || !Regex.IsMatch(text, @"See https://\S+$")
                || Regex.IsMatch(text, @"\{\d+\}"))
            {
                failures.Add($"{Describe(entry)}: \"{text}\"");
            }
        }

        failures.Should().BeEmpty("a missing resource, a bad format argument, or a template left unfilled would reach the reader");
    }

    /// <summary>
    /// No component claims the same removed parameter twice, directly or through a base type, so no hint silently overrides another.
    /// </summary>
    [Test]
    public void NoTwoEntriesClaimTheSameParameter()
    {
        var failures = new List<string>();
        var entries = ParameterMigration.All;

        for (var i = 0; i < entries.Length; i++)
        {
            for (var j = i + 1; j < entries.Length; j++)
            {
                if (!string.Equals(entries[i].ParameterName, entries[j].ParameterName, StringComparison.OrdinalIgnoreCase))
                    continue;

                var first = Resolve(entries[i]);
                var second = Resolve(entries[j]);

                if (first is null || second is null || InheritsFrom(first, second) || InheritsFrom(second, first))
                    failures.Add($"{Describe(entries[i])} conflicts with {Describe(entries[j])}");
            }
        }

        failures.Should().BeEmpty();
    }

    /// <summary>
    /// Every entry is exercised by at least one consumer-facing case, which is what keeps the synthetic and real-Razor coverage complete as the catalog grows.
    /// </summary>
    [Test]
    public void EveryEntryIsCoveredByAConsumerCase()
    {
        var failures = ParameterMigration.All
            .Where(entry => !MigrationHintCases.All.Any(c =>
                string.Equals(c.Removed, entry.ParameterName, StringComparison.Ordinal)
                && string.Equals(c.Replacement, entry.ReplacementParameterName, StringComparison.Ordinal)
                && _compilation.GetTypeByMetadataName(c.ComponentMetadataName) is { } component
                && InheritsFrom(component, Resolve(entry)!)))
            .Select(Describe);

        failures.Should().BeEmpty();
    }

    /// <summary>
    /// A type that only shares a catalog component's name gets no hints, and a partial MudBlazor gets hints only for the components it declares.
    /// </summary>
    [Test]
    public void ResolverSkipsComponentsTheCompilationCannotSee()
    {
        var lookalike = AnalyzerCompilationFactory.CreateCompilation(
            "namespace Other { public class MudBaseButton : Microsoft.AspNetCore.Components.ComponentBase { } }",
            referenceMudBlazor: false);
        HintsFor(lookalike, "Other.MudBaseButton").Should().BeEmpty();

        var partial = AnalyzerCompilationFactory.CreateCompilation(PartialMudBlazorSource, referenceMudBlazor: false);
        HintsFor(partial, "MudBlazor.MudButton").Should().BeEquivalentTo(
            ParameterMigration.All.Where(x => x.ComponentMetadataName == "MudBlazor.MudBaseButton").Select(x => x.ParameterName));
        HintsFor(partial, "MudBlazor.MudCheckBox").Should().BeEmpty();

        static IEnumerable<string> HintsFor(Compilation compilation, string metadataName) =>
            ComponentDescriptor.GetComponentDescriptor(
                compilation.GetTypeByMetadataName(metadataName)!,
                compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Components.ParameterAttribute"),
                new ParameterMigrationResolver(compilation)).MigrationHints.Keys;
    }

    /// <summary>
    /// Against a partial MudBlazor, a resolvable component still gets its hint while a same-named type of a different arity gets none, and nothing crashes.
    /// </summary>
    [Test]
    public async Task PartialMudBlazorExplainsOnlyWhatItCanResolve()
    {
        var diagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            PartialMudBlazorSource,
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase,
            referenceMudBlazor: false);

        diagnostics.Should().OnlyContain(x => x.Id == "MUD0002");
        diagnostics.Single("Link", "MudButton").ShouldCarryHint("Link", "MudButton", MigrationHintExpectations.Link);
        diagnostics.Single("Checked", "MudCheckBox").ShouldCarryNoHint("Checked", "MudCheckBox");
    }

    private const string PartialMudBlazorSource =
        """
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Components.Rendering;

        namespace MudBlazor
        {
            public abstract class MudComponentBase : ComponentBase
            {
            }

            public abstract class MudBaseButton : MudComponentBase
            {
                [Parameter]
                public string? Href { get; set; }
            }

            public class MudButton : MudBaseButton
            {
            }

            public class MudCheckBox : MudComponentBase
            {
            }
        }

        namespace MudBlazor.Analyzers.TestComponents
        {
            public class PartialMudBlazorSource : ComponentBase
            {
                protected override void BuildRenderTree(RenderTreeBuilder builder)
                {
                    builder.OpenComponent<MudBlazor.MudButton>(0);
                    builder.AddAttribute(1, "Link", "/somewhere");
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudCheckBox>(2);
                    builder.AddAttribute(3, "Checked", true);
                    builder.CloseComponent();
                }
            }
        }
        """;

    private static string Describe(ParameterMigration entry) => $"{entry.ComponentMetadataName}.{entry.ParameterName}";

    private static INamedTypeSymbol? Resolve(ParameterMigration entry) => _compilation.GetTypeByMetadataName(entry.ComponentMetadataName);

    /// <summary>
    /// The registered component and every type in MudBlazor that derives from it, which is everything the hint can reach.
    /// </summary>
    private static IEnumerable<INamedTypeSymbol> Reached(ParameterMigration entry)
    {
        if (Resolve(entry) is not { } component)
            return [];

        return AllTypes(MudComponentBase.ContainingAssembly.GlobalNamespace).Where(x => InheritsFrom(x, component));
    }

    private static IEnumerable<INamedTypeSymbol> AllTypes(INamespaceSymbol ns) =>
        ns.GetTypeMembers().SelectMany(Nested).Concat(ns.GetNamespaceMembers().SelectMany(AllTypes));

    private static IEnumerable<INamedTypeSymbol> Nested(INamedTypeSymbol type) =>
        type.GetTypeMembers().SelectMany(Nested).Prepend(type);

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, baseType.OriginalDefinition))
                return true;
        }

        return false;
    }

    private static IPropertySymbol? FindParameter(INamedTypeSymbol type, string name, StringComparison comparison)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var match = current.OriginalDefinition.GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(p => string.Equals(p.Name, name, comparison)
                    && p.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, ParameterAttribute)));

            if (match is not null)
                return match;
        }

        return null;
    }

    private static bool IsNullableBoolean(ITypeSymbol? type) =>
        type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable
        && nullable.TypeArguments[0].SpecialType is SpecialType.System_Boolean;

    private static object? GetDefault(string metadataName, string parameterName)
    {
        var type = typeof(MudBlazor.MudComponentBase).Assembly.GetType(metadataName, throwOnError: true)!;
        if (type.IsGenericTypeDefinition)
            type = type.MakeGenericType([.. Enumerable.Repeat(typeof(string), type.GetGenericArguments().Length)]);

        var component = Activator.CreateInstance(type);

        return type.GetProperty(parameterName, BindingFlags.Public | BindingFlags.Instance)!.GetValue(component);
    }
}
#nullable restore
