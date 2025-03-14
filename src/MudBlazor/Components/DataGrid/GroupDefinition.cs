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
    private bool _indentation = true;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="grouping">The LINQ definition of the grouping.</param>
    /// <param name="expanded">Expands this group.</param>
    public GroupDefinition(IGrouping<object, T> grouping, bool expanded)
    {
        Grouping = grouping;
        Expanded = expanded;
    }

    /// <summary>
    /// The LINQ definition of the grouping.
    /// </summary>
    public IGrouping<object, T> Grouping { get; set; }

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
    /// Indents the first column cell for this group and child groups.
    /// </summary>
    /// <remarks>
    /// When set, all child group definitions are also updated.  Must be set for the first grouping level.
    /// <para>Defaults to <c>true</c>.</para>
    /// </remarks>
    public bool Indentation
    {
        get => _indentation;
        set
        {
            _indentation = value;
            if (InnerGroup is not null)
            {
                InnerGroup.Indentation = value;
            }
        }
    }

    /// <summary>
    /// The parent group definition.
    /// </summary>
    internal GroupDefinition<T>? Parent { get; private set; }

    /// <summary>
    /// Gets the nesting level of this group.
    /// </summary>
    internal int Level
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
