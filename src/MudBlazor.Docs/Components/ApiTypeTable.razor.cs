﻿// Copyright (c) MudBlazor 2021
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
/// Represents a table which displays documented types.
/// </summary>
public partial class ApiTypeTable
{
    /// <summary>
    /// This table.
    /// </summary>
    public MudTable<DocumentedType>? Table { get; set; }

    /// <summary>
    /// The currently selected categories.
    /// </summary>
    public IReadOnlyCollection<string>? SelectedCategories { get; set; }

    /// <summary>
    /// Any search keyword to find.
    /// </summary>
    public string Keyword { get; set; } = "";

    /// <inheritdoc />
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
    public async Task<TableData<DocumentedType>> GetData(TableState state, CancellationToken token)
    {
        // Get properties which are in the selected categories
        var types = ApiDocumentation.Types.Values.ToList().AsQueryable();

        // Are we only viewing components?
        if (SelectedCategories != null && !SelectedCategories.Contains("Types") && SelectedCategories.Contains("Components"))
        {
            types = types.Where(type => type.IsComponent);
        }
        // Only viewing types?
        else if (SelectedCategories != null && SelectedCategories.Contains("Types") && !SelectedCategories.Contains("Components"))
        {
            types = types.Where(type => !type.IsComponent);
        }
        else if (SelectedCategories == null || SelectedCategories.Count == 0)
        {
            return new() { Items = [] };
        }

        // Filter by any search keyword
        if (!string.IsNullOrEmpty(Keyword))
        {
            types = types.Where(type =>
                type.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase)
                || (type.Summary != null && type.Summary.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                || (type.Remarks != null && type.Remarks.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
            );
        }

        // ... then by sort column
        types = state.SortLabel switch
        {
            "Name" => state.SortDirection == SortDirection.Ascending ? types.OrderBy(type => type.Name) : types.OrderByDescending(type => type.Name),
            "Description" => state.SortDirection == SortDirection.Ascending ? types.OrderBy(type => type.Summary) : types.OrderByDescending(type => type.Summary),
            _ => state.SortDirection == SortDirection.Ascending ? types.OrderBy(type => type.Name) : types.OrderByDescending(type => type.Name),
        };

        // Make the final results
        var results = types.ToList();

        // What categories are selected?
        return await Task.FromResult(new TableData<DocumentedType>()
        {
            Items = results,
            TotalItems = results.Count,
        });
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
