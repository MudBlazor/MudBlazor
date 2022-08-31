#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using Microsoft.JSInterop.Infrastructure;
using MudBlazor.Interop;
using System.Text.Json;
using System.Linq.Expressions;
using PrimitiveCalculator;

namespace MudBlazor.UnitTests.Components
{
    public record TestModel1(string Name, int? Age);
    public record TestModel2(string Name, int? Age, DateTime? Date);
    public record TestModel3(string Name, int? Age, Severity? Status);
    public record TestModel4(string Name, int? Age, bool? Hired);

    [TestFixture]
    public class DataGridTests : BunitTest
    {
        [Test]
        public async Task DataGridSortableTest()
        {
            var comp = Context.RenderComponent<DataGridSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(9, because: "1 header row + 7 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(21, because: "We have 7 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by Name.
            cells[0].TextContent.Should().Be("C"); cells[1].TextContent.Should().Be("33"); cells[2].TextContent.Should().Be("33333");
            cells[3].TextContent.Should().Be("C"); cells[4].TextContent.Should().Be("44"); cells[5].TextContent.Should().Be("1111111");
            cells[6].TextContent.Should().Be("C"); cells[7].TextContent.Should().Be("55"); cells[8].TextContent.Should().Be("222222");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("73"); cells[14].TextContent.Should().Be("7");
            cells[15].TextContent.Should().Be("A"); cells[16].TextContent.Should().Be("11"); cells[17].TextContent.Should().Be("4444");
            cells[18].TextContent.Should().Be("A"); cells[19].TextContent.Should().Be("99"); cells[20].TextContent.Should().Be("66");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            cells = dataGrid.FindAll("td");

            // Back to original order without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            var column = dataGrid.FindComponent<Column<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => column.Instance.SortBy = x => { return x.Name; });
            ////await comp.InvokeAsync(() => column.Instance.CompileSortBy());

            // Check the values of rows - should not be sorted and should be in the original order.
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // sort through the sort icon
            dataGrid.Find(".column-options button").Click();
            cells = dataGrid.FindAll("td");
            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // test other sort methods
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
            await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter());
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters());

