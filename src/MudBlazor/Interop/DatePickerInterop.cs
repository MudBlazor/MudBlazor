// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Interop;

internal class DatePickerInterop
{
    private readonly IJSRuntime _jsRuntime;

    public DatePickerInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask HandleMouseoverOnPickerCalendarDayButton(string elementId, int tempId)
    {
        return _jsRuntime.InvokeVoidAsync("mudDatePicker.handleMouseoverOnPickerCalendarDayButton", elementId, tempId);
    }
}
