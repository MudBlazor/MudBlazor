// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor;

/// <summary>
/// Represents a checkbox column used to select rows in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item to select.</typeparam>
/// <seealso cref="MudDataGrid{T}"/>
public partial class SelectColumn<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : TemplateColumn<T>
{
    /// <summary>
    /// Shows a checkbox in the header.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, all rows can be checked by selecting this checkbox.
    /// </remarks>
    [Parameter]
    public bool ShowInHeader { get; set; } = true;

    /// <summary>
    /// Shows a checkbox in the footer.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, all rows can be checked by selecting this checkbox.
    /// </remarks>
    [Parameter]
    public bool ShowInFooter { get; set; } = false;

    /// <summary>
    /// The size of the checkbox icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// Determines if the checkbox for a specific row should be disabled.
    /// </summary>
    /// <remarks>
    /// When set, this function is called for each row to determine if the checkbox should be disabled.
    /// </remarks>
    [Parameter]
    public Func<T, bool>? DisabledFunc { get; set; }

    public override RenderFragment<HeaderContext<T>>? GetHeaderTemplate() => ShowInHeader ? GetSelectHeaderTemplate() : null;
    public override RenderFragment<CellContext<T>> GetCellTemplate() => GetSelectCellTemplate();
    public override RenderFragment<FooterContext<T>>? GetFooterTemplate() => ShowInFooter ? GetSelectFooterTemplate() : null;

    public SelectColumn()
    {
        Tag = "select-column";
        Editable = false;
        Sortable = false;
        Resizable = false;
        Filterable = false;
        ShowColumnOptions = false;
        HeaderStyle = "width:0%";
    }
}
