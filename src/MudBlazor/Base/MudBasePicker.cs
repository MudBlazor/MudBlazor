using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public abstract class MudBasePicker : MudComponentBase
    {
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
