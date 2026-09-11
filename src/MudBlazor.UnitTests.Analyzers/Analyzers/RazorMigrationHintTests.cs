// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MudBlazor.Analyzers.TestComponents;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;

#nullable enable
/// <summary>
/// Runs MUD0002 over <c>MigrationHints.razor</c> as the Razor compiler actually lowers it, so the hints are verified against real markup rather than against hand-written render-tree calls.
/// </summary>
[TestFixture]
public class RazorMigrationHintTests
{
    private const string FixtureClassName = "MudBlazor.Analyzers.TestComponents.MigrationHints";

    private static IReadOnlyList<Diagnostic> LowerCaseDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> NoneDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> AnyDiagnostics { get; set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        using var projectCompilation = await ProjectCompilation.CreateAsync(Util.ProjectPath());

        LowerCaseDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase);
        NoneDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None);
        AnyDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.Any);

        async Task<IReadOnlyList<Diagnostic>> GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern pattern)
        {
            var analyzer = new MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer();
            var options = TestAnalyzerOptions.Create(pattern, projectCompilation.AdditionalTexts);
            var diagnostics = await projectCompilation.GetDiagnosticsAsync(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer), options);

            return diagnostics.FilterToClass(FixtureClassName);
        }
    }

    /// <summary>
    /// Every removed parameter in the fixture reports once and carries its own component-specific guidance.
    /// </summary>
    [Test]
    public void RemovedParametersCarryTheirOwnGuidance()
    {
        var links = LowerCaseDiagnostics.All("Link", "MudButton").WithHint();

        links.Should().HaveCount(2, "the single-line and multiline usages are both explained");

        foreach (var diagnostic in links)
        {
            diagnostic.ShouldCarryHint("Link", "MudButton", MigrationHintExpectations.Link);
        }

        foreach (var diagnostic in LowerCaseDiagnostics.All("DisableRipple", "MudButton"))
        {
            diagnostic.ShouldCarryHint("DisableRipple", "MudButton", MigrationHintExpectations.DisableRipple);
        }

        foreach (var diagnostic in LowerCaseDiagnostics.All("AutoGrow", "MudTextField"))
        {
            diagnostic.ShouldCarryHint("AutoGrow", "MudTextField", MigrationHintExpectations.AutoGrow);
        }
    }

    /// <summary>
    /// Literal and expression-valued attributes, including multiline markup with nested content, all report.
    /// </summary>
    [Test]
    public void LiteralAndExpressionValuesBothReport()
    {
        LowerCaseDiagnostics.All("DisableRipple", "MudButton").Should().HaveCount(4,
            "the fixture uses true, false, a simple expression, and a compound expression");

        LowerCaseDiagnostics.All("Link", "MudButton").Should().HaveCount(3,
            "the fixture has a single-line, a multiline, and a shadow-type usage");
    }

    /// <summary>
    /// A generic MudTextField bound with @bind-Value lowers through a TypeInference helper and still reports.
    /// </summary>
    [Test]
    public void BoundGenericTextFieldReportsThroughTypeInference()
    {
        var diagnostics = LowerCaseDiagnostics.All("AutoGrow", "MudTextField");

        diagnostics.Should().HaveCount(4, "two bound usages lower through TypeInference and two are inline");
    }

    /// <summary>
    /// A component derived from MudButton inherits the hint.
    /// </summary>
    [Test]
    public void DerivedButtonInheritsHint()
    {
        LowerCaseDiagnostics.Single("Link", "DerivedMudButton").ShouldCarryHint("Link", "DerivedMudButton", MigrationHintExpectations.Link);
    }

    /// <summary>
    /// A derived component that declares its own Link parameter is not reported at all.
    /// </summary>
    [Test]
    public void DerivedButtonDeclaringLinkRaisesNoDiagnostic()
    {
        NoneDiagnostics.All("Link", "ReintroducedLinkButton").Should().BeEmpty();
    }

    /// <summary>
    /// A type that only shares MudButton's display name keeps the generic warning.
    /// </summary>
    [Test]
    public void ShadowTypeWithMudButtonNameKeepsGenericWarning()
    {
        var withoutHint = LowerCaseDiagnostics
            .All("Link", "MudButton")
            .Where(x => !x.GetMessage().Contains(MigrationHintExpectations.HintMarker))
            .ToList();

        withoutHint.Should().ContainSingle("only the shadow type's usage is left unexplained");
    }

    /// <summary>
    /// Components outside the mapping, and attribute names that merely look similar, keep the generic warning.
    /// </summary>
    [Test]
    public void UnrelatedComponentsAndSimilarNamesKeepGenericWarning()
    {
        LowerCaseDiagnostics.Single("Link", "MudChip").ShouldCarryNoHint("Link", "MudChip");
        LowerCaseDiagnostics.Single("Link", "MudIconButton").ShouldCarryNoHint("Link", "MudIconButton");
        LowerCaseDiagnostics.Single("Linked", "MudButton").ShouldCarryNoHint("Linked", "MudButton");
        LowerCaseDiagnostics.Single("DisableRippleEffect", "MudButton").ShouldCarryNoHint("DisableRippleEffect", "MudButton");
        LowerCaseDiagnostics.Single("AutoGrowth", "MudTextField").ShouldCarryNoHint("AutoGrowth", "MudTextField");
    }

    /// <summary>
    /// Href, Ripple, and Sizing usages in the fixture compile and raise nothing.
    /// </summary>
    [Test]
    public void ReplacementMarkupRaisesNoDiagnostic()
    {
        NoneDiagnostics.All("Href", "MudButton").Should().BeEmpty();
        NoneDiagnostics.All("Ripple", "MudButton").Should().BeEmpty();
        NoneDiagnostics.All("Sizing", "MudTextField").Should().BeEmpty();
        NoneDiagnostics.All("Lines", "MudTextField").Should().BeEmpty();
    }

    /// <summary>
    /// Removed parameters written inside a Razor comment, an HTML comment, or a C# string are never analyzed.
    /// </summary>
    [Test]
    public void MarkupInCommentsAndStringsIsNotAnalyzed()
    {
        NoneDiagnostics.Should().NotContain(x => x.GetMessage().Contains("/razor-comment"));
        NoneDiagnostics.Should().NotContain(x => x.GetMessage().Contains("/string-literal"));
        NoneDiagnostics.All("Link", "MudButton").Should().HaveCount(3, "the commented-out usages add nothing");
    }

    /// <summary>
    /// A removed parameter supplied through an @attributes dictionary is a known blind spot, not a hint.
    /// </summary>
    /// <remarks>
    /// The analyzer only sees the splat call, never the dictionary's keys, so this usage is undiagnosed rather than migration-clean.
    /// </remarks>
    [Test]
    public void DynamicAttributeSplatIsNotDiagnosed()
    {
        NoneDiagnostics.Should().NotContain(x => x.GetMessage().Contains("/splatted"));
        NoneDiagnostics.All("Link", "MudButton").Should().HaveCount(3, "the splatted Link is invisible to the analyzer");
    }

    /// <summary>
    /// Diagnostics map back to the .razor file, which the migration hint does not change.
    /// </summary>
    [Test]
    public void DiagnosticsStillMapToTheRazorFile()
    {
        var enriched = LowerCaseDiagnostics.Single("Link", "DerivedMudButton");

        enriched.Location.GetLineSpan().Path.Should().EndWith("MigrationHints.razor");
        enriched.AdditionalLocations.Should().ContainSingle();
        enriched.AdditionalLocations[0].GetLineSpan().Path.Should().EndWith(".g.cs");
    }

    /// <summary>
    /// The fixture's diagnostic population does not change with the hints, and Any still reports nothing.
    /// </summary>
    [Test]
    public void DiagnosticPopulationIsUnchanged()
    {
        LowerCaseDiagnostics.Should().HaveCount(17);
        LowerCaseDiagnostics.Should().OnlyContain(x => x.Id == "MUD0002");
        LowerCaseDiagnostics.Count(x => x.GetMessage().Contains(MigrationHintExpectations.HintMarker)).Should().Be(11);
        AnyDiagnostics.Should().BeEmpty();
    }
}
#nullable restore
