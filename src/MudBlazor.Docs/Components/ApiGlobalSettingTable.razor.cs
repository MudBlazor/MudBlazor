// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a table which displays global settings for a documented type.
/// </summary>
public partial class ApiGlobalSettingTable
{
    /// <summary>
    /// This table.
    /// </summary>
    public MudTable<DocumentedProperty>? Table { get; set; }

    /// <summary>
    /// The type to display methods for.
    /// </summary>
    [Parameter]
    public DocumentedType? Type { get; set; }

    /// <summary>
    /// The currently selected grouping.
    /// </summary>
    public ApiMemberGrouping CurrentGrouping { get; set; } = ApiMemberGrouping.Categories!;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (Table != null)
        {
            await Table.ReloadServerData();
        }
    }

    /// <summary>
    /// Requests data for the table.
    /// </summary>
    /// <param name="state">The current table state.</param>
    /// <param name="token">A <see cref="CancellationToken"/> for aborting ongoing requests.</param>
    /// <returns></returns>
    public async Task<TableData<DocumentedProperty>> GetData(TableState state, CancellationToken token)
    {
        if (Type == null)
        {
            return new TableData<DocumentedProperty> { };
        }

        // Get properties which are in the selected categories
        var properties = Type.GlobalSettings.Values.AsQueryable();

        // What's the grouping?
        if (CurrentGrouping == ApiMemberGrouping.Categories)
        {
            // Sort by category
            var orderedProperties = properties.OrderBy(property => property.Category);

            // ... then by sort column
            orderedProperties = state.SortLabel switch
            {
                "Name" => state.SortDirection == SortDirection.Ascending ? orderedProperties.ThenBy(property => property.Name) : orderedProperties.ThenByDescending(property => property.Name),
                "Type" => state.SortDirection == SortDirection.Ascending ? orderedProperties.ThenBy(property => property.Type) : orderedProperties.ThenByDescending(property => property.Type),
                "Description" => state.SortDirection == SortDirection.Ascending ? orderedProperties.ThenBy(property => property.Summary) : orderedProperties.ThenByDescending(property => property.Summary),
                _ => state.SortDirection == SortDirection.Ascending ? orderedProperties.ThenBy(property => property.Name) : orderedProperties.ThenByDescending(property => property.Name),
            };

            properties = orderedProperties;
        }

        // Make the final results
        var results = properties.ToList();

        // What categories are selected?
        return await Task.FromResult(new TableData<DocumentedProperty>()
        {
            Items = results,
            TotalItems = results.Count,
        });
    }

    /// <summary>
    /// The current groups.
    /// </summary>
    public TableGroupDefinition<DocumentedProperty> CurrentGroups
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
