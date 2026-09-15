// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class TreeViewProjectionTests
{
    /// <summary>
    /// Verifies that a rebuild flattens only expanded visible rows and records visible sibling metadata.
    /// </summary>
    [Test]
    public void RebuildFlattensExpandedVisibleRowsWithSiblingMetadata()
    {
        var hiddenChild = new TreeItemData<string> { Value = "Hidden", Visible = false };
        var visibleChild = new TreeItemData<string> { Value = "Visible" };
        var collapsedGrandchild = new TreeItemData<string> { Value = "Collapsed grandchild" };
        var collapsedChild = new TreeItemData<string>
        {
            Value = "Collapsed child",
            Children = [collapsedGrandchild]
        };
        var root = new TreeItemData<string>
        {
            Value = "Root",
            Expanded = true,
            Children = [hiddenChild, visibleChild, collapsedChild]
        };
        var otherRoot = new TreeItemData<string> { Value = "Other root" };
        var projection = new TreeViewProjection<string>();

        projection.Rebuild([root, otherRoot], [], StringComparer.Ordinal);

        projection.Rows.Select(row => row.Item.Value)
            .Should().Equal("Root", "Visible", "Collapsed child", "Other root");
        projection.Rows.Select(row => row.Depth).Should().Equal(0, 1, 1, 0);
        projection.Rows.Select(row => row.PositionInSet).Should().Equal(1, 1, 2, 2);
        projection.Rows.Select(row => row.SetSize).Should().Equal(2, 2, 2, 2);
        projection.Rows.Should().NotContain(row => ReferenceEquals(row.Item, collapsedGrandchild));
    }

    /// <summary>
    /// Verifies that repeated references to one item get distinct render keys and are found by key.
    /// </summary>
    [Test]
    public void RebuildAssignsDistinctRowKeysToRepeatedReferences()
    {
        var shared = new TreeItemData<string> { Value = "Shared" };
        var parent = new TreeItemData<string> { Value = "Parent", Expanded = true, Children = [shared] };
        var projection = new TreeViewProjection<string>();

        projection.Rebuild([shared, parent], [], StringComparer.Ordinal);

        var keys = projection.Rows.Select(row => row.RowKey).ToList();
        keys.Should().OnlyHaveUniqueItems();
        keys.Should().Contain(new TreeViewRowKey<string>(shared, 0));
        keys.Should().Contain(new TreeViewRowKey<string>(shared, 1));
        projection.FindRow(new TreeViewRowKey<string>(shared, 1)).Should().BeSameAs(projection.Rows[2]);
        projection.FindRow(new TreeViewRowKey<string>(shared, 2)).Should().BeNull();
    }

    /// <summary>
    /// Verifies that a parent with only filtered direct children is projected as a visible leaf.
    /// </summary>
    [Test]
    public void RebuildTreatsFilteredDirectChildrenAsNotVisible()
    {
        var parent = new TreeItemData<string>
        {
            Value = "Parent",
            Expanded = true,
            Children =
            [
                new TreeItemData<string> { Value = "Hidden one", Visible = false },
                new TreeItemData<string> { Value = "Hidden two", Visible = false }
            ]
        };
        var projection = new TreeViewProjection<string>();

        projection.Rebuild([parent], [], StringComparer.Ordinal);

        projection.Rows.Should().ContainSingle();
        projection.Rows[0].HasVisibleChildren.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that collapsed descendants still contribute to the projected checkbox state.
    /// </summary>
    [Test]
    public void RebuildIncludesCollapsedDescendantsInCheckboxState()
    {
        var root = new TreeItemData<string>
        {
            Value = "Root",
            Expanded = false,
            Children = [new TreeItemData<string> { Value = "Collapsed child" }]
        };
        var projection = new TreeViewProjection<string>();

        projection.Rebuild([root], ["Root"], StringComparer.Ordinal);

        projection.Rows.Should().ContainSingle();
        projection.Rows[0].HasSelectableValues.Should().BeTrue();
        projection.Rows[0].IsSelected.Should().BeTrue();
        projection.Rows[0].CheckState.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a null leaf has neither selection nor mixed checkbox semantics.
    /// </summary>
    [Test]
    public void RebuildProjectsNullLeafWithoutSelectionSemantics()
    {
        var leaf = new TreeItemData<int?> { Text = "Null leaf" };
        var projection = new TreeViewProjection<int?>();

        projection.Rebuild([leaf], [], EqualityComparer<int?>.Default);

        var row = projection.Rows.Should().ContainSingle().Subject;
        row.HasSelectableValues.Should().BeFalse();
        row.IsSelected.Should().BeFalse();
        row.CheckState.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a null structural parent derives true and mixed states from selectable descendants.
    /// </summary>
    [Test]
    public void RebuildProjectsNullStructuralParentFromDescendants()
    {
        var firstChild = new TreeItemData<int?> { Value = 1 };
        var secondChild = new TreeItemData<int?> { Value = 2 };
        var parent = new TreeItemData<int?>
        {
            Text = "Parent",
            Children = [firstChild, secondChild]
        };
        var projection = new TreeViewProjection<int?>();

        projection.Rebuild([parent], [1, 2], EqualityComparer<int?>.Default);

        var allSelectedRow = projection.Rows.Should().ContainSingle().Subject;
        allSelectedRow.HasSelectableValues.Should().BeTrue();
        allSelectedRow.IsSelected.Should().BeFalse();
        allSelectedRow.CheckState.Should().BeTrue();

        projection.Rebuild([parent], [1], EqualityComparer<int?>.Default);

        projection.Rows.Should().ContainSingle().Subject.CheckState.Should().BeNull();
        allSelectedRow.CheckState.Should().BeTrue("row snapshots remain immutable after a rebuild");
    }

    /// <summary>
    /// Verifies that backing multi-selection changes win after initialization while values without a backing item are kept, as in a standard tree.
    /// </summary>
    [Test]
    public void ReconcileSelectionReflectsExternalMultiSelectionAndKeepsMissingValues()
    {
        var first = new TreeItemData<string> { Value = "First" };
        var second = new TreeItemData<string> { Value = "Second" };
        var projection = new TreeViewProjection<string>();
        projection.Rebuild([first, second], ["Second"], StringComparer.Ordinal);
        var initial = projection.ReconcileSelection(true, null, ["Second"]);
        first.Selected = true;
        second.Selected = false;

        var changed = projection.ReconcileSelection(true, null, initial.SelectedValues);

        changed.SelectedValues.Should().Equal("First");
        changed.SelectionChanged.Should().BeTrue();
        changed.BackingItemsChanged.Should().BeFalse();

        projection.Rebuild([first], changed.SelectedValues.Concat(["Second"]).ToArray(), StringComparer.Ordinal);

        var replaced = projection.ReconcileSelection(true, null, changed.SelectedValues.Concat(["Second"]).ToArray());

        replaced.SelectedValues.Should().BeEquivalentTo(["First", "Second"]);
        replaced.SelectionChanged.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that backing single-selection changes win after initialization while a selected value without a backing item is kept, as in a standard tree.
    /// </summary>
    [Test]
    public void ReconcileSelectionReflectsExternalSingleSelectionAndKeepsMissingValue()
    {
        var first = new TreeItemData<string> { Value = "First" };
        var second = new TreeItemData<string> { Value = "Second" };
        var projection = new TreeViewProjection<string>();
        projection.Rebuild([first, second], ["First"], StringComparer.Ordinal);
        var initial = projection.ReconcileSelection(false, "First", null);
        first.Selected = false;
        second.Selected = true;

        var changed = projection.ReconcileSelection(false, initial.SelectedValue, null);

        changed.SelectedValue.Should().Be("Second");
        changed.SelectionChanged.Should().BeTrue();
        changed.BackingItemsChanged.Should().BeFalse();

        projection.Rebuild([first], ["Second"], StringComparer.Ordinal);

        var replaced = projection.ReconcileSelection(false, changed.SelectedValue, null);

        replaced.SelectedValue.Should().Be("Second");
        replaced.SelectionChanged.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that selected values synchronize all represented backing items using the comparer and string text fallback.
    /// </summary>
    [Test]
    public void SynchronizeSelectionUpdatesBackingItemsUsingComparerAndTextFallback()
    {
        var textOnly = new TreeItemData<string> { Text = "Alpha" };
        var duplicate = new TreeItemData<string> { Value = "ALPHA" };
        var other = new TreeItemData<string> { Value = "Other", Selected = true };
        var projection = new TreeViewProjection<string>();

        var represented = projection.SynchronizeSelection(
            [textOnly, duplicate, other],
            ["alpha"],
            StringComparer.OrdinalIgnoreCase);

        represented.Should().BeEquivalentTo(["alpha"]);
        textOnly.Selected.Should().BeTrue();
        duplicate.Selected.Should().BeTrue();
        other.Selected.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that a subtree toggle updates selectable descendants and then applies the direct-child rule to each ancestor.
    /// </summary>
    [Test]
    public void ToggleSubtreeSelectionUpdatesDescendantsAndAncestors()
    {
        var leaf = new TreeItemData<string> { Value = "Leaf" };
        var structuralNode = new TreeItemData<string> { Children = [leaf] };
        var target = new TreeItemData<string> { Value = "Target", Children = [structuralNode] };
        var parent = new TreeItemData<string> { Value = "Parent", Expanded = true, Children = [target] };
        var root = new TreeItemData<string> { Value = "Root", Expanded = true, Children = [parent] };
        var projection = new TreeViewProjection<string>();
        projection.Rebuild([root], [], StringComparer.Ordinal);

        var selected = projection.ToggleSubtreeSelection(projection.Rows.Single(row => ReferenceEquals(row.Item, target)), [], true);

        selected.Should().BeEquivalentTo(["Root", "Parent", "Target", "Leaf"]);
        root.Selected.Should().BeTrue();
        parent.Selected.Should().BeTrue();
        target.Selected.Should().BeTrue();
        structuralNode.Selected.Should().BeFalse();
        leaf.Selected.Should().BeTrue();

        var deselected = projection.ToggleSubtreeSelection(projection.Rows.Single(row => ReferenceEquals(row.Item, target)), selected, true);

        deselected.Should().BeEmpty();
        root.Selected.Should().BeFalse();
        parent.Selected.Should().BeFalse();
        target.Selected.Should().BeFalse();
        leaf.Selected.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that toggling a repeated reference only updates the ancestors of the toggled row.
    /// </summary>
    [Test]
    public void ToggleSubtreeSelectionUsesToggledRowAncestors()
    {
        var shared = new TreeItemData<string> { Value = "Shared" };
        var parent = new TreeItemData<string> { Value = "Parent", Expanded = true, Children = [shared] };
        var projection = new TreeViewProjection<string>();
        projection.Rebuild([shared, parent], [], StringComparer.Ordinal);

        var selected = projection.ToggleSubtreeSelection(projection.Rows[0], [], true);

        selected.Should().BeEquivalentTo(["Shared"]);
        parent.Selected.Should().BeFalse();

        var selectedThroughParent = projection.ToggleSubtreeSelection(projection.Rows[2], [], true);

        selectedThroughParent.Should().BeEquivalentTo(["Shared", "Parent"]);
        parent.Selected.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that auto-expand finds selected filtered descendants and expands only expandable ancestors.
    /// </summary>
    [Test]
    public void AutoExpandUsesAllDescendantsAndHonorsExpandable()
    {
        var selectedLeaf = new TreeItemData<string> { Value = "Selected", Visible = false };
        var nestedParent = new TreeItemData<string> { Value = "Nested", Children = [selectedLeaf] };
        var fixedParent = new TreeItemData<string> { Value = "Fixed", Expandable = false, Children = [nestedParent] };
        var root = new TreeItemData<string> { Value = "Root", Children = [fixedParent] };
        var projection = new TreeViewProjection<string>();
        projection.Rebuild([root], ["Selected"], StringComparer.Ordinal);

        var changed = projection.AutoExpand(["Selected"]);

        changed.Should().BeTrue();
        root.Expanded.Should().BeTrue();
        fixedParent.Expanded.Should().BeFalse();
        nestedParent.Expanded.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that hierarchy auto-expand mirrors the projected rule on the backing data.
    /// </summary>
    [Test]
    public void HierarchyAutoExpandExpandsExpandableAncestorsOfSelectedItems()
    {
        var selectedLeaf = new TreeItemData<string> { Value = "Selected" };
        var fixedParent = new TreeItemData<string> { Value = "Fixed", Expandable = false, Children = [selectedLeaf] };
        var root = new TreeItemData<string> { Value = "Root", Children = [fixedParent] };

        TreeViewHierarchy<string>.AutoExpand([root], ["Selected"], StringComparer.Ordinal).Should().BeTrue();

        root.Expanded.Should().BeTrue();
        fixedParent.Expanded.Should().BeFalse();
        TreeViewHierarchy<string>.AutoExpand([root], ["Selected"], StringComparer.Ordinal).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that bulk expansion and collapse mutate the backing hierarchy and accurately report changes.
    /// </summary>
    [Test]
    public void ExpandAllAndCollapseAllUpdateBackingExpansion()
    {
        var grandchild = new TreeItemData<string> { Value = "Grandchild", Expanded = true };
        var child = new TreeItemData<string> { Value = "Child", Children = [grandchild] };
        var fixedParent = new TreeItemData<string> { Value = "Fixed", Expandable = false, Children = [child] };
        var root = new TreeItemData<string> { Value = "Root", Children = [fixedParent] };

        TreeViewHierarchy<string>.ExpandAll([root]).Should().BeTrue();

        root.Expanded.Should().BeTrue();
        fixedParent.Expanded.Should().BeFalse();
        child.Expanded.Should().BeTrue();
        TreeViewHierarchy<string>.ExpandAll([root]).Should().BeFalse();

        TreeViewHierarchy<string>.CollapseAll([root]).Should().BeTrue();

        root.Expanded.Should().BeFalse();
        child.Expanded.Should().BeFalse();
        grandchild.Expanded.Should().BeFalse();
        TreeViewHierarchy<string>.CollapseAll([root]).Should().BeFalse();
    }

    /// <summary>
    /// Verifies post-order filtering and that reset restores visibility without changing expansion.
    /// </summary>
    [Test]
    public async Task FilterAndResetUseBackingHierarchyRules()
    {
        var matchingGrandchild = new TreeItemData<string> { Value = "Match", Visible = false };
        var child = new TreeItemData<string> { Value = "Child", Children = [matchingGrandchild] };
        var root = new TreeItemData<string> { Value = "Root", Children = [child] };
        var other = new TreeItemData<string> { Value = "Other" };

        var changed = await TreeViewHierarchy<string>.FilterAsync([root, other], item => Task.FromResult(item.Value == "Match"));

        changed.Should().BeTrue();
        root.Visible.Should().BeTrue();
        root.Expanded.Should().BeTrue();
        child.Visible.Should().BeTrue();
        child.Expanded.Should().BeTrue();
        matchingGrandchild.Visible.Should().BeTrue();
        matchingGrandchild.Expanded.Should().BeTrue();
        other.Visible.Should().BeFalse();
        other.Expanded.Should().BeFalse();

        TreeViewHierarchy<string>.ResetFilter([root, other]).Should().BeTrue();

        other.Visible.Should().BeTrue();
        root.Expanded.Should().BeTrue();
        child.Expanded.Should().BeTrue();
        matchingGrandchild.Expanded.Should().BeTrue();
        other.Expanded.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that hierarchy traversals visit a repeated reference once.
    /// </summary>
    [Test]
    public void HierarchyTraversalsVisitRepeatedReferencesOnce()
    {
        var shared = new TreeItemData<string> { Value = "Shared", Children = [new TreeItemData<string> { Value = "Leaf" }] };
        var root = new TreeItemData<string> { Value = "Root", Children = [shared, shared] };

        TreeViewHierarchy<string>.ExpandAll([root]).Should().BeTrue();

        shared.Expanded.Should().BeTrue();
        TreeViewHierarchy<string>.ExpandAll([root]).Should().BeFalse();
    }

    /// <summary>
    /// Verifies that server-load entries are keyed by backing item reference and reset supersedes the current generation.
    /// </summary>
    [Test]
    public void ServerLoadEntriesUseBackingItemReferenceIdentityAndResetSupersedes()
    {
        var first = new TreeItemData<string> { Value = "Same" };
        var second = new TreeItemData<string> { Value = "Same" };
        var state = new TreeViewServerLoadState<string>();

        var entry = state.GetEntry(first);
        entry.Version++;
        entry.IsLoading = true;

        state.GetEntry(first).Should().BeSameAs(entry);
        state.GetEntry(second).Should().NotBeSameAs(entry);
        state.GetEntry(second).IsLoading.Should().BeFalse();

        var reserved = entry.Reset();

        reserved.Should().Be(2);
        entry.Version.Should().Be(2);
        entry.IsLoading.Should().BeFalse();
        entry.IsLoaded.Should().BeFalse();
    }
}
