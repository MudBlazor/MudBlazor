// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// A portion of a <see cref="MudMatrix"/>.
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
            .AddStyle("grid-column", $"span {ColumnSpan}")
            .AddStyle("grid-row", $"span {RowSpan}")
            .AddStyle(Style)
            .Build();

    [CascadingParameter]
    private MudMatrix? Parent { get; set; }

    /// <summary>
    /// Number of columns this item spans.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int ColumnSpan { get; set; } = 1;

    /// <summary>
    /// Number of rows this item spans.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int RowSpan { get; set; } = 1;

    // ToDo false,auto,true on all sizes.

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        // NOTE: we can't throw here, the component must be able to live alone for the docs API to infer default parameters
        //if (Parent == null)
        //    throw new ArgumentNullException(nameof(Parent), "Item must exist within a Grid");
        base.OnInitialized();
    }
}
