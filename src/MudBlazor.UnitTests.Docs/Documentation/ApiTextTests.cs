// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Docs.Components;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for the <see cref="ApiText"/> component.
/// </summary>
[TestFixture]
public sealed class ApiTextTests : BunitTest
{
    /// <summary>
    /// Ensures that malformed text is handled gracefully.
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
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Gets or sets the icon for this widget.</span>");
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
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", ""));
        comp.Markup.Should().Be("");
    }

    /// <summary>
    /// Ensures that self closing <see href="" /> links render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_SelfClosing()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\" /> right now."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>");
        // Then a link to https://www.mudblazor.com with the same text
        comp.Markup.Should().Contain("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">https://www.mudblazor.com");
        // Ending with another text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> right now.</span>");
    }

    /// <summary>
    /// Ensures that <see href=""/> links with interior text render properly.
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
    /// Ensures that empty <see href=""/> links are skipped.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_EmptyUrl()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "For another Blazor library, go to <see href=\"\" />."));
        // The link should be skipped completely
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For another Blazor library, go to </span><span class=\"mud-typography mud-typography-caption\">.</span>");
    }

    /// <summary>
    /// Ensures that <see cref=""/> links to existing properties render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingProperty()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Occurs when <see cref=\"P:MudBlazor.MudComponentBase.Class\" /> has changed."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>");
        // Then a link to /api/MudComponentBase#Class
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
    /// Ensures that <see cref=""/> links to existing methods render properly.
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
    /// Ensures that invalid <see cref=""/> links to non-existant methods render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantMethod()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "When set, calls <see cref=\"M:MudBlazor.NotExistingType.NotExistingMethod\" /> to do stuff."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>");
        // There's no valid link, just a span for the non-existant method
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
        comp.Markup.Should().Contain("<a href=\"/api/Adornment#End");
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

    /// <summary>
    /// Ensures that <see cref=""/> links to existing events render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingEvent()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Gets set when the <see cref=\"E:MudBlazor.MudAlert.OnClick\" /> event occurs."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Gets set when the </span>");
        // There should be a link to /api/MudAlert#OnClick
        comp.Markup.Should().Contain("<a href=\"/api/MudAlert#OnClick\"");
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> event occurs.</span>");
    }

    /// <summary>
    /// Ensures that <see cref=""/> links to non-existant events render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantEvent()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "Gets set when the <see cref=\"E:MudBlazor.MudAlert.OnSmokeAlarmInYourHouse\" /> event occurs."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Gets set when the </span>");
        // There should be a text span
        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.MudAlert.OnSmokeAlarmInYourHouse</code>");
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> event occurs.</span>");
    }

    /// <summary>
    /// Ensures that links to external types render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_External_MicrosoftType()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "The button can contain a <see cref=\"T:Microsoft.AspNetCore.Components.RenderFragment\" />."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">The button can contain a </span>");
        // There should be a link to Microsoft docs
        comp.Markup.Should().Contain("<a href=\"https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.components.renderfragment\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">RenderFragment");
        // There should be a Link icon
        comp.Markup.Should().Contain("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>");
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>");
    }

    /// <summary>
    /// Ensures that links to external types render properly.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_External_SystemType()
    {
        var comp = Context.RenderComponent<ApiText>(Parameter("Text", "The popover unique ID is a <see cref=\"T:System.Guid\" />."));
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">The popover unique ID is a </span>");
        // There should be a link to Microsoft docs
        comp.Markup.Should().Contain("<a href=\"https://learn.microsoft.com/dotnet/api/system.guid\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">Guid");
        // There should be a Link icon
        comp.Markup.Should().Contain("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>");
        // There should be a text span
        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>");
    }
}
