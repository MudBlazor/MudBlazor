using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bunit;
using FluentAssertions;
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
        }

        [Test]
        [Ignore("todo")]
        public void TableMultiSelection2()
        {
            // checking the header checkbox should select all items (all checkboxes on, all items in SelectedItems)
        }
    }
}
