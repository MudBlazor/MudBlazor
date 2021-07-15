﻿#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class TableTests
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
        /// OnRowClick event callback should be fired regardless of the selection state
        /// </summary>
        [Test]
        public void TableRowClick()
        {
            var comp = ctx.RenderComponent<TableRowClickTest>();
            Console.WriteLine(comp.Markup);
            comp.Find("p").TextContent.Trim().Should().BeEmpty();
            var trs = comp.FindAll("tr");
            trs[1].Click();
            comp.Find("p").TextContent.Trim().Should().Be("0");
            trs[1].Click();
            comp.Find("p").TextContent.Trim().Should().Be("0,0");
            trs[2].Click();
            comp.Find("p").TextContent.Trim().Should().Be("0,0,1");
            trs[0].Click(); // clicking the header should add -1
            comp.Find("p").TextContent.Trim().Should().Be("0,0,1,-1");
            trs[4].Click(); // clicking the header should add 100
            comp.Find("p").TextContent.Trim().Should().Be("0,0,1,-1,100");
        }

        /// <summary>
        /// Show that sorting is disabled
        /// </summary>
        [Test]
        public async Task TableDisabledSort()
        {
            // Get access to the table
            var comp = ctx.RenderComponent<TableDisabledSortTest>();

            // Count the number of rows including header
            comp.FindAll("tr").Count.Should().Be(4); // Three rows + header row

            // Check the values of rows
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            // Access to the table
            var table = comp.FindComponent<MudTable<string>>();

            // Get the mudtablesortlabels associated to the table
            var mudTableSortLabels = table.Instance.Context.SortLabels;

            // Sort the first column
            await table.InvokeAsync(() => mudTableSortLabels[0].ToggleSortDirection());

            // Check the values of rows
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("A");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("B");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            // Sort the first column
            await table.InvokeAsync(() => mudTableSortLabels[0].ToggleSortDirection());

            // Check the values of rows
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("B");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("A");

            // Disabled the sorting of the column
            mudTableSortLabels[0].Enabled = false;

            // Sort the first column
            await table.InvokeAsync(() => mudTableSortLabels[0].ToggleSortDirection());

            // The values remain the same
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("B");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("A");
        }

        /// Check if the loading parameter is adding a supplementary row.
        /// </summary>
        [Test]
        public void TableLoadingTest()
        {
            var comp = ctx.RenderComponent<TableLoadingTest>();

            // Count the number of rows
            var trs = comp.FindAll("tr");

            // It should be equal to 3 = two rows + header row
            trs.Count.Should().Be(3);

            // Find the loading switch
            var switchElement = comp.Find("#switch");

            // Click the loading switch
            switchElement.Change(true);

            // Count the number of rows
            trs = comp.FindAll("tr");

            // It should be equal to 4 = two rows + header row + loading row
            trs.Count.Should().Be(4);
        }

        /// Check if if empty row text is correct
        /// </summary>
        [Test]
        public void TableHeadContentTest()
        {
            var comp = ctx.RenderComponent<TableLoadingTest>();
            var searchString = comp.Find("#searchString");
            var switchElement = comp.Find("#switch");

            // It should be equal to 3 = two rows + header row
            comp.FindAll("tr").Count.Should().Be(3);

            // Filter out all table rows
            searchString.Change("ZZZ");

            // It should be equal to 2 = two rows + header row
            comp.FindAll("tr").Count.Should().Be(2);
            comp.FindAll("tr")[1].TextContent.Should().Be("No matching records found");

            // It should be equal to 3 = empty row string + header row + loading row
            switchElement.Change(true); 
            comp.FindAll("tr").Count.Should().Be(3);
            comp.FindAll("tr")[2].TextContent.Should().Be("Loading...");
        }

        /// <summary>
        /// should only be able to select one item and selecteditems.count should never exceed 1
        /// </summary>
        [Test]
        public void TableSingleSelection()
        {
            var comp = ctx.RenderComponent<TableSingleSelectionTest1>();
            // print the generated html      
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int?>>().Instance;
            table.SelectedItem.Should().BeNull();
            table.SelectedItems.Count.Should().Be(0);
            var trs = comp.FindAll("tr");
            // Click on row 1 (index 0)
            trs[0].Click();
            // Check SelectedItem and SelectedItems count
            table.SelectedItem.Should().Be(0);
            table.SelectedItems.Count.Should().Be(1);
            table.SelectedItems.First().Should().Be(0);
            // Repeat
            trs[2].Click();
            table.SelectedItem.Should().Be(2);
            table.SelectedItems.Count.Should().Be(1);
            table.SelectedItems.First().Should().Be(2);
        }

        /// <summary>
        /// test filtereditems and rendering without pager
        /// </summary>
        [Test]
        public void TableFilter()
        {
            var comp = ctx.RenderComponent<TableFilterTest1>();
            // print the generated html      
            Console.WriteLine(comp.Markup);
            var table = comp.FindComponent<MudTable<string>>().Instance;
            var searchString = comp.Find("#searchString");
            // should return 3 items
            searchString.Change("Ala");
            table.GetFilteredItemsCount().Should().Be(3);
            string.Join(",", table.FilteredItems).Should().Be("Alabama,Alaska,Palau");
            comp.FindAll("tr").Count.Should().Be(3);
            // no matches
            searchString.Change("ZZZ");
            table.GetFilteredItemsCount().Should().Be(0);
            table.FilteredItems.Count().Should().Be(0);
            comp.FindAll("tr").Count.Should().Be(0);
            // should return 1 item
            searchString.Change("Alaska");
            table.GetFilteredItemsCount().Should().Be(1);
            table.FilteredItems.First().Should().Be("Alaska");
            comp.FindAll("tr").Count.Should().Be(1);
            // clear search
            searchString.Change(string.Empty);
            table.GetFilteredItemsCount().Should().Be(59);
            comp.FindAll("tr").Count.Should().Be(59);
        }

        /// <summary>
        /// simple navigation using the paging buttons
        /// </summary>
        [Test]
        public void TablePagingNavigationButtons()
        {
            var comp = ctx.RenderComponent<TablePagingTest1>();
            // print the generated html      
            Console.WriteLine(comp.Markup);
            // after initial load
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            var pagingButtons = comp.FindAll("button");
            // click next page
            pagingButtons[2].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("11-20 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // last page
            pagingButtons[3].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(9);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("51-59 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            // previous page
            pagingButtons[1].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("41-50 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // first page
            pagingButtons[0].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
        }

        /// <summary>
        /// page size select tests
        /// </summary>
        [Test]
        public async Task TablePagingChangePageSize()
        {
            var comp = ctx.RenderComponent<TablePagingTest1>();
            // print the generated html      
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<string>>();
            var pager = comp.FindComponent<MudSelect<string>>().Instance;
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(20));
            pager.Value.Should().Be("20");
            comp.FindAll("tr.mud-table-row").Count.Should().Be(20);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-20 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(60));
            pager.Value.Should().Be("60");
            comp.FindAll("tr.mud-table-row").Count.Should().Be(59);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-59 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(10));
            pager.Value.Should().Be("10");
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
        }

        /// <summary>
        /// page size select after paging tests
        /// </summary>
        [Test]
        public async Task TablePagingChangePageSizeAfterPaging()
        {
            var comp = ctx.RenderComponent<TableServerSideDataTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>();
            var pager = comp.FindComponent<MudSelect<string>>().Instance;
            // after initial load
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("3");
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 99");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // last page
            comp.FindAll("div.mud-table-pagination-actions button")[3].Click(); // last >
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("28");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("29");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("30");
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("91-99 of 99");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(100));
            pager.Value.Should().Be("100");
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-99 of 99");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
        }

        /// <summary>
        /// simple filter with pager
        /// </summary>
        [Test]
        public async Task TablePagingFilter()
        {
            var comp = ctx.RenderComponent<TablePagingTest1>();
            var searchString = comp.Find("#searchString");
            // search returns 3 items
            searchString.Change("Ala");
            comp.FindAll("tr").Count.Should().Be(3);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-3 of 3");
            // clear search
            searchString.Change(string.Empty);
            comp.FindAll("tr").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
        }

        /// <summary>
        /// adjust current page when filtereditems.count is less than the current page start item index
        /// </summary>
        [Test]
        public async Task TablePagingFilterAdjustCurrentPage()
        {
            var comp = ctx.RenderComponent<TablePagingTest1>();
            // print the generated html      
            Console.WriteLine(comp.Markup);
            // after initial load
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            var pagingButtons = comp.FindAll("button");
            // goto page 3
            pagingButtons[2].Click();
            pagingButtons[2].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("21-30 of 59");
            // should return 3 items and 
            var table = comp.FindComponent<MudTable<string>>().Instance;
            var searchString = comp.Find("#searchString");
            searchString.Change("Ala");
            table.GetFilteredItemsCount().Should().Be(3);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(3);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-3 of 3");
            searchString.Change(string.Empty);
            table.GetFilteredItemsCount().Should().Be(59);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
        }

        /// <summary>
        /// setting the selecteditems to null should create a new selecteditems collection
        /// </summary>
        [Test]
        public void TableMultiSelectionSelectedItemsEqualsNull()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            // click checkboxes and verify selection text
            var inputs = comp.FindAll("input").ToArray();
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(1);
            comp.InvokeAsync(() => { table.SelectedItems = null; });
            table.SelectedItems.Count.Should().Be(0);
        }

        /// <summary>
        /// the selected items (check-box click or row click) should be in SelectedItems
        /// </summary>
        [Test]
        public void TableMultiSelectionTest1()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest1>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
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

        /// <summary>
        /// checking the header checkbox should select all items (all checkboxes on, all items in SelectedItems)
        /// </summary>
        [Test]
        public void TableMultiSelectionTest2()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest2>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
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

        /// <summary>
        /// checking the header checkbox should select all items (all checkboxes on, all items in SelectedItems) with multiheader
        /// </summary>
        [Test]
        public void TableMultiSelectionTest2B()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest2B>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
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

        /// <summary>
        /// Initially the values bound to SelectedItems should be selected
        /// </summary>
        [Test]
        public void TableMultiSelectionTest3()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest3>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxRendered = comp.FindComponents<MudCheckBox<bool>>().ToArray();
            var checkboxes = checkboxRendered.Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(1); // selected items should be empty
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(1);
            checkboxes[0].Checked.Should().Be(false);
            checkboxes[2].Checked.Should().Be(true);
            // uncheck it
            checkboxRendered[2].Find("input").Change(false);
            // check result
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// The checkboxes should all be checked on load, even the header checkbox.
        /// </summary>
        [Test]
        public void TableMultiSelectionTest4()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest4>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxRendered = comp.FindComponents<MudCheckBox<bool>>().ToArray();
            var checkboxes = checkboxRendered.Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(3);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(4);
            // uncheck only row 1 => header checkbox should be off then
            checkboxRendered[2].Find("input").Change(false);
            checkboxes[0].Checked.Should().Be(false); // header checkbox should be off
            table.SelectedItems.Count.Should().Be(2);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(2);
        }

        /// <summary>
        /// Paging should not influence multi-selection
        /// </summary>
        [Test]
        public async Task TableMultiSelectionTest5()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest5>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxRendered = comp.FindComponents<MudCheckBox<bool>>().ToArray();
            var checkboxes = checkboxRendered.Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(4);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2, 3 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(3);
            // uncheck a row then switch to page 2 and both checkboxes on page 2 should be checked
            checkboxRendered[1].Find("input").Change(false);
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(1);
            // switch page 
            await comp.InvokeAsync(() => table.CurrentPage = 1);
            // now two checkboxes should be checked on page 2
            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(2);
        }

        /// <summary>
        /// checking the footer checkbox should select all items (all checkboxes on, all items in SelectedItems) with multiheader
        /// </summary>
        [Test]
        public void TableMultiSelectionTest6()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest6>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(5); // <-- one header, three rows, one footer
            var th = comp.FindAll("th").ToArray();
            th.Length.Should().Be(2); //  one for the checkbox, one for the header
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(8); // two td per row for multi selection + two for footer
            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(5); // one checkbox per row + one for the header + 1 for the footer
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            // click footer checkbox and verify selection text
            inputs[4].Change(true);
            table.SelectedItems.Count.Should().Be(3);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(5);
            inputs = comp.FindAll("input").ToArray();
            inputs[4].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// checking the footer checkbox should select all items (all checkboxes on, all items in SelectedItems) with multiheader
        /// </summary>
        [Test]
        public void TableMultiSelectionTest6B()
        {
            var comp = ctx.RenderComponent<TableMultiSelectionTest6B>();
            // print the generated html
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(5); // <-- one header, three rows
            var th = comp.FindAll("th").ToArray();
            th.Length.Should().Be(2); //  one for the checkbox, one for the header
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(8); // two td per row for multi selection + 2 footer
            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(5); // one checkbox per row + one for the header + 1 for the footer
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            // click footer checkbox and verify selection text
            inputs[4].Change(true);
            table.SelectedItems.Count.Should().Be(3);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(5);
            inputs = comp.FindAll("input").ToArray();
            inputs[4].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Checked ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// Setting items delayed should work well and update pager also
        /// </summary>
        [Test]
        public async Task TablePaginationTest1()
        {
            var comp = ctx.RenderComponent<TablePaginationTest1>();
            await Task.Delay(200);
            Console.WriteLine(comp.Markup);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(11); // ten rows + header row
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");
        }

        /// <summary>
        /// Even without a MudTablePager the table should call ServerReload to get the items on start.
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest1()
        {
            var comp = ctx.RenderComponent<TableServerSideDataTest1>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("3");
        }

        /// <summary>
        /// The table should call ServerReload to get the items for the current page according to MudTablePager
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest2()
        {
            var comp = ctx.RenderComponent<TableServerSideDataTest2>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("3");
            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("4");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("5");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("6");
            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("7");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("8");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("9");
            comp.FindAll("div.mud-table-pagination-actions button")[0].Click(); // |<
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("3");
        }

        /// <summary>
        /// The server-side loaded table should reflect initial sort direction in its initial table state.
        /// In this case, the items should be sorted with descending order.
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest3()
        {
            var comp = ctx.RenderComponent<TableServerSideDataTest3>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("3");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("1");
            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("6");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("5");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("4");
            comp.FindAll("div.mud-table-pagination-actions button")[0].Click(); // |<
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("3");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("1");
        }

        /// <summary>
        /// The server-side loaded table should reload when mobile sort if performed
        /// (IEnumerable variation).
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest4()
        {
            var comp = ctx.RenderComponent<TableServerSideDataTest4>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("3");
            comp.FindAll("div.mud-select-input")[0].Click(); // mobile sort drop down
            comp.FindAll("div.mud-list-item-clickable")[1].Click(); // sort b column
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("3");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("1");
        }

        /// <summary>
        /// The server-side loaded table should reload when mobile sort if performed
        /// (IQueryable variation).
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest4b()
        {
            var comp = ctx.RenderComponent<TableServerSideDataTest4b>();
            Console.WriteLine(comp.Markup);
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("3");
            comp.FindAll("div.mud-select-input")[0].Click(); // mobile sort drop down
            comp.FindAll("div.mud-list-item-clickable")[1].Click(); // sort b column
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("3");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("1");
        }

        /// <summary>
        /// The table should render the classes and style to the tr using the RowStyleFunc and RowClassFunc parameters
        /// </summary>
        [Test]
        public async Task TableRowClassStyleTest()
        {
            var comp = ctx.RenderComponent<TableRowClassStyleTest>();
            Console.WriteLine(comp.Markup);
            var trs = comp.FindAll("tr");
            trs.Count.Should().Be(5); // four rows + header row

            var tds = comp.FindAll("td");
            tds[0].TextContent.Trim().Should().Be("0");
            tds[1].TextContent.Trim().Should().Be("1");
            tds[2].TextContent.Trim().Should().Be("2");
            tds[3].TextContent.Trim().Should().Be("3");

            trs[1].GetAttribute("style").Contains("color: red");
            trs[2].GetAttribute("style").Contains("color: red");
            trs[3].GetAttribute("style").Contains("color: blue");
            trs[4].GetAttribute("style").Contains("color: blue");

            trs[1].GetAttribute("class").Contains("even");
            trs[2].GetAttribute("class").Contains("odd");
            trs[3].GetAttribute("class").Contains("even");
            trs[4].GetAttribute("class").Contains("odd");
        }

        public class TableRowValidatorTest : TableRowValidator
        {
            public int ControlCount => _formControls.Count;
        }

        [Test]
        public async Task TableInlineEdit_TableRowValidator()
        {
            var comp = ctx.RenderComponent<TableInlineEditTest>();
            var validator = new TableRowValidatorTest();
            comp.Instance.Table.Validator = validator;

            Console.WriteLine(comp.Markup);

            var trs = comp.FindAll("tr");
            trs.Count.Should().Be(4); // three rows + header row

            trs[1].Click();
            //every item will be add twice - see MudTextField.razor
            validator.ControlCount.Should().Be(2);
            for (int i = 0; i < 10; ++i)
            {
                trs[i % 3 + 1].Click();
            }
            validator.ControlCount.Should().Be(2);
        }

        /// <summary>
        /// This test validates the processing of the Commit and Cancel buttons for an inline editing table.
        /// </summary>
        [Test]
        public async Task TableInlineEditCancelTest()
        {
            var comp = ctx.RenderComponent<TableInlineEditCancelTest>();

            // Check that the value in the second row is equal to 'B'
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("B");

            // Click on the second row
            var trs = comp.FindAll("tr");
            trs[2].Click();

            // Find the textfield and change the value to 'C'
            comp.Find("#Id2").Change("C");

            // Click the commit button
            var commitButton = comp.Find("button");
            commitButton.Click();

            // Value in the second row should be now equal to 'C'
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            // Click on the second row
            trs[2].Click();

            // Find the textfield and change the value to 'D'
            comp.Find("#Id2").Change("D");

            // Click the cancel button
            var cancelButton = comp.FindAll("button")[1];
            cancelButton.Click();

            // Value in the second row should still be equal to 'C'
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("C");
        }

        /// <summary>
        /// This test validates the processing of the Commit and Cancel buttons for an inline editing table.
        /// </summary>
        [Test]
        public async Task TableInlineEditCancel2Test()
        {
            var comp = ctx.RenderComponent<TableInlineEditCancelTest>();

            // Check that the value in the second row is equal to 'B'
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("B");

            // Click on the second row
            var trs = comp.FindAll("tr");
            trs[2].Click();

            // Find the textfield and change the value to 'Z'
            comp.Find("#Id2").Change("Z");

            // Click on the first row
            trs[1].Click();

            // Click on the second row
            trs[2].Click();

            // Click the cancel button
            var cancelButton = comp.FindAll("button")[1];
            cancelButton.Click();

            // Value in the second row should still be equal to 'B'
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("B");
        }

        /// <summary>
        /// This test validates the behavior of RowEditPreview. It should run after SelectedItem has been updated.
        /// </summary>
        [Test]
        public async Task TableInlineEditCancel3Test()
        {
            var comp = ctx.RenderComponent<TableInlineEditCancelTest>();
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // Get the table and define the RowEditPreview method 
            var instance = comp.Instance;
            var table = instance.Table;
            table.RowEditPreview = RowEditPreview;

            // Click on the second row to trigger the RowEditPreview method
            var trs = comp.FindAll("tr");
            trs[2].Click();

            void RowEditPreview(object item)
            {
                // Get the value of the SelectedItem
                string selectedItemValue = table.SelectedItem.Value;

                // Get the value of the object from the RowEditPreview method
                var rowEditPreviewValue = item.GetType().GetProperty("Value").GetValue(item, null).ToString();

                // Compare these values are equal and are correct
                if (selectedItemValue == "B" && rowEditPreviewValue == selectedItemValue)
                {
                    // Return  a success
                    taskCompletionSource.SetResult(true);
                }
                else
                {
                    // Return a failure
                    taskCompletionSource.SetResult(false);
                }
            }

            // Wait for the result during one second maximum
            // It should be true meaning that SelecteItem had  the correct value before RowEditPreview has finished to complete
            // Also the object in RowEditPreview and the SelectedItem should be equal
            var result = taskCompletionSource.Task.Wait(1000);

            // Check that the result should be true
            result.Should().Be(true);
        }

        /// <summary>
        /// This test validates that when the CanCancel option is set to true and no SelectedItem has been defined, 
        /// by clicking on another row, the previous row is no longer editable. Meaning there are always only 2 buttons
        /// </summary>
        [Test]
        public async Task TableInlineEditCancel4Test()
        {
            // Get access to the test table
            var comp = ctx.RenderComponent<TableInlineEditCancel2Test>();

            // List all the rows
            var trs = comp.FindAll("tr");

            // Click on the third row
            trs[3].Click();

            // How many buttons? Should be 2
            comp.FindAll("button").Count.Should().Be(2);

            // Click on the second row
            trs[2].Click();

            // How many buttons? Should be 2
            comp.FindAll("button").Count.Should().Be(2);

            // Click on the first row
            trs[1].Click();

            // How many buttons? Should be 2
            comp.FindAll("button").Count.Should().Be(2);
        }

        /// <summary>
        /// Tests the grouping behavior and ensure that it won't break anything else.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TableGroupingTest()
        {
            // without grouping, to ensure that anything was broken:
            var comp = ctx.RenderComponent<TableGroupingTest>();
            var table = comp.Instance.tableInstance;
            table.Context.HeaderRows.Count.Should().Be(1);
            table.Context.GroupRows.Count.Should().Be(0);
            table.Context.Rows.Count.Should().Be(9);

            // now, with multi selection:
            table.MultiSelection = true;
            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(10);
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(9);
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            //group by Racing Category:
            comp = ctx.RenderComponent<TableGroupingTest>();
            table = comp.Instance.tableInstance;
            table.GroupBy = new TableGroupDefinition<TableGroupingTest.RacingCar>(rc => rc.Category, null) { GroupName = "Category" };
            comp.Render();
            table.Context.GroupRows.Count.Should().Be(4);
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(18); // 1 table header + 4 group headers + 9 item rows + 4 group footers

            // multi selection:
            table.MultiSelection = true;
            inputs = comp.FindAll("input").ToArray();

            inputs[1].Change(true); // selecting only LMP1 category
            table.SelectedItems.Count.Should().Be(2); // only one porsche and one audi
            inputs[1].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            inputs[4].Change(true); // selecting only GTE category
            table.SelectedItems.Count.Should().Be(3);
            inputs[4].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            inputs[0].Change(true); // all
            table.SelectedItems.Count.Should().Be(9);
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            //group by Racing Category and Brand:
            comp = ctx.RenderComponent<TableGroupingTest>();
            table = comp.Instance.tableInstance;
            table.GroupBy = new TableGroupDefinition<TableGroupingTest.RacingCar>()
            {
                GroupName = "Category",
                Selector = rc => rc.Category,
                InnerGroup = new TableGroupDefinition<TableGroupingTest.RacingCar>()
                {
                    GroupName = "Brand",
                    Selector = rc => rc.Brand 
                }
            };
            comp.Render();
            table.Context.GroupRows.Count.Should().Be(13); // 4 categories and 9 cars (can repeat on different categories)
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(36); // 1 table header + 13 group headers + 9 item rows + 13 group footers

            // multi selection:
            table.MultiSelection = true;
            inputs = comp.FindAll("input").ToArray();
            inputs[0].Change(true); // all
            table.SelectedItems.Count.Should().Be(9);
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            inputs[1].Change(true); // selecting only LMP1 category
            table.SelectedItems.Count.Should().Be(2);

            // indentation:
            table.GroupBy.Indentation = true;
            comp.Render();
            tr = comp.FindAll("tr.mud-table-row-group-indented-1").ToArray();
            tr.Length.Should().Be(27); // (4 LMP1 group (h / f) + 6 GTE + 4 GTE + 4 Formula 1) brands groups per category + 9 data rows
            tr = comp.FindAll("tr.mud-table-row-group-indented-2").ToArray();
            tr.Length.Should().Be(0); // indentation works with Level - 1 class. (level 1 doens't need to be indented)

            // expand and collpase groups:
            table.GroupBy.Indentation = false;
            table.GroupBy.Expandable = true;
            table.GroupBy.InnerGroup.Expandable = true;
            comp.Render();

            var buttons = comp.FindAll("button").ToArray();
            buttons.Length.Should().Be(13);// 4 categories and 9 cars (can repeat on different categories)
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(36); // 1 table header + 8 category group rows (h + f)  + 18 brands group rows (see line 915) + 9 car rows

            // collapsing category LMP1:
            buttons[0].Click();
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(29); // 1 table header + 8 category group rows (h + f) - LMP1 footer + 18 brands group rows (see line 915) - 2 brands LMP2 Header - 2 brands LMP1 footer + 9 car rows - 2 LMP1 car rows
            buttons[0].Click();
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(36); 
        }
    }
}
