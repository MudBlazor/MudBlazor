
#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ListTests : BunitTest
    {
        /// <summary>
        /// Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.
        /// 
        /// In this test no item is selected to begin with
        /// </summary>
        [Test]
        public async Task ListSelectionTest()
        {
            var comp = Context.RenderComponent<ListSelectionTest>();
            //Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudList<int>>().Instance;
            list.SelectedItem.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups 
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0); //nested lists generate 1 selected item tag
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Pu'er"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Cafe Latte"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[8].Markup.Should().Contain("mud-selected-item");
        }

        [Test]
        public async Task ListMultiSelectionTest()
        {
            var comp = Context.RenderComponent<ListSelectionTest>();
            //Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudList<int>>().Instance;
            list.SelectedItem.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups 
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0); //nested lists generate 1 selected item tag
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            list.MultiSelection = true;
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValues"));
            //comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Pu'er"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(2));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item"));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[4].Markup.Should().Contain("mud-selected-item"));
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValues"));
            //comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Cafe Latte"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(3);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudListItem<int>>()[4].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudListItem<int>>()[8].Markup.Should().Contain("mud-selected-item");

            comp.FindAll("div.mud-list-item")[4].Click();
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValues"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(2);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudListItem<int>>()[8].Markup.Should().Contain("mud-selected-item");

            await comp.InvokeAsync(() => list.Clear());
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(0));
        }

        /// <summary>
        /// Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.
        /// 
        /// This test starts with a pre-selected item (by value)
        /// </summary>
        [Test]
        public async Task ListWithPreSelectedValueTest()
        {
            var comp = Context.RenderComponent<ListSelectionInitialValueTest>();
            //Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudList<int?>>().Instance;
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            // we have seven choices, 1 is active because of the initial value of SelectedValue
            comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item").Count.Should().Be(9)); // 7 choices, 2 groups
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            // set Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.InvokeAsync(()=> comp.Instance.SetSelectedValue(4));
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(4));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Pu'er"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[4].Markup.Should().Contain("mud-selected-item"));
            // set Cafe Latte via changing SelectedValue
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(7));
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Cafe Latte"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[8].Markup.Should().Contain("mud-selected-item"));
            // set water
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(1));
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[0].Markup.Should().Contain("mud-selected-item"));
            // set nothing
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(null));
            await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Should().Be(null));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(0));
        }

        [Test]
        [TestCase(Color.Default)]
        [TestCase(Color.Primary)]
        [TestCase(Color.Secondary)]
        [TestCase(Color.Tertiary)]
        [TestCase(Color.Info)]
        [TestCase(Color.Success)]
        [TestCase(Color.Warning)]
        [TestCase(Color.Error)]
        [TestCase(Color.Dark)]
        public void ListColorTest(Color color)
        {
            var comp = Context.RenderComponent<ListSelectionInitialValueTest>(x => x.Add(c => c.Color, color));

            var list = comp.FindComponent<MudList<int?>>().Instance;
            list.SelectedItem.Text.Should().Be("Sparkling Water");

            var listItemClasses = comp.Find(".mud-selected-item");
            listItemClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"mud-{color.ToDescriptionString()}-hover" });
        }
    }
}
