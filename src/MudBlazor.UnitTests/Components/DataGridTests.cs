#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

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
            dataGrid.FindAll("tr").Count.Should().Be(5); // header row + three rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(SortDirection.Ascending, x => { return x.Name; }, "Name"));

            // Check the values of rows - should be sorted ascending by Name.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(SortDirection.Descending, x => { return x.Name; }, "Name"));

            // Check the values of rows - should be sorted descending by Name.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("A");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(SortDirection.None, x => { return x.Name; }, "Name"));

            var column = dataGrid.FindComponent<Column<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => column.Instance.SortBy = x => { return x.Name; });
            await comp.InvokeAsync(() => column.Instance.CompileSortBy());

            // Check the values of rows - should not be sorted and should be in the original order.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            // sort through the sort icon
            dataGrid.Find(".column-options button").Click();
            // Check the values of rows - should be sorted ascending by Name.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");

            // test other sort methods
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync());
            await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter());
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters());

            await comp.InvokeAsync(() => dataGrid.Instance.Sortable = false);
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
        public async Task DataGridInlineEditWithTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditWithTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithTemplateTest.Model>>();

            //Console.WriteLine(dataGrid.FindAll("td input")[2].HasAttribute("checked"));

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
            var comp = Context.RenderComponent<DataGridServerSideSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerSideSortableTest.Item>>();

            // Check the values of rows - should be the default order of the items.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("2");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[4].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[5].TextContent.Trim().Should().Be("2");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(SortDirection.Ascending, x => { return x.A; }, "A"));

            // Check the values of rows - should be sorted ascending by A.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[4].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[5].TextContent.Trim().Should().Be("2");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(SortDirection.Descending, x => { return x.A; }, "A"));

            //Console.WriteLine(dataGrid.Markup);

            // Check the values of rows - should be sorted descending by A.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("2");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("2");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[4].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[5].TextContent.Trim().Should().Be("1");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(SortDirection.None, x => { return x.A; }, "A"));

            //Console.WriteLine(dataGrid.Markup);

            // Check the values of rows - should be the default order of the items.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("2");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("1");
            dataGrid.FindAll("td")[4].TextContent.Trim().Should().Be("3");
            dataGrid.FindAll("td")[5].TextContent.Trim().Should().Be("2");

            await comp.InvokeAsync(() => dataGrid.Instance.Sortable = false);
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

            //Console.WriteLine(dataGrid.FindAll(".mud-table-head th").ToMarkup());

            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);
            await comp.InvokeAsync(() =>
            {
                dataGrid.Instance._columns[0].Hide();
                dataGrid.Instance.ExternalStateHasChanged();
            });
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(1);
            await comp.InvokeAsync(() =>
            {
                dataGrid.Instance._columns[0].Show();
                dataGrid.Instance.ExternalStateHasChanged();
            });
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);

            await comp.InvokeAsync(() => dataGrid.Instance.ShowColumnsPanel());
            dataGrid.FindAll(".mud-paper.columns-panel").Count.Should().Be(1);
            await comp.InvokeAsync(() => dataGrid.Instance.HideColumnsPanel());
            dataGrid.FindAll(".mud-paper.columns-panel").Count.Should().Be(0);

            await comp.InvokeAsync(() => dataGrid.Instance.HideAllColumns());
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(0);
            await comp.InvokeAsync(() => dataGrid.Instance.ShowAllColumns());
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(2);
        }
    }
}
