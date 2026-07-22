// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// The base type to filter types by.
    /// </summary>
    [Parameter]
    public DocumentedType? BaseType { get; set; }

    /// <summary>
    /// The keyword to search for.
    /// </summary>
    public string Keyword { get; set; } = "";

    /// <summary>
    /// Occurs when <see cref="Keyword"/> has changed.
    /// </summary>
    /// <param name="keyword">The text to search for.</param>
    public Task OnKeywordChanged(string keyword)
    {
        Keyword = keyword;
        return Table!.ReloadServerData();
    }

    /// <summary>
    /// Requests data for the table.
    /// </summary>
    /// <param name="state">The current table state.</param>
    /// <param name="token">A <see cref="CancellationToken"/> for aborting ongoing requests.</param>
    /// <returns></returns>
    public Task<TableData<DocumentedType>> GetData(TableState state, CancellationToken token)
    {
        // Get properties which are in the selected categories
        var types = ApiDocumentation.Types.Values.ToList().AsQueryable();

        // Are we filtering by base types?
        if (BaseType != null)
        {
            types = types.Where(type => type.BaseTypeName == BaseType.Name);
        }

        // Is there a search keyword?
        if (!string.IsNullOrEmpty(Keyword))
        {
            types = types.Where(type => type.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase)
                || (type.Summary != null && type.Summary.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                || (type.Remarks != null && type.Remarks.Contains(Keyword, StringComparison.OrdinalIgnoreCase)));
        }

        // ... then by sort column
        types = state.SortLabel switch
        {
            "Name" => state.SortDirection == SortDirection.Ascending ? types.OrderBy(type => type.Name) : types.OrderByDescending(type => type.Name),
            "Description" => state.SortDirection == SortDirection.Ascending ? types.OrderBy(type => type.Summary) : types.OrderByDescending(type => type.Summary),
            _ => state.SortDirection == SortDirection.Ascending ? types.OrderBy(type => type.Name) : types.OrderByDescending(type => type.Name),
        };

        // Get the total count
        var count = types.Count();

        // Make the final results
        var results = types
            .Skip(state.Page * state.PageSize)
            .Take(state.PageSize)
            .ToList();

        // What categories are selected?
        return Task.FromResult(new TableData<DocumentedType>()
        {
            Items = results,
            TotalItems = count,
        });
    }
}
