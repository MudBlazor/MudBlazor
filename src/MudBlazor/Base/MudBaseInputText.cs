﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudBaseInputText : ComponentBase
    {
        private string _value;

        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool Error { get; set; }
        [Parameter] public bool FullWidth { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public string Placeholder { get; set; }
        [Parameter] public string HelperText { get; set; }

        [Parameter] public InputType InputType { get; set; } = InputType.Text;
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string Value
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
