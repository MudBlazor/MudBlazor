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
        /// <summary>
        /// Verifies that navigation items render with their initial parameters.
        /// </summary>
        [Test]
        public void NavigationBar_WhenRendered_InitializesItems()
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

        /// <summary>
        /// Verifies that clicking enabled items updates the selection while disabled items are ignored.
        /// </summary>
        [Test]
        public async Task NavigationBarItem_WhenClicked_UpdatesSelection()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "a"));

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "b");
                    item.Add(x => x.Disabled, true);
                });

                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "c"));
            });

            comp.Instance.GetSelectedItemId().Should().BeNull();

            await comp.FindAll(".mud-nav-bar-item")[0].ClickAsync();

            comp.Instance.GetSelectedItemId().Should().Be("a");

            await comp.FindAll(".mud-nav-bar-item")[1].ClickAsync();

            comp.Instance.GetSelectedItemId().Should().Be("a");

            await comp.FindAll(".mud-nav-bar-item")[2].ClickAsync();

            comp.Instance.GetSelectedItemId().Should().Be("c");
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
                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "First"));

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Disabled");
                    item.Add(x => x.Disabled, true);
                });

                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "Last"));
            });

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));

            comp.Instance.GetSelectedItemId().Should().Be("First");

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));

            comp.Instance.GetSelectedItemId().Should().Be("Last");

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(-1));

            comp.Instance.GetSelectedItemId().Should().Be("First");
        }

        /// <summary>
        /// Verifies that items without an Href render as native buttons.
        /// </summary>
        [Test]
        public void NavigationBarItem_WhenNoHref_UsesNativeButton()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                    item.Add(x => x.Text, "First"));

                builder.AddChildContent<MudNavigationBarItem>(item =>
                    item.Add(x => x.Text, "Second"));
            });

            var buttons = comp.FindAll("button.mud-nav-bar-item");

            buttons.Should().HaveCount(2);

            foreach (var button in buttons)
            {
                button.GetAttribute("type").Should().Be("button");
                button.HasAttribute("disabled").Should().BeFalse();
                button.GetAttribute("tabindex").Should().Be("0");
            }
        }

        /// <summary>
        /// Verifies that ResetAsync clears the selected item.
        /// </summary>
        [Test]
        public async Task ResetAsync_WhenItemSelected_ClearsSelection()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                    item.Add(x => x.Text, "First"));
            });

            await comp.Find("button.mud-nav-bar-item").ClickAsync();

            comp.Instance.GetSelectedItemId().Should().Be("First");

            await comp.InvokeAsync(() => comp.Instance.ResetAsync());

            comp.Instance.GetSelectedItemId().Should().BeNull();

            comp.Find("button.mud-nav-bar-item").ClassList.Should().NotContain("mud-nav-bar-item-selected");
        }

        /// <summary>
        /// Verifies that selecting the same item twice raises the selection callback only once.
        /// </summary>
        [Test]
        public async Task NavigationBarItem_WhenSelectedTwice_RaisesSingleSelectionChange()
        {
            var selections = new List<string?>();

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.Add(x => x.OnSelectionChange,
                    EventCallback.Factory.Create<string?>(this, value => selections.Add(value)));

                builder.AddChildContent<MudNavigationBarItem>(item => item.Add(x => x.Text, "First"));
            });

            await comp.Find(".mud-nav-bar-item").ClickAsync();
            await comp.Find(".mud-nav-bar-item").ClickAsync();

            selections.Should().ContainSingle().Which.Should().Be("First");
        }

        /// <summary>
        /// Verifies that adjacent navigation follows the target item's Href.
        /// </summary>
        [Test]
        public async Task NavigateAdjacentAsync_WhenTargetHasHref_Navigates()
        {
            var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

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

            navigationManager.NavigateTo("first");

            comp.WaitForAssertion(() => comp.Instance.GetSelectedItemId().Should().Be("First"));

            await comp.InvokeAsync(() => comp.Instance.NavigateAdjacentAsync(1));

            navigationManager.Uri.Should().EndWith("/second");

            comp.WaitForAssertion(() => comp.Instance.GetSelectedItemId().Should().Be("Second"));
        }

        /// <summary>
        /// Verifies that custom Href comparison receives a base-relative location.
        /// </summary>
        [Test]
        public void NavigationBar_WhenLocationChanges_UsesBaseRelativeLocation()
        {
            var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

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

        /// <summary>
        /// Verifies that disabled buttons are not focusable or selectable.
        /// </summary>
        [Test]
        public async Task NavigationBarItem_WhenDisabled_IsNotKeyboardFocusable()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                    item.Add(x => x.Text, "Enabled"));

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Disabled");
                    item.Add(x => x.Disabled, true);
                });
            });

            var items = comp.FindAll("button.mud-nav-bar-item");

            items.Should().HaveCount(2);

            items[0].HasAttribute("disabled").Should().BeFalse();
            items[1].HasAttribute("disabled").Should().BeTrue();
            items[1].GetAttribute("tabindex").Should().Be("-1");

            await comp.FindAll("button.mud-nav-bar-item")[1].ClickAsync();

            comp.Instance.GetSelectedItemId().Should().BeNull();
        }

        /// <summary>
        /// Verifies that an Href without a fragment does not match a different path.
        /// </summary>
        [Test]
        public void NavigationBar_WhenHrefDoesNotMatch_DoesNotSelectItem()
        {
            var navigationManager = Context.Services.GetRequiredService<NavigationManager>();
            navigationManager.NavigateTo("other");

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "First");
                    item.Add(x => x.Href, "first");
                });
            });

            comp.Instance.GetSelectedItemId().Should().BeNull();

            navigationManager.NavigateTo("first");

            comp.WaitForAssertion(() => comp.Instance.GetSelectedItemId().Should().Be("First"));

            navigationManager.NavigateTo("other");

            comp.WaitForAssertion(() => comp.Instance.GetSelectedItemId().Should().BeNull());
        }

        /// <summary>
        /// Verifies that the current navigation destination is exposed to assistive technology.
        /// </summary>
        [Test]
        public void NavigationBarItem_WhenHrefSelected_ExposesAriaCurrentPage()
        {
            var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

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

            navigationManager.NavigateTo("first");

            comp.WaitForAssertion(() =>
            {
                var items = comp.FindAll(".mud-nav-bar-item");

                items[0].GetAttribute("aria-current").Should().Be("page");
                items[1].HasAttribute("aria-current").Should().BeFalse();
            });

            navigationManager.NavigateTo("second");

            comp.WaitForAssertion(() =>
            {
                var items = comp.FindAll(".mud-nav-bar-item");

                items[0].HasAttribute("aria-current").Should().BeFalse();
                items[1].GetAttribute("aria-current").Should().Be("page");
            });
        }

        /// <summary>
        /// Verifies that the selected button exposes its current state.
        /// </summary>
        [Test]
        public async Task NavigationBarItem_WhenButtonSelected_ExposesAriaCurrent()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "First"));

                builder.AddChildContent<MudNavigationBarItem>(
                    item => item.Add(x => x.Text, "Second"));
            });

            var items = comp.FindAll("button.mud-nav-bar-item");

            items[0].HasAttribute("aria-current").Should().BeFalse();
            items[1].HasAttribute("aria-current").Should().BeFalse();

            await comp.FindAll("button.mud-nav-bar-item")[0].ClickAsync();

            items = comp.FindAll("button.mud-nav-bar-item");

            items[0].GetAttribute("aria-current").Should().Be("true");
            items[1].HasAttribute("aria-current").Should().BeFalse();

            await comp.FindAll("button.mud-nav-bar-item")[1].ClickAsync();

            items = comp.FindAll("button.mud-nav-bar-item");

            items[0].HasAttribute("aria-current").Should().BeFalse();
            items[1].GetAttribute("aria-current").Should().Be("true");
        }

        /// <summary>
        /// Verifies that navigation items use native HTML semantics.
        /// </summary>
        [Test]
        public void NavigationBarItem_WhenRendered_UsesNativeElements()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Home");
                    item.Add(x => x.Href, "home");
                });

                builder.AddChildContent<MudNavigationBarItem>(item =>
                    item.Add(x => x.Text, "Action"));
            });

            comp.Find("a.mud-nav-bar-item")
                .GetAttribute("href").Should().Be("home");

            comp.Find("button.mud-nav-bar-item")
                .GetAttribute("type").Should().Be("button");
        }

        /// <summary>
        /// Verifies that disabled links cannot navigate or receive keyboard focus.
        /// </summary>
        [Test]
        public void NavigationBarItem_WhenLinkDisabled_RemovesHref()
        {
            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Home");
                    item.Add(x => x.Href, "home");
                    item.Add(x => x.Disabled, true);
                });
            });

            var link = comp.Find("a.mud-nav-bar-item");

            link.HasAttribute("href").Should().BeFalse();
            link.GetAttribute("aria-disabled").Should().Be("true");
            link.GetAttribute("tabindex").Should().Be("-1");
        }

        /// <summary>
        /// Verifies that the most specific matching route is selected.
        /// </summary>
        [Test]
        public void NavigationBar_WhenRoutesOverlap_SelectsMostSpecific()
        {
            var navigation = Context.Services
                .GetRequiredService<NavigationManager>();

            navigation.NavigateTo("settings/profile");

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Settings");
                    item.Add(x => x.Href, "settings");
                });

                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "Profile");
                    item.Add(x => x.Href, "settings/profile");
                });
            });

            comp.WaitForAssertion(() =>
            {
                comp.Instance.GetSelectedItemId()
                    .Should().Be("Profile");
            });

            navigation.NavigateTo("settings");

            comp.WaitForAssertion(() =>
            {
                comp.Instance.GetSelectedItemId()
                    .Should().Be("Settings");
            });
        }

        /// <summary>
        /// Verifies that changing an item's Href updates route selection.
        /// </summary>
        [Test]
        public async Task NavigationBarItem_WhenHrefChanges_UpdatesSelection()
        {
            var navigation = Context.Services.GetRequiredService<NavigationManager>();
            navigation.NavigateTo("second");

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "First");
                    item.Add(x => x.Href, "first");
                });
            });

            comp.Instance.GetSelectedItemId().Should().BeNull();

            var item = comp.FindComponent<MudNavigationBarItem>();

            await item.SetParametersAndRenderAsync(parameters =>
                parameters.Add(x => x.Href, "second"));

            comp.WaitForAssertion(() =>
                comp.Instance.GetSelectedItemId().Should().Be("First"));
        }

        /// <summary>
        /// Verifies that disabling the selected link clears selection.
        /// </summary>
        [Test]
        public async Task NavigationBarItem_WhenSelectedLinkDisabled_ClearsSelection()
        {
            var navigation = Context.Services.GetRequiredService<NavigationManager>();
            navigation.NavigateTo("first");

            var comp = Context.Render<MudNavigationBar>(builder =>
            {
                builder.AddChildContent<MudNavigationBarItem>(item =>
                {
                    item.Add(x => x.Text, "First");
                    item.Add(x => x.Href, "first");
                });
            });

            comp.WaitForAssertion(() => comp.Instance.GetSelectedItemId().Should().Be("First"));

            var item = comp.FindComponent<MudNavigationBarItem>();

            await item.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Disabled, true));

            comp.WaitForAssertion(() => comp.Instance.GetSelectedItemId().Should().BeNull());
        }
    }
}
