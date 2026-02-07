using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using MudBlazor.Analyzers.TestComponents;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;

#nullable enable
[TestFixture]
//[Ignore("Until a solution for matching SDK/roslyn package reference is found see https://github.com/dotnet/roslyn/issues/77979")]
public class ValidAttributeTests : BunitTest
{
    private static ProjectCompilation Workspace { get; set; } = null!;

    private static DiagnosticAnalyzer Analyzer { get; } = new MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer();

    private static IEnumerable<Diagnostic> LowerCaseAttributesDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> DefaultAttributesListDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> CustomAttributesListDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> DataAndAriaAttributesDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> NoAttributesDiagnostics { get; set; } = null!;
    private static IEnumerable<Diagnostic> AnyAttributesDiagnostics { get; set; } = null!;

    private static ExpectedDiagnostic IllegalAttributeOffsetXOnMudAutocomplete { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(63, 12), new LinePosition(63, 62)),
        "Illegal Attribute 'OffsetX' on 'MudAutocomplete'");

    private static ExpectedDiagnostic IllegalAttributeiconOnMudFab { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(67, 12), new LinePosition(67, 60)),
        "Illegal Attribute 'icon' on 'MudFab'");

    private static ExpectedDiagnostic IllegalAttributeTextOnMudSlider { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(71, 12), new LinePosition(71, 60)),
        "Illegal Attribute 'Text' on 'MudSlider'");

    private static ExpectedDiagnostic IllegalAttributeAvatarOnInheritedMudChip { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(84, 12), new LinePosition(84, 61)),
        "Illegal Attribute 'Avatar' on 'InheritedMudChip'");

    private static ExpectedDiagnostic IllegalAttributeImageOnMudAvatar { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(107, 16), new LinePosition(107, 66)),
        "Illegal Attribute 'Image' on 'MudAvatar'");

    private static ExpectedDiagnostic IllegalAttributeMinimumOnMudProgressLinear { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(113, 12), new LinePosition(113, 63)),
        "Illegal Attribute 'Minimum' on 'MudProgressLinear'");

    private static ExpectedDiagnostic IllegalAttributeDenseOnMudToggleGroup { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(125, 12), new LinePosition(125, 64)),
        "Illegal Attribute 'Dense' on 'MudToggleGroup'");

    private static ExpectedDiagnostic IllegalAttributebindOnMudChip { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(180, 12), new LinePosition(180, 13)),
        "Illegal Attribute '@bind' on 'MudChip'");

    private static ExpectedDiagnostic IllegalAttributebindafterOnMudChip { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(198, 12), new LinePosition(198, 70)),
        "Illegal Attribute '@bind:after' on 'MudChip'");

    private static ExpectedDiagnostic IllegalAttributelowerCaseOnMudProgressCircular { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(228, 12), new LinePosition(228, 66)),
        "Illegal Attribute 'lowerCase' on 'MudProgressCircular'");

    private static ExpectedDiagnostic IllegalAttributeUpperCaseOnMudProgressCircular { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(229, 12), new LinePosition(229, 66)),
        "Illegal Attribute 'UpperCase' on 'MudProgressCircular'");

    private static ExpectedDiagnostic IllegalAttributedataanimationOnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(233, 12), new LinePosition(233, 70)),
        "Illegal Attribute 'data-animation' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributeariadisabledOnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(234, 12), new LinePosition(234, 73)),
        "Illegal Attribute 'aria-disabled' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributeroleOnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(235, 12), new LinePosition(235, 63)),
        "Illegal Attribute 'role' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributeunknownOnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(239, 12), new LinePosition(239, 76)),
        "Illegal Attribute 'unknownAttribute' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributehiddenOnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(243, 12), new LinePosition(243, 62)),
        "Illegal Attribute 'hidden' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributeInertOnMudRadio { get; set; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(247, 12), new LinePosition(247, 62)),
        "Illegal Attribute 'Inert' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributecustomattributeOnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(251, 12), new LinePosition(251, 72)),
        "Illegal Attribute 'customattribute' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributecustomAttribute2OnMudRadio { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(255, 14), new LinePosition(255, 73)),
        "Illegal Attribute 'customAttribute2' on 'MudRadio'");

    private static ExpectedDiagnostic IllegalAttributeErrorTextChangedOnMudCheckBox { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(303, 8), new LinePosition(303, 75)),
        "Illegal Attribute 'RequiredErrorChanged' on 'MudCheckBox'");

    private static ExpectedDiagnostic IllegalAttributeAvatarClassOnMudChip { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(318, 8), new LinePosition(318, 70)),
        "Illegal Attribute 'AvatarClass' on 'MudChip'");

    private static ExpectedDiagnostic IllegalAttributeValueChangedOnMudChip { get; } = new(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
        new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(328, 8), new LinePosition(328, 71)),
        "Illegal Attribute 'ValueChanged' on 'MudChip'");

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        Workspace = await ProjectCompilation.CreateAsync(Util.ProjectPath());
        Workspace.Should().NotBeNull("Workspace null");

        LowerCaseAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase, Workspace.AdditionalTexts));
        DefaultAttributesListDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes, Workspace.AdditionalTexts));
        CustomAttributesListDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes, Workspace.AdditionalTexts, "customattribute,customAttribute2"));
        DataAndAriaAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.DataAndAria, Workspace.AdditionalTexts));
        NoAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None, Workspace.AdditionalTexts));
        AnyAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.Any, Workspace.AdditionalTexts));
    }

    [OneTimeTearDown]
    public static void Cleanup()
    {
        Workspace.Dispose();
    }

    [Test]
    public void AllowLowerCaseAttributes()
    {
        var diagnostics = LowerCaseAttributesDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

        var expectedDiagnostics = new List<ExpectedDiagnostic>([
            IllegalAttributeOffsetXOnMudAutocomplete,
            IllegalAttributeTextOnMudSlider,
            IllegalAttributeAvatarOnInheritedMudChip,
            IllegalAttributeImageOnMudAvatar,
            IllegalAttributeMinimumOnMudProgressLinear,
            IllegalAttributeDenseOnMudToggleGroup,
            IllegalAttributebindOnMudChip,
            IllegalAttributebindafterOnMudChip,
            IllegalAttributeUpperCaseOnMudProgressCircular,
            IllegalAttributeInertOnMudRadio,
            IllegalAttributeErrorTextChangedOnMudCheckBox,
            IllegalAttributeAvatarClassOnMudChip,
            IllegalAttributeValueChangedOnMudChip
        ]);

        ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
    }

    [Test]
    public void AllowDefaultListAttributes()
    {
        var diagnostics = DefaultAttributesListDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

        var expectedDiagnostics = new List<ExpectedDiagnostic>([
            IllegalAttributeOffsetXOnMudAutocomplete,
            IllegalAttributeiconOnMudFab,
            IllegalAttributeTextOnMudSlider,
            IllegalAttributeAvatarOnInheritedMudChip,
            IllegalAttributeImageOnMudAvatar,
            IllegalAttributeMinimumOnMudProgressLinear,
            IllegalAttributeDenseOnMudToggleGroup,
            IllegalAttributebindOnMudChip,
            IllegalAttributebindafterOnMudChip,
            IllegalAttributelowerCaseOnMudProgressCircular,
            IllegalAttributeUpperCaseOnMudProgressCircular,
            IllegalAttributeunknownOnMudRadio,
            IllegalAttributeInertOnMudRadio,
            IllegalAttributecustomattributeOnMudRadio,
            IllegalAttributecustomAttribute2OnMudRadio,
            IllegalAttributeErrorTextChangedOnMudCheckBox,
            IllegalAttributeAvatarClassOnMudChip,
            IllegalAttributeValueChangedOnMudChip]);

        ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
    }

    [Test]
    public void AllowCustomListAttributes()
    {
        var diagnostics = CustomAttributesListDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

        var expectedDiagnostics = new List<ExpectedDiagnostic>([
            IllegalAttributeOffsetXOnMudAutocomplete,
            IllegalAttributeiconOnMudFab,
            IllegalAttributeTextOnMudSlider,
            IllegalAttributeAvatarOnInheritedMudChip,
            IllegalAttributeImageOnMudAvatar,
            IllegalAttributeMinimumOnMudProgressLinear,
            IllegalAttributeDenseOnMudToggleGroup,
            IllegalAttributebindOnMudChip,
            IllegalAttributebindafterOnMudChip,
            IllegalAttributelowerCaseOnMudProgressCircular,
            IllegalAttributeUpperCaseOnMudProgressCircular,
            IllegalAttributeunknownOnMudRadio,
            IllegalAttributehiddenOnMudRadio,
            IllegalAttributeInertOnMudRadio,
            IllegalAttributeErrorTextChangedOnMudCheckBox,
            IllegalAttributeAvatarClassOnMudChip,
            IllegalAttributeValueChangedOnMudChip]);

        ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
    }

    [Test]
    public void AllowDataAndAriaAttributes()
    {
        var diagnostics = DataAndAriaAttributesDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

        var expectedDiagnostics = new List<ExpectedDiagnostic>([
            IllegalAttributeOffsetXOnMudAutocomplete,
            IllegalAttributeiconOnMudFab,
            IllegalAttributeTextOnMudSlider,
            IllegalAttributeAvatarOnInheritedMudChip,
            IllegalAttributeImageOnMudAvatar,
            IllegalAttributeMinimumOnMudProgressLinear,
            IllegalAttributeDenseOnMudToggleGroup,
            IllegalAttributebindOnMudChip,
            IllegalAttributebindafterOnMudChip,
            IllegalAttributelowerCaseOnMudProgressCircular,
            IllegalAttributeUpperCaseOnMudProgressCircular,
            IllegalAttributeunknownOnMudRadio,
            IllegalAttributehiddenOnMudRadio,
            IllegalAttributeInertOnMudRadio,
            IllegalAttributecustomattributeOnMudRadio,
            IllegalAttributecustomAttribute2OnMudRadio,
            IllegalAttributeErrorTextChangedOnMudCheckBox,
            IllegalAttributeAvatarClassOnMudChip,
            IllegalAttributeValueChangedOnMudChip]);

        ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
    }

    [Test]
    public void AllowNoAttributes()
    {
        var diagnostics = NoAttributesDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

        var expectedDiagnostics = new List<ExpectedDiagnostic>([
            IllegalAttributeOffsetXOnMudAutocomplete,
            IllegalAttributeiconOnMudFab,
            IllegalAttributeTextOnMudSlider,
            IllegalAttributeAvatarOnInheritedMudChip,
            IllegalAttributeImageOnMudAvatar,
            IllegalAttributeMinimumOnMudProgressLinear,
            IllegalAttributeDenseOnMudToggleGroup,
            IllegalAttributebindOnMudChip,
            IllegalAttributebindafterOnMudChip,
            IllegalAttributelowerCaseOnMudProgressCircular,
            IllegalAttributeUpperCaseOnMudProgressCircular,
            IllegalAttributedataanimationOnMudRadio,
            IllegalAttributeariadisabledOnMudRadio,
            IllegalAttributeroleOnMudRadio,
            IllegalAttributeunknownOnMudRadio,
            IllegalAttributehiddenOnMudRadio,
            IllegalAttributeInertOnMudRadio,
            IllegalAttributecustomattributeOnMudRadio,
            IllegalAttributecustomAttribute2OnMudRadio,
            IllegalAttributeErrorTextChangedOnMudCheckBox,
            IllegalAttributeAvatarClassOnMudChip,
            IllegalAttributeValueChangedOnMudChip]);

        ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
    }

    [Test]
    public void AllowAnyAttributes()
    {
        var diagnostics = AnyAttributesDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

        var expectedDiagnostics = new List<ExpectedDiagnostic>([]);

        ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
    }
}
#nullable restore
