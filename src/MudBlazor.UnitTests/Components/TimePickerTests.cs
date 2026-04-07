using System.Globalization;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.TimePicker;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TimePickerTests : BunitTest
    {
        public async Task<IRenderedComponent<SimpleTimePickerTest>> OpenPicker(Action<ComponentParameterCollectionBuilder<SimpleTimePickerTest>> parameterBuilder = null)
        {
            IRenderedComponent<SimpleTimePickerTest> comp;
            if (parameterBuilder is null)
            {
                comp = Context.Render<SimpleTimePickerTest>();
            }
            else
            {
                comp = Context.Render<SimpleTimePickerTest>(parameterBuilder);
            }

            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            await comp.Find("input").ClickAsync();
            // now its open
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            return comp;
        }

        [Test]
        public void TimePickerOpenButtonDefaultAriaLabel()
        {
            var comp = Context.Render<MudTimePicker>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open");
        }

        [Test]
        public async Task TimePicker_Should_Clear()
        {
            var comp = Context.Render<MudTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.ReadOnly.Should().Be(false);
            picker.Text.Should().Be(null);
            picker.ReadValue.Should().Be(null);
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Clearable, true)
                .Add(p => p.Time, new TimeSpan(637940935730000000)));
            picker.ReadValue.Should().Be(new TimeSpan(637940935730000000));
            picker.Text.Should().Be(new TimeSpan(637940935730000000).ToIsoString());

            await comp.Find(".mud-input-clear-button").ClickAsync(); //clear the input

            picker.Text.Should().Be(""); //ensure the text and time are reset. Note this is an empty string rather than null due to how the reset works internally
            picker.ReadValue.Should().Be(null);
        }

        [Test]
        public async Task Open_ClickOutside_CheckClosed()
        {
            var comp = await OpenPicker();
            // clicking outside to close
            await comp.Find("div.mud-overlay").ClickAsync();
            // should not be open any more
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public async Task Change_24hrsTo12Hours_CheckHours()
        {
            var comp = await OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>();
            // count hours
            underlyingPicker.Instance.AmPm.Should().Be(false);
            comp.FindAll("div.mud-hour").Count.Should().Be(24);
            // change to 12 hour

            await underlyingPicker.SetParametersAndRenderAsync(x => x.Add(p =>
                p.AmPm, true));

            // count hours
            underlyingPicker.Instance.AmPm.Should().Be(true);
            comp.FindAll("div.mud-hour").Count.Should().Be(12);
        }

        [Test]
        public void TimePicker_WithAmPmTrue_Displays12HourText()
        {
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(x => x.Culture, CultureInfo.InvariantCulture)
                .Add(x => x.AmPm, true)
                .Add(x => x.Time, new TimeSpan(17, 45, 0)));

            comp.Find("input").GetAttribute("value").Should().Be("05:45 PM");
        }

        [Test]
        public async Task OpenToHours_CheckMinutesHidden()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, OpenTo.Hours));
            // Are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToHours_ChangeTo_Minutes_ReOpen_CheckStillHours()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, OpenTo.Hours));
            // Are minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click on the minutes input
            await comp.FindAll("button.mud-timepicker-button")[1].ClickAsync();
            // clicking outside to close
            await comp.Find("div.mud-overlay").ClickAsync();
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            await comp.FindAll("input")[0].ClickAsync();
            // Are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToMinutes_CheckHoursHidden()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, OpenTo.Minutes));
            // Are Hours hidden
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task TimeEditModeMinutes_CheckHoursHidden()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.TimeEditMode, TimeEditMode.OnlyMinutes));
            // Are Hours hidden
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task TimeEditModeHours_CheckMinutesHidden()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.TimeEditMode, TimeEditMode.OnlyHours));
            // Are Minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task TimeEditModeNormal_CheckMinutesHidden()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.TimeEditMode, TimeEditMode.Normal));
            // Are Minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task ChangeToMinutes_FromHours_CheckHoursHidden()
        {
            var comp = await OpenPicker();
            // click on the minutes input
            await comp.FindAll("button.mud-timepicker-button")[1].ClickAsync();
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task InputStringValues_CheckParsing()
        {
            var comp = Context.Render<MudTimePicker>();
            var picker = comp.Instance;

            // valid time
            await comp.Find("input").ChangeAsync("23:02");
            picker.TimeIntermediate.Should().Be(new TimeSpan(23, 2, 0));
            picker.ConversionError.Should().BeFalse();
            picker.ConversionErrorMessage.Should().BeNull();
            // empty string equals null TimeSpan?
            await comp.Find("input").ChangeAsync("");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeFalse();
            picker.ConversionErrorMessage.Should().BeNull();
            // invalid time (format, AmPm)
            await comp.Find("input").ChangeAsync("09:o6 AM");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
            comp.Find("input").GetAttribute("aria-invalid").Should().Be("true");
            // invalid time (overflow, AmPm)
            await comp.Find("input").ChangeAsync("13:45 AM");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
            // invalid time (format)
            await comp.Find("input").ChangeAsync("2o:32");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
            // invalid time (overflow)
            await comp.Find("input").ChangeAsync("25:06");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.Render<SimpleTimePickerTest>();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
            // clicking the button should open the picker
            await comp.Instance.Open();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(1);
            // closing programmatically
            await comp.Instance.Close();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
        }

        [Test]
        public async Task TimePicker_KeyboardNavigation()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<SimpleTimePickerTest>();
            var timePickerComponent = comp.FindComponent<MudTimePicker>();
            var timePicker = timePickerComponent.Instance;

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Time, new TimeSpan(02, 00, 00)));
            await comp.WaitForAssertionAsync(() => comp.Instance.Time.Should().Be(new TimeSpan(02, 00, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 00, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 00, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 59, 00)));
            //Enter keys submit, so time should only change with enter
            await comp.WaitForAssertionAsync(() => timePicker.ReadValue.Should().Be(new TimeSpan(02, 00, 00)));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.ReadValue.Should().Be(new TimeSpan(01, 59, 00)));
            //If Open is false, arrowkeys should now change TimeIntermediate
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.ReadValue.Should().Be(new TimeSpan(01, 59, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 00, 00)));
            //Escape key should turn last submitted time
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 59, 00)));
            await comp.WaitForAssertionAsync(() => timePicker.ReadValue.Should().Be(new TimeSpan(01, 59, 00)));
            //Space key should also submit
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 00, 00)));
            await comp.WaitForAssertionAsync(() => timePicker.ReadValue.Should().Be(new TimeSpan(02, 00, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = " ", CtrlKey = true, Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", CtrlKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(03, 00, 00)));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Time, new TimeSpan(03, 56, 00)));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(04, 01, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(03, 56, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", CtrlKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 56, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 51, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 56, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowUp", ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(07, 56, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "ArrowDown", ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 56, 00)));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true, ShiftKey = true, Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().Be(null));
            await comp.WaitForAssertionAsync(() => timePicker.ReadValue.Should().Be(null));

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            //When its disabled, keys should not work
            await timePickerComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));

            await timePicker.FocusAsync();
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await timePickerComponent.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.TimeFormat, "hhmm")
                .Add(x => x.ReadOnly, true));

            await comp.InvokeAsync(timePicker.SubmitAsync);
        }

        /// <summary>
        /// A time picker with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void DatePickerWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudTimePicker>(parameters
                => parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A time picker with a label and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void DatePickerWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "test-id";
            var comp = Context.Render<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", expectedId }
                    }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        [Test]
        public void TimePickerInputId()
        {
            var comp = Context.Render<SimpleTimePickerTest>(parameters => parameters
                .Add(c => c.InputId, "start-time"));

            comp.Find("input[id='start-time']").Should().NotBeNull();
        }

        [Test]
        public void TimePicker_CustomClearIcon_Should_BeRenderedInMarkup()
        {
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(p => p.Time, new TimeSpan(10, 30, 0))
                .Add(p => p.Editable, true)
                .Add(p => p.Clearable, true)
                .Add(p => p.ClearIcon, Icons.Custom.Brands.MudBlazor));

            comp.Markup.Should().Contain(comp.Instance.ClearIcon);
        }

        [Test]
        public async Task StaticReadOnly_ShouldNotChangeTime()
        {
            var initialTime = new TimeSpan(10, 30, 0);
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(p => p.PickerVariant, PickerVariant.Static)
                .Add(p => p.ReadOnly, true)
                .Add(p => p.Time, initialTime));
            var picker = comp.Instance;

            // Simulate clock stick interaction (as invoked from JS)
            await comp.InvokeAsync(() => picker.SelectTimeFromStick(5, false));

            // Time should remain unchanged because ReadOnly is true
            picker.Time.Should().Be(initialTime);
            picker.TimeIntermediate.Should().Be(initialTime);
        }
    }
}
