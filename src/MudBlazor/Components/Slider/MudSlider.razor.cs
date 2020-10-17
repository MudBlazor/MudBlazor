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

        /// <summary>
        /// The minimum allowed value of the slider. Should not be equal to max.
        /// </summary>
        [Parameter] public int Min { get; set; } = 0;

        /// <summary>
        /// The maximum allowed value of the slider. Should not be equal to min.
        /// </summary>
        /// 
        [Parameter] public int Max { get; set; } = 100;
        /// <summary>
        /// How many steps the slider should take on eatch move.
        /// </summary>
        /// 
        [Parameter] public int Step { get; set; } = 1;
        /// <summary>
        /// If true, the slider will be disabled.
        /// </summary>
        /// 
        [Parameter] public bool Disabled { get; set; }
        /// <summary>
        /// Child content of component.
        /// </summary>
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
