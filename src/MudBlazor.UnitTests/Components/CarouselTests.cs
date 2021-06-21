﻿#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Threading.Tasks;
using Bunit;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CarouselTests
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

        /// <summary>
        /// Default Carousel, with three pages.
        /// Testing if selection is sync with move commands.
        /// </summary>
        [Test]
        public async Task CarouselTest1()
        {
            var comp = ctx.RenderComponent<CarouselTest>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            //// select elements needed for the test
            var carousel = comp.FindComponent<MudCarousel<object>>().Instance;
            //// validating some renders
            carousel.Should().NotBeNull();
            comp.WaitForAssertion(()=>comp.FindAll("div.mud-carousel-item").Count.Should().Be(1));
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(0);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(0);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(5); //left + right + 3 pages
            carousel.LastContainer.Should().BeNull();
            //// changing current index from 0 to 1
            carousel.SelectedIndex.Should().Be(0);
            carousel.SelectedContainer.Should().Be(carousel.Items[0]);
            var last = carousel.SelectedContainer;
            await comp.InvokeAsync(() => carousel.Next());
            carousel.SelectedIndex.Should().Be(1);
            carousel.SelectedContainer.Should().Be(carousel.Items[1]);
            carousel.SelectedItem.Should().Be(carousel.Items[1]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1); //last item continues on DOM because it need's to act with transition effect
            comp.FindAll("div.fake-class-item2").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(0);
            //// changing current index from 1 to 0
            last = carousel.SelectedContainer;
            await comp.InvokeAsync(() => carousel.Previous());
            carousel.SelectedIndex.Should().Be(0);
            carousel.SelectedContainer.Should().Be(carousel.Items[0]);
            carousel.SelectedItem.Should().Be(carousel.Items[0]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(0);
            //// changing current index from 0 to 2 with MoveTo()
            last = carousel.SelectedContainer;
            await comp.InvokeAsync(() => carousel.MoveTo(2));
            carousel.SelectedIndex.Should().Be(2);
            carousel.SelectedContainer.Should().Be(carousel.Items[2]);
            carousel.SelectedItem.Should().Be(carousel.Items[2]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(0);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(1);
            //// changing current index from 2 to 0 with Next()
            last = carousel.SelectedContainer;
            await comp.InvokeAsync(() => carousel.Next());
            carousel.SelectedIndex.Should().Be(0);
            carousel.SelectedContainer.Should().Be(carousel.Items[0]);
            carousel.SelectedItem.Should().Be(carousel.Items[0]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(0);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(1);
            //// changing current index from 0 to 2 with Previous()
            last = carousel.SelectedContainer;
            await comp.InvokeAsync(() => carousel.Previous());
            carousel.SelectedIndex.Should().Be(2);
            carousel.SelectedContainer.Should().Be(carousel.Items[2]);
            carousel.SelectedItem.Should().Be(carousel.Items[2]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(0);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(1);
            //// changing current index from 1 to 2 with Next() - rendering test
            await comp.InvokeAsync(() => carousel.MoveTo(1)); //positioning only
            carousel.SelectedIndex.Should().Be(1);
            carousel.SelectedContainer.Should().Be(carousel.Items[1]);
            carousel.SelectedItem.Should().Be(carousel.Items[1]);
            await comp.InvokeAsync(() => carousel.Next()); //positioning only
            carousel.SelectedIndex.Should().Be(2);
            carousel.SelectedContainer.Should().Be(carousel.Items[2]);
            carousel.SelectedItem.Should().Be(carousel.Items[2]);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(0);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(1);
            //// Forcing SelectedIndex value by setter (for binding purposes)
            last = carousel.SelectedContainer;
            await comp.InvokeAsync(() => carousel.SelectedIndex = 0);
            carousel.SelectedIndex.Should().Be(0);
            carousel.SelectedContainer.Should().Be(carousel.Items[0]);
            carousel.SelectedItem.Should().Be(carousel.Items[0]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(0);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(1);
            ////Swipe from right to left
            last = carousel.SelectedContainer;
            var swipe = comp.Find("div.mud-carousel-swipe");
            TouchPoint[] _startPoints = { new() { ClientY = 0, ClientX = 150 } };
            swipe.TouchStart(0, _startPoints);
            TouchPoint[] _endPoints = { new() { ClientY = 0, ClientX = 20 } };
            swipe.TouchEnd(0, null, null, _endPoints);
            carousel.SelectedIndex.Should().Be(1);
            carousel.SelectedContainer.Should().Be(carousel.Items[1]);
            carousel.SelectedItem.Should().Be(carousel.Items[1]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(0);
            ////Swipe from left to right
            last = carousel.SelectedContainer;
            _startPoints[0].ClientX = 20;
            swipe.TouchStart(0, _startPoints);
            _endPoints[0].ClientX = 150;
            swipe.TouchEnd(0, null, null, _endPoints);
            carousel.SelectedIndex.Should().Be(0);
            carousel.SelectedContainer.Should().Be(carousel.Items[0]);
            carousel.SelectedItem.Should().Be(carousel.Items[0]);
            carousel.LastContainer.Should().Be(last);
            comp.FindAll("div.fake-class-item1").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item2").Count.Should().Be(1);
            comp.FindAll("div.fake-class-item3").Count.Should().Be(0);
        }

        /// <summary>
        /// Testing some parameters
        /// </summary>
        [Test]
        public void CarouselTest_RenderingOptions()
        {
            var comp = ctx.RenderComponent<MudCarousel<object>>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(2); //left + right
            /// adding some pages
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Render();
            //// playing with params
            comp.FindAll("button.mud-icon-button").Count.Should().Be(5); //left + right + 3 items
            comp.SetParam(p => p.ShowArrows, false);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(3);
            comp.SetParam(p => p.ShowDelimiters, false);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(0);
            comp.SetParam(p => p.ShowArrows, true);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(2);
            comp.SetParam(p => p.ShowDelimiters, true);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(5);
        }

        /// <summary>
        /// Testing autoCycle
        /// </summary>
        [Ignore("This test will not reliably run on the build server")]
        [Test]
        public async Task CarouselTest_AutoCycle()
        {
            int interval = 250;
            var comp = ctx.RenderComponent<MudCarousel<object>>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            /// adding some pages
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.SetParam(p => p.AutoCycleTime, TimeSpan.FromMilliseconds(interval));
            comp.SetParam(p => p.AutoCycle, true);
            await comp.InvokeAsync(() => comp.Instance.MoveTo(0));
            comp.Render();
            //// playing with autoCycle
            await Task.Delay(interval + 50); // 15ms just to ensure that transition runs before of Task.Delay() ends
            comp.Instance.SelectedIndex.Should().Be(1);
            comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[1]);
            await Task.Delay(interval + 50);
            comp.Instance.SelectedIndex.Should().Be(2);
            comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[2]);
            await Task.Delay(interval + 50);
            comp.Instance.SelectedIndex.Should().Be(0);
            comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[0]);
            ///changing internal
            interval = 150;
            comp.SetParam(p => p.AutoCycleTime, TimeSpan.FromMilliseconds(interval));
            await Task.Delay(interval + 50);
            comp.Instance.SelectedIndex.Should().Be(1);
            comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[1]);
            await Task.Delay(interval + 40);
            comp.Instance.SelectedIndex.Should().Be(2);
            comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[2]);
            await Task.Delay(interval + 40);
            comp.Instance.SelectedIndex.Should().Be(0);
            comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[0]);
        }


    }
}
