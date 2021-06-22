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
        private bool _hasBeenRendered;
        private ElementReference _fragmentRef;
        private ElementReference _anchorRef;
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public PortalItem Item { get; set; }

        public void Update() => StateHasChanged();

        protected override bool ShouldRender()
        {
            return base.ShouldRender();
        }
        private string AnchorClass =>
            new CssBuilder("portal-anchor")
            .AddClass("portal-anchor-hidden", !_hasBeenRendered)
            .Build();

        private string AnchorStyle =>
            new StyleBuilder()
            .AddStyle("position", Item.Position, _hasBeenRendered)
            .AddStyle("top", Item.AnchorRect?.AbsoluteTop.ToPixels())
            .AddStyle("left", Item.AnchorRect?.AbsoluteLeft.ToPixels())
            .AddStyle("height", Item.AnchorRect?.Height.ToPixels())
            .AddStyle("width", Item.AnchorRect?.Width.ToPixels())
            .AddStyle("z-index", new ZIndex().Popover.ToString(), Item.Position == "fixed")
            .Build();

        private string FragmentClass =>
            new CssBuilder("portal-fragment")
            .AddClass("portal-fragment-hidden", !_hasBeenRendered)
            .AddClass($"mud-popover-{Item.Direction.ToDescriptionString()}")
            .AddClass("mud-popover-offset-y", Item.OffsetY)
            .AddClass("mud-popover-offset-x", Item.OffsetX)
            .Build();

        private string FragmentStyle =>
            new StyleBuilder()
            
            .AddStyle("position", "absolute")
            .AddStyle("top", Item.FragmentRect?.AbsoluteTop.ToPixels())
            .AddStyle("left", Item.FragmentRect?.AbsoluteLeft.ToPixels())
            .AddStyle("height", Item.FragmentRect?.Height.ToPixels())
            .AddStyle("width", Item.AnchorRect?.Width.ToPixels())            
            .Build();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Item.FragmentRect = await _fragmentRef.MudGetBoundingClientRectAsync();
                Item.FragmentRect.SetRectInsideViewPort();
                _hasBeenRendered = true;
                StateHasChanged();
            }
        }
    }
}
