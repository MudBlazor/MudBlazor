// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// A portion of a <see cref="MudGrid"/>.
/// </summary>
/// <seealso cref="MudGrid"/>
public partial class MudItem : MudComponentBase
{
    protected string Classname
    {
        get
        {
            // Only the breakpoints that are set build a string.
            // Passing the interpolated class as an argument builds it whether or not the condition holds, and most items set one breakpoint out of six.
            var builder = new CssBuilder("mud-grid-item");
            if (xs != 0)
            {
                builder.AddClass($"mud-grid-item-xs-{xs}");
            }
            if (sm != 0)
            {
                builder.AddClass($"mud-grid-item-sm-{sm}");
            }
            if (md != 0)
            {
                builder.AddClass($"mud-grid-item-md-{md}");
            }
            if (lg != 0)
            {
                builder.AddClass($"mud-grid-item-lg-{lg}");
            }
            if (xl != 0)
            {
                builder.AddClass($"mud-grid-item-xl-{xl}");
            }
            if (xxl != 0)
            {
                builder.AddClass($"mud-grid-item-xxl-{xxl}");
            }

            return builder.AddClass(Class).Build();
        }
    }

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
