#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using System.Linq;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SelectListTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// Clicking the drinks selects them, clicking it again deselects it. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.
        /// </summary>
        [Test]
        public async Task SelectListSelectionTest()
        {
            var comp = ctx.RenderComponent<SelectListSelectionTest>();
            Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudSelectList<string>>().Instance;
            list.SelectedItems.Count.Should().Be(0);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");//4th MudSelectListItem, not 5th "div.mud-list-item", because of the group
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[6].Markup.Should().Contain("mud-selected-item");//6th MudSelectListItem, not 8th "div.mud-list-item", because of the group

            // click AGAIN Cafe Latte, deselects it
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItems.Count.Should().Be(0);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            comp.FindComponents<MudSelectListItem<string>>()[6].Markup.Should().NotContain("mud-selected-item");//6th MudSelectListItem, not 8th "div.mud-list-item", because of the group
        }

        /// <summary>
        /// Clicking the drinks selects them and cannot be unselected. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.
        /// </summary>
        [Test]
        public async Task SelectListRadioSelectionTest()
        {
            var comp = ctx.RenderComponent<SelectListRadioSelectionTest>();
            Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudSelectList<string>>().Instance;
            list.SelectedItems.Count.Should().Be(0);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");//4th MudSelectListItem, not 5th "div.mud-list-item", because of the group
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[6].Markup.Should().Contain("mud-selected-item");//6th MudSelectListItem, not 8th "div.mud-list-item", because of the group
            // click AGAIN Cafe Latte, it shouldn't have any effect
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[6].Markup.Should().Contain("mud-selected-item");//6th MudSelectListItem, not 8th "div.mud-list-item", because of the group
        }


        /// <summary>
        /// Test multi selection
        /// </summary>
        [Test]
        public async Task SelectListMultiSelectionTest()
        {
            var comp = ctx.RenderComponent<SelectListMultiSelectionTest>();
            Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudSelectList<string>>().Instance;
            list.SelectedItems.Count.Should().Be(0);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.First().Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedItems.Count.Should().Be(2);
            list.SelectedItems.Contains("Sparkling Water").Should().BeTrue();
            list.SelectedItems.Contains("Pu'er").Should().BeTrue();
            comp.FindAll("div.mud-selected-item").Count.Should().Be(2);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");//4th MudSelectListItem, not 5th "div.mud-list-item", because of the group
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItems.Count.Should().Be(3);
            list.SelectedItems.Contains("Sparkling Water").Should().BeTrue();
            list.SelectedItems.Contains("Pu'er").Should().BeTrue();
            list.SelectedItems.Contains("Cafe Latte").Should().BeTrue();
            comp.FindAll("div.mud-selected-item").Count.Should().Be(3);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");//4th MudSelectListItem, not 5th "div.mud-list-item", because of the group
            comp.FindComponents<MudSelectListItem<string>>()[6].Markup.Should().Contain("mud-selected-item");//6th MudSelectListItem, not 8th "div.mud-list-item", because of the group

            // click AGAIN Cafe Latte, deselects it
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItems.Count.Should().Be(2);
            list.SelectedItems.Contains("Sparkling Water").Should().BeTrue();
            list.SelectedItems.Contains("Pu'er").Should().BeTrue();
            comp.FindAll("div.mud-selected-item").Count.Should().Be(2);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");//4th MudSelectListItem, not 5th "div.mud-list-item", because of the group
            comp.FindComponents<MudSelectListItem<string>>()[6].Markup.Should().NotContain("mud-selected-item");//6th MudSelectListItem, not 8th "div.mud-list-item", because of the group
            // click water, deselects it
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedItems.Count.Should().Be(1);
            list.SelectedItems.Contains("Pu'er").Should().BeTrue();
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");//4th MudSelectListItem, not 5th "div.mud-list-item", because of the group
            // click Pu'er, deselects it
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedItems.Count.Should().Be(0);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
        }


        /// <summary>
        /// Programmatically change the list should reflect on the component
        /// </summary>
        [Test]
        public async Task SelectListChangeSelectionTest()
        {
            var comp = ctx.RenderComponent<SelectListChangeSelectionTest>();
            Console.WriteLine(comp.Markup);
            var list = comp.FindComponent<MudSelectList<string>>().Instance;
            list.SelectedItems.Count.Should().Be(2);
            list.SelectedItems.Contains("Sparkling Water").Should().BeTrue();
            list.SelectedItems.Contains("Matcha").Should().BeTrue();
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(2);
            comp.FindComponents<MudSelectListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudSelectListItem<string>>()[2].Markup.Should().Contain("mud-selected-item");
            // click button and change
            comp.Find(".mud-button").Click();
            list.SelectedItems.Count.Should().Be(2);
            list.SelectedItems.Contains("Irish Coffee").Should().BeTrue();
            list.SelectedItems.Contains("Pu'er").Should().BeTrue();
            comp.FindAll("div.mud-selected-item").Count.Should().Be(2);
            comp.FindComponents<MudSelectListItem<string>>()[3].Markup.Should().Contain("mud-selected-item");
            comp.FindComponents<MudSelectListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
        }
    }
}
