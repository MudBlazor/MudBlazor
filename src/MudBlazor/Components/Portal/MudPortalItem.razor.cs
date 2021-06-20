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
        private bool _hasBeenRendered=true;
        private ElementReference _fragmentRef;
        private ElementReference _anchorRef;
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public PortalItem Item { get; set; }

        public void Update() => StateHasChanged();

        protected override bool ShouldRender()
        {
            return base.ShouldRender();
        }
        private string AnchorStyle =>
            new StyleBuilder()

                   .AddStyle("position", "fixed", !_hasBeenRendered)
                   //.AddStyle("visibility", "hidden", !_hasBeenRendered)
                   .AddStyle("position", Item.Position, _hasBeenRendered)
                   .AddStyle("top", Item.AnchorRect?.AbsoluteTop.ToPixels())
                   .AddStyle("left", Item.AnchorRect?.AbsoluteLeft.ToPixels())
                   .AddStyle("height", Item.AnchorRect?.Height.ToPixels(), _hasBeenRendered)
                   .AddStyle("width", Item.AnchorRect?.Width.ToPixels(), _hasBeenRendered)
                   .AddStyle("z-index", new ZIndex().Popover.ToString(), Item.Position == "fixed")
                   .Build();

       

        private void SetDirection()
        {
           
            var viewPortHeight = Item.FragmentRect.WindowHeight;

            if (Item.FragmentRect?.Height + Item.AnchorRect.Bottom> viewPortHeight)
            {
                Item.AnchorRect.Top -= Item.AnchorRect.AbsoluteBottom + Item.FragmentRect.Height - viewPortHeight;
                Item.AnchorRect.Bottom = Item.AnchorRect.Top + Item.AnchorRect.Height;
                
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Item.FragmentRect = await _fragmentRef.MudGetBoundingClientRectAsync();
                //SetDirection();
                //StateHasChanged();
            }
            _hasBeenRendered = true;

        }
    }
}
