#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.ChipSet;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ChipSetTests : BunitTest
    {
        /// <summary>
        /// Clicking a chip selects it, clicking again de-selects it. Clicking one chip de-selects the other
        /// </summary>
        [Test]
        public async Task ChipSet_SingleSelection()
        {
            var comp = Context.RenderComponent<ChipSetSingleSelectionTest>();
            // initially nothing is selected
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Milk");
        }

        [Test]
        public async Task ChipSet_SingleSelection_WithInitialValue()
        {
            var comp = Context.RenderComponent<ChipSetSingleSelectionTest>(p => p.Add(x=>x.InitialValue, "Milk"));
            // initial value is selected
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Milk");
        }

        /// <summary>
        /// Clicking a chip selects it, clicking again does not de-select it when Mandatory="true"
        /// </summary>
        [Test]
        public async Task ChipSet_SingleSelection_Mandatory()
        {
            var comp = Context.RenderComponent<ChipSetSingleSelectionTest>(parameters => parameters
                .Add(p => p.Mandatory, true)
            );
            // initially nothing is selected
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection()
        {
            var comp = Context.RenderComponent<ChipSetMultiSelectionTest>();
            // select elements needed for the test
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk");
            // select red wine
            comp.FindAll("div.mud-chip")[6].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            // de-select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Red wine");
        }

        [Test]
        public async Task ChipSet_MultiSelection_WithInitialValues()
        {
            var comp = Context.RenderComponent<ChipSetMultiSelectionTest>(parameters => parameters.Add(x=>x.InitialValues, ["Corn flakes", "Milk", "Red wine"]));
            // initial values should be selected
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            // de-select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Red wine");
        }

        /// <summary>
        /// If multiple chips are marked as default, with a single selection only the last will be initially selected
        /// </summary>
        [Test]
        public async Task ChipSet_SingleSelection_WithMultipleDefaultChips()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>();
            // select elements needed for the test
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
        }

        [Test]
        public async Task ChipSet_MultiSelection_DefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>(p=>p.Add(x=>x.MultiSelection, true));
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes, Milk");
            // de-select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
            // select eggs
            comp.FindAll("div.mud-chip")[1].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Eggs, Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection_DefaultChipsShouldOverrideInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>(p => p
                .Add(x => x.MultiSelection, true)
                .Add(x=>x.InitialValues, ["Eggs", "Soap"])
            );
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes, Eggs, Milk");
            // de-select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Eggs, Milk");
            // select soap
            comp.FindAll("div.mud-chip")[2].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Eggs, Milk, Soap");
        }

        [Test]
        public async Task ChipSet_MultiSelection_LateDefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetLateDefaultTest>();
            Console.WriteLine(comp.Markup);
            // check that only one item is present
            comp.FindAll("div.mud-chip").Count.Should().Be(1);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Primary");
            // select extra item
            comp.FindAll("button")[0].Click();
            // check that extra item is selected
            comp.FindAll("div.mud-chip").Count.Should().Be(2);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Extra Chip, Primary");
        }

        /// <summary>
        /// If chip set parameter ReadOnly is set to true, mud-clickable and mud-ripple should not be
        /// added to chips and chip click event should return without executing any code
        /// </summary>
        [Test]
        public async Task ChipSet_ReadOnly()
        {
            var comp = Context.RenderComponent<ChipSetReadOnlyTest>();
            // print the generated html
            // no chip should have mud-clickable or mud-ripple classes
            var chipset = comp.FindComponent<MudChipSet<string>>();
            comp.FindAll("div.mud-clickable").Count.Should().Be(0);
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            //Click test
            comp.FindAll("div.mud-chip")[0].Click();

            //Should not throw an error
            comp.FindAll("button.mud-chip-close-button")[0].Click();

            chipset.Instance.SelectedValue.Should().Be(null);
        }

        /// <summary>
        /// In this test component two chipsets are synchronized via a single selectedValues collection
        /// Whenever one ChipSet changes the other must update to the same selection state.
        /// </summary>
        [Test]
        public async Task ChipSet_SelectedValues_TwoWayBinding()
        {
            var comp = Context.RenderComponent<ChipSetSelectionTwoWayBindingTest>();
            // initial values check
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 1");
            comp.FindComponents<MudChip<int>>()[0].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find("div").ClassList.Should().NotContain("mud-chip-selected");

            // change selection and check state of both sets
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("p.set").TrimmedText().Should().Be("Selection:");
            comp.FindComponents<MudChip<int>>()[0].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[1].Click();
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 2");
            comp.FindComponents<MudChip<int>>()[0].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[2].Click();
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 1, 2");
            comp.FindComponents<MudChip<int>>()[0].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 1");
            comp.FindComponents<MudChip<int>>()[0].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find("div").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find("div").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find("div").ClassList.Should().NotContain("mud-chip-selected");
        }

        [Test]
        public async Task ChipSetComparerTest()
        {
            var comp = Context.RenderComponent<ChipSetComparerTest>();
            // initial values check
           comp.Find("p.sel").TrimmedText().Should().Be("Selection:");

            // change selection and check state
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cappuccino");

            // set new selection and see if the comparer works correctly
            comp.FindComponent<MudButton>().Find("button").Click();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cafe Latte!, Espresso!");

            // change selection and check state
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cafe Latte!, Cappuccino, Espresso!");

            // change selection and check state
            comp.FindAll("div.mud-chip")[1].Click();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cappuccino, Espresso!");
        }

        [Test]
        public async Task ChipSet_MultiSelection_AfterChipArraySetNull_ShouldBeAbleToSelectSameChip()
        {
            var comp = Context.RenderComponent<ChipSetClearSelectionTest>();
            var chipSet = comp.FindComponent<MudChipSet<string>>();

            // Select one chip
            comp.FindAll("div.mud-chip")[0].Click();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");

            // Set chip array to null
            comp.FindAll("button")[0].Click();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedValues.Count.Should().Be(0));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");

            // Select same chip again
            comp.FindAll("div.mud-chip")[0].Click();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection_AfterChipArraySetEmpty_ShouldBeAbleToSelectSameChip()
        {
            var comp = Context.RenderComponent<ChipSetClearSelectionTest>();
            var chipSet = comp.FindComponent<MudChipSet<string>>();

            // Select one chip
            comp.FindAll("div.mud-chip")[0].Click();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");

            // Set chip array to empty
            comp.FindAll("button")[1].Click();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedValues.Count.Should().Be(0));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");

            // Select same chip again
            comp.FindAll("div.mud-chip")[0].Click();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
        }

        [Test]
        public async Task Chip_GetValue_ShouldReturnTextIfValueIsNullAndT_IsString()
        {
            // Backwards compatibility with non-generic chips where setting the Text without a Value treated the Text as Value
            Context.RenderComponent<MudChip<string>>(p => p
                .Add(x=>x.Text, "はい")
            ).Instance.GetValue().Should().Be("はい");
            Context.RenderComponent<MudChip<string>>(p => p
                .Add(x => x.Text, "はい")
                .Add(x => x.Value, "Yes")
            ).Instance.GetValue().Should().Be("Yes");
            // Not for types != string though!
            Context.RenderComponent<MudChip<int?>>(p => p
                .Add(x => x.Text, "Zero")
            ).Instance.GetValue().Should().Be(null);
            Context.RenderComponent<MudChip<int?>>(p => p
                .Add(x => x.Text, "Zero")
                .Add(x => x.Value, 0)
            ).Instance.GetValue().Should().Be(0);
        }
    }

}
