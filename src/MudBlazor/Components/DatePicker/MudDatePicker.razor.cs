using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;

namespace MudBlazor
{
    public partial class MudDatePicker : MudBasePicker
    {
        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime MaxDate { get; set; }

        /// <summary>
        /// Max selectable date.
        /// </summary>
        [Parameter] public DateTime MinDate { get; set; }

        /// <summary>
        /// First view to show in the MudDatePicker.
        /// </summary>
        [Parameter] public OpenTo OpenTo { get; set; } = OpenTo.Date;

        /// <summary>
        /// Sets the Input Icon.
        /// </summary>
        [Parameter] public string InputIcon { get; set; } = Icons.Material.Event;

    }
}
