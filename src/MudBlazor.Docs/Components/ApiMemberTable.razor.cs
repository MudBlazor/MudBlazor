// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a table which displays methods for a documented type.
/// </summary>
public partial class ApiMemberTable
{
    /// <summary>
    /// This table.
    /// </summary>
    public MudTable<DocumentedMember>? Table { get; set; }

    /// <summary>
    /// The name of the type to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string? TypeName { get; set; }

    /// <summary>
    /// The type to display members for.
    /// </summary>
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The kind of member to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public ApiMemberTableMode Mode { get; set; }

    /// <summary>
    /// The currently selected grouping.
    /// </summary>
    public ApiMemberGrouping CurrentGrouping { get; set; } = ApiMemberGrouping.Categories!;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        // Has the type to display changed?
        if (Type == null || Type.Name != TypeName)
        {
            // Load the new type
            Type = ApiDocumentation.GetType(TypeName);
            // Is a table available?
            if (Table != null)
            {
                // Yup.  Reload it
                await Table.ReloadServerData();
            }
        }
    }

    /// <summary>
    /// Requests data for the table.
    /// </summary>
    /// <param name="state">The current table state.</param>
    /// <param name="token">A <see cref="CancellationToken"/> for aborting ongoing requests.</param>
    /// <returns></returns>
    public async Task<TableData<DocumentedMember>> GetData(TableState state, CancellationToken token)
    {
        if (Type == null || Mode == ApiMemberTableMode.None)
        {
            return new TableData<DocumentedMember> { };
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

        // What's the grouping?
        if (CurrentGrouping == ApiMemberGrouping.Categories)
        {
            // Sort by category
            var orderedMembers = members.OrderBy(property => property.Order).ThenBy(property => property.Category);

            // ... then by sort column
            members = state.SortLabel switch
            {
                "Description" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(property => property.Summary) : orderedMembers.ThenByDescending(property => property.Summary),
                "Name" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(property => property.Name) : orderedMembers.ThenByDescending(property => property.Name),
                "Return Type" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(property => property.TypeFriendlyName) : orderedMembers.ThenByDescending(property => property.TypeFriendlyName),
                "Type" => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(property => property.TypeFriendlyName) : orderedMembers.ThenByDescending(property => property.TypeFriendlyName),
                _ => state.SortDirection == SortDirection.Ascending ? orderedMembers.ThenBy(property => property.Name) : orderedMembers.ThenByDescending(property => property.Name),
            };
        }

        // Make the final results
        var results = members.ToList();

        // What categories are selected?
        return await Task.FromResult(new TableData<DocumentedMember>()
        {
            Items = results,
            TotalItems = results.Count,
        });
    }

    /// <summary>
    /// The current groups.
    /// </summary>
    public TableGroupDefinition<DocumentedMember> CurrentGroups
    {
        get
        {
            return CurrentGrouping switch
            {
                ApiMemberGrouping.Categories => new() { Selector = (property) => property.Category ?? "" },
                ApiMemberGrouping.Inheritance => new() { Selector = (property) => property.DeclaringType?.Name ?? "" },
                _ => new() { Selector = (property) => property.Category ?? "" }
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
