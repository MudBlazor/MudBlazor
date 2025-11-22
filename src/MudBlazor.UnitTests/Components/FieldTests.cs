// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Field;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class FieldTests : BunitTest
    {
        [Test]
        public void FieldTest_ShouldRender_Variants()
        {
            var comp = Context.Render<FieldTest>();
            var fields = comp.FindAll(".mud-grid .mud-input-control.mud-field");
            fields.Should().HaveCount(3);
            fields[0].ClassList.Should().Contain("mud-input-text-with-label");
            fields[0].TextContent.Trim().Should().Be("Standard");
            fields[1].ClassList.Should().Contain("mud-input-filled-with-label");
            fields[1].TextContent.Trim().Should().Be("Filled");
            fields[2].ClassList.Should().Contain("mud-input-outlined-with-label");
            fields[2].TextContent.Trim().Should().Be("OutlinedOutlined"); // Outlined includes a special fieldset label
        }

        [Test]
        public void FieldTest_ShouldRender_AriaAdornment()
        {
            var comp = Context.Render<FieldTest>();
            var fields = comp.FindAll(".mud-grid .mud-input-control.mud-field");
            fields.Should().HaveCount(3);
            fields[0].ClassList.Should().Contain("mud-input-text-with-label");
            fields[0].TextContent.Trim().Should().Be("Standard");
            var adornmentAria = comp.Find(".mud-grid .mud-input-control.mud-field svg.mud-input-adornment-icon");
            // get what adornmentAria aria-label says
            adornmentAria.GetAttribute("aria-label").Trim().Should().Be("test-aria");
        }

        [Test]
        public void FieldTests_ShrinkLabel()
        {
            // Issue 7533, when ChildContent is null, the mud-shrink class is applied
            // Add a shrink label override to the field in addition to the ChildContent
            var comp = Context.Render<FieldStartAdornmentTest>();
            // find all the mud-fields inner area
            var fields = comp.FindAll(".mud-input-control.mud-field > .mud-input-control-input-container > .mud-input");
            var fieldLabels = comp.FindAll(".mud-input-control.mud-field > .mud-input-control-input-container label");
            fields.Should().HaveCount(5);

            // with end adornment no content
            fields[0].ClassList.Should().NotContain("mud-shrink");
            fieldLabels[0].TextContent.Trim().Should().Contain("What am I? (0)");
            // with start adornment        
            fields[1].ClassList.Should().Contain("mud-shrink");
            fieldLabels[1].TextContent.Trim().Should().Be("What am I? (1)");
            // content
            fields[2].ClassList.Should().Contain("mud-shrink");
            fields[2].TextContent.Trim().Should().Be("Some Content Here");
            fieldLabels[2].TextContent.Trim().Should().Be("What am I? (2)");

            // with shrink label override
            //start adornment
            fields[3].ClassList.Should().NotContain("mud-shrink");
            fieldLabels[3].TextContent.Trim().Should().Be("What am I? (3)");
            // content and end adornment
            fields[4].ClassList.Should().NotContain("mud-shrink");
            fields[4].TextContent.Trim().Should().Be("Some Content Here");
            fieldLabels[4].TextContent.Trim().Should().Be("What am I? (4)");
        }
    }
}
