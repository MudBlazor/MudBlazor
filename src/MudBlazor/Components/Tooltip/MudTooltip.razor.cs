using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTooltip : MudComponentBase
    {
        private ElementReference _tooltipRef;
        protected string ContainerClass => new CssBuilder("mud-tooltip-root")
            .AddClass("mud-tooltip-inline", Inline)
            .Build();
        protected string Classname => new CssBuilder("mud-tooltip")
            .AddClass($"mud-tooltip-placement-{Placement.ToDescriptionString()}")
            .AddClass(Class)
            .Build();

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

        [Parameter] public bool IsVisible { get; set; }

        private void HandleMouseOver()
        {
            IsVisible = true;
            Console.WriteLine("in");
        }

        private void HandleMouseOut()
        {
            IsVisible = false;
            Console.WriteLine("out");
        }

        protected string GetTimeDelay()
        {
            return $"transition-delay: {Delay.ToString(CultureInfo.InvariantCulture)}ms;";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SetDirection();
        }

        private async Task SetDirection()
        {

            if (!IsVisible) { return; }
            if (_tooltipRef.Context == null) { return; }
            var rect = await _tooltipRef.MudGetBoundingClientRectAsync();
            if (rect == null) { return; }


            Console.WriteLine("render");

            var viewport = await WindowSize.GetBrowserWindowSize();
            string style = "";
            var placement = Placement;
            switch (Placement)
            {
                case Placement.Top:
                case Placement.Bottom:
                    if (rect.Top < 0 && Placement == Placement.Top)
                    { placement = Placement.Bottom; }
                    if (rect.Bottom > viewport.Height && Placement == Placement.Bottom)
                    { placement = Placement.Top; }

                    if (rect.Right > viewport.Width)
                    {
                        style = $"right:{(rect.Right - viewport.Width - 14).ToPixels()};left:unset;";
                    }
                    if (rect.Left < 0)
                    {
                        style = $"left:14px;right:unset;";
                    }
                    break;



                case Placement.Start:
                case Placement.End:
                    if (rect.Left < 0 && Placement == Placement.Start)
                    { placement = Placement.End; }
                    if (rect.Right > viewport.Width && Placement == Placement.End)
                    { placement = Placement.Start; }


                    if (rect.Top < 0) { style = $"top:14px;bottom:unset;"; }
                    if (rect.Bottom > viewport.Height)
                    { style = $"bottom:{(rect.Bottom - viewport.Height - 14).ToPixels()};top:unset;"; }
                    break;

            }
            Style = style;
            StateHasChanged();
        }

    }

}
