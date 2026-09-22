// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.Shared;
using MudBlazor.UnitTests.Shared.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for the <see cref="SectionContent"/> component.
/// </summary>
[TestFixture]
public sealed class SectionContentTests : BunitTest
{
    private const string ExampleOne = "ButtonFilledExample";
    private const string ExampleTwo = "ButtonTextExample";

    private RecordingJsApiService _jsApi = null!;

    public override void Setup()
    {
        base.Setup();

        _jsApi = new RecordingJsApiService();
        Context.Services.AddSingleton<IJsApiService>(_jsApi);
        Context.Services.AddTransient<IDocsJsApiService, MockDocsJsApiService>();
    }

    /// <summary>
    /// The example's file name is shown, so the reader can tell which file the sample lives in.
    /// </summary>
    [Test]
    public void ShowsTheExampleFileName()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .AddChildContent("<p>preview</p>"));

        comp.Find(".docs-section-filename").TextContent.Should().Be($"{ExampleOne}.razor");
    }

    /// <summary>
    /// The bar is a plain group of buttons. It previously declared role="toolbar" without an
    /// accessible name or the roving tabindex behaviour that role promises.
    /// </summary>
    [Test]
    public void ToolbarDoesNotDeclareAnUnsupportedToolbarRole()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .AddChildContent("<p>preview</p>"));

        comp.FindAll(".docs-section-content-toolbar[role=toolbar]").Should().BeEmpty();
    }

    /// <summary>
    /// The toggle reports its state, and the source region is genuinely hidden rather than
    /// collapsed to zero height while still being read by assistive technology.
    /// </summary>
    [Test]
    public void TogglingCodeUpdatesAriaExpandedAndHidesTheRegion()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .Add(x => x.ShowCode, true)
            .AddChildContent("<p>preview</p>"));

        var toggle = comp.Find(".docs-section-code-toggle");
        toggle.GetAttribute("aria-expanded").Should().Be("true");

        var regionId = toggle.GetAttribute("aria-controls");
        regionId.Should().NotBeNullOrWhiteSpace();
        comp.Find($"#{regionId}").HasAttribute("hidden").Should().BeFalse();

        toggle.Click();

        comp.Find(".docs-section-code-toggle").GetAttribute("aria-expanded").Should().Be("false");
        comp.Find($"#{regionId}").HasAttribute("hidden").Should().BeTrue();
    }

    /// <summary>
    /// Collapsing the code must not leave the source region rendering as an empty band.
    /// </summary>
    [Test]
    public void CollapsedSourceRegionIsTheElementThatCarriesHidden()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .Add(x => x.ShowCode, true)
            .AddChildContent("<p>preview</p>"));

        comp.Find(".docs-section-code-toggle").Click();

        comp.Find(".docs-section-source").HasAttribute("hidden").Should().BeTrue();
    }

    /// <summary>
    /// The reader's choice to collapse the code must survive a parent re-render.
    /// QueuedContent renders sections progressively, so parents keep re-rendering after page
    /// load and would otherwise re-supply ShowCode and silently re-open the pane.
    /// </summary>
    [Test]
    public async Task CollapsingCodeSurvivesAParentRerender()
    {
        var comp = Context.Render<SectionContentHost>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .Add(x => x.ShowCode, true));

        comp.Find(".docs-section-code-toggle").Click();
        comp.Find(".docs-section-code-toggle").GetAttribute("aria-expanded").Should().Be("false");

        // The parent renders again and re-supplies the very same parameters, which is what
        // QueuedContent does repeatedly while the rest of the page is still coming in.
        await comp.InvokeAsync(comp.Instance.RerenderFromParent);

        comp.Find(".docs-section-code-toggle").GetAttribute("aria-expanded").Should().Be("false");
    }

    /// <summary>
    /// The same applies to the selected file in a multi-file example.
    /// </summary>
    [Test]
    public async Task SelectedFileSurvivesAParentRerender()
    {
        var codes = new[]
        {
            new CodeFile("First", ExampleOne),
            new CodeFile("Second", ExampleTwo),
        };

        var comp = Context.Render<SectionContentHost>(parameters => parameters
            .Add(x => x.Codes, codes));

        comp.FindAll("button.file-button")[1].Click();
        comp.Find("button.file-button.active").TextContent.Trim().Should().Be("Second");

        await comp.InvokeAsync(comp.Instance.RerenderFromParent);

        comp.Find("button.file-button.active").TextContent.Trim().Should().Be("Second");
    }

    /// <summary>
    /// A highlight term containing regex metacharacters must not blank the pane. Unescaped, it
    /// threw out of Regex.Replace into the catch that renders nothing.
    /// </summary>
    [Test]
    public void HighlightTermWithRegexMetacharactersStillRendersTheSource()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .Add(x => x.HighLight, "Color(unclosed[")
            .AddChildContent("<p>preview</p>"));

        comp.Find(".docs-section-source-container").TextContent.Should().Contain("MudButton");
    }

    /// <summary>
    /// Escaping the term must not stop it matching. This mirrors a real usage: the CSS utility
    /// pages highlight class names, which the lookahead finds inside a class attribute.
    /// </summary>
    [Test]
    public void HighlightTermStillMarksMatches()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, "BorderStyleExample")
            .Add(x => x.HighLight, "border-dashed")
            .AddChildContent("<p>preview</p>"));

        comp.FindAll(".docs-section-source-container mark").Should().NotBeEmpty();
    }

    /// <summary>
    /// The comma-separated form marks every term.
    /// </summary>
    [Test]
    public void HighlightMarksEveryCommaSeparatedTerm()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, "BorderStyleExample")
            .Add(x => x.HighLight, "border-solid,border-dashed,border-dotted")
            .AddChildContent("<p>preview</p>"));

        var marks = comp.FindAll(".docs-section-source-container mark")
            .Select(x => x.TextContent)
            .ToList();

        marks.Should().Contain("border-solid");
        marks.Should().Contain("border-dashed");
        marks.Should().Contain("border-dotted");
    }

    /// <summary>
    /// When no source can be found, nothing is written to the clipboard and the reader is told.
    /// It previously copied "Snippet 'X' not found!" and reported success.
    /// </summary>
    [Test]
    public void MissingSourceIsNotCopiedToTheClipboard()
    {
        Context.Services.AddTransient<IDocsJsApiService, EmptyDocsJsApiService>();

        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, "ThisExampleDoesNotExist")
            .AddChildContent("<p>preview</p>"));

        comp.Find("button.copy-code-button").Click();

        _jsApi.ClipboardWrites.Should().BeEmpty();
    }

    /// <summary>
    /// A real example still copies its source.
    /// </summary>
    [Test]
    public void PresentSourceIsCopiedToTheClipboard()
    {
        var comp = Context.Render<SectionContent>(parameters => parameters
            .Add(x => x.Code, ExampleOne)
            .AddChildContent("<p>preview</p>"));

        comp.Find("button.copy-code-button").Click();

        _jsApi.ClipboardWrites.Should().ContainSingle()
            .Which.Should().Contain("MudButton");
    }

    /// <summary>
    /// Stands in for the page that hosts a <see cref="SectionContent"/>, so a re-render can be
    /// driven from the parent the way it happens on a real docs page.
    /// </summary>
    private sealed class SectionContentHost : ComponentBase
    {
        [Parameter] public string Code { get; set; }

        [Parameter] public IReadOnlyList<CodeFile> Codes { get; set; }

        [Parameter] public bool ShowCode { get; set; } = true;

        public void RerenderFromParent() => StateHasChanged();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<SectionContent>(0);
            builder.AddAttribute(1, nameof(SectionContent.Code), Code);
            builder.AddAttribute(2, nameof(SectionContent.Codes), Codes);
            builder.AddAttribute(3, nameof(SectionContent.ShowCode), ShowCode);
            builder.AddAttribute(4, nameof(SectionContent.ChildContent),
                (RenderFragment)(child => child.AddMarkupContent(0, "<p>preview</p>")));
            builder.CloseComponent();
        }
    }

    private sealed class RecordingJsApiService : IJsApiService
    {
        public List<string> ClipboardWrites { get; } = [];

        public ValueTask CopyToClipboardAsync(string text)
        {
            ClipboardWrites.Add(text);

            return ValueTask.CompletedTask;
        }

        public ValueTask Open(string link, string target) => ValueTask.CompletedTask;

        public ValueTask UpdateStyleProperty(string elementId, string propertyName, object value) => ValueTask.CompletedTask;

        public ValueTask OpenInNewTabAsync(string url) => ValueTask.CompletedTask;
    }

    /// <summary>
    /// Stands in for the browser reading an empty code pane, which is what happens when the
    /// snippet could not be rendered.
    /// </summary>
    private sealed class EmptyDocsJsApiService : IDocsJsApiService
    {
        public ValueTask<string> GetInnerTextByIdAsync(string id) => ValueTask.FromResult(string.Empty);
    }
}
