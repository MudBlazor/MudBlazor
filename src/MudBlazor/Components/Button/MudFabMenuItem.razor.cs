#nullable enable
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Represents an item for the <see cref="MudFabMenu"/>.
/// </summary>
public partial class MudFabMenuItem : MudFab
{
    private new string Classname => new CssBuilder(base.Classname)
        .AddClass("mud-fab-menu-item")
        .AddClass(Class)
        .Build();

    /// <summary>
    /// The size of the menu item.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Appearance)]
    public override Size Size { get; set; } = Size.Medium;
}
