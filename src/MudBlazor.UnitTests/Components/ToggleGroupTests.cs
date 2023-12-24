// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
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
        
        [Test]
        public  async Task ToggleGroup_HorizontalItemPadding_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Dense, false);
                builder.Add(x => x.Rounded, false);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-2");
                item.ClassList.Should().Contain("py-2");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Dense, true));
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-1");
                item.ClassList.Should().Contain("py-1");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Rounded, true));
            var item1 = comp.FindAll("div.mud-toggle-item").GetItemByIndex(0);
            var item2 = comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            var item3 = comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);
            // (x|_|_)
            item1.ClassList.Should().Contain("ps-2");
            item1.ClassList.Should().Contain("pe-1");
            item1.ClassList.Should().Contain("py-1");
            // (_|X|_)
            item2.ClassList.Should().Contain("px-1");
            item2.ClassList.Should().Contain("py-1");
            // (_|_|x)
            item3.ClassList.Should().Contain("pe-2");
            item3.ClassList.Should().Contain("ps-1");
            item3.ClassList.Should().Contain("py-1");
        }
        
        [Test]
        public async Task ToggleGroup_VerticalItemPadding_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Dense, false);
                builder.Add(x => x.Rounded, false);
                builder.Add(x => x.Vertical, true);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-2");
                item.ClassList.Should().Contain("py-2");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Dense, true));
            foreach (var item in comp.FindAll("div.mud-toggle-item"))
            {
                item.ClassList.Should().Contain("px-1");
                item.ClassList.Should().Contain("py-1");
            }
            await comp.InvokeAsync(() => comp.SetParam(x => x.Rounded, true));
            var item1 = comp.FindAll("div.mud-toggle-item").GetItemByIndex(0);
            var item2 = comp.FindAll("div.mud-toggle-item").GetItemByIndex(1);
            var item3 = comp.FindAll("div.mud-toggle-item").GetItemByIndex(2);
            // top (x|_|_) bottom
            item1.ClassList.Should().Contain("pt-2");
            item1.ClassList.Should().Contain("pb-1");
            item1.ClassList.Should().Contain("px-1");
            // top (_|X|_) bottom
            item2.ClassList.Should().Contain("px-1");
            item2.ClassList.Should().Contain("py-1");
            // top (_|_|x) bottom
            item3.ClassList.Should().Contain("pb-2");
            item3.ClassList.Should().Contain("pt-1");
            item3.ClassList.Should().Contain("px-1");
        }
        
        [Test]
        public void ToggleGroup_CustomClasses_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.IconClass, "c69");
                builder.Add(x => x.TextClass, "c42");
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a").Add(x=>x.Icon, @Icons.Material.Filled.Coronavirus));
            });
            var icon = comp.Find("svg");
            icon.ClassList.Should().Contain("c69");
            icon.ClassList.Should().Contain("me-2"); // <--- the spacing between icon and text
            var text = comp.Find(".mud-typography");
            text.ClassList.Should().Contain("c42");
            // now hide the text and check that above spacing is gone
            comp.InvokeAsync(() => comp.SetParam(x => x.ShowText, false));
            comp.FindAll(".mud-typography").Count.Should().Be(0);
            icon = comp.Find("svg");
            icon.ClassList.Should().Contain("c69");
            icon.ClassList.Should().NotContain("me-2");
        }
                
        [Test]
        public void ToggleItem_IsEmpty_Test()
        {
            new MudToggleItem<string>() { Text = null, Value = null }.IsEmpty.Should().Be(true);
            new MudToggleItem<string>() { Text = "", Value = null }.IsEmpty.Should().Be(true);
            new MudToggleItem<string>() { Text = "a", Value = null }.IsEmpty.Should().Be(false);
            new MudToggleItem<string>() { Text = null, Value = "a" }.IsEmpty.Should().Be(false);
        }
        
        [Test]
        public void ToggleGroup_ItemRegistration_Test()
        {
            var comp = Context.RenderComponent<MudToggleGroup<string>>(builder =>
            {
                builder.Add(x => x.Dense, false);
                builder.Add(x => x.Rounded, false);
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "a"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "b"));
                builder.AddChildContent<MudToggleItem<string>>(item => item.Add(x => x.Value, "c"));
            });
            comp.Instance.GetItems().Count().Should().Be(3);
            // re-registering an item won't do nothing
            comp.Instance.Register(comp.Instance.GetItems().First());
            comp.Instance.GetItems().Count().Should().Be(3);
        }
    }
}
