// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
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
/// These compile hand-written render-tree calls, which is fast but is not proof that the Razor compiler
/// lowers real markup the same way. <see cref="RazorMigrationHintTests"/> covers that separately.
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
    /// Components outside the mapping get no hint even when they carry an identically named attribute.
    /// </summary>
    [Test]
    public void UnrelatedComponentWithSameAttributeNameKeepsGenericWarning()
    {
        LowerCaseDiagnostics.Single("Link", "MudIconButton").ShouldCarryNoHint("Link", "MudIconButton");
        LowerCaseDiagnostics.Single("Link", "MudChip").ShouldCarryNoHint("Link", "MudChip");
        LowerCaseDiagnostics.Single("AutoGrow", "MudAutocomplete").ShouldCarryNoHint("AutoGrow", "MudAutocomplete");
        LowerCaseDiagnostics.Single("AutoGrow", "MudButton").ShouldCarryNoHint("AutoGrow", "MudButton");
    }

    /// <summary>
    /// A different type that merely shares MudButton's short name gets no hint, because the mapping matches
    /// on type identity rather than on the tag name the message displays.
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
