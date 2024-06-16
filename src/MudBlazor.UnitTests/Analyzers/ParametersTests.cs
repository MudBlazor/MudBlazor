using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Analyzers;
using NUnit.Framework;
using MudBlazor.UnitTests.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using MudBlazor.Analyzers.TestComponents;

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

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await Task.Yield();
            Workspace = await ProjectCompilation.CreateAsync(Util.ProjectPath());
            Workspace.Should().NotBeNull("Workspace null");

            ParametersV7IgnoreCaseDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.V7IgnoreCase, AllowedAttributePattern.Any, Workspace.AdditionalTexts));
            ParametersV7CaseSensitiveDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.V7CaseSensitive, AllowedAttributePattern.Any, Workspace.AdditionalTexts));

            AttributesLowerCaseDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.Disabled, AllowedAttributePattern.LowerCase, Workspace.AdditionalTexts));
            AttributesDataAndAriaDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.Disabled, AllowedAttributePattern.DataAndAria, Workspace.AdditionalTexts));
            AttributesNoneDiagnostics = await Workspace.GetDiagnosticsAsync([Analyzer], TestAnalyzerOptions.Create(IllegalParameters.Disabled, AllowedAttributePattern.None, Workspace.AdditionalTexts));
        }

        private FileLinePositionSpan IllegalParameter { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(67, 12), new LinePosition(67, 66));
        private FileLinePositionSpan IllegalParameterFullTag { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(72, 12), new LinePosition(72, 67));
        private FileLinePositionSpan IllegalParameterLowerCase { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(77, 12), new LinePosition(77, 68));
        private FileLinePositionSpan IllegalParameterNoValue { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(81, 12), new LinePosition(81, 61));
        private FileLinePositionSpan IllegalParameterNoValueFullTag { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(85, 12), new LinePosition(85, 61));

        private FileLinePositionSpan LowerCaseAttribute { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(90, 12), new LinePosition(90, 66));
        private FileLinePositionSpan UpperCaseAttribute { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(91, 12), new LinePosition(91, 66));

        private FileLinePositionSpan DataAttribute { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(96, 12), new LinePosition(96, 70));
        private FileLinePositionSpan AriaAttribute { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(97, 12), new LinePosition(97, 73));

        private FileLinePositionSpan InheritedAttributeAvatar { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(102, 12), new LinePosition(102, 61));

        private FileLinePositionSpan SubComponentMudAvatarParameter { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(117, 16), new LinePosition(117, 66));

        private FileLinePositionSpan IllegalParameterInCodeSection { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(123, 12), new LinePosition(131, 13));

        private FileLinePositionSpan IllegalParameterInForLoop { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(148, 12), new LinePosition(156, 13));

        private FileLinePositionSpan BindParameter { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(210, 12), new LinePosition(218, 13));
        private FileLinePositionSpan BindAfter { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(220, 12), new LinePosition(220, 70));

        private FileLinePositionSpan TextChangedTypeInference { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(277, 8), new LinePosition(277, 70));
        private FileLinePositionSpan AvatarClassTypeInference { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(284, 8), new LinePosition(284, 70));
        private FileLinePositionSpan ValueChangedTypeInference { get; set; } = new FileLinePositionSpan($"{nameof(ParametersTest)}_razor.g.cs", new LinePosition(286, 8), new LinePosition(286, 71));

        [Test]
        public void ParametersV7IgnoreCase()
        {
            var diagnostics = ParametersV7IgnoreCaseDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedLocations = new List<FileLinePositionSpan>([
                IllegalParameter,
                IllegalParameterFullTag,
                IllegalParameterLowerCase,
                IllegalParameterNoValue,
                IllegalParameterNoValueFullTag,
                InheritedAttributeAvatar,
                SubComponentMudAvatarParameter,
                IllegalParameterInCodeSection,
                IllegalParameterInForLoop,
                AvatarClassTypeInference]);

            diagnostics.Count.Should().Be(expectedLocations.Count);
            diagnostics.CompareLocations(expectedLocations);
        }

        [Test]
        public void ParametersV7CaseSensitive()
        {
            var diagnostics = ParametersV7CaseSensitiveDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedLocations = new List<FileLinePositionSpan>([
                IllegalParameter,
                IllegalParameterFullTag,
                IllegalParameterNoValue,
                IllegalParameterNoValueFullTag,
                InheritedAttributeAvatar,
                SubComponentMudAvatarParameter,
                IllegalParameterInCodeSection,
                IllegalParameterInForLoop,
                AvatarClassTypeInference
                ]);

            diagnostics.Count.Should().Be(expectedLocations.Count);
            diagnostics.CompareLocations(expectedLocations);
        }

        [Test]
        public void AttributesLowerCase()
        {
            var diagnostics = AttributesLowerCaseDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedLocations = new List<FileLinePositionSpan>([
                IllegalParameter,
                IllegalParameterFullTag,
                IllegalParameterNoValue,
                IllegalParameterNoValueFullTag,
                UpperCaseAttribute,
                InheritedAttributeAvatar,
                SubComponentMudAvatarParameter,
                IllegalParameterInCodeSection,
                IllegalParameterInForLoop,
                BindParameter,
                BindAfter,
                TextChangedTypeInference,
                AvatarClassTypeInference,
                ValueChangedTypeInference
                ]);

            diagnostics.Count.Should().Be(expectedLocations.Count);
            diagnostics.CompareLocations(expectedLocations);
        }


        [Test]
        public void AttributesDataAndAria()
        {
            var diagnostics = AttributesDataAndAriaDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedLocations = new List<FileLinePositionSpan>([
                IllegalParameter,
                IllegalParameterFullTag,
                IllegalParameterLowerCase,
                IllegalParameterNoValue,
                IllegalParameterNoValueFullTag,
                LowerCaseAttribute,
                UpperCaseAttribute,
                InheritedAttributeAvatar,
                SubComponentMudAvatarParameter,
                IllegalParameterInCodeSection,
                IllegalParameterInForLoop,
                BindParameter,
                BindAfter,
                TextChangedTypeInference,
                AvatarClassTypeInference,
                ValueChangedTypeInference]);

            diagnostics.Count.Should().Be(expectedLocations.Count);
            diagnostics.CompareLocations(expectedLocations);
        }

        [Test]
        public void AttributesNone()
        {
            var diagnostics = AttributesNoneDiagnostics.FilterToClass(typeof(ParametersTest).FullName);

            var expectedLocations = new List<FileLinePositionSpan>([
                IllegalParameter,
                IllegalParameterFullTag,
                IllegalParameterLowerCase,
                IllegalParameterNoValue,
                IllegalParameterNoValueFullTag,
                LowerCaseAttribute,
                UpperCaseAttribute,
                DataAttribute,
                AriaAttribute,
                InheritedAttributeAvatar,
                SubComponentMudAvatarParameter,
                IllegalParameterInCodeSection,
                IllegalParameterInForLoop,
                BindParameter,
                BindAfter,
                TextChangedTypeInference,
                AvatarClassTypeInference,
                ValueChangedTypeInference]);

            diagnostics.Count.Should().Be(expectedLocations.Count);
            diagnostics.CompareLocations(expectedLocations);
        }

    }
#nullable restore
}
