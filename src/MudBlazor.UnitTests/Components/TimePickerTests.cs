using System.Globalization;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
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
        [TestCase(false, "", "17:45")]          // 24-hour default
        [TestCase(true, "", "05:45 PM")]         // 12-hour default when AmPm is on
        [TestCase(false, "hh.mm tt", "05.45 PM")] // a custom TimeFormat overrides the AmPm-derived default
        [TestCase(true, "HH-mm", "17-45")]
        public void TimePicker_FormatsInputValue(bool amPm, string timeFormat, string expected)
        {
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(x => x.Culture, CultureInfo.InvariantCulture)
                .Add(x => x.AmPm, amPm)
                .Add(x => x.TimeFormat, timeFormat)
                .Add(x => x.Time, new TimeSpan(17, 45, 0)));

            comp.Find("input").GetAttribute("value").Should().Be(expected);
        }

        [Test]
        [TestCase(OpenTo.Hours, "minute")]
        [TestCase(OpenTo.Minutes, "hour")]
        public async Task OpenTo_ShowsRequestedDial_HidesOther(OpenTo openTo, string hiddenDial)
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, openTo));

            comp.FindAll($"div.mud-time-picker-{hiddenDial}.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task OpenToHours_ChangeTo_Minutes_ReOpen_CheckStillHours()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, OpenTo.Hours));
            // Are minutes hidden
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click on the minutes input
            await comp.FindAll("button.mud-timepicker-button")[1].ClickAsync();
            // the view switched to minutes, so the hour dial is now the hidden one
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // clicking outside to close
            await comp.Find("div.mud-overlay").ClickAsync();
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            await comp.FindAll("input")[0].ClickAsync();
            // Are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        // Normal defers to OpenTo (default Hours); OnlyHours/OnlyMinutes force their single dial.
        [Test]
        [TestCase(TimeEditMode.Normal, "minute")]
        [TestCase(TimeEditMode.OnlyHours, "minute")]
        [TestCase(TimeEditMode.OnlyMinutes, "hour")]
        public async Task TimeEditMode_ShowsOnlyEditableDial(TimeEditMode mode, string hiddenDial)
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.TimeEditMode, mode));

            comp.FindAll($"div.mud-time-picker-{hiddenDial}.mud-time-picker-dial-hidden").Count.Should().Be(1);
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
        }

        /// <summary>
        /// Regression test for the PR #13328 review: the Time parameter setter is suppressed so a
        /// parameter-driven value does not touch the picker, but a genuine user submit (Enter) must still
        /// mark the picker Touched and fire MudForm.FieldChanged.
        /// </summary>
        [Test]
        public async Task TimePicker_UserSubmit_TouchesAndFiresFieldChanged()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<FormTimePickerSubmitTest>();
            var timePicker = comp.FindComponent<MudTimePicker>().Instance;

            timePicker.Touched.Should().BeFalse();
            comp.Instance.FormFieldChangedEventArgs.Should().BeNull();

            // Open the picker and change the time with the keyboard.
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs { Key = "ArrowUp", Type = "keydown" }));
            await comp.WaitForAssertionAsync(() => timePicker.TimeIntermediate.Should().NotBeNull());

            // Submit the user-picked time.
            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(timePicker.ElementId, new KeyboardEventArgs { Key = "Enter", Type = "keydown" }));

            await comp.WaitForAssertionAsync(() => timePicker.Touched.Should().BeTrue());
            comp.Instance.FormFieldChangedEventArgs.Should().NotBeNull();
        }

        /// <summary>
        /// A time picker with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void TimePickerWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
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
        public void TimePickerWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
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

            // SubmitAsync is likewise a no-op while ReadOnly
            await comp.InvokeAsync(picker.SubmitAsync);
            picker.Time.Should().Be(initialTime);
        }

        [Test]
        public async Task SelectTimeFromStick_IgnoresSentinelValue()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.Time, new TimeSpan(8, 20, 0)));
            var picker = comp.FindComponent<MudTimePicker>().Instance;

            // -1 signals that no stick was the event target; it must be a no-op.
            await comp.InvokeAsync(() => picker.SelectTimeFromStick(-1, false));

            picker.TimeIntermediate.Should().Be(new TimeSpan(8, 20, 0));
        }

        [Test]
        public async Task OnStickClick_OnHour_SwitchesToMinutesView_InNormalMode()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, OpenTo.Hours));
            var picker = comp.FindComponent<MudTimePicker>().Instance;

            // Picking an hour in Normal mode advances the dial to minutes.
            await comp.InvokeAsync(() => picker.OnStickClick(3));

            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task OnStickClick_OnMinute_SubmitsAndClosesPicker()
        {
            // ClosingDelay = 0 removes the post-commit close timer so the minute click commits and closes
            // deterministically, without depending on a timer settling (advancing a fake clock after the
            // commit races the timer registration and hangs under CI load).
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Minutes)
                .Add(x => x.ClosingDelay, 0));
            var picker = comp.FindComponent<MudTimePicker>().Instance;

            await comp.InvokeAsync(() => picker.SelectTimeFromStick(20, false));

            await comp.InvokeAsync(() => picker.OnStickClick(20));

            await comp.WaitForAssertionAsync(() =>
            {
                picker.Time.Should().Be(new TimeSpan(0, 20, 0));
                comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            });
        }

        [Test]
        public async Task OnStickClick_OnHour_InOnlyHoursMode_SubmitsAndCloses()
        {
            // OnlyHours mode has nothing left to pick, so the hour click commits and closes. ClosingDelay = 0
            // removes the post-commit close timer so the close is deterministic, without depending on a timer
            // settling (advancing a fake clock after the commit races the timer registration and hangs).
            var comp = await OpenPicker(parameters => parameters
                .Add(x => x.OpenTo, OpenTo.Hours)
                .Add(x => x.TimeEditMode, TimeEditMode.OnlyHours)
                .Add(x => x.ClosingDelay, 0));
            var picker = comp.FindComponent<MudTimePicker>().Instance;

            await comp.InvokeAsync(() => picker.SelectTimeFromStick(9, false));

            await comp.InvokeAsync(() => picker.OnStickClick(9));

            await comp.WaitForAssertionAsync(() =>
            {
                picker.Time.Should().Be(new TimeSpan(9, 0, 0));
                comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            });
        }

        [Test]
        [TestCase(7, 0)]   // rounds down to 0
        [TestCase(8, 15)]  // rounds up to 15
        [TestCase(22, 15)] // rounds down to 15
        [TestCase(23, 30)] // rounds up to 30
        [TestCase(58, 0)]  // rounds up to 60, which wraps back to 0
        public async Task MinuteSelectionStep_SnapsSelectedMinuteToInterval(int rawMinute, int expectedMinute)
        {
            // Static + no actions submits immediately, so the snapped value lands on Time.
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(x => x.PickerVariant, PickerVariant.Static)
                .Add(x => x.OpenTo, OpenTo.Minutes)
                .Add(x => x.MinuteSelectionStep, 15));
            var picker = comp.Instance;

            await comp.InvokeAsync(() => picker.SelectTimeFromStick(rawMinute, false));

            picker.Time!.Value.Minutes.Should().Be(expectedMinute);
        }

        [Test]
        [TestCase(9, false, 9)]   // AM: 9 stays 9
        [TestCase(12, false, 0)]  // AM: 12 maps to 0
        [TestCase(3, true, 15)]   // PM: 3 maps to 15
        [TestCase(12, true, 12)]  // PM: 12 stays 12
        public async Task AmPm_ConvertsClickedHourTo24HourClock(int clickedHour, bool pm, int expectedHour24)
        {
            var startTime = pm ? new TimeSpan(13, 0, 0) : new TimeSpan(9, 0, 0);
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(x => x.PickerVariant, PickerVariant.Static)
                .Add(x => x.AmPm, true)
                .Add(x => x.OpenTo, OpenTo.Hours)
                .Add(x => x.Time, startTime));
            var picker = comp.Instance;

            await comp.InvokeAsync(() => picker.SelectTimeFromStick(clickedHour, false));

            picker.Time!.Value.Hours.Should().Be(expectedHour24);
        }

        [Test]
        public async Task Toolbar_AmPmButtons_ToggleBetween12And24Hour()
        {
            var comp = Context.Render<MudTimePicker>(parameters => parameters
                .Add(x => x.PickerVariant, PickerVariant.Static)
                .Add(x => x.AmPm, true)
                .Add(x => x.Time, new TimeSpan(15, 30, 0)));
            var picker = comp.Instance;

            // Toolbar buttons are [0]=hours [1]=minutes [2]=AM [3]=PM.
            await comp.FindAll("button.mud-timepicker-button")[2].ClickAsync();
            picker.Time.Should().Be(new TimeSpan(3, 30, 0));

            await comp.FindAll("button.mud-timepicker-button")[3].ClickAsync();
            picker.Time.Should().Be(new TimeSpan(15, 30, 0));
        }

        [Test]
        public async Task Toolbar_HoursButton_SwitchesBackToHoursView()
        {
            var comp = await OpenPicker(parameters => parameters.Add(x => x.OpenTo, OpenTo.Minutes));
            // Starts on the minutes view, so the hour dial is hidden.
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            await comp.FindAll("button.mud-timepicker-button")[0].ClickAsync();

            // The hours toolbar button brings the hour dial back and hides the minute dial.
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public async Task PickerActions_WithAutoClose_CommitsTimeOnClockSelection()
        {
            var comp = Context.Render<AutoCompleteTimePickerTest>(parameters => parameters.Add(x => x.AutoClose, true));
            var picker = comp.Instance.Picker;
            await comp.InvokeAsync(() => picker.OpenAsync());

            // With PickerActions defined but AutoClose enabled, a clock selection commits without clicking OK.
            await comp.InvokeAsync(() => picker.SelectTimeFromStick(5, false));

            await comp.WaitForAssertionAsync(() => picker.Time.Should().Be(new TimeSpan(5, 45, 0)));
        }

        [Test]
        public async Task OnClick_Callback_FiresWhenInputClicked()
        {
            var clicked = false;
            var comp = Context.Render<SimpleTimePickerTest>();
            var picker = comp.FindComponent<MudTimePicker>();
            await picker.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicked = true)));

            await comp.Find("input").ClickAsync();

            // Clicking a non-editable picker's input toggles it open and invokes the OnClick callback.
            clicked.Should().BeTrue();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));
        }

        [Test]
        public async Task ClearAsync_WithAutoClose_ClosesPicker()
        {
            var comp = Context.Render<AutoCompleteTimePickerTest>(parameters => parameters.Add(x => x.AutoClose, true));
            var picker = comp.Instance.Picker;
            await comp.InvokeAsync(() => picker.OpenAsync());
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => picker.ClearAsync());

            picker.Time.Should().BeNull();
            await comp.WaitForAssertionAsync(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
        }
    }
}
