// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class NavigationBarTests : BunitTest
    {
        [Test]
        public void NavigationBar_InitialStateText()
        {
            var comp = Context.RenderComponent<MudNavigationBar>(builder =>
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
            var comp = Context.RenderComponent<MudNavigationBar>(builder =>
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
            var comp = Context.RenderComponent<MudNavigationBar>(builder =>
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

    }
}
