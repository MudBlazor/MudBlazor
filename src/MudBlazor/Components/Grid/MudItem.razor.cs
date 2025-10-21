// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A portion of a <see cref="MudGrid"/>.
/// </summary>
/// <seealso cref="MudGrid"/>
public partial class MudItem : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-grid-item")
            .AddClass($"mud-grid-item-xs-{xs}", xs != 0)
            .AddClass($"mud-grid-item-sm-{sm}", sm != 0)
            .AddClass($"mud-grid-item-md-{md}", md != 0)
            .AddClass($"mud-grid-item-lg-{lg}", lg != 0)
            .AddClass($"mud-grid-item-xl-{xl}", xl != 0)
            .AddClass($"mud-grid-item-xxl-{xxl}", xxl != 0)
            .AddClass($"order-{Order}", Order != null)
            .AddClass($"order-sm-{OrderSm}", OrderSm != null)
            .AddClass($"order-md-{OrderMd}", OrderMd != null)
            .AddClass($"order-lg-{OrderLg}", OrderLg != null)
            .AddClass($"order-xl-{OrderXl}", OrderXl != null)
            .AddClass($"order-xxl-{OrderXxl}", OrderXxl != null)
            .AddClass(Class)
            .Build();

    [CascadingParameter]
    private MudGrid? Parent { get; set; }

    /// <summary>
    /// Sets the number of columns to occupy at the 'extra small' breakpoint.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int xs { get; set; }

    /// <summary>
    ///Sets the number of columns to occupy at the 'small' breakpoint.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int sm { get; set; }

    /// <summary>
    /// Sets the number of columns to occupy at the 'medium' breakpoint.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int md { get; set; }

    /// <summary>
    /// Sets the number of columns to occupy at the 'large' breakpoint.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int lg { get; set; }

    /// <summary>
    /// Sets the number of columns to occupy at the 'extra large' breakpoint.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int xl { get; set; }

    /// <summary>
    /// Sets the number of columns to occupy at the 'extra extra large' breakpoint.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int xxl { get; set; }

    /// <summary>
    /// Controls the visual order of the grid item for all breakpoints by default.
    /// Lower values appear before higher ones.  
    /// If multiple order parameters are set (e.g., <see cref="OrderMd"/>),  
    /// the largest active breakpoint takes precedence.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int? Order { get; set; }

    /// <summary>
    /// Controls the visual order of the grid item starting from the 'small' breakpoint (≥600px).
    /// Overrides <see cref="Order"/> when the viewport width is within this range or larger.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int? OrderSm { get; set; }

    /// <summary>
    /// Controls the visual order of the grid item starting from the 'medium' breakpoint (≥960px).
    /// Overrides smaller breakpoint orders (<see cref="Order"/> and <see cref="OrderSm"/>)
    /// when the viewport width is within this range or larger.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int? OrderMd { get; set; }

    /// <summary>
    /// Controls the visual order of the grid item starting from the 'large' breakpoint (≥1280px).
    /// Overrides <see cref="Order"/>, <see cref="OrderSm"/>, and <see cref="OrderMd"/>  
    /// when this breakpoint is active.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int? OrderLg { get; set; }

    /// <summary>
    /// Controls the visual order of the grid item starting from the 'extra large' breakpoint (≥1920px).
    /// Overrides all smaller breakpoint order values when active.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int? OrderXl { get; set; }

    /// <summary>
    /// Controls the visual order of the grid item starting from the 'extra extra large' breakpoint (≥2560px).
    /// Overrides all smaller breakpoint order values when active.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Item.Behavior)]
    public int? OrderXxl { get; set; }


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
