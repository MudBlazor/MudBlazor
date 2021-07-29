using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTooltip : MudComponentBase
    {
        protected string ContainerClass => new CssBuilder("mud-tooltip-root")
            .AddClass("mud-tooltip-inline", Inline)
            .Build();
        protected string Classname => new CssBuilder("mud-tooltip")
            .AddClass($"mud-tooltip-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
             .AddClass("mud-tooltip-visible", _isVisible)
            .AddClass(Class)
            .Build();


        [CascadingParameter]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the text to be displayed inside the tooltip.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Changes the default transition delay in milliseconds.
        /// </summary>
        [Parameter] public double Delay { get; set; } = 200;

        /// <summary>
        /// Changes the default transition delay in seconds.
        /// </summary>
        [Obsolete]
        [Parameter]
        public double Delayed
        {
            get { return Delay / 1000; }
            set { Delay = value * 1000; }
        }

        /// <summary>
        /// Tooltip placement.
        /// </summary>
        [Parameter] public Placement Placement { get; set; } = Placement.Bottom;

        private Placement ConvertPlacement(Placement placement)
        {
            return placement switch
            {
                Placement.Start => RightToLeft ? Placement.Right : Placement.Left,
                Placement.End => RightToLeft ? Placement.Left : Placement.Right,
                _ => placement
            };
        }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Tooltip content. May contain any valid html
        /// </summary>
        [Parameter] public RenderFragment TooltipContent { get; set; }

        /// <summary>
        /// Determines if this component should be inline with it's surrounding (default) or if it should behave like a block element.
        /// </summary>
        [Parameter] public Boolean Inline { get; set; } = true;

        private bool _isVisible;
        public void HandleMouseOver() => _isVisible = true;
        private void HandleMouseOut() => _isVisible = false;



        protected string GetTimeDelay()
        {
            return $"transition-delay: {Delay.ToString(CultureInfo.InvariantCulture)}ms;{Style}";
        }
    }
}
