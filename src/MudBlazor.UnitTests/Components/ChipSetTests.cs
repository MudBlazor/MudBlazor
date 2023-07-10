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
            var comp = Context.RenderComponent<ChipSetTest>();
            // print the generated html
            // select elements needed for the test
            var chipset = comp.FindComponent<MudChipSet>();
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            chipset.Instance.SelectedChip.Should().Be(null);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");
            chipset.Instance.SelectedChip.Should().Be(null);
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
            chipset.Instance.SelectedChip.Text.Should().Be("Milk");
        }

        /// <summary>
        /// Clicking a chip selects it, clicking again does not de-select it when Mandatory="true"
        /// </summary>
        [Test]
        public async Task ChipSet_SingleSelection_Mandatory()
        {
            var comp = Context.RenderComponent<ChipSetTest>();
            // print the generated html
            // select elements needed for the test
            var chipset = comp.FindComponent<MudChipSet>();
            await chipset.InvokeAsync(() => chipset.Instance.Mandatory = true);
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            chipset.Instance.SelectedChip.Should().Be(null);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // select cornflakes again (no deselection)
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
            chipset.Instance.SelectedChip.Text.Should().Be("Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection()
        {
            var comp = Context.RenderComponent<ChipSetTest>();
            // print the generated html
            // select elements needed for the test
            var chipset = comp.FindComponent<MudChipSet>();
            await chipset.InvokeAsync(() => chipset.Instance.MultiSelection = true);
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            chipset.Instance.SelectedChip.Should().Be(null);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");
            chipset.Instance.SelectedChip.Should().Be(null);
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes, Milk");
            string.Join(", ", chipset.Instance.SelectedChips.Select(x => x.Text).OrderBy(x => x)).Should().Be("Corn flakes, Milk");
            // select red wine
            comp.FindAll("div.mud-chip")[6].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            string.Join(", ", chipset.Instance.SelectedChips.Select(x => x.Text).OrderBy(x => x)).Should().Be("Corn flakes, Milk, Red wine");
        }

        /// <summary>
        /// If multiple chips are marked as default, with a single selection only the last will be initially selected
        /// </summary>
        [Test]
        public async Task ChipSet_SingleSelection_WithMultipleDefaultChips()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>();
            // print the generated html
            // select elements needed for the test
            var chipset = comp.FindComponent<MudChipSet>();
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.WaitForAssertion(() => chipset.Instance.SelectedChip.Text.Should().Be("Salad"), TimeSpan.FromSeconds(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Salad");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");
            chipset.Instance.SelectedChip.Should().Be(null);
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes");
            chipset.Instance.SelectedChip.Text.Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
            chipset.Instance.SelectedChip.Text.Should().Be("Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection_DefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>(ComponentParameter.CreateParameter("MultiSelection", true));
            // print the generated html
            // select elements needed for the test
            var chipset = comp.FindComponent<MudChipSet>();
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            chipset.Instance.SelectedChips.Length.Should().Be(2);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Eggs, Salad");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes, Eggs, Salad");
            string.Join(", ", chipset.Instance.SelectedChips.Select(x => x.Text).OrderBy(x => x)).Should().Be("Corn flakes, Eggs, Salad");
            // de-select eggs
            comp.FindAll("div.mud-chip")[1].Click();
            comp.FindAll("p")[0].TrimmedText().Should().Be("Corn flakes, Salad");
            string.Join(", ", chipset.Instance.SelectedChips.Select(x => x.Text).OrderBy(x => x)).Should().Be("Corn flakes, Salad");
        }


        [Test]
        public async Task ChipSet_MultiSelection_LateDefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetLateDefaultTest>();
            // print the generated html
            // check that only one item is present
            var chipset = comp.FindComponent<MudChipSet>();
            comp.FindAll("div.mud-chip").Count.Should().Be(1);
            chipset.Instance.SelectedChips.Length.Should().Be(1);
            string.Join(", ", chipset.Instance.SelectedChips.Select(x => x.Text).OrderBy(x => x)).Should().Be("Primary");
            // select extra item
            comp.FindAll("button")[0].Click();
            // check that extra item is selected
            comp.FindAll("div.mud-chip").Count.Should().Be(2);
            string.Join(", ", chipset.Instance.SelectedChips.Select(x => x.Text).OrderBy(x => x)).Should().Be("Extra Chip, Primary");
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
            var chipset = comp.FindComponent<MudChipSet>();
            comp.FindAll("div.mud-clickable").Count.Should().Be(0);
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            //Click test
            comp.FindAll("div.mud-chip")[0].Click();
            
            //Should not throw an error
            comp.FindAll("button.mud-chip-close-button")[0].Click();

            chipset.Instance.SelectedChip.Should().Be(null);
        }

        /// <summary>
        /// In this test component two chipsets are synchronized via a single selectedValues collection
        /// Whenever one ChipSet changes the other must update to the same selection state.
        /// </summary>
        [Test]
        public async Task ChipSet_SelectedValues_TwoWayBinding()
        {
            var comp = Context.RenderComponent<ChipSetSelectionTwoWayBindingTest>();
            // print the generated html
            // initial values check
            comp.Find("p.set1").TrimmedText().Should().Be("Set1 Selection: Set1 Chip1");
            comp.Find("p.set2").TrimmedText().Should().Be("Set2 Selection: Set2 Chip1");

            // change selection and check state of both sets
            comp.FindAll("div.mud-chip")[0].Click();
            comp.WaitForAssertion(() => comp.Find("p.set1").TrimmedText().Should().Be("Set1 Selection:"));
            comp.WaitForAssertion(() => comp.Find("p.set2").TrimmedText().Should().Be("Set2 Selection:"));
            comp.FindAll("div.mud-chip")[1].Click();
            comp.WaitForAssertion(() => comp.Find("p.set1").TrimmedText().Should().Be("Set1 Selection: Set1 Chip2"));
            comp.WaitForAssertion(() => comp.Find("p.set2").TrimmedText().Should().Be("Set2 Selection: Set2 Chip2"));
            comp.FindAll("div.mud-chip")[2].Click();
            comp.WaitForAssertion(() => comp.Find("p.set1").TrimmedText().Should().Be("Set1 Selection: Set1 Chip1, Set1 Chip2"));
            comp.WaitForAssertion(() => comp.Find("p.set2").TrimmedText().Should().Be("Set2 Selection: Set2 Chip1, Set2 Chip2"));
            comp.FindAll("div.mud-chip")[3].Click();
            comp.WaitForAssertion(() => comp.Find("p.set1").TrimmedText().Should().Be("Set1 Selection: Set1 Chip1"));
            comp.WaitForAssertion(() => comp.Find("p.set2").TrimmedText().Should().Be("Set2 Selection: Set2 Chip1"));
        }

        [Test]
        public async Task ChipSet_InitialSelectedValuesShouldBeApplied_ButOverriddenByChipDefault()
        {
            var comp = Context.RenderComponent<ChipSetSelectionInitialValuesTest>();
            // print the generated html
            // initial values check
            comp.WaitForAssertion(() => comp.Find("p.sel").TrimmedText().Should().Be("Selection: Chip1, Chip3"));

            // change selection and check state
            comp.FindAll("div.mud-chip")[0].Click();
            comp.WaitForAssertion(() => comp.Find("p.sel").TrimmedText().Should().Be("Selection: Chip3"));
        }

        [Test]
        public async Task ChipSetComparerTest()
        {
            var comp = Context.RenderComponent<ChipSetComparerTest>();
            // print the generated html
            // initial values check
            comp.WaitForAssertion(() => comp.Find("p.sel").TrimmedText().Should().Be("Selection:"));

            // change selection and check state
            comp.FindAll("div.mud-chip")[0].Click();
            comp.WaitForAssertion(() => comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cappuccino"));

            // set new selection and see if the comparer works correctly
            comp.FindComponent<MudButton>().Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cafe Latte, Espresso"));
        }

        [Test]
        public async Task ChipSet_OtherTest()
        {
            var comp = Context.RenderComponent<ChipSetTest>();
            var chipSet = comp.FindComponent<MudChipSet>();
            var chip = comp.FindComponents<MudChip>().FirstOrDefault();

            comp.WaitForAssertion(() => chipSet.Instance.SelectedChips.Length.Should().Be(0));
            await comp.InvokeAsync(() => chipSet.Instance.SelectedChip = (MudChip)chip.Instance.Value);
            comp.WaitForAssertion(() => chipSet.Instance.SelectedChips.Length.Should().Be(1));


            await comp.InvokeAsync(() => chipSet.Instance.OnChipDeletedAsync((MudChip)chip.Instance.Value));
            comp.WaitForAssertion(() => chipSet.Instance.SelectedChips.Length.Should().Be(0));

            await comp.InvokeAsync(() => chipSet.Instance.SelectedChip = null);
            await comp.InvokeAsync(() => chipSet.Instance.SetSelectedValuesAsync(null));
            comp.WaitForAssertion(() => chipSet.Instance.SelectedChips.Length.Should().Be(0));
        }
    }

}
