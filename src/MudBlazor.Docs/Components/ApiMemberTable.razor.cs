﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a table which displays methods for a documented type.
/// </summary>
public partial class ApiMemberTable
{
    private DocumentedType? _type;

    /// <summary>
    /// This table.
    /// </summary>
    public MudTable<DocumentedMember>? Table { get; set; }

    /// <summary>
    /// The text to search for.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// The type to display members for.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public DocumentedType? Type
    {
        get => _type;
        set
        {
            _type = value;
            Table?.ReloadServerData();
        }
    }

    /// <summary>
    /// The kind of member to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public ApiMemberTableMode Mode { get; set; }

    /// <summary>
    /// The currently selected grouping.
    /// </summary>
    [Parameter]
    public ApiMemberGrouping Grouping { get; set; } = ApiMemberGrouping.Categories;

    /// <summary>
    /// Shows an icon for properties marked with <c>[Parameter]</c>.
    /// </summary>
    [Parameter]
    public bool ShowParameters { get; set; } = false;

    /// <summary>
    /// Shows a bindable icon for properties or events which support <c>@bind-</c>.
    /// </summary>
    [Parameter]
    public bool ShowBindable { get; set; } = true;

    /// <summary>
    /// Requests data for the table.
    /// </summary>
    /// <param name="state">The current table state.</param>
    /// <param name="token">A <see cref="CancellationToken"/> for aborting ongoing requests.</param>
    /// <returns></returns>
    public Task<TableData<DocumentedMember>> GetData(TableState state, CancellationToken token)
    {
        if (Type == null || Mode == ApiMemberTableMode.None)
        {
            return Task.FromResult(new TableData<DocumentedMember>() { Items = [] });
        }

        // Get members for the desired mode
        var members = Mode switch
        {
            ApiMemberTableMode.Events => Type.Events.Values.AsQueryable(),
            ApiMemberTableMode.Fields => Type.Fields.Values.AsQueryable(),
            ApiMemberTableMode.Methods => Type.Methods.Values.AsQueryable(),
            ApiMemberTableMode.Properties => Type.Properties.Values.AsQueryable(),
            _ => new List<DocumentedMember>().AsQueryable(),
        };

        // Is there a keyword?
        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            members = members.Where(member => member.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase)
                || (member.Summary != null && member.Summary.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                || (member.Remarks != null && member.Remarks.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
            );
        }

        // What's the grouping?
        if (Grouping == ApiMemberGrouping.None)
        {
            // Order by the column
            members = state.SortLabel switch
            {
                "Description" => state.SortDirection == SortDirection.Ascending ? members.OrderBy(member => member.Summary) : members.OrderByDescending(member => member.Summary),
                "Name" => state.SortDirection == SortDirection.Ascending ? members.OrderBy(member => member.Name) : members.OrderByDescending(member => member.Name),
                "Return Type" => state.SortDirection == SortDirection.Ascending ? members.OrderBy(member => member.TypeFriendlyName) : members.OrderByDescending(member => member.TypeFriendlyName),
                "Type" => state.SortDirection == SortDirection.Ascending ? members.OrderBy(member => member.TypeFriendlyName) : members.OrderByDescending(member => member.TypeFriendlyName),
                _ => state.SortDirection == SortDirection.Ascending ? members.OrderBy(member => member.Name) : members.OrderByDescending(member => member.Name),
            };
        }
        else if (Grouping == ApiMemberGrouping.Categories)
        {
            // Sort by category
            var orderedMembers = members.OrderBy(member => member.Order).ThenBy(member => member.Category);

            // ... then by sort column
            members = state.SortLabel switch
            {
                "Description" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.Summary) : orderedMembers.ThenByDescending(member => member.Summary),
                "Name" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.Name) : orderedMembers.ThenByDescending(member => member.Name),
                "Return Type" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.TypeFriendlyName) : orderedMembers.ThenByDescending(member => member.TypeFriendlyName),
                "Type" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.TypeFriendlyName) : orderedMembers.ThenByDescending(member => member.TypeFriendlyName),
                _ => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.Name) : orderedMembers.ThenByDescending(member => member.Name),
            };
        }
        else if (Grouping == ApiMemberGrouping.Inheritance)
        {
            // Sort by declaring type
            var orderedMembers = members.OrderBy(member => GetInheritanceLevel(member.DeclaringType)).ThenBy(member => member.DeclaringType!.NameFriendly);

            // ... then by sort column
            members = state.SortLabel switch
            {
                "Description" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.Summary) : orderedMembers.ThenByDescending(member => member.Summary),
                "Name" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.Name) : orderedMembers.ThenByDescending(member => member.Name),
                "Return Type" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.TypeFriendlyName) : orderedMembers.ThenByDescending(member => member.TypeFriendlyName),
                "Type" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.TypeFriendlyName) : orderedMembers.ThenByDescending(member => member.TypeFriendlyName),
                _ => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(member => member.Name) : orderedMembers.ThenByDescending(member => member.Name),
            };
        }

        // Make the final results
        var results = members.ToList();

        // What categories are selected?
        return Task.FromResult(new TableData<DocumentedMember>()
        {
            Items = results,
            TotalItems = results.Count,
        });
    }

    /// <summary>
    /// Occurs when <see cref="Keyword"/> has changed.
    /// </summary>
    /// <param name="keyword">The text to search for.</param>
    public Task OnKeywordChangedAsync(string keyword)
    {
        Keyword = keyword;
        return Table!.ReloadServerData();
    }

    /// <summary>
    /// Occurs when the table grouping has changed.
    /// </summary>
    /// <param name="grouping">The new grouping.</param>
    public Task OnGroupingChangedAsync(ApiMemberGrouping grouping)
    {
        Grouping = grouping;
        return Table!.ReloadServerData();
    }

    /// <summary>
    /// Occurs when the table grouping has changed.
    /// </summary>
    /// <param name="grouping"></param>
    public Variant GetGroupingVariant(ApiMemberGrouping grouping)
        => Grouping == grouping ? Variant.Filled : Variant.Outlined;

    /// <summary>
    /// Gets the depth of the specified type relative to this type.
    /// </summary>
    /// <param name="otherType">The type to compare.</param>
    /// <returns>The depth of the specified class relative to this class.</returns>
    public int GetInheritanceLevel(DocumentedType? otherType)
    {
        // Is no type specified?  If so, we can't do anything
        if (otherType == null) return 0;

        // Is the other type the same as this?
        if (otherType == this.Type) return 0;

        // Walk down to the base class
        var baseType = this.Type?.BaseType;
        var level = 1;

        // Are we at the specified type?
        while (baseType != otherType)
        {
            // No, go one level deeper
            level++;
            baseType = baseType!.BaseType;
            // Prevent infinite loops just in case
            if (baseType == null)
            {
                break;
            }
        }
        return level;
    }

    /// <summary>
    /// The current groups.
    /// </summary>
    public TableGroupDefinition<DocumentedMember>? CurrentGroups
    {
        get
        {
            return Grouping switch
            {
                ApiMemberGrouping.Categories => new() { Selector = (property) => property.Category ?? "" },
                ApiMemberGrouping.Inheritance => new() { Selector = (property) => (property.DeclaringType is not null && property.DeclaringType == this.Type) ? property.DeclaringType?.NameFriendly ?? "" : $"Inherited from {property.DeclaringType?.NameFriendly}" },
                _ => null
            };
        }
    }

    [Inject]
    private NavigationManager? Browser { get; set; }

    /// <summary>
    /// Occurs when a declaring type has been clicked.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public void OnDeclaringTypeClicked(string url)
    {
        // Force a new load (otherwise MudChips won't properly update)
        Browser?.NavigateTo(url, false);
    }
}
