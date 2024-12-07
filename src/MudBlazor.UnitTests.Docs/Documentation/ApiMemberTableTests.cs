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
public sealed class ApiMemberTableTests : BunitTest
{
    /// <summary>
    /// Ensures that a missing type renders properly.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderMissingType()
    {
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", null));
        // There should be a message saying no members are found
        comp.Markup.Should().Contain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.None"/> renders properly.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderNoneMode()
    {
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudAlert), Parameter("Mode", ApiMemberTableMode.None));
        // There should be a message saying no members are found
        comp.Markup.Should().Contain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Properties"/> renders properly with showing protected properties.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderProperties_WithProtected()
    {
        // Get a type with protected properties
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudAlert), Parameter("Mode", ApiMemberTableMode.Properties), Parameter("ShowProtected", true));
        // There should NOT be a message saying no members are found
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
        // There should be a switch for protected properties
        comp.Markup.Should().Contain("<p class=\"mud-typography mud-typography-body1 mud-switch mud-switch-label-small mud-input-content-placement-end\">Show Protected</p>");
        // The "Classname" protected property should be visible
        comp.Markup.Should().Contain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"Classname\">");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Properties"/> renders properly without showing protected properties.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderProperties_WithoutProtected()
    {
        // Get a type without protected properties
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudAlert), Parameter("Mode", ApiMemberTableMode.Properties), Parameter("ShowProtected", false));
        // There should NOT be a message saying no members are found
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
        // There should be a switch for protected properties
        comp.Markup.Should().Contain("<p class=\"mud-typography mud-typography-body1 mud-switch mud-switch-label-small mud-input-content-placement-end\">Show Protected</p>");
        // The "Classname" protected property should NOT be visible
        comp.Markup.Should().NotContain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"Classname\">");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Methods"/> renders properly with showing protected methods.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderMethods_WithProtected()
    {
        // Get a type without protected methods
        var mudAutocomplete = ApiDocumentation.GetType("MudBlazor.MudAutocomplete`1");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudAutocomplete), Parameter("Mode", ApiMemberTableMode.Methods), Parameter("ShowProtected", true));
        // There should NOT be a message saying no members are found
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
        // There should be a switch for protected properties
        comp.Markup.Should().Contain("<p class=\"mud-typography mud-typography-body1 mud-switch mud-switch-label-small mud-input-content-placement-end\">Show Protected</p>");
        // The "BeginValidateAsync" protected method should be visible
        comp.Markup.Should().Contain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"BeginValidateAsync\">");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Methods"/> renders properly without showing protected methods.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderMethods_WithoutProtected()
    {
        // Get a type without protected methods
        var mudAutocomplete = ApiDocumentation.GetType("MudBlazor.MudAutocomplete`1");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudAutocomplete), Parameter("Mode", ApiMemberTableMode.Methods), Parameter("ShowProtected", false));
        // There should NOT be a message saying no members are found
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
        // There should be a switch for protected properties
        comp.Markup.Should().Contain("<p class=\"mud-typography mud-typography-body1 mud-switch mud-switch-label-small mud-input-content-placement-end\">Show Protected</p>");
        // The "BeginValidateAsync" protected method should NOT be visible
        comp.Markup.Should().NotContain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"BeginValidateAsync\">");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Methods"/> renders properly with showing protected fields.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderFields_WithProtected()
    {
        // Get a type with protected fields
        var mudBaseDatePicker = ApiDocumentation.GetType("MudBlazor.MudBaseDatePicker");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudBaseDatePicker), Parameter("Mode", ApiMemberTableMode.Fields), Parameter("ShowProtected", true));
        // There should NOT be a message saying no members are found
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
        // There should be a switch for protected properties
        comp.Markup.Should().Contain("<p class=\"mud-typography mud-typography-body1 mud-switch mud-switch-label-small mud-input-content-placement-end\">Show Protected</p>");
        // The "CurrentView" protected field should be visible
        comp.Markup.Should().Contain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"CurrentView\">");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Fields"/> renders properly without showing protected fields.
    /// </summary>
    [Test]
    public void ApiMemberTable_RenderFields_WithoutProtected()
    {
        // Get a type without protected fields
        var mudBaseDatePicker = ApiDocumentation.GetType("MudBlazor.MudBaseDatePicker");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudBaseDatePicker), Parameter("Mode", ApiMemberTableMode.Fields), Parameter("ShowProtected", false));
        // There should be a message saying no members are found  (since the protected field was the ONLY field)
        comp.Markup.Should().Contain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
        // There should be a switch for protected properties
        comp.Markup.Should().Contain("<p class=\"mud-typography mud-typography-body1 mud-switch mud-switch-label-small mud-input-content-placement-end\">Show Protected</p>");
        // The "CurrentView" protected field should NOT be visible
        comp.Markup.Should().NotContain("<td data-label=\"Name\" class=\"mud-table-cell docs-content-api-cell\" id=\"CurrentView\">");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Events"/> renders properly for a type with events.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are no protected events in the entire MudBlazor library.
    /// </remarks>
    [Test]
    public void ApiMemberTable_RenderEvents()
    {
        // Get a type with events
        var mudDataGrid = ApiDocumentation.GetType("MudBlazor.MudDataGrid`1");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudDataGrid), Parameter("Mode", ApiMemberTableMode.Events));
        // There should NOT be a message saying no members are found  
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Globals"/> renders properly for a type with globals.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are globals for <see cref="MudButton"/>.
    /// </remarks>
    [Test]
    public void ApiMemberTable_RenderGlobals_WhenExisting()
    {
        // Get a type with globals
        var mudButton = ApiDocumentation.GetType("MudBlazor.MudButton");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudButton), Parameter("Mode", ApiMemberTableMode.Globals));
        // There should NOT be a message saying no members are found  
        comp.Markup.Should().NotContain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
    }

    /// <summary>
    /// Ensures that a mode of <see cref="ApiMemberTableMode.Globals"/> renders properly for a type WITHOUT globals.
    /// </summary>
    /// <remarks>
    /// At the time of writing this test, there are NO globals for <see cref="MudAlert"/>.
    /// </remarks>
    [Test]
    public void ApiMemberTable_RenderGlobals_WhenNotExisting()
    {
        // Get a type with no globals
        var mudAlert = ApiDocumentation.GetType("MudBlazor.MudAlert");
        using var comp = Context.RenderComponent<ApiMemberTable>(Parameter("Type", mudAlert), Parameter("Mode", ApiMemberTableMode.Globals));
        // There should be a message saying no members are found  
        comp.Markup.Should().Contain("<div class=\"mud-alert-message\">No members match the current filters.</div>");
    }
}
