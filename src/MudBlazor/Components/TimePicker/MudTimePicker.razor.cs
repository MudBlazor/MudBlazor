using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public partial class MudTimePicker : MudBasePicker
    {

        /// <summary>
        /// Reference to the Picker, initialized via @ref
        /// </summary>
        private MudPicker Picker;

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Hours;

        /// <summary>
        /// If true, sets 12 hour selection clock.
        /// </summary>
        [Parameter] public bool AmPm { get; set; }

        /// <summary>
        /// Sets the Input Icon.
        /// </summary>
        [Parameter] public string InputIcon { get; set; } = Icons.Material.Event;


        private async void OnHourClick()
        {
            OpenTo = OpenTo.Hours;
            StateHasChanged();
        }

        private async void OnMinutesClick()
        {
            OpenTo = OpenTo.Minutes;
            StateHasChanged();
        }

        private string GetClockPinColor()
        {
           return $"mud-picker-time-clock-pin mud-color-{Color.ToDescriptionString()}";
        }
        private string GetClockPointerColor()
        {
            return $"mud-picker-time-clock-pointer mud-picker-time-clock-pointer-animation mud-color-{Color.ToDescriptionString()}";
        }

        private string GetClockPointerThumbColor()
        {
            return $"mud-picker-time-clock-pointer-thumb mud-color-{Color.ToDescriptionString()}";
        }
    }
}
