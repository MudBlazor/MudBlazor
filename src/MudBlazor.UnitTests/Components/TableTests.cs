#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class TableTests : BunitTest
    {
        /// <summary>
        /// OnRowClick event callback should be fired regardless of the selection state
        /// </summary>
        [Test]
        public void TableRowClick()
        {
            var comp = Context.RenderComponent<TableRowClickTest>();
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
        /// Check if the OnRowMouseEnter and OnRowMouseLeave event callbacks are fired as intended
        /// </summary>
        [Test]
        public void TableRowHover()
        {
            var comp = Context.RenderComponent<TableRowHoverTest>();
            comp.Find("p").TextContent.Trim().Should().Be("Current: '', last: ''");

            var trs = comp.FindAll("tr");

            trs[0].TriggerEvent("onpointerenter", new PointerEventArgs());
            comp.Find("p").TextContent.Trim().Should().Be("Current: 'A', last: ''");

            trs[0].TriggerEvent("onpointerleave", new PointerEventArgs());
            comp.Find("p").TextContent.Trim().Should().Be("Current: '', last: 'A'");

            trs[1].TriggerEvent("onpointerenter", new PointerEventArgs());
            comp.Find("p").TextContent.Trim().Should().Be("Current: 'B', last: 'A'");

            trs[1].TriggerEvent("onpointerleave", new PointerEventArgs());
            comp.Find("p").TextContent.Trim().Should().Be("Current: '', last: 'B'");

            trs[0].TriggerEvent("onpointerenter", new PointerEventArgs());
            comp.Find("p").TextContent.Trim().Should().Be("Current: 'A', last: 'B'");

            trs[0].TriggerEvent("onpointerleave", new PointerEventArgs());
            comp.Find("p").TextContent.Trim().Should().Be("Current: '', last: 'A'");
        }

        /// <summary>
        /// Show that sorting is disabled
        /// </summary>
        [Test]
        public async Task TableDisabledSort()
        {
            // Get access to the table
            var comp = Context.RenderComponent<TableDisabledSortTest>();

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

        /// <summary>
        /// Check if the loading parameter is adding a supplementary row.
        /// </summary>
        [Test]
        public void TableLoadingTest()
        {
            var comp = Context.RenderComponent<TableLoadingTest>();

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

        /// <summary>
        /// Ensure that when the loading switch is enabled,
        /// a new row appears in the table header without affecting the table body.
        /// </summary>
        [Test]
        public void LoadingSwitchAddsRowToHeaderWithoutAffectingBody()
        {
            // Render the component
            var comp = Context.RenderComponent<TableLoadingTest>();

            // Initial count of header and body rows
            var initialHeaderRows = comp.FindAll("thead tr");
            var initialBodyRows = comp.FindAll("tbody tr");

            // Verify initial state: 1 row in the header and 2 rows in the body
            initialHeaderRows.Count.Should().Be(1);
            initialBodyRows.Count.Should().Be(2);

            // Toggle the loading switch to the 'loading' state
            var loadingSwitch = comp.Find("#switch");
            loadingSwitch.Change(true);

            // Count rows after toggling the switch
            var updatedHeaderRows = comp.FindAll("thead tr");
            var updatedBodyRows = comp.FindAll("tbody tr");

            // Verify updated state:
            // 2 rows in the header (original + loading row) and 2 rows in the body (unchanged)
            updatedHeaderRows.Count.Should().Be(2);
            updatedBodyRows.Count.Should().Be(2);
        }

        /// <summary>
        /// Ensure that when the table loader is visible,
        /// adding new columns dynamically such as multi-selection
        /// will not affect the number of columns in the row with the loader.
        /// </summary>
        [Test]
        public void DynamicColumnsDoNotAffectLoadingRow()
        {
            // Render the component
            var comp = Context.RenderComponent<TableLoadingTest>();

            // Ensure table initially has 6 columns
            var headersRow = comp.FindAll("thead tr")[0];
            headersRow.ChildElementCount.Should().Be(6);

            // Toggle the loading switch to the 'loading' state
            var loadingSwitch = comp.Find("#switch");
            loadingSwitch.Change(true);

            // Get the loader row which is second row in the thead
            var loaderRow = comp.FindAll("thead tr")[1];

            // Verify that loader row has one child which is a loader cell
            var loaderCell = loaderRow.QuerySelector(".mud-table-loading");
            loaderCell.IsOnlyChild();

            // Toggle the multi-selection switch to the 'on' state
            var multiSelectionSwitch = comp.Find("#multi-selection");
            multiSelectionSwitch.Change(true);

            // Ensure table has 7 columns
            headersRow = comp.FindAll("thead tr")[0];
            headersRow.ChildElementCount.Should().Be(7);

            // Verify that loader row still has one child
            // which is a loader cell
            loaderCell = loaderRow.QuerySelector(".mud-table-loading");
            loaderCell.IsOnlyChild();
        }

        /// <summary>
        /// Check if the loading and no records functionality is working in grouped table.
        /// </summary>
        [Test]
        public void TableGroupLoadingAndNoRecordsTest()
        {
            var comp = Context.RenderComponent<TableGroupLoadingAndNoRecordsTest>();
            var searchString = comp.Find("#searchString");
            var switchElement = comp.Find("#switch");

            // It should be equal to 5 = header row + group header row + 2 rows + footer row
            comp.FindAll("tr").Count.Should().Be(5);

            // Add filter
            searchString.Change("ZZZ");

            // It should be equal to 2 = header row + no records row
            comp.FindAll("tr").Count.Should().Be(2);
            comp.FindAll("tr")[1].TextContent.Should().Be("No records");

            // It should be equal to 3 = header row + loading progress row + loading text
            switchElement.Change(true);
            comp.FindAll("tr").Count.Should().Be(3);
            comp.FindAll("tr")[2].TextContent.Should().Be("Loading...");

            // Remove filter
            searchString.Change("");

            // It should be equal to 6 = header row + loading progress row + group header row + 2 rows + footer row
            comp.FindAll("tr").Count.Should().Be(6);
        }

        /// <summary>
        /// Check if if empty row text is correct
        /// </summary>
        [Test]
        public void TableHeadContentTest()
        {
            var comp = Context.RenderComponent<TableLoadingTest>();
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
        [TestCase(TableEditTrigger.RowClick)]
        [TestCase(TableEditTrigger.EditButton)]
        public void TableSingleSelection(TableEditTrigger trigger)
        {
            var comp = Context.RenderComponent<TableSingleSelectionTest1>(parameters => parameters
                .Add(p => p.EditTrigger, trigger));
            // print the generated html
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
            var comp = Context.RenderComponent<TableFilterTest1>();
            // print the generated html
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

        [Test]
        public void TableFilterCachingTest()
        {
            var comp = Context.RenderComponent<TableFilterTest1>();
            // print the generated html
            var table = comp.FindComponent<MudTable<string>>().Instance;
            var searchString = comp.Find("#searchString");
            table.FilteringRunCount.Should().Be(1);
            // should return 3 items
            searchString.Change("Ala");
            table.FilteringRunCount.Should().Be(2);
            // no matches
            searchString.Change("ZZZ");
            table.FilteringRunCount.Should().Be(3);
            // should return 1 item
            searchString.Change("Alaska");
            table.FilteringRunCount.Should().Be(4);
            // clear search
            searchString.Change(string.Empty);
            table.FilteringRunCount.Should().Be(5);
        }

        /// <summary>
        /// simple navigation using the paging buttons
        /// </summary>
        [Test]
        public void TablePagingNavigationButtons()
        {
            var comp = Context.RenderComponent<TablePagingTest1>();
            // print the generated html
            // after initial load
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            IRefreshableElementCollection<IElement> PagingButtons() => comp.FindAll("button");
            // click next page
            PagingButtons()[2].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("11-20 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // last page
            PagingButtons()[3].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(9);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("51-59 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            // previous page
            PagingButtons()[1].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("41-50 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // first page
            PagingButtons()[0].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
        }

        /// <summary>
        /// navigate to page test
        /// </summary>
        [TestCase(0, "Alabama")]
        [TestCase(-1, "Alabama")]
        [TestCase(2, "Kentucky")]
        [TestCase(5, "Texas")]
        [TestCase(6, "Texas")]
        [Test]
        public async Task TableNavigateToPage(int pageIndex, string expectedFirstItem)
        {
            var comp = Context.RenderComponent<TablePagingTest1>();
            // print the generated html
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<string>>();
            //navigate to specified page
            await table.InvokeAsync(() => table.Instance.NavigateTo(pageIndex));
            comp.FindAll("tr.mud-table-row")[0].TextContent.Should().Be(expectedFirstItem);
        }

        /// <summary>
        /// page size option initial value test. Initial value should not be 10 since PageSizeOption is set to be new int[]{8, 16, 32}
        /// </summary>
        [Test]
        public async Task TablePageSizeOptions()
        {
            var comp = Context.RenderComponent<TablePageSizeOptionsTest>();
            // print the generated html
            // select elements needed for the test
            var pager = comp.FindComponent<MudSelect<int>>().Instance;
            pager.Value.Should().Be(8);
        }

        /// <summary>
        /// page size select tests
        /// </summary>
        [Test]
        public async Task TablePagingChangePageSize()
        {
            var comp = Context.RenderComponent<TablePagingTest1>();
            // print the generated html
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<string>>();
            var pager = comp.FindComponent<MudSelect<int>>().Instance;
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(20));
            pager.Value.Should().Be(20);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(20);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-20 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(60));
            pager.Value.Should().Be(60);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(59);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-59 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(10));
            pager.Value.Should().Be(10);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
        }

        /// <summary>
        /// Tests that the "All" table pager option shows all items
        /// </summary>
        [Test]
        public async Task TablePagingAllTest()
        {
            var comp = Context.RenderComponent<TablePagingTest1>();

            var table = comp.FindComponent<MudTable<string>>();
            var pager = comp.FindComponent<MudSelect<int>>().Instance;

            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59"); //check initial value
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(int.MaxValue));
            pager.Value.Should().Be(int.MaxValue);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(59);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-59 of 59");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true); //buttons are disabled
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            pager.Value.Should().Be(int.MaxValue);
        }

        /// <summary>
        /// page size select after paging tests
        /// </summary>
        [Test]
        public async Task TablePagingChangePageSizeAfterPaging()
        {
            var comp = Context.RenderComponent<TableServerSideDataTest2>();
            // print the generated html
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>();
            var pager = comp.FindComponent<MudSelect<int>>().Instance;
            // after initial load
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("3");
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 99");
            comp.FindAll("button")[0].IsDisabled().Should().Be(true);
            comp.FindAll("button")[1].IsDisabled().Should().Be(true);
            comp.FindAll("button")[2].IsDisabled().Should().Be(false);
            comp.FindAll("button")[3].IsDisabled().Should().Be(false);
            // last page
            comp.FindAll("div.mud-table-pagination-actions button")[3].Click(); // last >
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("28");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("29");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("30");
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("91-99 of 99");
            comp.FindAll("button")[0].IsDisabled().Should().Be(false);
            comp.FindAll("button")[1].IsDisabled().Should().Be(false);
            comp.FindAll("button")[2].IsDisabled().Should().Be(true);
            comp.FindAll("button")[3].IsDisabled().Should().Be(true);
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(100));
            pager.Value.Should().Be(100);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-99 of 99");
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
            var comp = Context.RenderComponent<TablePagingTest1>();
            var searchString = comp.Find("#searchString");
            // search returns 3 items
            searchString.Change("Ala");
            comp.FindAll("tr").Count.Should().Be(3);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-3 of 3");
            // clear search
            searchString.Change(string.Empty);
            comp.FindAll("tr").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
        }

        /// <summary>
        /// adjust current page when filtereditems.count is less than the current page start item index
        /// </summary>
        [Test]
        public async Task TablePagingFilterAdjustCurrentPage()
        {
            var comp = Context.RenderComponent<TablePagingTest1>();
            // print the generated html
            // after initial load
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            IRefreshableElementCollection<IElement> PagingButtons() => comp.FindAll("button");
            // goto page 3
            PagingButtons()[2].Click();
            PagingButtons()[2].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("21-30 of 59");
            // should return 3 items and
            var table = comp.FindComponent<MudTable<string>>().Instance;
            var searchString = comp.Find("#searchString");
            searchString.Change("Ala");
            table.GetFilteredItemsCount().Should().Be(3);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(3);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-3 of 3");
            searchString.Change(string.Empty);
            table.GetFilteredItemsCount().Should().Be(59);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
        }

        /// <summary>
        /// setting the selecteditems to null should create a new selecteditems collection
        /// </summary>
        [Test]
        public void TableMultiSelectionSelectedItemsEqualsNull()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest1>();
            // print the generated html
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


        [Test]
        public void TableMultiSelection_CheckboxAndRowClickTest()
        {
            var comp = Context.RenderComponent<TableMultiSelection_CheckboxAndRowClickTest>();
            var checkboxes = comp.FindComponent<MudTable<int>>().FindAll("input").ToArray();
            var table = comp.FindComponent<MudTable<int>>().Instance;

            foreach (var cbx in checkboxes) cbx.Change(true);
            table.SelectedItems.Count.Should().Be(3);

            foreach (var cbx in checkboxes) cbx.Change(false);
            table.SelectedItems.Count.Should().Be(0);
        }

        [Test]
        public void TableMultiSelection_IgnoreCheckbox_RowClickTest()
        {
            var comp = Context.RenderComponent<TableMultiSelection_IgnoreCheckbox_RowClickTest>();
            var rows = comp.FindComponent<MudTable<int>>().FindAll("tr").ToArray();
            var table = comp.FindComponent<MudTable<int>>().Instance;

            foreach (var row in rows) row.Click();
            table.SelectedItems.Count.Should().Be(0);
        }

        [Test]
        public void TableMultiSelection_MultiGrouping_DefaultCheckboxStatesTest()
        {
            var comp = Context.RenderComponent<TableMultiSelection_MultiGrouping_DefaultCheckboxStatesTest>();
            var mudTable = comp.Instance.MudTable;

            // All row checkbox states must be false.
            mudTable.Context.Rows.Count(r => r.Value.Checked).Should().Be(0);

            // All grouprow checkbox states must be false.
            mudTable.Context.GroupRows.Count(r => r.Checked.HasValue && !r.Checked.Value).Should().Be(14);

            // The headerrow checkbox state must be false.
            mudTable.Context.HeaderRows.Count(r => r.Checked.HasValue && !r.Checked.Value).Should().Be(1);

            // The footerrow checkbox state must be false.
            mudTable.Context.FooterRows.Count(r => r.Checked.HasValue && !r.Checked.Value).Should().Be(0);
        }

        /// <summary>
        /// checking the header checkbox should select all items (all checkboxes on, all items in SelectedItems)
        /// </summary>
        [Test]
        public void TableMultiSelectionTest2()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest2>();
            // print the generated html
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
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3);
            inputs = comp.FindAll("input").ToArray();
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// checking the header checkbox should select all items (all checkboxes on, all items in SelectedItems) with multiheader
        /// </summary>
        [Test]
        public void TableMultiSelectionTest2B()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest2B>();
            // print the generated html
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
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3);
            inputs = comp.FindAll("input").ToArray();
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// Initially the values bound to SelectedItems should be selected
        /// </summary>
        [Test]
        public void TableMultiSelectionTest3()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest3>();
            // print the generated html
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxRendered = comp.FindComponents<MudCheckBox<bool>>().ToArray();
            var checkboxes = checkboxRendered.Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(1); // selected items should be empty
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(1);
            checkboxes[0].Value.Should().Be(false);
            checkboxes[1].Value.Should().Be(true);
            // uncheck it
            checkboxRendered[1].Find("input").Change(false);
            // check result
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// The checkboxes should all be checked on load, even the header checkbox.
        /// </summary>
        [Test]
        public void TableMultiSelectionTest4()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest4>();
            // print the generated html
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxRendered = comp.FindComponents<MudCheckBox<bool>>().ToArray();
            var checkboxes = checkboxRendered.Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(3);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3);
            // uncheck only row 1 => header checkbox should be off then
            checkboxRendered[2].Find("input").Change(false);
            checkboxes[0].Value.Should().Be(true); // header checkbox should be on
            table.SelectedItems.Count.Should().Be(2);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(2);
        }

        /// <summary>
        /// Paging should not influence multi-selection
        /// </summary>
        [Test]
        public async Task TableMultiSelectionTest5()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest5>();
            // print the generated html
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var text = comp.FindComponent<MudText>();
            var checkboxRendered = comp.FindComponents<MudCheckBox<bool>>().ToArray();
            var checkboxes = checkboxRendered.Select(x => x.Instance).ToArray();
            table.SelectedItems.Count.Should().Be(4);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2, 3 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(2);
            // uncheck a row then switch to page 2 and both checkboxes on page 2 should be checked
            checkboxRendered[1].Find("input").Change(false);
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(1);
            // switch page
            await comp.InvokeAsync(() => table.CurrentPage = 1);
            // now two checkboxes should be checked on page 2
            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(2);
        }

        /// <summary>
        /// checking the footer checkbox should select all items (all checkboxes on, all items in SelectedItems) with multiheader
        /// </summary>
        [Test]
        public void TableMultiSelectionTest6()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest6>();
            // print the generated html
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
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3);
            inputs = comp.FindAll("input").ToArray();
            inputs[4].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// checking the footer checkbox should select all items (all checkboxes on, all items in SelectedItems) with multiheader
        /// </summary>
        [Test]
        public void TableMultiSelectionTest6B()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest6B>();
            // print the generated html
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
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3);
            inputs = comp.FindAll("input").ToArray();
            inputs[4].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// Filtering and cancelling the filter should not change checking the header checkbox, which should select all items (all checkboxes on, all items in SelectedItems)
        /// </summary>
        [Test]
        public void TableMultiSelectionTest7()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest7>();
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(4); // <-- one header, three rows
            var th = comp.FindAll("th").ToArray();
            th.Length.Should().Be(2); //  one for the checkbox, one for the header
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(6); // two td per row for multi selection
            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(5); // one checkbox per row + one for the header
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty

            inputs[4].Change("1"); // search for 1
            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // click header checkbox and verify selection text
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(1);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1 }"); // only "1" should be present
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(1);
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");

            inputs[4].Change(""); // reset to default
            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // click header checkbox and verify selection text
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(3);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2 }"); // we reset search, so all three numbers should be searched
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3);
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            comp.Find("p").TextContent.Should().Be("SelectedItems {  }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);
        }

        /// <summary>
        /// Removing rows should not uncheck the header checkbox, which should select all items (all checkboxes on, all items in SelectedItems)
        /// </summary>
        [Test]
        public void TableMultiSelectionTest8()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest8>();
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<int>>().Instance;
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // click header checkbox and verify selection text
            var inputs = comp.Find("input");
            inputs.Change(true);
            table.SelectedItems.Count.Should().Be(5);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 2, 3, 4 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(5);

            // click delete button
            var buttons = comp.FindAll("button");
            buttons[2].Click(); //delete one of the elements

            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            //verify table markup
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(5); // <-- one header, four rows
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(12); // three td per row for multi selection
            var inputs2 = comp.FindAll("input").ToArray();
            inputs2.Length.Should().Be(5); // one checkbox per row + one for the header

            //verify selection
            table.SelectedItems.Count.Should().Be(4);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 0, 1, 3, 4 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(4);

            checkboxes[0].Value.Should().BeTrue(); //manually verify header is checked after deleting item
        }

        /// <summary>
        /// The checkboxes should all be disabled on load, even the header and footer checkboxes.
        /// </summary>
        [Test]
        public void TableMultiSelectionTest9()
        {
            var comp = Context.RenderComponent<TableMultiSelectionTest9>();
            // select elements needed for the test
            var tableComponent = comp.FindComponent<MudTable<TableGroupingTest.RacingCar>>();
            var table = tableComponent.Instance;
            var rows = tableComponent.FindAll("tr").ToArray();
            var headerAndFooterCheckboxes = comp.FindComponents<MudCheckBox<bool?>>().Select(x => x.Instance).ToArray();
            var dataCheckboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();
            foreach (var row in rows.Where(el => el.ClassName.Contains("row-click-test"))) // simulate selection on row click, excluding headers and footer
                row.Click();
            // check result
            headerAndFooterCheckboxes.Sum(x => x.Disabled ? 0 : 1).Should().Be(0); // No checkbox should be enabled on header, group headers and footer
            dataCheckboxes.Sum(x => x.Disabled ? 1 : 0).Should().Be(comp.Instance.Items.Count()); // No checkbox should be enabled on rows
            table.SelectedItems.Count.Should().Be(0); // No item should be selected
        }

        /// <summary>
        /// Changing page should retain the selected items using ServerData
        /// </summary>
        [Test]
        public void TableMultiSelectionServerDataTest()
        {
            var comp = Context.RenderComponent<TableMultiSelectionServerDataTest>();
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<TableMultiSelectionServerDataTest.ComplexObject>>().Instance;
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // click header checkbox and verify selection text
            var inputs = comp.Find("input");
            inputs.Change(true);
            table.SelectedItems.Count.Should().Be(10);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(10);

            // click next page button
            var buttons = comp.FindAll("button[aria-label=\"Next page\"]");
            buttons[0].Click();

            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // verify table markup
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(11); // <-- one header, ten rows
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(10 * 6); // six td per row for multi selection
            var inputs2 = comp.FindAll("input").ToArray();
            inputs2.Length.Should().Be(11); // one checkbox per row + one for the header

            // verify selection - All items should remain selected
            table.SelectedItems.Count.Should().Be(10);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");
            // No item from current page should be checked
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);

            // Click the checkbox of the row with id 12
            inputs2[2].Change(true);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12 }");
            // One checkbox of the current page should be checked
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(1);
        }

        /// <summary>
        /// Changing page should retain the selected items using Items (not ServerData)
        /// </summary>
        [Test]
        public void TableMultiSelectionItemsTest1_PageChange()
        {
            var comp = Context.RenderComponent<TableMultiSelectionItemsTest1>();
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<TableMultiSelectionItemsTest1.ComplexObject>>().Instance;
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // Skip first two inputs (date filters)
            var inputs = comp.FindAll("input").Skip(3);
            foreach (var input in inputs)
            {
                input.Change(true);
            }
            table.SelectedItems.Count.Should().Be(10);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(10);

            // click next page button
            var buttons = comp.FindAll("button[aria-label=\"Next page\"]");
            buttons[0].Click();

            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // verify table markup
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(11); // <-- one header, ten rows
            var td = comp.FindAll("td").ToArray();
            td.Length.Should().Be(10 * 6); // six td per row for multi selection
            // Find checkboxes, and skip date filter and table header checkbox
            var inputs2 = comp.FindAll("input").Skip(3);
            inputs2.Count().Should().Be(10); // one checkbox per row + one for the header + two date filters

            // verify selection - All items should remain selected
            table.SelectedItems.Count.Should().Be(10);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");
            // No item from current page should be checked
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0);

            // Click the checkbox of the row with id 12
            inputs2.ElementAt(1).Change(true);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12 }");
            // One checkbox of the current page should be checked
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(1);
        }

        /// <summary>
        /// Changing filters should retain the selected items using Items (not ServerData)
        /// </summary>
        [Test]
        public void TableMultiSelectionItemsTest1_FilterChange()
        {
            var comp = Context.RenderComponent<TableMultiSelectionItemsTest1>();
            // select elements needed for the test
            var table = comp.FindComponent<MudTable<TableMultiSelectionItemsTest1.ComplexObject>>().Instance;
            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // Skip first two inputs (date filters)
            var inputs = comp.FindAll("input").Skip(3);
            foreach (var input in inputs)
            {
                input.Change(true);
            }
            table.SelectedItems.Count.Should().Be(10);
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(10);

            // Change filter
            comp.Instance.StartDate = DateTime.Parse("2024-04-07 00:00:00");
            comp.Instance.EndDate = DateTime.Parse("2024-04-13 00:00:00");
            comp.Render();

            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // Find checkboxes, and skip date filter and table header checkbox
            inputs = comp.FindAll("input").Skip(3);
            inputs.Count().Should().Be(5); // one checkbox per row + one for the header + two date filters
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(2);
            // Selection should remain intact
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");

            // Clear filters
            comp.Instance.StartDate = null;
            comp.Instance.EndDate = null;
            comp.Render();

            checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            // Find checkboxes, and skip date filter and table header checkbox
            inputs = comp.FindAll("input").Skip(3);
            inputs.Count().Should().Be(10); // one checkbox per row + one for the header + two date filters
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(10);
            // Selection should remain intact
            comp.Find("p").TextContent.Should().Be("SelectedItems { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }");
        }

        /// <summary>
        /// Checkbox click must not bubble up.
        /// </summary>
        [Test]
        public void TableMultiSelection_Checkbox_Executes_Callback()
        {
            var comp = Context.RenderComponent<TableMultiSelectionCheckboxExecutesCallback>();

            var table = comp.FindComponent<MudTable<int>>().Instance;
            var inputs = comp.FindAll("input").ToArray();
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            Action onclick = () => inputs[1].Click(); // OnRowClick is not called anymore, neither .GotClicked<>(), so selectedItems didn't add any element.
            onclick.Should().Throw<Bunit.MissingEventHandlerException>().WithMessage("The element does not have an event handler for the event 'onclick'. It does however have an event handler for the 'onchange' event.");
            table.SelectedItems.Count.Should().Be(0);
        }

        /// <summary>
        /// Setting items delayed should work well and update pager also
        /// </summary>
        [Test]
        public async Task TablePaginationTest1()
        {
            var comp = Context.RenderComponent<TablePaginationTest1>();
            await Task.Delay(200);
            comp.FindAll("tr.mud-table-row").Count.Should().Be(11); // ten rows + header row
            comp.FindAll("div.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");
        }

        /// <summary>
        /// Even without a MudTablePager the table should call ServerReload to get the items on start.
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest1()
        {
            var comp = Context.RenderComponent<TableServerSideDataTest1>();
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
            var comp = Context.RenderComponent<TableServerSideDataTest2>();
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
            var comp = Context.RenderComponent<TableServerSideDataTest3>();
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
            var comp = Context.RenderComponent<TableServerSideDataTest4>();
            comp.WaitForAssertion(() => comp.FindAll("tr").Count.Should().Be(4)); // three rows + header row
            comp.WaitForAssertion(() => comp.FindAll("td")[0].TextContent.Trim().Should().Be("1"));
            comp.WaitForAssertion(() => comp.FindAll("td")[2].TextContent.Trim().Should().Be("2"));
            comp.WaitForAssertion(() => comp.FindAll("td")[4].TextContent.Trim().Should().Be("3"));
            comp.FindAll("div.mud-select-input")[0].Click(); // mobile sort drop down
            comp.FindAll("div.mud-list-item-clickable")[1].Click(); // sort b column
            comp.WaitForAssertion(() => comp.FindAll("td")[0].TextContent.Trim().Should().Be("3"));
            comp.WaitForAssertion(() => comp.FindAll("td")[2].TextContent.Trim().Should().Be("2"));
            comp.WaitForAssertion(() => comp.FindAll("td")[4].TextContent.Trim().Should().Be("1"));
        }

        /// <summary>
        /// The server-side loaded table should reload when mobile sort if performed
        /// (IQueryable variation).
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest4b()
        {
            var comp = Context.RenderComponent<TableServerSideDataTest4b>();
            comp.FindAll("tr").Count.Should().Be(4); // three rows + header row
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("3");
            comp.FindAll("div.mud-select-input")[0].Click(); // mobile sort drop down
            comp.FindAll("div.mud-list-item-clickable")[1].Click(); // sort b column
            comp.WaitForAssertion(() => comp.FindAll("td")[0].TextContent.Trim().Should().Be("3"));
            comp.WaitForAssertion(() => comp.FindAll("td")[2].TextContent.Trim().Should().Be("2"));
            comp.WaitForAssertion(() => comp.FindAll("td")[4].TextContent.Trim().Should().Be("1"));
        }

        /// <summary>
        /// The server-side load callback should be called only once per page change
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest5()
        {
            var comp = Context.RenderComponent<TableServerSideDataTest5>();
            comp.Find("#counter").TextContent.Should().Be("1"); //initial counter

            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.Find("#counter").TextContent.Should().Be("2");

            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.Find("#counter").TextContent.Should().Be("3");

            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.Find("#counter").TextContent.Should().Be("4");

            comp.FindAll("div.mud-table-pagination-actions button")[1].Click(); // < previous
            comp.Find("#counter").TextContent.Should().Be("5");

            comp.FindAll("div.mud-table-pagination-actions button")[1].Click(); // < previous
            comp.Find("#counter").TextContent.Should().Be("6");

            comp.FindAll("div.mud-table-pagination-actions button")[1].Click(); // < previous
            comp.Find("#counter").TextContent.Should().Be("7");

            comp.Find("#reseter").Click(); //reset counter and test again
            comp.Find("#counter").TextContent.Should().Be("0");

            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.Find("#counter").TextContent.Should().Be("1");

            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.Find("#counter").TextContent.Should().Be("2");

            comp.FindAll("div.mud-table-pagination-actions button")[2].Click(); // next >
            comp.Find("#counter").TextContent.Should().Be("3");

            comp.FindAll("div.mud-table-pagination-actions button")[1].Click(); // < previous
            comp.Find("#counter").TextContent.Should().Be("4");

            comp.FindAll("div.mud-table-pagination-actions button")[1].Click(); // < previous
            comp.Find("#counter").TextContent.Should().Be("5");

            comp.FindAll("div.mud-table-pagination-actions button")[1].Click(); // < previous
            comp.Find("#counter").TextContent.Should().Be("6");
        }

        /// <summary>
        /// The server-side load callback should be called only once per sort change
        /// </summary>
        [Test]
        public async Task TableServerSideDataTest6()
        {
            var comp = Context.RenderComponent<TableServerSideDataTest5>();
            comp.Find("#counter").TextContent.Should().Be("1"); //initial counter

            comp.Find("span.mud-button-root.mud-table-sort-label").Click(); // sort
            comp.Find("#counter").TextContent.Should().Be("2");

            comp.Find("span.mud-button-root.mud-table-sort-label").Click(); // sort
            comp.Find("#counter").TextContent.Should().Be("3");

            comp.Find("span.mud-button-root.mud-table-sort-label").Click(); // sort
            comp.Find("#counter").TextContent.Should().Be("4");

            comp.Find("#reseter").Click(); //reset counter and test again
            comp.Find("#counter").TextContent.Should().Be("0");

            comp.Find("span.mud-button-root.mud-table-sort-label").Click(); // sort
            comp.Find("#counter").TextContent.Should().Be("1");

            comp.Find("span.mud-button-root.mud-table-sort-label").Click(); // sort
            comp.Find("#counter").TextContent.Should().Be("2");

            comp.Find("span.mud-button-root.mud-table-sort-label").Click(); // sort
            comp.Find("#counter").TextContent.Should().Be("3");
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/8298
        /// </summary>
        [Test]
        public async Task SetRowsPerPageAsync_CallOneTimeServerData()
        {
            // Arrange

            var comp = Context.RenderComponent<TableServerSideDataTest2>();
            var table = comp.FindComponent<MudTable<int>>();
            table.Instance.CurrentPage = 2;
            var serverDataCallCount = 0;
            var originalServerDataFunc = table.Instance.ServerData;
            table.Instance.ServerData = (state, cancellationToken) =>
            {
                serverDataCallCount++;
                return originalServerDataFunc(state, cancellationToken);
            };

            // Act

            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(25));

            // Assert

            serverDataCallCount.Should().Be(1);
        }

        /// <summary>
        /// The table should not crash if its ServerData Items are null
        /// </summary>
        [Test]
        public async Task TableServerSideDataNull()
        {
            var comp = Context.RenderComponent<TableServerSideDataTest6>();
        }

        /// <summary>
        /// The table should not render its NoContent fragment prior to loading server data
        /// </summary>
        [Test]
        public async Task TableServerDataLoadingTest()
        {
            var comp = Context.RenderComponent<TableServerDataLoadingTest>();
            comp.Instance.NoRecordsHasRendered.Should().BeFalse();
        }

        /// <summary>
        /// Ensures that multiple calls to reload the table properly flag the CancellationToken.
        /// </summary>
        /// <returns>A <see cref="Task"/> object.</returns>
        [Test]
        public async Task TableServerDataLoadingTestWithCancel()
        {
            // Render the server-side data (with cancellation) test
            var comp = Context.RenderComponent<TableServerDataLoadingTestWithCancel>();
            // Get the MudTable<int> component
            var table = comp.FindComponent<MudTable<int>>();

            // Make a cancellation token we can monitor
            CancellationToken? cancelToken = null;
            // Make a task completion source
            var first = new TaskCompletionSource<TableData<int>>();
            // Set the ServerData function
            table.SetParam(p => p.ServerData, new Func<TableState, CancellationToken, Task<TableData<int>>>((s, cancellationToken) =>
            {
                // Remember the cancellation token
                cancelToken = cancellationToken;
                // Return a task that never completes
                return first.Task;
            }));

            await Task.Delay(20);

            // Test

            // Make sure this first request was not canceled
            comp.WaitForAssertion(() => cancelToken?.IsCancellationRequested.Should().BeFalse());

            // Arrange a table refresh
            var second = new TaskCompletionSource<TableData<int>>();
            // Set the ServerData function to a new method...
            table.SetParam(p => p.ServerData, new Func<TableState, CancellationToken, Task<TableData<int>>>((s, cancellationToken) =>
            {
                // ... which returns the second task.
                return second.Task;
            }));

            await Task.Delay(20);

            // Test

            // Make sure this second request DID cancel the first request's token
            comp.WaitForAssertion(() => cancelToken?.IsCancellationRequested.Should().BeTrue());
        }

        /// <summary>
        /// The table should render the classes and style to the tr using the RowStyleFunc and RowClassFunc parameters
        /// </summary>
        [Test]
        public async Task TableRowClassStyleTest()
        {
            var comp = Context.RenderComponent<TableRowClassStyleTest>();
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
        public async Task TableInlineEdit_SetValidatorModel()
        {
            var comp = Context.RenderComponent<TableInlineEditTest>();
            var validator = comp.Instance.Table.Validator;

            var trs = comp.FindAll("tr");
            trs.Count.Should().Be(4); // three rows + header row

            trs[1].Click();
            validator.Model.Should().Be("A");
            trs[2].Click();
            validator.Model.Should().Be("B");
            trs[3].Click();
            validator.Model.Should().Be("C");
        }

        [Test]
        public async Task TableInlineEdit_TableRowValidator()
        {
            var comp = Context.RenderComponent<TableInlineEditTest>();
            var validator = new TableRowValidatorTest();
            comp.Instance.Table.Validator = validator;

            var trs = comp.FindAll("tr");
            trs.Count.Should().Be(4); // three rows + header row

            trs[1].Click();
            validator.ControlCount.Should().Be(1);
            for (var i = 0; i < 10; ++i)
            {
                trs[(i % 3) + 1].Click();
            }
            validator.ControlCount.Should().Be(1);
        }

        [Theory]
        [TestCase(TableApplyButtonPosition.StartAndEnd)]
        [TestCase(TableApplyButtonPosition.Start)]
        [TestCase(TableApplyButtonPosition.End)]
        public async Task TableInlineEdit_ApplyButtonPosition(TableApplyButtonPosition position)
        {
            var comp = Context.RenderComponent<TableInlineEditTestApplyButtons>(
                p => p.Add(x => x.ApplyButtonPosition, position));

            var trs = comp.FindAll("tr");

            //header + 3 items + footer
            trs.Should().HaveCount(5);

            var header = trs[0];
            var footer = trs[trs.Count - 1];
            var expectedAmount = position switch
            {
                TableApplyButtonPosition.Start or TableApplyButtonPosition.End => 2,
                TableApplyButtonPosition.StartAndEnd => 3,
                _ => throw new NotImplementedException()
            };

            header.ChildElementCount.Should().Be(expectedAmount);
            footer.ChildElementCount.Should().Be(expectedAmount);

            trs[2].Click();

            var trs2 = comp.FindAll("tr");
            var relevantRow = trs2[2];
            relevantRow.ChildElementCount.Should().Be(expectedAmount);

            if (position == TableApplyButtonPosition.Start)
            {
                relevantRow.Children[0].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
                relevantRow.Children[1].FindDescendant<AngleSharp.Html.Dom.IHtmlInputElement>().Should().NotBeNull();
            }
            else if (position == TableApplyButtonPosition.End)
            {
                relevantRow.Children[0].FindDescendant<AngleSharp.Html.Dom.IHtmlInputElement>().Should().NotBeNull();
                relevantRow.Children[1].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
            }
            else if (position == TableApplyButtonPosition.StartAndEnd)
            {
                relevantRow.Children[0].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
                relevantRow.Children[1].FindDescendant<AngleSharp.Html.Dom.IHtmlInputElement>().Should().NotBeNull();
                relevantRow.Children[2].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
            }
        }

        [Test]
        public async Task TableInlineEdit_RowSwitching()
        {
            var comp = Context.RenderComponent<TableInlineEditTest>();

            var trs = comp.FindAll("tr");

            trs[1].InnerHtml.Contains("input").Should().BeFalse();

            trs[1].Click();

            var trs2 = comp.FindAll("tr");
            trs2[1].InnerHtml.Contains("input").Should().BeTrue();

            trs[2].Click();

            var trs3 = comp.FindAll("tr");
            trs3[1].InnerHtml.Contains("input").Should().BeFalse();
            trs3[2].InnerHtml.Contains("input").Should().BeTrue();
        }

        [Test]
        public async Task TableInlineEdit_RowSwitchingBlocked()
        {
            var comp = Context.RenderComponent<TableInlineEditRowBlockingTest>();

            var trs = comp.FindAll("tr");

            trs[1].InnerHtml.Contains("input").Should().BeFalse();

            trs[1].Click();

            var trs2 = comp.FindAll("tr");
            trs2[1].InnerHtml.Contains("input").Should().BeTrue();

            trs[2].Click();

            var trs3 = comp.FindAll("tr");
            trs3[1].InnerHtml.Contains("input").Should().BeTrue();
            trs3[2].InnerHtml.Contains("input").Should().BeFalse();
        }

        /// <summary>
        /// This test validates the edit row maintains position on changing sort key for an inline editing table.
        /// </summary>
        [Test]
        public async Task TableInlineEditSortTest()
        {
            var comp = Context.RenderComponent<TableInlineEditSortTest>();

            // Count the number of rows including header
            comp.FindAll("tr").Count.Should().Be(4); // Three rows + header row

            // Check the values of rows
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("A");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("C");

            // Access to the table
            var table = comp.FindComponent<MudTable<TableInlineEditSortTest.Element>>();

            // Get the mudtablesortlabels associated to the table
            var mudTableSortLabels = table.Instance.Context.SortLabels;

            // Sort the first column
            await table.InvokeAsync(() => mudTableSortLabels[0].ToggleSortDirection());

            // Check the values of rows
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("A");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("B");
            comp.FindAll("td")[4].TextContent.Trim().Should().Be("C");

            // Click on the second row
            var trs = comp.FindAll("tr");
            trs[2].Click();

            // Change row two data
            var input = comp.Find("#Id1");
            input.Change("D");

            // Check row two is still in position 2 of the data rows
            var trs2 = comp.FindAll("tr");
            trs2[1].InnerHtml.Contains("input").Should().BeFalse();
            trs2[2].InnerHtml.Contains("input").Should().BeTrue();
            trs2[3].InnerHtml.Contains("input").Should().BeFalse();
        }

        /// <summary>
        /// This test validates the processing of the Commit and Cancel buttons for an inline editing table.
        /// </summary>
        [Test]
        public async Task TableInlineEditCancelTest()
        {
            var comp = Context.RenderComponent<TableInlineEditCancelTest>();

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
            var comp = Context.RenderComponent<TableInlineEditCancelTest>();

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
            var comp = Context.RenderComponent<TableInlineEditCancelTest>();
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
                var selectedItemValue = table.SelectedItem.Value;

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
            // It should be true meaning that SelectedItem had  the correct value before RowEditPreview has finished to complete
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
            var comp = Context.RenderComponent<TableInlineEditCancelNoSelectedItemTest>();

            // List all the rows
            var trs = comp.FindAll("tr");

            // Click on the third row
            trs[3].Click();

            // How many buttons? It should be equal to 2. One for commit and one for cancel
            comp.FindAll("button").Count.Should().Be(2);

            // Click on the second row
            trs[2].Click();

            // How many buttons? It should always be equal to 2
            comp.FindAll("button").Count.Should().Be(2);

            // Click on the first row
            trs[1].Click();

            // How many buttons? It should always be equal to 2
            comp.FindAll("button").Count.Should().Be(2);
        }

        /// <summary>
        /// Ensures the table buttons render correctly
        /// </summary>
        [Test]
        [TestCase(true, TableApplyButtonPosition.Start, TableEditButtonPosition.Start)]
        [TestCase(true, TableApplyButtonPosition.StartAndEnd, TableEditButtonPosition.StartAndEnd)]
        [TestCase(true, TableApplyButtonPosition.End, TableEditButtonPosition.End)]
        [TestCase(false, TableApplyButtonPosition.Start, TableEditButtonPosition.Start)]
        [TestCase(false, TableApplyButtonPosition.StartAndEnd, TableEditButtonPosition.StartAndEnd)]
        [TestCase(false, TableApplyButtonPosition.End, TableEditButtonPosition.End)]
        public async Task TableEditButtonRenderTest(bool customButton, TableApplyButtonPosition buttonApplyPosition, TableEditButtonPosition buttonEditPosition)
        {
            IRenderedComponent<ComponentBase> comp;
            if (customButton)
            {
                comp = Context.RenderComponent<TableCustomEditButtonRenderTest>(parameters => parameters
                    .Add(p => p.ApplyButtonPosition, buttonApplyPosition)
                    .Add(p => p.EditButtonPosition, buttonEditPosition));
            }
            else
            {
                comp = Context.RenderComponent<TableEditButtonRenderTest>(parameters => parameters
                    .Add(p => p.ApplyButtonPosition, buttonApplyPosition)
                    .Add(p => p.EditButtonPosition, buttonEditPosition));
            }

            var trs = comp.FindAll("tr");

            //header + 3 items
            trs.Should().HaveCount(4);

            var header = trs[0];
            var expectedAmount = buttonEditPosition switch
            {
                TableEditButtonPosition.Start or TableEditButtonPosition.End => 2,
                TableEditButtonPosition.StartAndEnd => 3,
                _ => throw new NotImplementedException()
            };

            header.ChildElementCount.Should().Be(expectedAmount);

            var trs2 = comp.FindAll("tr");
            var relevantRow = trs2[2];
            relevantRow.ChildElementCount.Should().Be(expectedAmount);

            if (buttonEditPosition == TableEditButtonPosition.Start)
            {
                relevantRow.Children[0].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
                relevantRow.Children[1].FindDescendant<AngleSharp.Html.Dom.IHtmlDivElement>().Should().NotBeNull();
            }
            else if (buttonEditPosition == TableEditButtonPosition.End)
            {
                relevantRow.Children[0].FindDescendant<AngleSharp.Html.Dom.IHtmlDivElement>().Should().NotBeNull();
                relevantRow.Children[1].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
            }
            else if (buttonEditPosition == TableEditButtonPosition.StartAndEnd)
            {
                relevantRow.Children[0].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
                relevantRow.Children[1].FindDescendant<AngleSharp.Html.Dom.IHtmlDivElement>().Should().NotBeNull();
                relevantRow.Children[2].FindDescendant<AngleSharp.Html.Dom.IHtmlButtonElement>().Should().NotBeNull();
            }
        }

        /// <summary>
        /// Tests the trigger of the edit button
        /// </summary>
        [Test]
        public async Task TableEditButtonTriggerTest()
        {
            var comp = Context.RenderComponent<TableEditButtonRenderTest>();
            var trs = comp.FindAll("tr");
            trs[1].InnerHtml.Contains("input").Should().BeFalse();

            var buttons = comp.FindAll("button");

            buttons[0].Click();
            var trs2 = comp.FindAll("tr");
            trs2[1].InnerHtml.Contains("input").Should().BeTrue();

            buttons[1].Click();
            var trs3 = comp.FindAll("tr");
            trs3[1].InnerHtml.Contains("input").Should().BeFalse();
            trs3[2].InnerHtml.Contains("input").Should().BeTrue();
        }

        /// <summary>
        /// Tests the trigger of the custom edit button
        /// </summary>
        [Test]
        public async Task TableCustomEditButtonTriggerTest()
        {
            var comp = Context.RenderComponent<TableCustomEditButtonRenderTest>();
            var trs = comp.FindAll("tr");
            trs[1].InnerHtml.Contains("input").Should().BeFalse();

            var buttons = comp.FindAll("button");

            buttons[0].Click();
            var trs2 = comp.FindAll("tr");
            trs2[1].InnerHtml.Contains("input").Should().BeTrue();

            buttons[1].Click();
            var trs3 = comp.FindAll("tr");
            trs3[1].InnerHtml.Contains("input").Should().BeFalse();
            trs3[2].InnerHtml.Contains("input").Should().BeTrue();
        }

        /// <summary>
        /// Row item data should be passed to EditButtonContext
        /// </summary>
        [Test]
        public async Task TableCustomEditButtonItemContext()
        {
            var comp = Context.RenderComponent<TableCustomEditButtonItemContextRenderTest>();

            var buttons = comp.FindAll("button");
            buttons[0].Click();
            comp.Instance.LatestButtonClickItem.Should().Be("A");

            buttons[1].Click();
            comp.Instance.LatestButtonClickItem.Should().Be("B");

            buttons[2].Click();
            comp.Instance.LatestButtonClickItem.Should().Be("C");
        }

        /// <summary>
        /// Ensures clicking a different button does not switch the row
        /// </summary>
        [Test]
        public async Task TableEditButtonRowSwitchBlockTest()
        {
            var comp = Context.RenderComponent<TableEditButtonRenderTest>(parameters => parameters
                    .Add(p => p.BlockRowSwitching, true));
            var trs = comp.FindAll("tr");
            trs[1].InnerHtml.Contains("input").Should().BeFalse();

            var buttons = comp.FindAll("button");

            buttons[0].Click();
            var trs2 = comp.FindAll("tr");
            trs2[1].InnerHtml.Contains("input").Should().BeTrue();

            buttons[1].Click();
            var trs3 = comp.FindAll("tr");
            trs3[1].InnerHtml.Contains("input").Should().BeTrue();
            trs3[2].InnerHtml.Contains("input").Should().BeFalse(); //the row has not switched
        }

        /// <summary>
        /// Clicking the edit button should not trigger the row click event
        /// </summary>
        [Test]
        public async Task TableEditButtonNoRowTrigger()
        {
            var timesClicked = 0;
            void OnRowClick()
            {
                timesClicked++;
            }
            var comp = Context.RenderComponent<TableEditButtonRenderTest>(parameters => parameters
                    .Add(p => p.RowClicked, OnRowClick));

            var trs = comp.FindAll("tr");
            trs[1].Click();
            timesClicked.Should().Be(1);

            trs[2].Click();
            timesClicked.Should().Be(2);

            var buttons = comp.FindAll("button");
            buttons[0].Click();
            timesClicked.Should().Be(2); //clicking the button should not trigger the row click event
        }

        /// <summary>
        /// Tests the grouping behavior and ensure that it won't break anything else.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TableGroupingTest()
        {
            // without grouping, to ensure that anything was broken:
            var comp = Context.RenderComponent<TableGroupingTest>();
            var table = comp.Instance.tableInstance;
            table.Context.HeaderRows.Count.Should().Be(1);
            table.Context.GroupRows.Count.Should().Be(0);
            table.Context.Rows.Count.Should().Be(9);

            IRefreshableElementCollection<IElement> Inputs() => comp.FindAll("input");
            IRefreshableElementCollection<IElement> Buttons() => comp.FindAll("button");

            // now, with multi selection:
            table.MultiSelection = true;
            Inputs().Count.Should().Be(10);
            Inputs()[0].Change(true);
            table.SelectedItems.Count.Should().Be(9);
            Inputs()[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            //group by Racing Category:
            comp = Context.RenderComponent<TableGroupingTest>();
            table = comp.Instance.tableInstance;
            table.GroupBy = new TableGroupDefinition<TableGroupingTest.RacingCar>(rc => rc.Category, null) { GroupName = "Category" };
            comp.Render();
            table.Context.GroupRows.Count.Should().Be(4);
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(18); // 1 table header + 4 group headers + 9 item rows + 4 group footers

            // multi selection:
            table.MultiSelection = true;

            Inputs()[1].Change(true); // selecting only LMP1 category
            table.SelectedItems.Count.Should().Be(2); // only one porsche and one audi
            Inputs()[1].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            Inputs()[4].Change(true); // selecting only GTE category
            table.SelectedItems.Count.Should().Be(3);
            Inputs()[4].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            Inputs()[0].Change(true); // all
            table.SelectedItems.Count.Should().Be(9);
            Inputs()[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            //group by Racing Category and Brand:
            comp = Context.RenderComponent<TableGroupingTest>();
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
            Inputs()[0].Change(true); // all
            table.SelectedItems.Count.Should().Be(9);
            Inputs()[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            Inputs()[1].Change(true); // selecting only LMP1 category
            table.SelectedItems.Count.Should().Be(2);

            // indentation:
            table.GroupBy.Indentation = true;
            comp.Render();
            tr = comp.FindAll("tr.mud-table-row-group-indented-1").ToArray();
            tr.Length.Should().Be(27); // (4 LMP1 group (h / f) + 6 GTE + 4 GTE + 4 Formula 1) brands groups per category + 9 data rows
            tr = comp.FindAll("tr.mud-table-row-group-indented-2").ToArray();
            tr.Length.Should().Be(0); // indentation works with Level - 1 class. (level 1 doesn't need to be indented)

            // expand and collapse groups:
            table.GroupBy.Indentation = false;
            table.GroupBy.Expandable = true;
            table.GroupBy.InnerGroup.Expandable = true;
            comp.Render();

            Buttons().Count.Should().Be(13);// 4 categories and 9 cars (can repeat on different categories)
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(36); // 1 table header + 8 category group rows (h + f)  + 18 brands group rows (see line 915) + 9 car rows

            // collapsing category LMP1:
            Buttons()[0].Click();
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(29); // 1 table header + 8 category group rows (h + f) - LMP1 footer + 18 brands group rows (see line 915) - 2 brands LMP2 Header - 2 brands LMP1 footer + 9 car rows - 2 LMP1 car rows
            Buttons()[0].Click();
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(36);


            //verify the collapse and expand selection on UI and items

            Inputs()[1].Change(false); // LMP1

            table.GroupBy.Indentation = true;
            table.GroupBy.Expandable = true;
            table.GroupBy.IsInitiallyExpanded = true;
            table.GroupBy.InnerGroup.Indentation = true;
            table.GroupBy.InnerGroup.Expandable = true;
            table.GroupBy.InnerGroup.IsInitiallyExpanded = true;


            comp.Render();

            table.SelectedItems.Count.Should().Be(0);
            Inputs().Where(x => x.IsChecked()).Count().Should().Be(0);

            Inputs()[1].Change(true); // LMP1
            table.SelectedItems.Count.Should().Be(2);

            Inputs().Where(x => x.IsChecked()).Count().Should().Be(5);

            Buttons()[0].Click(); //collapse
            Buttons()[0].Click(); //expand
            //selected item should persist
            table.SelectedItems.Count.Should().Be(2);

            Inputs().Where(x => x.IsChecked()).Count().Should().Be(5);

            Inputs()[1].Change(false);
            table.SelectedItems.Count.Should().Be(0);

            Inputs().Where(x => x.IsChecked()).Count().Should().Be(0);

        }

        /// <summary>
        /// Tests the grouping behavior and ensure that it won't break anything else.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TableGroupingAndPaginationTest()
        {
            // without grouping, to ensure that anything was broken:
            var comp = Context.RenderComponent<TableGroupingTest2>();
            var table = comp.Instance.tableInstance;
            table.Context.HeaderRows.Count.Should().Be(1);

            // Page 01:
            // [00] Porsche
            //      [01] LMP1
            //      [02] GTE
            //      [03] GT3
            // [04] Audi
            //      [05] LMP1
            //      [06] GT3
            // [07] Ferrari
            //      [08] Formula 1
            // [09] McLaren
            //      [10] Formula 1
            //      [11] GT3
            // [12] Aston Martin
            //      [13] GTE
            table.Context.GroupRows.Count.Should().Be(14);
            table.Context.Rows.Count.Should().Be(10);
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(39); // 01 Table header + 14 Group Headers + 14 Group Footers + 10 Entries

            // Navigating to page 2
            table.NavigateTo(1);

            // Page 02:
            // [00] Aston Martin
            //      [01] GTE
            table.Context.GroupRows.Count.Should().Be(2);
            table.Context.Rows.Count.Should().Be(1);
            tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(6); // 01 Table header + 02 Group Headers + 02 Group Footers + 01 Entries

        }


        /// <summary>
        /// Tests the IsInitiallyExpanded grouping behavior.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TableGroupIsInitiallyExpandedTest()
        {
            // group by Racing Category and collapse groups as default:
            var comp = Context.RenderComponent<TableGroupingTest>();
            var table = comp.Instance.tableInstance;
            table.GroupBy = new TableGroupDefinition<TableGroupingTest.RacingCar>(rc => rc.Category, null)
            {
                GroupName = "Category",
                Expandable = true,
                IsInitiallyExpanded = false
            };
            comp.Render();
            table.Context.GroupRows.Count.Should().Be(4); // 4 categories
            var tr = comp.FindAll("tr").ToArray();
            tr.Length.Should().Be(5); // 1 table header + 4 group headers
        }

        [Test]
        public void ExpandAndCollapsAllGroupsTest()
        {
            var comp = Context.RenderComponent<TableGroupingTest>();
            var table = comp.Instance.tableInstance;
            table.GroupBy = new TableGroupDefinition<TableGroupingTest.RacingCar>(rc => rc.Category, null) { GroupName = "Category", IsInitiallyExpanded = false, Expandable = true };
            comp.Render();

            // Header only since we have IsInitiallyExpanded = false
            table.Context.GroupRows.Count.Should().Be(4);
            comp.FindAll("tr").ToArray().Length.Should().Be(5); // 1 table header + 4 group headers

            // Expand all groups
            table.ExpandAllGroups();
            comp.Render();
            comp.FindAll("tr").ToArray().Length.Should().Be(18); // 1 table header + 4 group headers + 9 item rows + 4 group footers

            // Collapse all groups
            table.CollapseAllGroups();
            comp.Render();
            comp.FindAll("tr").ToArray().Length.Should().Be(5); // 1 table header + 4 group headers
        }

        /// <summary>
        /// Tests the correct output when filter does not return any matching elements
        /// </summary>
        /// <returns>The awaitable <see cref="Task"/></returns>
        [Test]
        public async Task TablePagerInfoTextTest()
        {
            // create the component
            var tableComponent = Context.RenderComponent<TablePagerInfoTextTest>();

            // print the generated html

            // assert correct info-text
            tableComponent.Find("div.mud-table-page-number-information").Text().Should().Be("1-10 of 59", "No filter applied yet.");

            // get the instance
            var tableInstance = tableComponent.FindComponent<MudTable<string>>().Instance;

            // get the search-string
            var searchString = tableComponent.Find("#searchString");

            // should return 3 items
            searchString.Change("Ala");
            tableInstance.GetFilteredItemsCount().Should().Be(3);
            string.Join(",", tableInstance.FilteredItems).Should().Be("Alabama,Alaska,Palau");
            tableComponent.FindAll("tr").Count.Should().Be(3);
            tableComponent.Find("div.mud-table-page-number-information").Text().Should().Be("1-3 of 3", "'Ala' filter applied.");

            // no matches
            searchString.Change("ZZZ");
            tableInstance.GetFilteredItemsCount().Should().Be(0);
            tableInstance.FilteredItems.Count().Should().Be(0);
            tableComponent.FindAll("tr").Count.Should().Be(0);
            tableComponent.Find("div.mud-table-page-number-information").Text().Should().Be("0-0 of 0", "'ZZZ' filter applied.");
        }

        /// <summary>
        /// Tests the aria-labels for the pager control buttons
        /// </summary>
        /// <param name="controlButton">The type of the control button. Page.First for the navigate-to-first-page button.</param>
        /// <param name="expectedButtonAriaLabel">The expected value in the aria-label.</param>
        [TestCase(Page.First, "First page")]
        [TestCase(Page.Previous, "Previous page")]
        [TestCase(Page.Next, "Next page")]
        [TestCase(Page.Last, "Last page")]
        [Test]
        public void TablePagerControlButtonAriaLabelTest(Page controlButton, string expectedButtonAriaLabel)
        {
            var tableComponent = Context.RenderComponent<TablePagerInfoTextTest>();

            //get control button
            var buttons = tableComponent.FindAll("div.mud-table-pagination-actions button");
            var button = controlButton switch
            {
                Page.First => buttons[0],
                Page.Previous => buttons[1],
                Page.Next => buttons[^2],
                Page.Last => buttons[^1],
                _ => throw new ArgumentOutOfRangeException(nameof(controlButton), controlButton,
                    "This control button type is not supported!")
            };

            //Expected values
            button.GetAttribute("aria-label")?.Should().Be(expectedButtonAriaLabel);
        }

        /// <summary>
        /// Tests checks that RowsPerPage Parameter is two-way bindable
        /// </summary>
        [Test]
        public async Task RowsPerPageParameterTwoWayBinding()
        {
            var rowsPerPage = 5;
            var newRowsPerPage = 25;
            var comp = Context.RenderComponent<TableRowsPerPageTwoWayBindingTest>(parameters => parameters
                .Add(p => p.RowsPerPage, rowsPerPage)
                .Add(p => p.RowsPerPageChanged, (s) =>
                {
                    rowsPerPage = int.Parse(s.ToString());
                })
            );
            //Check the component rendered correctly with the initial RowsPerPage
            var t = comp.Find("input.mud-select-input").GetAttribute("Value");
            int.Parse(t).Should().Be(rowsPerPage, "The component rendered correctly");
            //open the menu
            var menuItem = comp.Find("div.mud-input-control");
            menuItem.Click();

            //Now select the 25 and check it
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();
            comp.WaitForAssertion(() => rowsPerPage.Should().Be(newRowsPerPage, "ValueChanged EventCallback fired correctly"));
        }

        /// <summary>
        /// Tests that clicking a row in a non-editable table does not set IsEditing to true and stop the table from updating.
        /// </summary>
        [Test]
        public void TableRowClickNotEditable()
        {
            var comp = Context.RenderComponent<TableRowClickNotEditableTest>();

            // Get table instance
            var tableInstance = comp.FindComponent<MudTable<string>>().Instance;

            // Check number of filtered items
            tableInstance.GetFilteredItemsCount().Should().Be(3);

            // Click row
            var trs = comp.FindAll("tr");
            trs[1].Click();

            // Filter items
            var searchString = comp.Find("#searchString");
            searchString.Change("b");

            // Make sure number of items has updated
            tableInstance.GetFilteredItemsCount().Should().Be(1);
        }

        /// <summary>
        /// Tests that AllowEditItem is respected when clicking a row
        /// </summary>
        [Test]
        [TestCase(TableEditTrigger.RowClick)]
        [TestCase(TableEditTrigger.EditButton)]
        public void AllowEditRowPreventsEdit(TableEditTrigger trigger)
        {
            var comp = Context.RenderComponent<TableNotEditableRowTest>(parameters => parameters.Add(x => x.EditTrigger, trigger));

            // Get table instance
            var tableInstance = comp.FindComponent<MudTable<int>>().Instance;

            // Check number of filtered items
            tableInstance.GetFilteredItemsCount().Should().Be(3);

            var trs = comp.FindAll("tr");

            if (trigger == TableEditTrigger.RowClick)
            {
                trs[0].InnerHtml.Contains("input").Should().BeFalse();
                trs[1].InnerHtml.Contains("input").Should().BeFalse();

                trs[0].Click();
                tableInstance.SelectedItem.Should().Be(5);
                tableInstance.Editing.Should().BeFalse();

                trs[1].Click();
                tableInstance.Editing.Should().BeTrue();
                tableInstance.SelectedItem.Should().Be(10);


                var trs2 = comp.FindAll("tr");
                trs2[0].InnerHtml.Contains("input").Should().BeFalse();
                trs2[1].InnerHtml.Contains("input").Should().BeTrue();
            }
            else
            {
                trs[0].InnerHtml.Contains("button").Should().BeFalse();
                trs[1].InnerHtml.Contains("button").Should().BeTrue();
                trs[2].InnerHtml.Contains("button").Should().BeTrue();
                trs[1].InnerHtml.Contains("input").Should().BeFalse();

                var buttons = comp.FindAll("button");
                buttons[0].Click();

                var trs2 = comp.FindAll("tr");
                trs2[0].InnerHtml.Contains("input").Should().BeFalse();
                trs2[1].InnerHtml.Contains("input").Should().BeTrue();
                trs2[2].InnerHtml.Contains("input").Should().BeFalse();
            }
        }

        /// Issue #3033
        /// Tests changing RowsPerPage Parameter from code - Table should re-render new RowsPerPage parameter and parameter value should be set
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task RowsPerPageChangeValueFromCode()
        {
            var testComponent = Context.RenderComponent<TablePagerChangeRowsPerPageTest>();
            var table = testComponent.FindComponent<MudTable<string>>().Instance;
            var buttonComponent = testComponent.FindComponent<MudButton>();
            testComponent.WaitForAssertion(() => table.RowsPerPage.Should().Be(35));
            //Toggle the rows per page value from 35 to 10
            buttonComponent.Find("button").Click();
            testComponent.WaitForAssertion(() => table.RowsPerPage.Should().Be(10));
            //Toggle the rows per page value from 10 back to  to 35
            buttonComponent.Find("button").Click();
            testComponent.WaitForAssertion(() => table.RowsPerPage.Should().Be(35));
        }

        /// <summary>
        /// Tests whether record type table items are kept track of when edited
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TableRecordEditingMultiSelectTest()
        {
            var comp = Context.RenderComponent<TableRecordComparerTest>();
            var table = comp.FindComponent<MudTable<TableRecordComparerTest.Element>>().Instance;

            var checkboxes = comp.FindComponents<MudCheckBox<bool>>().Select(x => x.Instance).ToArray();

            var inputs = comp.FindAll("input").ToArray();
            inputs.Length.Should().Be(4); // one checkbox per row + one for the header
            table.SelectedItems.Count.Should().Be(0); // selected items should be empty
            // click header checkbox
            inputs[0].Change(true);
            table.SelectedItems.Count.Should().Be(3);

            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3); //there should be 3 items
            comp.Find("p").TextContent.Should().Be("Elements { A, B, C }");

            // Click on the second row
            var trs = comp.FindAll("tr");
            trs[2].Click();

            // Change row two data
            var input = comp.Find("#Id2");
            input.Change("Change");

            table.SelectedItems.Count.Should().Be(3);

            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(3); //there should be 3 items
            comp.Find("p").TextContent.Should().Be("Elements { A, Change, C }");

            // Uncheck and verify that all items are removed
            inputs[0].Change(false);
            table.SelectedItems.Count.Should().Be(0);
            checkboxes.Sum(x => x.Value ? 1 : 0).Should().Be(0); //there should be 4 items
            comp.Find("p").TextContent.Should().Be("Elements {  }");
        }

        /// <summary>
        /// Setting a comparer should be reflected in all layers of the table
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TableComparerContextTest()
        {
            var comp = Context.RenderComponent<TableComparerContextTest>();
            var table = comp.FindComponent<MudTable<TableComparerContextTest.Element>>().Instance;

            // Comparer is null by default
            var context = table.Context;
            table.Comparer.Should().Be(null);
            context.Comparer.Should().Be(null);

            await comp.InvokeAsync(() => comp.Instance.SetComparer());

            // All comparer values should match
            table.Comparer.Should().Be(comp.Instance.Comparer);
            context.Comparer.Should().Be(comp.Instance.Comparer);
            context.Selection.Comparer.Should().Be(comp.Instance.Comparer); //check comparer is set in HashSet and Dictionary
            context.Rows.Comparer.Should().Be(comp.Instance.Comparer);
        }

        /// <summary>
        /// Using a virtualized table with multiselection must preserve checked items
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestVirtualizedTableWithMultiSelection()
        {
            var comp = Context.RenderComponent<TableMultiSelectionVirtualizedTest>();
            var table = comp.FindComponent<MudTable<TableMultiSelectionVirtualizedTest.TestItem>>();
            var virtualized = comp.FindComponent<Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize<TableMultiSelectionVirtualizedTest.TestItem>>();
            // find first checkbox item and check it
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell .mud-checkbox-input")[0].IsChecked().Should().Be(false);
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell")[1].TextContent.Should().Be("Value_0");
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell .mud-checkbox-input")[0].Change(true);
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell .mud-checkbox-input")[0].IsChecked().Should().Be(true);

            // scroll down
            virtualized.SetParam(
                v => v.Items,
                table.Instance.Items.ToList().GetRange(1000, 100));
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell")[1].TextContent.Should().Be("Value_1000");
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell .mud-checkbox-input")[0].IsChecked().Should().Be(false);

            // scroll up
            virtualized.SetParam(
                v => v.Items,
                table.Instance.Items.ToList().GetRange(0, 100));
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell")[1].TextContent.Should().Be("Value_0");
            comp.FindAll(".mud-table-body .mud-table-row .mud-table-cell .mud-checkbox-input")[0].IsChecked().Should().Be(true);
        }

        /// <summary>
        /// Selecting the 'Select All' checkbox should trigger the 'SelectedItemsChanged' event only once
        /// </summary>
        [Test]
        public async Task TestSelectedItemsChanedWithMultiSelection()
        {
            var comp = Context.RenderComponent<TableMultiSelectionSelectedItemsChangedTest>();
            var selectAllCheckbox = comp.Find("input");
            selectAllCheckbox.Change(true);
            comp.Find("#counter").TextContent.Should().Be("1");
        }
    }
}
