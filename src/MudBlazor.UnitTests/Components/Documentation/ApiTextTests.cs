// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Docs.Components;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components.Documentation;

/// <summary>
/// Tests for the <see cref="ApiText"/> component.
/// </summary>
[TestFixture]
public sealed class ApiTextTests : BunitTest
{
    /// <summary>
    /// Ensures that regular text renders properly.
    /// </summary>
    [Test]
    public void ApiText_HandleMalformedXmlDocs()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Sorry guys, I was drunk when I <see cref wrote these docs, </burp>"));
        comp.Markup.Should().Be("<span class=\"mud-typography mud-typography-caption mud-warning-text\">XML documentation error.</span>");
    }

    /// <summary>
    /// Ensures that regular text renders properly.
    /// </summary>
    [Test]
    public void ApiText_RenderJustText()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Gets or sets the icon for this widget."));
        comp.Markup.Should().Be("<span class=\"mud-typography mud-typography-caption\">Gets or sets the icon for this widget.</span>");
    }

    /// <summary>
    /// Ensures that null text renders properly.
    /// </summary>
    [Test]
    public void ApiText_RenderNullText()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", null));
        comp.Markup.Should().Be("");
    }

    /// <summary>
    /// Ensures that empty text renders properly.
    /// </summary>
    [Test]
    public void ApiText_RenderEmptyText()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", null));
        comp.Markup.Should().Be("");
    }

    /// <summary>
    /// Ensures that valid <see href=""/> links to existing properties render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_SelfClosing()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\" /> right now."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>");
        // Then a link to https://www.mudblazor.com
        comp.Markup.Should().Contain("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">https://www.mudblazor.com");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> right now.</span>");
    }

    /// <summary>
    /// Ensures that valid <see href=""/> links to existing properties render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_WithText()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\">MudBlazor</see> right now."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>");
        // Then a link to "MudBlazor" (text)
        comp.Markup.Should().Contain("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">MudBlazor");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> right now.</span>");
    }

    /// <summary>
    /// Ensures that valid <see href=""/> links to empty URL's are skipped.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_EmptyUrl()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "For another Blazor library, go to <see href=\"\" />."));
        // The link should be skipped completely
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For another Blazor library, go to </span><span class=\"mud-typography mud-typography-caption\">.</span>");
    }

    /// <summary>
    /// Ensures that valid <see cref=""/> links to existing properties render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingProperty()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Occurs when <see cref=\"P:MudBlazor.MudComponentBase.Class\" /> has changed."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>");
        // Then a link to /api/MudAlert#Icon
        comp.Markup.Should().Contain("<a href=\"/api/MudComponentBase#Class\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">Class</a>");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> has changed.</span>");
    }

    /// <summary>
    /// Ensures that invalid <see cref=""/> links to non-existant properties render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantProperty()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Occurs when <see cref=\"P:MudBlazor.NotExistingType.NotExistingProperty\" /> has changed."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>");
        // There's no valid link, just a span for the non-existant property
        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.NotExistingType.NotExistingProperty</code>");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> has changed.</span>");
    }

    /// <summary>
    /// Ensures that <see cref=""/> links render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingMethod()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "When set, calls <see cref=\"M:MudBlazor.AggregateDefinition`1.SimpleAvg\" /> to receive viewport changes."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>");
        // Then a link to /api/AggregateDefinition`1#SimpleAvg
        comp.Markup.Should().Contain("<a href=\"/api/AggregateDefinition`1#SimpleAvg\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">SimpleAvg</a>");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> to receive viewport changes.</span>");
    }

    /// <summary>
    /// Ensures that invalid <see cref=""/> links render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantMethod()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "When set, calls <see cref=\"M:MudBlazor.NotExistingType.NotExistingMethod\" /> to do stuff."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>");
        // There's no valid link, just a span for the non-existant property
        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.NotExistingType.NotExistingMethod</code>");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> to do stuff.</span>");
    }

    /// <summary>
    /// Ensures that <see cref=""/> links to existing fields render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingField()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Shows when set to <see cref=\"F:MudBlazor.Adornment.End\" />."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Shows when set to </span>");
        // There should be a link to /api/Adornment
        comp.Markup.Should().Contain("<a href=\"/api/Adornment\"");
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>");
    }

    /// <summary>
    /// Ensures that <see cref=""/> links to non-existant fields render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantField()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Shows when set to <see cref=\"F:MudBlazor.Adornment.EndOfTheUniverse\" />."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Shows when set to </span>");
        // There should be a text span
        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.Adornment.EndOfTheUniverse</code>");
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>");
    }
}
