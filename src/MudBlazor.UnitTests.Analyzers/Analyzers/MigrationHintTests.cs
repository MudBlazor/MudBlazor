// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Text;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;

#nullable enable
/// <summary>
/// Covers the migration guidance MUD0002 appends for parameters an earlier major version removed.
/// </summary>
/// <remarks>
/// These compile hand-written render-tree calls, which is fast but is not proof that the Razor compiler lowers real markup the same way.
/// <see cref="RazorMigrationHintTests"/> covers that separately.
/// </remarks>
[TestFixture]
public class MigrationHintTests
{
    private static IReadOnlyList<Diagnostic> LowerCaseDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> DefaultListDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> CustomListDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> DataAndAriaDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> NoneDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> AnyDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> CatalogDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> BoundaryDiagnostics { get; set; } = null!;

    private static IEnumerable<string> CaseNames => MigrationHintCases.All.Select(x => x.ToString());

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        var source = CreateGeneratedSource();

        LowerCaseDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase);
        DefaultListDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes);
        CustomListDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes, "Link,DisableRipple,AutoGrow");
        DataAndAriaDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.DataAndAria);
        NoneDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None);
        AnyDiagnostics = await GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.Any);

        CatalogDiagnostics = (await AnalyzerCompilationFactory.GetDiagnosticsAsync(CreateCatalogSource(), MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase))
            .FilterToClass("MudBlazor.Analyzers.TestComponents.MigrationHintCatalogSource");
        BoundaryDiagnostics = (await AnalyzerCompilationFactory.GetDiagnosticsAsync(CreateBoundarySource(), MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase))
            .FilterToClass("MudBlazor.Analyzers.TestComponents.MigrationHintBoundarySource");

        async Task<IReadOnlyList<Diagnostic>> GetDiagnosticsAsync(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern pattern, string customAllowedAttributes = "")
        {
            var diagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(source, pattern, customAllowedAttributes);

            return diagnostics.FilterToClass("MudBlazor.Analyzers.TestComponents.MigrationHintSource");
        }
    }

    /// <summary>
    /// A removed MudButton.Link points the reader at Href.
    /// </summary>
    [Test]
    public void LinkOnMudButtonExplainsHref()
    {
        var diagnostics = LowerCaseDiagnostics.All("Link", "MudButton");

        diagnostics.Should().HaveCount(2, "the real MudButton and the shadow type share a display name");
        diagnostics.WithHint().Should().ContainSingle();
        diagnostics.WithHint()[0].ShouldCarryHint("Link", "MudButton", MigrationHintExpectations.Link);
    }

    /// <summary>
    /// A removed MudButton.DisableRipple explains that Ripple inverts the value in both directions.
    /// </summary>
    [Test]
    public void DisableRippleOnMudButtonExplainsInvertedRipple()
    {
        var diagnostics = LowerCaseDiagnostics.All("DisableRipple", "MudButton");

        diagnostics.Should().HaveCount(3, "the literal, simple expression, and complex expression forms are all reported");

        foreach (var diagnostic in diagnostics)
        {
            diagnostic.ShouldCarryHint("DisableRipple", "MudButton", MigrationHintExpectations.DisableRipple);
        }
    }

    /// <summary>
    /// A removed MudTextField.AutoGrow explains both enum outcomes and the conditional expression form.
    /// </summary>
    [Test]
    public void AutoGrowOnMudTextFieldExplainsSizing()
    {
        var diagnostics = LowerCaseDiagnostics.All("AutoGrow", "MudTextField");

        diagnostics.Should().HaveCount(3, "two inline usages plus one lowered through a TypeInference helper");

        foreach (var diagnostic in diagnostics)
        {
            diagnostic.ShouldCarryHint("AutoGrow", "MudTextField", MigrationHintExpectations.AutoGrow);
        }
    }

    /// <summary>
    /// The hint follows the removed parameter's own casing rules, so a lower-cased spelling still resolves.
    /// </summary>
    [Test]
    public void DifferentlyCasedRemovedParameterStillExplainsSizing()
    {
        NoneDiagnostics.Single("autogrow", "MudTextField").ShouldCarryHint("autogrow", "MudTextField", MigrationHintExpectations.AutoGrow);
    }

    /// <summary>
    /// The replacement parameters compile and raise nothing at all.
    /// </summary>
    [Test]
    public void ReplacementParametersRaiseNoDiagnostic()
    {
        NoneDiagnostics.All("Href", "MudButton").Should().BeEmpty();
        NoneDiagnostics.All("Ripple", "MudButton").Should().BeEmpty();
        NoneDiagnostics.All("Sizing", "MudTextField").Should().BeEmpty();
        NoneDiagnostics.All("MaxLines", "MudTextField").Should().BeEmpty();
        NoneDiagnostics.All("Value", "MudTextField").Should().BeEmpty();
    }

    /// <summary>
    /// A misspelling that is not in the mapping keeps the unchanged generic warning.
    /// </summary>
    [Test]
    public void UnmappedTypoKeepsGenericWarning()
    {
        LowerCaseDiagnostics.Single("Linked", "MudButton").ShouldCarryNoHint("Linked", "MudButton");
        LowerCaseDiagnostics.Single("DisableRippleEffect", "MudButton").ShouldCarryNoHint("DisableRippleEffect", "MudButton");
        LowerCaseDiagnostics.Single("AutoGrowth", "MudTextField").ShouldCarryNoHint("AutoGrowth", "MudTextField");
    }

    /// <summary>
    /// Link on MudIconButton and MudChip was removed alongside MudButton's, so they get the same Href guidance.
    /// </summary>
    [Test]
    public void LinkOnOtherComponentsThatDroppedItExplainsHref()
    {
        LowerCaseDiagnostics.Single("Link", "MudIconButton").ShouldCarryHint("Link", "MudIconButton", MigrationHintExpectations.Link);
        LowerCaseDiagnostics.Single("Link", "MudChip").ShouldCarryHint("Link", "MudChip", MigrationHintExpectations.Link);
    }

    /// <summary>
    /// A removed name written on a component that never declared it gets no hint, even when another component's hint uses the same name.
    /// </summary>
    [Test]
    public void RemovedNameOnComponentThatNeverHadItKeepsGenericWarning()
    {
        LowerCaseDiagnostics.Single("AutoGrow", "MudAutocomplete").ShouldCarryNoHint("AutoGrow", "MudAutocomplete");
        LowerCaseDiagnostics.Single("AutoGrow", "MudButton").ShouldCarryNoHint("AutoGrow", "MudButton");
    }

    /// <summary>
    /// Every catalog case, written on the concrete component a consumer uses, carries its own guidance.
    /// </summary>
    [TestCaseSource(nameof(CaseNames))]
    public void CatalogCaseCarriesItsHint(string caseName)
    {
        var hintCase = MigrationHintCases.All.Single(x => x.ToString() == caseName);

        CatalogDiagnostics.Single(hintCase.Removed, hintCase.TagName).ShouldCarryHint(hintCase.Removed, hintCase.TagName, [.. hintCase.ExpectedHint]);
    }

    /// <summary>
    /// The catalog source raises exactly one diagnostic per case, every one explained, and nothing else.
    /// </summary>
    [Test]
    public void CatalogSourceRaisesOneExplainedDiagnosticPerCase()
    {
        MigrationHintCases.All.Select(x => x.ToString()).Should().OnlyHaveUniqueItems();
        CatalogDiagnostics.Should().HaveCount(MigrationHintCases.All.Count);
        CatalogDiagnostics.Should().OnlyContain(x => x.Id == "MUD0002" && x.GetMessage().Contains(MigrationHintExpectations.HintMarker));
    }

    /// <summary>
    /// Names that look like catalog entries but were never removed parameters on that component keep the generic warning.
    /// </summary>
    [Test]
    public void LookalikesOfCatalogEntriesKeepGenericWarning()
    {
        // MudMenu.Link was removed with no navigation replacement, so MudMenuItem's Href advice must not leak onto it.
        BoundaryDiagnostics.Single("Link", "MudMenu").ShouldCarryNoHint("Link", "MudMenu");
        // v6 MudSelectItem inherited DisableRipple from a base that no longer exists, and today's MudSelectItem has no Ripple.
        BoundaryDiagnostics.Single("DisableRipple", "MudSelectItem").ShouldCarryNoHint("DisableRipple", "MudSelectItem");
        // IsOpen was never a parameter, so the IsOpenChanged rename does not make IsOpen a migration.
        BoundaryDiagnostics.Single("IsOpen", "MudMenu").ShouldCarryNoHint("IsOpen", "MudMenu");
        BoundaryDiagnostics.Single("IsOpen", "MudAutocomplete").ShouldCarryNoHint("IsOpen", "MudAutocomplete");
        // MudRadio's Checked was internal, so the checkbox and switch hint must not reach it through their shared base.
        BoundaryDiagnostics.Single("Checked", "MudRadio").ShouldCarryNoHint("Checked", "MudRadio");
        // Only MudTr ever had IsChecked as a parameter.
        BoundaryDiagnostics.Single("IsChecked", "MudTHeadRow").ShouldCarryNoHint("IsChecked", "MudTHeadRow");
    }

    /// <summary>
    /// Current parameters that share a name with a removed one, case-only renames, and components MUD0002 does not inspect raise nothing.
    /// </summary>
    [Test]
    public void CurrentNamesAndUninspectedComponentsRaiseNothing()
    {
        BoundaryDiagnostics.All("PanelClass", "MudTabPanel").Should().BeEmpty("MudTabPanel.PanelClass is a current parameter");
        BoundaryDiagnostics.All("IconExpanded", "MudTreeViewItem").Should().BeEmpty("IconExpanded is a current parameter, not the replacement for ExpandedIcon");
        BoundaryDiagnostics.All("ExpandedIcon", "MudTreeViewItemToggleButton").Should().BeEmpty("the toggle button still declares ExpandedIcon");
        BoundaryDiagnostics.All("UnCheckedColor", "MudCheckBox").Should().BeEmpty("a case-only rename still matches case-insensitively");
        BoundaryDiagnostics.All("ObserveSystemThemeChange", "MudThemeProvider").Should().BeEmpty("MudThemeProvider does not derive from MudComponentBase");
    }

    /// <summary>
    /// A consumer component deriving from a closed generic MudCheckBox inherits the hint, and one that reintroduces Checked keeps its own parameter while its callback is still explained.
    /// </summary>
    [Test]
    public void GenericDerivedComponentsFollowTheirBase()
    {
        BoundaryDiagnostics.Single("Checked", "DerivedCheckBox").ShouldCarryHint("Checked", "DerivedCheckBox", ExpectedHint("MudCheckBox.Checked"));
        BoundaryDiagnostics.All("Checked", "ReintroducedCheckedBox").Should().BeEmpty();
        BoundaryDiagnostics.Single("CheckedChanged", "ReintroducedCheckedBox").ShouldCarryHint("CheckedChanged", "ReintroducedCheckedBox", ExpectedHint("MudCheckBox.CheckedChanged"));
    }

    /// <summary>
    /// The pagers share a removed name but not a conversion: MudTablePager keeps the value, MudDataGridPager inverts it.
    /// </summary>
    [Test]
    public void PagersExplainOppositeConversionsForTheSameName()
    {
        var table = CatalogDiagnostics.Single("DisableRowsPerPage", "MudTablePager").GetMessage();
        var grid = CatalogDiagnostics.Single("DisableRowsPerPage", "MudDataGridPager").GetMessage();

        table.Should().Contain("DisableRowsPerPage=\"true\" becomes HideRowsPerPage=\"true\"").And.NotContain("PageSizeSelector");
        grid.Should().Contain("DisableRowsPerPage=\"true\" becomes PageSizeSelector=\"false\"").And.NotContain("HideRowsPerPage");
    }

    /// <summary>
    /// A different type that merely shares MudButton's short name gets no hint, because the mapping matches on type identity rather than on the tag name the message displays.
    /// </summary>
    [Test]
    public void ShadowTypeWithMudButtonNameKeepsGenericWarning()
    {
        var unexplained = LowerCaseDiagnostics.All("Link", "MudButton").WithoutHint();

        unexplained.Should().ContainSingle("only the shadow type's usage is left unexplained");
        unexplained[0].ShouldCarryNoHint("Link", "MudButton");
    }

    /// <summary>
    /// A component derived from MudButton inherits the hint when it does not declare the removed parameter.
    /// </summary>
    [Test]
    public void DerivedMudButtonInheritsHint()
    {
        LowerCaseDiagnostics.Single("Link", "DerivedButton").ShouldCarryHint("Link", "DerivedButton", MigrationHintExpectations.Link);
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
    /// The enriched diagnostic keeps MUD0002's identity, severity, help link, properties, and locations.
    /// </summary>
    [Test]
    public void EnrichedDiagnosticKeepsDiagnosticShape()
    {
        var enriched = LowerCaseDiagnostics.Single("Link", "DerivedButton");
        var generic = LowerCaseDiagnostics.Single("Linked", "MudButton");

        enriched.Id.Should().Be("MUD0002").And.Be(generic.Id);
        enriched.Descriptor.Should().BeSameAs(generic.Descriptor);
        enriched.Severity.Should().Be(DiagnosticSeverity.Warning).And.Be(generic.Severity);
        enriched.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        enriched.Descriptor.IsEnabledByDefault.Should().BeTrue();
        enriched.Descriptor.HelpLinkUri.Should().Be(generic.Descriptor.HelpLinkUri).And.NotBeNullOrEmpty();
        enriched.IsSuppressed.Should().BeFalse();
        enriched.Properties.Should().ContainKey(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey);
        enriched.AdditionalLocations.Should().HaveCount(1).And.HaveCount(generic.AdditionalLocations.Count);
        enriched.Location.SourceTree?.FilePath.Should().Be(generic.Location.SourceTree?.FilePath);
    }

    /// <summary>
    /// Every allowed-attribute mode reports the same removed parameters as before, with the hints attached.
    /// </summary>
    [Test]
    public void AllowedAttributeModesReportTheSameRemovedParameters()
    {
        foreach (var diagnostics in new[] { LowerCaseDiagnostics, DefaultListDiagnostics, DataAndAriaDiagnostics, NoneDiagnostics })
        {
            diagnostics.Single("Link", "DerivedButton").ShouldCarryHint("Link", "DerivedButton", MigrationHintExpectations.Link);
            diagnostics.All("DisableRipple", "MudButton").Should().HaveCount(3);
            diagnostics.All("AutoGrow", "MudTextField").Should().HaveCount(3);
        }
    }

    /// <summary>
    /// A custom allowed-attribute list still silences the removed parameters it names.
    /// </summary>
    [Test]
    public void CustomAllowedAttributeListSuppressesTheHint()
    {
        CustomListDiagnostics.All("Link", "MudButton").Should().BeEmpty();
        CustomListDiagnostics.All("Link", "DerivedButton").Should().BeEmpty();
        CustomListDiagnostics.All("DisableRipple", "MudButton").Should().BeEmpty();
        CustomListDiagnostics.All("AutoGrow", "MudTextField").Should().BeEmpty();
    }

    /// <summary>
    /// The Any pattern still turns MUD0002 off completely.
    /// </summary>
    [Test]
    public void AnyPatternReportsNothing()
    {
        AnyDiagnostics.Should().BeEmpty();
    }

    /// <summary>
    /// A pragma still suppresses an enriched diagnostic.
    /// </summary>
    [Test]
    public async Task PragmaSuppressesEnrichedDiagnostic()
    {
        var diagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateSuppressedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase);

        diagnostics.Where(x => !x.IsSuppressed).Should().BeEmpty();
    }

    /// <summary>
    /// Escalating MUD0002 to an error still escalates an enriched diagnostic.
    /// </summary>
    [Test]
    public async Task WarningsAsErrorsEscalatesEnrichedDiagnostic()
    {
        var diagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase,
            specificDiagnosticOptions: ImmutableDictionary<string, ReportDiagnostic>.Empty
                .Add(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.DiagnosticId, ReportDiagnostic.Error));

        var enriched = diagnostics.Single("Link", "DerivedButton");

        enriched.Severity.Should().Be(DiagnosticSeverity.Error);
        enriched.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        enriched.ShouldCarryHint("Link", "DerivedButton", MigrationHintExpectations.Link);
    }

    /// <summary>
    /// Analysis raises MUD0002 and nothing else, so no analyzer crash is reported alongside it.
    /// </summary>
    [Test]
    public void AnalysisRaisesNoOtherDiagnostic()
    {
        LowerCaseDiagnostics.Should().NotBeEmpty();
        LowerCaseDiagnostics.Should().OnlyContain(x => x.Id == "MUD0002");
    }

    private static string CreateGeneratedSource() =>
        """
        using System;
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Components.Rendering;
        using MudBlazor;

        namespace MudBlazor.Analyzers.TestComponents.Shadow
        {
            public class MudButton : MudComponentBase
            {
            }
        }

        namespace MudBlazor.Analyzers.TestComponents
        {
        public class DerivedButton : MudButton
        {
        }

        public class ReintroducedLinkButton : MudButton
        {
            [Parameter]
            public string? Link { get; set; }
        }

        public class MigrationHintSource : ComponentBase
        {
            private readonly string _url = "/somewhere";
            private readonly bool _flag = true;
            private string _text = "y";

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<MudButton>(0);
                builder.AddAttribute(1, "Link", _url);
                builder.CloseComponent();

                builder.OpenComponent<MudButton>(2);
                builder.AddAttribute(3, "DisableRipple", true);
                builder.AddComponentParameter(4, "AutoGrow", true);
                builder.CloseComponent();

                builder.OpenComponent<MudButton>(5);
                builder.AddAttribute(6, "DisableRipple", _flag);
                builder.CloseComponent();

                builder.OpenComponent<MudButton>(7);
                builder.AddAttribute(8, "DisableRipple", !_flag && _url.Length > 0);
                builder.CloseComponent();

                builder.OpenComponent<MudButton>(9);
                builder.AddAttribute(10, "Linked", _url);
                builder.AddAttribute(11, "DisableRippleEffect", true);
                builder.CloseComponent();

                builder.OpenComponent<MudButton>(12);
                builder.AddAttribute(13, "Href", _url);
                builder.AddAttribute(14, "Ripple", !_flag);
                builder.CloseComponent();

                builder.OpenComponent<Shadow.MudButton>(15);
                builder.AddAttribute(16, "Link", _url);
                builder.CloseComponent();

                builder.OpenComponent<DerivedButton>(17);
                builder.AddAttribute(18, "Link", _url);
                builder.CloseComponent();

                builder.OpenComponent<ReintroducedLinkButton>(19);
                builder.AddAttribute(20, "Link", _url);
                builder.CloseComponent();

                builder.OpenComponent<MudIconButton>(21);
                builder.AddAttribute(22, "Link", _url);
                builder.CloseComponent();

                builder.OpenComponent<MudChip<string>>(23);
                builder.AddAttribute(24, "Link", _url);
                builder.CloseComponent();

                builder.OpenComponent<MudTextField<string>>(25);
                builder.AddAttribute(26, "AutoGrow", true);
                builder.AddAttribute(27, "AutoGrowth", true);
                builder.CloseComponent();

                builder.OpenComponent<MudTextField<string>>(28);
                builder.AddAttribute(29, "AutoGrow", _flag);
                builder.AddAttribute(30, "autogrow", _flag);
                builder.CloseComponent();

                builder.OpenComponent<MudTextField<string>>(31);
                builder.AddAttribute(32, "Sizing", InputSizing.Auto);
                builder.AddAttribute(33, "MaxLines", 8);
                builder.CloseComponent();

                builder.OpenComponent<MudAutocomplete<string>>(34);
                builder.AddAttribute(35, "AutoGrow", true);
                builder.CloseComponent();

                TypeInference.CreateMudTextField_0(builder, 36, _text, _flag);
            }
        }

        public static class TypeInference
        {
            public static void CreateMudTextField_0(RenderTreeBuilder builder, int sequence, string value, bool grow)
            {
                builder.OpenComponent<MudTextField<string>>(sequence);
                builder.AddAttribute(sequence + 1, "Value", value);
                builder.AddAttribute(sequence + 2, "AutoGrow", grow);
                builder.CloseComponent();
            }
        }
        }
        """;

    private static string[] ExpectedHint(string caseName) => [.. MigrationHintCases.All.Single(x => x.ToString() == caseName).ExpectedHint];

    /// <summary>
    /// One render-tree call per catalog case, so each case is analyzed exactly as its own component.
    /// </summary>
    private static string CreateCatalogSource()
    {
        var calls = new StringBuilder();
        var sequence = 0;

        foreach (var hintCase in MigrationHintCases.All)
        {
            calls.AppendLine($"        builder.OpenComponent<{hintCase.TypeSyntax}>({sequence++});");
            calls.AppendLine($"        builder.AddComponentParameter({sequence++}, \"{hintCase.Removed}\", true);");
            calls.AppendLine("        builder.CloseComponent();");
        }

        return $$"""
            using Microsoft.AspNetCore.Components;
            using Microsoft.AspNetCore.Components.Rendering;

            namespace MudBlazor.Analyzers.TestComponents;

            public class MigrationHintCatalogSource : ComponentBase
            {
                protected override void BuildRenderTree(RenderTreeBuilder builder)
                {
            {{calls}}
                }
            }
            """;
    }

    private static string CreateBoundarySource() =>
        """
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Components.Rendering;
        using MudBlazor;

        namespace MudBlazor.Analyzers.TestComponents
        {
        public class DerivedCheckBox : MudCheckBox<bool>
        {
        }

        public class ReintroducedCheckedBox<T> : MudCheckBox<T>
        {
            [Parameter]
            public bool Checked { get; set; }
        }

        public class MigrationHintBoundarySource : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<MudMenu>(0);
                builder.AddAttribute(1, "Link", "/menu");
                builder.AddAttribute(2, "IsOpen", true);
                builder.CloseComponent();

                builder.OpenComponent<MudSelectItem<string>>(3);
                builder.AddAttribute(4, "DisableRipple", true);
                builder.CloseComponent();

                builder.OpenComponent<MudAutocomplete<string>>(5);
                builder.AddAttribute(6, "IsOpen", true);
                builder.CloseComponent();

                builder.OpenComponent<MudRadio<string>>(7);
                builder.AddAttribute(8, "Checked", true);
                builder.CloseComponent();

                builder.OpenComponent<MudTHeadRow>(9);
                builder.AddAttribute(10, "IsChecked", true);
                builder.CloseComponent();

                builder.OpenComponent<MudTabPanel>(11);
                builder.AddAttribute(12, "PanelClass", "panel");
                builder.CloseComponent();

                builder.OpenComponent<MudTreeViewItem<string>>(13);
                builder.AddAttribute(14, "IconExpanded", "icon");
                builder.CloseComponent();

                builder.OpenComponent<MudTreeViewItemToggleButton>(15);
                builder.AddAttribute(16, "ExpandedIcon", "icon");
                builder.CloseComponent();

                builder.OpenComponent<MudCheckBox<bool>>(17);
                builder.AddAttribute(18, "UnCheckedColor", Color.Primary);
                builder.CloseComponent();

                builder.OpenComponent<MudThemeProvider>(19);
                builder.AddAttribute(20, "ObserveSystemThemeChange", true);
                builder.CloseComponent();

                builder.OpenComponent<DerivedCheckBox>(21);
                builder.AddAttribute(22, "Checked", true);
                builder.CloseComponent();

                builder.OpenComponent<ReintroducedCheckedBox<bool>>(23);
                builder.AddAttribute(24, "Checked", true);
                builder.AddAttribute(25, "CheckedChanged", true);
                builder.CloseComponent();
            }
        }
        }
        """;

    private static string CreateSuppressedSource() =>
        """
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Components.Rendering;
        using MudBlazor;

        namespace MudBlazor.Analyzers.TestComponents;

        public class SuppressedMigrationHintSource : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
        #pragma warning disable MUD0002
                builder.OpenComponent<MudButton>(0);
                builder.AddAttribute(1, "Link", "/somewhere");
                builder.CloseComponent();
        #pragma warning restore MUD0002
            }
        }
        """;
}
#nullable restore
