// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Traversals over a backing <see cref="ITreeItemData{T}"/> hierarchy.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
/// <remarks>
/// Every traversal visits each item instance once, so hierarchies which reference the same instance more than once are safe.
/// </remarks>
internal static class TreeViewHierarchy<T>
{
    /// <summary>
    /// Returns the selectable value of an item, using the text as a fallback for string trees.
    /// </summary>
    public static T? GetItemValue(ITreeItemData<T> item)
    {
        if (typeof(T) == typeof(string) && item.Value is null && item.Text is not null)
        {
            return (T)(object)item.Text;
        }

        return item.Value;
    }

    /// <summary>
    /// Expands every expandable item which has children.
    /// </summary>
    /// <returns>Whether any expansion changed.</returns>
    public static bool ExpandAll(IReadOnlyCollection<ITreeItemData<T>>? items)
    {
        var changed = false;
        Visit(items, item =>
        {
            if (item.Expandable && item.HasChildren && !item.Expanded)
            {
                item.Expanded = true;
                changed = true;
            }
        });

        return changed;
    }

    /// <summary>
    /// Collapses every item.
    /// </summary>
    /// <returns>Whether any expansion changed.</returns>
    public static bool CollapseAll(IReadOnlyCollection<ITreeItemData<T>>? items)
    {
        var changed = false;
        Visit(items, item =>
        {
            if (item.Expanded)
            {
                item.Expanded = false;
                changed = true;
            }
        });

        return changed;
    }

    /// <summary>
    /// Makes every item visible without changing expansion.
    /// </summary>
    /// <returns>Whether any visibility changed.</returns>
    public static bool ResetFilter(IReadOnlyCollection<ITreeItemData<T>>? items)
    {
        var changed = false;
        Visit(items, item =>
        {
            if (!item.Visible)
            {
                item.Visible = true;
                changed = true;
            }
        });

        return changed;
    }

    /// <summary>
    /// Filters the hierarchy in post-order: an item is visible and expanded when it or any descendant matches.
    /// </summary>
    /// <returns>Whether any visibility or expansion changed.</returns>
    public static async Task<bool> FilterAsync(
        IReadOnlyCollection<ITreeItemData<T>>? items,
        Func<ITreeItemData<T>, Task<bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var changed = false;
        var visited = new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
        foreach (var item in items ?? [])
        {
            var result = await FilterItemAsync(item, filter, visited);
            changed = result.Changed || changed;
        }

        return changed;

        static async Task<(bool Visible, bool Changed)> FilterItemAsync(
            ITreeItemData<T> item,
            Func<ITreeItemData<T>, Task<bool>> filter,
            HashSet<ITreeItemData<T>> visited)
        {
            if (!visited.Add(item))
            {
                return (item.Visible, false);
            }

            var hasVisibleChild = false;
            var itemChanged = false;
            if (item.HasChildren)
            {
                foreach (var child in item.Children)
                {
                    var childResult = await FilterItemAsync(child, filter, visited);
                    hasVisibleChild = childResult.Visible || hasVisibleChild;
                    itemChanged = childResult.Changed || itemChanged;
                }
            }

            var visible = await filter(item) || hasVisibleChild;
            itemChanged = item.Visible != visible || item.Expanded != visible || itemChanged;
            item.Visible = visible;
            item.Expanded = visible;
            return (visible, itemChanged);
        }
    }

    /// <summary>
    /// Expands the expandable ancestors of every selected item.
    /// </summary>
    /// <returns>Whether any expansion changed.</returns>
    public static bool AutoExpand(
        IReadOnlyCollection<ITreeItemData<T>>? items,
        IReadOnlyCollection<T> selectedValues,
        IEqualityComparer<T?> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        var selection = new HashSet<T>(selectedValues, comparer);
        var changed = false;
        var visited = new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
        foreach (var item in items ?? [])
        {
            ExpandSelectedAncestors(item, selection, visited, ref changed);
        }

        return changed;

        static bool ExpandSelectedAncestors(ITreeItemData<T> item, HashSet<T> selection, HashSet<ITreeItemData<T>> visited, ref bool changed)
        {
            var childContainsSelection = false;
            if (visited.Add(item) && item.HasChildren)
            {
                foreach (var child in item.Children)
                {
                    childContainsSelection = ExpandSelectedAncestors(child, selection, visited, ref changed) || childContainsSelection;
                }
            }

            if (childContainsSelection && item.Expandable && !item.Expanded)
            {
                item.Expanded = true;
                changed = true;
            }

            var value = GetItemValue(item);
            return childContainsSelection || (value is not null && selection.Contains(value));
        }
    }

    private static void Visit(IReadOnlyCollection<ITreeItemData<T>>? items, Action<ITreeItemData<T>> visit)
    {
        var visited = new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
        VisitItems(items ?? [], visit, visited);

        static void VisitItems(IReadOnlyCollection<ITreeItemData<T>> items, Action<ITreeItemData<T>> visit, HashSet<ITreeItemData<T>> visited)
        {
            foreach (var item in items)
            {
                if (!visited.Add(item))
                {
                    continue;
                }

                visit(item);
                if (item.HasChildren)
                {
                    VisitItems(item.Children, visit, visited);
                }
            }
        }
    }
}
