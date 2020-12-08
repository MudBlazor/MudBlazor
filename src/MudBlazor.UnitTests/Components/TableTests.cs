using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class TableTests
    {
        [Test]
        [Ignore("todo")]
        public void TableSingleSelection()
        {
            // setup
            using var ctx = new Bunit.TestContext();
            //var comp = ctx.RenderComponent<TableSingleSelection>();
            // print the generated html
            //Console.WriteLine(comp.Markup);
            // select elements needed for the test
            //var group = comp.FindComponent<MudTable>();
            // ...

            // the item of the clicked row should be in SelectedItem
        }

        [Test]
        [Ignore("todo")]
        public void TableFilter()
        {
            // non-matching rows should disappear
        }

        [Test]
        [Ignore("todo")]
        public void TablePaging()
        {
            // there must not be more rows than page size
            // switch page and check the rows of the selected page
        }

        [Test]
        public void TableMultiSelectionTest1()
        {
            // the selected items (check-box click or row click) should be in SelectedItems
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<TableMultiSelectionTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(3);
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(6); // two td per row for multi selection
            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(3); // one checkbox per row
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            // click checkboxes and verify selection text
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(1);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0 }");
            inputs = comp.FindAll("input").ToArray();
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            // row click
            tr[1].Click();
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1 }");
        }

        [Test]
        public void TableMultiSelectionTest2()
        {
            // checking the header checkbox should select all items (all checkboxes on, all items in SelectedItems)
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<TableMultiSelectionTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x=>x.Instance).ToArray();
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(4); // <-- one header, three rows
            var th = comp.FindAll("th").ToArray();
            th.Length.Should().Be(2); //  one for the checkbox, one for the header
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(6); // two td per row for multi selection
            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(4); // one checkbox per row + one for the header
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            // click header checkbox and verify selection text
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(3);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(4);
            inputs = comp.FindAll("input").ToArray();
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(0);
        }

        [Test]
        public void TableMultiSelectionTest3()
        {
            // Initially the values bound to SelectedItems should be selected
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<TableMultiSelectionTest3>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(1); // selected items should be empty
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(1);
            checkboxes[0].Checked.Should().Be(false);
            checkboxes[2].Checked.Should().Be(true);
            // uncheck it
            comp.InvokeAsync(() => { 
                checkboxes[2].Checked = false;
            });
            comp.WaitForState(()=>checkboxes[2].Checked==false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(0);
        }

        [Test]
        public void TableMultiSelectionTest4()
        {
            //The checkboxes should all be checked on load, even the header checkbox.
            // setup
            using var ctx = new Bunit.TestContext();
            var comp = ctx.RenderComponent<TableMultiSelectionTest4>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(3); 
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(4);
            // uncheck only row 1 => header checkbox should be off then
            comp.InvokeAsync(() => {
                checkboxes[2].Checked = false;
            });
            comp.WaitForState(() => checkboxes[2].Checked == false);
            checkboxes[0].Checked.Should().Be(false); // header checkbox should be off
            table.SelectedItems.Count.Should().Be(2);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(2);
        }

        [Test]
        public void TableMultiSelectionTest5()
        {
            // Paging should not influence multi-selection
            // setup
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<TableMultiSelectionTest5>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(4); 
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2, 3 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(3);
            // uncheck a row then switch to page 2 and both checkboxes on page 2 should be checked
            comp.InvokeAsync(() => checkboxes[1].Checked = false);
            comp.WaitForState(() => checkboxes[1].Checked == false);
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(1);
            // switch page 
            comp.InvokeAsync(() => table.CurrentPage = 1);
            comp.WaitForState(() => table.CurrentPage == 1);
            // now two checkboxes should be checked on page 2
            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(2);
        }
    }
}
