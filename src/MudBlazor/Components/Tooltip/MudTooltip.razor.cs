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
        /// 
        /// </summary>
        [Parameter] public bool Arrow { get; set; }

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

        private Origin AnchorOrigin;
        private Origin TransformOrigin;

        private Origin ConvertPlacement()
        {
            if(Placement == Placement.Bottom)
            {
                AnchorOrigin = Origin.BottomCenter;
                TransformOrigin = Origin.TopCenter;
                return Origin.BottomCenter;
            }
            if(Placement == Placement.Top)
            {
                AnchorOrigin = Origin.TopCenter;
                TransformOrigin = Origin.BottomCenter;
                return Origin.TopCenter;
            }
            if(Placement == Placement.Left || Placement == Placement.Start && !RightToLeft || Placement == Placement.End && RightToLeft)
            {
                AnchorOrigin = Origin.CenterLeft;
                TransformOrigin = Origin.CenterRight;
                return Origin.CenterLeft;
            }
            if (Placement == Placement.Right || Placement == Placement.End && !RightToLeft || Placement == Placement.Start && RightToLeft)
            {
                AnchorOrigin = Origin.CenterRight;
                TransformOrigin = Origin.CenterLeft;
                return Origin.CenterRight;
            }
            else{
                return Origin.BottomCenter;
            }
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
        [Parameter] public bool Inline { get; set; } = true;

        private bool _isVisible;
        public void HandleMouseOver() => _isVisible = true;
        private void HandleMouseOut() => _isVisible = false;

        protected string GetTimeDelay()
        {
            return $"transition-delay: {Delay.ToString(CultureInfo.InvariantCulture)}ms;{Style}";
        }
    }
}
