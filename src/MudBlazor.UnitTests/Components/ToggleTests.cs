// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using AngleSharp.Common;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleTests : BunitTest
    {
        [Test]
        public void ToggleBind_Test()
        {
            var comp = Context.RenderComponent<ToggleBindTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            var toggleItem = comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().BeNull();
            toggleSecond.Instance.Value.Should().BeNull();
            toggleItem.Click();
            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Value.Should().Be("Item Two");
        }

        [Test]
        public void ToggleCustomFragmentBind_Test()
        {
            var comp = Context.RenderComponent<ToggleCustomFragmentTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            var toggleItem = comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);

            toggleFirst.Instance.Value.Should().BeNull();
            toggleSecond.Instance.Value.Should().BeNull();
            toggleItem.Click();
            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.Value.Should().Be("Item Two");
        }

        [Test]
        public void ToggleBindMultiSelection_Test()
        {
            var comp = Context.RenderComponent<ToggleBindMultiSelectionTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            var toggleItemSecond = comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            var toggleItemThird = comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);

            toggleFirst.Instance.SelectedValues.Should().BeNull();
            toggleSecond.Instance.SelectedValues.Should().BeNull();
            toggleItemSecond.Click();
            toggleFirst.Instance.SelectedValues.Should().Contain("Item Two");
            toggleSecond.Instance.SelectedValues.Should().Contain("Item Two");
            toggleItemThird.Click();
            toggleFirst.Instance.SelectedValues.Should().BeEquivalentTo("Item Two", "Item Three");
            toggleSecond.Instance.SelectedValues.Should().Contain("Item Three");
            toggleItemSecond.Click();
            toggleFirst.Instance.SelectedValues.Should().BeEquivalentTo("Item Three");
            toggleSecond.Instance.SelectedValues.Should().Contain("Item Three");
        }

        [Test]
        public void ToggleInitialize_Test()
        {
            var comp = Context.RenderComponent<ToggleInitializeTest>();
            var toggleFirst = comp.FindComponents<MudToggleGroup<string>>().First();
            var toggleSecond = comp.FindComponents<MudToggleGroup<string>>().Last();
            var buttonOne = comp.FindAll("button").First();
            var buttonTwo = comp.FindAll("button").Last();

            toggleFirst.Instance.Value.Should().Be("Item Two");
            toggleSecond.Instance.SelectedValues.Should().BeEquivalentTo("Item One", "Item Three");

            buttonOne.Click();
            toggleFirst.Instance.Value.Should().Be("Item One");

            buttonTwo.Click();
            toggleSecond.Instance.SelectedValues.Should().BeEquivalentTo("Item Two", "Item Three");
        }

        [Test]
        public void ToggleToggleSelection_Test()
        {
            var comp = Context.RenderComponent<ToggleToggleSelectionTest>();
            var toggle = comp.FindComponent<MudToggleGroup<string>>();
            var toggleItem = comp.FindAll("div.mud-toggle-item").GetItemByIndex(0);

            toggle.Instance.Value.Should().BeNull();
            toggleItem.Click();
            toggle.Instance.Value.Should().Be("Item One");
            toggleItem.Click();
            toggle.Instance.Value.Should().BeNull();
        }
    }
}
