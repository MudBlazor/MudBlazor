// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor;

internal class MudDatePickerService : IMudDatePickerService
{
    private readonly DatePickerInterop _datePickerInterop;
    
    public MudDatePickerService(IJSRuntime jsInterop)
    {
        this._datePickerInterop = new(jsInterop);
    }
    
    public ValueTask HandleMouseoverOnPickerCalendarDayButton(string elementId, int tempId)
    {
        return this._datePickerInterop.HandleMouseoverOnPickerCalendarDayButton(elementId, tempId);
    }
}
