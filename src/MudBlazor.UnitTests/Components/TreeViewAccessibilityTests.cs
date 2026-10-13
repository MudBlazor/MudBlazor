// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.TreeView;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class TreeViewAccessibilityTests : BunitTest
{
    /// <summary>
    /// Multi-selection checkboxes must expose the corresponding item text as their accessible name (#13977).
    /// </summary>
    [Test]
    public void MultiSelection_Checkboxes_AreNamedByTheirItems()
    {
        Context.AddFakeTimeProvider();
        var tree = Context.Render<TreeViewMultiSelectionCheckboxTest>();
        var namedCount = 0;

        foreach (var item in tree.FindAll(".mud-treeview-item-content"))
        {
            var checkbox = item.QuerySelector("input.mud-checkbox-input");
            if (checkbox is null)
            {
                continue;
            }

            var text = item.QuerySelector(".mud-treeview-item-label");
            text.Should().NotBeNull();

            var labelId = checkbox.GetAttribute("aria-labelledby");
            labelId.Should().NotBeNullOrWhiteSpace();
            var label = tree.Find($"[id='{labelId}']");
            label.TextContent.Should().Be(text!.TextContent);
            namedCount++;
        }

        namedCount.Should().BeGreaterThan(1);
    }

    /// <summary>
    /// The checkbox name follows an explicit TreeView item Text instead of its Value (#13977).
    /// </summary>
    [Test]
    public void MultiSelection_CustomItemText_NamesCheckbox()
    {
        Context.AddFakeTimeProvider();
        var tree = Context.Render<MudTreeView<string>>(parameters => parameters
            .Add(p => p.SelectionMode, SelectionMode.MultiSelection)
            .AddChildContent<MudTreeViewItem<string>>(item => item
                .Add(x => x.Value, "documents")
                .Add(x => x.Text, "My documents")));

        var checkbox = tree.Find("input.mud-checkbox-input");
        var labelId = checkbox.GetAttribute("aria-labelledby");

        labelId.Should().NotBeNullOrWhiteSpace();
        tree.Find($"[id='{labelId}']").TextContent.Should().Be("My documents");
    }
}
