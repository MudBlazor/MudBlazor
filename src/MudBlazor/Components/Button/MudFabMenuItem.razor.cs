using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A floating action button shown as one of the selectable options within a <see cref="MudFabMenu"/>.
/// </summary>
/// <seealso cref="MudFab" />
/// <seealso cref="MudFabMenu" />
public partial class MudFabMenuItem : MudFab
{
    [CascadingParameter]
    protected MudFabMenu? ParentMenu { get; set; }

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

    protected override async Task OnClickHandler(MouseEventArgs ev)
    {
        if (GetDisabledState())
        {
            return;
        }

        await base.OnClickHandler(ev);

        // The menu used to close because the click bubbled up to the menu container. It now closes from
        // here, so that stopping propagation does not also stop the menu from closing.
        if (ParentMenu is not null)
        {
            await ParentMenu.CloseOnItemClickedAsync();
        }
    }
}
