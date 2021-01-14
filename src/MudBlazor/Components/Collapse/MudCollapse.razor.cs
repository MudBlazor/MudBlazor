using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCollapse : MudComponentBase
    {

        /// <summary>
        /// If true, expands the panel, otherwise collapse it. Setting this prop enables control over the panel.
        /// </summary>
        [Parameter] public bool Expanded { get; set; }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Modified transition duration that scales with height parameter.
        /// Basic implementation for now but should be a math formula to allow it to scale between 0.1s and 1s for the effect to be consistently smooth.
        /// </summary>
        private decimal CalculatedTransitionDuration
        {
            get
            {
                if (MaxHeight.HasValue && Expanded)
                {
                    if (MaxHeight <= 200) { return 0.2m; }
                    else if (MaxHeight <= 600) { return 0.4m; }
                    else if (MaxHeight <= 1400) { return 0.6m; };
                }
                return 1;
            }
            set { }
        }

        protected string Classname =>
            new CssBuilder("mud-collapse-container")
            .AddClass($"mud-collapse-expanded", Expanded)
            .AddClass(Class)
            .Build();
    }
}
