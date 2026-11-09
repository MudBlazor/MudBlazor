// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Docs.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for the <see cref="ApiText"/> component.
/// </summary>
[TestFixture]
public sealed class ApiTextTests : BunitTest
{
    /// <summary>
    /// Handles malformed XML documentation text gracefully.
    /// </summary>
    [Test]
    public void ApiText_HandleMalformedXmlDocs()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Sorry guys, I was drunk when I <see cref wrote these docs, </burp>"));

        comp.Markup.Should().Be("<span class=\"mud-typography mud-typography-caption mud-warning-text\">XML documentation error.</span>");
    }

    /// <summary>
    /// Renders plain text.
    /// </summary>
    [Test]
    public void ApiText_RenderJustText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Gets or sets the icon for this widget."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Gets or sets the icon for this widget.</span>");
    }

    /// <summary>
    /// Renders null text as empty output.
    /// </summary>
    [Test]
    public void ApiText_RenderNullText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, null));

        comp.Markup.Should().Be("");
    }

    /// <summary>
    /// Renders empty text as empty output.
    /// </summary>
    [Test]
    public void ApiText_RenderEmptyText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, ""));

        comp.Markup.Should().Be("");
    }

    /// <summary>
    /// Renders self-closing <c>&lt;see href="" /&gt;</c> links.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_SelfClosing()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\" /> right now."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">https://www.mudblazor.com", "Then a link to https://www.mudblazor.com with the same text");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> right now.</span>", "Ending with another text span");
    }

    /// <summary>
    /// Renders <c>&lt;see href=""&gt;...&lt;/see&gt;</c> links with inner text.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_WithText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\">MudBlazor</see> right now."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">MudBlazor", "Then a link to \"MudBlazor\" (text)");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> right now.</span>", "Ending with another text span");
    }

    /// <summary>
    /// Skips empty <c>&lt;see href="" /&gt;</c> links.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeHref_EmptyUrl()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "For another Blazor library, go to <see href=\"\" />."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">For another Blazor library, go to </span><span class=\"mud-typography mud-typography-caption\">.</span>", "The link should be skipped completely");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing properties.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingProperty()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Occurs when <see cref=\"P:MudBlazor.MudComponentBase.Class\" /> has changed."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"/api/MudComponentBase#Class\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">Class</a>", "Then a link to /api/MudComponentBase#Class");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> has changed.</span>", "Ending with another text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent properties as code.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantProperty()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Occurs when <see cref=\"P:MudBlazor.NotExistingType.NotExistingProperty\" /> has changed."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>", "There should be a text span");

        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.NotExistingType.NotExistingProperty</code>", "There's no valid link, just a span for the non-existant property");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> has changed.</span>", "Ending with another text span");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing methods.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingMethod()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "When set, calls <see cref=\"M:MudBlazor.AggregateDefinition`1.SimpleAvg\" /> to receive viewport changes."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"/api/AggregateDefinition`1#SimpleAvg\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">SimpleAvg</a>", "Then a link to /api/AggregateDefinition`1#SimpleAvg");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> to receive viewport changes.</span>", "Ending with another text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent methods as code.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantMethod()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "When set, calls <see cref=\"M:MudBlazor.NotExistingType.NotExistingMethod\" /> to do stuff."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>", "There should be a text span");

        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.NotExistingType.NotExistingMethod</code>", "There's no valid link, just a span for the non-existant method");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> to do stuff.</span>", "Ending with another text span");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing fields.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingField()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Shows when set to <see cref=\"F:MudBlazor.Adornment.End\" />."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Shows when set to </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"/api/Adornment#End", "There should be a link to /api/Adornment");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>", "There should be a text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent fields as code.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantField()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Shows when set to <see cref=\"F:MudBlazor.Adornment.EndOfTheUniverse\" />."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Shows when set to </span>", "There should be a text span");

        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.Adornment.EndOfTheUniverse</code>", "There should be a text span");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>", "There should be a text span");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing events.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_ExistingEvent()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Gets set when the <see cref=\"E:MudBlazor.MudAlert.OnClick\" /> event occurs."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Gets set when the </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"/api/MudAlert#OnClick\"", "There should be a link to /api/MudAlert#OnClick");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> event occurs.</span>", "There should be a text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent events as code.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_NonExistantEvent()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Gets set when the <see cref=\"E:MudBlazor.MudAlert.OnSmokeAlarmInYourHouse\" /> event occurs."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">Gets set when the </span>", "There should be a text span");

        comp.Markup.Should().Contain("<code class=\"docs-code docs-code-primary\">MudBlazor.MudAlert.OnSmokeAlarmInYourHouse</code>", "There should be a text span");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\"> event occurs.</span>", "There should be a text span");
    }

    /// <summary>
    /// Renders links to external Microsoft types.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_External_MicrosoftType()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "The button can contain a <see cref=\"T:Microsoft.AspNetCore.Components.RenderFragment\" />."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">The button can contain a </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.components.renderfragment\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">RenderFragment", "There should be a link to Microsoft docs");

        comp.Markup.Should().Contain("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>", "There should be a Link icon");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>", "There should be a text span");
    }

    /// <summary>
    /// Renders links to external system types.
    /// </summary>
    [Test]
    public void ApiText_RenderSeeCref_External_SystemType()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "The popover unique ID is a <see cref=\"T:System.Guid\" />."));

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">The popover unique ID is a </span>", "There should be a text span");

        comp.Markup.Should().Contain("<a href=\"https://learn.microsoft.com/dotnet/api/system.guid\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">Guid", "There should be a link to Microsoft docs");

        comp.Markup.Should().Contain("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>", "There should be a Link icon");

        comp.Markup.Should().Contain("<span class=\"mud-typography mud-typography-caption\">.</span>", "There should be a text span");
    }
}
