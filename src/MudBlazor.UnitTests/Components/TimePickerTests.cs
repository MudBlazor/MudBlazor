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
            picker.ReadOnly.Should().Be(false);
            picker.Text.Should().Be(null);
            picker.Time.Should().Be(null);
            comp.SetParam(p => p.Clearable, true);
            comp.SetParam(p => p.Time, new TimeSpan(637940935730000000));
            picker.Time.Should().Be(new TimeSpan(637940935730000000));
            picker.Text.Should().Be(new TimeSpan(637940935730000000).ToIsoString());

            comp.Find(".mud-input-clear-button").Click(); //clear the input

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

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        [TestCase(8, 8)]
        [TestCase(9, 9)]
        [TestCase(10, 10)]
        [TestCase(11, 11)]
        [TestCase(12, 12)]
        [TestCase(13, 13)]
        [TestCase(14, 14)]
        [TestCase(15, 15)]
        [TestCase(16, 16)]
        [TestCase(17, 17)]
        [TestCase(18, 18)]
        [TestCase(19, 19)]
        [TestCase(20, 20)]
        [TestCase(21, 21)]
        [TestCase(22, 22)]
        [TestCase(23, 22)]
        public void MinTime_Should_Hide_Disabled_Hours_In_24HourFormat(int hour, int numberOfDisabledItems)
        {
            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.PickerVariant, PickerVariant.Static)
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan((hour + 1) % 24, 0, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number").Count.Should().Be(24);
            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number.mud-hidden").Count.Should().Be(numberOfDisabledItems);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        [TestCase(8, 8)]
        [TestCase(9, 9)]
        [TestCase(10, 10)]
        [TestCase(11, 11)]
        [TestCase(12, 0)]
        [TestCase(13, 1)]
        [TestCase(14, 2)]
        [TestCase(15, 3)]
        [TestCase(16, 4)]
        [TestCase(17, 5)]
        [TestCase(18, 6)]
        [TestCase(19, 7)]
        [TestCase(20, 8)]
        [TestCase(21, 9)]
        [TestCase(22, 10)]
        [TestCase(23, 10)]
        public void MinTime_Should_Hide_Disabled_Hours_In_12HourFormat(int hour, int numberOfDisabledItems)
        {
            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.PickerVariant, PickerVariant.Static)
                    .Add(p => p.AmPm, true)
                    .Add(p => p.Time, (hour == 11) ? new TimeSpan(0, 0, 0) : (hour == 23) ? new TimeSpan(12, 0, 0) : new TimeSpan((hour + 1) % 24, 0, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number.mud-hidden").Count.Should().Be(numberOfDisabledItems);
        }

        [Test]
        [TestCase(0, 23)]
        [TestCase(1, 22)]
        [TestCase(2, 21)]
        [TestCase(3, 20)]
        [TestCase(4, 19)]
        [TestCase(5, 18)]
        [TestCase(6, 17)]
        [TestCase(7, 16)]
        [TestCase(8, 15)]
        [TestCase(9, 14)]
        [TestCase(10, 13)]
        [TestCase(11, 12)]
        [TestCase(12, 11)]
        [TestCase(13, 10)]
        [TestCase(14, 9)]
        [TestCase(15, 8)]
        [TestCase(16, 7)]
        [TestCase(17, 6)]
        [TestCase(18, 5)]
        [TestCase(19, 4)]
        [TestCase(20, 3)]
        [TestCase(21, 2)]
        [TestCase(22, 1)]
        [TestCase(23, 0)]
        public void MaxTime_Should_Hide_Disabled_Hours_In_24HourFormat(int hour, int numberOfDisabledItems)
        {
            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.PickerVariant, PickerVariant.Static)
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan((hour - 1) % 24, 0, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number").Count.Should().Be(24);
            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number.mud-hidden").Count.Should().Be(numberOfDisabledItems);
        }

        [Test]
        [TestCase(0, 12)]
        [TestCase(1, 10)]
        [TestCase(2, 9)]
        [TestCase(3, 8)]
        [TestCase(4, 7)]
        [TestCase(5, 6)]
        [TestCase(6, 5)]
        [TestCase(7, 4)]
        [TestCase(8, 3)]
        [TestCase(9, 2)]
        [TestCase(10, 1)]
        [TestCase(11, 0)]
        [TestCase(12, 11)]
        [TestCase(13, 10)]
        [TestCase(14, 9)]
        [TestCase(15, 8)]
        [TestCase(16, 7)]
        [TestCase(17, 6)]
        [TestCase(18, 5)]
        [TestCase(19, 4)]
        [TestCase(20, 3)]
        [TestCase(21, 2)]
        [TestCase(22, 1)]
        [TestCase(23, 0)]
        public void MaxTime_Should_Hide_Disabled_Hours_In_12HourFormat(int hour, int numberOfDisabledItems)
        {
            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.PickerVariant, PickerVariant.Static)
                    .Add(p => p.AmPm, true)
                    .Add(p => p.Time, (hour == 12) ? new TimeSpan(23, 0, 0) : (hour == 23) ? new TimeSpan(12, 0, 0) : new TimeSpan((hour - 1) % 24, 0, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-hour > p.mud-clock-number.mud-hidden").Count.Should().Be(numberOfDisabledItems);
        }

        [Test]
        [TestCase(0, 11)]
        [TestCase(5, 10)]
        [TestCase(10, 9)]
        [TestCase(15, 8)]
        [TestCase(20, 7)]
        [TestCase(25, 6)]
        [TestCase(30, 5)]
        [TestCase(35, 4)]
        [TestCase(40, 3)]
        [TestCase(45, 2)]
        [TestCase(50, 1)]
        [TestCase(55, 0)]
        public void MaxTime_Should_Hide_Disabled_Minutes(int minutes, int numberOfDisabledItems)
        {
            int hour = 5;

            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.PickerVariant, PickerVariant.Static)
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 1, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, minutes, 0))
                    .Add(p => p.OpenTo, OpenTo.Minutes));

            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number.mud-hidden").Count.Should().Be(numberOfDisabledItems);

            comp.SetParametersAndRender(x => x.Add(p =>
                p.Time, new TimeSpan(hour + 1, 1, 0)));

            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number.mud-hidden").Count.Should().Be(12);

            comp.SetParametersAndRender(x => x.Add(p =>
                p.Time, new TimeSpan(hour - 1, 1, 0)));

            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number.mud-hidden").Count.Should().Be(0);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(5, 1)]
        [TestCase(10, 2)]
        [TestCase(15, 3)]
        [TestCase(20, 4)]
        [TestCase(25, 5)]
        [TestCase(30, 6)]
        [TestCase(35, 7)]
        [TestCase(40, 8)]
        [TestCase(45, 9)]
        [TestCase(50, 10)]
        [TestCase(55, 11)]
        public void MinTime_Should_Hide_Disabled_Minutes(int minutes, int numberOfDisabledItems)
        {
            int hour = 5;

            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.PickerVariant, PickerVariant.Static)
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 1, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, minutes, 0))
                    .Add(p => p.OpenTo, OpenTo.Minutes));

            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number.mud-hidden").Count.Should().Be(numberOfDisabledItems);

            comp.SetParametersAndRender(x => x.Add(p =>
                p.Time, new TimeSpan(hour + 1, 1, 0)));

            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number.mud-hidden").Count.Should().Be(0);

            comp.SetParametersAndRender(x => x.Add(p =>
                p.Time, new TimeSpan(hour - 1, 1, 0)));

            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number").Count.Should().Be(12);
            comp.FindAll("div.mud-time-picker-minute > p.mud-clock-number.mud-hidden").Count.Should().Be(12);
        }

        [Test]
        public async Task KeyboardNavigation_GetNextValidHourInterval()
        {
            int hour = 12;

            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 0, 0))
                    .Add(p => p.MinTime, new TimeSpan(11, 0, 0))
                    .Add(p => p.MaxTime, new TimeSpan(13, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            var instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            // Increase hours by one, 12 => 13
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(13));

            // Increase hours by one but because MaxTime.Hour == 13 and MinTime.Hour == 11 so the next available hour is 11
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(11));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 0, 0))
                    .Add(p => p.MinTime, new TimeSpan(11, 0, 0))
                    .Add(p => p.MaxTime, new TimeSpan(13, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(11));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(13));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 0, 0))
                    .Add(p => p.MinTime, new TimeSpan(6, 0, 0))
                    .Add(p => p.MaxTime, new TimeSpan(18, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            // Increase hours by 5, 12 => 17
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(17));
            // Increase hours by 5 but because 17 + 5 is not a valid hour we find the first available hour: 18
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(18));
            // Increase hours by 5 but because 18 + 5 is not a valid hour we find the first available hour: 6
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowUp", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(6));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 0, 0))
                    .Add(p => p.MinTime, new TimeSpan(6, 0, 0))
                    .Add(p => p.MaxTime, new TimeSpan(18, 0, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(7));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(6));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowDown", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(18));
        }

        [Test]
        public async Task KeyboardNavigation_GetNextValidMinuteInterval()
        {
            int hour = 12;

            var comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 30, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 29, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 31, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            var instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            // Increase minutes by one, 12:30 => 12:31
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(31));

            // Increase minutes by one but because MaxTime == 12:31 and MinTime.Hour == 12:29 so the next available time is 12:29
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(29));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 30, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 29, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 31, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(29));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(31));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 30, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 24, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 36, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            // Increase minutes by 5, 12:30 => 12:35
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(35));
            // Increase minutes by 5 but because 12:40 is not a valid hour we increase by one minute: 12:35 => 12:36
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(36));
            // Increase minutes by 5 but because 12:41 is not a valid hour we find the first available time: 12:36 => 12:24
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(24));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 30, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 24, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 36, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(25));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(24));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(36));

            comp = Context.RenderComponent<MudTimePicker>(parameters
                => parameters
                    .Add(p => p.AmPm, false)
                    .Add(p => p.Time, new TimeSpan(hour, 30, 0))
                    .Add(p => p.MinTime, new TimeSpan(hour, 30, 0))
                    .Add(p => p.MaxTime, new TimeSpan(hour, 30, 0))
                    .Add(p => p.OpenTo, OpenTo.Hours));

            instance = comp.Instance;
            await comp.InvokeAsync(() => instance.OpenAsync());

            //Testing for no infinite loop if no time is available
            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowRight", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(30));

            await comp.InvokeAsync(() => instance.OnHandleKeyDownAsync(new KeyboardEventArgs() { Key = "ArrowLeft", ShiftKey = true, Type = "keydown", }));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Hours.Should().Be(12));
            comp.WaitForAssertion(() => instance.TimeIntermediate.Value.Minutes.Should().Be(30));
        }
    }
}
