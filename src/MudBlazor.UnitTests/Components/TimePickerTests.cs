#pragma warning disable 1998

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
            ctx.AddMudBlazorServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// open and close the picker
        /// </summary>
        [Test]
        public void OpenCloseTest()
        {
            var comp = ctx.RenderComponent<MudTimePicker>();
            Console.WriteLine(comp.Markup);
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            Console.WriteLine(comp.Markup);
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // clicking outside to close
            comp.Find("div.mud-overlay").Click();
            // should not be open any more
            // should not be open any more
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
        }

        /// <summary>
        /// change from 24 to 12 hours and count rendered hours
        /// </summary>
        [Test]
        public async Task ChangeTo12Hours() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>();
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
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

        /// <summary>
        /// select the time in 24 hour mode
        /// click the hour and select a new time
        /// </summary>
        [Test]
        public async Task SelectTime24Hours() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>();
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
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

        /// <summary>
        /// select the time in 12 hour mode
        /// click the hour and select a new time
        /// </summary>
        [Test]
        public async Task SelectTime12Hours() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("AmPm", true));
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
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

        /// <summary>
        /// select the time in 12 hour mode
        /// am/pm buttons
        /// </summary>
        [Test]
        public async Task SelectTime12HoursAmPm() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("AmPm", true));
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
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

        /// <summary>
        /// opento hours
        /// </summary>
        [Test]
        public async Task OpenToHours() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Hours));
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Are hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        /// <summary>
        /// opento minutes
        /// </summary>
        [Test]
        public async Task OpenToMinutes() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Minutes));
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        /// <summary>
        /// change to minutes
        /// </summary>
        [Test]
        public async Task ChangeToMinutes() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>();
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // click on the minutes input
            comp.FindAll("button.mud-timepicker-button").Skip(1).First().Click();
            // Are minutes displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
        }

        /// <summary>
        /// drag hours
        /// </summary>
        [Test]
        public async Task DragMouseHour() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>();
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
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

        /// <summary>
        /// drag minutes
        /// </summary>
        [Test]
        public async Task DragMouseMinutes() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Minutes));
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
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

        /// <summary>
        /// test imput parsing
        /// </summary>
        [Test]
        public async Task SetValues() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Minutes));
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

        /// <summary>
        /// drag and click 12 hours for test coverage
        /// </summary>
        [Test]
        public async Task DragAndClickAllHours12h() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Hours),("AmPm", true));
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (int i = 0; i < 12; i++) 
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-hour").Skip(i).First().MouseOver();
                picker.Time.Value.Hours.Should().Be(i+1);
                comp.FindAll("div.mud-hour").Skip(i).First().MouseUp();
                picker.Time.Value.Hours.Should().Be(i+1);
                comp.FindAll("div.mud-hour").Skip(i).First().Click();
                picker.Time.Value.Hours.Should().Be(i+1);
            }
        }

        /// <summary>
        /// drag and click 24 hours for test coverage
        /// </summary>
        [Test]
        public async Task DragAndClickAllHours24h() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Hours));
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-minute.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag 13 to 00 on outer dial
            for (int i = 0; i < 12; i++) 
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(i).First().MouseOver();
                picker.Time.Value.Hours.Should().Be(i+13 == 24 ? 0 : i+13);
                comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(i).First().MouseUp();
                picker.Time.Value.Hours.Should().Be(i+13 == 24 ? 0 : i+13);
                comp.FindAll("div.mud-picker-stick-outer.mud-hour").Skip(i).First().Click();
                picker.Time.Value.Hours.Should().Be(i+13 == 24 ? 0 : i+13);
            }
            // click and drag 1 to 12 on inner dial
            for (int i = 0; i < 12; i++) 
            {
                comp.Find("div.mud-time-picker-hour").MouseDown();
                comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(i).First().MouseOver();
                picker.Time.Value.Hours.Should().Be(i+1);
                comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(i).First().MouseUp();
                picker.Time.Value.Hours.Should().Be(i+1);
                comp.FindAll("div.mud-picker-stick-inner.mud-hour").Skip(i).First().Click();
                picker.Time.Value.Hours.Should().Be(i+1);
            }
        }

        /// <summary>
        /// drag and click minutes for test coverage
        /// </summary>
        [Test]
        public async Task DragAndClickAllMinutes() 
        {
            // Use bare component
            var comp = ctx.RenderComponent<MudTimePicker>(("OpenTo", OpenTo.Minutes));
            var picker = comp.Instance;
            // should not be open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(0);
            // click to to open menu
            comp.Find("input").Click();
            // now its open
            comp.FindAll("div.mud-picker-open").Count.Should().Be(1);
            // Any hours displayed
            comp.FindAll("div.mud-time-picker-hour.mud-time-picker-dial-hidden").Count.Should().Be(1);
            // click and drag
            for (int i = 0; i < 60; i++) 
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