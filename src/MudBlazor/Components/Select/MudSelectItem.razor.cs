﻿using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseMudSelectItem : MudBaseSelectItem
    {
        [CascadingParameter] public MudSelect MudSelect { get; set; }
        [Parameter] public string Value { get; set; }
    }
}
