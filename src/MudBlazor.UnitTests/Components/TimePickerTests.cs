#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
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
            return OpenPicker(new ComponentParameter[] { parameter });
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
            Console.Write(comp.Markup);

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
            await comp.InvokeAsync(() => picker.Open());

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
            Console.Write(comp.Markup);
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
        public void DragMouse_SelectHour_CheckMinutesAppear()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // click on the minutes input
            comp.Find("div.mud-time-picker-hour").MouseDown();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].MouseOver();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(16);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[5].MouseOver();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[5].MouseUp();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(18);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(0);
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void DragMouse_SelectMinutes()
        {
            // Use bare component
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Minutes));
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // Any hours displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            comp.Find("div.mud-time-picker-minute").MouseDown();
            comp.FindAll("div.mud-minute")[3].MouseOver();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(3);
            comp.FindAll("div.mud-minute")[31].MouseOver();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(31);
            comp.FindAll("div.mud-minute")[5].MouseOver();
            comp.FindAll("div.mud-minute")[5].MouseUp();
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
            // invalid time
            comp.Find("input").Change("25:06");
            picker.TimeIntermediate.Should().BeNull();
            // invalid time
            comp.Find("input").Change("");
            picker.TimeIntermediate.Should().BeNull();
        }

        [Test]
        public void DragAndClick_AllHours12h_TestCoverage()
        {
            var comp = OpenPicker(new ComponentParameter[] { Parameter("OpenTo", OpenTo.Hours), Parameter("AmPm", true) });
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            Console.Write(comp.Markup);
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (var i = 0; i < 12; i++)
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-hour")[i].MouseOver();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-hour")[i].MouseUp();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-hour")[i].Click();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 1);
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
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-picker-stick-outer.mud-hour")[i].MouseOver();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 13 == 24 ? 0 : i + 13);
                comp.FindAll("div.mud-picker-stick-outer.mud-hour")[i].MouseUp();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 13 == 24 ? 0 : i + 13);
                comp.FindAll("div.mud-picker-stick-outer.mud-hour")[i].Click();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 13 == 24 ? 0 : i + 13);
            }
            // click and drag 1 to 12 on inner dial
            for (var i = 0; i < 12; i++)
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-picker-stick-inner.mud-hour")[i].MouseOver();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-picker-stick-inner.mud-hour")[i].MouseUp();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-picker-stick-inner.mud-hour")[i].Click();
                underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(i + 1);
            }
        }

        /// <summary>
        /// drag and mouseup on minutes for test coverage
        /// </summary>
        [Test]
        public void DragAndMouseUp_AllMinutes()
        {
            var comp = OpenPicker(Parameter("TimeEditMode", TimeEditMode.OnlyMinutes));
            var picker = comp.Instance;
            var underlyingPicker = comp.FindComponent<MudTimePicker>().Instance;

            // Any minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);

            // click and drag (hold mouse down)
            comp.Find("div.mud-time-picker-minute").MouseDown();
            for (var i = 0; i < 60; i++)
            {
                comp.FindAll("div.mud-minute")[i].MouseOver();
                underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(i);

                if (i == 59)
                {
                    comp.FindAll("div.mud-minute")[i].MouseUp();
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
            var comp = Context.RenderComponent<MudTimePicker>(new ComponentParameter[] { Parameter("TimeEditMode", TimeEditMode.OnlyMinutes), Parameter("PickerVariant", PickerVariant.Static), Parameter("OpenTo", OpenTo.Minutes) });
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
        /// drag and mouseup in every view
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

            // drag to 13 and mouse up
            comp.Find("div.mud-time-picker-hour").MouseDown();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[0].MouseOver();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[0].MouseUp();
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);

            // check if the view changed to minutes
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            comp.FindAll("div.mud-time-picker-minute").Count.Should().Be(1);

            // drag to 37 minutes and mouse up
            comp.Find("div.mud-time-picker-minute").MouseDown();
            comp.FindAll("div.mud-minute")[37].MouseOver();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);
            comp.FindAll("div.mud-minute")[37].MouseUp();
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);

            // check if closed
            comp.WaitForAssertion(() => comp.FindAll("div.mud-picker-open").Count.Should().Be(0));

            // check correct time
            underlyingPicker.TimeIntermediate.Value.Hours.Should().Be(13);
            underlyingPicker.TimeIntermediate.Value.Minutes.Should().Be(37);
            Console.WriteLine($"{underlyingPicker.TimeIntermediate.Value.Hours}:{underlyingPicker.TimeIntermediate.Value.Minutes}");
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
            Console.WriteLine($"{underlyingPicker.TimeIntermediate.Value.Hours}:{underlyingPicker.TimeIntermediate.Value.Minutes}");
        }

        [Test]
        public async Task Open_Programmatically_CheckOpen_Close_Programmatically_CheckClosed()
        {
            var comp = Context.RenderComponent<SimpleTimePickerTest>();
            Console.WriteLine(comp.Markup + "\n");
            comp.FindAll("div.mud-picker-content").Count.Should().Be(0);
            // clicking the button should open the picker
            await comp.Instance.Open();
            Console.WriteLine(comp.Markup);
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
            Console.Write(comp.Markup);
        }

        [Test]
        public async Task CheckAutoCloseTimePickerTest()
        {
            // Get access to the timepicker of the instance
            var comp = Context.RenderComponent<AutoCompleteTimePickerTest>();
            var timePicker = comp.FindComponent<MudTimePicker>();

            // Open the timepicker
            await comp.InvokeAsync(() => timePicker.Instance.Open());
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

            // Change the value of autoclose
            timePicker.Instance.AutoClose = true;

            // Open the timepicker
            await comp.InvokeAsync(() => timePicker.Instance.Open());
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
        }

        public async Task CheckReadOnlyTest()
        {
            // Get access to the timepicker of the instance
            var comp = Context.RenderComponent<MudTimePicker>();
            var picker = comp.Instance;

            // Open the timepicker
            await comp.InvokeAsync(() => picker.Open());

            // Select 16 hours
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[3].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(16);

            // Select 30 minutes
            comp.FindAll("div.mud-minute")[30].Click();
            picker.TimeIntermediate.Value.Minutes.Should().Be(30);

            // Click outside of the timepicker
            comp.Find("div.mud-overlay").Click();

            // Check that the time have been changed
            picker.Time.Should().Be(new TimeSpan(16, 30, 00));

            // Changer the readonly to true
            picker.ReadOnly = true;

            // Open the timepicker
            await comp.InvokeAsync(() => picker.Open());

            // Select 17 hours
            comp.FindAll("div.mud-picker-stick-outer.mud-hour")[4].Click();
            picker.TimeIntermediate.Value.Hours.Should().Be(17);

            // Select 31 minutes
            comp.FindAll("div.mud-minute")[34].Click();
            picker.TimeIntermediate.Value.Minutes.Should().Be(34);

            // Click outside of the timepicker
            comp.Find("div.mud-overlay").Click();

            // Check that the time have not been changed
            picker.Time.Should().Be(new TimeSpan(16, 30, 00));
        }
    }
}
