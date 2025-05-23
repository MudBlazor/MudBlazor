// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents the grouping information for columns in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GroupDefinition<T>
{
    private GroupDefinition<T>? _innerGroup;

    /// <summary>
    /// The LINQ definition of the grouping.
    /// </summary>
    public required IGrouping<object?, T> Grouping { get; set; }

    /// <summary>
    /// The function which selects items for this group.
    /// </summary>
    /// <remarks>
    /// Typically used during a LINQ <c>GroupBy()</c> call to group items.
    /// </remarks>
    public Func<T, object> Selector { get; set; } = default!;

    /// <summary>
    /// Expands this group.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>False</c>.
    /// </remarks>
    public bool Expanded { get; set; }

    /// <summary>
    /// The template for the grouped column.
    /// </summary>
    public RenderFragment<GroupDefinition<T>>? GroupTemplate { get; set; }

    /// <summary>
    /// The title of the grouped column
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The group definition within this definition.
    /// </summary>
    public GroupDefinition<T>? InnerGroup
    {
        get => _innerGroup;
        set
        {
            if (_innerGroup is not null)
            {
                _innerGroup.Parent = null;
            }

            _innerGroup = value;

            if (_innerGroup is not null)
            {
                _innerGroup.Parent = this;
                _innerGroup.Indentation = Indentation;
            }
        }
    }

    /// <summary>
    /// Indents the each Group beyond the first by 48 px.
    /// </summary>
    public bool Indentation { get; set; } = true;

    /// <summary>
    /// The parent group definition.
    /// </summary>
    internal GroupDefinition<T>? Parent { get; set; }

    /// <summary>
    /// Gets the nesting level of this group.
    /// </summary>
    public int Level
    {
        get
        {
            if (Parent is null)
            {
                return 1;
            }

            return Parent.Level + 1;
        }
    }
}
