// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Pages.Api;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services.XmlDocs;
using LoxSmoke.DocXml;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents a table which displays methods for a documented type.
/// </summary>
public partial class ApiMemberTable
{
    /// <summary>
    /// The service for XML documentation.
    /// </summary>
    [Inject]
    public IXmlDocsService? Docs { get; set; }

    /// <summary>
    /// This table.
    /// </summary>
    public MudTable<MemberInfo>? Table { get; set; }

    /// <summary>
    /// The kind of member to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public ApiMemberType Mode { get; set; } = ApiMemberType.None;

    /// <summary>
    /// The currently selected grouping.
    /// </summary>
    [Parameter]
    public ApiMemberGrouping Grouping { get; set; } = ApiMemberGrouping.Categories;

    /// <summary>
    /// The type containing the members.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string? TypeName { get; set; }

    /// <summary>
    /// The type to display members for.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// The members to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public List<MemberInfo> Members { get; set; } = [];

    protected override async Task OnParametersSetAsync()
    {
        // Do we have to look up a new type?
        if (!string.IsNullOrEmpty(TypeName) && (Type == null || Type.Name != TypeName))
        {
            Type = Docs!.GetType(TypeName);
            if (Table != null)
            {
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
    public async Task<TableData<MemberInfo>> GetData(TableState state, CancellationToken token)
    {
        if (Members == null || Members.Count == 0 || Mode == ApiMemberType.None)
        {
            return new TableData<MemberInfo> { Items = [], TotalItems = 0 };
        }

        // Get a queryable list of members to start with
        var members = Members.AsQueryable();

        // First, sort by grouping
        switch (Grouping)
        {
            case ApiMemberGrouping.None:
                // Nothing to do                
                members = SortByColumn(members.OrderBy(member => 1), state);
                break;
            case ApiMemberGrouping.Categories:
                // Group by category order
                members = SortByColumn(members.OrderBy(member => member.GetCategoryOrder()), state);
                break;
            case ApiMemberGrouping.Inheritance:
                // Group by base class
                members = SortByColumn(members.OrderBy(member => member.DeclaringType!.GetFriendlyName()), state);
                break;
        }

        // Get the total count
        var totalItems = members.Count();

        // Make the final results
        var items = members.Skip(state.Page * state.PageSize).Take(state.PageSize);

        // Return the final results
        return await Task.FromResult(new TableData<MemberInfo>()
        {
            Items = items,
            TotalItems = totalItems
        });
    }

    /// <summary>
    /// Sorts a list of members by the sort column and direction.
    /// </summary>
    /// <param name="members"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    private IQueryable<MemberInfo> SortByColumn(IOrderedQueryable<MemberInfo> members, TableState state)
    {
        // Next, sort by column
        return state.SortLabel switch
        {
            "Name" => state.SortDirection == SortDirection.Ascending
                                ? members.ThenBy(member => member.Name)
                                : members.ThenByDescending(member => member.Name),
            "Type" => Mode switch
            {
                ApiMemberType.Properties => state.SortDirection == SortDirection.Ascending
                                            ? members.ThenBy(member => ((PropertyInfo)member).PropertyType.GetFriendlyName())
                                            : members.ThenByDescending(member => ((PropertyInfo)member).PropertyType.GetFriendlyName()),
                ApiMemberType.Methods => state.SortDirection == SortDirection.Ascending
                                            ? members.ThenBy(member => ((MethodInfo)member).ReturnType.GetFriendlyName())
                                            : members.ThenByDescending(member => ((MethodInfo)member).ReturnType.GetFriendlyName()),
                ApiMemberType.Fields => state.SortDirection == SortDirection.Ascending
                                            ? members.ThenBy(member => ((FieldInfo)member).FieldType.GetFriendlyName())
                                            : members.ThenByDescending(member => ((FieldInfo)member).FieldType.GetFriendlyName()),
                _ => members,
            },
            "Description" => state.SortDirection == SortDirection.Ascending
                                    ? members.ThenBy(member => Docs!.GetMemberComments(member)!.Summary ?? "")
                                    : members.ThenByDescending(member => Docs!.GetMemberComments(member)!.Summary ?? ""),
            _ => members,
        };
    }

    /// <summary>
    /// The current groups.
    /// </summary>
    public TableGroupDefinition<MemberInfo>? CurrentGroups => Grouping switch
    {
        ApiMemberGrouping.None => null,
        ApiMemberGrouping.Categories => new() { Selector = (property) => property.GetCategoryName() },
        ApiMemberGrouping.Inheritance => new() { Selector = (property) => property.DeclaringType?.GetFriendlyName() ?? "" },
        _ => null
    };

    /// <summary>
    /// Gets the grouping button variant based on the current grouping.
    /// </summary>
    /// <param name="grouping">The grouping to compare.</param>
    /// <returns>The button variant to use.</returns>
    public Variant GetButtonVariant(ApiMemberGrouping grouping)
    {
        return Grouping == grouping ? Variant.Filled : Variant.Outlined;
    }

    /// <summary>
    /// Changes the grouping for this table.
    /// </summary>
    /// <param name="grouping">The new grouping to use.</param>
    /// <returns></returns>
    public void OnGroupBy(ApiMemberGrouping grouping)
    {
        Grouping = grouping;
        StateHasChanged();
    }

    /// <summary>
    /// The service for navigating to other pages.
    /// </summary>
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
