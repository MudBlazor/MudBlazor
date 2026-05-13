// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using MudBlazor.UnitTests.Analyzers.Internal;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Analyzers;

extern alias MudBlazorAnalyzer;

#nullable enable
[TestFixture]
public class ValidAttributeTests
{
    private const string GeneratedComponentTypeName = "MudBlazor.UnitTests.Analyzers.Generated.GeneratedAttributeTestComponent";

    private static IReadOnlyList<Diagnostic> LowerCaseAttributesDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> DefaultAttributesListDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> CustomAttributesListDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> DataAndAriaAttributesDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> NoAttributesDiagnostics { get; set; } = null!;

    private static IReadOnlyList<Diagnostic> AnyAttributesDiagnostics { get; set; } = null!;

    private static IReadOnlyList<ExpectedAttributeDiagnostic> LowerCaseExpectedDiagnostics { get; } =
    [
        new("OffsetX", "MudAutocomplete", "LowerCase", "AddAttribute(1, \"OffsetX\""),
        new("Text", "MudSlider", "LowerCase", "AddAttribute(5, \"Text\""),
        new("Avatar", "InheritedMudChip", "LowerCase", "AddAttribute(8, \"Avatar\""),
        new("Image", "MudAvatar", "LowerCase", "AddComponentParameter(11, \"Image\""),
        new("Minimum", "MudProgressLinear", "LowerCase", "AddAttribute(13, \"Minimum\""),
        new("Dense", "MudToggleGroup", "LowerCase", "AddAttribute(15, \"Dense\""),
        new("@bind", "MudChip", "LowerCase", "AddAttribute(35, \"@bind\""),
        new("@bind:after", "MudChip", "LowerCase", "AddAttribute(36, \"@bind:after\""),
        new("UpperCase", "MudProgressCircular", "LowerCase", "AddAttribute(18, \"UpperCase\""),
        new("Inert", "MudRadio", "LowerCase", "AddAttribute(25, \"Inert\""),
        new("RequiredErrorChanged", "MudCheckBox", "LowerCase", "AddAttribute(29, \"RequiredErrorChanged\""),
        new("AvatarClass", "MudChip", "LowerCase", "AddAttribute(32, \"AvatarClass\""),
        new("ValueChanged", "MudChip", "LowerCase", "AddAttribute(33, \"ValueChanged\"")
    ];

    private static IReadOnlyList<ExpectedAttributeDiagnostic> DefaultListExpectedDiagnostics { get; } =
    [
        new("OffsetX", "MudAutocomplete", "HTMLAttributes", "AddAttribute(1, \"OffsetX\""),
        new("icon", "MudFab", "HTMLAttributes", "AddAttribute(3, \"icon\""),
        new("Text", "MudSlider", "HTMLAttributes", "AddAttribute(5, \"Text\""),
        new("Avatar", "InheritedMudChip", "HTMLAttributes", "AddAttribute(8, \"Avatar\""),
        new("Image", "MudAvatar", "HTMLAttributes", "AddComponentParameter(11, \"Image\""),
        new("Minimum", "MudProgressLinear", "HTMLAttributes", "AddAttribute(13, \"Minimum\""),
        new("Dense", "MudToggleGroup", "HTMLAttributes", "AddAttribute(15, \"Dense\""),
        new("@bind", "MudChip", "HTMLAttributes", "AddAttribute(35, \"@bind\""),
        new("@bind:after", "MudChip", "HTMLAttributes", "AddAttribute(36, \"@bind:after\""),
        new("lowerCase", "MudProgressCircular", "HTMLAttributes", "AddAttribute(17, \"lowerCase\""),
        new("UpperCase", "MudProgressCircular", "HTMLAttributes", "AddAttribute(18, \"UpperCase\""),
        new("unknownAttribute", "MudRadio", "HTMLAttributes", "AddAttribute(23, \"unknownAttribute\""),
        new("Inert", "MudRadio", "HTMLAttributes", "AddAttribute(25, \"Inert\""),
        new("customattribute", "MudRadio", "HTMLAttributes", "AddAttribute(26, \"customattribute\""),
        new("customAttribute2", "MudRadio", "HTMLAttributes", "AddAttribute(27, \"customAttribute2\""),
        new("RequiredErrorChanged", "MudCheckBox", "HTMLAttributes", "AddAttribute(29, \"RequiredErrorChanged\""),
        new("AvatarClass", "MudChip", "HTMLAttributes", "AddAttribute(32, \"AvatarClass\""),
        new("ValueChanged", "MudChip", "HTMLAttributes", "AddAttribute(33, \"ValueChanged\"")
    ];

