using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudBasePicker : MudComponentBase
    {
        protected string MudPickerClass =>
        new CssBuilder("mud-picker")
           .AddClass($"mud-picker-inline", PickerVariant == PickerVariant.Inline)
          .AddClass($"mud-picker-input-button", !AllowKeyboardInput)
          .AddClass($"mud-picker-input-text", AllowKeyboardInput )
          .AddClass($"mud-disabled", Disabled)
        .Build();

        private string _value;
        /// <summary>
        /// If string has value, helpertext will be applied.
        /// </summary>
        [Parameter] public string HelperText { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, the picker will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Hide toolbar and show only date/time views.
        /// </summary>
        [Parameter] public bool DisableToolbar { get; set; }

        /// <summary>
        /// Picker container option
        /// </summary>
        [Parameter] public PickerVariant PickerVariant { get; set; } = PickerVariant.Inline;

        /// <summary>
        /// InputVariant, if Picker is static this option will not change anything.
        /// </summary>
        [Parameter] public Variant InputVariant { get; set; } = Variant.Text;

        /// <summary>
        /// Sets if the icon will be att start or end, set to false to disable.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.End;

        /// <summary>
        /// What orientation to render in when in PickerVariant Static Mode.
        /// </summary>
        [Parameter] public Orientation Orientation { get; set; } = Orientation.Portrait;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Allows text input from keyboard.
        /// </summary>
        [Parameter] public bool AllowKeyboardInput { get; set; }

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

        public bool isOpen { get; set; }

        public void OnOpen()
        {
            isOpen = !isOpen;
            StateHasChanged();
        }

        public void OnClose()
        {
            isOpen = false;
            StateHasChanged();
        }
    }
}
