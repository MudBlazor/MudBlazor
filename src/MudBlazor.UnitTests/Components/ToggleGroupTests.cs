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
    public class ToggleGroupTests : BunitTest
    {
        [Test]
        public void ToggleGroup_Bind_Test()
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
        public void ToggleGroup_CustomFragmentBind_Test()
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
        public void ToggleGroup_SelectionMode_Test()
        {
            var comp = Context.RenderComponent<ToggleBindMultiSelectionTest>();
            var group1 = comp.FindComponents<MudToggleGroup<string>>().First();
            var group2 = comp.FindComponents<MudToggleGroup<string>>().Last();
            var toggleItemSecond = comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            var toggleItemThird = comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);

            group1.Instance.SelectedValues.Should().BeNull();
            group2.Instance.SelectedValues.Should().BeNull();
            toggleItemSecond.Click();
            group1.Instance.SelectedValues.Should().Contain("Item Two");
            group2.Instance.SelectedValues.Should().Contain("Item Two");
            toggleItemThird.Click();
            group1.Instance.SelectedValues.Should().BeEquivalentTo("Item Two", "Item Three");
            group2.Instance.SelectedValues.Should().Contain("Item Three");
            toggleItemSecond.Click();
            group1.Instance.SelectedValues.Should().BeEquivalentTo("Item Three");
            group2.Instance.SelectedValues.Should().Contain("Item Three");
        }

        [Test]
        public void ToggleGroup_Initialize_Test()
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
        public void ToggleGroup_ToggleSelection_Test()
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
