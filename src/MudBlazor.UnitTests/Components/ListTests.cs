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
        /// <para>Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.</para>
        /// <para>In this test no item is selected to begin with</para>
        /// </summary>
        [Test]
        public void ListSelectionTest()
        {
            var comp = Context.RenderComponent<ListSelectionTest>();
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedItem.Should().Be(null);
            // we have seven choices, none is active
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
            // click water
            comp.FindAll("div.mud-list-item")[0].Click();
            list.SelectedItem.Text.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // click Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            comp.FindAll("div.mud-list-item")[4].Click();
            list.SelectedItem.Text.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // click Cafe Latte
            comp.FindAll("div.mud-list-item")[8].Click();
            list.SelectedItem.Text.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
        }

        /// <summary>
        /// <para>Clicking the drinks selects them. The child lists are updated accordingly, meaning, only ever 1 list item can have the active class.</para>
        /// <para>This test starts with a pre-selected item (by value)</para>
        /// </summary>
        [Test]
        public async Task ListWithPreSelectedValueTest()
        {
            var comp = Context.RenderComponent<ListSelectionInitialValueTest>();
            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedItem.Text.Should().Be("Sparkling Water");
            // we have seven choices, 1 is active because of the initial value of SelectedValue
            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            // set Pu'er, a heavily fermented Chinese tea that tastes like an old leather glove
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue("Pu'er"));
            list.SelectedItem.Text.Should().Be("Pu'er");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[4].Markup.Should().Contain("mud-selected-item");
            // set Cafe Latte via changing SelectedValue
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue("Cafe Latte"));
            list.SelectedItem.Text.Should().Be("Cafe Latte");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[8].Markup.Should().Contain("mud-selected-item");
            // set water
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue("Sparkling Water"));
            list.SelectedItem.Text.Should().Be("Sparkling Water");
            comp.FindAll("div.mud-selected-item").Count.Should().Be(1);
            comp.FindComponents<MudListItem<string>>()[0].Markup.Should().Contain("mud-selected-item");
            // set nothing
            await comp.InvokeAsync(() => comp.Instance.SetSelectedValue(null));
            list.SelectedItem.Should().Be(null);
            comp.FindAll("div.mud-selected-item").Count.Should().Be(0);
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

            var list = comp.FindComponent<MudList<string>>().Instance;
            list.SelectedItem.Text.Should().Be("Sparkling Water");

            var listItemClasses = comp.Find(".mud-selected-item");
            listItemClasses.ClassList.Should().ContainInOrder(new[] { $"mud-{color.ToDescriptionString()}-text", $"mud-{color.ToDescriptionString()}-hover" });
        }

        /// <summary>
        /// The child lists should honor the Dense property of their parent list if not overriden.
        /// </summary>
        [Test]
        [TestCase(true, null, 9)]
        [TestCase(false, null, 0)]
        [TestCase(true, true, 9)]
        [TestCase(false, false, 0)]
        [TestCase(true, false, 5)]
        [TestCase(false, true, 4)]
        public void ListDenseInheritanceTest(bool dense, bool? innerListDense, int expectedDenseClassCount)
        {
            var comp = Context.RenderComponent<ListDenseInheritanceTest>(x => x.Add(c => c.Dense, dense).Add(c => c.InnerListDense, innerListDense));

            comp.FindAll("div.mud-list-item").Count.Should().Be(9); // 7 choices, 2 groups
            comp.FindAll("div.mud-list-item-dense").Count.Should().Be(expectedDenseClassCount); // 7 choices, 2 groups
        }
    }
}
