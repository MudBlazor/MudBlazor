using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CarouselTests : BunitTest
    {
        /// <summary>
        /// Default Carousel, with three pages.
        /// Testing if selection is sync with move commands.
        /// </summary>
        [Test]
        public async Task CarouselTest1()
        {
            var comp = Context.RenderComponent<CarouselTest>();
            // print the generated html
            //// select elements needed for the test
            var carousel = comp.FindComponent<MudCarousel<object>>().Instance;
            //// validating some renders
            carousel.Should().NotBeNull();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-carousel-item").Count.Should().Be(1));
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
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            await comp.InvokeAsync(() => carousel.SelectedIndex = 0);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
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
            var comp = Context.RenderComponent<MudCarousel<object>>();
            // print the generated html
            comp.FindAll("button.mud-icon-button").Count.Should().Be(2); //left + right
            // adding some pages
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Render();
            // playing with params
            comp.FindAll("button.mud-icon-button").Count.Should().Be(5); //left + right + 3 items
            comp.SetParam(p => p.ShowArrows, false);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(3);
            comp.SetParam(p => p.ShowBullets, false);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(0);
            comp.SetParam(p => p.ShowArrows, true);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(2);
            comp.SetParam(p => p.ShowBullets, true);
            comp.FindAll("button.mud-icon-button").Count.Should().Be(5);
            // Custom classes for navigation elements
            comp.SetParam(p => p.BulletsClass, "fake-delimiter-class");
            comp.SetParam(p => p.NavigationButtonsClass, "fake-navigation-class");
            comp.FindAll("button.fake-delimiter-class").Count.Should().Be(3);
            comp.FindAll("button.fake-navigation-class").Count.Should().Be(2);
            comp.SetParam(p => p.BulletsClass, null);
            comp.SetParam(p => p.NavigationButtonsClass, null);
            comp.FindAll("button.fake-delimiter-class").Count.Should().Be(0);
            comp.FindAll("button.fake-navigation-class").Count.Should().Be(0);
        }

        /// <summary>
        /// Testing autoCycle
        /// </summary>
        [Test]
        public async Task CarouselTest_AutoCycle()
        {
            var comp = Context.RenderComponent<MudCarousel<object>>();
            // print the generated html
            // adding some pages
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());

            comp.SetParam(p => p.AutoCycle, true);
            await comp.InvokeAsync(() => comp.Instance.MoveTo(0));
            comp.Render();
            //// playing with autoCycle
            for (var interval = 150; interval <= 300; interval += 150)
            {
                comp.SetParam(p => p.AutoCycleTime, TimeSpan.FromMilliseconds(interval));
                await Task.Delay(interval);
                comp.WaitForAssertion(() => comp.Instance.SelectedIndex.Should().Be(1), TimeSpan.FromMilliseconds(3000));
                comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[1]);
                await Task.Delay(interval);
                comp.WaitForAssertion(() => comp.Instance.SelectedIndex.Should().Be(2), TimeSpan.FromMilliseconds(3000));
                comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[2]);
                await Task.Delay(interval);
                comp.WaitForAssertion(() => comp.Instance.SelectedIndex.Should().Be(0), TimeSpan.FromMilliseconds(3000));
                comp.Instance.SelectedContainer.Should().Be(comp.Instance.Items[0]);
            }
        }

        /// <summary>
        /// Testing Transition With SelectedIndex
        /// </summary>
        [Test]
        public void CarouselTest_SelectedIndexTransition()
        {
            var comp = Context.RenderComponent<CarouselTest>();

            // No change
            comp.Instance.SelectedIndex = 0;
            comp.Render();
            comp.Find(".mud-carousel-transition-slide-next-enter.fake-class-item1");
            comp.FindAll(".mud-carousel-item").Should().HaveCount(1);

            // Slide next
            comp.Instance.SelectedIndex = 2;
            comp.Render();
            comp.Find(".mud-carousel-transition-slide-next-exit.fake-class-item1");
            comp.Find(".mud-carousel-transition-slide-next-enter.fake-class-item3");

            // Slide prev
            comp.Instance.SelectedIndex = 0;
            comp.Render();
            comp.Find(".mud-carousel-transition-slide-prev-exit.fake-class-item3");
            comp.Find(".mud-carousel-transition-slide-prev-enter.fake-class-item1");

        }

        /// <summary>
        /// Testing when DisableSwipeGesture
        /// </summary>
        [Test]
        public async Task CarouselTest_DisableSwipeGesture()
        {
            var comp = Context.RenderComponent<MudCarousel<object>>();

            //Add some pages
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());
            comp.Instance.Items.Add(new());

            //Move the SelectedIndex from -1 to 0
            await comp.InvokeAsync(() => comp.Instance.MoveTo(0));

            var mudSwipeArea = comp.FindComponent<MudSwipeArea>().Instance;

            var initialTouchPoints = new TouchPoint[]
            {
                new() {ClientX = 200, ClientY = 0},
            };
            var touchPoints = new TouchPoint[]
            {
                new() {ClientX = 100, ClientY = 0},
            };

#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            comp.Instance.EnableSwipeGesture = false;
            await comp.InvokeAsync(() => mudSwipeArea.OnTouchStart(new TouchEventArgs() { Touches = initialTouchPoints }));
            await comp.InvokeAsync(async () => await mudSwipeArea.OnTouchEnd(new TouchEventArgs() { ChangedTouches = touchPoints }));
            comp.Instance.SelectedIndex.Should().Be(0);

            comp.Instance.EnableSwipeGesture = true;
            await comp.InvokeAsync(() => mudSwipeArea.OnTouchStart(new TouchEventArgs() { Touches = initialTouchPoints }));
            await comp.InvokeAsync(async () => await mudSwipeArea.OnTouchEnd(new TouchEventArgs() { ChangedTouches = touchPoints }));
            comp.Instance.SelectedIndex.Should().Be(1);
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
        }

        /// <summary>
        /// Testing DataBinding with Add and Remove from data source (MVVM, MVC and another patterns)
        /// </summary>
        /// <returns></returns>
        [Test]
        public void CarouselTest_DataBinding()
        {
            var comp = Context.RenderComponent<CarouselBindingTest>();
            // print the generated html
            //// select elements needed for the test
            var carousel = comp.FindComponent<MudCarousel<string>>().Instance;
            //// validating some renders
            carousel.Should().NotBeNull();
            carousel.MoveTo(0);
            //// working with ItemsSource
            var source = carousel.ItemsSource;
            source.Count().Should().Be(5);
            carousel.Items.Count.Should().Be(5);
            carousel.SelectedIndex.Should().Be(0);
            //// adding item
            ((IList<string>)source).Add("Item added by hand");
            source.Count().Should().Be(6);
            carousel.Items.Count.Should().Be(5); // should call StateHasChanged() or Task.Delay(1)
            comp.Render();
            carousel.Items.Count.Should().Be(6);
            //// removing item
            ((IList<string>)source).RemoveAt(source.Count() - 1);
            source.Count().Should().Be(5);
            carousel.Items.Count.Should().Be(6); // should call StateHasChanged() or Task.Delay(1)
            comp.Render();
            carousel.Items.Count.Should().Be(5);
        }

    }
}
