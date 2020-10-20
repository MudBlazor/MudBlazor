using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudBaseInputText : MudComponentBase
    {
        private string _value;

        /// <summary>
        /// If true, the input element will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read only.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// If true, the label will be displayed in an error state.
        /// </summary>
        [Parameter] public bool Error { get; set; }

        /// <summary>
        /// If true, the input will take up the full width of its container.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// If true, the input will not have an underline.
        /// </summary>
        [Parameter] public bool DisableUnderLine { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// If string has value, helpertext will be applied.
        /// </summary>
        [Parameter] public string HelperText { get; set; }

        /// <summary>
        /// Icon that will be used if Adornment is set to Start or End.
        /// </summary>
        [Parameter] public string AdornmentIcon { get; set; }

        /// <summary>
        /// Text that will be used if Adornment is set to Start or End, the Text overrides Icon.
        /// </summary>
        [Parameter] public string AdornmentText { get; set; }
        /// <summary>
        /// Sets Start or End Adornment if not set to None.
        /// </summary>
        [Parameter] public Adornment Adornment { get; set; } = Adornment.None;

        /// <summary>
        /// Button click event if set and Adornment used.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }
        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// Variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        ///  Will adjust vertical spacing. 
        /// </summary>
        [Parameter] public Margin Margin { get; set; } = Margin.None;

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
