// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.JSInterop;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.DatePicker;

public class MudDatePickerServiceTests
{
    private readonly Mock<IJSRuntime> _jsInterop;
    private readonly MudDatePickerService _testee;

    public MudDatePickerServiceTests()
    {
        this._jsInterop = new();
        this._testee = new(this._jsInterop.Object);
    }

    [Test]
    public async ValueTask HandleMouseoverOnPickerCalendarDayButton_MustCallJavascriptFunction()
    {
        const string ElementId = "anyId";
        const int TempId = 44;

        await this._testee.HandleMouseoverOnPickerCalendarDayButton(ElementId, TempId);

        this._jsInterop.Verify(x => x.InvokeAsync<object>("mudDatePicker.handleMouseoverOnPickerCalendarDayButton",
            It.Is<object[]>(args => args.Length == 2 && (string)args[0] == ElementId && (int)args[1] == TempId)));
    }
}
