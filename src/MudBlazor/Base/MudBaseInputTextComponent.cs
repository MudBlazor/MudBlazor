using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudBaseInputTextComponent : ComponentBase
    {
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Error { get; set; }
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public string Value { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public string Placeholder { get; set; }
        [Parameter] public string HelperText { get; set; }

        [Parameter] public Type Type { get; set; } = Type.Text;
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

    }
}
