using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace MudBlazor
{
    public abstract class MudBaseColumn : ComponentBase
    {
        public enum Rendermode
        {
            Header, Item, Edit, Footer
        }

        [CascadingParameter(Name = "Mode")]
        public Rendermode Mode { get; set; }

        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public string HeaderText { get; set; }

        protected bool IsDefault<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}
