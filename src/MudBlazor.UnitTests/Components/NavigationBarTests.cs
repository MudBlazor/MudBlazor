// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

#nullable enable

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavigationBarTests : BunitTest
    {
        [Test]
        public void NavigationBar_InitialStateText()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "a"));
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "b");
                    item.Add(x => x.Disabled, true);
                });
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "c"));
            });
            var items = comp.FindComponents<MudNavigationBarItem>();
            comp.Instance.Should().NotBeNull();
            comp.Instance.Hover.Should().BeTrue();
            items[0].Instance.BadgeParameters.Should().BeOfType<NavigationBarBadgeParameters>();
            items[1].Instance.Text.Should().Be("b");
            items[1].Markup.Should().Contain("mud-disabled");
            items[2].Markup.Should().NotContain("mud-disabled");
        }

        [Test]
        public void NavigationBar_ClickTest()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "a"));
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "b");
                    item.Add(x => x.Disabled, true);
                });
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "c"));
            });
            var items = comp.FindComponents<MudNavigationBarItem>();
            var clickableItems = comp.FindAll("div.mud-nav-bar-item");
            items[0].Instance._isSelected.Should().BeFalse();
            clickableItems[0].Click();
            items[0].Instance._isSelected.Should().BeTrue();
            clickableItems[1].Click();
            items[0].Instance._isSelected.Should().BeTrue();
            items[1].Instance._isSelected.Should().BeFalse();
            clickableItems[2].Click();
            items[0].Instance._isSelected.Should().BeFalse();
            items[2].Instance._isSelected.Should().BeTrue();
        }

        [Test]
        public async Task NavigationBar_FocusandKeyTest()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "a"));
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "b");
                    item.Add(x => x.Disabled, true);
                });
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "c"));
            });
            var items = comp.FindComponents<MudNavigationBarItem>();
            items[0].Instance._isSelected.Should().BeFalse();
            await comp.InvokeAsync(() => items[0].Instance.FocusAsync());
            await comp.InvokeAsync(() => items[0].Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter" }));
            items[0].Instance._isSelected.Should().BeTrue();
            await comp.InvokeAsync(() => items[0].Instance.BlurAsync());
            await comp.InvokeAsync(() => items[0].Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "Enter" }));
            items[0].Instance._isSelected.Should().BeTrue();
            await comp.InvokeAsync(() => items[2].Instance.FocusAsync());
            await comp.InvokeAsync(() => items[2].Instance.HandleKeyDownAsync(new KeyboardEventArgs() { Key = "NumpadEnter" }));
            items[0].Instance._isSelected.Should().BeFalse();
            items[2].Instance._isSelected.Should().BeTrue();
        }

        /// <summary>
        /// Verifies that adjacent navigation is safe when no items exist.
        /// </summary>
        [Test]
        public async Task NavigateAdjacentAsync_WhenEmpty_DoesNotThrow()
        {
            var comp = Context.Render<MudNavigationBar>();

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));
            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(-1));

            comp.Instance.GetSelectedItemId().Should().BeNull();
        }

        /// <summary>
        /// Verifies that adjacent navigation skips disabled items.
        /// </summary>
        [Test]
        public async Task NavigateAdjacentAsync_WhenTargetDisabled_SkipsItem()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "First"));

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Disabled");
                    item.Add(x => x.Disabled, true);
                });

                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "Last"));
            });

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));

            comp.Instance.GetSelectedItemId().Should().Be("First");

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));

            comp.Instance.GetSelectedItemId().Should().Be("Last");

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(-1));

            comp.Instance.GetSelectedItemId().Should().Be("First");
        }

        /// <summary>
        /// Verifies that Space activates a navigation item without an Href.
        /// </summary>
        [Test]
        public void NavigationBarItem_WhenSpacePressed_SelectsItem()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "First"));

                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "Second"));
            });

            var items = comp.FindAll(".mud-nav-bar-item");

            items[1].KeyDown(new KeyboardEventArgs { Key = " " });

            comp.Instance.GetSelectedItemId().Should().Be("Second");
        }

        /// <summary>
        /// Verifies that ResetAsync clears the selected item.
        /// </summary>
        [Test]
        public async Task ResetAsync_WhenItemSelected_ClearsSelection()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "First"));
            });

            comp.Find(".mud-nav-bar-item").Click();

            comp.Instance.GetSelectedItemId().Should().Be("First");

            await comp.InvokeAsync(() => comp.Instance.ResetAsync());

            comp.Instance.GetSelectedItemId().Should().BeNull();
            comp.Find(".mud-nav-bar-item")
                .ClassList.Should().NotContain("mud-nav-bar-item-selected");
        }

        /// <summary>
        /// Verifies that selecting the same item twice does not repeat the callback.
        /// </summary>
        [Test]
        public void NavigationBarItem_WhenSelectedTwice_RaisesSingleSelectionChange()
        {
            var selections = new List<string?>();

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.Add(x => x.OnSelectionChange,
                    EventCallback.Factory.Create<string?>(
                        this, value => selections.Add(value)));

                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "First"));
            });

            var item = comp.Find(".mud-nav-bar-item");

            item.Click();
            item.Click();

            selections.Should().ContainSingle()
                .Which.Should().Be("First");
        }

        /// <summary>
        /// Verifies that adjacent navigation activates the target Href.
        /// </summary>
        [Test]
        public async Task NavigateAdjacentAsync_WhenTargetHasHref_Navigates()
        {
            var navigationManager =
                Context.Services.GetRequiredService<NavigationManager>();

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "First");
                    item.Add(x => x.Href, "first");
                });

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Second");
                    item.Add(x => x.Href, "second");
                });
            });

            comp.FindAll(".mud-nav-bar-item")[0].Click();

            navigationManager.Uri.Should().EndWith("/first");

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));

            navigationManager.Uri.Should().EndWith("/second");
        }

        /// <summary>
        /// Verifies that custom Href comparison receives a base-relative location.
        /// </summary>
        [Test]
        public void NavigationBar_WhenLocationChanges_UsesBaseRelativeLocation()
        {
            var navigationManager =
                Context.Services.GetRequiredService<NavigationManager>();

            var receivedLocations = new List<string?>();

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.Add(x => x.CustomHrefComparisonFunc,
                    (Func<string?, string?, bool>)((location, href) =>
                    {
                        receivedLocations.Add(location);
                        return location == href;
                    }));

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "First");
                    item.Add(x => x.Href, "first");
                });
            });

            navigationManager.NavigateTo("first");

            comp.WaitForAssertion(() =>
            {
                comp.Instance.GetSelectedItemId().Should().Be("First");
                receivedLocations.Should().Contain("first");
            });

            receivedLocations.Should().NotContain(location => location != null && location.StartsWith("http", StringComparison.OrdinalIgnoreCase));
        }
    }
}
