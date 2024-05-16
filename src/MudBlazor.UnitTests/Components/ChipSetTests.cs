using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.Docs.Examples;
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
        public void ChipSet_SingleSelection()
        {
            var comp = Context.RenderComponent<ChipSetSingleSelectionTest>();
            // initially nothing is selected
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
        }

        [Test]
        public void ChipSet_SingleSelection_WithInitialValue()
        {
            var comp = Context.RenderComponent<ChipSetSingleSelectionTest>(p => p.Add(x => x.InitialValue, "Milk"));
            // initial value is selected
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
        }

        /// <summary>
        /// Clicking a chip selects it, clicking again does not de-select it when Mandatory="true"
        /// </summary>
        [Test]
        public void ChipSet_SingleSelection_Mandatory()
        {
            var comp = Context.RenderComponent<ChipSetSingleSelectionTest>(parameters => parameters
                .Add(p => p.SelectionMode, SelectionMode.SingleSelection)
            );
            // initially nothing is selected
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
        }

        [Test]
        public void ChipSet_MultiSelection()
        {
            var comp = Context.RenderComponent<ChipSetMultiSelectionTest>();
            // select elements needed for the test
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk");
            // select red wine
            comp.FindAll("div.mud-chip")[6].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            // de-select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Red wine");
        }

        [Test]
        public void ChipSet_MultiSelection_WithInitialValues()
        {
            var comp = Context.RenderComponent<ChipSetMultiSelectionTest>(parameters => parameters.Add(x => x.InitialValues, ["Corn flakes", "Milk", "Red wine"]));
            // initial values should be selected
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            // de-select milk
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Red wine");
        }

        /// <summary>
        /// If multiple chips are marked as default, with a single selection only the last will be initially selected
        /// </summary>
        [Test]
        public void ChipSet_SingleSelection_WithMultipleDefaultChips()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>();
            // select elements needed for the test
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find(".selected-value").TrimmedText().Should().Be("Corn flakes");
        }

        [Test]
        public void ChipSet_MultiSelection_DefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>(p => p.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find(".selected-values").TrimmedText().Should().Be("Corn flakes, Milk");
            // de-select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find(".selected-values").TrimmedText().Should().Be("Milk");
            // select eggs
            comp.FindAll("div.mud-chip")[1].Click();
            comp.Find(".selected-values").TrimmedText().Should().Be("Eggs, Milk");
        }

        [Test]
        public void ChipSet_MultiSelection_DefaultChipsShouldOverrideInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetDefaultChipsTest>(p => p
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.InitialValues, ["Eggs", "Soap"])
            );
            comp.FindAll("div.mud-chip").Count.Should().Be(7);
            comp.Find(".selected-values").TrimmedText().Should().Be("Corn flakes, Eggs, Milk");
            // de-select cornflakes
            comp.FindAll("div.mud-chip")[3].Click();
            comp.Find(".selected-values").TrimmedText().Should().Be("Eggs, Milk");
            // select soap
            comp.FindAll("div.mud-chip")[2].Click();
            comp.Find(".selected-values").TrimmedText().Should().Be("Eggs, Milk, Soap");
        }

        [Test]
        public void ChipSet_MultiSelection_LateDefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.RenderComponent<ChipSetLateDefaultTest>();
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
        public void ChipSet_ReadOnly()
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
        public void ChipSet_SelectedValues_TwoWayBinding()
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
        public void ChipSetComparerTest()
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
        public void ChipSet_MultiSelection_AfterChipArraySetNull_ShouldBeAbleToSelectSameChip()
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
        public void ChipSet_MultiSelection_AfterChipArraySetEmpty_ShouldBeAbleToSelectSameChip()
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
        public void Chip_GetValue_ShouldReturnTextIfValueIsNullAndT_IsString()
        {
            // Backwards compatibility with non-generic chips where setting the Text without a Value treated the Text as Value
            Context.RenderComponent<MudChip<string>>(p => p
                .Add(x => x.Text, "はい")
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

        [Test]
        public async Task ChipSet_CheckMark_Parameter()
        {
            var comp = Context.RenderComponent<MudChipSet<string>>(self => self
                .Add(x => x.CheckMark, true)
                .Add(x => x.SelectedValue, "x")
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "x"))
                .Add(x => x.CheckedIcon, Icons.Material.Filled.Cake)
                .Add(x => x.CloseIcon, Icons.Material.Filled.Plagiarism)
                .Add(x => x.Ripple, false)
                .Add(x => x.IconColor, Color.Error)
            );
            comp.FindAll("svg").Count.Should().Be(1);
            comp.Instance.CheckMark.Should().Be(true);
            comp.Instance.CheckMark.Should().Be(true);
            comp.SetParametersAndRender(self => self.Add(x => x.CheckMark, false));
            comp.FindAll("svg").Count.Should().Be(0);
            comp.Instance.CheckMark.Should().Be(false);
            comp.Instance.CheckMark.Should().Be(false);
            // for coverage
            new MudChip<int>().ShowCheckMark.Should().Be(false);
            var chip = Context.RenderComponent<MudChip<string>>(chip => chip
                .Add(x => x.CheckedIcon, Icons.Material.Filled.Cake)
                .Add(x => x.CloseIcon, Icons.Material.Filled.Plagiarism)
                .Add(x => x.Ripple, false)
                .Add(x => x.IconColor, Color.Error)
                .Add(x => x.Selected, true)
            ).Instance;
            await comp.InvokeAsync(() => chip.UpdateSelectionStateAsync(true));
            chip.ShowCheckMark.Should().Be(false); // because not in a chipset
            Context.RenderComponent<MudChip<int>>(self => self.Add(x => x.Variant, (Variant)69)).Instance.GetVariant().Should().Be(Variant.Outlined); // falls back to outlined
        }

        [Test]
        public async Task ChipSet_RemoveChip_Logic()
        {
            IReadOnlyCollection<string> selectedValues = ["x", "y", "z"];
            var comp = Context.RenderComponent<MudChipSet<string>>(self => self
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Bind(x => x.SelectedValues, selectedValues, x => selectedValues = x)
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "x"))
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "y"))
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "z"))
            );
            await comp.InvokeAsync(() => comp.Instance.RemoveAsync(comp.FindComponent<MudChip<string>>().Instance));
            string.Join(", ", selectedValues).Should().Be("y, z");
            // removing a foreign chip doesn't do anything
            await comp.Instance.RemoveAsync(Context.RenderComponent<MudChip<string>>(chip => chip.Add(x => x.Value, "y")).Instance);
            string.Join(", ", selectedValues).Should().Be("y, z");
            // removing from a disposed chipset doesn't raise events, so in this case the selection stays the same
            var chipY = comp.FindComponent<MudChip<string>>().Instance;
            comp.Instance.Dispose();
            await comp.Instance.RemoveAsync(chipY);
            string.Join(", ", selectedValues).Should().Be("y, z");
        }

        [Test]
        public void ChipSet_With_NonValueTypes_DoesntCrash()
        {
            var a = new object();
            var b = new object();
            var c = new object();
            IReadOnlyCollection<object> selectedValues = [a];
            var comp = Context.RenderComponent<MudChipSet<object>>(self => self
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Bind(x => x.SelectedValues, selectedValues, x => selectedValues = x)
                .AddChildContent<MudChip<object>>(chip => chip.Add(x => x.Value, a))
                .AddChildContent<MudChip<object>>(chip => chip.Add(x => x.Value, b))
                .AddChildContent<MudChip<object>>(chip => chip.Add(x => x.Value, c))
            );
            comp.FindAll("div.mud-chip")[1].Click();
            selectedValues.Should().Contain(a).And.Contain(b);
            comp.FindAll("div.mud-chip")[0].Click();
            selectedValues.Should().NotContain(a).And.Contain(b);
        }

        [Test]
        public void Chip_TwoWayBinding_ShouldUpdateSelection()
        {
            var comp = Context.RenderComponent<ChipSetChipBindingTest>();
            comp.Find("div.selection").TrimmedText().Should().Be("Add ingredients to your coctail.");
            // initial state
            comp.FindAll("div.mud-chip")[0].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[2].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-false");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-false");

            // click Vodka chip
            comp.FindAll("div.mud-chip")[0].Click();
            comp.Find("div.selection").TrimmedText().Should().Be("Vodka");
            comp.FindAll("div.mud-chip")[0].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[2].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-true");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-false");

            // click Olive checkbox
            comp.FindAll("input.mud-checkbox-input")[2].Change(true);
            comp.Find("div.selection").TrimmedText().Should().Be("Olive, Vodka");
            comp.FindAll("div.mud-chip")[0].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[2].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-true");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-true");

            // click Vodka checkbox
            comp.FindAll("input.mud-checkbox-input")[0].Change(false);
            comp.Find("div.selection").TrimmedText().Should().Be("Olive");
            comp.FindAll("div.mud-chip")[0].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll("div.mud-chip")[2].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-false");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-true");
        }
    }

}
