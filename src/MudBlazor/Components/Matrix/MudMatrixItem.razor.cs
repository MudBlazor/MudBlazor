// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// An item within the layout of a <see cref="MudMatrix"/>.
/// </summary>
/// <seealso cref="MudMatrix"/>
public partial class MudMatrixItem : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-matrix-item")
            .AddClass(Class)
            .Build();
    protected string Stylename =>
        new StyleBuilder()
            .AddStyle("grid-column", GetTrackPlacement(ColumnPosition, ColumnSpan, ColumnSpanBackward))
            .AddStyle("grid-row", GetTrackPlacement(RowPosition, RowSpan, RowSpanBackward))
            .AddStyle(Style)
            .Build();

    [CascadingParameter]
    private MudMatrix? Parent { get; set; }

    /// <summary>
    /// Number of columns this item spans.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public int ColumnSpan { get; set; } = 1;

    /// <summary>
    /// Number of rows this item spans.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public int RowSpan { get; set; } = 1;

    /// <summary>
    /// When <see cref="ColumnPosition"/> is set, controls whether this item spans forward or backward from that position.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Has no effect when <see cref="ColumnPosition"/> is not set.
    /// </para>
    /// <para>
    /// <c>Default is false. </c>
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public bool ColumnSpanBackward { get; set; }

    /// <summary>
    /// When <see cref="RowPosition"/> is set, controls whether this item spans forward or backward from that position.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Has no effect when <see cref="RowPosition"/> is not set.
    /// </para>
    /// <para>
    /// <c>Default is false </c>
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public bool RowSpanBackward { get; set; }

    /// <summary>
    /// The column this item is placed in. 
    /// Positive values are forwards from the beginning of the row.
    /// Negative values are backwards from the end of the row.
    /// </summary>
    ///  <para>
    /// <c>Default is null</c>, allows <see cref="MudMatrix"/> to control placement
    /// </para>
    /// <para>
    /// <c>0 is invalid</c>
    /// </para>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public int? ColumnPosition { get; set; }

    /// <summary>
    /// The row this item is placed in. 
    /// Positive values are forwards from the beginning of the column.
    /// Negative values are backwards from the end of the column.
    /// </summary>
    ///  <para>
    /// <c>Default is null</c>, allows <see cref="MudMatrix"/> to control placement
    /// </para>
    /// <para>
    /// <c>0 is invalid</c>
    /// </para>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public int? RowPosition { get; set; }

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.MatrixItem.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    private static string GetTrackPlacement(int? position, int span, bool spanBackward)
    {
        if (position is null)
        {
            return $"span {span}";
        }

        return spanBackward ? $"span {span} / {position}" : $"{position} / span {span}";
    }
}
