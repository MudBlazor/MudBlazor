// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a table which displays methods for a documented type.
/// </summary>
public partial class ApiPropertyTable
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
    /// Any search keyword to find.
    /// </summary>
    public string Keyword { get; set; } = "";

    /// <summary>
    /// The currently selected categories.
    /// </summary>
    public IReadOnlyCollection<string>? SelectedCategories { get; set; }

    /// <summary>
    /// The currently selected grouping.
    /// </summary>
    public Grouping CurrentGrouping { get; set; } = Grouping.Categories!;

    protected override async Task OnParametersSetAsync()
    {
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
        var properties = Type.Properties.Values
            .Where(property => SelectedCategories != null && SelectedCategories.Contains(property.Category));

        // Filter by any search keyword
        if (!string.IsNullOrEmpty(Keyword))
        {
            properties = properties.Where(property =>
                property.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase)
                || (property.Summary != null && property.Summary.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                || (property.Remarks != null && property.Remarks.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
            );
        }

        // Sort results
        switch (state.SortLabel)
        {
            case "Name":
                properties = state.SortDirection == SortDirection.Ascending ? properties.OrderBy(property => property.Name) : properties.OrderByDescending(property => property.Name);
                break;
            case "Type":
                properties = state.SortDirection == SortDirection.Ascending ? properties.OrderBy(property => property.Type) : properties.OrderByDescending(property => property.Type);
                break;
            case "Description":
                properties = state.SortDirection == SortDirection.Ascending ? properties.OrderBy(property => property.Summary) : properties.OrderByDescending(property => property.Summary);
                break;
            default:
                properties = state.SortDirection == SortDirection.Ascending ? properties.OrderBy(property => property.Name) : properties.OrderByDescending(property => property.Name);
                break;
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
                Grouping.Categories => new() { Selector = (property) => property.Category ?? "" },
                Grouping.Inheritance => new() { Selector = (property) => property.DeclaringType ?? "" },
                _ => new() { Selector = (property) => property.Category ?? "" }
            };
        }
    }

    /// <summary>
    /// Occurs when the search keyword has changed.
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task OnSearchAsync(string keyword)
    {
        Keyword = keyword;
        if (Table != null)
        {
            await Table.ReloadServerData();
        }
    }
}

public enum Grouping
{
    Categories,
    Inheritance,
    None
}

