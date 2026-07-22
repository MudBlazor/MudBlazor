// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents the grouping information for columns in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GroupDefinition<T>
{
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
    /// Creates a new instance.
    /// </summary>
    /// <param name="grouping">The LINQ definition of the grouping.</param>
    /// <param name="expanded">Expands this group.</param>
    public GroupDefinition(IGrouping<object, T> grouping, bool expanded)
    {
        Grouping = grouping;
        Expanded = expanded;
    }
}
