// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Docs.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.UnitTests.Docs.Documentation;
/// <summary>
/// Tests for the <see cref="ApiMemberTable"/> component.
/// </summary>
public sealed class ApiSeeAlsoLinksTests : BunitTest
{
    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.SeeAlso"/> when see-also links exist.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are see-also links for <see cref="MudButton"/>.
    /// </remarks>
    [Test]
    public async Task ApiSeeAlsoLinks_RenderSeeAlso_WhenExisting()
    {
        // Get a type with see-also links
        var mudButton = ApiDocumentation.GetType("MudBlazor.MudButton");
        using var comp = Context.Render<ApiSeeAlsoLinks>(parameters => parameters.Add(x => x.Type, mudButton));

        await Assert.That(comp.Markup).Contains("<a href=\"/api/MudButtonGroup\"").Because("There should be a see-also link to MudButtonGroup");

        await Assert.That(comp.Markup).Contains("class=\"mud-typography mud-link mud-primary-text mud-link-underline-hover mud-typography-body1 docs-link docs-code docs-code-primary\">MudButtonGroup</a>").Because("There should be a see-also link to MudButtonGroup");

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No see-also links match the current filters.</div>").Because("There should NOT be a message saying no members are found");
    }

    /// <summary>
    /// Renders the empty state in <see cref="ApiMemberTableMode.SeeAlso"/> when no see-also links exist.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are no see-also links for <see cref="MudAlert"/>.
    /// </remarks>
    [Test]
    public async Task ApiSeeAlsoLinks_RenderSeeAlso_WhenNotExisting()
    {
        // Get a type with no see-also links
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.Render<ApiSeeAlsoLinks>(parameters => parameters.Add(x => x.Type, mudAlert));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No see-also links match the current filters.</div>").Because("the current assertion expects no empty-state message");
    }
}
