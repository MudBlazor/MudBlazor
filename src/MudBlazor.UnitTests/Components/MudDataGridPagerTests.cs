// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class MudDataGridPagerTests : BunitTest
{
    /// <summary>
    /// The rows-per-page combobox references its visible caption, including custom text (#13978).
    /// </summary>
    [TestCase(null)]
    [TestCase("Grid entries per page:")]
    public void RowsPerPageSelect_ReferencesVisibleCaption(string? customCaption)
    {
        Context.AddFakeTimeProvider();
        var pager = Context.Render<MudDataGridPager<int>>(parameters => parameters
            .Add(p => p.RowsPerPageString, customCaption ?? string.Empty));

        var combobox = pager.Find("[role='combobox']");
        var captionId = combobox.GetAttribute("aria-labelledby");

        captionId.Should().NotBeNullOrWhiteSpace();
        var caption = pager.Find($"[id='{captionId}']");
        caption.TextContent.Should().NotBeNullOrWhiteSpace();
        if (customCaption is not null)
        {
            caption.TextContent.Should().Be(customCaption);
        }
    }

    /// <summary>
    /// Each pager instance has its own caption ID so accessible names do not collide (#13978).
    /// </summary>
    [Test]
    public void MultiplePagers_HaveDistinctCaptionIds()
    {
        Context.AddFakeTimeProvider();
        var first = Context.Render<MudDataGridPager<int>>();
        var second = Context.Render<MudDataGridPager<int>>();

        var firstId = first.Find("[role='combobox']").GetAttribute("aria-labelledby");
        var secondId = second.Find("[role='combobox']").GetAttribute("aria-labelledby");

        firstId.Should().NotBeNullOrWhiteSpace();
        secondId.Should().NotBeNullOrWhiteSpace();
        firstId.Should().NotBe(secondId);
    }

    /// <summary>
    /// Hiding the page-size selector does not leave an unlabelled combobox behind (#13978).
    /// </summary>
    [Test]
    public void HiddenRowsPerPageSelect_DoesNotRenderCombobox()
    {
        Context.AddFakeTimeProvider();
        var pager = Context.Render<MudDataGridPager<int>>(parameters => parameters
            .Add(p => p.PageSizeSelector, false));

        pager.FindAll("[role='combobox']").Should().BeEmpty();
        pager.FindAll("[id^='datagridpager-rows-per-page']").Should().BeEmpty();
    }
}
