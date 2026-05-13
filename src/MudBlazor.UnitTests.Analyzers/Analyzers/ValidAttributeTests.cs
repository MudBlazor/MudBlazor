using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;

#nullable enable
[TestFixture]
public class ValidAttributeTests : BunitTest
{
    private const string AttributeTestClassName = "MudBlazor.Analyzers.TestInputs.AttributeTest";
    private const string SecondaryAttributeTestClassName = "MudBlazor.Analyzers.TestInputs.SecondaryAttributeTest";

    private static readonly DiagnosticExpectation[] _coreDiagnostics =
    [
        new("OffsetX", "MudAutocomplete"),
        new("Text", "MudSlider"),
        new("Avatar", "InheritedMudChip"),
        new("Image", "MudAvatar"),
        new("Minimum", "MudProgressLinear"),
        new("Dense", "MudToggleGroup"),
        new("RequiredErrorChanged", "MudCheckBox"),
        new("AvatarClass", "MudChip"),
        new("ValueChanged", "MudChip")
    ];

    private static readonly DiagnosticExpectation[] _lowerCaseDiagnostics =
    [
        .. _coreDiagnostics,
        new("UpperCase", "MudProgressCircular"),
        new("Inert", "MudRadio")
    ];

    private static readonly DiagnosticExpectation[] _defaultAttributesDiagnostics =
    [
        .. _coreDiagnostics,
        new("lowerCase", "MudProgressCircular"),
        new("UpperCase", "MudProgressCircular"),
        new("unknownAttribute", "MudRadio"),
        new("Inert", "MudRadio"),
        new("customattribute", "MudRadio"),
        new("customAttribute2", "MudRadio")
    ];

    private static readonly DiagnosticExpectation[] _customAttributesDiagnostics =
    [
        .. _coreDiagnostics,
        new("lowerCase", "MudProgressCircular"),
        new("UpperCase", "MudProgressCircular"),
        new("unknownAttribute", "MudRadio"),
        new("hidden", "MudRadio"),
        new("Inert", "MudRadio")
    ];

    private static readonly DiagnosticExpectation[] _dataAndAriaDiagnostics =
    [
        .. _coreDiagnostics,
        new("lowerCase", "MudProgressCircular"),
        new("UpperCase", "MudProgressCircular"),
        new("unknownAttribute", "MudRadio"),
        new("hidden", "MudRadio"),
        new("Inert", "MudRadio"),
        new("customattribute", "MudRadio"),
        new("customAttribute2", "MudRadio")
    ];

    private static readonly DiagnosticExpectation[] _noAttributesDiagnostics =
    [
        .. _coreDiagnostics,
        new("lowerCase", "MudProgressCircular"),
        new("UpperCase", "MudProgressCircular"),
        new("data-animation", "MudRadio"),
        new("aria-disabled", "MudRadio"),
        new("role", "MudRadio"),
        new("unknownAttribute", "MudRadio"),
        new("hidden", "MudRadio"),
        new("Inert", "MudRadio"),
        new("customattribute", "MudRadio"),
        new("customAttribute2", "MudRadio")
    ];

    private static IEnumerable<Diagnostic> LowerCaseAttributesDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> DefaultAttributesListDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> CustomAttributesListDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> DataAndAriaAttributesDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> NoAttributesDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> AnyAttributesDiagnostics { get; set; } = null!;

    private static IEnumerable<Diagnostic> MappedLocationDiagnostics { get; set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        LowerCaseAttributesDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase);

        DefaultAttributesListDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes);

        CustomAttributesListDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes,
            "customattribute,customAttribute2");

        DataAndAriaAttributesDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.DataAndAria);

        NoAttributesDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None);

        AnyAttributesDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateGeneratedSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.Any);

        MappedLocationDiagnostics = await AnalyzerCompilationFactory.GetDiagnosticsAsync(
            CreateMappedLocationSource(),
            MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None,
            sourcePath: "MappedAttributeTest.razor.g.cs");
    }

    [Test]
    public void AllowLowerCaseAttributes()
    {
        AssertDiagnostics(
            LowerCaseAttributesDiagnostics.FilterToClass(AttributeTestClassName),
            _lowerCaseDiagnostics);
    }

    [Test]
    public void AllowDefaultListAttributes()
    {
        AssertDiagnostics(
            DefaultAttributesListDiagnostics.FilterToClass(AttributeTestClassName),
            _defaultAttributesDiagnostics);
    }

    [Test]
    public void AllowCustomListAttributes()
    {
        AssertDiagnostics(
            CustomAttributesListDiagnostics.FilterToClass(AttributeTestClassName),
            _customAttributesDiagnostics);
    }

    [Test]
    public void AllowDataAndAriaAttributes()
    {
        AssertDiagnostics(
            DataAndAriaAttributesDiagnostics.FilterToClass(AttributeTestClassName),
            _dataAndAriaDiagnostics);
    }

    [Test]
    public void AllowNoAttributes()
    {
        AssertDiagnostics(
            NoAttributesDiagnostics.FilterToClass(AttributeTestClassName),
            _noAttributesDiagnostics);
    }

    [Test]
    public void AllowAnyAttributes()
    {
        AnyAttributesDiagnostics.Should().BeEmpty();
    }

    [Test]
    public void FilterToClassUsesDiagnosticClassNameProperty()
    {
        var attributeTestDiagnostics = NoAttributesDiagnostics.FilterToClass(AttributeTestClassName);
        var secondaryDiagnostics = NoAttributesDiagnostics.FilterToClass(SecondaryAttributeTestClassName);

        attributeTestDiagnostics.Should().HaveCount(_noAttributesDiagnostics.Length);
        secondaryDiagnostics.Should().ContainSingle();
        secondaryDiagnostics[0].Properties.Should().ContainKey(
            MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey);
        secondaryDiagnostics[0].Properties[MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey]
            .Should().Be(SecondaryAttributeTestClassName);
        secondaryDiagnostics[0].GetMessage().Should().StartWith("Illegal Attribute 'SecondaryOnly' on 'MudRadio'");
    }

    [Test]
    public void UsesMappedRazorLocationWhenChecksumPragmaExists()
    {
        var diagnostic = MappedLocationDiagnostics.Should().ContainSingle().Subject;

        diagnostic.Id.Should().Be(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.DiagnosticId);
        diagnostic.Location.GetLineSpan().Path.Should().Be("MappedAttributeTest.razor");
        diagnostic.AdditionalLocations.Should().ContainSingle();
        diagnostic.AdditionalLocations[0].GetLineSpan().Path.Should().Be("MappedAttributeTest.razor.g.cs");
        diagnostic.Properties[MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey]
            .Should().Be(AttributeTestClassName);
        diagnostic.GetMessage().Should().StartWith("Illegal Attribute 'OffsetX' on 'MudAutocomplete'");
    }

    private static void AssertDiagnostics(
        IReadOnlyList<Diagnostic> diagnostics,
        IReadOnlyList<DiagnosticExpectation> expectations)
    {
        diagnostics.Should().HaveCount(expectations.Count);

        foreach (var diagnostic in diagnostics)
        {
            diagnostic.Id.Should().Be(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.DiagnosticId);
            diagnostic.Properties[MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey]
                .Should().Be(AttributeTestClassName);
        }

        foreach (var expectation in expectations)
        {
            diagnostics.Should().ContainSingle(x =>
                x.GetMessage().StartsWith(
                    $"Illegal Attribute '{expectation.AttributeName}' on '{expectation.ComponentName}'",
                    StringComparison.Ordinal));
        }
    }

    private static string CreateGeneratedSource() =>
        """
        using System;
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Components.Rendering;

        namespace MudBlazor
        {
            public abstract class MudComponentBase : ComponentBase
            {
            }

            public class MudAutocomplete<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
            }

            public class MudFab : MudComponentBase
            {
                [Parameter] public string? Icon { get; set; }
            }

            public class MudSlider<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
            }

            public class MudChipBase : MudComponentBase
            {
                [Parameter] public string? AvatarClass { get; set; }
            }

            public class InheritedMudChip : MudChipBase
            {
                [Parameter] public string? Text { get; set; }
            }

            public class MudAvatar : MudComponentBase
            {
                [Parameter] public string? Alt { get; set; }
            }

            public class MudProgressLinear : MudComponentBase
            {
                [Parameter] public int Max { get; set; }
            }

            public class MudToggleGroup<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
            }

            public class MudProgressCircular : MudComponentBase
            {
                [Parameter] public bool Indeterminate { get; set; }
            }

            public class MudRadio<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
            }

            public class MudCheckBox<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
                [Parameter] public T? RequiredError { get; set; }
            }

            public class MudChip<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
                [Parameter] public string? Text { get; set; }
            }
        }

        namespace MudBlazor.Analyzers.TestInputs
        {
            public class AttributeTest : ComponentBase
            {
                private readonly string _bindValue = "y";

                protected override void BuildRenderTree(RenderTreeBuilder builder)
                {
                    builder.OpenComponent<MudBlazor.MudAutocomplete<string>>(0);
                    builder.AddAttribute(1, "Value", _bindValue);
                    builder.AddAttribute(2, "OffsetX", "5");
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudFab>(3);
                    builder.AddAttribute(4, "icon", "dd");
                    builder.AddAttribute(5, "MudFab", true);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudSlider<int>>(6);
                    builder.AddAttribute(7, "Text", true);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.InheritedMudChip>(8);
                    builder.AddAttribute(9, "Text", "Href set");
                    builder.AddAttribute(10, "AvatarClass", _bindValue);
                    builder.AddAttribute(11, "Avatar", string.Empty);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudAvatar>(12);
                    builder.AddAttribute(13, "Image", "avatar.png");
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudProgressLinear>(14);
                    builder.AddAttribute(15, "Minimum", 0);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudToggleGroup<string>>(16);
                    builder.AddAttribute(17, "Dense", true);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudCheckBox<string>>(18);
                    builder.AddAttribute(19, "RequiredError", _bindValue);
                    builder.AddComponentParameter(20, "RequiredErrorChanged", _bindValue);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudProgressCircular>(21);
                    builder.AddAttribute(22, "lowerCase", true);
                    builder.AddAttribute(23, "UpperCase", true);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudRadio<string>>(24);
                    builder.AddAttribute(25, "data-animation", "a");
                    builder.AddAttribute(26, "aria-disabled", "false");
                    builder.AddAttribute(27, "role", "test");
                    builder.AddAttribute(28, "unknownAttribute", "false");
                    builder.AddAttribute(29, "hidden", true);
                    builder.AddAttribute(30, "Inert", true);
                    builder.AddAttribute(31, "customattribute", true);
                    builder.AddAttribute(32, "customAttribute2", true);
                    builder.CloseComponent();

                    builder.OpenComponent<MudBlazor.MudChip<string>>(33);
                    builder.AddAttribute(34, "Text", "Href set");
                    builder.AddAttribute(35, "AvatarClass", _bindValue);
                    builder.CloseComponent();

                    TypeInference.CreateMudChip_0(builder, 36, _bindValue, After);
                }

                private void After()
                {
                }
            }

            public class SecondaryAttributeTest : ComponentBase
            {
                protected override void BuildRenderTree(RenderTreeBuilder builder)
                {
                    builder.OpenComponent<MudBlazor.MudRadio<string>>(0);
                    builder.AddAttribute(1, "SecondaryOnly", true);
                    builder.CloseComponent();
                }
            }

            public static class TypeInference
            {
                public static void CreateMudChip_0(RenderTreeBuilder builder, int sequence, string value, Action after)
                {
                    builder.OpenComponent<MudBlazor.MudChip<string>>(sequence);
                    builder.AddAttribute(sequence + 1, "Value", value);
                    builder.AddAttribute(sequence + 2, "ValueChanged", after);
                    builder.CloseComponent();
                }
            }
        }
        """;

    private static string CreateMappedLocationSource() =>
        """
        // The checksum payload is intentionally synthetic; only the mapped Razor path matters for this test.
        #pragma checksum "MappedAttributeTest.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1234567890ABCDEF1234567890ABCDEF12345678"
        using Microsoft.AspNetCore.Components;
        using Microsoft.AspNetCore.Components.Rendering;

        namespace MudBlazor
        {
            public abstract class MudComponentBase : ComponentBase
            {
            }

            public class MudAutocomplete<T> : MudComponentBase
            {
                [Parameter] public T? Value { get; set; }
            }
        }

        namespace MudBlazor.Analyzers.TestInputs
        {
            public class AttributeTest : ComponentBase
            {
                protected override void BuildRenderTree(RenderTreeBuilder builder)
                {
                    builder.OpenComponent<MudBlazor.MudAutocomplete<string>>(0);
                    builder.AddAttribute(1, "OffsetX", "5");
                    builder.CloseComponent();
                }
            }
        }
        """;

    private sealed record DiagnosticExpectation(string AttributeName, string ComponentName);
}
#nullable restore
