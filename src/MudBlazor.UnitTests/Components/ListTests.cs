
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
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
            Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudList>().Instance;
            list.SelectedItem.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedItem.Text.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedItem.Text.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItem.Text.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem>()[8].Markup.Should().Contain("mud-selected-item");
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
            Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudList>().Instance;
            list.SelectedItem.Text.Should().Be("Sparkling Water");
            // we have seven choices, 1 is active because of the initial value of SelectedValue
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            // set Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.InvokeAsync(()=>comp.Instance.SetSelecedValue(4));
            list.SelectedItem.Text.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem>()[4].Markup.Should().Contain("mud-selected-item");
            // set Cafe Latte via changing SelectedValue
            await comp.InvokeAsync(() => comp.Instance.SetSelecedValue(7));
            list.SelectedItem.Text.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem>()[8].Markup.Should().Contain("mud-selected-item");
            // set water
            await comp.InvokeAsync(() => comp.Instance.SetSelecedValue(1));
            list.SelectedItem.Text.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem>()[0].Markup.Should().Contain("mud-selected-item");
            // set nothing
            await comp.InvokeAsync(() => comp.Instance.SetSelecedValue(null));
            list.SelectedItem.Should().Be(null);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
        }
    }
}
