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

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class ParametersTests : BunitTest
    {
        ProjectCompilation Workspace { get; set; } = default!;

        DiagnosticAnalyzer Analyzer { get; set; } = new MudComponentUnknownParametersAnalyzer();

        IEnumerable<Diagnostic> AttributesLowerCaseDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> AttributesDataAndAriaDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> AttributesNoneDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> ParametersV7IgnoreCaseDiagnostics { get; set; } = default!;
        IEnumerable<Diagnostic> ParametersV7CaseSensitiveDiagnostics { get; set; } = default!;

        private ExpectedDiagnostic IllegalAttributeOffsetXOnMudAutocomplete { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(66, 12), new LinePosition(66, 62)),
            "Illegal Attribute 'OffsetX' on 'MudAutocomplete'");

        private ExpectedDiagnostic IllegalAttributeiconOnMudFab { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(70, 12), new LinePosition(70, 60)),
            "Illegal Attribute 'icon' on 'MudFab'");

        private ExpectedDiagnostic IllegalAttributeTextOnMudSlider { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(74, 12), new LinePosition(74, 60)),
            "Illegal Attribute 'Text' on 'MudSlider'");

        private ExpectedDiagnostic IllegalAttributeAvatarOnInheritedMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(79, 12), new LinePosition(79, 61)),
            "Illegal Attribute 'Avatar' on 'InheritedMudChip'");

        private ExpectedDiagnostic IllegalAttributeImageOnMudAvatar { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(94, 16), new LinePosition(94, 66)),
            "Illegal Attribute 'Image' on 'MudAvatar'");

        private ExpectedDiagnostic IllegalAttributeMinimumOnMudProgressLinear { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(100, 12), new LinePosition(100, 63)),
            "Illegal Attribute 'Minimum' on 'MudProgressLinear'");

        private ExpectedDiagnostic IllegalAttributeDenseOnMudToggleGroup { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(111, 12), new LinePosition(111, 64)),
            "Illegal Attribute 'Dense' on 'MudToggleGroup'");

        private ExpectedDiagnostic IllegalAttributebindOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(165, 12), new LinePosition(173, 13)),
            "Illegal Attribute '@bind' on 'MudChip'");

        private ExpectedDiagnostic IllegalAttributebindafterOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(175, 12), new LinePosition(175, 70)),
            "Illegal Attribute '@bind:after' on 'MudChip'");

        private ExpectedDiagnostic IllegalAttributelowerCaseOnMudProgressCircular { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(205, 12), new LinePosition(205, 66)),
            "Illegal Attribute 'lowerCase' on 'MudProgressCircular'");

        private ExpectedDiagnostic IllegalAttributeUpperCaseOnMudProgressCircular { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(206, 12), new LinePosition(206, 66)),
            "Illegal Attribute 'UpperCase' on 'MudProgressCircular'");

        private ExpectedDiagnostic IllegalAttributedataanimationOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(210, 12), new LinePosition(210, 70)),
            "Illegal Attribute 'data-animation' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeariadisabledOnMudRadio { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(211, 12), new LinePosition(211, 73)),
            "Illegal Attribute 'aria-disabled' on 'MudRadio'");

        private ExpectedDiagnostic IllegalAttributeErrorTextChangedOnMudCheckBox { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(242, 8), new LinePosition(242, 75)),
            "Illegal Attribute 'ErrorTextChanged' on 'MudCheckBox'");

        private ExpectedDiagnostic IllegalAttributeAvatarClassOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(249, 8), new LinePosition(249, 70)),
            "Illegal Attribute 'AvatarClass' on 'MudChip'");

        private ExpectedDiagnostic IllegalAttributeValueChangedOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.AttributeDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(251, 8), new LinePosition(251, 71)),
            "Illegal Attribute 'ValueChanged' on 'MudChip'");

        private ExpectedDiagnostic IllegalParameterOffsetXOnMudAutocomplete { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(66, 12), new LinePosition(66, 62)),
            "Illegal Parameter 'OffsetX' on 'MudAutocomplete'");

        private ExpectedDiagnostic IllegalParametericonOnMudFab { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(70, 12), new LinePosition(70, 60)),
            "Illegal Parameter 'icon' on 'MudFab'");

        private ExpectedDiagnostic IllegalParameterTextOnMudSlider { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(74, 12), new LinePosition(74, 60)),
            "Illegal Parameter 'Text' on 'MudSlider'");

        private ExpectedDiagnostic IllegalParameterAvatarOnInheritedMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(79, 12), new LinePosition(79, 61)),
            "Illegal Parameter 'Avatar' on 'InheritedMudChip'");

        private ExpectedDiagnostic IllegalParameterImageOnMudAvatar { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(94, 16), new LinePosition(94, 66)),
            "Illegal Parameter 'Image' on 'MudAvatar'");

        private ExpectedDiagnostic IllegalParameterMinimumOnMudProgressLinear { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(100, 12), new LinePosition(100, 63)),
            "Illegal Parameter 'Minimum' on 'MudProgressLinear'");

        private ExpectedDiagnostic IllegalParameterDenseOnMudToggleGroup { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(111, 12), new LinePosition(111, 64)),
            "Illegal Parameter 'Dense' on 'MudToggleGroup'");

        private ExpectedDiagnostic IllegalParameterAvatarClassOnMudChip { get; set; } = new ExpectedDiagnostic(MudComponentUnknownParametersAnalyzer.ParameterDescriptor,
            new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(249, 8), new LinePosition(249, 70)),
            "Illegal Parameter 'AvatarClass' on 'MudChip'");

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            Workspace = await ProjectCompilation.CreateAsync(Util.ProjectPath());
            Workspace.Should().NotBeNull("Workspace null");

            ParametersV7IgnoreCaseDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.V7IgnoreCase, AllowedAttributePattern.Any, Workspace.AdditionalTexts));
            ParametersV7CaseSensitiveDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.V7CaseSensitive, AllowedAttributePattern.Any, Workspace.AdditionalTexts));

            AttributesLowerCaseDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.Disabled, AllowedAttributePattern.LowerCase, Workspace.AdditionalTexts));
            AttributesDataAndAriaDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.Disabled, AllowedAttributePattern.DataAndAria, Workspace.AdditionalTexts));
            AttributesNoneDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.Disabled, AllowedAttributePattern.None, Workspace.AdditionalTexts));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Workspace?.Dispose();
        }

        [Test]
        public void ParametersV7IgnoreCase()
        {
            var diagnostics = ParametersV7IgnoreCaseDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedDiagnostics = new List<ExpectedDiagnostic>([
                IllegalParameterOffsetXOnMudAutocomplete,
                IllegalParametericonOnMudFab,
                IllegalParameterTextOnMudSlider,
                IllegalParameterAvatarOnInheritedMudChip,
                IllegalParameterImageOnMudAvatar,
                IllegalParameterMinimumOnMudProgressLinear,
                IllegalParameterDenseOnMudToggleGroup,
                IllegalParameterAvatarClassOnMudChip]);

            diagnostics.Count.Should().Be(expectedDiagnostics.Count);
            ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
        }

        [Test]
        public void ParametersV7CaseSensitive()
        {
            var diagnostics = ParametersV7CaseSensitiveDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedDiagnostics = new List<ExpectedDiagnostic>([
                IllegalParameterOffsetXOnMudAutocomplete,
                IllegalParameterTextOnMudSlider,
                IllegalParameterAvatarOnInheritedMudChip,
                IllegalParameterImageOnMudAvatar,
                IllegalParameterMinimumOnMudProgressLinear,
                IllegalParameterDenseOnMudToggleGroup,
                IllegalParameterAvatarClassOnMudChip
                ]);

            diagnostics.Count.Should().Be(expectedDiagnostics.Count);
            ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
        }

        [Test]
        public void AttributesLowerCase()
        {
            var diagnostics = AttributesLowerCaseDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

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
                IllegalAttributeErrorTextChangedOnMudCheckBox,
                IllegalAttributeAvatarClassOnMudChip,
                IllegalAttributeValueChangedOnMudChip
            ]);

            diagnostics.Count.Should().Be(expectedDiagnostics.Count);
            ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
        }

        [Test]
        public void AttributesDataAndAria()
        {
            var diagnostics = AttributesDataAndAriaDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

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
                IllegalAttributeErrorTextChangedOnMudCheckBox,
                IllegalAttributeAvatarClassOnMudChip,
                IllegalAttributeValueChangedOnMudChip]);

            diagnostics.Count.Should().Be(expectedDiagnostics.Count);
            ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
        }

        [Test]
        public void AttributesNone()
        {
            var diagnostics = AttributesNoneDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

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
                IllegalAttributeErrorTextChangedOnMudCheckBox,
                IllegalAttributeAvatarClassOnMudChip,
                IllegalAttributeValueChangedOnMudChip]);

            diagnostics.Count.Should().Be(expectedDiagnostics.Count);
            ExpectedDiagnostic.Compare(diagnostics, expectedDiagnostics);
        }
    }
#nullable restore
}
