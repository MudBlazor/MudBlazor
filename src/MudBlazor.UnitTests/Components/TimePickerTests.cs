#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
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
        public void SelectTime_UsingClicks_24HourMode_Midnight_CheckTime()
        {
            var comp = OpenPicker();
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // select 00 hours
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[11].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(0);
            underlyingPicker.TimeIntermediate.Value.Days.Should().Be(0);
        }

        [Test]
        public async Task SelectTime_UsingClicks_24HourMode_CheckTimeAsync()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // select 16 hours on outer dial and 30 mins
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(16);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);

            // check if view changed to minutes
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // select 30 minutes
            comp.FindAll("div.mud-minute")[30].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(16);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(30);

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            // open picker
            await comp.Instance.Open();

            // click 04 hours on the inner dial and 21 mins
            //comp.FindAll("button.mud-timepicker-button")[0].Click();
            comp.FindAll("div.mud-picker-stick-inner.mud-hour")[3].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(4);

            // check if view changed to minutes
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click 21 mins
            comp.FindAll("div.mud-minute")[21].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(21);

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(4);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(21);

            // open picker
            await comp.InvokeAsync(picker.Open);

            // click 10 hours
            comp.FindAll("div.mud-picker-stick-inner.mud-hour")[9].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(10);

            // check if view changed to minutes
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click 56 mins
            comp.FindAll("div.mud-minute")[56].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(56);

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
        }

        [Test]
        public void SelectTime_UsingClicks_12HourMode_CheckTime()
        {
            var comp = OpenPicker(Parameter("AmPm", true));
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // select 11 hours on outer dial and 30 mins
            comp.FindAll("div.mud-hour")[10].Click();
            comp.FindAll("div.mud-minute")[30].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(11);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(30);
            // click 04 hours on the inner dial and 21 mins
            comp.FindAll("button.mud-timepicker-button")[0].Click();
            comp.FindAll("div.mud-hour")[11].Click();
            comp.FindAll("div.mud-minute")[21].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(0);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(21);
            // click 10 hours on the inner dial and 56 mins
            comp.FindAll("button.mud-timepicker-button")[0].Click();
            comp.FindAll("div.mud-hour")[9].Click();
            comp.FindAll("div.mud-minute")[56].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(10);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(56);
            // click pm button
            comp.FindAll("button.mud-timepicker-button")[3].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(22);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(56);
        }

        [Test]
        public void SelectTime_UsingClicks_12HourMode_ChangeAmPm_CheckTime()
        {
            var comp = Context.RenderComponent<MudTimePicker>(x =>
            {
                x.Add(p => p.PickerVariant, PickerVariant.Static);
                x.Add(p => p.AmPm, true);
            });
            var picker = comp.Instance;

            // select 11 hours on outer dial and 30 mins
            comp.FindAll("div.mud-hour")[10].Click();
            comp.FindAll("div.mud-minute")[30].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(11);
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);
            // click pm button
            comp.FindAll("button.mud-timepicker-button")[3].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(23);
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);
            // add 12 hours in pm mode
            comp.FindAll("div.mud-hour")[10].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(23);
            // click am button should subtract 12 hours
            comp.FindAll("button.mud-timepicker-button")[2].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(11);
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);
        }

        [Test]
        public void MinuteSelectionStep_15_SelectTime_UsingClicks_CheckTime()
        {
            var comp = OpenPicker(Parameter(nameof(MudTimePicker.MinuteSelectionStep), 15));
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // select 16 hours on outer dial
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(16);

            // click 2 minutes on the dial result rounds down to 00
            comp.FindAll("div.mud-minute")[2].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(00);
            comp.FindAll("div.mud-time-picker-minute p.mud-theme-primary")[0].TextContent.Should().Be("00");
            // click 18 minutes on the dial result rounds down to 15
            comp.FindAll("div.mud-minute")[18].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(15);
            comp.FindAll("div.mud-time-picker-minute p.mud-theme-primary")[0].TextContent.Should().Be("15");
            // click 30 minutes on the dial result is 30
            comp.FindAll("div.mud-minute")[30].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(30);
            comp.FindAll("div.mud-time-picker-minute p.mud-theme-primary")[0].TextContent.Should().Be("30");
            // click 43 minutes on the dial result rounds up to 45
            comp.FindAll("div.mud-minute")[43].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(45);
            comp.FindAll("div.mud-time-picker-minute p.mud-theme-primary")[0].TextContent.Should().Be("45");
            // click 57 minutes on the dial result rounds 'up' to 00
            comp.FindAll("div.mud-minute")[57].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
            comp.FindAll("div.mud-time-picker-minute p.mud-theme-primary")[0].TextContent.Should().Be("00");
        }

        /// <summary>
        /// Check that using an invalid value of 0 is ignored
        /// </summary>
        [Test]
        public void MinuteSelectionStep_0_SelectTime_UsingClicks_CheckTime()
        {
            var comp = Context.RenderComponent<MudTimePicker>([Parameter(nameof(MudTimePicker.MinuteSelectionStep), 0), Parameter("TimeEditMode", TimeEditMode.OnlyMinutes), Parameter("PickerVariant", PickerVariant.Static), Parameter("OpenTo", OpenTo.Minutes)]);
            var picker = comp.Instance;

            // Any minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click every minute
            for (var i = 0; i < 60; i++)
            {
                comp.FindAll("div.mud-minute")[i].Click();
                picker.TimeIntermediate.Value.Minutes.Should().Be(i);
            }
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

        /// <summary>
        /// Tests if picker works correctly with TimeEditMode.OnlyHours
        /// </summary>
        [Test]
        public void TimeEditModeHours_CheckSelection()
        {
            var comp = Context.RenderComponent<SimpleTimePickerTest>(Parameter("TimeEditMode", TimeEditMode.OnlyHours));
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // click to to open picker
            comp.Find("input").Click();

            // should be open
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            // click on 13 hour
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[0].Click();

            // should be closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            // should be 13 hours
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);

            // click to to open picker
            comp.Find("input").Click();

            // should be open
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(1));

            //click on 14 hour
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[1].Click();

            // should be closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            // should be 14 hours
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(14);
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
        public void DragPointer_SelectHour_CheckMinutesAppear()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // click on the minutes input
            comp.Find("div.mud-time-picker-hour").PointerDown();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].PointerOver();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(16);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[5].PointerOver();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[5].PointerUp();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(18);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void DragPointer_SelectMinutes()
        {
            // Use bare component
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Minutes));
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // Any hours displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            comp.Find("div.mud-time-picker-minute").PointerDown();
            comp.FindAll("div.mud-minute")[3].PointerOver();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(3);
            comp.FindAll("div.mud-minute")[31].PointerOver();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(31);
            comp.FindAll("div.mud-minute")[5].PointerOver();
            comp.FindAll("div.mud-minute")[5].PointerUp();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(5);
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
        public void DragAndClick_AllHours12h_AM_TestCoverage()
        {
            var comp = OpenPicker([Parameter("OpenTo", OpenTo.Hours), Parameter("AmPm", true)]);
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (var i = 0; i < 12; i++)
            {
                var expected = i == 11 ? 0 : i + 1;
                comp.Find("div.mud-time-picker-hour").PointerDown();
                comp.FindAll("div.mud-hour")[i].PointerOver();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-hour")[i].PointerUp();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-hour")[i].Click();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
            }
        }

        [Test]
        public void DragAndClick_AllHours12h_PM_TestCoverage()
        {
            var comp = OpenPicker([
                Parameter("OpenTo", OpenTo.Hours),
                Parameter("AmPm", true),
                Parameter("Time", new TimeSpan(13, 0, 0))
            ]);
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (var i = 0; i < 12; i++)
            {
                var expected = i == 11 ? 12 : i + 13;
                comp.Find("div.mud-time-picker-hour").PointerDown();
                comp.FindAll("div.mud-hour")[i].PointerOver();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-hour")[i].PointerUp();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-hour")[i].Click();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
            }
        }

        [Test]
        public void DragAndClick_AllHours24h_TestCoverage()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Hours));
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag 13 to 00 on outer dial
            for (var i = 0; i < 12; i++)
            {
                var expected = i + 13 == 24 ? 0 : i + 13;
                comp.Find("div.mud-time-picker-hour").PointerDown();
                comp.FindAll("div.mud-picker-stick-outer.mud-hour")[i].PointerOver();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-picker-stick-outer.mud-hour")[i].PointerUp();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-picker-stick-outer.mud-hour")[i].Click();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
            }
            // click and drag 1 to 12 on inner dial
            for (var i = 0; i < 12; i++)
            {
                var expected = i + 1;
                comp.Find("div.mud-time-picker-hour").PointerDown();
                comp.FindAll("div.mud-picker-stick-inner.mud-hour")[i].PointerOver();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-picker-stick-inner.mud-hour")[i].PointerUp();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
                comp.FindAll("div.mud-picker-stick-inner.mud-hour")[i].Click();
                comp.WaitForAssertion(() => underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(expected));
            }
        }

        /// <summary>
        /// drag and pointerup on minutes for test coverage
        /// </summary>
        [Test]
        public void DragAndPointerUp_AllMinutes()
        {
            var comp = OpenPicker(Parameter("TimeEditMode", TimeEditMode.OnlyMinutes));
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // Any minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click and drag (hold pointer down)
            comp.Find("div.mud-time-picker-minute").PointerDown();
            for (var i = 0; i < 60; i++)
            {
                comp.FindAll("div.mud-minute")[i].PointerOver();
                underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(i);

                if (i == 59)
                {
                    comp.FindAll("div.mud-minute")[i].PointerUp();
                    underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(i);
                }
            }

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));
        }

        /// <summary>
        /// click every minute for test coverage
        /// </summary>
        [Test]
        public void Click_AllMinutes_Static()
        {
            var comp = Context.RenderComponent<MudTimePicker>([Parameter("TimeEditMode", TimeEditMode.OnlyMinutes), Parameter("PickerVariant", PickerVariant.Static), Parameter("OpenTo", OpenTo.Minutes)]);
            var picker = comp.Instance;

            // Any minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click every minute
            for (var i = 0; i < 60; i++)
            {
                comp.FindAll("div.mud-minute")[i].Click();
                picker.TimeIntermediate.Value.Minutes.Should().Be(i);
            }
        }

        /// <summary>
        /// drag and pointerup in every view
        /// </summary>
        [Test]
        public void SelectTime_UsingDrag_DefaultMode_CheckCloseWhenFinished()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // check if correct view (Hour)
            comp.FindAll("div.mud-time-picker-hour").Count.Should().Be(1);
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // drag to 13 and pointer up
            comp.Find("div.mud-time-picker-hour").PointerDown();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[0].PointerOver();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[0].PointerUp();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);

            // check if the view changed to minutes
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            comp.FindAll("div.mud-time-picker-minute").Count.Should().Be(1);

            // drag to 37 minutes and pointer up
            comp.Find("div.mud-time-picker-minute").PointerDown();
            comp.FindAll("div.mud-minute")[37].PointerOver();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);
            comp.FindAll("div.mud-minute")[37].PointerUp();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            // check correct time
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);
        }

        /// <summary>
        /// click select time in every view
        /// </summary>
        [Test]
        public void SelectTime_UsingClick_DefaultMode_CheckCloseWhenFinished()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // check if correct view (Hour)
            comp.FindAll("div.mud-time-picker-hour").Count.Should().Be(1);
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click on 13 (hour)
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[0].Click();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);

            // check if the view changed to minutes
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            comp.FindAll("div.mud-time-picker-minute").Count.Should().Be(1);

            // click on 37 (minute)
            comp.FindAll("div.mud-minute")[37].Click();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            // check correct time
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);
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
        public void SelectTime_Close_CheckTime()
        {
            var comp = OpenPicker(Parameter("AmPm", true));
            var picker = comp.Instance;
            // select 11 hours on outer dial and 30 mins
            comp.FindAll("div.mud-hour")[10].Click();
            comp.FindAll("div.mud-minute")[30].Click();
            comp.Find("div.mud-overlay").Click();
            picker.Time.Value.Hours.Should().Be(11);
            picker.Time.Value.Minutes.Should().Be(30);
        }

        [Test]
        public void SelectTimeStatic_UsingClicks_CheckTime()
        {
            var comp = Context.RenderComponent<MudTimePicker>(Parameter(nameof(MudTimePicker.PickerVariant), PickerVariant.Static));
            var picker = comp.Instance;
            // select 16 hours on outer dial and 30 mins
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click();
            picker.Time.Value.Hours.Should().Be(16);
            picker.Time.Value.Minutes.Should().Be(0);
            // select 30 minutes
            comp.FindAll("div.mud-minute")[30].Click();
            picker.Time.Value.Hours.Should().Be(16);
            picker.Time.Value.Minutes.Should().Be(30);
        }

        [Test]
        public async Task CheckAutoCloseTimePickerTest()
        {
            // Get access to the timepicker of the instance
            var comp = Context.RenderComponent<AutoCompleteTimePickerTest>();
            var timePicker = comp.FindComponent<MudTimePicker>();

            // Open the timepicker
            await comp.InvokeAsync(timePicker.Instance.OpenAsync);
            var picker = timePicker.Instance;

            // Select 16 hours
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(16);

            // Select 30 minutes
            comp.FindAll("div.mud-minute")[30].Click();
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);

            // Click outside of the timepicker
            comp.Find("div.mud-overlay").Click();

            // Check that the time should remain the same because autoclose is false
            // and there are actions which are defined
            picker.Time.Should().Be(new TimeSpan(00, 45, 00));

            await comp.InvokeAsync(timePicker.Instance.OpenAsync);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => timePicker.Instance.ClearAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));
            await comp.InvokeAsync(() => timePicker.Instance.CloseAsync(false));
            // Change the value of autoclose
            timePicker.Instance.AutoClose = true;

            // Open the timepicker
            await comp.InvokeAsync(timePicker.Instance.OpenAsync);
            picker = timePicker.Instance;

            // Select 16 hours
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(16);

            // Select 30 minutes
            comp.FindAll("div.mud-minute")[30].Click();
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);

            // Click outside of the timepicker
            comp.Find("div.mud-overlay").Click();

            // Check that the time should be equal to the selection this time!
            picker.Time.Should().Be(new TimeSpan(16, 30, 00));

            await comp.InvokeAsync(timePicker.Instance.OpenAsync);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(1));

            await comp.InvokeAsync(() => timePicker.Instance.ClearAsync());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover-open").Count.Should().Be(0));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));
        }

        [Test]
        public async Task CheckReadOnlyTest()
        {
            // Get access to the timepicker of the instance
            var comp = Context.RenderComponent<SimpleTimePickerTest>();
            var picker = comp.FindComponent<MudTimePicker>().Instance;
            comp.WaitForAssertion(() => picker.Time.Should().Be(null));
            // Open the timepicker
            await comp.InvokeAsync(picker.OpenAsync);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));

            // Select 16 hours
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click());
            picker.TimeIntermediate.Value.Hours.Should().Be(16);

            // Select 30 minutes
            await comp.InvokeAsync(() => comp.FindAll("div.mud-minute")[30].Click());
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);

            // Click outside of the timepicker
            await comp.InvokeAsync(() => comp.Find("div.mud-overlay").Click());

            // Check that the time have been changed
            comp.WaitForAssertion(() => picker.Time.Should().Be(new TimeSpan(16, 30, 00)));

            // Changer the readonly to true
            await comp.InvokeAsync(() => picker.ReadOnly = true);

            // Open the timepicker
            await comp.InvokeAsync(picker.OpenAsync);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));

            // Select 17 hours
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-stick-outer.mud-hour")[4].Click());
            comp.WaitForAssertion(() => picker.TimeIntermediate.Value.Hours.Should().Be(17));

            // Select 31 minutes
            await comp.InvokeAsync(() => comp.FindAll("div.mud-minute")[34].Click());
            comp.WaitForAssertion(() => picker.TimeIntermediate.Value.Minutes.Should().Be(34));

            // Click outside of the timepicker
            await comp.InvokeAsync(() => comp.Find("div.mud-overlay").Click());

            // Check that the time have not been changed
            comp.WaitForAssertion(() => picker.Time.Should().Be(new TimeSpan(16, 30, 00)));
        }

        [Test]
        public async Task TimePickerTest_KeyboardNavigation()
        {
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
        }

        [Test]
        public async Task CheckCallOneTimeChanged()
        {
            // Get access to the timepicker of the instance
            var comp = Context.RenderComponent<SimpleTimePickerTest>();
            var picker = comp.FindComponent<MudTimePicker>().Instance;
            comp.WaitForAssertion(() => picker.Time.Should().Be(null));
            // Open the timepicker
            await comp.InvokeAsync(picker.OpenAsync);
            comp.WaitForAssertion(() => comp.FindAll("div.mud-popover").Count.Should().Be(1));

            var count = 0;
            picker.TimeChanged = new EventCallbackFactory().Create<TimeSpan?>(this, v =>
            {
                count++;
            });

            // Select 16 hours
            await comp.InvokeAsync(() => comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click(new PointerEventArgs()));
            picker.TimeIntermediate.Value.Hours.Should().Be(16);

            // Select 30 minutes
            await comp.InvokeAsync(() => comp.FindAll("div.mud-minute")[30].Click(new PointerEventArgs()));
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);

            count.Should().Be(1);
            // Click outside of the timepicker
            await comp.InvokeAsync(() => comp.Find("div.mud-overlay").Click());

            // Check that the time have been changed
            comp.WaitForAssertion(() => picker.Time.Should().Be(new TimeSpan(16, 30, 00)));
        }

        // See #7483 for details
        [Test]
        [TestCase(12, 16, 3, 17, 4)]
        [TestCase(17, 16, 3, 17, 4, Description = "Switching back to original hour")]
        public void Selecting_hour_multiple_times_should_immediately_update_displayed_hour(
            int initialHour,
            int nextHour,
            int nextHourIndex,
            int finalHour,
            int finalHourIndex)
        {
            var comp = OpenPicker(Parameter(nameof(MudTimePicker.Time), TimeSpan.FromHours(initialHour)));
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // click on the hour input
            comp.FindAll("button.mud-timepicker-button")[0].Click();
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[nextHourIndex].PointerUp();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[nextHourIndex].Click();
            comp.FindAll("div.mud-timepicker-hourminute button.mud-timepicker-button span.mud-button-label")[0].TextContent.Should().Be(nextHour.ToString());
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(nextHour);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
            // are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // return to hour input
            comp.FindAll("button.mud-timepicker-button")[0].Click();
            // are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[finalHourIndex].PointerUp();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[finalHourIndex].Click();
            // ensure displayed hour text is has updated
            comp.FindAll("div.mud-timepicker-hourminute button.mud-timepicker-button span.mud-button-label")[0].TextContent.Should().Be(finalHour.ToString());
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(finalHour);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
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
