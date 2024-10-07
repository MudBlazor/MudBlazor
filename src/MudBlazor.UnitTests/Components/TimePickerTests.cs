using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.TimePicker;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TimePickerTests : BunitTest
    {
        public IRenderedComponent<SimpleTimePickerTest> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker([parameter]);
        }

        public IRenderedComponent<SimpleTimePickerTest> OpenPicker(ComponentParameter[] parameters = null)
        {
            IRenderedComponent<SimpleTimePickerTest> comp;
            if (parameters is null)
            {
                comp = Context.RenderComponent<SimpleTimePickerTest>();
            }
            else
            {
                comp = Context.RenderComponent<SimpleTimePickerTest>(parameters);
            }

            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            return comp;
        }

        [Test]
        public void TimePickerOpenButtonAriaLabel()
        {
            var comp = Context.RenderComponent<MudTimePicker>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open Time Picker");
        }

        [Test]
        public void TimePicker_Should_Clear()
        {
            var comp = Context.RenderComponent<MudTimePicker>();
            // select elements needed for the test
            var picker = comp.Instance;
            picker.Text.Should().Be(null);
            picker.Time.Should().Be(null);
            comp.SetParam(p => p.Clearable, true);
            comp.SetParam(p => p.Time, new TimeSpan(637940935730000000));
            picker.Time.Should().Be(new TimeSpan(637940935730000000));
            picker.Text.Should().Be(new TimeSpan(637940935730000000).ToIsoString());

            comp.Find("button").Click(); //clear the input

            picker.Text.Should().Be(""); //ensure the text and time are reset. Note this is an empty string rather than null due to how the reset works internally
            picker.Time.Should().Be(null);
        }

        [Test]
        public void Open_ClickOutside_CheckClosed()
        {
            var comp = OpenPicker();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open any more
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        [Test]
        public void Change_24hrsTo12Hours_CheckHours()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>();
            // count hours
            underlyingPicker.Instance.AmPm.Should().Be(false);
            comp.FindAll("div.mud-hour").Count.Should().Be(24);
            // change to 12 hour

            underlyingPicker.SetParametersAndRender(x => x.Add(p =>
                p.AmPm, true));

            // count hours
            underlyingPicker.Instance.AmPm.Should().Be(true);
            comp.FindAll("div.mud-hour").Count.Should().Be(12);
        }

        [Test]
        public void OpenToHours_CheckMinutesHidden()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Hours));
            // Are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void OpenToHours_ChangeTo_Minutes_ReOpen_CheckStillHours()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Hours));
            // Are minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click on the minutes input
            comp.FindAll("button.mud-timepicker-button")[1].Click();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.FindAll("input")[0].Click();
            // Are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void OpenToMinutes_CheckHoursHidden()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Minutes));
            // Are Hours hidden
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void TimeEditModeMinutes_CheckHoursHidden()
        {
            var comp = OpenPicker(Parameter("TimeEditMode", TimeEditMode.OnlyMinutes));
            // Are Hours hidden
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void TimeEditModeHours_CheckMinutesHidden()
        {
            var comp = OpenPicker(Parameter("TimeEditMode", TimeEditMode.OnlyHours));
            // Are Minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void TimeEditModeNormal_CheckMinutesHidden()
        {
            var comp = OpenPicker(Parameter("TimeEditMode", TimeEditMode.Normal));
            // Are Minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void ChangeToMinutes_FromHours_CheckHoursHidden()
        {
            var comp = OpenPicker();
            // click on the minutes input
            comp.FindAll("button.mud-timepicker-button")[1].Click();
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void InputStringValues_CheckParsing()
        {
            var comp = Context.RenderComponent<MudTimePicker>();
            var picker = comp.Instance;

            // valid time
            comp.Find("input").Change("23:02");
            picker.TimeIntermediate.Should().Be(new TimeSpan(23, 2, 0));
            picker.ConversionError.Should().BeFalse();
            picker.ConversionErrorMessage.Should().BeNull();
            // empty string equals null TimeSpan?
            comp.Find("input").Change("");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeFalse();
            picker.ConversionErrorMessage.Should().BeNull();
            // invalid time (format, AmPm)
            comp.Find("input").Change("09:o6 AM");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
            // invalid time (overflow, AmPm)
            comp.Find("input").Change("13:45 AM");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
            // invalid time (format)
            comp.Find("input").Change("2o:32");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
            // invalid time (overflow)
            comp.Find("input").Change("25:06");
            picker.TimeIntermediate.Should().BeNull();
            picker.ConversionError.Should().BeTrue();
            picker.ConversionErrorMessage.Should().Be("Not a valid time span");
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.RenderComponent<SimpleTimePickerTest>();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
            // clicking the button should open the picker
            await comp.Instance.Open();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(1);
            // closing programmatically
            await comp.Instance.Close();
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
        }

        [Test]
        public async Task TimePickerTest_KeyboardNavigation()
        {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            var comp = Context.RenderComponent<SimpleTimePickerTest>();
            var timePicker = comp.FindComponent<MudTimePicker>().Instance;
            var overlay = comp.FindComponent<MudOverlay>();

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", AltKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            comp.SetParam("Time", new TimeSpan(02, 00, 00));
            comp.WaitForAssertion(() => comp.Instance.Time.Should().Be(new TimeSpan(02, 00, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 00, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 00, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 59, 00)));
            //Enter keys submit, so time should only change with enter
            comp.WaitForAssertion(() => timePicker.Time.Should().Be(new TimeSpan(02, 00, 00)));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.Time.Should().Be(new TimeSpan(01, 59, 00)));
            //If Open is false, arrowkeys should now change TimeIntermediate
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 59, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 00, 00)));
            //Escape key should turn last submitted time
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(01, 59, 00)));
            comp.WaitForAssertion(() => timePicker.Time.Should().Be(new TimeSpan(01, 59, 00)));
            //Space key should also submit
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 00, 00)));
            comp.WaitForAssertion(() => timePicker.Time.Should().Be(new TimeSpan(02, 00, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = " ", CtrlKey = true, Type = "keydown", }));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(03, 00, 00)));

            comp.SetParam("Time", new TimeSpan(03, 56, 00));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(04, 01, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(03, 56, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", CtrlKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 56, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 51, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 56, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(07, 56, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(new TimeSpan(02, 56, 00)));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Backspace", CtrlKey = true, ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => timePicker.TimeIntermediate.Should().Be(null));
            comp.WaitForAssertion(() => timePicker.Time.Should().Be(null));

            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            //When its disabled, keys should not work
            timePicker.Disabled = true;
            await timePicker.FocusAsync();
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            await comp.InvokeAsync(() => timePicker.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter", Type = "keydown", }));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            await comp.InvokeAsync(() => timePicker.TimeFormat = "hhmm");
            await comp.InvokeAsync(() => timePicker.TimeFormat = "hhmm");

            timePicker.ReadOnly = true;
            await comp.InvokeAsync(timePicker.SubmitAsync);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
        }

        /// <summary>
        /// A time picker with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void DatePickerWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.RenderComponent<MudTimePicker>(parameters
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
            var comp = Context.RenderComponent<MudTimePicker>(parameters
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
    }
}
