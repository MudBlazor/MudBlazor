﻿using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public abstract class MudBaseColumn : ComponentBase
    {
        public enum Rendermode
        {
            Header,Item,Edit,Footer
        }

        [CascadingParameter(Name = "Mode")]
        public Rendermode Mode { get; set; }

        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string HeaderText { get; set; }

    }
}
