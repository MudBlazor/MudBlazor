using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using MudBlazor.Analyzers;
using MudBlazor.Analyzers.TestComponents;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers
{
#nullable enable
    [TestFixture]
    public class ValidAttributeTests : BunitTest
    {
        ProjectCompilation Workspace { get; set; } = default!;

        DiagnosticAnalyzer Analyzer { get; set; } = new MudComponentUnknownParametersAnalyzer();

        IEnumerable<Diagnostic> LowerCaseAttributesDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> DefaultAttributesListDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> CustomAttributesListDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> DataAndAriaAttributesDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> NoAttributesDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> AnyAttributesDiagnostics { get; set; } = default!;


        private ExpectedDiagnostic IllegalAttributeOffsetXOnMudAutocomplete { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(63, 12), new LinePosition(63, 62)),
            "Illegal Attribute 'OffsetX' on 'MudAutocomplete'");

        private ExpectedDiagnostic IllegalAttributeiconOnMudFab { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(67, 12), new LinePosition(67, 60)),
            "Illegal Attribute 'icon' on 'MudFab'");

        private ExpectedDiagnostic IllegalAttributeTextOnMudSlider { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(71, 12), new LinePosition(71, 60)),
            "Illegal Attribute 'Text' on 'MudSlider'");

        private ExpectedDiagnostic IllegalAttributeAvatarOnInheritedMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(84, 12), new LinePosition(84, 61)),
            "Illegal Attribute 'Avatar' on 'InheritedMudChip'");

        private ExpectedDiagnostic IllegalAttributeImageOnMudAvatar { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(107, 16), new LinePosition(107, 66)),
            "Illegal Attribute 'Image' on 'MudAvatar'");

        private ExpectedDiagnostic IllegalAttributeMinimumOnMudProgressLinear { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(113, 12), new LinePosition(113, 63)),
            "Illegal Attribute 'Minimum' on 'MudProgressLinear'");

        private ExpectedDiagnostic IllegalAttributeDenseOnMudToggleGroup { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(125, 12), new LinePosition(125, 64)),
            "Illegal Attribute 'Dense' on 'MudToggleGroup'");

        private ExpectedDiagnostic IllegalAttributebindOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(180, 12), new LinePosition(180, 13)),
            "Illegal Attribute '@bind' on 'MudChip'");

        private ExpectedDiagnostic IllegalAttributebindafterOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(198, 12), new LinePosition(198, 70)),
            "Illegal Attribute '@bind:after' on 'MudChip'");

        private ExpectedDiagnostic IllegalAttributelowerCaseOnMudProgressCircular { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(228, 12), new LinePosition(228, 66)),
            "Illegal Attribute 'lowerCase' on 'MudProgressCircular'");

        private ExpectedDiagnostic IllegalAttributeUpperCaseOnMudProgressCircular { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(229, 12), new LinePosition(229, 66)),
            "Illegal Attribute 'UpperCase' on 'MudProgressCircular'");

        private ExpectedDiagnostic IllegalAttributedataanimationOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(233, 12), new LinePosition(233, 70)),
            "Illegal Attribute 'data-animation' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeariadisabledOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(234, 12), new LinePosition(234, 73)),
            "Illegal Attribute 'aria-disabled' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeroleOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(235, 12), new LinePosition(235, 63)),
            "Illegal Attribute 'role' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeunknownOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(239, 12), new LinePosition(239, 76)),
            "Illegal Attribute 'unknownAttribute' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributehiddenOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(243, 12), new LinePosition(243, 62)),
            "Illegal Attribute 'hidden' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeInertOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(247, 12), new LinePosition(247, 62)),
            "Illegal Attribute 'Inert' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributecustomattributeOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(251, 12), new LinePosition(251, 72)),
            "Illegal Attribute 'customattribute' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributecustomAttribute2OnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(255, 14), new LinePosition(255, 73)),
            "Illegal Attribute 'customAttribute2' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeErrorTextChangedOnMudCheckBox { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(303, 8), new LinePosition(303, 75)),
            "Illegal Attribute 'ErrorTextChanged' on 'MudCheckBox'");

        private ExpectedDiagnostic IllegalAttributeAvatarClassOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(318, 8), new LinePosition(318, 70)),
            "Illegal Attribute 'AvatarClass' on 'MudChip'");

        private ExpectedDiagnostic IllegalAttributeValueChangedOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(AttributeTest)}_razor.g.cs", new LinePosition(328, 8), new LinePosition(328, 71)),
            "Illegal Attribute 'ValueChanged' on 'MudChip'");

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            Workspace = await ProjectCompilation.CreateAsync(Util.ProjectPath());
            Workspace.Should().NotBeNull("Workspace null");

            LowerCaseAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(AllowedAttributePattern.LowerCase, Workspace.AdditionalTexts));
            DefaultAttributesListDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(AllowedAttributePattern.HTMLAttributes, Workspace.AdditionalTexts));
            CustomAttributesListDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(AllowedAttributePattern.HTMLAttributes, Workspace.AdditionalTexts, "customattribute,customAttribute2"));
            DataAndAriaAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(AllowedAttributePattern.DataAndAria, Workspace.AdditionalTexts));
            NoAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(AllowedAttributePattern.None, Workspace.AdditionalTexts));
            AnyAttributesDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(AllowedAttributePattern.Any, Workspace.AdditionalTexts));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Workspace?.Dispose();
        }

        [Test]
        [Ignore("https://github.com/MudBlazor/MudBlazor/issues/10869")]
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
        [Ignore("https://github.com/MudBlazor/MudBlazor/issues/10869")]
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
        [Ignore("https://github.com/MudBlazor/MudBlazor/issues/10869")]
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
        [Ignore("https://github.com/MudBlazor/MudBlazor/issues/10869")]
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
        [Ignore("https://github.com/MudBlazor/MudBlazor/issues/10869")]
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
        [Ignore("https://github.com/MudBlazor/MudBlazor/issues/10869")]
        public void AllowAnyAttributes()
        {
            var diagnostics = AnyAttributesDiagnostics.FilterToClass(typeof(AttributeTest).FullName);

            var expectedDiagnostics = new List<ExpectedDiagnostic>([]);

            ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
        }
    }
#nullable restore
}
