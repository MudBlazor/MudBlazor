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

        private string  AnchorStyle =>
            new StyleBuilder()
            .AddStyle("top", Item.AnchorRect.AbsoluteTop.ToPixels())
            .AddStyle("left", Item.AnchorRect.AbsoluteLeft.ToPixels())
            .AddStyle("height", Item.AnchorRect.Height.ToPixels())
            .AddStyle("width", Item.AnchorRect.Width.ToPixels())
            .AddStyle("z-index", new ZIndex().Popover.ToString(), Item.Position == "fixed")
            .Build();
    }
}
