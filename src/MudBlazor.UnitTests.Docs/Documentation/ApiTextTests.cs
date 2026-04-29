// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Docs.Components;

namespace MudBlazor.UnitTests.Docs.Documentation;
/// <summary>
/// Tests for the <see cref="ApiText"/> component.
/// </summary>
public sealed class ApiTextTests : BunitTest
{
    /// <summary>
    /// Handles malformed XML documentation text gracefully.
    /// </summary>
    [Test]
    public async Task ApiText_HandleMalformedXmlDocs()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Sorry guys, I was drunk when I <see cref wrote these docs, </burp>"));

        await Assert.That(comp.Markup).IsEqualTo("<span class=\"mud-typography mud-typography-caption mud-warning-text\">XML documentation error.</span>");
    }

    /// <summary>
    /// Renders plain text.
    /// </summary>
    [Test]
    public async Task ApiText_RenderJustText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Gets or sets the icon for this widget."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Gets or sets the icon for this widget.</span>");
    }

    /// <summary>
    /// Renders null text as empty output.
    /// </summary>
    [Test]
    public async Task ApiText_RenderNullText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, null));

        await Assert.That(comp.Markup).IsEqualTo("");
    }

    /// <summary>
    /// Renders empty text as empty output.
    /// </summary>
    [Test]
    public async Task ApiText_RenderEmptyText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, ""));

        await Assert.That(comp.Markup).IsEqualTo("");
    }

    /// <summary>
    /// Renders self-closing <c>&lt;see href="" /&gt;</c> links.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeHref_SelfClosing()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\" /> right now."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">https://www.mudblazor.com").Because("Then a link to https://www.mudblazor.com with the same text");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> right now.</span>").Because("Ending with another text span");
    }

    /// <summary>
    /// Renders <c>&lt;see href=""&gt;...&lt;/see&gt;</c> links with inner text.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeHref_WithText()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "For the best Blazor components, go to <see href=\"https://www.mudblazor.com\">MudBlazor</see> right now."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">For the best Blazor components, go to </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"https://www.mudblazor.com\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">MudBlazor").Because("Then a link to \"MudBlazor\" (text)");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> right now.</span>").Because("Ending with another text span");
    }

    /// <summary>
    /// Skips empty <c>&lt;see href="" /&gt;</c> links.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeHref_EmptyUrl()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "For another Blazor library, go to <see href=\"\" />."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">For another Blazor library, go to </span><span class=\"mud-typography mud-typography-caption\">.</span>").Because("The link should be skipped completely");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing properties.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_ExistingProperty()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Occurs when <see cref=\"P:MudBlazor.MudComponentBase.Class\" /> has changed."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"/api/MudComponentBase#Class\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">Class</a>").Because("Then a link to /api/MudComponentBase#Class");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> has changed.</span>").Because("Ending with another text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent properties as code.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_NonExistantProperty()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Occurs when <see cref=\"P:MudBlazor.NotExistingType.NotExistingProperty\" /> has changed."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Occurs when </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<code class=\"docs-code docs-code-primary\">MudBlazor.NotExistingType.NotExistingProperty</code>").Because("There's no valid link, just a span for the non-existant property");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> has changed.</span>").Because("Ending with another text span");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing methods.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_ExistingMethod()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "When set, calls <see cref=\"M:MudBlazor.AggregateDefinition`1.SimpleAvg\" /> to receive viewport changes."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"/api/AggregateDefinition`1#SimpleAvg\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">SimpleAvg</a>").Because("Then a link to /api/AggregateDefinition`1#SimpleAvg");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> to receive viewport changes.</span>").Because("Ending with another text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent methods as code.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_NonExistantMethod()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "When set, calls <see cref=\"M:MudBlazor.NotExistingType.NotExistingMethod\" /> to do stuff."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">When set, calls </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<code class=\"docs-code docs-code-primary\">MudBlazor.NotExistingType.NotExistingMethod</code>").Because("There's no valid link, just a span for the non-existant method");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> to do stuff.</span>").Because("Ending with another text span");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing fields.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_ExistingField()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Shows when set to <see cref=\"F:MudBlazor.Adornment.End\" />."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Shows when set to </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"/api/Adornment#End").Because("There should be a link to /api/Adornment");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">.</span>").Because("There should be a text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent fields as code.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_NonExistantField()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Shows when set to <see cref=\"F:MudBlazor.Adornment.EndOfTheUniverse\" />."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Shows when set to </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<code class=\"docs-code docs-code-primary\">MudBlazor.Adornment.EndOfTheUniverse</code>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">.</span>").Because("There should be a text span");
    }

    /// <summary>
    /// Renders <c>&lt;see cref="" /&gt;</c> links to existing events.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_ExistingEvent()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Gets set when the <see cref=\"E:MudBlazor.MudAlert.OnClick\" /> event occurs."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Gets set when the </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"/api/MudAlert#OnClick\"").Because("There should be a link to /api/MudAlert#OnClick");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> event occurs.</span>").Because("There should be a text span");
    }

    /// <summary>
    /// Renders invalid <c>&lt;see cref="" /&gt;</c> links to non-existent events as code.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_NonExistantEvent()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "Gets set when the <see cref=\"E:MudBlazor.MudAlert.OnSmokeAlarmInYourHouse\" /> event occurs."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">Gets set when the </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<code class=\"docs-code docs-code-primary\">MudBlazor.MudAlert.OnSmokeAlarmInYourHouse</code>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\"> event occurs.</span>").Because("There should be a text span");
    }

    /// <summary>
    /// Renders links to external Microsoft types.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_External_MicrosoftType()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "The button can contain a <see cref=\"T:Microsoft.AspNetCore.Components.RenderFragment\" />."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">The button can contain a </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.components.renderfragment\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">RenderFragment").Because("There should be a link to Microsoft docs");

        await Assert.That(comp.Markup).Contains("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>").Because("There should be a Link icon");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">.</span>").Because("There should be a text span");
    }

    /// <summary>
    /// Renders links to external system types.
    /// </summary>
    [Test]
    public async Task ApiText_RenderSeeCref_External_SystemType()
    {
        var comp = Context.Render<ApiText>(parameters => parameters.Add(x => x.Text, "The popover unique ID is a <see cref=\"T:System.Guid\" />."));

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">The popover unique ID is a </span>").Because("There should be a text span");

        await Assert.That(comp.Markup).Contains("<a href=\"https://learn.microsoft.com/dotnet/api/system.guid\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">Guid").Because("There should be a link to Microsoft docs");

        await Assert.That(comp.Markup).Contains("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>").Because("There should be a Link icon");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-caption\">.</span>").Because("There should be a text span");
    }
}
