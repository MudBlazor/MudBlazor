using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{

    public partial class MudTooltip : ComponentBase
    {
        protected string Classname => new CssBuilder("mud-tooltip")
            .AddClass($"mud-tooltip-placement-{Placement.ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Sets the text to be displayed inside the tooltip.
        /// </summary>
        [Parameter] public string Text { get; set; }


        /// <summary>
        /// User class names, separated by space
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Changes the default transition delay in seconds.
        /// </summary>
        [Parameter] public double Delayed { get; set; } = 0.2;

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

        protected string GetTimeDelay()
        {
            return $"transition-delay: {Delayed.ToString(CultureInfo.InvariantCulture)}s;";
        }
    }
}