    private static IReadOnlyList<ExpectedAttributeDiagnostic> CustomListExpectedDiagnostics { get; } =
    [
        new("OffsetX", "MudAutocomplete", "HTMLAttributes", "AddAttribute(1, \"OffsetX\""),
        new("icon", "MudFab", "HTMLAttributes", "AddAttribute(3, \"icon\""),
        new("Text", "MudSlider", "HTMLAttributes", "AddAttribute(5, \"Text\""),
        new("Avatar", "InheritedMudChip", "HTMLAttributes", "AddAttribute(8, \"Avatar\""),
        new("Image", "MudAvatar", "HTMLAttributes", "AddComponentParameter(11, \"Image\""),
        new("Minimum", "MudProgressLinear", "HTMLAttributes", "AddAttribute(13, \"Minimum\""),
        new("Dense", "MudToggleGroup", "HTMLAttributes", "AddAttribute(15, \"Dense\""),
        new("@bind", "MudChip", "HTMLAttributes", "AddAttribute(35, \"@bind\""),
        new("@bind:after", "MudChip", "HTMLAttributes", "AddAttribute(36, \"@bind:after\""),
        new("lowerCase", "MudProgressCircular", "HTMLAttributes", "AddAttribute(17, \"lowerCase\""),
        new("UpperCase", "MudProgressCircular", "HTMLAttributes", "AddAttribute(18, \"UpperCase\""),
        new("unknownAttribute", "MudRadio", "HTMLAttributes", "AddAttribute(23, \"unknownAttribute\""),
        new("hidden", "MudRadio", "HTMLAttributes", "AddAttribute(24, \"hidden\""),
        new("Inert", "MudRadio", "HTMLAttributes", "AddAttribute(25, \"Inert\""),
        new("RequiredErrorChanged", "MudCheckBox", "HTMLAttributes", "AddAttribute(29, \"RequiredErrorChanged\""),
        new("AvatarClass", "MudChip", "HTMLAttributes", "AddAttribute(32, \"AvatarClass\""),
        new("ValueChanged", "MudChip", "HTMLAttributes", "AddAttribute(33, \"ValueChanged\"")
    ];

    private static IReadOnlyList<ExpectedAttributeDiagnostic> DataAndAriaExpectedDiagnostics { get; } =
    [
        new("OffsetX", "MudAutocomplete", "DataAndAria", "AddAttribute(1, \"OffsetX\""),
        new("icon", "MudFab", "DataAndAria", "AddAttribute(3, \"icon\""),
        new("Text", "MudSlider", "DataAndAria", "AddAttribute(5, \"Text\""),
        new("Avatar", "InheritedMudChip", "DataAndAria", "AddAttribute(8, \"Avatar\""),
        new("Image", "MudAvatar", "DataAndAria", "AddComponentParameter(11, \"Image\""),
        new("Minimum", "MudProgressLinear", "DataAndAria", "AddAttribute(13, \"Minimum\""),
        new("Dense", "MudToggleGroup", "DataAndAria", "AddAttribute(15, \"Dense\""),
        new("@bind", "MudChip", "DataAndAria", "AddAttribute(35, \"@bind\""),
        new("@bind:after", "MudChip", "DataAndAria", "AddAttribute(36, \"@bind:after\""),
        new("lowerCase", "MudProgressCircular", "DataAndAria", "AddAttribute(17, \"lowerCase\""),
        new("UpperCase", "MudProgressCircular", "DataAndAria", "AddAttribute(18, \"UpperCase\""),
        new("unknownAttribute", "MudRadio", "DataAndAria", "AddAttribute(23, \"unknownAttribute\""),
        new("hidden", "MudRadio", "DataAndAria", "AddAttribute(24, \"hidden\""),
        new("Inert", "MudRadio", "DataAndAria", "AddAttribute(25, \"Inert\""),
        new("customattribute", "MudRadio", "DataAndAria", "AddAttribute(26, \"customattribute\""),
        new("customAttribute2", "MudRadio", "DataAndAria", "AddAttribute(27, \"customAttribute2\""),
        new("RequiredErrorChanged", "MudCheckBox", "DataAndAria", "AddAttribute(29, \"RequiredErrorChanged\""),
        new("AvatarClass", "MudChip", "DataAndAria", "AddAttribute(32, \"AvatarClass\""),
        new("ValueChanged", "MudChip", "DataAndAria", "AddAttribute(33, \"ValueChanged\"")
    ];

