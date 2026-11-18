// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.Docs.Components;
using MudBlazor.UnitTests.Shared;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Tests for the <see cref="SectionHeader"/> component.
/// </summary>
[TestFixture]
public sealed class SectionHeaderTests : BunitTest
{
    /// <summary>
    /// A top-level section is an h2. The element used to follow Typo, which made the page's
    /// highest heading an h4 and collapsed every nesting level past the first into h6.
    /// </summary>
    [Test]
    public void TopLevelSectionRendersAnH2()
    {
        var comp = Context.Render<SectionHeader>(parameters => parameters
            .Add(x => x.Title, "Filled Buttons"));

        comp.Find("h2").TextContent.Should().Contain("Filled Buttons");
    }

    /// <summary>
    /// The visual scale is unchanged: the element moved, the typography class did not.
    /// </summary>
    [Test]
    public void HeadingKeepsItsTypographyClass()
    {
        var comp = Context.Render<SectionHeader>(parameters => parameters
            .Add(x => x.Title, "Filled Buttons"));

        comp.Find("h2").ClassList.Should().Contain("mud-typography-h5");
    }

    /// <summary>
    /// A subtitle is not a heading and must not appear in the document outline.
    /// </summary>
    [Test]
    public void SubTitleIsNotRenderedAsAHeading()
    {
        var comp = Context.Render<SectionHeader>(parameters => parameters
            .Add(x => x.Title, "Filled Buttons")
            .Add(x => x.SubTitle, builder => builder.AddContent(0, "A subtitle")));

        comp.FindAll("h6").Should().BeEmpty();
        comp.Find("p").TextContent.Should().Contain("A subtitle");
    }

    /// <summary>
    /// The element id must not change between renders. The fallback previously called
    /// Guid.NewGuid() from inside the render path, handing the element a new id every pass.
    /// </summary>
    [Test]
    public void SectionIdIsStableAcrossRenders()
    {
        var comp = Context.Render<SectionHeader>(parameters => parameters
            .Add(x => x.Title, "Filled Buttons"));

        var first = comp.Find("div.docs-section-header").GetAttribute("id");

        comp.Render();
        var second = comp.Find("div.docs-section-header").GetAttribute("id");

        comp.Render();
        var third = comp.Find("div.docs-section-header").GetAttribute("id");

        first.Should().NotBeNullOrWhiteSpace();
        second.Should().Be(first);
        third.Should().Be(first);
    }

    /// <summary>
    /// A section with no title renders no heading at all.
    /// </summary>
    [Test]
    public void SectionWithoutATitleRendersNoHeading()
    {
        var comp = Context.Render<SectionHeader>(parameters => parameters
            .Add(x => x.Description, builder => builder.AddContent(0, "Just a description")));

        comp.FindAll("h1, h2, h3, h4, h5, h6").Should().BeEmpty();
    }
}
