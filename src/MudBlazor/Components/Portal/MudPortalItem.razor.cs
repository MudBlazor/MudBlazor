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
            .AddClass($"mud-popover-{Item.Direction.ToDescriptionString()}")
            .AddClass("portal-fragment-hidden", !_hasBeenRendered)
            .Build();

        private void SetDirection()
        {
            //nom funciona porque fragment rect height =0 quando position fixed
            if (Item.FragmentRect.IsOutOfViewPort)
            {
                if (Item.Direction == Direction.Bottom) Item.AnchorRect.Top -= Item.FragmentRect.Height;
                if (Item.Direction == Direction.Top) Item.AnchorRect.Top += Item.FragmentRect.Height;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Item.FragmentRect = await _fragmentRef.MudGetBoundingClientRectAsync();
                //SetDirection();
                _hasBeenRendered = false;
                StateHasChanged();
            }
        }
    }
}
