using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseMudMenuItem : MudBaseButton
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [CascadingParameter] public MudMenu MudMenu { get; set; }
        [Parameter] public bool Disabled { get; set; }
    }
}
