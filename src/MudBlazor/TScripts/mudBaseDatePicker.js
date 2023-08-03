// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//Functions related to MudDatePicker and MudDateRangePicker

class MudDatePicker {
    handleMouseoverOnPickerCalendarDayButton (elementId, tempId) {
        const element = document.getElementById(elementId);
        element.style.setProperty('--selected-day', tempId);
    }
};

window.mudDatePicker = new MudDatePicker();