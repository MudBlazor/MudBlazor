using System;
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
        private string AnchorClass =>
            new CssBuilder("portal-anchor")
            .Build();
        
        /// <summary>
        /// Set the coordinates (x,y,width,height) of the point where the Portal is going to be anchored
        /// </summary>
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
            .AddStyle("z-index", new ZIndex().Popover.ToString(), Item.CssPosition == "fixed")
            .Build();

        /// <summary>
        ///  The PortalItem that is going to be rendered
        /// </summary>
        [Parameter] public PortalItem Item { get; set; }


        /// <summary>
        /// True if this should render
        /// </summary>
        [Parameter] public bool NeedsToBeRendered { get; set; }

        protected override bool ShouldRender() => NeedsToBeRendered;

    }
}
