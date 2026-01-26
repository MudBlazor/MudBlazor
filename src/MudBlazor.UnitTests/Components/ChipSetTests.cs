using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Docs.Examples;
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
            var comp = Context.Render<ChipSetSingleSelectionTest>();
            // initially nothing is selected
            comp.FindAll(".mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // de-select cornflakes by clicking again
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select milk
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
        }

        [Test]
        public void ChipSet_SingleSelection_WithInitialValue()
        {
            var comp = Context.Render<ChipSetSingleSelectionTest>(p => p.Add(x => x.InitialValue, "Milk"));
            // initial value is selected
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
        }

        /// <summary>
        /// Clicking a chip selects it, clicking again does not de-select it when Mandatory="true"
        /// </summary>
        [Test]
        public async Task ChipSet_SingleSelection_Mandatory()
        {
            var comp = Context.Render<ChipSetSingleSelectionTest>(parameters => parameters
                .Add(p => p.SelectionMode, SelectionMode.SingleSelection)
            );
            // initially nothing is selected
            comp.FindAll(".mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // de-select cornflakes by clicking again
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Corn flakes");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select milk
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("Milk");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
        }

        [Test]
        public async Task ChipSet_MultiSelection()
        {
            var comp = Context.Render<ChipSetMultiSelectionTest>();
            // select elements needed for the test
            comp.FindAll(".mud-chip").Count.Should().Be(7);
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // de-select cornflakes by clicking again
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Nothing selected");
            // select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes");
            // select milk
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk");
            // select red wine
            await comp.FindAll("button.mud-chip")[6].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            // de-select milk
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Red wine");
        }

        [Test]
        public async Task ChipSet_MultiSelection_WithInitialValues()
        {
            var comp = Context.Render<ChipSetMultiSelectionTest>(parameters => parameters.Add(x => x.InitialValues, ["Corn flakes", "Milk", "Red wine"]));
            // initial values should be selected
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Milk, Red wine");
            // de-select milk
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("div.selected-value").TrimmedText().Should().Be("");
            comp.Find("div.selected-values").TrimmedText().Should().Be("Corn flakes, Red wine");
        }

        /// <summary>
        /// If multiple chips are marked as default, with a single selection only the last will be initially selected
        /// </summary>
        [Test]
        public void ChipSet_SingleSelection_WithMultipleDefaultChips()
        {
            var comp = Context.Render<ChipSetDefaultChipsTest>();
            // select elements needed for the test
            comp.FindAll(".mud-chip").Count.Should().Be(7);
            comp.Find(".selected-value").TrimmedText().Should().Be("Corn flakes");
        }

        [Test]
        public async Task ChipSet_MultiSelection_DefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.Render<ChipSetDefaultChipsTest>(p => p.Add(x => x.SelectionMode, SelectionMode.MultiSelection));
            comp.FindAll(".mud-chip").Count.Should().Be(7);
            comp.Find(".selected-values").TrimmedText().Should().Be("Corn flakes, Milk");
            // de-select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find(".selected-values").TrimmedText().Should().Be("Milk");
            // select eggs
            await comp.FindAll("button.mud-chip")[1].ClickAsync();
            comp.Find(".selected-values").TrimmedText().Should().Be("Eggs, Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection_DefaultChipsShouldOverrideInitiallySelected()
        {
            var comp = Context.Render<ChipSetDefaultChipsTest>(p => p
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Add(x => x.InitialValues, ["Eggs", "Soap"])
            );
            comp.FindAll(".mud-chip").Count.Should().Be(7);
            comp.Find(".selected-values").TrimmedText().Should().Be("Corn flakes, Eggs, Milk");
            // de-select cornflakes
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find(".selected-values").TrimmedText().Should().Be("Eggs, Milk");
            // select soap
            await comp.FindAll("button.mud-chip")[2].ClickAsync();
            comp.Find(".selected-values").TrimmedText().Should().Be("Eggs, Milk, Soap");
        }

        [Test]
        public async Task ChipSet_MultiSelection_LateDefaultChipsShouldBeInitiallySelected()
        {
            var comp = Context.Render<ChipSetLateDefaultTest>();
            // check that only one item is present
            comp.FindAll(".mud-chip").Count.Should().Be(1);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Primary");
            // select extra item
            await comp.Find("#enable-button").ClickAsync();
            // check that extra item is selected
            comp.FindAll(".mud-chip").Count.Should().Be(2);
            comp.FindAll("p")[0].TrimmedText().Should().Be("Extra Chip, Primary");
        }

        /// <summary>
        /// If chip set parameter ReadOnly is set to true, mud-clickable and mud-ripple should not be
        /// added to chips and chip click event should return without executing any code
        /// </summary>
        [Test]
        public async Task ChipSet_ReadOnly()
        {
            var comp = Context.Render<ChipSetReadOnlyTest>();
            // print the generated html
            // no chip should have mud-clickable or mud-ripple classes
            var chipset = comp.FindComponent<MudChipSet<string>>();
            comp.FindAll("div.mud-clickable").Count.Should().Be(0);
            comp.FindAll("div.mud-ripple").Count.Should().Be(0);

            //Click test
            comp.FindAll(".mud-chip")[0].TagName.Should().Be("DIV");

            //Should not throw an error because it won't click
            await comp.FindAll("button.mud-chip-close-button")[0].ClickAsync();

            chipset.Instance.SelectedValue.Should().Be(null);
        }

        /// <summary>
        /// In this test component two chipsets are synchronized via a single selectedValues collection
        /// Whenever one ChipSet changes the other must update to the same selection state.
        /// </summary>
        [Test]
        public async Task ChipSet_SelectedValues_TwoWayBinding()
        {
            var comp = Context.Render<ChipSetSelectionTwoWayBindingTest>();
            // initial values check
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 1");
            comp.FindComponents<MudChip<int>>()[0].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");

            // change selection and check state of both sets
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("p.set").TrimmedText().Should().Be("Selection:");
            comp.FindComponents<MudChip<int>>()[0].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            await comp.FindAll("button.mud-chip")[1].ClickAsync();
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 2");
            comp.FindComponents<MudChip<int>>()[0].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            await comp.FindAll("button.mud-chip")[2].ClickAsync();
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 1, 2");
            comp.FindComponents<MudChip<int>>()[0].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            await comp.FindAll("button.mud-chip")[3].ClickAsync();
            comp.Find("p.set").TrimmedText().Should().Be("Selection: 1");
            comp.FindComponents<MudChip<int>>()[0].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[1].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[2].Find(".mud-chip").ClassList.Should().Contain("mud-chip-selected");
            comp.FindComponents<MudChip<int>>()[3].Find(".mud-chip").ClassList.Should().NotContain("mud-chip-selected");
        }

        [Test]
        public async Task ChipSetComparer()
        {
            var comp = Context.Render<ChipSetComparerTest>();
            // initial values check
            comp.Find("p.sel").TrimmedText().Should().Be("Selection:");

            // change selection and check state
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cappuccino");

            // set new selection and see if the comparer works correctly
            await comp.FindComponent<MudButton>().Find("button").ClickAsync();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cafe Latte!, Espresso!");

            // change selection and check state
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cafe Latte!, Cappuccino, Espresso!");

            // change selection and check state
            await comp.FindAll("button.mud-chip")[1].ClickAsync();
            comp.Find("p.sel").TrimmedText().Should().Be("Selection: Cappuccino, Espresso!");
        }

        [Test]
        public async Task ChipSet_MultiSelection_AfterChipArraySetNull_ShouldBeAbleToSelectSameChip()
        {
            var comp = Context.Render<ChipSetClearSelectionTest>();
            var chipSet = comp.FindComponent<MudChipSet<string>>();

            // Select one chip
            await comp.FindAll("button.mud-chip")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");

            // Set chip array to null
            await comp.FindAll("button")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => chipSet.Instance.SelectedValues.Count.Should().Be(0));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");

            // Select same chip again
            await comp.FindAll("button.mud-chip")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
        }

        [Test]
        public async Task ChipSet_MultiSelection_AfterChipArraySetEmpty_ShouldBeAbleToSelectSameChip()
        {
            var comp = Context.Render<ChipSetClearSelectionTest>();
            var chipSet = comp.FindComponent<MudChipSet<string>>();

            // Select one chip
            await comp.FindAll("button.mud-chip")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");

            // Set chip array to empty
            await comp.Find("#set-empty").ClickAsync();

            await comp.WaitForAssertionAsync(() => chipSet.Instance.SelectedValues.Count.Should().Be(0));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Nothing selected.");

            // Select same chip again
            await comp.FindAll("button.mud-chip")[0].ClickAsync();

            await comp.WaitForAssertionAsync(() => chipSet.Instance.SelectedValues.Count.Should().Be(1));
            comp.FindAll("p")[0].TrimmedText().Should().Be("Milk");
        }

        [Test]
        public void Chip_GetValue_ShouldReturnTextIfValueIsNullAndT_IsString()
        {
            // Backwards compatibility with non-generic chips where setting the Text without a Value treated the Text as Value
            Context.Render<MudChip<string>>(p => p
                .Add(x => x.Text, "はい")
            ).Instance.GetValue().Should().Be("はい");
            Context.Render<MudChip<string>>(p => p
                .Add(x => x.Text, "はい")
                .Add(x => x.Value, "Yes")
            ).Instance.GetValue().Should().Be("Yes");
            // Not for types != string though!
            Context.Render<MudChip<int?>>(p => p
                .Add(x => x.Text, "Zero")
            ).Instance.GetValue().Should().Be(null);
            Context.Render<MudChip<int?>>(p => p
                .Add(x => x.Text, "Zero")
                .Add(x => x.Value, 0)
            ).Instance.GetValue().Should().Be(0);
        }

        [Test]
        public async Task ChipSet_CheckMark_Parameter()
        {
            var comp = Context.Render<MudChipSet<string>>(self => self
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
            await comp.SetParametersAndRenderAsync(self => self.Add(x => x.CheckMark, false));
            comp.FindAll("svg").Count.Should().Be(0);
            comp.Instance.CheckMark.Should().Be(false);
            comp.Instance.CheckMark.Should().Be(false);
            // for coverage
            new MudChip<int>().ShowCheckMark.Should().Be(false);
            var chip = Context.Render<MudChip<string>>(chip => chip
                .Add(x => x.CheckedIcon, Icons.Material.Filled.Cake)
                .Add(x => x.CloseIcon, Icons.Material.Filled.Plagiarism)
                .Add(x => x.Ripple, false)
                .Add(x => x.IconColor, Color.Error)
                .Add(x => x.Selected, true)
            ).Instance;
            await comp.InvokeAsync(() => chip.UpdateSelectionStateAsync(true));
            chip.ShowCheckMark.Should().Be(false); // because not in a chipset
            Context.Render<MudChip<int>>(self => self.Add(x => x.Variant, (Variant)69)).Instance.GetVariant().Should().Be(Variant.Outlined); // falls back to outlined
        }

        [Test]
        public async Task ChipSet_RemoveChip_Logic()
        {
            IReadOnlyCollection<string> selectedValues = ["x", "y", "z"];
            var comp = Context.Render<MudChipSet<string>>(self => self
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Bind(x => x.SelectedValues, selectedValues, x => selectedValues = x)
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "x"))
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "y"))
                .AddChildContent<MudChip<string>>(chip => chip.Add(x => x.Value, "z"))
            );
            await comp.InvokeAsync(() => comp.Instance.RemoveAsync(comp.FindComponent<MudChip<string>>().Instance));
            string.Join(", ", selectedValues).Should().Be("y, z");
            // removing a foreign chip doesn't do anything
            await comp.Instance.RemoveAsync(Context.Render<MudChip<string>>(chip => chip.Add(x => x.Value, "y")).Instance);
            string.Join(", ", selectedValues).Should().Be("y, z");
            // removing from a disposed chipset doesn't raise events, so in this case the selection stays the same
            var chipY = comp.FindComponent<MudChip<string>>().Instance;
            comp.Instance.Dispose();
            await comp.Instance.RemoveAsync(chipY);
            string.Join(", ", selectedValues).Should().Be("y, z");
        }

        [Test]
        public async Task ChipSet_With_NonValueTypes_DoesntCrash()
        {
            var a = new object();
            var b = new object();
            var c = new object();
            IReadOnlyCollection<object> selectedValues = [a];
            var comp = Context.Render<MudChipSet<object>>(self => self
                .Add(x => x.SelectionMode, SelectionMode.MultiSelection)
                .Bind(x => x.SelectedValues, selectedValues, x => selectedValues = x)
                .AddChildContent<MudChip<object>>(chip => chip.Add(x => x.Value, a))
                .AddChildContent<MudChip<object>>(chip => chip.Add(x => x.Value, b))
                .AddChildContent<MudChip<object>>(chip => chip.Add(x => x.Value, c))
            );
            await comp.FindAll("button.mud-chip")[1].ClickAsync();
            selectedValues.Should().Contain(a).And.Contain(b);
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            selectedValues.Should().NotContain(a).And.Contain(b);
        }

        [Test]
        public async Task Chip_TwoWayBinding_ShouldUpdateSelection()
        {
            var comp = Context.Render<ChipSetChipBindingTest>();
            comp.Find("div.selection").TrimmedText().Should().Be("Add ingredients to your cocktail.");
            // initial state
            comp.FindAll(".mud-chip")[0].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll(".mud-chip")[2].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-false");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-false");

            // click Vodka chip
            await comp.FindAll("button.mud-chip")[0].ClickAsync();
            comp.Find("div.selection").TrimmedText().Should().Be("Vodka");
            comp.FindAll(".mud-chip")[0].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll(".mud-chip")[2].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-true");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-false");

            // click Olive checkbox
            await comp.FindAll("input.mud-checkbox-input")[2].ChangeAsync(true);
            comp.Find("div.selection").TrimmedText().Should().Be("Olive, Vodka");
            comp.FindAll(".mud-chip")[0].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll(".mud-chip")[2].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-true");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-true");

            // click Vodka checkbox
            await comp.FindAll("input.mud-checkbox-input")[0].ChangeAsync(false);
            comp.Find("div.selection").TrimmedText().Should().Be("Olive");
            comp.FindAll(".mud-chip")[0].ClassList.Should().NotContain("mud-chip-selected");
            comp.FindAll(".mud-chip")[2].ClassList.Should().Contain("mud-chip-selected");
            comp.FindAll(".mud-checkbox span")[0].ClassList.Should().Contain("mud-checkbox-false");
            comp.FindAll(".mud-checkbox span")[2].ClassList.Should().Contain("mud-checkbox-true");
        }

        [Test]
        public async Task Should_provide_accessible_keyboard_navigation()
        {
            var onCloseCount = 0;
            var comp = Context.Render<ChipSetKeyboardNavigationTests>(parameters => parameters
                .Add(p => p.AreChipsClosable, false)
                .Add(p => p.OnClose, () => onCloseCount++));

            // add two chips
            await comp.Find("#add-chip-button").ClickAsync();
            await comp.Find("#add-chip-button").ClickAsync();

            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().BeNullOrEmpty();
            comp.FindComponents<MudChip<string>>().Should().HaveCount(2);

            // pressing a chip using Space or Enter should toggle their state
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = " " });
            // comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.Find("#chip-2").ClickAsync(); // https://github.com/MudBlazor/MudBlazor/pull/10488#issuecomment-2558409773
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(2);

            // pressing the Delete or Backspace keys should have no impact when the chips are not closable
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = "Delete" });
            await comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });
            onCloseCount.Should().Be(0);
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(2);

            // re-pressing a chip with Space or Enter should un-toggle their state
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = " " });
            // comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.Find("#chip-2").ClickAsync(); // https://github.com/MudBlazor/MudBlazor/pull/10488#issuecomment-2558409773
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().BeNullOrEmpty();

            // toggle the chips again, then delete them (the chipset should no longer consider them part of its group, and remove them from selected values)
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.AreChipsClosable, true));
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = " " });
            // comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            await comp.Find("#chip-2").ClickAsync(); // https://github.com/MudBlazor/MudBlazor/pull/10488#issuecomment-2558409773
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(2);

            // pressing the Delete or Backspace keys should remove the chips from the chipset now that they are closable
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = "Delete" });
            await comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });
            onCloseCount.Should().Be(2);
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Should_not_accept_keyboard_inputs_when_disabled_or_readonly()
        {
            var onCloseCount = 0;
            var comp = Context.Render<ChipSetKeyboardNavigationTests>(parameters => parameters
                .Add(p => p.AreChipsClosable, true)
                .Add(p => p.Disabled, true)
                .Add(p => p.OnClose, () => onCloseCount++));

            // add two chips
            await comp.Find("#add-chip-button").ClickAsync();
            await comp.Find("#add-chip-button").ClickAsync();

            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().BeNullOrEmpty();
            comp.FindComponents<MudChip<string>>().Should().HaveCount(2);

            // pressing a chip using Space or Enter shouldn't toggle their state because the set is disabled
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = " " });
            await comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(0);

            // pressing the Delete or Backspace keys should have no impact either
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = "Delete" });
            await comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });
            onCloseCount.Should().Be(0);
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(0);

            // toggle the chips again, then delete them (the chipset should no longer consider them part of its group, and remove them from selected values)
            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Disabled, false)
                .Add(p => p.ReadOnly, true));

            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().BeNullOrEmpty();
            comp.FindComponents<MudChip<string>>().Should().HaveCount(2);

            // pressing a chip using Space or Enter shouldn't toggle their state because the set is readOnly
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = " " });
            await comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(0);

            // pressing the Delete or Backspace keys should have no impact either
            await comp.Find("#chip-1").KeyDownAsync(new KeyboardEventArgs { Key = "Delete" });
            await comp.Find("#chip-2").KeyDownAsync(new KeyboardEventArgs { Key = "Backspace" });
            onCloseCount.Should().Be(0);
            comp.FindComponent<MudChipSet<string>>().Instance.SelectedValues.Should().HaveCount(0);
        }
    }
}