    private static IReadOnlyList<ExpectedAttributeDiagnostic> NoAttributesExpectedDiagnostics { get; } =
    [
        new("OffsetX", "MudAutocomplete", "None", "AddAttribute(1, \"OffsetX\""),
        new("icon", "MudFab", "None", "AddAttribute(3, \"icon\""),
        new("Text", "MudSlider", "None", "AddAttribute(5, \"Text\""),
        new("Avatar", "InheritedMudChip", "None", "AddAttribute(8, \"Avatar\""),
        new("Image", "MudAvatar", "None", "AddComponentParameter(11, \"Image\""),
        new("Minimum", "MudProgressLinear", "None", "AddAttribute(13, \"Minimum\""),
        new("Dense", "MudToggleGroup", "None", "AddAttribute(15, \"Dense\""),
        new("@bind", "MudChip", "None", "AddAttribute(35, \"@bind\""),
        new("@bind:after", "MudChip", "None", "AddAttribute(36, \"@bind:after\""),
        new("lowerCase", "MudProgressCircular", "None", "AddAttribute(17, \"lowerCase\""),
        new("UpperCase", "MudProgressCircular", "None", "AddAttribute(18, \"UpperCase\""),
        new("data-animation", "MudRadio", "None", "AddAttribute(20, \"data-animation\""),
        new("aria-disabled", "MudRadio", "None", "AddAttribute(21, \"aria-disabled\""),
        new("role", "MudRadio", "None", "AddAttribute(22, \"role\""),
        new("unknownAttribute", "MudRadio", "None", "AddAttribute(23, \"unknownAttribute\""),
        new("hidden", "MudRadio", "None", "AddAttribute(24, \"hidden\""),
        new("Inert", "MudRadio", "None", "AddAttribute(25, \"Inert\""),
        new("customattribute", "MudRadio", "None", "AddAttribute(26, \"customattribute\""),
        new("customAttribute2", "MudRadio", "None", "AddAttribute(27, \"customAttribute2\""),
        new("RequiredErrorChanged", "MudCheckBox", "None", "AddAttribute(29, \"RequiredErrorChanged\""),
        new("AvatarClass", "MudChip", "None", "AddAttribute(32, \"AvatarClass\""),
        new("ValueChanged", "MudChip", "None", "AddAttribute(33, \"ValueChanged\"")
    ];

