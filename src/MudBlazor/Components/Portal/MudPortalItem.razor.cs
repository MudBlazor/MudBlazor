using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPortalItem
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public PortalItem Item { get; set; }
        private string AnchorClass =>
            new CssBuilder("portal-anchor")
            .AddClass("portal-anchor-hoverable", Item.OpenOnHover)
            .Build();

        private string AnchorStyle =>
            new StyleBuilder()
            .AddStyle("top", Item.CssPosition == "fixed"
                ? Item.AnchorRect?.Top.ToPixels()
                : Item.AnchorRect?.AbsoluteTop.ToPixels())
            .AddStyle("left", Item.CssPosition == "fixed"
                ? Item.AnchorRect?.Left.ToPixels()
                : Item.AnchorRect?.AbsoluteLeft.ToPixels())
             .AddStyle("height", Item.AnchorRect?.Height.ToPixels())
            .AddStyle("width", Item.AnchorRect?.Width.ToPixels())
            .AddStyle("position", Item.CssPosition)
            .AddStyle("z-index", new ZIndex().Popover.ToString(), Item.CssPosition == "fixed" || Item.OpenOnHover)
            .Build();
    }
}
