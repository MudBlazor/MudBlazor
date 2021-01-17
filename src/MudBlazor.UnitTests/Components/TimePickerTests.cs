#pragma warning disable IDE1006 // leading underscore

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class TimePickerTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();


        public IRenderedComponent<MudTimePicker> OpenPicker(ComponentParameter parameter)
        {
            return OpenPicker(new ComponentParameter[] { parameter });
        }

        public IRenderedComponent<MudTimePicker> OpenPicker(ComponentParameter[] parameters = null)
        {
            IRenderedComponent<MudTimePicker> comp;
            if (parameters is null)
            {
                comp = ctx.RenderComponent<MudTimePicker>();
            }
            else
            {
                comp = ctx.RenderComponent<MudTimePicker>(parameters);
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
        public async Task Change_24hrsTo12Hours_CheckHours()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            // count hours
            picker.AmPm.Should().Be(false);
            comp.FindAll("div.mud-hour").Count.Should().Be(24);
            // change to 12 hour
            await comp.InvokeAsync(() => picker.AmPm = true);
            comp.Render();
            // count hours
            comp.Instance.AmPm.Should().Be(true);
            comp.FindAll("div.mud-hour").Count.Should().Be(12);
        }

        [Test]
        public void SelectTime_UsingClicks_24HourMode_CheckTime()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            // select 16 hours on outer dial and 30 mins
            comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(3).First().Click();
            picker.Time.Value.Hours.Should().Be(16);
            picker.Time.Value.Minutes.Should().Be(0);
            // select 30 minutes
            comp.FindAll("div.mud-minute").Skip(30).First().Click();
            picker.Time.Value.Hours.Should().Be(16);
            picker.Time.Value.Minutes.Should().Be(30);
            Console.Write(comp.Markup);
            // click 04 hours on the inner dial and 21 mins
            comp.FindAll("button.mud-timepicker-button").First().Click();
            comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(3).First().Click();
            comp.FindAll("div.mud-minute").Skip(21).First().Click();
            picker.Time.Value.Hours.Should().Be(4);
            picker.Time.Value.Minutes.Should().Be(21);
            // click 10 hours on the inner dial and 56 mins
            comp.FindAll("button.mud-timepicker-button").First().Click();
            comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(9).First().Click();
            comp.FindAll("div.mud-minute").Skip(56).First().Click();
            picker.Time.Value.Hours.Should().Be(10);
            picker.Time.Value.Minutes.Should().Be(56);
        }

        [Test]
        public void SelectTime_UsingClicks_12HourMode_CheckTime()
        {
            var comp = OpenPicker(Parameter("AmPm", true));
            var picker = comp.Instance;
            // select 11 hours on outer dial and 30 mins
            comp.FindAll("div.mud-hour").Skip(10).First().Click();
            comp.FindAll("div.mud-minute").Skip(30).First().Click();
            picker.Time.Value.Hours.Should().Be(11);
            picker.Time.Value.Minutes.Should().Be(30);
            Console.Write(comp.Markup);
            // click 04 hours on the inner dial and 21 mins
            comp.FindAll("button.mud-timepicker-button").First().Click();
            comp.FindAll("div.mud-hour").Skip(11).First().Click();
            comp.FindAll("div.mud-minute").Skip(21).First().Click();
            picker.Time.Value.Hours.Should().Be(0);
            picker.Time.Value.Minutes.Should().Be(21);
            // click 10 hours on the inner dial and 56 mins
            comp.FindAll("button.mud-timepicker-button").First().Click();
            comp.FindAll("div.mud-hour").Skip(9).First().Click();
            comp.FindAll("div.mud-minute").Skip(56).First().Click();
            picker.Time.Value.Hours.Should().Be(10);
            picker.Time.Value.Minutes.Should().Be(56);
            // click pm button
            comp.FindAll("button.mud-timepicker-button").Skip(3).First().Click();
            picker.Time.Value.Hours.Should().Be(22);
            picker.Time.Value.Minutes.Should().Be(56);
        }

        [Test]
        public void SelectTime_UsingClicks_12HourMode_ChangeAmPm_CheckTime()
        {
            var comp = OpenPicker(Parameter("AmPm", true));
            var picker = comp.Instance;
            // select 11 hours on outer dial and 30 mins
            comp.FindAll("div.mud-hour").Skip(10).First().Click();
            comp.FindAll("div.mud-minute").Skip(30).First().Click();
            picker.Time.Value.Hours.Should().Be(11);
            picker.Time.Value.Minutes.Should().Be(30);
            // click pm button
            comp.FindAll("button.mud-timepicker-button").Skip(3).First().Click();
            picker.Time.Value.Hours.Should().Be(23);
            picker.Time.Value.Minutes.Should().Be(30);
            // add 12 hours in pm mode
            comp.FindAll("div.mud-hour").Skip(10).First().Click();
            picker.Time.Value.Hours.Should().Be(23);
            // click am button shoulkd subtract 12 hours
            comp.FindAll("button.mud-timepicker-button").Skip(2).First().Click();
            picker.Time.Value.Hours.Should().Be(11);
            picker.Time.Value.Minutes.Should().Be(30);
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
            comp.FindAll("button.mud-timepicker-button").Skip(1).First().Click();
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            comp.FindAll("input").First().Click();
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
        public void ChangeToMinutes_FromHours_CheckHoursHidden()
        {
            var comp = OpenPicker();
            // click on the minutes input
            comp.FindAll("button.mud-timepicker-button").Skip(1).First().Click();
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }


        [Test]
        public void DragMouse_SelectHour_CheckMinutesAppear()
        {
            var comp = OpenPicker();
            var picker = comp.Instance;
            // click on the minutes input
            comp.Find("div.mud-time-picker-hour").MouseDown();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(3).First().MouseOver();
            picker.Time.Value.Hours.Should().Be(16);
            picker.Time.Value.Minutes.Should().Be(0);
            comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(5).First().MouseOver();
            comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(5).First().MouseUp();
            picker.Time.Value.Hours.Should().Be(18);
            picker.Time.Value.Minutes.Should().Be(0);
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        [Test]
        public void DragMouse_SelectMinutes()
        {
            // Use bare component
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Minutes));
            var picker = comp.Instance;
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            comp.Find("div.mud-time-picker-minute").MouseDown();
            comp.FindAll("div.mud-minute").Skip(3).First().MouseOver();
            picker.Time.Value.Minutes.Should().Be(3);
            comp.FindAll("div.mud-minute").Skip(31).First().MouseOver();
            picker.Time.Value.Minutes.Should().Be(31);
            comp.FindAll("div.mud-minute").Skip(5).First().MouseOver();
            comp.FindAll("div.mud-minute").Skip(5).First().MouseUp();
            picker.Time.Value.Minutes.Should().Be(5);
        }

        [Test]
        public void InputStringValues_CheckParsing()
        {
            var comp = ctx.RenderComponent<MudTimePicker>();
            var picker = comp.Instance;
            // valid time
            comp.Find("input").Change("23:02");
            picker.Time.Should().Be(new TimeSpan(23, 2, 0));
            // invalid time
            comp.Find("input").Change("25:06");
            picker.Time.Should().BeNull();
            // invalid time
            comp.Find("input").Change("");
            picker.Time.Should().BeNull();
        }

        [Test]
        public void DragAndClick_AllHours12h_TestCoverage()
        {
            var comp = OpenPicker(new ComponentParameter[] { Parameter("OpenTo", OpenTo.Hours), Parameter("AmPm", true) });
            var picker = comp.Instance;
            Console.Write(comp.Markup);
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (var i = 0; i < 12; i++)
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-hour").Skip(i).First().MouseOver();
                picker.Time.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-hour").Skip(i).First().MouseUp();
                picker.Time.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-hour").Skip(i).First().Click();
                picker.Time.Value.Hours.Should().Be(i + 1);
            }
        }

        [Test]
        public void DragAndClick_AllHours24h_TestCoverage()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Hours));
            var picker = comp.Instance;
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag 13 to 00 on outer dial
            for (var i = 0; i < 12; i++)
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(i).First().MouseOver();
                picker.Time.Value.Hours.Should().Be(i + 13 == 24 ? 0 : i + 13);
                comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(i).First().MouseUp();
                picker.Time.Value.Hours.Should().Be(i + 13 == 24 ? 0 : i + 13);
                comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(i).First().Click();
                picker.Time.Value.Hours.Should().Be(i + 13 == 24 ? 0 : i + 13);
            }
            // click and drag 1 to 12 on inner dial
            for (var i = 0; i < 12; i++)
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(i).First().MouseOver();
                picker.Time.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(i).First().MouseUp();
                picker.Time.Value.Hours.Should().Be(i + 1);
                comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(i).First().Click();
                picker.Time.Value.Hours.Should().Be(i + 1);
            }
        }

        /// <summary>
        /// drag and click minutes for test coverage
        /// </summary>
        [Test]
        public void DragAndClick_AllMinutes()
        {
            var comp = OpenPicker(Parameter("OpenTo", OpenTo.Minutes));
            var picker = comp.Instance;
            // Any minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (var i = 0; i < 60; i++)
            {
                comp.Find("div.mud-time-picker-minute").MouseDown();
                comp.FindAll("div.mud-minute").Skip(i).First().MouseOver();
                picker.Time.Value.Minutes.Should().Be(i);
                comp.FindAll("div.mud-minute").Skip(i).First().MouseUp();
                picker.Time.Value.Minutes.Should().Be(i);
                comp.FindAll("div.mud-minute").Skip(i).First().Click();
                picker.Time.Value.Minutes.Should().Be(i);
            }
        }
    }
}
