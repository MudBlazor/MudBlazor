using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTooltip : MudComponentBase
    {
        private ElementReference _tooltipRef;
        protected string ContainerClass => new CssBuilder("mud-tooltip-root")
            .AddClass("mud-tooltip-inline", Inline)
            .Build();
        protected string Classname
        {
            get
            {
                var result = new CssBuilder("mud-tooltip")
                    //.AddClass($"mud-tooltip-placement-{Placement.ToDescriptionString()}")
                    .AddClass("mud-tooltip-visible", IsVisible)
                    .AddClass(Class)
                    .Build();

                return result;
            }
        }


        protected string Stylename
        {
            get
            {
                var result = new StyleBuilder()
                    .AddStyle($"transition-delay: {Delay.ToString(CultureInfo.InvariantCulture)}ms")
                    .AddStyle(Style)
                    .Build();
                return result;
            }
        }

        [Inject] public IBrowserWindowSizeProvider WindowSize { get; set; }

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

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Tooltip content. May contain any valid html
        /// </summary>
        [Parameter] public RenderFragment TooltipContent { get; set; }

        /// <summary>
        /// Determines if this component should 
        /// with it's surrounding (default) or if it should behave like a block element.
        /// </summary>
        [Parameter] public Boolean Inline { get; set; } = true;

        public bool IsVisible { get; set; }

        public void HandleMouseOver()
        {
            IsVisible = true;
            StateHasChanged();
        }
        private void HandleMouseOut() => IsVisible = false;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }
    }

}


