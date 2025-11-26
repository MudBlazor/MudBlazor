// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Docs.Components;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for the <see cref="ApiTypeLink"/> component.
/// </summary>
[TestFixture]
public sealed class ApiTypeLinkTests : BunitTest
{
    /// <summary>
    /// Ensures that booleans render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Boolean()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Boolean"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">bool</code>");
    }

    /// <summary>
    /// Ensures that boolean arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_BooleanArray()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Boolean[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">bool[]</code>");
    }

    /// <summary>
    /// Ensures that integers render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Int32()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Int32"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">int</code>");
    }

    /// <summary>
    /// Ensures that int arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Int32Array()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Int32[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">int[]</code>");
    }

    /// <summary>
    /// Ensures that longs render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Int64()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Int64"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">long</code>");
    }

    /// <summary>
    /// Ensures that long arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Int64Array()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Int64[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">long[]</code>");
    }

    /// <summary>
    /// Ensures that strings render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_String()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.String"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">string</code>");
    }

    /// <summary>
    /// Ensures that string arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_StringArray()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.String[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">string[]</code>");
    }

    /// <summary>
    /// Ensures that doubles render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Double()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Double"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">double</code>");
    }

    /// <summary>
    /// Ensures that double arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_DoubleArray()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Double[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">double[]</code>");
    }

    /// <summary>
    /// Ensures that floats render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Single()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Single"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">float</code>");
    }

    /// <summary>
    /// Ensures that float arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_SingleArray()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Single[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">float[]</code>");
    }

    /// <summary>
    /// Ensures that objects render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Object()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Object"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">object</code>");
    }

    /// <summary>
    /// Ensures that object arrays render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_ObjectArray()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Object[]"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">object[]</code>");
    }

    /// <summary>
    /// Ensures that void return types render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_Void()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Void"));
        comp.Markup.Should().Be("<code class=\"docs-code docs-code-primary\">void</code>");
    }

    /// <summary>
    /// Ensures that MudBlazor component links render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_MudBlazor_Component()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "MudBlazor.MudAlert"));
        // There should be a link to MudAlert
        comp.Markup.Should().Contain("<a href=\"/api/MudAlert\" blazor:onclick=\"6\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-body1 docs-link docs-code docs-code-primary\">MudAlert</a>");
    }

    /// <summary>
    /// Ensures that MudBlazor enum links render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_MudBlazor_Enums()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "MudBlazor.Adornment"));
        // There should be a link to Adornment
        comp.Markup.Should().Contain("<a href=\"/api/Adornment\"");
        comp.Markup.Should().Contain("class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-body1 docs-link docs-code docs-code-primary\">Adornment</a>");
    }

    /// <summary>
    /// Ensures that external types render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_External_MicrosoftType()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "Microsoft.AspNetCore.Components.RenderFragment"));
        // There should be a link to Microsoft docs
        comp.Markup.Should().Contain("<a href=\"https://learn.microsoft.com/dotnet/api/Microsoft.AspNetCore.Components.RenderFragment\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">");
        // There should be a Link icon
        comp.Markup.Should().Contain("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>");
    }

    /// <summary>
    /// Ensures that external types render properly.
    /// </summary>
    [Test]
    public void ApiTypeLink_External_SystemType()
    {
        var comp = Context.RenderComponent<ApiTypeLink>(Parameter("TypeName", "System.Guid"));
        // There should be a link to Microsoft docs
        comp.Markup.Should().Contain("<a href=\"https://learn.microsoft.com/dotnet/api/System.Guid\" target=\"_external\" blazor:onclick=\"1\" class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-caption docs-link docs-code docs-code-primary\">");
        // There should be a Link icon
        comp.Markup.Should().Contain("<svg class=\"mud-icon-root mud-icon-default mud-svg-icon mud-icon-size-small\" style=\"position:relative;top:7px;\" focusable=\"false\" viewBox=\"0 0 24 24\" aria-hidden=\"true\" role=\"img\"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z\"/></svg>");
    }
}
