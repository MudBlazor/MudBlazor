using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudChip : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-chip")
          .AddClass($"mud-chip-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-chip-color-{Color.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The color of the component.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the button. small is equivalent to the dense button styling.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
