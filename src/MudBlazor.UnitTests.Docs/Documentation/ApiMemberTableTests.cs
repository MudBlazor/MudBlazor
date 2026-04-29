// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Docs.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.UnitTests.Docs.Documentation;
/// <summary>
/// Tests for the <see cref="ApiMemberTable"/> component.
/// </summary>
public sealed class ApiMemberTableTests : BunitTest
{
    /// <summary>
    /// Renders the empty-state message for a missing type.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderMissingType()
    {
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters.Add(x => x.Type, null));

        await Assert.That(comp.Markup).Contains("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should be a message saying no members are found");
    }

    /// <summary>
    /// Renders the empty-state message in <see cref="ApiMemberTableMode.None"/>.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderNoneMode()
    {
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudAlert)
            .Add(x => x.Mode, ApiMemberTableMode.None));

        await Assert.That(comp.Markup).Contains("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should be a message saying no members are found");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Properties"/> with protected properties shown.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderProperties_WithProtected()
    {
        // Get a type with protected properties
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudAlert)
            .Add(x => x.Mode, ApiMemberTableMode.Properties)
            .Add(x => x.ShowProtected, true));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-body1 mud-switch mud-input-content-placement-end\">Show Protected</span>").Because("There should be a switch for protected properties");

        await Assert.That(comp.Markup).Contains("<td data-label=\"Name\" class=\"mud-table-cell  docs-content-api-cell\" id=\"Classname\">").Because("The \"Classname\" protected property should be visible");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Properties"/> with protected properties hidden.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderProperties_WithoutProtected()
    {
        // Get a type without protected properties
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudAlert)
            .Add(x => x.Mode, ApiMemberTableMode.Properties)
            .Add(x => x.ShowProtected, false));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-body1 mud-switch mud-input-content-placement-end\">Show Protected</span>").Because("There should be a switch for protected properties");

        await Assert.That(comp.Markup).DoesNotContain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"Classname\">").Because("The \"Classname\" protected property should NOT be visible");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Methods"/> with protected methods shown.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderMethods_WithProtected()
    {
        // Get a type without protected methods
        var mudAutocomplete = ApiDocumentation.GetType("MudBlazor.MudAutocomplete`1");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudAutocomplete)
            .Add(x => x.Mode, ApiMemberTableMode.Methods)
            .Add(x => x.ShowProtected, true));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-body1 mud-switch mud-input-content-placement-end\">Show Protected</span>").Because("There should be a switch for protected properties");

        await Assert.That(comp.Markup).Contains("<td data-label=\"Name\" class=\"mud-table-cell  docs-content-api-cell\" id=\"BeginValidateAsync\">").Because("The \"BeginValidateAsync\" protected method should be visible");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Methods"/> with protected methods hidden.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderMethods_WithoutProtected()
    {
        // Get a type without protected methods
        var mudAutocomplete = ApiDocumentation.GetType("MudBlazor.MudAutocomplete`1");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudAutocomplete)
            .Add(x => x.Mode, ApiMemberTableMode.Methods)
            .Add(x => x.ShowProtected, false));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-body1 mud-switch mud-input-content-placement-end\">Show Protected</span>").Because("There should be a switch for protected properties");

        await Assert.That(comp.Markup).DoesNotContain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"BeginValidateAsync\">").Because("The \"BeginValidateAsync\" protected method should NOT be visible");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Fields"/> with protected fields shown.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderFields_WithProtected()
    {
        // Get a type with protected fields
        var mudBaseDatePicker = ApiDocumentation.GetType("MudBlazor.MudBaseDatePicker");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudBaseDatePicker)
            .Add(x => x.Mode, ApiMemberTableMode.Fields)
            .Add(x => x.ShowProtected, true));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-body1 mud-switch mud-input-content-placement-end\">Show Protected</span>").Because("There should be a switch for protected properties");

        await Assert.That(comp.Markup).Contains("<td data-label=\"Name\" class=\"mud-table-cell  docs-content-api-cell\" id=\"CurrentView\">").Because("The \"CurrentView\" protected field should be visible");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Fields"/> with protected fields hidden.
    /// </summary>
    [Test]
    public async Task ApiMemberTable_RenderFields_WithoutProtected()
    {
        // Get a type without protected fields
        var mudBaseDatePicker = ApiDocumentation.GetType("MudBlazor.MudBaseDatePicker");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudBaseDatePicker)
            .Add(x => x.Mode, ApiMemberTableMode.Fields)
            .Add(x => x.ShowProtected, false));

        await Assert.That(comp.Markup).Contains("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should be a message saying no members are found  (since the protected field was the ONLY field)");

        await Assert.That(comp.Markup).Contains("<span class=\"mud-typography mud-typography-body1 mud-switch mud-input-content-placement-end\">Show Protected</span>").Because("There should be a switch for protected properties");

        await Assert.That(comp.Markup).DoesNotContain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"CurrentView\">").Because("The \"CurrentView\" protected field should NOT be visible");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Events"/> for a type with events.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are no protected events in the entire MudBlazor library.
    /// </remarks>
    [Test]
    public async Task ApiMemberTable_RenderEvents()
    {
        // Get a type with events
        var mudDataGrid = ApiDocumentation.GetType("MudBlazor.MudDataGrid`1");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudDataGrid)
            .Add(x => x.Mode, ApiMemberTableMode.Events));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");
    }

    /// <summary>
    /// Renders <see cref="ApiMemberTableMode.Globals"/> for a type with globals.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are globals for <see cref="MudMenu"/>.
    /// </remarks>
    [Test]
    public async Task ApiMemberTable_RenderGlobals_WhenExisting()
    {
        // Get a type with globals
        var mudMenu = ApiDocumentation.GetType("MudBlazor.MudMenu");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudMenu)
            .Add(x => x.Mode, ApiMemberTableMode.Globals));

        await Assert.That(comp.Markup).DoesNotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should NOT be a message saying no members are found");
    }

    /// <summary>
    /// Renders the empty state in <see cref="ApiMemberTableMode.Globals"/> when no globals exist.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are NO globals for <see cref="MudAlert"/>.
    /// </remarks>
    [Test]
    public async Task ApiMemberTable_RenderGlobals_WhenNotExisting()
    {
        // Get a type with no globals
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.Render<ApiMemberTable>(parameters => parameters
            .Add(x => x.Type, mudAlert)
            .Add(x => x.Mode, ApiMemberTableMode.Globals));

        await Assert.That(comp.Markup).Contains("<div class=\"mud-alert-message\">No members match the current filters.</div>").Because("There should be a message saying no members are found");
    }
}
