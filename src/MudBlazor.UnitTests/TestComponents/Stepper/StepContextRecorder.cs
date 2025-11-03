#nullable enable
using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MudBlazor.UnitTests.TestComponents.Stepper;

public class StepContextRecorder : ComponentBase
{
    [Parameter]
    public string? ActiveMarkup { get; set; }

    [Parameter]
    public Action<MudStepContext?>? Capture { get; set; }

    [CascadingParameter]
    public MudStepContext? Context { get; set; }

    protected override void OnParametersSet()
    {
        Capture?.Invoke(Context);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Context?.IsActive == true && !string.IsNullOrEmpty(ActiveMarkup))
        {
            builder.AddMarkupContent(0, ActiveMarkup);
        }
    }
}