    private static string GeneratedComponentSource =>
        """
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;

namespace MudBlazor.UnitTests.Analyzers.Generated;

public partial class GeneratedAttributeTestComponent : ComponentBase
{
    private string _bindValue = "y";

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        __builder.OpenComponent<MudAutocomplete<string>>(0);
        __builder.AddAttribute(1, "OffsetX", 5);
        __builder.CloseComponent();

        __builder.OpenComponent<MudFab>(2);
        __builder.AddAttribute(3, "icon", "dd");
        __builder.CloseComponent();

        __builder.OpenComponent<MudSlider<int>>(4);
        __builder.AddAttribute(5, "Text", true);
        __builder.CloseComponent();

        __builder.OpenComponent<InheritedMudChip<string>>(6);
        __builder.AddAttribute(7, "Text", "Href set");
        __builder.AddAttribute(8, "Avatar", string.Empty);
        __builder.AddAttribute(9, "AvatarClass", _bindValue);
        __builder.CloseComponent();

        __builder.OpenComponent<MudAvatar>(10);
        __builder.AddComponentParameter(11, "Image", "y");
        __builder.CloseComponent();

        __builder.OpenComponent<MudProgressLinear>(12);
        __builder.AddAttribute(13, "Minimum", 0);
        __builder.CloseComponent();

        __builder.OpenComponent<MudToggleGroup<string>>(14);
        __builder.AddAttribute(15, "Dense", true);
        __builder.CloseComponent();

        TypeInference.CreateMudChip_0(__builder, _bindValue, Test);

        __builder.OpenComponent<MudProgressCircular>(16);
        __builder.AddAttribute(17, "lowerCase", true);
        __builder.AddAttribute(18, "UpperCase", true);
        __builder.CloseComponent();

        __builder.OpenComponent<MudRadio<string>>(19);
        __builder.AddAttribute(20, "data-animation", "a");
        __builder.AddAttribute(21, "aria-disabled", "false");
        __builder.AddAttribute(22, "role", "test");
        __builder.AddAttribute(23, "unknownAttribute", "false");
        __builder.AddAttribute(24, "hidden", true);
        __builder.AddAttribute(25, "Inert", true);
        __builder.AddAttribute(26, "customattribute", true);
        __builder.AddAttribute(27, "customAttribute2", true);
        __builder.CloseComponent();

        __builder.OpenComponent<MudCheckBox<bool>>(28);
        __builder.AddAttribute(29, "RequiredErrorChanged", default(EventCallback<string>));
        __builder.CloseComponent();

        __builder.OpenComponent<MudChip<string>>(30);
        __builder.AddAttribute(31, "Text", "Href set");
        __builder.AddAttribute(32, "AvatarClass", _bindValue);
        __builder.AddAttribute(33, "ValueChanged", default(EventCallback<string>));
        __builder.CloseComponent();
    }

    private Task Test() => Task.CompletedTask;

    internal static class TypeInference
    {
        public static void CreateMudChip_0(RenderTreeBuilder __builder, string value, Func<Task> after)
        {
            __builder.OpenComponent<MudChip<string>>(34);
            __builder.AddAttribute(35, "@bind", value);
            __builder.AddAttribute(36, "@bind:after", after);
            __builder.AddAttribute(37, "Text", "Href set");
            __builder.CloseComponent();
        }
    }
}

public class InheritedMudChip<T> : MudChip<T>
{
    [Parameter]
    public string? AvatarClass { get; set; }
}
""";

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        LowerCaseAttributesDiagnostics = await MudComponentUnknownParametersAnalyzerFixture.RunAsync(GeneratedComponentSource, MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.LowerCase);
        DefaultAttributesListDiagnostics = await MudComponentUnknownParametersAnalyzerFixture.RunAsync(GeneratedComponentSource, MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes);
        CustomAttributesListDiagnostics = await MudComponentUnknownParametersAnalyzerFixture.RunAsync(GeneratedComponentSource, MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.HTMLAttributes, "customattribute,customAttribute2");
        DataAndAriaAttributesDiagnostics = await MudComponentUnknownParametersAnalyzerFixture.RunAsync(GeneratedComponentSource, MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.DataAndAria);
        NoAttributesDiagnostics = await MudComponentUnknownParametersAnalyzerFixture.RunAsync(GeneratedComponentSource, MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.None);
        AnyAttributesDiagnostics = await MudComponentUnknownParametersAnalyzerFixture.RunAsync(GeneratedComponentSource, MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern.Any);
    }

    [Test]
    public void AllowLowerCaseAttributes()
    {
        AssertDiagnostics(LowerCaseAttributesDiagnostics, LowerCaseExpectedDiagnostics);
    }

    [Test]
    public void AllowDefaultListAttributes()
    {
        AssertDiagnostics(DefaultAttributesListDiagnostics, DefaultListExpectedDiagnostics);
    }

    [Test]
    public void AllowCustomListAttributes()
    {
        AssertDiagnostics(CustomAttributesListDiagnostics, CustomListExpectedDiagnostics);
    }

    [Test]
    public void AllowDataAndAriaAttributes()
    {
        AssertDiagnostics(DataAndAriaAttributesDiagnostics, DataAndAriaExpectedDiagnostics);
    }

    [Test]
    public void AllowNoAttributes()
    {
        AssertDiagnostics(NoAttributesDiagnostics, NoAttributesExpectedDiagnostics);
    }

    [Test]
    public void AllowAnyAttributes()
    {
        AnyAttributesDiagnostics.Should().BeEmpty();
    }

    private static void AssertDiagnostics(IReadOnlyList<Diagnostic> diagnostics, IReadOnlyList<ExpectedAttributeDiagnostic> expectedDiagnostics)
    {
        var filteredDiagnostics = diagnostics.FilterToClass(GeneratedComponentTypeName);
        var orderedExpectedDiagnostics = expectedDiagnostics
            .OrderBy(x => GeneratedComponentSource.IndexOf(x.SourceMarker, StringComparison.Ordinal))
            .ToArray();

        filteredDiagnostics.Should().HaveCount(orderedExpectedDiagnostics.Length);

        for (var i = 0; i < filteredDiagnostics.Count; i++)
        {
            var actual = filteredDiagnostics[i];
            var expected = orderedExpectedDiagnostics[i];
            var sourceText = actual.AdditionalLocations[0].SourceTree!.GetText();

            actual.Id.Should().Be(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.DiagnosticId);
            actual.GetMessage().Should().Contain($"Illegal Attribute '{expected.AttributeName}' on '{expected.ComponentName}'");
            actual.GetMessage().Should().Contain(expected.Pattern);
            actual.Properties.Should().ContainKey(MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey);
            actual.Properties[MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer.ClassNamePropertyKey].Should().Be(GeneratedComponentTypeName);
            actual.AdditionalLocations.Should().ContainSingle();
            sourceText.ToString(actual.AdditionalLocations[0].SourceSpan).Should().Contain(expected.SourceMarker);
        }
    }

    private sealed record ExpectedAttributeDiagnostic(string AttributeName, string ComponentName, string Pattern, string SourceMarker);
}
#nullable restore
