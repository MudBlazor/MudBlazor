using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public partial class MudSlider : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-slider")
          .AddClass(Class)
        .Build();

        private int _value;
        [Parameter] public int Min { get; set; } = 0;
        [Parameter] public int Max { get; set; } = 100;
        [Parameter] public int Step { get; set; } = 1;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback<int> ValueChanged { get; set; }

        [Parameter]
        public int Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    ValueChanged.InvokeAsync(value);
                }
            }
        }

    }
}
