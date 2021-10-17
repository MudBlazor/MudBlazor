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
            .AddClass($"mud-tooltip-default", Color == Color.Default)
            .AddClass($"mud-tooltip-{ConvertPlacement().ToDescriptionString()}")
            .AddClass($"mud-tooltip-arrow", Arrow)
            .AddClass($"mud-border-{Color.ToDescriptionString()}", Arrow && Color != Color.Default)
            .AddClass($"mud-theme-{Color.ToDescriptionString()}", Color != Color.Default)
            .AddClass(Class)
            .Build();


        private bool _isVisible;

        private Origin _anchorOrigin;
        private Origin _transformOrigin;

        [CascadingParameter]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Sets the text to be displayed inside the tooltip.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// If true, a arrow will be displayed pointing towards the content from the tooltip.
        /// </summary>
        [Parameter] public bool Arrow { get; set; }

        /// <summary>
        /// Changes the default transition delay in milliseconds.
        /// </summary>
        [Obsolete("Delay is obsolete. Use AnimationDelay instead.", false)]
        [Parameter] public double Delay { get; set; } = 200;

        /// <summary>
        /// Sets the length of time that the opening animation takes to complete.
        /// </summary>
        [Parameter] public double AnimationDuration { get; set; } = 251;

        /// <summary>
        /// Sets the amount of time to wait from opening the popover before beginning to perform the animation. 
        /// </summary>
        [Parameter] public double AnimationDelay { get; set; } = 200;

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
        [Parameter] public bool Inline { get; set; } = true;

        private void HandleMouseOver() => _isVisible = true;
        private void HandleMouseOut() => _isVisible = false;

        private Origin ConvertPlacement()
        {
            if (Placement == Placement.Bottom)
            {
                _anchorOrigin = Origin.BottomCenter;
                _transformOrigin = Origin.TopCenter;
                return Origin.BottomCenter;
            }
            if (Placement == Placement.Top)
            {
                _anchorOrigin = Origin.TopCenter;
                _transformOrigin = Origin.BottomCenter;
                return Origin.TopCenter;
            }
            if (Placement == Placement.Left || Placement == Placement.Start && !RightToLeft || Placement == Placement.End && RightToLeft)
            {
                _anchorOrigin = Origin.CenterLeft;
                _transformOrigin = Origin.CenterRight;
                return Origin.CenterLeft;
            }
            if (Placement == Placement.Right || Placement == Placement.End && !RightToLeft || Placement == Placement.Start && RightToLeft)
            {
                _anchorOrigin = Origin.CenterRight;
                _transformOrigin = Origin.CenterLeft;
                return Origin.CenterRight;
            }
            else
            {
                return Origin.BottomCenter;
            }
        }
    }
}
