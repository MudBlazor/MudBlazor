using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudChip : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-chip")
          .AddClass($"mud-chip-color-{Color.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
