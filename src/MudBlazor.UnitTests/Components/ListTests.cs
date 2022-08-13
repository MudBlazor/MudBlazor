
#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005

using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
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
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Pu'er"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
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
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            list.MultiSelection = true;
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Pu'er"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(2));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item"));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[4].Markup.Should().Contain("mud-selected-item"));
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Cafe Latte"));
            comp.FindAll("div.mud-selected-item").Count.Should().Be(3);
            comp.FindComponents<MudListItem<int>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudListItem<int>>()[4].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudListItem<int>>()[8].Markup.Should().Contain("mud-selected-item");

            comp.FindAll("div.mud-list-item")[4].Click();
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
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[0].Markup.Should().Contain("mud-selected-item"));
            comp.WaitForAssertion(() => list.SelectedItem.Value.Should().Be(1));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            // we have seven choices, 1 is active because of the initial value of SelectedValue
            comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item").Count.Should().Be(9)); // 7 choices, 2 groups
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            // set Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.InvokeAsync(()=> comp.Instance.SetSelectedValue(4));
            //await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(4));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Pu'er"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[4].Markup.Should().Contain("mud-selected-item"));
            // set Cafe Latte via changing SelectedValue
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(7));
            //await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Cafe Latte"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[8].Markup.Should().Contain("mud-selected-item"));
            // set water
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(1));
            //await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water"));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(1));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int?>>()[0].Markup.Should().Contain("mud-selected-item"));
            // set nothing
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(null));
            //await comp.InvokeAsync(() => list.HandleCentralValueCommander("SelectedValue"));
            comp.WaitForAssertion(() => list.SelectedItem.Should().Be(null));
            comp.WaitForAssertion(() => comp.FindAll("div.mud-selected-item").Count.Should().Be(0));
        }

        [Test]
        public async Task List_ProgrammaticallyChangeValueAndItemTest()
        {
            var comp = Context.RenderComponent<ListVariantTest>();
            var list = comp.FindComponent<MudList<int?>>().Instance;

            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(1));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Sparkling Water (1)"));

            comp.FindAll("button.mud-button-root")[1].Click();
            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(2));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Still Water (2)"));

            comp.FindAll("button.mud-button-root")[3].Click();
            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(3));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Earl Grey (3)"));

            // Changing multiselection should not affect value or item
            await comp.InvokeAsync(() => list.MultiSelection = true);
            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(3));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Earl Grey (3)"));

            comp.FindAll("button.mud-button-root")[2].Click();
            comp.WaitForAssertion(() => list.SelectedValues.Should().ContainInOrder(new int?[] { 2, 4 }));
            comp.WaitForAssertion(() => string.Join(", ", list.SelectedItems.Select(x => x.Text)).Should().Be("Still Water (2), Matcha (4)"));

            comp.FindAll("button.mud-button-root")[4].Click();
            comp.WaitForAssertion(() => list.SelectedValues.Should().ContainInOrder(new int?[] { 3, 5 }));
            comp.WaitForAssertion(() => string.Join(", ", list.SelectedItems.Select(x => x.Text)).Should().Be("Earl Grey (3), Pu'er (5)"));

            // Changing multiselection now should select only one value
            await comp.InvokeAsync(() => list.MultiSelection = false);
            comp.WaitForAssertion(() => list.SelectedValue.Should().Be(3));
            comp.WaitForAssertion(() => list.SelectedItem.Text.Should().Be("Earl Grey (3)"));
            comp.WaitForAssertion(() => list.SelectedValues.Should().ContainSingle());
            comp.WaitForAssertion(() => string.Join(", ", list.SelectedItems.Select(x => x.Text)).Should().Be("Earl Grey (3)"));
        }

        [Test]
        public async Task List_KeyboardNavigationTest()
        {
            var comp = Context.RenderComponent<ListEnhancedTest>();
            //Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudList<int>>().Instance;

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().BeNullOrEmpty());

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().ContainSingle());
            //Second item is functional, should skip.
            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Instance.IsActive.Should().BeFalse());
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[2].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().ContainSingle());

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "NumpadEnter" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[2].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().ContainInOrder(new int[] { 1, 3 }));

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown" }));
            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown" }));
            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[2].Instance.IsActive.Should().BeFalse());
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[5].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(3));
            comp.WaitForAssertion(() => list.SelectedValues.Should().Contain(5));

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[5].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(2));
            comp.WaitForAssertion(() => list.SelectedValues.Should().Contain(3));
            //Last disabled item should not be active
            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "End" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[5].Instance.IsActive.Should().BeTrue());

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "Home" }));
            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(1));
            comp.WaitForAssertion(() => list.SelectedValues.Should().NotContain(1));

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "a", CtrlKey = true }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(7));
            comp.WaitForAssertion(() => list.SelectedValues.Should().NotContain(2));

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "A", CtrlKey = true }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[0].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(0));

            await comp.InvokeAsync(() => comp.FindAll("div.mud-list-item")[5].Click());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(1));
            comp.WaitForAssertion(() => list.SelectedValues.Should().Contain(5));

            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowDown" }));
            await comp.InvokeAsync(() => list.HandleKeyDown(new KeyboardEventArgs() { Key = "Enter" }));
            comp.WaitForAssertion(() => comp.FindComponents<MudListItem<int>>()[6].Instance.IsActive.Should().BeTrue());
            comp.WaitForAssertion(() => list.SelectedValues.Should().HaveCount(2));
            comp.WaitForAssertion(() => list.SelectedValues.Should().Contain(6));
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