            await comp.InvokeAsync(() => dataGrid.Instance.SortMode = SortMode.None);
            dataGrid.Render();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridFilterableTest()
        {
            var comp = Context.RenderComponent<DataGridFilterableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6); // header row + four rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            dataGrid.Instance.FilterDefinitions.Add(new FilterDefinition<DataGridFilterableTest.Item>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = "C"
            });
            dataGrid.Render();

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            dataGrid.Instance.Filterable = false;
        }

        [Test]
        public async Task DataGridFilterableServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridFilterableServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableServerDataTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6); // header row + four rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            dataGrid.Instance.FilterDefinitions.Add(new FilterDefinition<DataGridFilterableServerDataTest.Item>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = "C"
            });
            dataGrid.Render();

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            dataGrid.Instance.Filterable = false;
        }

        [Test]
        public async Task DataGridMultiSelectionTest()
        {
            var comp = Context.RenderComponent<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("input")[0].Change(true);
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, dataGrid.Instance.SelectedItems.First()));
            dataGrid.Instance.SelectedItems.Count.Should().Be(2);

            // select an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(dataGrid.Instance.Items.First()));
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(true));
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // deselect from the footer
            dataGrid.Find("tfoot input").Change(false);
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("0");

            // click to go to the next page
            dataGrid.FindAll(".mud-table-pagination-actions button")[2].Click();

            // test that we are on the second page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("10");
            dataGrid.Instance.RowsPerPage.Should().Be(10);

            // set rows per page programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetRowsPerPageAsync(4));
            dataGrid.Instance.RowsPerPage.Should().Be(4);

            // navigate to the last page programmatically
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Last));
            dataGrid.Instance.CurrentPage.Should().Be(4);

            // navigate to the previous page programmatically
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Previous));
            dataGrid.Instance.CurrentPage.Should().Be(3);

            // navigate back to the first page programmatically
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.First));
            dataGrid.Instance.CurrentPage.Should().Be(0);
        }

        [Test]
        public async Task DataGridInlineEditTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Trim().Should().Be("32");
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].Change(52d);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
        }

        [Test]
        public async Task DataGridInlineEditWithNullableTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditWithNullableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithNullableTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            Assert.IsNull(dataGrid.FindAll("td input")[5].GetAttribute("value"));
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].Change(52d);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
        }

        [Test]
        public async Task DataGridInlineEditWithTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditWithTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithTemplateTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].HasAttribute("checked").Should().Be(false);
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[5].HasAttribute("checked").Should().Be(true);
            dataGrid.FindAll("td input")[6].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[7].GetAttribute("value").Trim().Should().Be("32");
            dataGrid.FindAll("td input")[8].HasAttribute("value").Should().Be(false);
            dataGrid.FindAll("td input")[0].Change("Jonathan");
            dataGrid.FindAll("td input")[1].Change(52d);
            dataGrid.FindAll("td input")[2].Change(true);
            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("52");
            dataGrid.FindAll("td input")[2].HasAttribute("checked").Should().Be(true);

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            var hired = dataGrid.Instance.Items.First().Hired;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
            hired.Should().Be(true);
        }

        [Test]
        public async Task DataGridVisualStylingTest()
        {
            var comp = Context.RenderComponent<DataGridVisualStylingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridVisualStylingTest.Item>>();

            dataGrid.FindAll("td")[1].GetAttribute("style").Should().Contain("background-color:#E5BDE5");
            dataGrid.FindAll("td")[2].GetAttribute("style").Should().Contain("font-weight:bold");
        }

        [Test]
        public async Task DataGridEventCallbacksTest()
        {
            var comp = Context.RenderComponent<DataGridEventCallbacksTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEventCallbacksTest.Item>>();

            // Include callbacks in test coverage.
            dataGrid.Instance.RowClick.HasDelegate.Should().Be(true);
            dataGrid.Instance.SelectedItemChanged.HasDelegate.Should().Be(true);
            dataGrid.Instance.CommittedItemChanges.HasDelegate.Should().Be(true);

            // Set some parameters manually so that they are covered.
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.MultiSelection), true));
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ReadOnly), false));
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.EditMode), DataGridEditMode.Cell));
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.EditTrigger), DataGridEditTrigger.OnRowClick));
            dataGrid.SetParametersAndRender(parameters.ToArray());

            // Make sure that the callbacks have not been fired yet.
            comp.Instance.RowClicked.Should().Be(false);
            comp.Instance.SelectedItemChanged.Should().Be(false);
            comp.Instance.CommittedItemChanges.Should().Be(false);

            // Fire RowClick, SelectedItemChanged, SelectedItemsChanged, and StartedEditingItem callbacks.
            dataGrid.FindAll(".mud-table-body tr")[0].Click();

            //Console.WriteLine(dataGrid.Markup);
            // Edit an item.
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("A test");

            // Make sure that the callbacks have been fired.
            comp.Instance.RowClicked.Should().Be(true);
            comp.Instance.SelectedItemChanged.Should().Be(true);
            comp.Instance.CommittedItemChanges.Should().Be(true);
        }

        [Test]
        public async Task DataGridServerSideSortableTest()
        {
            // Disable simulated load on server side:
            TestComponents.DataGridServerSideSortableTest.DisableServerTimeoutForTests = true;

            var comp = Context.RenderComponent<DataGridServerSideSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerSideSortableTest.Item>>();

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(21, because: "We have 7 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by A.
            cells[0].TextContent.Should().Be("C"); cells[1].TextContent.Should().Be("33"); cells[2].TextContent.Should().Be("33333");
            cells[3].TextContent.Should().Be("C"); cells[4].TextContent.Should().Be("44"); cells[5].TextContent.Should().Be("1111111");
            cells[6].TextContent.Should().Be("C"); cells[7].TextContent.Should().Be("55"); cells[8].TextContent.Should().Be("222222");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("73"); cells[14].TextContent.Should().Be("7");
            cells[15].TextContent.Should().Be("A"); cells[16].TextContent.Should().Be("11"); cells[17].TextContent.Should().Be("4444");
            cells[18].TextContent.Should().Be("A"); cells[19].TextContent.Should().Be("99"); cells[20].TextContent.Should().Be("66");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be the default order of the items.
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SortMode = SortMode.None);
            dataGrid.Render();

            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task FilterDefinitionStringTest()
        {
            var filterDefinition = new FilterDefinition<TestModel1>();
            Func<TestModel1, bool> func = null;

            #region FilterOperator.String.Contains

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Does not contain", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Does not contain", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.NotContains

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotContains,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Does not contain", 45)));
            Assert.IsFalse(func.Invoke(new("Joe", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotContains,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Does not contain", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.Equal

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Not Joe", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.NotEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Not Joe", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsFalse(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEqual,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.StartsWith

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Not Joe", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.StartsWith,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Not Joe", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.String.EndsWith

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.EndsWith,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.String.Empty

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func.Invoke(new("", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new(String.Empty, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func.Invoke(new("", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsFalse(func.Invoke(new(String.Empty, 45)));

            #endregion

            #region FilterOperator.String.NotEmpty

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func.Invoke(new("", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsFalse(func.Invoke(new(String.Empty, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func.Invoke(new("", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));
            Assert.IsFalse(func.Invoke(new(String.Empty, 45)));

            #endregion

            // handle null operator
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = null,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func.Invoke(new(null, 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
        }

        [Test]
        public async Task FilterDefinitionStringForIDictionaryTest()
        {
            var filterDefinition = new FilterDefinition<IDictionary<string, object>>();
            Func<IDictionary<string, object>, bool> func = null;

            #region FilterOperator.String.Contains

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Does not contain" }, { "Age", 45} }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Does not contain" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.NotContains

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotContains,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Does not contain" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotContains,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Does not contain" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.Equal

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Not Joe" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.NotEqual

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Not Joe" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEqual,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.StartsWith

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Not Joe" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.StartsWith,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Not Joe" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.EndsWith

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.EndsWith,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.Empty

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Empty,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", String.Empty }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", String.Empty }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.String.NotEmpty

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", String.Empty }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null,
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", String.Empty }, { "Age", 45 } }));

            #endregion

            // handle null operator
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = null,
                Value = "Joe",
                FieldType = typeof(string)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe Not" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", null }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));
        }

        [Test]
        public async Task FilterDefinitionNumberTest()
        {
            var filterDefinition = new FilterDefinition<TestModel1>();
            Func<TestModel1, bool> func = null;

            #region FilterOperator.Number.Equal

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.Equal,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 456)));
            Assert.IsFalse(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.Equal,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            // data type is an int
            Assert.IsTrue(func.Invoke(new("Sam", 456)));
            Assert.IsTrue(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.Number.NotEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.NotEqual,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456)));
            Assert.IsTrue(func.Invoke(new("Sam", null)));
            Assert.IsFalse(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.NotEqual,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456)));
            Assert.IsTrue(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.Number.GreaterThan

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThan,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456)));
            Assert.IsFalse(func.Invoke(new("Joe", 45)));
            Assert.IsFalse(func.Invoke(new("Joe", null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThan,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", null)));

            #endregion

            #region FilterOperator.Number.LessThan

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThan,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 4)));
            Assert.IsFalse(func.Invoke(new("Joe", 45)));
            Assert.IsFalse(func.Invoke(new("Joe", null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThan,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 4)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", null)));

            #endregion

            #region FilterOperator.Number.GreaterThanOrEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 4)));
            Assert.IsFalse(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 4)));
            Assert.IsTrue(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.Number.LessThanOrEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Console.WriteLine(func.Invoke(new("Joe", 45)));
            Assert.IsFalse(func.Invoke(new("Sam", 46)));
            Assert.IsFalse(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Console.WriteLine(func.Invoke(new("Joe", 45)));
            Assert.IsTrue(func.Invoke(new("Sam", 46)));
            Assert.IsTrue(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = null,
                Value = 45
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456)));
            Assert.IsTrue(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
        }

        [Test]
        public async Task FilterDefinitionNumberForDictionaryTest()
        {
            var filterDefinition = new FilterDefinition<IDictionary<string, object>>();
            Func<IDictionary<string, object>, bool> func = null;

            #region FilterOperator.Number.Equal

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.Equal,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.Equal,
                Value = null,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            // data type is an int
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.Number.NotEqual

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.NotEqual,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.NotEqual,
                Value = null,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Joe" }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.Number.GreaterThan

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThan,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThan,
                Value = null,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));

            #endregion

            #region FilterOperator.Number.LessThan

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThan,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 4 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThan,
                Value = null,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 4 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));

            #endregion

            #region FilterOperator.Number.GreaterThanOrEqual

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 4 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = null,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 4 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));

            #endregion

            #region FilterOperator.Number.LessThanOrEqual

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 46 } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = null,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 46 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = null,
                Value = 45,
                FieldType = typeof(int)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 456 } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", null } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 } }));
        }

        [Test]
        public async Task FilterDefinitionBoolTest()
        {
            #region FilterOperator.Boolean.Is

            var filterDefinition = new FilterDefinition<TestModel4>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = FilterOperator.Boolean.Is,
                Value = true
            };
            var func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 45, false)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, true)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel4>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = FilterOperator.Boolean.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, false)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, true)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel4>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = null,
                Value = true
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, false)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, true)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));
        }

        [Test]
        public async Task FilterDefinitionBoolForDictionaryTest()
        {
            #region FilterOperator.Boolean.Is

            var filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = FilterOperator.Boolean.Is,
                Value = true,
                FieldType = typeof(bool)
            };
            var func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", false } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", true } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = FilterOperator.Boolean.Is,
                Value = null,
                FieldType = typeof(bool)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", false } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", true } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", null } }));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = null,
                Value = true,
                FieldType = typeof(bool)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", false } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", true } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Hired", null } }));
        }

        [Test]
        public async Task FilterDefinitionEnumTest()
        {
            #region FilterOperator.Enum.Is

            var filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.Is,
                Value = Severity.Normal
            };
            var func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 456, Severity.Info)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, Severity.Normal)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456, Severity.Info)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, Severity.Normal)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.Enum.IsNot

            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.IsNot,
                Value = Severity.Normal
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 456, Severity.Normal)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, Severity.Info)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456, Severity.Normal)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, Severity.Info)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = null,
                Value = Severity.Normal
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 456, Severity.Normal)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, Severity.Info)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));
        }

        [Test]
        public async Task FilterDefinitionEnumForDictionaryTest()
        {
            #region FilterOperator.Enum.Is

            var filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.Is,
                Value = Severity.Normal,
                FieldType = typeof(Severity)
            };
            var func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Info } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Normal } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.Is,
                Value = null,
                FieldType = typeof(Severity)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Info } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Normal } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", null } }));

            #endregion

            #region FilterOperator.Enum.IsNot

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.IsNot,
                Value = Severity.Normal,
                FieldType = typeof(Severity)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Normal } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Info } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.IsNot,
                Value = null,
                FieldType = typeof(Severity)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Normal } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Info } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", null } }));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = null,
                Value = Severity.Normal,
                FieldType = typeof(Severity)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Normal } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", Severity.Info } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Status", null } }));
        }

        [Test]
        public async Task FilterDefinitionDateTimeTest()
        {
            var utcnow = DateTime.UtcNow;

            #region FilterOperator.DateTime.Is

            var filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Is,
                Value = utcnow
            };
            var func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.IsNot

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.IsNot,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.After

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.After,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.After,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.OnOrAfter

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.Before

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Before,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Before,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.OnOrBefore

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.Empty

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Empty,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.NotEmpty

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = null,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, null)));
        }

        [Test]
        public async Task FilterDefinitionDateTimeForDictionaryTest()
        {
            var utcnow = DateTime.UtcNow;

            #region FilterOperator.DateTime.Is

            var filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Is,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            var func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Is,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.IsNot

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.IsNot,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.IsNot,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.After

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.After,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.After,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.OnOrAfter

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.Before

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Before,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Before,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.OnOrBefore

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.Empty

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Empty,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Empty,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            #region FilterOperator.DateTime.NotEmpty

            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            // null value
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = null,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsFalse(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = null,
                Value = utcnow,
                FieldType = typeof(DateTime)
            };
            func = filterDefinition.GenerateFilterFunction();
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", utcnow } }));
            Assert.IsTrue(func.Invoke(new Dictionary<string, object> { { "Name", "Sam" }, { "Age", 45 }, { "Date", null } }));
        }

        [Test]
        public async Task FilterDefinitionStringExpressionTest()
        {
            var filterDefinition = new FilterDefinition<TestModel1>();
            Expression<Func<TestModel1, bool>> expression = null;

            #region FilterOperator.String.Contains

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func =expression.Compile();
            Assert.IsFalse(func.Invoke(new("Does not contain", 45)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));
            Assert.IsFalse(func.Invoke(new(null, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func2 = expression.Compile();
            Assert.IsTrue(func2.Invoke(new("Does not contain", 45)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45)));
            Assert.IsTrue(func2.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.NotContains

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotContains,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func3 = expression.Compile();
            Assert.IsTrue(func3.Invoke(new("Does not contain", 45)));
            Assert.IsFalse(func3.Invoke(new("Joe", 45)));
            Assert.IsFalse(func3.Invoke(new(null, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotContains,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func4 = expression.Compile();
            Assert.IsTrue(func4.Invoke(new("Does not contain", 45)));
            Assert.IsTrue(func4.Invoke(new("Joe", 45)));
            Assert.IsTrue(func4.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.Equal

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func5 = expression.Compile();
            Assert.IsFalse(func5.Invoke(new("Not Joe", 45)));
            Assert.IsFalse(func5.Invoke(new(null, 45)));
            Assert.IsTrue(func5.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Equal,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func6 = expression.Compile();
            Assert.IsTrue(func6.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func6.Invoke(new("Joe", 45)));
            Assert.IsTrue(func6.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.NotEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func7 = expression.Compile();
            Assert.IsTrue(func7.Invoke(new("Not Joe", 45)));
            Assert.IsFalse(func7.Invoke(new(null, 45)));
            Assert.IsFalse(func7.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEqual,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func8 = expression.Compile();
            Assert.IsTrue(func8.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func8.Invoke(new("Joe", 45)));
            Assert.IsTrue(func8.Invoke(new(null, 45)));

            #endregion

            #region FilterOperator.String.StartsWith

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func9 = expression.Compile();
            Assert.IsFalse(func9.Invoke(new("Not Joe", 45)));
            Assert.IsFalse(func9.Invoke(new(null, 45)));
            Assert.IsTrue(func9.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.StartsWith,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func10 = expression.Compile();
            Assert.IsTrue(func10.Invoke(new("Not Joe", 45)));
            Assert.IsTrue(func10.Invoke(new(null, 45)));
            Assert.IsTrue(func10.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.String.EndsWith

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func11 = expression.Compile();
            Assert.IsFalse(func11.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func11.Invoke(new(null, 45)));
            Assert.IsTrue(func11.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.EndsWith,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func12 = expression.Compile();
            Assert.IsTrue(func12.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func12.Invoke(new(null, 45)));
            Assert.IsTrue(func12.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.String.Empty

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.Empty,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func13 = expression.Compile();
            Assert.IsFalse(func13.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func13.Invoke(new("", 45)));
            Assert.IsTrue(func13.Invoke(new(null, 45)));
            Assert.IsTrue(func13.Invoke(new(String.Empty, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func14 = expression.Compile();
            Assert.IsTrue(func14.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func14.Invoke(new("", 45)));
            Assert.IsFalse(func14.Invoke(new(null, 45)));
            Assert.IsFalse(func14.Invoke(new(String.Empty, 45)));

            #endregion

            #region FilterOperator.String.NotEmpty

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func15 = expression.Compile();
            Assert.IsTrue(func15.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func15.Invoke(new("", 45)));
            Assert.IsFalse(func15.Invoke(new(null, 45)));
            Assert.IsFalse(func15.Invoke(new(String.Empty, 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func16 = expression.Compile();
            Assert.IsTrue(func16.Invoke(new("Joe Not", 45)));
            Assert.IsFalse(func16.Invoke(new("", 45)));
            Assert.IsFalse(func16.Invoke(new(null, 45)));
            Assert.IsFalse(func16.Invoke(new(String.Empty, 45)));

            #endregion

            // handle null operator
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = null,
                Value = "Joe"
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func17 = expression.Compile();
            Assert.IsTrue(func17.Invoke(new("Joe Not", 45)));
            Assert.IsTrue(func17.Invoke(new(null, 45)));
            Assert.IsTrue(func17.Invoke(new("Joe", 45)));
        }


        [Test]
        public async Task FilterDefinitionNumberExpressionTest()
        {
            var filterDefinition = new FilterDefinition<TestModel1>();
            Expression<Func<TestModel1, bool>> expression = null;

            #region FilterOperator.Number.Equal

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.Equal,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func = expression.Compile();
            Assert.IsFalse(func.Invoke(new("Sam", 456)));
            Assert.IsFalse(func.Invoke(new("Sam", null)));
            Assert.IsTrue(func.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.Equal,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func2 = expression.Compile();
            // data type is an int
            Assert.IsTrue(func2.Invoke(new("Sam", 456)));
            Assert.IsTrue(func2.Invoke(new("Sam", null)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.Number.NotEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.NotEqual,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func3 = expression.Compile();
            Assert.IsTrue(func3.Invoke(new("Sam", 456)));
            Assert.IsTrue(func3.Invoke(new("Sam", null)));
            Assert.IsFalse(func3.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.NotEqual,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func4 = expression.Compile();
            Assert.IsTrue(func4.Invoke(new("Sam", 456)));
            Assert.IsTrue(func4.Invoke(new("Sam", null)));
            Assert.IsTrue(func4.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.Number.GreaterThan

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThan,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func5 = expression.Compile();
            Assert.IsTrue(func5.Invoke(new("Sam", 456)));
            Assert.IsFalse(func5.Invoke(new("Joe", 45)));
            Assert.IsFalse(func5.Invoke(new("Joe", null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThan,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func6 = expression.Compile();
            Assert.IsTrue(func6.Invoke(new("Sam", 456)));
            Assert.IsTrue(func6.Invoke(new("Joe", 45)));
            Assert.IsTrue(func6.Invoke(new("Joe", null)));

            #endregion

            #region FilterOperator.Number.LessThan

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThan,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func7 = expression.Compile();
            Assert.IsTrue(func7.Invoke(new("Sam", 4)));
            Assert.IsFalse(func7.Invoke(new("Joe", 45)));
            Assert.IsFalse(func7.Invoke(new("Joe", null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThan,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func8 = expression.Compile();
            Assert.IsTrue(func8.Invoke(new("Sam", 4)));
            Assert.IsTrue(func8.Invoke(new("Joe", 45)));
            Assert.IsTrue(func8.Invoke(new("Joe", null)));

            #endregion

            #region FilterOperator.Number.GreaterThanOrEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func9 = expression.Compile();
            Assert.IsFalse(func9.Invoke(new("Sam", 4)));
            Assert.IsFalse(func9.Invoke(new("Sam", null)));
            Assert.IsTrue(func9.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func10 = expression.Compile();
            Assert.IsTrue(func10.Invoke(new("Sam", 4)));
            Assert.IsTrue(func10.Invoke(new("Sam", null)));
            Assert.IsTrue(func10.Invoke(new("Joe", 45)));

            #endregion

            #region FilterOperator.Number.LessThanOrEqual

            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func11 = expression.Compile();
            Console.WriteLine(func11.Invoke(new("Joe", 45)));
            Assert.IsFalse(func11.Invoke(new("Sam", 46)));
            Assert.IsFalse(func11.Invoke(new("Sam", null)));
            Assert.IsTrue(func11.Invoke(new("Joe", 45)));

            // null value
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func12 = expression.Compile();
            Console.WriteLine(func12.Invoke(new("Joe", 45)));
            Assert.IsTrue(func12.Invoke(new("Sam", 46)));
            Assert.IsTrue(func12.Invoke(new("Sam", null)));
            Assert.IsTrue(func12.Invoke(new("Joe", 45)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel1>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = null,
                Value = 45
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func13 = expression.Compile();
            Assert.IsTrue(func13.Invoke(new("Sam", 456)));
            Assert.IsTrue(func13.Invoke(new("Sam", null)));
            Assert.IsTrue(func13.Invoke(new("Joe", 45)));
        }


        [Test]
        public async Task FilterDefinitionBoolExpressionTest()
        {
            #region FilterOperator.Boolean.Is

            Expression<Func<TestModel4, bool>> expression = null;
            var filterDefinition = new FilterDefinition<TestModel4>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = FilterOperator.Boolean.Is,
                Value = true
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func = expression.Compile();
            Assert.IsFalse(func.Invoke(new("Sam", 45, false)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, true)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel4>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = FilterOperator.Boolean.Is,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func2 = expression.Compile();
            Assert.IsTrue(func2.Invoke(new("Sam", 45, false)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45, true)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45, null)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel4>
            {
                Id = Guid.NewGuid(),
                Field = "Hired",
                Operator = null,
                Value = true
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func3 = expression.Compile();
            Assert.IsTrue(func3.Invoke(new("Sam", 45, false)));
            Assert.IsTrue(func3.Invoke(new("Joe", 45, true)));
            Assert.IsTrue(func3.Invoke(new("Joe", 45, null)));
        }

        [Test]
        public async Task FilterDefinitionEnumExpressionTest()
        {
            #region FilterOperator.Enum.Is

            Expression<Func<TestModel3, bool>> expression = null;
            var filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.Is,
                Value = Severity.Normal
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func = expression.Compile();
            Assert.IsFalse(func.Invoke(new("Sam", 456, Severity.Info)));
            Assert.IsTrue(func.Invoke(new("Joe", 45, Severity.Normal)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.Is,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func2 = expression.Compile();
            Assert.IsTrue(func2.Invoke(new("Sam", 456, Severity.Info)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45, Severity.Normal)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.Enum.IsNot

            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.IsNot,
                Value = Severity.Normal
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func3 = expression.Compile();
            Assert.IsFalse(func3.Invoke(new("Sam", 456, Severity.Normal)));
            Assert.IsTrue(func3.Invoke(new("Joe", 45, Severity.Info)));
            Assert.IsTrue(func3.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = FilterOperator.Enum.IsNot,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func4 = expression.Compile();
            Assert.IsTrue(func4.Invoke(new("Sam", 456, Severity.Normal)));
            Assert.IsTrue(func4.Invoke(new("Joe", 45, Severity.Info)));
            Assert.IsTrue(func4.Invoke(new("Joe", 45, null)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel3>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = null,
                Value = Severity.Normal
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func5 = expression.Compile();
            Assert.IsTrue(func5.Invoke(new("Sam", 456, Severity.Normal)));
            Assert.IsTrue(func5.Invoke(new("Joe", 45, Severity.Info)));
            Assert.IsTrue(func5.Invoke(new("Joe", 45, null)));
        }

        [Test]
        public async Task FilterDefinitionDateTimeExpressionTest()
        {
            var utcnow = DateTime.UtcNow;
            Expression<Func<TestModel2, bool>> expression = null;
            #region FilterOperator.DateTime.Is

            var filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Is,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func = expression.Compile();
            Assert.IsTrue(func.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Is,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func2 = expression.Compile();
            Assert.IsTrue(func2.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func2.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.IsNot

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.IsNot,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func3 = expression.Compile();
            Assert.IsFalse(func3.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func3.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.IsNot,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func4 = expression.Compile();
            Assert.IsTrue(func4.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func4.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.After

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.After,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func5 = expression.Compile();
            Assert.IsFalse(func5.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func5.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.After,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func6 = expression.Compile();
            Assert.IsTrue(func6.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func6.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.OnOrAfter

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func7 = expression.Compile();
            Assert.IsTrue(func7.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func7.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func8 = expression.Compile();
            Assert.IsTrue(func8.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func8.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.Before

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Before,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func9 = expression.Compile();
            Assert.IsFalse(func9.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func9.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Before,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func10 = expression.Compile();
            Assert.IsTrue(func10.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func10.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.OnOrBefore

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func11 = expression.Compile();
            Assert.IsTrue(func11.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func11.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func12 = expression.Compile();
            Assert.IsTrue(func12.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func12.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.Empty

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Empty,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func13 = expression.Compile();
            Assert.IsFalse(func13.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func13.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.Empty,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func14 = expression.Compile();
            Assert.IsFalse(func14.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func14.Invoke(new("Joe", 45, null)));

            #endregion

            #region FilterOperator.DateTime.NotEmpty

            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func15 = expression.Compile();
            Assert.IsTrue(func15.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func15.Invoke(new("Joe", 45, null)));

            // null value
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = null
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func16 = expression.Compile();
            Assert.IsTrue(func16.Invoke(new("Sam", 45, utcnow)));
            Assert.IsFalse(func16.Invoke(new("Joe", 45, null)));

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<TestModel2>
            {
                Id = Guid.NewGuid(),
                Field = "Date",
                Operator = null,
                Value = utcnow
            };
            expression = filterDefinition.GenerateFilterExpression();
            var func17 = expression.Compile();
            Assert.IsTrue(func17.Invoke(new("Sam", 45, utcnow)));
            Assert.IsTrue(func17.Invoke(new("Joe", 45, null)));
        }

        [Test]
        public async Task DataGridFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            // test filter definition on the Name property (string contains)
            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = "contains",
                Value = "John"
            };
            // test filter definition on the Age property (int >)
            var filterDefinition2 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = ">",
                Value = 30
            };
            // test filter definition on the Status property (Enum is)
            var filterDefinition3 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = "is",
                Value = Severity.Normal
            };

            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition2));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition3));
            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            var filters = comp.FindComponents<Filter<DataGridFiltersTest.Model>>();

            // assertions for string
            Assert.AreEqual(filterDefinition.Id, filters[0].Instance.Id);
            Assert.AreEqual(filterDefinition.Field, filters[0].Instance.Field);
            Assert.AreEqual(filterDefinition.Operator, filters[0].Instance.Operator);
            Assert.AreEqual(filterDefinition.Value, filters[0].Instance.Value);
            filters[0].Instance.Value = "Not Joe";
            await comp.InvokeAsync(async () => await filters[0].Instance.ValueChanged.InvokeAsync(filters[0].Instance.Value));
            Assert.AreEqual(filterDefinition.Value, "Not Joe");

            // assertions for int
            Assert.AreEqual(filterDefinition2.Id, filters[1].Instance.Id);
            Assert.AreEqual(filterDefinition2.Field, filters[1].Instance.Field);
            Assert.AreEqual(filterDefinition2.Operator, filters[1].Instance.Operator);
            Assert.AreEqual(filterDefinition2.Value, filters[1].Instance.Value);
            filters[1].Instance.Value = 45;
            await comp.InvokeAsync(async () => await filters[1].Instance.ValueChanged.InvokeAsync(filters[1].Instance.Value));
            Assert.AreEqual(filterDefinition2.Value, 45);

            // assertions for Enum
            Assert.AreEqual(filterDefinition3.Id, filters[2].Instance.Id);
            Assert.AreEqual(filterDefinition3.Field, filters[2].Instance.Field);
            Assert.AreEqual(filterDefinition3.Operator, filters[2].Instance.Operator);
            Assert.AreEqual(filterDefinition3.Value, filters[2].Instance.Value);
            filters[2].Instance.Value = Severity.Error;
            await comp.InvokeAsync(async () => await filters[2].Instance.ValueChanged.InvokeAsync(filters[2].Instance.Value));
            Assert.AreEqual(filterDefinition3.Value, Severity.Error);

            // check the number of filters displayed in the filters panel
            dataGrid.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(3);

            // click the Add Filter button in the filters panel to add a filter
            dataGrid.FindAll(".filters-panel > button")[0].Click();

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            dataGrid.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(4);

            // add a filter via the AddFilter method
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            dataGrid.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(5);

            // add a filter via the AddFilter method
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter(Guid.NewGuid(), "Status"));

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            dataGrid.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(6);

            // Changes from the UI
            var selects = filters[1].FindComponents<MudSelect<string>>();
            var input = selects[1].Find("div.mud-input-control");
            // change the operator to "="
            input.Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0), TimeSpan.FromSeconds(10));
            var items = comp.FindAll("div.mud-list-item").ToArray();
            items[0].Click();
            // change the operator to "!="
            input.Click();
            comp.WaitForAssertion(() => comp.FindAll("div.mud-list-item").Count.Should().BeGreaterThan(0), TimeSpan.FromSeconds(10));
            items = comp.FindAll("div.mud-list-item").ToArray();
            items[1].Click();

            await comp.InvokeAsync(() => filters[0].Instance.StringValueChanged("test"));
            await comp.InvokeAsync(() => filters[1].Instance.NumberValueChanged(55));

            filterDefinition.Value.Should().Be("test");
            filterDefinition2.Value.Should().Be(55);

            await comp.InvokeAsync(() => filters[0].Instance.RemoveFilter());
            filters = comp.FindComponents<Filter<DataGridFiltersTest.Model>>();
            filters.Count.Should().Be(5);

            // toggle the filters menu (should be closed after this)
            await comp.InvokeAsync(() => dataGrid.Instance.ToggleFiltersMenu());
            dataGrid.FindAll(".filters-panel").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridIDictionaryFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridIDictionaryFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<IDictionary<string, object>>>();

            // test filter definition on the Name property (string contains)
            var filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Name",
                Operator = "contains",
                Value = "John"
            };
            // test filter definition on the Age property (int >)
            var filterDefinition2 = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Age",
                Operator = ">",
                Value = 30
            };
            // test filter definition on the Status property (Enum is)
            var filterDefinition3 = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                Field = "Status",
                Operator = "is",
                Value = Severity.Normal
            };

            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition2));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition3));
            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            var filters = dataGrid.Instance.FilterDefinitions;

            // assertions for string
            Assert.AreEqual(filterDefinition.Id, filters[0].Id);
            Assert.AreEqual(filterDefinition.Field, filters[0].Field);
            Assert.AreEqual(filterDefinition.Operator, filters[0].Operator);
            Assert.AreEqual(filterDefinition.Value, filters[0].Value);
            filters[0].Value = "Not Joe";
            Assert.AreEqual(filterDefinition.Value, "Not Joe");

            // assertions for int
            Assert.AreEqual(filterDefinition2.Id, filters[1].Id);
            Assert.AreEqual(filterDefinition2.Field, filters[1].Field);
            Assert.AreEqual(filterDefinition2.Operator, filters[1].Operator);
            Assert.AreEqual(filterDefinition2.Value, filters[1].Value);
            filters[1].Value = 45;
            Assert.AreEqual(filterDefinition2.Value, 45);

            // assertions for Enum
            Assert.AreEqual(filterDefinition3.Id, filters[2].Id);
            Assert.AreEqual(filterDefinition3.Field, filters[2].Field);
            Assert.AreEqual(filterDefinition3.Operator, filters[2].Operator);
            Assert.AreEqual(filterDefinition3.Value, filters[2].Value);
            filters[2].Value = Severity.Error;
            Assert.AreEqual(filterDefinition3.Value, Severity.Error);
        }

        [Test]
        public async Task DataGridColGroupTest()
        {
            var comp = Context.RenderComponent<DataGridColGroupTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColGroupTest.Model>>();

            dataGrid.FindAll("col").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridHeaderTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridHeaderTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHeaderTemplateTest.Model>>();

            Console.WriteLine(dataGrid.Markup);

            dataGrid.Find("thead th").TextContent.Trim().Should().Be("test");

            dataGrid.Find("span.column-header").FirstChild.NodeName.Should().Be("svg");
            dataGrid.Find("span.column-header span").TextContent.Should().Be("Name");
        }

        [Test]
        public async Task DataGridChildRowContentTest()
        {
            var comp = Context.RenderComponent<DataGridChildRowContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridChildRowContentTest.Model>>();

            dataGrid.FindAll("td")[3].TextContent.Trim().Should().StartWith("uid = Sam|56|Normal|");
        }

        [Test]
        public async Task DataGridLoadingTest()
        {
            var comp = Context.RenderComponent<DataGridLoadingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridLoadingTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("Data loading, please wait...");
        }

        [Test]
        public async Task DataGridNoRecordsContentTest()
        {
            var comp = Context.RenderComponent<DataGridNoRecordsContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridNoRecordsContentTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("There are no records to view.");
        }

        [Test]
        public async Task DataGridFooterTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridFooterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFooterTemplateTest.Model>>();

            //Console.WriteLine(dataGrid.Markup);

            dataGrid.FindAll("tfoot td").First().TextContent.Trim().Should().Be("Names: Sam, Alicia, Ira, John");
            dataGrid.FindAll("tfoot td").Last().TextContent.Trim().Should().Be($"Highest: {132000:C0} | 2 Over {100000:C0}");
        }

        [Test]
        public async Task DataGridServerDataQuickFilterTest()
        {
            var comp = Context.RenderComponent<DataGridServerDataQuickFilterTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataQuickFilterTest.Item>>();

            dataGrid.FindAll("tr").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridServerPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridServerPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerPaginationTest.Model>>();

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("Name 0");

            // click to go to the next page
            dataGrid.FindAll(".mud-table-pagination-actions button")[2].Click();

            // test that we are on the second page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("Name 10");

            // test reloading server side results programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.ReloadServerData());
        }

        [Test]
        public async Task DataGridCellTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridCellTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellTemplateTest.Model>>();

            //Console.WriteLine(dataGrid.Markup);

            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("45");
        }

        [Test]
        public async Task DataGridColumnChooserTest()
        {
            var comp = Context.RenderComponent<DataGridColumnChooserTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnChooserTest.Model>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();
            var popover = dataGrid.FindComponent<MudPopover>();

            //Console.WriteLine(dataGrid.FindAll(".mud-table-head th").ToMarkup());

            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);
            await comp.InvokeAsync(() =>
            {
                var columnHamburger = dataGrid.FindAll("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
                columnHamburger[2].Click();

                var listItems = popoverProvider.FindComponents<MudListItem>();
                listItems.Count.Should().Be(2);
                var clickablePopover = listItems[1].Find(".mud-list-item");
                clickablePopover.Click();

                //dataGrid.Instance._columns[0].Hide();
                dataGrid.Instance.ExternalStateHasChanged();
            });
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(1);
            await comp.InvokeAsync(() =>
            {
                var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
                columnsButton.Click();

                popover.Instance.Open.Should().BeTrue("Should be open once clicked");
                var listItems = popoverProvider.FindComponents<MudListItem>();
                listItems.Count.Should().Be(1);
                var clickablePopover = listItems[0].Find(".mud-list-item");
                clickablePopover.Click();

                // at this point, the column picker should be open
                var switches = dataGrid.FindComponents<MudSwitch<bool>>();
                switches.Count.Should().Be(2);

                var buttons = dataGrid.FindComponents<MudButton>();
                // this is the show all button
                buttons[1].Find("button").Click();
                // 2 columns, 0 hidden
                dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);

                //dataGrid.Instance._columns[0].Hide();
                dataGrid.Instance.ExternalStateHasChanged();
            });
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);

            await comp.InvokeAsync(() => dataGrid.Instance.ShowColumnsPanel());
            dataGrid.FindAll(".mud-paper.columns-panel").Count.Should().Be(1);
            await comp.InvokeAsync(() => dataGrid.Instance.HideColumnsPanel());
            dataGrid.FindAll(".mud-paper.columns-panel").Count.Should().Be(0);

            await comp.InvokeAsync(() => dataGrid.Instance.HideAllColumnsAsync());
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(0);
            await comp.InvokeAsync(() => dataGrid.Instance.ShowAllColumnsAsync());
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);
        }

        [Test]
        public async Task DataGridColumnHiddenTest()
        {
            var comp = Context.RenderComponent<DataGridColumnHiddenTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnHiddenTest.Model>>();

            //Console.WriteLine(dataGrid.FindAll(".mud-table-head th").ToMarkup());

            var popoverProvider = comp.FindComponent<MudPopoverProvider>();
            var popover = dataGrid.FindComponent<MudPopover>();
            popover.Instance.Open.Should().BeFalse("Should start as closed");

           

            var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
            columnsButton.Click();

            popover.Instance.Open.Should().BeTrue("Should be open once clicked");
            var listItems = popoverProvider.FindComponents<MudListItem>();
            listItems.Count.Should().Be(1);
            var clickablePopover = listItems[0].Find(".mud-list-item");
            clickablePopover.Click();

            // at this point, the column picker should be open
            var switches = dataGrid.FindComponents<MudSwitch<bool>>();
            switches.Count.Should().Be(2);

            switches[0].Instance.Checked.Should().BeFalse();
            switches[1].Instance.Checked.Should().BeTrue();

            var buttons = dataGrid.FindComponents<MudButton>();

            // this is the hide all button
            buttons[0].Find("button").Click();
            switches[0].Instance.Checked.Should().BeTrue();
            switches[1].Instance.Checked.Should().BeTrue();
            // 2 columns, 2 hidden
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(0);


            // this is the show all button
            buttons[1].Find("button").Click();
            switches[0].Instance.Checked.Should().BeFalse();
            switches[1].Instance.Checked.Should().BeFalse();
            // 2 columns, 0 hidden
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);
        }

        [Test]
        public async Task DataGridShowMenuIconTest()
        {
            var comp = Context.RenderComponent<DataGridShowMenuIconTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridShowMenuIconTest.Item>>();
            dataGrid.FindAll(".mud-table-toolbar .mud-menu").Should().BeEmpty();
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ShowMenuIcon), true));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.FindAll(".mud-table-toolbar .mud-menu").Should().NotBeEmpty();
        }

        [Test]
        public async Task DataGridColumnPopupFilteringTest()
        {
            var comp = Context.RenderComponent<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Find(".filter-button").Click();
            var input = comp.FindComponent<MudTextField<string>>();
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(input.Instance.Value), "test"));
            input.SetParametersAndRender(parameters.ToArray());
            comp.Find(".apply-filter-button").Click();

            // Cannot figure out why the above is not working...
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(new FilterDefinition<DataGridColumnPopupFilteringTest.Model>
            {
                Field = "Name",
                Operator = FilterOperator.String.Contains,
                Value = "test"
            }));

            dataGrid.Render();
            dataGrid.FindAll("tbody tr").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridColumnPopupCustomFilteringTest()
        {
            var comp = Context.RenderComponent<DataGridColumnPopupCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupCustomFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Instance.FilterHired = true;
            await comp.InvokeAsync(() =>
            {
                var filterContext = dataGrid.Instance.RenderedColumns[3].filterContext;
                comp.Instance.ApplyFilter(filterContext);
            });

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCustomFilteringTest()
        {
            var comp = Context.RenderComponent<DataGridCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Instance.FilterHiredToggled(true, dataGrid.Instance.FilterDefinitions);
            dataGrid.Render();
            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridShowFilterIconTest()
        {
            var comp = Context.RenderComponent<DataGridCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomFilteringTest.Model>>();
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.Filterable), false));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.FindAll(".filter-button").Should().BeEmpty();
            parameters.Clear();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.Filterable), true));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.FindAll(".filter-button").Should().NotBeEmpty();
            parameters.Clear();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ShowFilterIcons), false));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.FindAll(".filter-button").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridStickyColumnsTest()
        {
            var comp = Context.RenderComponent<DataGridStickyColumnsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridStickyColumnsTest.Model>>();

            dataGrid.Find("th").ClassList.Should().Contain("sticky-left");
            dataGrid.FindAll("th").Last().ClassList.Should().Contain("sticky-right");
        }

        [Test]
        public async Task DataGridCellContextTest()
        {
            var comp = Context.RenderComponent<DataGridCellContextTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellContextTest.Model>>();

            var item = dataGrid.Instance.Items.FirstOrDefault();

            var column = dataGrid.Instance.RenderedColumns.First();
            var cell = new Cell<DataGridCellContextTest.Model>(dataGrid.Instance, column, item);

            cell.cellContext.IsSelected.Should().Be(false);
            cell.cellContext.Actions.SetSelectedItem(true);
            cell.cellContext.IsSelected.Should().Be(true);
        }

        [Test]
        public async Task DataGridAggregationTest()
        {
            var comp = Context.RenderComponent<DataGridAggregationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridAggregationTest.Model>>();

            //Console.WriteLine(dataGrid.Markup);

            dataGrid.FindAll("td.footer-cell")[1].TrimmedText().Should().Be("Average age is 56");
            dataGrid.FindAll("tfoot td.footer-cell")[1].TrimmedText().Should().Be("Average age is 43");
        }
        
        [Test]
        public async Task DataGridSequenceContainsNoElementsTest()
        {
            var comp = Context.RenderComponent<DataGridSequenceContainsNoElementsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSequenceContainsNoElementsTest.Model>>();

            // This test will result in an error if the 'sequence contains no elements' issue is present.
        }

        [Test]
        public async Task DataGridObservabilityTest()
        {
            var comp = Context.RenderComponent<DataGridObservabilityTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridObservabilityTest.Model>>();

            var addButton = comp.Find(".add-item-btn");
            var removeButton = comp.Find(".remove-item-btn");

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);

            addButton.Click();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(9);

            removeButton.Click();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);
        }

        public async Task TableFilterGuid()
        {
            var comp = Context.RenderComponent<DataGridFilterGuid<Guid>>();
            var grid = comp.Instance.MudGridRef;
            
            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Field = "Id",
                Operator = "equals",
                Value = "invalid guid",
                FieldType = typeof(Guid),
            });
            grid.FilteredItems.Count().Should().Be(0);
            
            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Field = "Id",
                Operator = "equals",
                Value = comp.Instance.Guid1,
                FieldType = typeof(Guid),
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid1);
            
            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Field = "Id",
                Operator = "not equals",
                Value = comp.Instance.Guid1,
                FieldType = typeof(Guid),
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid2);
        }

        [Test]
        public async Task TableFilterNullableGuid()
        {
            var comp = Context.RenderComponent<DataGridFilterGuid<Nullable<Guid>>>();
            var grid = comp.Instance.MudGridRef;
            
            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Nullable<Guid>>.WeatherForecast>()
            {
                Field = "Id",
                Operator = "equals",
                Value = "invalid guid",
                FieldType = typeof(Nullable<Guid>),
            });
            grid.FilteredItems.Count().Should().Be(0);
            
            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Nullable<Guid>>.WeatherForecast>()
            {
                Field = "Id",
                Operator = "equals",
                Value = comp.Instance.Guid1,
                FieldType = typeof(Nullable<Guid>),
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid1);
            
            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Nullable<Guid>>.WeatherForecast>()
            {
                Field = "Id",
                Operator = "not equals",
                Value = comp.Instance.Guid1,
                FieldType = typeof(Nullable<Guid>),
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid2);
        }

        [Test]
        public async Task TableFilterGuidInDictionary()
        {
            var comp = Context.RenderComponent<DataGridFilterDictionaryGuid>();
            var grid = comp.Instance.MudGridRef;
            
            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            
            grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
            {
                Field = "Id",
                Operator = "equals",
                Value = "invalid guid",
                FieldType = typeof(Nullable<Guid>),
            });
            grid.FilteredItems.Count().Should().Be(0);
            
            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
            {
                Field = "Id",
                Operator = "equals",
                Value = comp.Instance.Guid1,
                FieldType = typeof(Nullable<Guid>),
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()["Id"].Should().Be(Guid.Parse(comp.Instance.Guid1));
            
            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
            {
                Field = "Id",
                Operator = "not equals",
                Value = comp.Instance.Guid1,
                FieldType = typeof(Nullable<Guid>),
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()["Id"].Should().Be(Guid.Parse(comp.Instance.Guid2));
        }
    }
}
