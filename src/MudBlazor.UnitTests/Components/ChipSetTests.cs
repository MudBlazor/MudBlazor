#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
            Console.WriteLine(comp.Markup);
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
    }

}
