// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Docs.Components;
using MudBlazor.Docs.Models;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for the <see cref="ApiMemberTable"/> component.
/// </summary>
[TestFixture]
public sealed class ApiSeeAlsoLinksTests : BunitTest
{
    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.SeeAlso"/> renders properly for a type without see-also links.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are see-also links for <see cref="MudButton"/>.
    /// </remarks>
    [Test]
    public void ApiSeeAlsoLinks_RenderSeeAlso_WhenExisting()
    {
        // Get a type with see-also links
        var mudButton = ApiDocumentation.GetType("MudBlazor.MudButton");
        using var comp = Context.RenderComponent<ApiSeeAlsoLinks>(Parameter("Type", mudButton));
        // There should be a see-also link to MudButtonGroup
        comp.Markup.Should().Contain("<a href=\"/api/MudButtonGroup\"");
        comp.Markup.Should().Contain("class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-body1 docs-link docs-code docs-code-primary\">MudButtonGroup</a>");
        // There should NOT be a message saying no members are found  
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No see-also links match the current filters.</div>");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.SeeAlso"/> renders properly for a type without see-also links.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are no see-also links for <see cref="MudAlert"/>.
    /// </remarks>
    [Test]
    public void ApiSeeAlsoLinks_RenderSeeAlso_WhenNotExisting()
    {
        // Get a type with no see-also links
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.RenderComponent<ApiSeeAlsoLinks>(Parameter("Type", mudAlert));
        // There should be a message saying no members are found  
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No see-also links match the current filters.</div>");
    }
}
