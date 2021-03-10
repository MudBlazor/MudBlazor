#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
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
            trs[0].Click(); // clicking the header row shouldn't to anything
            comp.Find("p").TextContent.Trim().Should().Be("0,0,1");
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
            var pagingButtons = comp.FindAll("button");
            // click next page
            pagingButtons[2].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("11-20 of 59");
            // last page
            pagingButtons[3].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(9);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("51-59 of 59");
            // previous page
            pagingButtons[1].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("41-50 of 59");
            // first page
            pagingButtons[0].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
        }

        /// <summary>
        /// simple navigation using the paging buttons - RTL
        /// </summary>
        [Test]
        public void TablePagingNavigationButtonsRtl()
        {
            var comp = ctx.RenderComponent<TablePagingTest1Rtl>();
            // print the generated html      
            Console.WriteLine(comp.Markup);
            // after initial load
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
            var pagingButtons = comp.FindAll("button");
            // click next page
            pagingButtons[1].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("11-20 of 59");
            // last page
            pagingButtons[0].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(9);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("51-59 of 59");
            // previous page
            pagingButtons[2].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("41-50 of 59");
            // first page
            pagingButtons[3].Click();
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
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
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(60));
            pager.Value.Should().Be("60");
            comp.FindAll("tr.mud-table-row").Count.Should().Be(59);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-59 of 59");
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(10));
            pager.Value.Should().Be("10");
            comp.FindAll("tr.mud-table-row").Count.Should().Be(10);
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 59");
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
            // last page
            comp.FindAll("div.mud-table-pagination-actions button")[3].Click(); // last >
            comp.FindAll("td")[0].TextContent.Trim().Should().Be("28");
            comp.FindAll("td")[1].TextContent.Trim().Should().Be("29");
            comp.FindAll("td")[2].TextContent.Trim().Should().Be("30");
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("91-99 of 99");
            // change page size
            await table.InvokeAsync(() => table.Instance.SetRowsPerPage(100));
            pager.Value.Should().Be("100");
            comp.FindAll("p.mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-99 of 99");
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
        public async Task TableInlineEdit_CheckMemoryUsage()
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
    }
}
