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

    private const string CatalogNamespace = "MudBlazor.Analyzers.TestComponents.MigrationHintCatalog.";

    private static IReadOnlyList<Diagnostic> LowerCaseDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> NoneDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> AnyDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> CatalogDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> ReplacementDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> LegacyBindingDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> BoundaryDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> AttributeTestLowerCaseDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> AttributeTestNoneDiagnostics { get; set; } = null!;

    private static IEnumerable<string> CaseNames => MigrationHintCases.All.Select(x => x.ToString());

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        using var projectCompilation = await ProjectCompilation.CreateAsync(Util.ProjectPath());

        var lowerCase = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase);
        var none = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None);

        LowerCaseDiagnostics = lowerCase.FilterToClass(FixtureClassName);
        NoneDiagnostics = none.FilterToClass(FixtureClassName);
        AnyDiagnostics = (await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.Any)).FilterToClass(FixtureClassName);

        CatalogDiagnostics = [.. lowerCase.Where(x => ClassName(x).StartsWith($"{CatalogNamespace}Removed", StringComparison.Ordinal))];
        ReplacementDiagnostics = none.FilterToClass($"{CatalogNamespace}Replacements");
        LegacyBindingDiagnostics = lowerCase.FilterToClass($"{CatalogNamespace}LegacyBindings");
        BoundaryDiagnostics = lowerCase.FilterToClass($"{CatalogNamespace}Boundaries");
        AttributeTestLowerCaseDiagnostics = lowerCase.FilterToClass("MudBlazor.Analyzers.TestComponents.AttributeTest");
        AttributeTestNoneDiagnostics = none.FilterToClass("MudBlazor.Analyzers.TestComponents.AttributeTest");

        async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern pattern)
        {
            var analyzer = new MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer();
            var options = TestAnalyzerOptions.Create(pattern, projectCompilation.AdditionalTexts);

            return await projectCompilation.GetDiagnosticsAsync(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer), options);
        }

        static string ClassName(Diagnostic diagnostic) =>
            diagnostic.Properties.TryGetValue(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey, out var name) ? name ?? string.Empty : string.Empty;
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
    /// MudChip and MudIconButton dropped Link in the same release as MudButton, so they carry the same Href guidance.
    /// </summary>
    [Test]
    public void LinkOnMudChipAndMudIconButtonExplainsHref()
    {
        LowerCaseDiagnostics.Single("Link", "MudChip").ShouldCarryHint("Link", "MudChip", MigrationHintExpectations.Link);
        LowerCaseDiagnostics.Single("Link", "MudIconButton").ShouldCarryHint("Link", "MudIconButton", MigrationHintExpectations.Link);
    }

    /// <summary>
    /// Attribute names that merely look like removed parameters keep the generic warning.
    /// </summary>
    [Test]
    public void SimilarNamesKeepGenericWarning()
    {
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
        LowerCaseDiagnostics.Count(x => x.GetMessage().Contains(MigrationHintExpectations.HintMarker)).Should().Be(13);
        AnyDiagnostics.Should().BeEmpty();
    }

    /// <summary>
    /// Every catalog case, written in real markup on the component a consumer uses, carries its own guidance.
    /// </summary>
    [TestCaseSource(nameof(CaseNames))]
    public void CatalogCaseCarriesItsHint(string caseName)
    {
        var hintCase = MigrationHintCases.All.Single(x => x.ToString() == caseName);

        CatalogDiagnostics.Single(hintCase.Removed, hintCase.TagName).ShouldCarryHint(hintCase.Removed, hintCase.TagName, [.. hintCase.ExpectedHint]);
    }

    /// <summary>
    /// The catalog fixtures raise exactly one diagnostic per case, every one explained, and nothing else.
    /// </summary>
    [Test]
    public void CatalogFixturesRaiseOneExplainedDiagnosticPerCase()
    {
        CatalogDiagnostics.Should().HaveCount(MigrationHintCases.All.Count);
        CatalogDiagnostics.Should().OnlyContain(x => x.Id == "MUD0002" && x.GetMessage().Contains(MigrationHintExpectations.HintMarker));
    }

    /// <summary>
    /// The markup every hint and documented example points at compiles and raises nothing, even with every attribute checked.
    /// </summary>
    [Test]
    public void ReplacementMarkupRaisesNothingUnderTheStrictestPattern()
    {
        ReplacementDiagnostics.Select(x => x.GetMessage()).Should().BeEmpty();
    }

    /// <summary>
    /// A legacy @bind- lowers into the removed value and its removed callback, and both halves are explained, as are explicitly typed callbacks.
    /// </summary>
    [Test]
    public void LegacyBindingsExplainBothHalves()
    {
        LegacyBindingDiagnostics.Should().HaveCount(23, "ten legacy bindings report two halves each, plus three explicitly typed attributes");
        LegacyBindingDiagnostics.Should().OnlyContain(x => x.GetMessage().Contains(MigrationHintExpectations.HintMarker));

        AssertEach("Checked", "MudCheckBox", 3);
        AssertEach("CheckedChanged", "MudCheckBox", 3);
        AssertEach("Checked", "MudSwitch", 1);
        AssertEach("CheckedChanged", "MudSwitch", 2);
        AssertEach("SelectedOption", "MudRadioGroup", 1);
        AssertEach("SelectedOptionChanged", "MudRadioGroup", 1);
        AssertEach("IsVisible", "MudDialog", 1);
        AssertEach("IsVisibleChanged", "MudDialog", 1);
        AssertEach("IsVisible", "MudMessageBox", 1);
        AssertEach("IsVisibleChanged", "MudMessageBox", 1);
        AssertEach("IsVisible", "MudTooltip", 1);
        AssertEach("IsVisibleChanged", "MudTooltip", 1);
        AssertEach("IsHidden", "MudHidden", 1);
        AssertEach("IsHiddenChanged", "MudHidden", 1);
        AssertEach("IsExpanded", "MudExpansionPanel", 1);
        AssertEach("IsExpandedChanged", "MudExpansionPanel", 1);
        AssertEach("IsChecked", "MudTr", 1);
        AssertEach("IsCheckedChanged", "MudTr", 1);

        static void AssertEach(string attributeName, string tagName, int count)
        {
            var hintCase = MigrationHintCases.All.Single(x => x.ToString() == $"{tagName}.{attributeName}");
            var diagnostics = LegacyBindingDiagnostics.All(attributeName, tagName);

            diagnostics.Should().HaveCount(count);

            foreach (var diagnostic in diagnostics)
            {
                diagnostic.ShouldCarryHint(attributeName, tagName, [.. hintCase.ExpectedHint]);
            }
        }
    }

    /// <summary>
    /// Lookalikes keep the generic warning, and current parameters, a case-only rename, and a component MUD0002 does not inspect raise nothing.
    /// </summary>
    [Test]
    public void BoundariesKeepGenericWarningOrStaySilent()
    {
        BoundaryDiagnostics.Should().HaveCount(7);
        BoundaryDiagnostics.Single("Link", "MudMenu").ShouldCarryNoHint("Link", "MudMenu");
        BoundaryDiagnostics.Single("IsOpen", "MudMenu").ShouldCarryNoHint("IsOpen", "MudMenu");
        BoundaryDiagnostics.Single("DisableRipple", "MudSelectItem").ShouldCarryNoHint("DisableRipple", "MudSelectItem");
        BoundaryDiagnostics.Single("IsOpen", "MudAutocomplete").ShouldCarryNoHint("IsOpen", "MudAutocomplete");
        BoundaryDiagnostics.Single("Checked", "MudRadio").ShouldCarryNoHint("Checked", "MudRadio");
        BoundaryDiagnostics.Single("IsChecked", "MudTHeadRow").ShouldCarryNoHint("IsChecked", "MudTHeadRow");
        BoundaryDiagnostics.Single("Title", "MudIconButton").ShouldCarryNoHint("Title", "MudIconButton");
    }

    /// <summary>
    /// The existing attribute fixture keeps every warning it had, and only the removed parameters it already used gain a hint.
    /// </summary>
    [Test]
    public void ExistingAttributeFixtureOnlyGainsIntendedHints()
    {
        AttributeTestLowerCaseDiagnostics.Should().HaveCount(13);
        AttributeTestLowerCaseDiagnostics.WithHint().Should().ContainSingle();
        AttributeTestLowerCaseDiagnostics.Single("Minimum", "MudProgressLinear").ShouldCarryHint("Minimum", "MudProgressLinear", [.. MigrationHintCases.All.Single(x => x.ToString() == "MudProgressLinear.Minimum").ExpectedHint]);

        AttributeTestNoneDiagnostics.Should().HaveCount(22);
        AttributeTestNoneDiagnostics.WithHint().Should().HaveCount(2);
        AttributeTestNoneDiagnostics.Single("icon", "MudFab").ShouldCarryHint("icon", "MudFab", [.. MigrationHintCases.All.Single(x => x.ToString() == "MudFab.Icon").ExpectedHint]);
    }
}
#nullable restore
