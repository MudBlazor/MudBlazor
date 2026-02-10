// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Resources;

namespace MudBlazor;

/// <summary>
/// Represents a checkbox column used to select rows in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item to select.</typeparam>
/// <seealso cref="MudDataGrid{T}"/>
public partial class SelectColumn<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : TemplateColumn<T>
{
    [Inject]
    private InternalMudLocalizer Localizer { get; set; } = null!;

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

    /// <summary>
    /// Provides a custom aria-label for each row selection checkbox.
    /// </summary>
    /// <remarks>
    /// When not set, a default localized label is used.
    /// </remarks>
    [Parameter]
    public Func<T, int, string?>? RowCheckboxAriaLabelFunc { get; set; }

    /// <summary>
    /// Provides a custom aria-labelledby value for each row selection checkbox.
    /// </summary>
    /// <remarks>
    /// When set, this value is applied in addition to any aria-label.
    /// </remarks>
    [Parameter]
    public Func<T, int, string?>? RowCheckboxAriaLabelledByFunc { get; set; }

    /// <summary>
    /// The aria-label applied to the header and footer select-all checkboxes.
    /// </summary>
    /// <remarks>
    /// Defaults to a localized label when not provided.
    /// </remarks>
    [Parameter]
    public string? SelectAllAriaLabel { get; set; }

    /// <summary>
    /// The aria-labelledby value applied to the header and footer select-all checkboxes.
    /// </summary>
    [Parameter]
    public string? SelectAllAriaLabelledBy { get; set; }

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

    private Dictionary<string, object?> GetSelectAllAttributes()
    {
        var label = string.IsNullOrWhiteSpace(SelectAllAriaLabel)
            ? Localizer[LanguageResource.MudDataGrid_SelectAllRows].Value
            : SelectAllAriaLabel!;

        return BuildAriaAttributes(label, SelectAllAriaLabelledBy);
    }

    private Dictionary<string, object?> GetRowCheckboxAttributes(CellContext<T> context)
    {
        var label = RowCheckboxAriaLabelFunc?.Invoke(context.Item, context.RowIndex);
        if (string.IsNullOrWhiteSpace(label))
        {
            label = GetDefaultRowAriaLabel(context.RowIndex);
        }

        var labelledBy = RowCheckboxAriaLabelledByFunc?.Invoke(context.Item, context.RowIndex);

        return BuildAriaAttributes(label, labelledBy);
    }

    private string GetDefaultRowAriaLabel(int rowIndex)
    {
        return rowIndex >= 0
            ? Localizer[LanguageResource.MudDataGrid_SelectRowWithIndex, rowIndex + 1].Value
            : Localizer[LanguageResource.MudDataGrid_SelectRow].Value;
    }

    private static Dictionary<string, object?> BuildAriaAttributes(string label, string? labelledBy)
    {
        var attributes = new Dictionary<string, object?>(2)
        {
            ["aria-label"] = label
        };

        if (!string.IsNullOrWhiteSpace(labelledBy))
        {
            attributes["aria-labelledby"] = labelledBy;
        }

        return attributes;
    }
}
