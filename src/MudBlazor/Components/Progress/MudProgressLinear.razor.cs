﻿using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudProgressLinear : MudComponentBase
    {
        protected string DivClassname =>
            new CssBuilder("mud-progress-linear")
                .AddClass($"mud-progress-linear-color-{Color.ToDescriptionString()}", !Buffer)
                .AddClass(Class)
                .Build();

        protected string LinearClassname =>
            new CssBuilder("mud-progress-linear-bar")
                .AddClass($"mud-{Color.ToDescriptionString()}")
                .AddClass($"mud-progress-indeterminate", Indeterminate)
                .AddClass($"mud-progress-linear-bar-1-determinate", !Indeterminate)
                .Build();

        protected string BufferClassname =>
            new CssBuilder("mud-progress-linear-dashed")
                .AddClass($"mud-progress-linear-dashed-color-{Color.ToDescriptionString()}")
                .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public bool Indeterminate { get; set; }
        [Parameter] public bool Buffer { get; set; }
        [Parameter] public bool Static { get; set; }
        [Parameter] public int StrokeWidth { get; set; } = 3;

        [Parameter] public double Minimum { get; set; } = 0.0;

        [Parameter] public double Maximum { get; set; } = 100.0;

        private double _value;
        private double _bufferValue;

        [Parameter]
        public double Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value))
                    return;
                _value = value;
                InvokeAsync(StateHasChanged);
            }
        }

        [Parameter]
        public double BufferValue
        {
            get => _bufferValue;
            set
            {
                if (_bufferValue.Equals(value))
                    return;
                _bufferValue = value;
                InvokeAsync(StateHasChanged);
            }
        }
    }
}
