using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A floating action button shown as one of the selectable options within a <see cref="MudFabMenu"/>.
/// </summary>
/// <seealso cref="MudFab" />
/// <seealso cref="MudFabMenu" />
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
