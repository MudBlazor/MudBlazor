// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.UnitTests.TestComponents.DataGrid;
using MudBlazor.Utilities.Clone;
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
        [SetCulture("")]
        [SetUICulture("")]
        public void DataGridPropertyNullCheck()
        {
            var comp = Context.Render<DataGridPropertyColumnNullCheckTest>();
            var cells = comp.FindAll("td").ToArray();

            // First Row
            cells[0].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[1].TextContent.Should().BeEmpty();
            cells[2].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[3].TextContent.Should().BeEmpty();
            cells[4].TextContent.Should().BeEmpty();
            cells[5].TextContent.Should().BeEmpty();

            // Second Row
            cells[6].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[7].TextContent.Should().Be("01/01/0001 00:00:00 +00:00");
            cells[8].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[9].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[10].TextContent.Should().Be("some text");
            cells[11].TextContent.Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSortable()
        {
            var comp = Context.Render<DataGridSortableTest>();
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
            await column.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortBy, x => x.Name));
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
            await dataGrid.Find(".column-options button").ClickAsync();
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
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs()));
            //await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter(new MouseEventArgs()));
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters(new MouseEventArgs()));

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortMode, SortMode.None));
            dataGrid.Instance.DropContainerHasChanged();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public void DataGridVirtualizeSpacerElementsAreTableRows()
        {
            var comp = Context.Render<DataGridServerDataWithVirtualizeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataWithVirtualizeTest.Item>>();

            // Virtualize spacer elements must be <tr>, not <div>, so that CSS table layout respects
            // their height. A <div> inside <tbody> has its height ignored, causing scroll-position jumping.
            var tbody = dataGrid.Find("tbody");
            tbody.QuerySelectorAll(":scope > div").Should().BeEmpty(because: "Virtualize spacers that are direct children of <tbody> must be <tr> elements, not <div>s");

            // Virtualize renders one before-spacer and one after-spacer <tr>; neither has the mud-table-row class.
            var spacerTrs = tbody.QuerySelectorAll("tr:not(.mud-table-row)").ToList();
            spacerTrs.Should().HaveCount(2, because: "Virtualize renders exactly one before-spacer and one after-spacer <tr>");
        }

        [Test]
        public void DataGridWithServerDataAndVirtualize()
        {
            var comp = Context.Render<DataGridServerDataWithVirtualizeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataWithVirtualizeTest.Item>>();

            // Count data rows using the mud-table-row class; avoids counting Virtualize spacer <tr> elements.
            var dataRows = dataGrid.FindAll("tbody tr.mud-table-row");
            dataRows.Count.Should().Be(5, because: "5 data rows");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(5, because: "We have 5 data rows with one column");

            cells[0].TextContent.Should().Be("Value_0");
            cells[1].TextContent.Should().Be("Value_1");
            cells[2].TextContent.Should().Be("Value_2");
            cells[3].TextContent.Should().Be("Value_3");
            cells[4].TextContent.Should().Be("Value_4");
        }

        [Test]
        public async Task DataGridSortableVirtualizeServerData()
        {
            var comp = Context.Render<DataGridSortableVirtualizeServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableVirtualizeServerDataTest.Item>>();

            // Count data rows using the mud-table-row class; avoids counting Virtualize spacer <tr> elements.
            var dataRows = dataGrid.FindAll("tbody tr.mud-table-row");
            dataRows.Count.Should().Be(7, because: "7 data rows");

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

            var column = dataGrid.FindComponent<Column<DataGridSortableVirtualizeServerDataTest.Item>>();
            await column.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortBy, x => x.Name));

            // Check the values of rows - should not be sorted and should be in the original order.
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // sort through the sort icon
            await dataGrid.Find(".column-options button").ClickAsync();
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
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableVirtualizeServerDataTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
            //await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter(new MouseEventArgs()));
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters(new MouseEventArgs()));

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortMode, SortMode.None));
            dataGrid.Instance.DropContainerHasChanged();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSortableHeaderRow()
        {
            var comp = Context.Render<DataGridSortableHeaderRowTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableHeaderRowTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(6, because: "2 header rows + 3 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(9, because: "We have 3 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("B"); cells[7].TextContent.Should().Be("42"); cells[8].TextContent.Should().Be("555");
        }

        [Test]
        public async Task DataGridSortableTemplateColumn()
        {
            var comp = Context.Render<DataGridSortableTemplateColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTemplateColumnTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(9, because: "1 header row + 7 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(7, because: "We have 7 data rows with one column");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("C");
            cells[4].TextContent.Should().Be("A");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");

            // property name is the hash code of the template column
            var templatePropertyName = dataGrid.Instance.RenderedColumns.FirstOrDefault().PropertyName;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(templatePropertyName, SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[4].TextContent.Should().Be("C");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(templatePropertyName, SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by Name.
            cells[0].TextContent.Should().Be("C");
            cells[1].TextContent.Should().Be("C");
            cells[2].TextContent.Should().Be("C");
            cells[3].TextContent.Should().Be("B");
            cells[4].TextContent.Should().Be("A");
            cells[5].TextContent.Should().Be("A");
            cells[6].TextContent.Should().Be("A");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync(templatePropertyName));
            cells = dataGrid.FindAll("td");

            // Back to original order without sorting
            cells[0].TextContent.Should().Be("B");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("C");
            cells[4].TextContent.Should().Be("A");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");

            // sort through the sort icon
            await dataGrid.Find(".column-options button").ClickAsync();
            cells = dataGrid.FindAll("td");
            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[4].TextContent.Should().Be("C");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");
        }

        [Test]
        public async Task DataGridFilterableVirtualizeServerData()
        {
            var comp = Context.Render<DataGridFilterableVirtualizeServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableVirtualizeServerDataTest.Item>>();

            // Count data rows using the mud-table-row class; avoids counting Virtualize spacer <tr> elements.
            dataGrid.FindAll("tbody tr.mud-table-row").Count.Should().Be(4, because: "four data rows");

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterableVirtualizeServerDataTest.Item>
            {
                Column = dataGrid.Instance.RenderedColumns.First(),
                Operator = FilterOperator.String.Equal,
                Value = "C"
            }));

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Filterable, false));
        }

        [Test]
        public async Task DataGridFilterable()
        {
            var comp = Context.Render<DataGridFilterableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6); // header row + four rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterableTest.Item>
            {
                Column = dataGrid.Instance.RenderedColumns.First(),
                Operator = FilterOperator.String.Equal,
                Value = "C"
            }));

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Filterable, false));
        }

        [Test]
        public void DataGrid_SetParameters_ServerData_Items_Throw()
        {
            var serverDataFunc =
                new Func<GridState<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.Render<MudDataGrid<TestModel1>>(parameters => parameters
                    .Add(p => p.ServerData, serverDataFunc)
                    .Add(p => p.Items, Array.Empty<TestModel1>())
                )
            );
            exception.Message.Should().Be(
                """
                MudBlazor.MudDataGrid`1[MudBlazor.UnitTests.Components.TestModel1] can only accept one item source from its parameters. Do not supply both 'Items' and 'ServerData'.
                """
            );
        }

        [Test]
        public void DataGrid_SetParameters_ServerData_QuickFilter_Throw()
        {
            var serverDataFunc =
                new Func<GridState<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.Render<MudDataGrid<TestModel1>>(parameters => parameters
                    .Add(p => p.ServerData, serverDataFunc)
                    .Add(p => p.QuickFilter, (TestModel1 x) => true)
                )
            );
            exception.Message.Should().Be("Do not supply both 'ServerData' and 'QuickFilter'.");
        }

        [Test]
        public void DataGrid_SetParameters_VirtualizeServerData_QuickFilter_Throw()
        {
            var virtualizeServerDataFunc =
                new Func<GridStateVirtualize<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.Render<MudDataGrid<TestModel1>>(parameters => parameters
                    .Add(p => p.VirtualizeServerData, virtualizeServerDataFunc)
                    .Add(p => p.QuickFilter, (TestModel1 x) => true)
                )
            );
            exception.Message.Should().Be("Do not supply both 'VirtualizeServerData' and 'QuickFilter'.");
        }

        [Test]
        public void DataGrid_SetParameters_ServerData_VirtualizeServerData_Throw()
        {
            var serverDataFunc =
                new Func<GridState<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var virtualizeServerDataFunc =
                new Func<GridStateVirtualize<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.Render<MudDataGrid<TestModel1>>(parameters => parameters
                    .Add(p => p.ServerData, serverDataFunc)
                    .Add(p => p.VirtualizeServerData, virtualizeServerDataFunc)
                )
            );

            exception.Message.Should().Be(
                """
                MudBlazor.MudDataGrid`1[MudBlazor.UnitTests.Components.TestModel1] can only accept one item source from its parameters. Do not supply both 'VirtualizeServerData' and 'ServerData'.
                """
            );
        }

        [Test]
        public void DataGrid_SetParameters_Items_VirtualizeServerData_Throw()
        {
            var virtualizeServerDataFunc =
                new Func<GridStateVirtualize<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.Render<MudDataGrid<TestModel1>>(parameters => parameters
                    .Add(p => p.Items, Array.Empty<TestModel1>())
                    .Add(p => p.VirtualizeServerData, virtualizeServerDataFunc)
                )
            );

            exception.Message.Should().Be(
                """
                MudBlazor.MudDataGrid`1[MudBlazor.UnitTests.Components.TestModel1] can only accept one item source from its parameters. Do not supply both 'Items' and 'VirtualizeServerData'.
                """
            );
        }

        [Test]
        public async Task DataGridFilterableServerData()
        {
            var comp = Context.Render<DataGridFilterableServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableServerDataTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6); // header row + four rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            await comp.InvokeAsync(() =>
            {
                return dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterableServerDataTest.Item>
                {
                    Column = dataGrid.Instance.RenderedColumns.FirstOrDefault(),
                    Operator = FilterOperator.String.Equal,
                    Value = "C"
                });
            });

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Filterable, false));
        }

        [Test]
        public async Task DataGridCustomComparer()
        {
            var comp = Context.Render<DataGridSelectionComparerTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionComparerTest.Person>>();

            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            // click the first row
            await dataGrid.FindAll("td")[1].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
            dataGrid.Instance.Selection.Comparer.Should().BeOfType<DataGridSelectionComparerTest.IdComparer>();

            //select a chip
            var chipSet = comp.FindComponent<MudChipSet<string>>();

            await chipSet.FindAll(".mud-chip")[2].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1); //only 1 item is set
            dataGrid.FindAll("input[type=checkbox]").Where(checkbox => checkbox.IsChecked()).ToArray().Length.Should().Be(2); //two items are checked
            dataGrid.Instance.Selection.Comparer.Should().BeOfType<DataGridSelectionComparerTest.RoleComparer>();
        }

        [Test]
        public async Task DataGridSingleSelection()
        {
            var comp = Context.Render<DataGridSingleSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSingleSelectionTest.Item>>();

            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            // select first item programmatically
            var firstItem = dataGrid.Instance.Items.ElementAt(0);
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(true, firstItem));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
            dataGrid.Instance.GetState(x => x.SelectedItem).Should().Be(firstItem);

            // select second item programmatically (still should be only one item selected)
            var secondItem = dataGrid.Instance.Items.ElementAt(1);
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(true, secondItem));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
            dataGrid.Instance.GetState(x => x.SelectedItem).Should().Be(secondItem);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, secondItem));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            dataGrid.Instance.GetState(x => x.SelectedItem).Should().BeNull();
        }

        [Test]
        public async Task DataGridMultiSelection()
        {
            var comp = Context.Render<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("input")[0].ChangeAsync(true);
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(4);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, dataGrid.Instance.GetState(x => x.SelectedItems).First()));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(3);

            // select an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(dataGrid.Instance.Items.First()));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(4);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            // select all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(true));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(4);

            // deselect from the footer
            await dataGrid.Find("tfoot input").ChangeAsync(false);
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
        }

        [Test]
        public void DataGridMultiSelection_Should_Not_Render_Footer_If_ShowInFooter_Is_False()
        {
            var comp = Context.Render<DataGridMultiSelectionTest>(parameters => parameters
                .Add(p => p.ShowInFooter, false));
            comp.FindAll("td.footer-cell").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSelectAllWithFilter()
        {
            var comp = Context.Render<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4, because: "all four rows shown by default");
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0, because: "no selected items by default");

            var twoBFilter = new FilterDefinition<DataGridMultiSelectionTest.Item>
            {
                Column = dataGrid.Instance.RenderedColumns.FirstOrDefault(c => c.PropertyName == "Name"),
                Operator = FilterOperator.String.Equal,
                Value = "B"
            };

            // Add a FilterDefinition to filter where the Name == "B".
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(twoBFilter));

            dataGrid.FindAll("tbody tr").Count.Should().Be(2, because: "two 'B' rows shown per the filter");

            // select-all
            await dataGrid.FindAll("input[type=checkbox]")[0].ChangeAsync(true);
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2, because: "only the two 'B' rows that are visible should get selected");

            await comp.InvokeAsync(() => dataGrid.Instance.ClearFiltersAsync());
            dataGrid.Render();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4, because: "all rows should be shown when filter disapplied");
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2, because: "selection should not have changed when filter disapplied");
            dataGrid.FindAll("input")[0].IsChecked().Should().BeFalse(because: "select all checkbox should reflect 'not all selected' state");
            dataGrid.FindAll("tfoot input")[0].IsChecked().Should().BeFalse(because: "select all checkbox should reflect 'not all selected' state");

            // ClearFiltersAsync() has cleared the value, so it needs to be set again
            twoBFilter.Value = "B";
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(twoBFilter));
            dataGrid.FindAll("input[type=checkbox]")[0].IsChecked().Should().BeTrue(because: "select all checkbox should reflect 'all selected' state");
            dataGrid.FindAll("tfoot input[type=checkbox]")[0].IsChecked().Should().BeTrue(because: "select all checkbox should reflect 'all selected' state");
        }

        [Test]
        public async Task DataGridServerMultiSelection()
        {
            var comp = Context.Render<DataGridServerMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerMultiSelectionTest.Item>>();

            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("input")[0].ChangeAsync(true);
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(3);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, dataGrid.Instance.GetState(x => x.SelectedItems).First()));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);

            // select an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(dataGrid.Instance.ServerItems.First()));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(3);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(true));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(3);

            // deselect from the footer
            await dataGrid.Find("tfoot input").ChangeAsync(false);
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridEditableSelection()
        {
            var comp = Context.Render<DataGridEditableWithSelectColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditableWithSelectColumnTest.Item>>();

            // test that all rows, header and footer have cell with a checkbox
            dataGrid.FindAll("input.mud-checkbox-input").Count.Should().Be(dataGrid.Instance.Items.Count() + 2);

            //test that changing header sets all items selected
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("input.mud-checkbox-input")[0].ChangeAsync(true);
            comp.Render();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(dataGrid.Instance.Items.Count());
            //test that changing footer unselects all items
            await dataGrid.FindAll("input.mud-checkbox-input")[^1].ChangeAsync(false);
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            //test that changing value in each row selects an item in grid
            for (var i = 1; i < dataGrid.Instance.Items.Count(); i++)
            {
                await dataGrid.FindAll("input.mud-checkbox-input")[i].ChangeAsync(true);
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(i);
            }
        }

        [Test]
        public async Task DataGridInlineEditVirtualizeServerData()
        {
            var comp = Context.Render<DataGridCellEditVirtualizeServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditVirtualizeServerDataTest.Item>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Trim().Should().Be("32");
            await dataGrid.FindAll(".mud-table-body tr td input")[0].ChangeAsync("Jonathan");
            await dataGrid.FindAll(".mud-table-body tr td input")[1].ChangeAsync(52d);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");
        }

        /// <summary>
        /// Ensures that multiple calls to reload the data grid data properly flag the CancellationToken.
        /// </summary>
        /// <returns>A <see cref="Task"/> object.</returns>
        [Test]
        public async Task DataGridVirtualizeServerDataLoadingWithCancel()
        {
            var comp = Context.Render<DataGridVirtualizeServerDataLoadingWithCancelTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<int>>();

            // Make a cancellation token we can monitor
            CancellationToken? cancelToken = null;
            // Make a task completion source
            var first = new TaskCompletionSource<GridData<int>>();
            // Set the ServerData function
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(p =>
                p.VirtualizeServerData,
                new Func<GridStateVirtualize<int>, CancellationToken, Task<GridData<int>>>((_, cancellationToken) =>
                {
                    // Remember the cancellation token
                    cancelToken = cancellationToken;
                    // Return a task that never completes
                    return first.Task;
                })));

            await Task.Delay(20);

            // Test

            // Make sure this first request was not canceled
            await comp.WaitForAssertionAsync(() => cancelToken?.IsCancellationRequested.Should().BeFalse());

            // Arrange a server data refresh
            var second = new TaskCompletionSource<GridData<int>>();
            // Set the VirtualizeServerData function to a new method...
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(p =>
                p.VirtualizeServerData,
                new Func<GridStateVirtualize<int>, CancellationToken, Task<GridData<int>>>((_, _) => second.Task)));

            await Task.Delay(20);

            // Test

            // Make sure this second request DID cancel the first request's token
            await comp.WaitForAssertionAsync(() => cancelToken?.IsCancellationRequested.Should().BeTrue());
        }

        [Test]
        public async Task DataGridPagination()
        {
            var comp = Context.Render<DataGridPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            // check that the page size dropdown is shown
            comp.FindComponents<MudSelect<int>>().Count.Should().Be(1);

            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("0");

            // click to go to the next page
            await dataGrid.FindAll(".mud-table-pagination-actions button")[2].ClickAsync();

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
        public void DataGridPaginationShouldFormatNumbersWithCommas()
        {
            var comp = Context.Render<DataGridPaginationFormattingTest>();

            comp.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 1,000");
        }

        [Test]
        public void DataGridPaginationShouldRespectCustomFormatWithSingleTag()
        {
            var comp = Context.Render<DataGridPaginationCustomFormatTest>();

            comp.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("Total: 1,000");
        }

        [Test]
        public void DataGridPaginationShouldUseGridCultureForFormatting()
        {
            // de-DE uses "." as a thousands separator, so 1000 is formatted as "1.000"
            var comp = Context.Render<DataGridPaginationCultureTest>();

            comp.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 1.000");
        }

        [Test]
        public void DataGridPaginationPageSizeDropDown()
        {
            var comp = Context.Render<DataGridPaginationTest>(self => self.Add(x => x.PageSizeDropDown, false));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("0");

            // page size drop-down is not shown
            comp.FindComponents<MudSelect<string>>().Should().BeEmpty();
        }

        /// <summary>
        /// Tests that the "All" data grid pager option shows all items
        /// </summary>
        [Test]
        public async Task DataGridPagingAll()
        {
            var comp = Context.Render<DataGridPaginationAllItemsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationAllItemsTest.Item>>();
            var pager = comp.FindComponent<MudSelect<int>>().Instance;

            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20"); //check initial value
            // change page size
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetRowsPerPageAsync(int.MaxValue));
            pager.Value.Should().Be(int.MaxValue);
            dataGrid.Instance.RowsPerPage.Should().Be(int.MaxValue);
            comp.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-20 of 20");

            comp.FindAll(".mud-table-pagination-actions button")[0].IsDisabled().Should().Be(true); //buttons are disabled
            comp.FindAll(".mud-table-pagination-actions button")[1].IsDisabled().Should().Be(true);
            comp.FindAll(".mud-table-pagination-actions button")[2].IsDisabled().Should().Be(true);
            comp.FindAll(".mud-table-pagination-actions button")[3].IsDisabled().Should().Be(true);

            comp.FindAll(".mud-table-pagination-select")[^1].TextContent.Trim().Should().Be("All");
        }

        [Test]
        public void DataGridPaginationNoItems()
        {
            var comp = Context.Render<DataGridPaginationNoItemsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationNoItemsTest.Item>>();
            // check that the page size dropdown is shown
            comp.FindComponents<MudSelect<int>>().Count.Should().Be(1);

            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("0-0 of 0");
        }

        [Test]
        public async Task DataGridHideNavigation()
        {
            var comp = Context.Render<DataGridPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            var pagerContent = comp.FindComponent<MudDataGridPager<DataGridPaginationTest.Item>>();

            comp.Markup.Should().Contain("mud-table-pagination-actions");
            comp.Markup.Should().Contain("M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z");
            comp.Markup.Should().Contain("1-10 of 20");
            await pagerContent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ShowNavigation, false));
            comp.Markup.Should().NotContain("mud-table-pagination-actions");
            await pagerContent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ShowPageNumber, false));
            comp.Markup.Should().NotContain("1-10 of 20");
        }

        [Test]
        public async Task DataGridRowsPerPageTwoWayBinding()
        {
            var comp = Context.Render<DataGridRowsPerPageBindingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridRowsPerPageBindingTest.Item>>();

            // confirm that BoundRowsPerPage is equal to the initial value of 5 (See DataGridRowsPerPageBindingTest)
            comp.Instance.BoundRowsPerPage.Should().Be(5);

            // programmatically set the datagrid rowsPerPage to 10
            await dataGrid.InvokeAsync(() => dataGrid.Instance.SetRowsPerPageAsync(10));

            // confirm that BoundRowsPerPage changes when rowsPerPage is set to 10
            comp.Instance.BoundRowsPerPage.Should().Be(10);
        }

        [Test]
        public async Task DataGridInlineEdit()
        {
            var comp = Context.Render<DataGridCellEditTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Trim().Should().Be("32");
            await dataGrid.FindAll(".mud-table-body tr td input")[0].ChangeAsync("Jonathan");
            await dataGrid.FindAll(".mud-table-body tr td input")[1].ChangeAsync(52d);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
        }

        [Test]
        public async Task DataGridInlineEditWithNullableChange()
        {
            var comp = Context.Render<DataGridCellEditWithNullableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithNullableTest.Model>>();

            // try setting a value to null
            await dataGrid.FindAll("td input")[1].ChangeAsync("");
            dataGrid.Instance.Items.First().Age.Should().Be(null);

            // try setting the value back to something not null
            await dataGrid.FindAll("td input")[1].ChangeAsync("15");
            dataGrid.Instance.Items.First().Age.Should().Be(15);
        }

        [Test]
        public async Task DataGridInlineEditWithNullable()
        {
            var comp = Context.Render<DataGridCellEditWithNullableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithNullableTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Should().BeNull();
            await dataGrid.FindAll(".mud-table-body tr td input")[0].ChangeAsync("Jonathan");
            await dataGrid.FindAll(".mud-table-body tr td input")[1].ChangeAsync(52);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
        }

        [Test]
        public async Task DataGridInlineEditWithTemplate()
        {
            var comp = Context.Render<DataGridCellEditWithTemplateTest>();
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
            await dataGrid.FindAll("td input")[0].ChangeAsync("Jonathan");
            await dataGrid.FindAll("td input")[1].ChangeAsync(52d);
            await dataGrid.FindAll("td input")[2].ChangeAsync(true);
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
        public async Task DataGridDialogEdit()
        {
            var comp = Context.Render<DataGridFormEditTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormEditTest.Model>>();

            //verify values before opening dialog
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("Johanna");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("23");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("Steve");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("32");
            dataGrid.FindAll("td")[8].Html().Trim().Should().Be("snakex64");

            //open edit dialog
            await dataGrid.FindAll("tbody tr")[1].ClickAsync();
            //No close button
            comp.FindAll("button[aria-label=\"Close dialog\"]").Should().BeEmpty();
            //edit data
            await comp.FindAll("div input")[0].ChangeAsync("Galadriel");
            await comp.FindAll("div input")[1].ChangeAsync(1);

            await comp.Find(".mud-dialog-actions .mud-button-filled-primary").ClickAsync();

            //verify values after saving dialog
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("Galadriel");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("Steve");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("32");
            dataGrid.FindAll("td")[8].Html().Trim().Should().Be("snakex64");

            //if no crash occurs, we know the datagrid is properly filtering out the GetOnly property when calling set
        }

        [Theory]
        [TestCase(12, true)]
        [TestCase(-12, false)]
        public async Task DataGridChangesBehaviorTest(int age, bool shouldClose)
        {
            var comp = Context.Render<DataGridEditFormActionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditFormActionTest.Model>>();

            //verify values before opening dialog
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("Johanna");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("23");

            //verify no dialog open
            comp.FindAll("div.mud-dialog").Count.Should().Be(0);

            //open edit dialog
            dataGrid.FindAll("tbody tr")[1].Click();

            //verify dialog open
            comp.Find("div.mud-dialog").Should().NotBeNull();

            //edit data
            comp.FindAll("div input")[0].Change("Galadriel");
            comp.FindAll("div input")[1].Change(age);

            comp.Find(".mud-dialog-actions .mud-button-filled-primary").Click();

            if (shouldClose)
            {
                await comp.WaitForAssertionAsync(() =>
                {
                    //verify dialog closed
                    comp.FindAll("div.mud-dialog").Count.Should().Be(0);

                    //verify values changed
                    dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
                    dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
                    dataGrid.FindAll("td")[2].Html().Trim().Should().Be("Galadriel");
                    dataGrid.FindAll("td")[3].Html().Trim().Should().Be($"{age}");
                });
            }
            else
            {
                await comp.WaitForAssertionAsync(() =>
                {
                    //verify dialog still open
                    comp.Find("div.mud-dialog").Should().NotBeNull();

                    //verify values not changed
                    dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
                    dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
                    dataGrid.FindAll("td")[2].Html().Trim().Should().Be("Johanna");
                    dataGrid.FindAll("td")[3].Html().Trim().Should().Be("23");
                });
            }
        }

        [Test(Description = "Checks if there is no NRE exception when nested property has a null value somewhere in the middle.")]
        public void DataGridNoNullExceptionWhenNestedPropertyNullValue()
        {
            var comp = Context.Render<DataGridNestedNullPropertyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridNestedNullPropertyTest.Model>>();
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("Class A");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be(string.Empty);

            var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
            IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
            alertTextFunc.Should().Throw<ComponentNotFoundException>();
        }

        [Test(Description = "Checks if clone strategy is working, if we used default one it would fail as STJ doesn't support abstract classes without additional configuration.")]
        public async Task DataGridDialogEditCloneStrategyTest1()
        {
            var comp = Context.Render<DataGridFormEditCloneStrategyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormEditCloneStrategyTest.Movement>>();

            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("James");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("Robert");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("first");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("David");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("2");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("second");

            //open edit dialog
            await dataGrid.FindAll("tbody tr")[1].ClickAsync();
            //No close button
            comp.FindAll("button[aria-label=\"Close dialog\"]").Should().BeEmpty();
            //edit data
            await comp.FindAll("div input")[0].ChangeAsync("Galadriel");
            await comp.FindAll("div input")[1].ChangeAsync("Steve");
            await comp.FindAll("div input")[2].ChangeAsync("3");

            await comp.Find(".mud-dialog-actions .mud-button-filled-primary").ClickAsync();

            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("James");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("Robert");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("first");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("Galadriel");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("Steve");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("3");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("second");
        }

        [Test]
        public async Task DataGridDialogEditCloneStrategyTest2()
        {
            var comp = Context.Render<DataGridFormEditCloneStrategyTest>(parameters => parameters
                .Add(p => p.CloneStrategy, SystemTextJsonDeepCloneStrategy<DataGridFormEditCloneStrategyTest.Movement>.Instance));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormEditCloneStrategyTest.Movement>>();

            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("James");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("Robert");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("first");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("David");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("2");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("second");

            //open edit dialog
            Func<Task> openDialog = () => dataGrid.FindAll("tbody tr")[1].ClickAsync();

            await openDialog.Should().ThrowAsync<NotSupportedException>("STJ doesn't support abstract classes without polymorphic type discriminators.");
        }

        /// <summary>
        /// DataGrid edit form should trigger the FormFieldChanged event
        /// </summary>
        [Test]
        public async Task DataGridFormFieldChanged()
        {
            var comp = Context.Render<DataGridFormFieldChangedTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormFieldChangedTest.Item>>();
            //open edit dialog
            await dataGrid.FindAll("tbody tr")[0].ClickAsync();

            //edit data
            await comp.Find("div input").ChangeAsync("J K Simmons");
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be("J K Simmons");

            var textfield = comp.FindComponent<MudTextField<string>>();
            textfield.Instance.Should().BeSameAs(comp.Instance.FormFieldChangedEventArgs.Field);
        }

        [Test]
        public async Task DataGridFormValidationErrorsPreventUpdate()
        {
            var comp = Context.Render<DataGridFormValidationErrorsPreventUpdateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormValidationErrorsPreventUpdateTest.Model>>();

            // open form dialog
            await dataGrid.Find("tbody tr button").ClickAsync();
            dataGrid.Instance._isEditFormOpen.Should().BeTrue();

            var field = comp.FindComponents<MudTextField<string>>()[2];

            // edit data
            field.Instance.Value.Should().Be("Augusta_Homenick26@mud.com");
            await (await field.WaitForElementAsync("input")).ChangeAsync("not-a-valid-email-address");

            // check the change occurred
            field.Instance.Value.Should().Be("not-a-valid-email-address");

            // ensure that validation message is displayed
            field.Markup.Should().Contain("This is not a valid e-mail address");

            var button = comp.FindComponents<MudButton>().Single(b => b.Markup.Contains("Save"));
            await (await button.WaitForElementAsync("button")).ClickAsync();

            // dialog should still be open and the items data should not have been updated
            using AssertionScope scope = new();
            dataGrid.Instance._isEditFormOpen.Should().BeTrue();
            comp.Instance.Items[0].Email.Should().Be("Augusta_Homenick26@mud.com");
        }

        [Test]
        public void DataGridVisualStyling()
        {
            var comp = Context.Render<DataGridVisualStylingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridVisualStylingTest.Item>>();

            dataGrid.FindAll("td")[1].GetAttribute("style").Should().Contain("background-color:#E5BDE5");
            dataGrid.FindAll("td")[2].GetAttribute("style").Should().Contain("font-weight:bold");

            dataGrid.FindAll("th")[0].GetAttribute("style").Should().Contain("background-color:#E5BDE5");
            dataGrid.FindAll("th")[0].GetAttribute("style").Should().Contain("font-weight:bold");

            dataGrid.FindAll("th")[0].GetAttribute("class").Should().Contain("class-a");
            dataGrid.FindAll("th")[0].GetAttribute("class").Should().Contain("class-b");
            dataGrid.FindAll("th")[0].GetAttribute("class").Should().Contain("class-c");
        }

        [Test]
        public async Task DataGridEventCallbacks()
        {
            var comp = Context.Render<DataGridEventCallbacksTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEventCallbacksTest.Item>>();

            // Include callbacks in test coverage.
            dataGrid.Instance.RowClick.HasDelegate.Should().Be(true);
            dataGrid.Instance.RowContextMenuClick.HasDelegate.Should().Be(true);
            dataGrid.Instance.SelectedItemChanged.HasDelegate.Should().Be(true);
            dataGrid.Instance.CommittedItemChanges.Should().NotBeNull();
            dataGrid.Instance.StartedEditingItem.HasDelegate.Should().Be(true);
            dataGrid.Instance.CanceledEditingItem.HasDelegate.Should().Be(true);
            dataGrid.Instance.CanceledEditingItem.Should().Be(dataGrid.Instance.CanceledEditingItem);

            // we test to make sure that we can set and get the cancelCallback via the CancelledEditingItem property
            var cancelCallback = dataGrid.Instance.CanceledEditingItem;
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(dg => dg.CanceledEditingItem, () => { }));
            dataGrid.Instance.CanceledEditingItem.Should().NotBe(cancelCallback);
#pragma warning disable BL0005
            dataGrid.Instance.CanceledEditingItem = cancelCallback;
#pragma warning restore BL0005
            dataGrid.Instance.CanceledEditingItem.Should().Be(cancelCallback);

            // Set some parameters manually so that they are covered.
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.MultiSelection, true)
                .Add(x => x.ReadOnly, false)
                .Add(x => x.EditMode, DataGridEditMode.Cell)
                .Add(x => x.EditTrigger, DataGridEditTrigger.OnRowClick));

            // Make sure that the callbacks have not been fired yet.
            comp.Instance.RowClicked.Should().Be(false);
            comp.Instance.RowContextMenuClicked.Should().Be(false);
            comp.Instance.SelectedItemChanged.Should().Be(false);
            comp.Instance.CommittedItemChanges.Should().Be(false);
            comp.Instance.StartedEditingItem.Should().Be(false);
            comp.Instance.CanceledEditingItem.Should().Be(false);

            // Fire RowClick, SelectedItemChanged, SelectedItemsChanged, and StartedEditingItem callbacks.
            await dataGrid.FindAll(".mud-table-body tr")[0].ClickAsync();

            // Fire RowContextMenuClick
            dataGrid.FindAll(".mud-table-body tr")[0].ContextMenu();

            // Edit an item.
            await dataGrid.FindAll(".mud-table-body tr td input")[0].ChangeAsync("A test");

            // Make sure that the callbacks have been fired.
            comp.Instance.RowClicked.Should().Be(true);
            comp.Instance.RowContextMenuClicked.Should().Be(true);
            comp.Instance.SelectedItemChanged.Should().Be(true);
            comp.Instance.CommittedItemChanges.Should().Be(true);
            comp.Instance.CanceledEditingItem.Should().Be(false);

            // TODO: Triggering of the CancelEditingItem callback appears to require the Form edit mode
            // but we can brute force it by directly calling the CancelEditingItemAsync method on the datagrid
            await dataGrid.InvokeAsync(dataGrid.Instance.CancelEditingItemAsync);
            comp.Instance.CanceledEditingItem.Should().Be(true);
        }

        [Test]
        public async Task DataGridEditComplexPropertyExpression()
        {
            var comp = Context.Render<DataGridEditComplexPropertyExpressionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditComplexPropertyExpressionTest.Item>>();

            dataGrid.Render();

            // Make sure that the value is as expected before we try to change it
            comp.Instance.Items[0].Name.Should().Be("A");
            comp.Instance.Items[0].SubItem.SubProperty.Should().Be("A-D");
            comp.Instance.Items[0].SubItem.SubItem2.SubProperty2.Should().Be("A-D-E");

            // Edit an item 'normally'
            await dataGrid.FindAll(".mud-table-body tr td input")[0].ChangeAsync("Test 1");
            comp.Instance.Items[0].Name.Should().Be("Test 1");

            // Edit an item that has a sub property like x.Something.SomethingElse
            await dataGrid.FindAll(".mud-table-body tr td input")[1].ChangeAsync("Test 2");
            comp.Instance.Items[0].SubItem.SubProperty.Should().Be("Test 2");

            // Edit an item that has a sub property like x.Something.SomethingElse.SomethingElseAgain
            await dataGrid.FindAll(".mud-table-body tr td input")[2].ChangeAsync("Test 3");
            comp.Instance.Items[0].SubItem.SubItem2.SubProperty2.Should().Be("Test 3");
        }

        [Test]
        public void DataGridOnContextMenuClickWhenIsGrouped()
        {
            var comp = Context.Render<DataGridGroupExpandedTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedTest.Fruit>>();

            // Include callbacks in test coverage.
            dataGrid.Instance.RowContextMenuClick.HasDelegate.Should().Be(true);

            // Make sure that the callbacks have not been fired yet.
            comp.Instance.RowContextMenuClicked.Should().Be(false);

            // Fire RowContextMenuClick
            dataGrid.FindAll(".mud-table-body tr")[1].ContextMenu();

            // Make sure that the callbacks have been fired.
            comp.Instance.RowContextMenuClicked.Should().Be(true);
        }

        [Test]
        public async Task DataGridServerSideSortable()
        {
            // Disable simulated load on server side:
            TestComponents.DataGrid.DataGridServerSideSortableTest.DisableServerTimeoutForTests = true;

            var comp = Context.Render<DataGridServerSideSortableTest>();
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

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortMode, SortMode.None));
            dataGrid.Instance.DropContainerHasChanged();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task FilterDefinitionString()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var nameColumn = dataGrid.Instance.GetColumnByPropertyName("Name");

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>();
            Func<DataGridFiltersTest.Model, bool> func = null;

            #region FilterOperator.String.Contains

            //default Case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();

            // null value default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = null,
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();

            // null value default case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = null
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.NotContains

            // default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotContains,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();

            // case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotContains,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotContains,
                Value = null
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.Equal

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = null,
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();

            // null value case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = null
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.NotEqual

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEqual,
                Value = null
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.StartsWith

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.StartsWith,
                Value = null
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.EndsWith

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe"
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.EndsWith,
                Value = null
            };
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.Empty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(string.Empty, 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEmpty,
                Value = null,
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(string.Empty, 45, null, null, null, null, null)).Should().BeFalse();

            #endregion

            #region FilterOperator.String.NotEmpty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(string.Empty, 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("", 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(string.Empty, 45, null, null, null, null, null)).Should().BeFalse();

            #endregion

            // handle null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = null,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionBool()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var hiredColumn = dataGrid.Instance.GetColumnByPropertyName("Hired");

            #region FilterOperator.Boolean.Is

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = hiredColumn,
                Operator = FilterOperator.Boolean.Is,
                Value = true
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, false, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, true, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = hiredColumn,
                Operator = FilterOperator.Boolean.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, false, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, true, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = hiredColumn,
                Operator = null,
                Value = true
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, false, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, true, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionEnum()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var statusColumn = dataGrid.Instance.GetColumnByPropertyName("Status");

            #region FilterOperator.Enum.Is

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.Is,
                Value = Severity.Normal
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Info, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, Severity.Normal, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Info, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, Severity.Normal, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Enum.IsNot

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.IsNot,
                Value = Severity.Normal
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Normal, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, Severity.Info, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Normal, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, Severity.Info, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = null,
                Value = Severity.Normal
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Normal, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, Severity.Info, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionDateTime()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var dateColumn = dataGrid.Instance.GetColumnByPropertyName("HiredOn");
            var utcnow = DateTime.UtcNow;

            #region FilterOperator.DateTime.Is

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Is,
                Value = utcnow
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.IsNot

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.IsNot,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.After

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.After,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.After,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.OnOrAfter

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.Before

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Before,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Before,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.OnOrBefore

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.Empty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Empty,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.NotEmpty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = null,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionDateOnly()
        {
            var comp = Context.Render<DataGridDateOnlyFilterTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridDateOnlyFilterTest.Model>>();
            var dateColumn = dataGrid.Instance.GetColumnByPropertyName("HiredOn");
            var testDate = new DateOnly(2020, 3, 10);

            #region FilterOperator.DateOnly.Is

            var filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.Is,
                Value = testDate
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();
            func.Invoke(new("Ira", 27, new DateOnly(2011, 1, 2))).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.IsNot

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.IsNot,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeFalse();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();
            func.Invoke(new("Ira", 27, new DateOnly(2011, 1, 2))).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.After

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.After,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeFalse();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();
            func.Invoke(new("John", 32, new DateOnly(2022, 6, 15))).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.After,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.OnOrAfter

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.OnOrAfter,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();
            func.Invoke(new("John", 32, new DateOnly(2022, 6, 15))).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.OnOrAfter,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.Before

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.Before,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeFalse();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();
            func.Invoke(new("Ira", 27, new DateOnly(2011, 1, 2))).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.Before,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.OnOrBefore

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.OnOrBefore,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();
            func.Invoke(new("Ira", 27, new DateOnly(2011, 1, 2))).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.OnOrBefore,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.Empty

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.Empty,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeFalse();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();
            func.Invoke(new("Ira", 27, new DateOnly(2011, 1, 2))).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeFalse();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateOnly.NotEmpty

            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.NotEmpty,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();
            func.Invoke(new("Ira", 27, new DateOnly(2011, 1, 2))).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateOnly.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeFalse();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridDateOnlyFilterTest.Model>
            {
                Column = dateColumn,
                Operator = null,
                Value = testDate
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 56, testDate)).Should().BeTrue();
            func.Invoke(new("Joe", 32, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionNumber()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var ageColumn = dataGrid.Instance.GetColumnByPropertyName("Age");

            #region FilterOperator.Number.Equal

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.Equal,
                Value = 45
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.Equal,
                Value = null
            };
            var func2 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            // data type is an int
            func2.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeTrue();
            func2.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeTrue();
            func2.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.NotEqual

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.NotEqual,
                Value = 45
            };
            var func3 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func3.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeTrue();
            func3.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeTrue();
            func3.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.NotEqual,
                Value = null
            };
            var func4 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func4.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeTrue();
            func4.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeTrue();
            func4.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.GreaterThan

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThan,
                Value = 45
            };
            var func5 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func5.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeTrue();
            func5.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func5.Invoke(new("Joe", null, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThan,
                Value = null
            };
            var func6 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func6.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeTrue();
            func6.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func6.Invoke(new("Joe", null, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.LessThan

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThan,
                Value = 45
            };
            var func7 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func7.Invoke(new("Sam", 4, null, null, null, null, null)).Should().BeTrue();
            func7.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeFalse();
            func7.Invoke(new("Joe", null, null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThan,
                Value = null
            };
            var func8 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func8.Invoke(new("Sam", 4, null, null, null, null, null)).Should().BeTrue();
            func8.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
            func8.Invoke(new("Joe", null, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.GreaterThanOrEqual

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = 45
            };
            var func9 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func9.Invoke(new("Sam", 4, null, null, null, null, null)).Should().BeFalse();
            func9.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeFalse();
            func9.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = null
            };
            var func10 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func10.Invoke(new("Sam", 4, null, null, null, null, null)).Should().BeTrue();
            func10.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeTrue();
            func10.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.LessThanOrEqual

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = 45
            };
            var func11 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func11.Invoke(new("Sam", 46, null, null, null, null, null)).Should().BeFalse();
            func11.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeFalse();
            func11.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = null
            };
            var func12 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func12.Invoke(new("Sam", 46, null, null, null, null, null)).Should().BeTrue();
            func12.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeTrue();
            func12.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = null,
                Value = 45
            };
            var func13 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func13.Invoke(new("Sam", 456, null, null, null, null, null)).Should().BeTrue();
            func13.Invoke(new("Sam", null, null, null, null, null, null)).Should().BeTrue();
            func13.Invoke(new("Joe", 45, null, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public async Task FilterDefinitionReplaceWithCustom()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            dataGrid.Instance.SetDefaultFilterDefinition<CustomFilterDefinitionMock<DataGridFiltersTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            // add a filter via the AddFilter method
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());

            // check the number of filters displayed in the filters panel
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            var filterDefinitionInstance = dataGrid.Instance.FilterDefinitions.FirstOrDefault();
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            filterDefinitionInstance.Should().NotBeNull();
            filterDefinitionInstance.Should().BeOfType<CustomFilterDefinitionMock<DataGridFiltersTest.Model>>();
        }

        [Test]
        public async Task DataGridClickFilterButton()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            IElement FilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            await FilterButton().ClickAsync();

            // check the number of filters displayed in the filters panel is 1
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            // click again on the filter button
            await FilterButton().ClickAsync();

            // check the number of filters displayed in the filters panel is still 1 (no duplicate filter)
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCloseFilters()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            IElement FilterButton() => dataGrid.FindAll(".filter-button")[0];

            // Helper method to select a filter operator and verify the outcome
            async Task SelectFilterOperator(int operatorIndex, int expectedFilterCount)
            {
                // Ensure the filter panel is open before interacting
                if (comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count == 0)
                {
                    await FilterButton().ClickAsync();
                    await comp.WaitForElementAsync(".filter-operator");
                }

                // Open the operator dropdown and select an item
                await comp.Find(".filter-operator").MouseDownAsync(new MouseEventArgs());
                var listItems = await comp.WaitForElementsAsync(".mud-list .mud-list-item");
                await listItems[operatorIndex].ClickAsync();

                // Click the overlay to close the dropdown and commit the selection
                await comp.Find(".mud-overlay").ClickAsync();

                // Assert that the number of active filters is correct
                await comp.WaitForAssertionAsync(() =>
                {
                    dataGrid.Instance.FilterDefinitions.Count.Should().Be(expectedFilterCount);
                });

                // Close the filter panel to ensure a clean state for the next test
                if (comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count > 0)
                {
                    await FilterButton().ClickAsync();
                }
            }

            // 1. Initial state: Open the filter panel and confirm it's visible
            await FilterButton().ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1));

            // 2. Test operators that should be removed when their value is empty
            await SelectFilterOperator(0, 0); // "contains"
            await SelectFilterOperator(1, 0); // "not contains"
            await SelectFilterOperator(2, 0); // "equals"
            await SelectFilterOperator(3, 0); // "not equals"
            await SelectFilterOperator(4, 0); // "starts with"
            await SelectFilterOperator(5, 0); // "ends with"

            // 3. Test operators that are valid without a value
            await SelectFilterOperator(6, 1); // "is empty"
            await SelectFilterOperator(7, 1); // "is not empty"
        }

        [Test]
        public async Task DataGridFilters()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            // test filter definition on the Name property (string contains)
            var stringFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Name"),
                Operator = "contains",
                Value = "John"
            };

            // test filter definition on the Age property (int >)
            var intFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Age"),
                Operator = ">",
                Value = 30
            };

            // test filter definition on the Status property (Enum is)
            var enumFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Status"),
                Operator = "is",
                Value = Severity.Normal
            };

            // test filter definition on the Hired property (Bool is)
            var boolFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Hired"),
                Operator = "is",
                Value = true
            };

            // test filter definition on the Hired property (Bool null)
            var boolFilterDefinitionWithNull = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Hired"),
                Value = null
            };

            // test filter definition on the HiredOn property (DateTime is)
            var dateTimeFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("HiredOn"),
                Operator = "is",
                Value = DateTime.UtcNow.Date
            };

            // test filter definition on the HiredOn property (DateTime null)
            var dateTimeFilterDefinitionWithNull = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("HiredOn"),
                Value = null
            };

            // test filter definition on the StartDate property (DateOnly is)
            var dateOnlyFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("StartDate"),
                Operator = "is",
                Value = new DateOnly(2020, 3, 10)
            };

            // test filter definition on the StartDate property (DateOnly null)
            var dateOnlyFilterDefinitionWithNull = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("StartDate"),
                Value = null
            };

            // test filter definition on the ApplicationId property (Guid equals)
            var guidFilterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("ApplicationId"),
                Operator = "equals",
                Value = Guid.NewGuid()
            };

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(stringFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(intFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(enumFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(boolFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(dateTimeFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(dateOnlyFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(guidFilterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            // check the number of filters displayed in the filters panel
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(7);

            // click the Add Filter button in the filters panel to add a filter
            await comp.FindAll(".filters-panel > button")[0].ClickAsync();

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(8);

            // add a filter via the AddFilter method
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(9);

            // add a filter via the AddFilter method
            //await comp.InvokeAsync(() => dataGrid.Instance.AddFilter(Guid.NewGuid(), "Status"));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Status")
            }));

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(10);

            // toggle the filters menu (should be closed after this)
            await comp.InvokeAsync(() => dataGrid.Instance.ToggleFiltersMenu());
            comp.FindAll(".filters-panel").Count.Should().Be(0);

            // test internal filter class for string data type.
            var internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, stringFilterDefinition, null);
            stringFilterDefinition.Column.dataType.Should().Be(typeof(string));
            await comp.InvokeAsync(() => internalFilter.StringValueChanged("J"));
            stringFilterDefinition.Value.Should().Be("J");

            // test internal filter class for number data type.
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, intFilterDefinition, null);
            intFilterDefinition.Column.dataType.Should().Be(typeof(int?));
            await comp.InvokeAsync(() => internalFilter.NumberValueChanged(35));
            intFilterDefinition.Value.Should().Be(35);

            // test internal filter class for enum data type.
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, enumFilterDefinition, null);
            enumFilterDefinition.Column.dataType.Should().Be(typeof(Severity?));
            await comp.InvokeAsync(() => internalFilter.EnumValueChanged(Severity.Warning));
            enumFilterDefinition.Value.Should().Be(Severity.Warning);
            enumFilterDefinition.FieldType.IsEnum.Should().Be(true);

            // test internal filter class for bool data type.
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, boolFilterDefinition, null);
            boolFilterDefinition.Column.dataType.Should().Be(typeof(bool?));
            await comp.InvokeAsync(() => internalFilter.BoolValueChanged(false));
            boolFilterDefinition.Value.Should().Be(false);

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(boolFilterDefinitionWithNull)); // Test adding Bool filter with null value
            boolFilterDefinitionWithNull.Value.Should().BeNull();

            // test internal filter class for datetime data type
            var date = DateTime.UtcNow;
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, dateTimeFilterDefinition, null);
            dateTimeFilterDefinition.Column.dataType.Should().Be(typeof(DateTime?));

            await comp.InvokeAsync(() => internalFilter.DateValueChanged(date));
            dateTimeFilterDefinition.Value.Should().Be(date.Date);

            await comp.InvokeAsync(() => internalFilter.TimeValueChanged(date.TimeOfDay));
            dateTimeFilterDefinition.Value.Should().Be(date);

            await comp.InvokeAsync(() => internalFilter.TimeValueChanged(null));
            dateTimeFilterDefinition.Value.Should().Be(date.Date);

            await comp.InvokeAsync(() => internalFilter.DateValueChanged(null));
            dateTimeFilterDefinition.Value.Should().BeNull();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(dateTimeFilterDefinitionWithNull)); // Test adding DateTime filter with null value
            dateTimeFilterDefinitionWithNull.Value.Should().BeNull();

            // test internal filter class for dateonly data type.
            var dateOnlyDateTimeInput = DateTime.UtcNow;
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, dateOnlyFilterDefinition, null);
            dateOnlyFilterDefinition.Column.dataType.Should().Be(typeof(DateOnly?));

            await comp.InvokeAsync(() => internalFilter.DateOnlyValueChanged(dateOnlyDateTimeInput));
            dateOnlyFilterDefinition.Value.Should().Be(DateOnly.FromDateTime(dateOnlyDateTimeInput));

            await comp.InvokeAsync(() => internalFilter.DateOnlyValueChanged(null));
            dateOnlyFilterDefinition.Value.Should().BeNull();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(dateOnlyFilterDefinitionWithNull)); // Test Adding DateOnly filter with null value
            dateOnlyFilterDefinitionWithNull.Value.Should().BeNull();

            // test internal filter class for guid data type.
            var guid = Guid.NewGuid();
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, guidFilterDefinition, null);
            guidFilterDefinition.Column.dataType.Should().Be(typeof(Guid?));
            await comp.InvokeAsync(() => internalFilter.GuidValueChanged(guid));
            guidFilterDefinition.Value.Should().Be(guid);
        }

        [Test]
        public async Task DataGridFilterRemoveAsync()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Name"),
                Operator = "contains",
                Value = "Test"
            };

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition));
            await comp.WaitForAssertionAsync(() => dataGrid.Instance.FilterDefinitions.Should().Contain(filterDefinition));

            var filter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition, null);
            await comp.InvokeAsync(() => filter.RemoveFilterAsync());

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.FilterDefinitions.Should().NotContain(filterDefinition));
        }

        [Test]
        public void DataGridFilterFieldChanged()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Name")
            };

            var filter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition, null);
            var newBoolColumn = dataGrid.Instance.GetColumnByPropertyName("Hired");

            filter.FieldChanged(newBoolColumn!);

            filterDefinition.Column.Should().Be(newBoolColumn);
            filterDefinition.Operator.Should().Be(FilterOperator.Boolean.Is);
            filterDefinition.Title.Should().Be(newBoolColumn.Title);
            filterDefinition.Value.Should().BeNull();
        }

        [Test]
        public async Task DataGridFilterPerColumn()
        {
            var comp = Context.Render<DataGridFilterPerColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterPerColumnTest.Model>>();

            IElement FirstnameFilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            await FirstnameFilterButton().ClickAsync();

            // check the number of filters displayed in the filters panel is 1
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            // get select menus
            var selects = comp.FindAll(".filters-panel .mud-grid-item .mud-input-control.mud-select");
            selects.Count.Should().Be(2);

            // open operator menu
            await selects[1].MouseDownAsync(new MouseEventArgs());

            //check available operators
            var items = comp.FindAll("div.mud-list-item");

            items.Count.Should().Be(4);
            items.ToMarkup()
                 .Should().Contain("starts with")
                 .And.Contain("ends with")
                 .And.Contain("equals")
                 .And.Contain("contains");
        }

        [Test]
        public void DataGridInvalidFilterPerColumn()
        {
            var exception = Assert.Throws<ArgumentException>(() => Context.Render<DataGridFilterPerColumnTest>(parameters => parameters.Add(x => x.AddInvalid, true)));

            exception.Message.Should().Be("Invalid filter operators for Severity: <");
        }

        [Test]
        public async Task DataGridIDictionaryFilters()
        {
            var comp = Context.Render<DataGridIDictionaryFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<IDictionary<string, object>>>();

            // test filter definition on the Name property (string contains)
            var filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                //Field = "Name",
                Operator = "contains",
                Value = "John"
            };
            // test filter definition on the Age property (int >)
            var filterDefinition2 = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                //Field = "Age",
                Operator = ">",
                Value = 30
            };
            // test filter definition on the Status property (Enum is)
            var filterDefinition3 = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                //Field = "Status",
                Operator = "is",
                Value = Severity.Normal
            };

            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition2));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition3));
            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            var filters = dataGrid.Instance.FilterDefinitions;

            // assertions for string
            filters[0].Id.Should().Be(filterDefinition.Id);
            filters[0].Operator.Should().Be(filterDefinition.Operator);
            filters[0].Value.Should().Be(filterDefinition.Value);
            filters[0].Value = "Not Joe";
            filterDefinition.Value.Should().Be("Not Joe");

            // assertions for int
            filters[1].Id.Should().Be(filterDefinition2.Id);
            filters[1].Operator.Should().Be(filterDefinition2.Operator);
            filters[1].Value.Should().Be(filterDefinition2.Value);
            filters[1].Value = 45;
            filterDefinition2.Value.Should().Be(45);

            // assertions for Enum
            filters[2].Id.Should().Be(filterDefinition3.Id);
            filters[2].Operator.Should().Be(filterDefinition3.Operator);
            filters[2].Value.Should().Be(filterDefinition3.Value);
            filters[2].Value = Severity.Error;
            filterDefinition3.Value.Should().Be(Severity.Error);
        }

        [Test]
        public void DataGridColGroup()
        {
            var comp = Context.Render<DataGridColGroupTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColGroupTest.Model>>();

            dataGrid.FindAll("col").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridColReorderRowFilters()
        {
            var comp = Context.Render<DataGridColReorderRowFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColReorderRowFiltersTest.Model>>();

            await comp.InvokeAsync(async () =>
            {
                // Should have 4 entries, 2 headers and an extra
                dataGrid.FindAll("tr").Count.Should().Be(7);

                var switchButton = dataGrid.Find("button.switch-button");
                await switchButton.ClickAsync();

                var filterHeaders = () => dataGrid.FindAll("input");
                var ageFilter = () => filterHeaders()[0];
                var nameFilter = () => filterHeaders()[1];

                await ageFilter().InputAsync(27);
                // Should have 1 entry + 3
                dataGrid.FindAll("tr").Count.Should().Be(4);

                await dataGrid.Instance.ClearFiltersAsync();
                await nameFilter().InputAsync("a");
                // Should have 3 entries + 3
                dataGrid.FindAll("tr").Count.Should().Be(6);
            });
        }

        [Test]
        public async Task DataGridColReorderRowModifiedFilters()
        {
            var comp = Context.Render<DataGridColReorderRowFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColReorderRowFiltersTest.Model>>();

            await comp.InvokeAsync(async () =>
            {
                // Should have 4 entries, 2 headers and an extra
                dataGrid.FindAll("tr").Count.Should().Be(7);

                var ageCol = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Age");
                var modifiedAgeFilter = ageCol.FilterContext.FilterDefinition;
                modifiedAgeFilter.Operator = ">";

                var nameCol = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Name");
                var modifiedNameFilter = nameCol.FilterContext.FilterDefinition;
                modifiedNameFilter.Operator = "not contains";

                await dataGrid.Instance.AddFilterAsync(modifiedAgeFilter);
                await dataGrid.Instance.AddFilterAsync(modifiedNameFilter);

                var switchButton = dataGrid.Find("button.switch-button");
                await switchButton.ClickAsync();

                var filterHeaders = () => dataGrid.FindAll("input");
                var ageFilter = () => filterHeaders()[0];
                var nameFilter = () => filterHeaders()[1];

                await ageFilter().InputAsync(27);
                // Should have 3 entries + 3
                dataGrid.FindAll("tr").Count.Should().Be(6);

                await nameFilter().InputAsync("a");
                // Should have 1 entry + 3
                dataGrid.FindAll("tr").Count.Should().Be(4);
            });
        }

        [Test]
        public void DataGridHeaderTemplate()
        {
            var comp = Context.Render<DataGridHeaderTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHeaderTemplateTest.Model>>();

            dataGrid.Find("thead th").TextContent.Trim().Should().Be("test");

            dataGrid.Find("span.column-header").FirstChild.NodeName.Should().Be("svg");
            dataGrid.Find("span.column-header span").TextContent.Should().Be("Name");
        }

        [Test]
        public async Task DataGridRowDetailOpen()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance
            .ToggleHierarchyVisibilityAsync(dataGrid.Instance.Items.First()));

            dataGrid.FindAll("td")[5].TextContent.Trim().Should().StartWith("uid = Sam|56|Normal|");
        }

        [Test]
        public void DataGridRowDetailClosed()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            dataGrid.FindAll("td").SingleOrDefault(x => x.TextContent.Trim().StartsWith("uid = Sam|56|Normal|")).Should().BeNull();
        }

        [Test]
        public async Task DataGrid_RowDetail_ExpandCollapseAll()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            await dataGrid.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(2));
            await dataGrid.InvokeAsync(() => dataGrid.Instance.CollapseAllHierarchy());
            await dataGrid.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(0));
            await dataGrid.InvokeAsync(() => dataGrid.Instance.ExpandAllHierarchy());
            // one is disabled and will not be expanded
            await dataGrid.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(4));
        }

        [Test]
        public async Task DataGrid_RowDetail_ExpandCollapseAllWithOne()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>(p => p
                .Add(x => x.LimitRowsToOne, true)
                .Add(x => x.EnableHeaderToggle, true)
            );
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            await dataGrid.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(0));
            var headerToggle = dataGrid.Find("th button.mud-hierarchy-toggle-button");
            await headerToggle.ClickAsync();
            await dataGrid.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(1));
            await headerToggle.ClickAsync();
            await dataGrid.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(0));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DataGrid_RowDetail_RTL_GroupIcon(bool rightToLeft)
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>(param => param
                .Add(p => p.RightToLeft, rightToLeft)
            );
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();
            var svg = dataGrid.Find(".mud-table-body .mud-table-row .mud-table-cell .mud-icon-root");

            if (!rightToLeft)
            {
                // ChevronRight by Default
                svg.InnerHtml.Should().Contain("<path d=\"M0 0h24v24H0z\"")
                    .And.Contain("<path d=\"M10 6L8.59 7.41");
            }
            else
            {
                // ChevronLeft when RTL is true
                svg.InnerHtml.Should().Contain("<path d=\"M0 0h24v24H0z\"")
                    .And.Contain("<path d=\"M15.41 7.41L14 6l-6");
            }
        }

        [Test]
        public void DataGridRowDetailButtonDisabled()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            dataGrid.FindAll("button")[10].OuterHtml.Contains("disabled")
                .Should().BeTrue();
        }

        [Test]
        public async Task DataGridRowDetailButtonDisabledClick()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            await comp.InvokeAsync(async () =>
            {
                var buttons = dataGrid.FindAll("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon");
                await buttons[10].ClickAsync();

                dataGrid.FindAll("td")
                .SingleOrDefault(x => x.TextContent.Trim().StartsWith("uid = Alicia|54|Info|")).Should().BeNull();
            });
        }

        [Test]
        public async Task DataGrid_HierarchyColumn_ShowAriaLabel()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var button = comp.Find("tbody tr button.mud-icon-button");

            button.GetAttribute("aria-label").Should().Be("Expand group");

            await button.ClickAsync(new MouseEventArgs());

            button.GetAttribute("aria-label").Should().Be("Collapse group");
        }

        [Test]
        public void DataGridChildRowContent()
        {
            var comp = Context.Render<DataGridChildRowContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridChildRowContentTest.Model>>();

            dataGrid.FindAll("td").SingleOrDefault(x => x.TextContent.Trim().StartsWith("uid = Sam|56|Normal|")).Should().NotBeNull();
        }

        [Test]
        public void DataGridLoadingContent()
        {
            var comp = Context.Render<DataGridLoadingContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridLoadingContentTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("Data loading, please wait...");
        }

        /// <summary>
        /// Verifies that enabling the loading switch adds a new row to the table header without altering the table body.
        /// </summary>
        [Test]
        public async Task DataGridLoadingProgress()
        {
            // Render the component
            var comp = Context.Render<DataGridLoadingProgressTest>();

            // Initial count of header and body rows
            var initialHeaderRows = comp.FindAll("thead tr");
            var initialBodyRows = comp.FindAll("tbody tr");

            // Verify initial state: 1 row in the header and 3 rows in the body
            initialHeaderRows.Count.Should().Be(1);
            initialBodyRows.Count.Should().Be(3);

            // Toggle the loading switch to the 'loading' state
            var loadingSwitch = comp.Find("#loadingSwitch");
            await loadingSwitch.ChangeAsync(true);

            // Count rows after toggling the switch
            var updatedHeaderRows = comp.FindAll("thead tr");
            var updatedBodyRows = comp.FindAll("tbody tr");

            // Verify updated state:
            // 2 rows in the header (original + loading row) and 3 rows in the body (unchanged)
            updatedHeaderRows.Count.Should().Be(2);
            updatedBodyRows.Count.Should().Be(3);
        }

        [Test]
        public void DataGridNoRecordsContent()
        {
            var comp = Context.Render<DataGridNoRecordsContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridNoRecordsContentTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("There are no records to view.");
        }

        [Test]
        public void DataGridNoRecordsContentVirtualize()
        {
            var comp = Context.Render<DataGridNoRecordsContentVirtualizeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridNoRecordsContentVirtualizeTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("There are no records to view.");
        }

        [Test]
        public void DataGridFooterTemplate()
        {
            var comp = Context.Render<DataGridFooterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFooterTemplateTest.Model>>();

            dataGrid.FindAll("tfoot td").First().TextContent.Trim().Should().Be("Names: Sam, Alicia, Ira, John");
            dataGrid.FindAll("tfoot td").Last().TextContent.Trim().Should().Be($"Highest: {132000:C0} | 2 Over {100000:C0}");
        }

        [Test]
        public async Task DataGridServerPagination()
        {
            var comp = Context.Render<DataGridServerPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerPaginationTest.Model>>();

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("Name 0");

            // click to go to the next page
            await dataGrid.FindAll(".mud-table-pagination-actions button")[2].ClickAsync();

            // test that we are on the second page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("Name 10");

            // test reloading server side results programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.ReloadServerData());
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/8298
        /// </summary>
        [Test]
        public async Task SetRowsPerPageAsync_CallOneTimeServerData()
        {
            // Arrange

            var comp = Context.Render<DataGridServerPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerPaginationTest.Model>>();
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CurrentPage, 2));
            var serverDataCallCount = 0;
            var originalServerDataFunc = dataGrid.Instance.ServerData;
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ServerData, (state, token) =>
            {
                serverDataCallCount++;
                return originalServerDataFunc(state, token);
            }));

            // Act

            await dataGrid.InvokeAsync(() => dataGrid.Instance.SetRowsPerPageAsync(25));

            // Assert

            serverDataCallCount.Should().Be(1);
        }

        [Test]
        public void DataGridCellTemplate()
        {
            var comp = Context.Render<DataGridCellTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellTemplateTest.Model>>();

            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("45");
        }

        [Test]
        public async Task DataGridColumnChooser()
        {
            var comp = Context.Render<DataGridColumnChooserTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnChooserTest.Model>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(6);
            await comp.InvokeAsync(async () =>
            {
                var columnHamburger = dataGrid.FindAll("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
                await columnHamburger[2].ClickAsync();
                var listItems = popoverProvider.FindComponents<MudMenuItem>();
                listItems.Count.Should().Be(2);
                var clickablePopover = listItems[1].Find(".mud-menu-item");
                await clickablePopover.ClickAsync();
                ((IMudStateHasChanged)dataGrid.Instance).StateHasChanged();
            });

            await dataGrid.WaitForAssertionAsync(() =>
            {
                dataGrid.FindAll(".mud-table-head th").Count.Should().Be(5);
            });

            await comp.InvokeAsync(async () =>
            {
                var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
                await columnsButton.ClickAsync();
                var popover = dataGrid.FindComponent<MudPopover>();
                popover.Instance.Open.Should().BeTrue("Should be open once clicked");
                var listItems = popoverProvider.FindComponents<MudMenuItem>();
                listItems.Count.Should().Be(1);
                var clickablePopover = listItems[0].Find(".mud-menu-item");
                await clickablePopover.ClickAsync();
            });

            // Wait for switches, icons and buttons to appear
            await comp.WaitForAssertionAsync(async () =>
            {
                var switches = comp.FindComponents<MudSwitch<bool>>();
                switches.Count.Should().Be(6);
                var iconbuttons = comp.FindComponents<MudIconButton>();
                iconbuttons.Count.Should().Be(29);
                var buttons = comp.FindComponents<MudButton>();
                buttons.Count.Should().BeGreaterThan(1);
                await buttons[1].Find("button").ClickAsync();
            });

            await comp.WaitForAssertionAsync(() =>
            {
                comp.FindAll(".mud-table-head th").Count.Should().Be(7);
            });

            await comp.InvokeAsync(() => dataGrid.Instance.ShowColumnsPanel());
            comp.FindAll(".mud-data-grid-columns-panel").Count.Should().Be(1);

            await comp.InvokeAsync(() => dataGrid.Instance.HideColumnsPanel());
            comp.FindAll(".mud-data-grid-columns-panel").Count.Should().Be(0);

            await comp.InvokeAsync(() => dataGrid.Instance.HideAllColumnsAsync());
            await dataGrid.WaitForAssertionAsync(() =>
            {
                dataGrid.FindAll(".mud-table-head th").Count.Should().Be(3);
            });

            await comp.InvokeAsync(() => dataGrid.Instance.ShowAllColumnsAsync());
            await dataGrid.WaitForAssertionAsync(() =>
            {
                dataGrid.FindAll(".mud-table-head th").Count.Should().Be(6);
            });
        }

        [Test]
        public async Task DataGridColumnHidden()
        {
            var comp = Context.Render<DataGridColumnHiddenTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnHiddenTest.Model>>();

            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            comp.Markup.Should().NotContain("mud-popover-open");
            var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
            await columnsButton.ClickAsync();

            comp.Markup.Should().Contain("mud-popover-open");
            var listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(1);
            var clickablePopover = listItems[0].Find(".mud-menu-item");
            await clickablePopover.ClickAsync();

            // at this point, the column picker should be open
            var switches = comp.FindComponents<MudSwitch<bool>>();
            switches.Count.Should().Be(6);

            switches[0].Instance.ReadValue.Should().BeFalse();
            switches[1].Instance.ReadValue.Should().BeTrue();
            switches[2].Instance.ReadValue.Should().BeFalse();
            switches[3].Instance.ReadValue.Should().BeFalse();
            switches[4].Instance.ReadValue.Should().BeFalse();
            switches[0].Instance.ReadValue.Should().BeFalse();

            var buttons = comp.FindComponents<MudButton>();

            // this is the hide all button
            await buttons[0].Find("button").ClickAsync();
            //all hideable columns should be hidden;
            switches[0].Instance.ReadValue.Should().BeTrue();
            switches[1].Instance.ReadValue.Should().BeTrue();
            switches[2].Instance.ReadValue.Should().BeTrue();
            switches[3].Instance.ReadValue.Should().BeFalse();
            switches[4].Instance.ReadValue.Should().BeFalse();
            switches[5].Instance.ReadValue.Should().BeFalse();

            // 6 columns, 3 hidden (+ already collapsed)
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(4);

            // this is the show all button
            await buttons[1].Find("button").ClickAsync();
            switches[0].Instance.ReadValue.Should().BeFalse();
            switches[1].Instance.ReadValue.Should().BeFalse();
            switches[2].Instance.ReadValue.Should().BeFalse();
            switches[3].Instance.ReadValue.Should().BeFalse();
            switches[4].Instance.ReadValue.Should().BeFalse();
            switches[5].Instance.ReadValue.Should().BeFalse();

            // 6 columns, 0 hidden (1 permanently collapsed)
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(7);

            //programmatically changing the hidden which overrides hideable
            await dataGrid.InvokeAsync(async () =>
            {
                foreach (var column in dataGrid.Instance.RenderedColumns)
                {
                    await column.HiddenState.SetValueAsync(true);
                }
            });

            // cannot render the component again there can be only one mudpopoverprovider

            // 6 columns, 6 hidden (1 permanently collapsed)
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(1);

            //programmatically changing the hidden which overrides hideable
            await dataGrid.InvokeAsync(async () =>
            {
                foreach (var column in dataGrid.Instance.RenderedColumns)
                {
                    await column.HiddenState.SetValueAsync(false);
                }
            });

            // 6 columns, 0 hidden (1 permanently hidden)
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(7);
        }

        // This is not easily convertible to the new property expression.
        //[Test]
        //public async Task DataGridFilterRowHidden()
        //{
        //    var comp = Context.Render<DataGridFilterRowHiddenTest>();
        //    var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterRowHiddenTest.Model>>();

        //    //there should be only one filter cell visible
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(1);

        //    var popoverProvider = comp.FindComponent<MudPopoverProvider>();
        //    var popover = dataGrid.FindComponent<MudPopover>();
        //    popover.Instance.Open.Should().BeFalse("Should start as closed");

        //    var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
        //    await columnsButton.ClickAsync();

        //    popover.Instance.Open.Should().BeTrue("Should be open once clicked");
        //    var listItems = popoverProvider.FindComponents<MudListItem>();
        //    listItems.Count.Should().Be(1);
        //    var clickablePopover = listItems[0].Find(".mud-menu-item");
        //    await clickablePopover.ClickAsync();

        //    // at this point, the column picker should be open
        //    var switches = dataGrid.FindComponents<MudSwitch<bool>>();
        //    switches.Count.Should().Be(2);

        //    switches[0].Instance.Checked.Should().BeFalse();
        //    switches[1].Instance.Checked.Should().BeTrue();

        //    var buttons = dataGrid.FindComponents<MudButton>();
        //    // this is the hide all button
        //    await buttons[0].Find("button").ClickAsync();
        //    switches[0].Instance.Checked.Should().BeTrue();
        //    switches[1].Instance.Checked.Should().BeTrue();
        //    // 2 columns, 2 hidden
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(0);

        //    // this is the show all button
        //    await buttons[1].Find("button").ClickAsync();
        //    switches[0].Instance.Checked.Should().BeFalse();
        //    switches[1].Instance.Checked.Should().BeFalse();
        //    // 2 columns, 0 hidden
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(2);
        //    dataGrid.Instance.RenderedColumns[0].Filterable = false;
        //    await comp.InvokeAsync(dataGrid.Instance.ExternalStateHasChanged);
        //    //If the column is visible and Filterable is false there still shouldďbe the cell
        //    //without the input
        //    dataGrid.FindAll(".mud-table-cell.filter-header-cell").Count.Should().Be(2);
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(1);
        //}

        [Test]
        public async Task DataGridShowMenuIcon()
        {
            var comp = Context.Render<DataGridShowMenuIconTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridShowMenuIconTest.Item>>();
            dataGrid.FindAll(".mud-table-toolbar .mud-menu").Should().BeEmpty();
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ShowMenuIcon, true));
            dataGrid.FindAll(".mud-table-toolbar .mud-menu").Should().NotBeEmpty();
        }

        [Test]
        public async Task DataGridColumnPopupFiltering()
        {
            var comp = Context.Render<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            await comp.Find(".filter-button").ClickAsync();
            var input = comp.FindComponent<MudTextField<string>>();
            await input.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Value, "test"));
            await comp.Find(".apply-filter-button").ClickAsync();

            await comp.InvokeAsync(() =>
            {
                return dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridColumnPopupFilteringTest.Model>
                {
                    Column = dataGrid.Instance.RenderedColumns.First(),
                    Operator = FilterOperator.String.Contains,
                    Value = "test"
                });
            });

            dataGrid.Render();
            dataGrid.FindAll("tbody tr").Count.Should().Be(0);
        }

        [Test]
        public void DataGridColumnShowFilterIcons()
        {
            var comp = Context.Render<DataGridColumnShowFilterIconsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnShowFilterIconsTest.Model>>();

            // Should have 5 columns, but only two with filter icons
            dataGrid.FindComponents<Column<DataGridColumnShowFilterIconsTest.Model>>().Should().HaveCount(5);
            dataGrid.FindAll(".column-filter-menu").Should().HaveCount(2);
        }

        [Test]
        public async Task DataGridColumnPopupFilteringEmpty()
        {
            var comp = Context.Render<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            await comp.InvokeAsync(() => dataGrid.Instance.CleanupIncompleteFilters());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridColumnPopupFilteringIntentionalEmpty()
        {
            var comp = Context.Render<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Operator = FilterOperator.String.Empty;

            await comp.InvokeAsync(() => dataGrid.Instance.CleanupIncompleteFilters());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridColumnFilterIconShouldIgnoreEmptyValueRequiredFilters()
        {
            var comp = Context.Render<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();
            var nameColumn = dataGrid.Instance.RenderedColumns.First(x => x.PropertyName == nameof(DataGridColumnPopupFilteringTest.Model.Name));

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridColumnPopupFilteringTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = null
            }));

            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridColumnPopupFilteringTest.Model>>()
                .First(x => x.Instance.Column?.PropertyName == nameof(DataGridColumnPopupFilteringTest.Model.Name));

            headerCell.Instance.hasFilter.Should().BeFalse();
        }

        [Test]
        public async Task DataGridFilterableFalse()
        {
            var comp = Context.Render<DataGridFilterableFalseTest>();

            await comp.Find(".filter-button").ClickAsync();
            comp.FindAll(".filters-panel").Count.Should().Be(1);

            await comp.FindAll("div.mud-input-control")[0].MouseDownAsync(new MouseEventArgs());
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridColumnPopupCustomFiltering()
        {
            var comp = Context.Render<DataGridColumnPopupCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupCustomFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Instance.FilterHired = true;
            await comp.InvokeAsync(async () =>
            {
                var filterContext = dataGrid.Instance.RenderedColumns[3].FilterContext;
                await comp.Instance.ApplyFilterAsync(filterContext);
            });

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCustomFiltering()
        {
            var comp = Context.Render<DataGridCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            await comp.InvokeAsync(async () =>
            {
                await comp.Instance.FilterHiredToggled(true, dataGrid.Instance);
            });

            dataGrid.Render();
            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCustomPropertyFilterTemplate()
        {
            var comp = Context.Render<DataGridCustomPropertyFilterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomPropertyFilterTemplateTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            await comp.Find(".filter-button").ClickAsync();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Ira"));
            await comp.Find(".apply-filter-button").ClickAsync();

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);

            await comp.Find(".filter-button").ClickAsync();
            await comp.Find(".reset-filter-button").ClickAsync();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridCustomPropertyFilterTemplateApplyFilterTwice()
        {
            var comp = Context.Render<DataGridCustomPropertyFilterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomPropertyFilterTemplateTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            // Apply the filter the first time
            await comp.Find(".filter-button").ClickAsync();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Ira"));
            await comp.Find(".apply-filter-button").ClickAsync();

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            // Apply the filter a second time
            await comp.Find(".filter-button").ClickAsync();
            await comp.Find(".apply-filter-button").ClickAsync();

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridFilterTemplateRendersInSimpleMode()
        {
            var comp = Context.Render<DataGridFilterTemplateSimpleModeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterTemplateSimpleModeTest.Model>>();

            // Initially should show all 4 rows
            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            var departmentColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Department");

            // Add filter for Department column programmatically
            await comp.InvokeAsync(async () =>
            {
                await dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterTemplateSimpleModeTest.Model>
                {
                    Column = departmentColumn,
                    Operator = FilterOperator.String.Equal,
                    Value = "Engineering"
                });
            });

            // Should show only Engineering employees (Sam and Ira)
            dataGrid.FindAll("tbody tr").Count.Should().Be(2);

            // Open filter panel by clicking the filter icon in the header
            await comp.Find(".mud-button-root.filter-button").ClickAsync();

            // The filter panel should now be visible
            comp.FindAll(".filters-panel").Count.Should().Be(1);

            // The FilterTemplate should be rendered in Simple mode - verify the custom filter class is present
            var departmentFilterSelects = comp.FindAll(".filters-panel .department-filter");
            departmentFilterSelects.Count.Should().Be(1, "FilterTemplate should render the custom department filter with 'department-filter' class in Simple mode");
        }

        [Test]
        public async Task DataGridFilterTemplateFiltersDataInSimpleMode()
        {
            var comp = Context.Render<DataGridFilterTemplateSimpleModeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterTemplateSimpleModeTest.Model>>();

            // Initially should show all 4 rows
            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            var departmentColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Department");

            // Add filter for Marketing
            await comp.InvokeAsync(async () =>
            {
                await dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterTemplateSimpleModeTest.Model>
                {
                    Column = departmentColumn,
                    Operator = FilterOperator.String.Equal,
                    Value = "Marketing"
                });
            });

            // Should show only Marketing employee (Alicia)
            dataGrid.FindAll("tbody tr").Count.Should().Be(1);

            // Change filter value to Sales
            var filterDefinition = dataGrid.Instance.FilterDefinitions.First();
            await comp.InvokeAsync(() =>
            {
                filterDefinition.Value = "Sales";
            });
            dataGrid.Render();

            // Should show only Sales employee (John)
            dataGrid.FindAll("tbody tr").Count.Should().Be(1);

            // Change to Engineering
            await comp.InvokeAsync(() =>
            {
                filterDefinition.Value = "Engineering";
            });
            dataGrid.Render();

            // Should show Engineering employees (Sam and Ira)
            dataGrid.FindAll("tbody tr").Count.Should().Be(2);
        }

        [Test]
        public async Task DataGridFilterTemplateInSimpleMode_ChangingColumnShouldClearCustomFilterFunction()
        {
            var comp = Context.Render<DataGridFilterTemplateSimpleModeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterTemplateSimpleModeTest.Model>>();

            var departmentColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Department");
            var nameColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Name");
            var filterDefinition = new FilterDefinition<DataGridFilterTemplateSimpleModeTest.Model>
            {
                Column = departmentColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Engineering",
                FilterFunction = item => item.Department == "Engineering"
            };

            await comp.InvokeAsync(async () => await dataGrid.Instance.AddFilterAsync(filterDefinition));

            dataGrid.FindAll("tbody tr").Count.Should().Be(2);

            await comp.Find(".mud-button-root.filter-button").ClickAsync();

            var columnSelect = comp.FindComponents<MudSelect<Column<DataGridFilterTemplateSimpleModeTest.Model>>>()
                .First(x => x.Instance.Class == "filter-field");

            await comp.InvokeAsync(async () => await columnSelect.Instance.ValueChanged.InvokeAsync(nameColumn));
            dataGrid.Render();

            filterDefinition.Column.Should().Be(nameColumn);
            filterDefinition.Value.Should().BeNull();
            filterDefinition.FilterFunction.Should().BeNull("changing the filter row to another column must clear any custom predicate from the previous column");
            dataGrid.FindAll("tbody tr").Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridShowFilterIcon()
        {
            var comp = Context.Render<DataGridCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomFilteringTest.Model>>();
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Filterable, false));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".filter-button").Should().BeEmpty();
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Filterable, true));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".filter-button").Should().NotBeEmpty();
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.ShowFilterIcons, false));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".filter-button").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridServerDataColumnFilterMenu()
        {
            var comp = Context.Render<DataGridServerDataColumnFilterMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterMenuTest.Model>>();
            var callCountText = comp.FindComponent<MudText>();
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
            callCountText.Markup.Should().Contain("Server call count: 1");

            await comp.Find(".filter-button").ClickAsync();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Sam"));
            await comp.Find(".apply-filter-button").ClickAsync();
            callCountText.Markup.Should().Contain("Server call count: 2");
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);

            await comp.Find(".filter-button").ClickAsync();
            await comp.Find(".clear-filter-button").ClickAsync();
            callCountText.Markup.Should().Contain("Server call count: 3");
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridColumnFilterMenuCloseAction_ShouldCloseColumnFilterPopup()
        {
            var comp = Context.Render<DataGridServerDataColumnFilterMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterMenuTest.Model>>();
            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridServerDataColumnFilterMenuTest.Model>>().First();

            await comp.Find(".filter-button").ClickAsync();
            comp.FindAll(".clear-filter-button").Count.Should().Be(1);

            await comp.InvokeAsync(async () =>
            {
                await headerCell.Instance.Column!.FilterContext.Actions.CloseFilterAsync();
            });

            await comp.WaitForAssertionAsync(() => comp.FindAll(".clear-filter-button").Count.Should().Be(0));
        }

        [Test]
        public async Task DataGridFilterTemplateApplyActionInSimpleMode_ShouldKeepSimplePanelOpen()
        {
            var comp = Context.Render<DataGridFilterTemplateSimpleModeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterTemplateSimpleModeTest.Model>>();
            var departmentColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Department");

            await comp.InvokeAsync(async () =>
            {
                await dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterTemplateSimpleModeTest.Model>
                {
                    Column = departmentColumn,
                    Operator = FilterOperator.String.Equal,
                    Value = null
                });
            });

            await comp.Find(".mud-button-root.filter-button").ClickAsync();
            comp.FindAll(".filters-panel").Count.Should().Be(1);

            var departmentFilter = comp.FindComponents<MudSelect<string>>()
                .First(x => x.Instance.Class == "filter-input department-filter");

            await comp.InvokeAsync(async () => await departmentFilter.Instance.SelectedValuesChanged.InvokeAsync(new[] { "Engineering" }));

            comp.FindAll(".filters-panel").Count.Should().Be(1);
            dataGrid.FindAll("tbody tr").Count.Should().Be(2);
        }

        [Test]
        public async Task DataGridFilterTemplateClearActionInSimpleMode_ShouldKeepSimplePanelOpen()
        {
            var comp = Context.Render<DataGridFilterTemplateSimpleModeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterTemplateSimpleModeTest.Model>>();
            var departmentColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Department");
            var filterDefinition = new FilterDefinition<DataGridFilterTemplateSimpleModeTest.Model>
            {
                Column = departmentColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Engineering",
                FilterFunction = item => item.Department == "Engineering"
            };

            await comp.InvokeAsync(async () => await dataGrid.Instance.AddFilterAsync(filterDefinition));
            dataGrid.FindAll("tbody tr").Count.Should().Be(2);

            await comp.Find(".mud-button-root.filter-button").ClickAsync();
            comp.FindAll(".filters-panel").Count.Should().Be(1);

            var filterContext = dataGrid.Instance.CreateFilterContext(departmentColumn, filterDefinition);

            await comp.InvokeAsync(async () =>
            {
                await filterContext.Actions.ClearFilterAsync(filterDefinition);
            });

            comp.FindAll(".filters-panel").Count.Should().Be(1);
            dataGrid.FindAll("tbody tr").Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridFilterTemplateCloseActionInSimpleMode_ShouldCloseSimplePanel()
        {
            var comp = Context.Render<DataGridFilterTemplateSimpleModeTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterTemplateSimpleModeTest.Model>>();
            var departmentColumn = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Department");
            var filterDefinition = new FilterDefinition<DataGridFilterTemplateSimpleModeTest.Model>
            {
                Column = departmentColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Engineering",
                FilterFunction = item => item.Department == "Engineering"
            };

            await comp.InvokeAsync(async () => await dataGrid.Instance.AddFilterAsync(filterDefinition));
            await comp.Find(".mud-button-root.filter-button").ClickAsync();
            comp.FindAll(".filters-panel").Count.Should().Be(1);

            var filterContext = dataGrid.Instance.CreateFilterContext(departmentColumn, filterDefinition);

            await comp.InvokeAsync(async () =>
            {
                await filterContext.Actions.CloseFilterAsync();
            });

            comp.FindAll(".filters-panel").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGrid_ColumnFilterMenu_OpensAtCursorPosition()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/11518
            var comp = Context.Render<DataGridServerDataColumnFilterMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterMenuTest.Model>>();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);

            (double Top, double Left) openPosition = (50, 50);
            var mouseArgs = new MouseEventArgs
            {
                PageY = openPosition.Top,
                PageX = openPosition.Left
            };
            comp.Find(".filter-button").Click(mouseArgs);
            await comp.WaitForAssertionAsync(() => dataGrid.Instance._openPosition.Should().Be(openPosition));
        }

        [Test]
        public async Task DataGridServerDataColumnFilterMenuApplyTwice()
        {
            var comp = Context.Render<DataGridServerDataColumnFilterMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterMenuTest.Model>>();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            // Apply the filter the first time
            await comp.Find(".filter-button").ClickAsync();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Sam"));
            await comp.Find(".apply-filter-button").ClickAsync();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            // Apply the filter a second time
            await comp.Find(".filter-button").ClickAsync();
            await comp.Find(".apply-filter-button").ClickAsync();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridServerDataColumnFilterRow()
        {
            var comp = Context.Render<DataGridServerDataColumnFilterRowTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterRowTest.Model>>();
            var callCountText = comp.FindComponent<MudText>();
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
            callCountText.Markup.Should().Contain("Server call count: 1");

            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Sam"));
            callCountText.Markup.Should().Contain("Server call count: 2");
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);

            await comp.Find("th > div > button.mud-button-root").ClickAsync(); // Clear filter button
            callCountText.Markup.Should().Contain("Server call count: 3");
            dataGrid.Render();
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
        }

        [Test]
        public void DataGridColumnFilterRowProperty()
        {
            var comp = Context.Render<DataGridColumnFilterRowPropertyTest>();

            Assert.DoesNotThrow(() => comp.FindComponent<MudTextField<string>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudNumericField<double?>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudSelect<Enum>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudSelect<bool?>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudDatePicker>());
        }

        [Test]
        public async Task DataGridColumnFilterRowPropertyClear()
        {
            var comp = Context.Render<DataGridColumnFilterRowPropertyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnFilterRowPropertyTest.Model>>();

            var inputsBefore = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            var hireDate = new DateTime(2011, 1, 2).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture);
            inputsBefore.Should().BeEquivalentTo("Ira", "27", "Success", "True", hireDate, "00:00");

            IReadOnlyList<IElement> ClearButtons() => dataGrid.FindAll(".align-self-center");
            ClearButtons().Should().HaveCount(5);
            await ClearAllFiltersOneByOneAsync();

            var inputsAfter = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            inputsAfter.Should().HaveCount(6).And.AllBe("", because: "clicking the clear buttons should reset all filters");

            Func<Task> action = ClearAllFiltersOneByOneAsync;

            // We had regressions here before https://github.com/MudBlazor/MudBlazor/issues/10034
            await action.Should().NotThrowAsync("We click clear again to make sure that no exception appear when there are no filters left.");

            async Task ClearAllFiltersOneByOneAsync()
            {
                for (var index = 0; index < ClearButtons().Count; index++)
                {
                    var clearButton = ClearButtons()[index];
                    await clearButton.ClickAsync();
                }
            }
        }

        [Test]
        public async Task DataGridColumnFilterRowPropertyClearAll()
        {
            var comp = Context.Render<DataGridColumnFilterRowPropertyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnFilterRowPropertyTest.Model>>();

            var inputsBefore = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            var hireDate = new DateTime(2011, 1, 2).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture);
            inputsBefore.Should().BeEquivalentTo("Ira", "27", "Success", "True", hireDate, "00:00");

            await dataGrid.Find(".clear-all-filters").ClickAsync();

            var inputsAfter = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            inputsAfter.Should().HaveCount(6).And.AllBe("", because: "clicking the clear button should reset all filters");
        }

        [Test]
        public void DataGridStickyColumns()
        {
            var comp = Context.Render<DataGridStickyColumnsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridStickyColumnsTest.Model>>();

            dataGrid.Find("th").ClassList.Should().Contain("sticky-left");
            dataGrid.FindAll("th").Last().ClassList.Should().Contain("sticky-right");
        }

        [Test]
        public void DataGridStickyColumnsResizer()
        {
            var comp = Context.Render<DataGridStickyColumnsResizerTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridStickyColumnsResizerTest.Model>>();

            var header = dataGrid.Find(".mud-table-toolbar");
            header.GetAttribute("style").Should().Contain("position:sticky");
            header.GetAttribute("style").Should().Contain("left:0px");

            var footer = dataGrid.Find(".mud-table-pagination");
            footer.GetAttribute("style").Should().Contain("position:sticky");
            footer.GetAttribute("style").Should().Contain("left:0px");

            var body = dataGrid.Find(".mud-table-container");
            body.GetAttribute("style").Should().Contain("width:max-content");
            body.GetAttribute("style").Should().Contain("overflow:clip");

            dataGrid.Find("th").ClassList.Should().Contain("sticky-left");
            dataGrid.FindAll("th").Last().ClassList.Should().Contain("sticky-right");
        }

        [Test]
        public async Task DataGridCellContext()
        {
            var comp = Context.Render<DataGridCellContextTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellContextTest.Model>>();

            var item = dataGrid.Instance.Items.FirstOrDefault();

            var column = dataGrid.Instance.RenderedColumns.First();
            var cell = new Cell<DataGridCellContextTest.Model>(dataGrid.Instance, column, item);

            cell._cellContext.Selected.Should().Be(false);
            await cell._cellContext.Actions.SetSelectedItemAsync(true);
            cell._cellContext.Selected.Should().Be(true);

            await cell._cellContext.Actions.ToggleHierarchyVisibilityForItemAsync();
            cell._cellContext.OpenHierarchies.Should().Contain(item);
            cell._cellContext.Open.Should().Be(true);
            await cell._cellContext.Actions.ToggleHierarchyVisibilityForItemAsync();
            cell._cellContext.OpenHierarchies.Should().NotContain(item);
            cell._cellContext.Open.Should().Be(false);
        }

        [Test]
        public void DataGridAggregation()
        {
            var comp = Context.Render<DataGridAggregationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridAggregationTest.Model>>();

            dataGrid.FindAll("td.footer-cell")[1].TrimmedText().Should().Be("Average age is 56");
            dataGrid.FindAll("tfoot td.footer-cell")[1].TrimmedText().Should().Be("Average age is 43");
        }

        [Test]
        public void DataGridSequenceContainsNoElements()
        {
            // Arrange & Act
            var component = Context.Render<DataGridSequenceContainsNoElementsTest>();
            var dataGridComponent = () => component.FindComponent<MudDataGrid<DataGridSequenceContainsNoElementsTest.Model>>();

            // This test will result in an error if the 'sequence contains no elements' issue is present.
            // Assert
            dataGridComponent.Should().NotThrow();
        }

        [Test]
        public async Task DataGridObservability()
        {
            var comp = Context.Render<DataGridObservabilityTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridObservabilityTest.Model>>();

            var addButton = comp.Find(".add-item-btn");
            var removeButton = comp.Find(".remove-item-btn");

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);

            await addButton.ClickAsync();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(9);

            await removeButton.ClickAsync();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);
        }

        /// <summary>
        /// Checks that when the collection is modified, the change is applied in the rendering.
        /// </summary>
        /// <remarks>
        /// https://github.com/MudBlazor/MudBlazor/issues/11758
        /// </remarks>
        [Test]
        public void DataGridObservabilityTest2()
        {
            // Arrange

            var sup = Context.Render<DataGridObservabilityTest>();
            var comp = sup.Instance;
            var dataGrid = sup.FindComponent<MudDataGrid<DataGridObservabilityTest.Model>>();

            // Assert : Initial state with 8 rows

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);

            // Act : Add 2 items

            comp.AddItem();
            comp.AddItem();

            // Arrange : DataGrid should display 10 rows

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(10);

            // Act : Remove 3 items

            comp.RemoveItem();
            comp.RemoveItem();
            comp.RemoveItem();

            // Arrange : DataGrid should display 7 rows

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(7);
        }

        public void TableFilterGuid()
        {
            var comp = Context.Render<DataGridFilterGuid<Guid>>();
            var grid = comp.Instance.MudGridRef;

            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            var guidColumn = grid.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Id");

            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                Value = "invalid guid",
            });
            grid.FilteredItems.Count().Should().Be(0);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                Value = comp.Instance.Guid1,
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid1);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "not equals",
                Value = comp.Instance.Guid1,
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid2);
        }

        [Test]
        public void TableFilterNullableGuid()
        {
            var comp = Context.Render<DataGridFilterGuid<Guid?>>();
            var grid = comp.Instance.MudGridRef;

            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            var guidColumn = grid.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Id");

            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid?>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                //Value = "invalid guid", cannot be a string here...
                Value = Guid.Empty
            });
            comp.Render();
            grid.FilteredItems.Count().Should().Be(0);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid?>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                Value = comp.Instance.Guid1,
            });
            comp.Render();
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid1);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid?>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "not equals",
                Value = comp.Instance.Guid1,
            });
            comp.Render();
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid2);
        }

        //[Test]
        //public async Task TableFilterGuidInDictionary()
        //{
        //    var comp = Context.Render<DataGridFilterDictionaryGuid>();
        //    var grid = comp.Instance.MudGridRef;

        //    grid.Items.Count().Should().Be(2);
        //    grid.FilteredItems.Count().Should().Be(2);
        //    var guidColumn = grid.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Id");

        //    grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
        //    {
        //        Column = guidColumn,
        //        Operator = "equals",
        //        Value = "invalid guid",
        //    });
        //    grid.FilteredItems.Count().Should().Be(0);

        //    grid.FilterDefinitions.Clear();
        //    grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
        //    {
        //        Column = guidColumn,
        //        Operator = "equals",
        //        Value = comp.Instance.Guid1,
        //    });
        //    grid.FilteredItems.Count().Should().Be(1);
        //    grid.FilteredItems.FirstOrDefault()["Id"].Should().Be(Guid.Parse(comp.Instance.Guid1));

        //    grid.FilterDefinitions.Clear();
        //    grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
        //    {
        //        Column = guidColumn,
        //        Operator = "not equals",
        //        Value = comp.Instance.Guid1,
        //    });
        //    grid.FilteredItems.Count().Should().Be(1);
        //    grid.FilteredItems.FirstOrDefault()["Id"].Should().Be(Guid.Parse(comp.Instance.Guid2));
        //}

        [Test]
        public void DataGridCultureColumnSimple()
        {
            var comp = Context.Render<DataGridCultureSimpleTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureSimpleTest.Model>>();

            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("3.5");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("5,2");
        }

        [Test]
        public void DataGridCultureColumnEditable()
        {
            var comp = Context.Render<DataGridCultureEditableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureEditableTest.Model>>();

            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("3.5");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("5,2");
        }

        [Test]
        public async Task DataGridCultureColumnFilter()
        {
            var comp = Context.Render<DataGridCultureSimpleTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureSimpleTest.Model>>();

            // amount with invariant culture (decimals separated by point)
            var amountHeader = dataGrid.FindAll("th .mud-menu button")[2];
            await amountHeader.ClickAsync();

            var filterAmount = comp.FindAll(".mud-menu-item")[1];
            await filterAmount.ClickAsync();

            var filterField = comp.Find(".filters-panel .filter-field .mud-select-input");
            filterField.TextContent.Trim().Should().Be("Amount");

            var filterInput = comp.FindAll(".filters-panel input")[2];
            await filterInput.InputAsync(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(22.0);

            dataGrid.Instance.FilterDefinitions.Clear();
            dataGrid.Render();

            // total with es-ES culture (decimals separated by comma)
            var totalHeader = dataGrid.FindAll("th .mud-menu button")[3];
            await totalHeader.ClickAsync();
            var filterTotal = comp.FindAll(".mud-menu-item")[1];
            await filterTotal.ClickAsync();

            var filterTotalField = comp.Find(".filters-panel .filter-field .mud-select-input");
            filterTotalField.TextContent.Trim().Should().Be("Total");

            var filterTotalInput = comp.FindAll(".filters-panel input")[2];
            await filterTotalInput.InputAsync(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(2.2);
        }

        [Test]
        public async Task DataGridCultureColumnFilterHeader()
        {
            var comp = Context.Render<DataGridCultureEditableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureEditableTest.Model>>();

            // amount with invariant culture (decimals separated by point)
            var filterAmount = dataGrid.FindAll("th.filter-header-cell input")[2];
            await filterAmount.InputAsync(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(22.0);

            dataGrid.Instance.FilterDefinitions.Clear();
            dataGrid.Render();

            var cells = dataGrid.FindAll(".mud-table-body td input[value]");

            // We want to check the cell values since there are cases when something is broken with MudNumericField's culture, and you won't know without checking values
            cells[0].GetAttribute("value").Should().Be("Sam"); cells[1].GetAttribute("value").Should().Be("56"); cells[2].GetAttribute("value").Should().Be("3.5"); cells[3].GetAttribute("value").Should().Be("5,2");
            cells[4].GetAttribute("value").Should().Be("Alicia"); cells[5].GetAttribute("value").Should().Be("54"); cells[6].GetAttribute("value").Should().Be("3.6"); cells[7].GetAttribute("value").Should().Be("4,8");
            cells[8].GetAttribute("value").Should().Be("Ira"); cells[9].GetAttribute("value").Should().Be("27"); cells[10].GetAttribute("value").Should().Be("3.9"); cells[11].GetAttribute("value").Should().Be("6,2");
            cells[12].GetAttribute("value").Should().Be("John"); cells[13].GetAttribute("value").Should().Be("32"); cells[14].GetAttribute("value").Should().Be("4.2"); cells[15].GetAttribute("value").Should().Be("3,2");

            // total with es-ES culture (decimals separated by comma)
            var filterTotal = dataGrid.FindAll("th.filter-header-cell input")[3];
            await filterTotal.InputAsync(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(2.2);
        }

        [Test]
        public void DataGridCultureColumnOverrides()
        {
            var comp = Context.Render<DataGridCulturesTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCulturesTest.Model>>();

            // amount with invariant culture (decimals separated by point)
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("3.5");
            // total with 'es' culture (decimals separated by commas)
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("5,2");
            // distance with custom culture (decimals separated by '#')
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("2#1");
        }

        [Test]
        public async Task DataGridSortIndicator()
        {
            var comp = Context.Render<DataGridSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTest.Item>>();

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Ascending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Value");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Descending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Descending);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-desc").Should().Be(true);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Ascending);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.None);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);
        }

        [Test]
        public async Task DataGridParentAndChildSamePropertyNameSort()
        {
            var comp = Context.Render<DataGridChildPropertiesWithSameNameSortTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridChildPropertiesWithSameNameSortTest.Employee>>();

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Manager.Name", SortDirection.Ascending, x => x.Manager.Name));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Name");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Manager.Name").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Name));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[0].TextContent.Trim().Should().Be("Name");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Ascending);
            dataGrid.Instance.GetColumnSortDirection("Manager.Name").Should().Be(SortDirection.None);
        }

        [Test]
        public async Task DataGridCustomSort()
        {
            var comp = Context.Render<DataGridCustomSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomSortableTest.Item>>();
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortMode, SortMode.Single));
            dataGrid.Instance.SortMode.Should().Be(SortMode.Single);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Ascending, x => x.Value, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Value");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Descending, x => x.Value, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Descending);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-desc").Should().Be(true);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Ascending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Value");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Name, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Ascending);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.None);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => x.Name, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Descending);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.None);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-desc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortMode, SortMode.Multiple));
            dataGrid.Instance.SortMode.Should().Be(SortMode.Multiple);

            //Assign a comparer to a column
            var column = dataGrid.FindComponent<Column<DataGridCustomSortableTest.Item>>();
            await column.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.Comparer, new MudBlazor.Utilities.NaturalComparer()));
            //Clear sorting
            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            //Sort by clicking on the header cell
            await dataGrid.Find(".column-options button").ClickAsync();
            var cells = dataGrid.FindAll("td");

            // Check the values of rows - should not be sorted and should be in the original order.
            cells[0].TextContent.Should().Be("0");
            cells[3].TextContent.Should().Be("1");
            cells[6].TextContent.Should().Be("1_2");
            cells[9].TextContent.Should().Be("1_10");
            cells[12].TextContent.Should().Be("1_11");
            cells[15].TextContent.Should().Be("2");
            cells[18].TextContent.Should().Be("10");

            //Multi click second column
            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridCustomSortableTest.Item>>()[1];
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { CtrlKey = true, Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Ascending);

            //Multi click second column a second time to change it to descending
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { CtrlKey = true, Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Descending);

            //remove first column from sort
            headerCell = dataGrid.FindComponents<HeaderCell<DataGridCustomSortableTest.Item>>()[0];
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { AltKey = true, Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.None);
        }

        [Test]
        public async Task DataGridPropertyColumnFormat()
        {
            var comp = Context.Render<DataGridFormatTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormatTest.Employee>>();

            comp.FindAll("tbody.mud-table-body td")[3].TextContent.Should().Be("$87,000.00");
            var column = (PropertyColumn<DataGridFormatTest.Employee, int>)dataGrid.Instance.GetColumnByPropertyName("Salary");
#pragma warning disable BL0005
            await comp.InvokeAsync(() => column.Format = "C0");
#pragma warning restore BL0005
            await comp.Find(".mud-switch-input").ChangeAsync(new ChangeEventArgs { Value = true });
            comp.FindAll("tbody.mud-table-body td")[3].TextContent.Should().Be("$87,000");
        }

        [Test]
        public async Task DataGridFilteredItemsCache()
        {
            var comp = Context.Render<DataGridSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTest.Item>>();

            var initialFilterCount = dataGrid.Instance.FilteringRunCount;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Name));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 1);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => x.Name));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 2);

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 3);

            var column = dataGrid.FindComponent<Column<DataGridSortableTest.Item>>();
            await column.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortBy, x => x.Name));
            dataGrid.Render();
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 4);

            // sort through the sort icon
            await dataGrid.Find(".column-options button").ClickAsync();
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 5);

            // test other sort methods
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs()));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 6);

            //await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 7);
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter(new MouseEventArgs()));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 8);
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters(new MouseEventArgs()));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 9);

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SortMode, SortMode.None));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 10);
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridMultiSelectOnRowClick()
        {
            var comp = Context.Render<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            // click on the first row
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("tbody.mud-table-body td")[1].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(1); //ensure selection is rendered

            // click on the second row
            await dataGrid.FindAll("tbody.mud-table-body td")[2].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(2);

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectOnRowClick, false));

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);

            // click on the first row
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("tbody.mud-table-body td")[1].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridSingleSelectOnRowClick()
        {
            var comp = Context.Render<DataGridSingleSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSingleSelectionTest.Item>>();

            // click on the first row
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("tbody.mud-table-body td")[1].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(1); //ensure selection is rendered

            // click on the second row
            await dataGrid.FindAll("tbody.mud-table-body td")[2].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(1);

            // click on the second row
            await dataGrid.FindAll("tbody.mud-table-body td")[2].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.SelectOnRowClick, false));

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            // click on the first row
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            await dataGrid.FindAll("tbody.mud-table-body td")[1].ClickAsync();
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridDragAndDrop()
        {
            var comp = Context.Render<DataGridDragAndDropTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridDragAndDropTest.Model>>();
            dataGrid.Instance.DropContainerHasChanged();

            var headerValues = dataGrid.FindAll(".sortable-column-header");
            headerValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            headerValues[0].InnerHtml.Should().Be("Name");
            headerValues[1].InnerHtml.Should().Be("Age");
            headerValues[2].InnerHtml.Should().Be("Status");
            headerValues[3].InnerHtml.Should().Be("Hired");
            headerValues[4].InnerHtml.Should().Be("HiredOn");

            var container = dataGrid.Find(".mud-drop-container");
            container.Children.Should().HaveCount(1);

            var zone = dataGrid.FindAll(".mud-drop-zone");
            zone.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            var firstDropZone = zone[1];
            var firstDropItem = firstDropZone.Children[0];

            var secondDropZone = zone[2];
            var secondDropItem = secondDropZone.Children[0];

            await firstDropItem.DragStartAsync(new DragEventArgs());
            await secondDropItem.DropAsync(new DragEventArgs());

            var newHeaderValues = dataGrid.FindAll(".sortable-column-header");
            newHeaderValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            newHeaderValues[0].InnerHtml.Should().Be("Name");
            newHeaderValues[1].InnerHtml.Should().Be("Status");
            newHeaderValues[2].InnerHtml.Should().Be("Age");
            newHeaderValues[3].InnerHtml.Should().Be("Hired");
            newHeaderValues[4].InnerHtml.Should().Be("HiredOn");

        }
        [Test]
        public async Task DataGridEditFormDialogIsCustomizable()
        {
            var comp = Context.Render<DataGridEditFormCustomizedDialogTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditFormCustomizedDialogTest.Model>>();

            //open edit dialog
            await dataGrid.FindAll("tbody tr")[1].ClickAsync();
            //check if dialog is open
            comp.FindAll("div.mud-dialog-container").Should().NotBeEmpty();
            //find button with arialabel close in dialog
            var closeButton = comp.Find("button[aria-label=\"Close\"]");
            closeButton.Should().NotBeNull();
            //click close button
            await comp.Find("button[aria-label=\"Close\"]").ClickAsync();
            //check if dialog is closed
            comp.FindAll("div.mud-dialog-container").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridDragAndDropWithDynamicColumns()
        {
            var comp = Context.Render<DataGridDragAndDropWithDynamicColumnsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridDragAndDropWithDynamicColumnsTest.Model>>();
            dataGrid.Instance.DropContainerHasChanged();

            var headerValues = dataGrid.FindAll(".sortable-column-header");
            headerValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            headerValues[0].InnerHtml.Should().Be("Name");
            headerValues[1].InnerHtml.Should().Be("Age");
            headerValues[2].InnerHtml.Should().Be("Status");
            headerValues[3].InnerHtml.Should().Be("Hired");
            headerValues[4].InnerHtml.Should().Be("HiredOn");

            var container = dataGrid.Find(".mud-drop-container");
            container.Children.Should().HaveCount(1);

            var zone = dataGrid.FindAll(".mud-drop-zone");
            zone.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            var firstDropZone = zone[1];
            var firstDropItem = firstDropZone.Children[0];

            var secondDropZone = zone[2];
            var secondDropItem = secondDropZone.Children[0];

            await firstDropItem.DragStartAsync(new DragEventArgs());
            await secondDropItem.DropAsync(new DragEventArgs());

            var newHeaderValues = dataGrid.FindAll(".sortable-column-header");
            newHeaderValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            newHeaderValues[0].InnerHtml.Should().Be("Name");
            newHeaderValues[1].InnerHtml.Should().Be("Status");
            newHeaderValues[2].InnerHtml.Should().Be("Age");
            newHeaderValues[3].InnerHtml.Should().Be("Hired");
            newHeaderValues[4].InnerHtml.Should().Be("HiredOn");
        }

        [Test]
        public async Task DataGridRedundantMenu()
        {
            var comp = Context.Render<DataGridRedundantMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridRedundantMenuTest.Model>>();

            await dataGrid.SetParametersAndRenderAsync(parameters => parameters
                .Add(x => x.FilterMode, DataGridFilterMode.ColumnFilterRow)
                .Add(x => x.SortMode, SortMode.None));

            // Assert that the `column-options` span is present but empty
            var columnOptionsSpan = comp.Find(".column-options");
            columnOptionsSpan.Should().NotBeNull();
            columnOptionsSpan.TextContent.Trim().Should().BeEmpty();
        }

        [Test]
        public void DataGridDynamicColumns()
        {
            var comp = Context.Render<DataGridDynamicColumnsTest>();

            comp.Instance.GridRenderedColumnsCount.Should().Be(0);

            comp.Instance.AddColumns();

            comp.Instance.GridRenderedColumnsCount.Should().Be(3);

            comp.Instance.RemoveColumn();

            comp.Instance.GridRenderedColumnsCount.Should().Be(2);

            comp.Instance.RemoveAllColumns();

            comp.Instance.GridRenderedColumnsCount.Should().Be(0);
        }

        [Test]
        public async Task DataGridSelectColumn()
        {
            var comp = Context.Render<DataGridSelectColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<int>>();
            var rowCheckbox = dataGrid.FindAll("td input");
            var selectAllCheckboxes = dataGrid.FindComponents<MudCheckBox<bool?>>();

            selectAllCheckboxes[0].Instance.ReadValue.Should().BeFalse();
            selectAllCheckboxes[1].Instance.ReadValue.Should().BeFalse();

            await rowCheckbox[0].ChangeAsync(true);

            selectAllCheckboxes[0].Instance.ReadValue.Should().BeNull();
            selectAllCheckboxes[1].Instance.ReadValue.Should().BeNull();

            await rowCheckbox[1].ChangeAsync(true);

            selectAllCheckboxes[0].Instance.ReadValue.Should().BeTrue();
            selectAllCheckboxes[1].Instance.ReadValue.Should().BeTrue();

            await rowCheckbox[1].ChangeAsync(false);

            selectAllCheckboxes[0].Instance.ReadValue.Should().BeNull();
            selectAllCheckboxes[1].Instance.ReadValue.Should().BeNull();

            await rowCheckbox[0].ChangeAsync(false);

            selectAllCheckboxes[0].Instance.ReadValue.Should().BeFalse();
            selectAllCheckboxes[1].Instance.ReadValue.Should().BeFalse();
        }

        [Test]
        public async Task FilterDefinitionTestHasFilterProperty()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFiltersTest.Model>
            { Column = dataGrid.Instance.GetColumnByPropertyName("Name"), Operator = FilterOperator.String.Empty }));

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFiltersTest.Model>
            { Column = dataGrid.Instance.GetColumnByPropertyName("Age"), Operator = FilterOperator.Number.GreaterThan, Value = 30 }));

            // test filter definition without value
            var nameHeaderCell = dataGrid.FindComponents<HeaderCell<DataGridFiltersTest.Model>>()[0];
            nameHeaderCell.Instance.hasFilter.Should().BeTrue();

            // test filter definition with value
            var ageHeaderCell = dataGrid.FindComponents<HeaderCell<DataGridFiltersTest.Model>>()[1];
            ageHeaderCell.Instance.hasFilter.Should().BeTrue();

            // test filter not applied
            var statusHeaderCell = dataGrid.FindComponents<HeaderCell<DataGridFiltersTest.Model>>()[2];
            statusHeaderCell.Instance.hasFilter.Should().BeFalse();
        }

        /// <summary>
        /// Reproduce the bug from https://github.com/MudBlazor/MudBlazor/issues/9585
        /// When a column is hidden by the menu and the precedent column is resized, then the app crash
        /// </summary>
        [Test]
        public async Task DataGrid_ResizeColumn_WhenNeighboringColumnIsHidden()
        {
            // Arrange

            var comp = Context.Render<DataGridHideAndResizeTest>();
            var dgComp = comp.FindComponent<MudDataGrid<DataGridHideAndResizeTest.Model>>();

            // Act : Hide the middle column and resize the first column

            // Open column the second column header menu
            var columnMenu = comp.FindAll("th .mud-menu button").ElementAt(1);
            await columnMenu.ClickAsync();

            // Click on the menu item 'Hide'
            await comp.WaitForAssertionAsync(() => comp.FindAll(".mud-menu-item").ElementAt(1));
            var hideMenuItem = comp.FindAll(".mud-menu-item").ElementAt(1);
            await hideMenuItem.ClickAsync();

            // Mock mudElementRef.getBoundingClientRect for DataGrid and visible columns
            var gridElement = (ElementReference)dgComp.Instance.GetType()
                .GetField("_gridElement", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(dgComp.Instance)!;
            Context.JSInterop
              .Setup<Interop.BoundingClientRect>("mudElementRef.getBoundingClientRect", gridElement)
              .SetResult(new Interop.BoundingClientRect { Width = 50 });
            var colComps = comp.FindComponents<HeaderCell<DataGridHideAndResizeTest.Model>>();
            foreach (var colComp in colComps)
            {
                var col = colComp.Instance;
                if (!col.Column.HiddenState.Value)
                {
                    var headerElement = (ElementReference)col.GetType()
                        .GetField("_headerElement", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .GetValue(col)!;
                    Context.JSInterop
                        .Setup<Interop.BoundingClientRect>("mudElementRef.getBoundingClientRect", headerElement)
                        .SetResult(new Interop.BoundingClientRect { Width = 50 });
                }
            }

            // Mouse click down
            var resizer = () => comp.FindAll(".mud-resizer").ElementAt(0);
            await resizer().PointerDownAsync(new PointerEventArgs { ClientX = 100, PointerId = 1, Detail = 1 });

            // Simulate pointer move and release (simplified since we're using pointer events directly)
            await resizer().PointerMoveAsync(new PointerEventArgs { ClientX = 90, PointerId = 1 });
            await resizer().PointerUpAsync(new PointerEventArgs { ClientX = 90, PointerId = 1 });

            // Assert
            comp.FindAll("th").Count.Should().Be(2, "Two columns are displayed");
            comp.Find("th").GetStyle().Should().Contain(cssProp => cssProp.Name == "width", "The first column is resized");
        }

        [Test]
        public void QueryFilterExtension()
        {
            var comp = Context.Render<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            var nameFilter = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Name"),
                Operator = FilterOperator.String.Contains,
                Value = "Sam",
            };
            var ageFilter = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Age"),
                Operator = FilterOperator.Number.GreaterThan,
                Value = 42,
            };

            var query = Array.Empty<DataGridFiltersTest.Model>().AsQueryable().Where([nameFilter, ageFilter]);
            query.ToString().Should().Match("*x.Name.Contains(*x.Age > Convert(42*");
        }

        [Test]
        public void QuerySortExtension()
        {
            var nameSort = new SortDefinition<DataGridFiltersTest.Model>("Name", Descending: true, 0, default!);
            var ageSort = new SortDefinition<DataGridFiltersTest.Model>("Age", Descending: false, 1, default!);

            var query = Array.Empty<DataGridFiltersTest.Model>().AsQueryable().OrderBy([nameSort, ageSort]);
            query.ToString().Should().Be("MudBlazor.UnitTests.TestComponents.DataGrid.DataGridFiltersTest+Model[].OrderByDescending(x => x.Name).ThenBy(x => x.Age)");
        }

        [Test]
        public void QuerySortExtensionTestAscendingThenDescending()
        {
            var nameSort = new SortDefinition<DataGridFiltersTest.Model>("Name", Descending: false, 0, default!);
            var ageSort = new SortDefinition<DataGridFiltersTest.Model>("Age", Descending: true, 1, default!);

            var query = Array.Empty<DataGridFiltersTest.Model>().AsQueryable().OrderBy([nameSort, ageSort]);
            query.ToString().Should().Be("MudBlazor.UnitTests.TestComponents.DataGrid.DataGridFiltersTest+Model[].OrderBy(x => x.Name).ThenByDescending(x => x.Age)");
        }

        [Test]
        public void QuerySortExtensionTestEmptyDefinitions()
        {
            var source = Array.Empty<DataGridFiltersTest.Model>().AsQueryable();
            var query = source.OrderBy([]);
            query.Should().BeSameAs(source);
        }

        [Test]
        public async Task DataGridEnumLocalization()
        {
            var comp = Context.Render<DataGridFilterEnumLocalizationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterEnumLocalizationTest.Item>>();

            IElement FilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            await FilterButton().ClickAsync();

            IElement SelectElement() => comp.Find("div.mud-select.filter-input");
            await SelectElement().MouseDownAsync(new MouseEventArgs());

            var items = comp.FindAll("div.mud-list-item").ToArray();

            items.Length.Should().Be(4);
            items[0].TextContent.Should().BeEmpty();
            items[1].TextContent.Should().Be("Free education");
            items[2].TextContent.Should().Be("Paid training");
            items[3].TextContent.Should().Be("Untranslated");
        }

        [Test]
        public async Task DataGridValidatorFormBinding()
        {
            var comp = Context.Render<DataGridValidatorTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridValidatorTest.Item>>().Instance;
            dataGrid.Validator.Should().BeSameAs(form);

            var textField = comp.FindComponent<MudTextField<string>>();
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();

            // input valid value into text field
            await textField.Find("input").InputAsync("not empty");

            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();

            // input invalid value into text field
            await textField.Find("input").InputAsync("");

            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeFalse();
        }

        /// <summary>
        /// Tests two-way binding on the CurrentPage parameter.
        /// The table should re-render with the newly provided value as the CurrentPage.
        /// </summary>
        [Test]
        public async Task TestCurrentPageParameterTwoWayBinding()
        {
            var comp = Context.Render<DataGridCurrentPageParameterTwoWayBindingTest>();
            var dataGridComponent = comp.FindComponent<MudDataGrid<int>>();
            var dataGrid = dataGridComponent.Instance;

            // Assert starting page index is 0 (default).
            await comp.WaitForAssertionAsync(() => dataGrid.CurrentPage.Should().Be(0));
            await comp.WaitForAssertionAsync(() => comp.Find(".mud-table-body .mud-table-row .mud-table-cell").TextContent.Should().Be("1"));

            // Assert modification via code correctly renders the corresponding page.
            await dataGridComponent.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.CurrentPage, 1));
            await comp.WaitForAssertionAsync(() => dataGrid.CurrentPage.Should().Be(1));
            await comp.WaitForAssertionAsync(() => comp.Find(".mud-table-body .mud-table-row .mud-table-cell").TextContent.Should().Be("2"));

            // Assert user input correctly updates the CurrentPage parameter value by clicking the "Next Page" button in the pager.
            await comp.FindAll(".mud-table-pagination-actions .mud-button-root")[2].ClickAsync();
            await comp.WaitForAssertionAsync(() => dataGrid.CurrentPage.Should().Be(2));
            await comp.WaitForAssertionAsync(() => comp.Find(".mud-table-body .mud-table-row .mud-table-cell").TextContent.Should().Be("3"));
        }

        /// <summary>
        /// Verifies data grid does not reuse row child components for different items (the @key for the row is set to the user supplied item).
        /// </summary>
        [Test]
        public async Task DataGridUniqueRowKey()
        {
            //Test the normal case
            var comp = Context.Render<DataGridUniqueRowKeyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<string>>();

            var sortByColumnName = dataGrid.Instance.RenderedColumns.FirstOrDefault().PropertyName;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(sortByColumnName, SortDirection.Ascending, x => x));
            var before = dataGrid.FindComponent<MudInput<string>>();
            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(sortByColumnName, SortDirection.Descending, x => x));
            var after = dataGrid.FindComponent<MudInput<string>>();

            before.Should().NotBeSameAs(after, because: "If the @key is correctly set to the row item, child components will be recreated on row reordering.");

            //Test the expanded group case
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Group, true));
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(sortByColumnName, SortDirection.Ascending, x => x));
            before = dataGrid.FindComponent<MudInput<string>>();
            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(sortByColumnName, SortDirection.Descending, x => x));
            after = dataGrid.FindComponent<MudInput<string>>();

            before.Should().NotBeSameAs(after, because: "If the @key is correctly set to the row item, child components will be recreated on row reordering.");
        }

        [Test]
        public async Task DataGrid_TwoWayBind_SelectedItem_SelectedItems()
        {
            int selectedItem = 3;
            var items = new List<int> { 1, 2, 3, 4, 5 };
            HashSet<int> selectedItems = new HashSet<int> { selectedItem };
            var comp = Context.Render<MudDataGrid<int>>(parameters =>
            {
                parameters.Add(x => x.Items, items);
                parameters.Bind(x => x.SelectedItem, selectedItem, x => selectedItem = x);
                parameters.Bind(x => x.SelectedItems, selectedItems, x => selectedItems = x);
                parameters.Add(x => x.MultiSelection, false);
            });

            comp.Instance.Items.Count().Should().Be(items.Count);
            comp.Instance.GetState(x => x.SelectedItem).Should().Be(selectedItem);
            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(selectedItem);

            // in single selection toggle selection using row click method
            await comp.Instance.SetSelectedItemAsync(5);

            // two way binding should have updated
            selectedItems.Should().Contain(5);
            selectedItems.Count.Should().Be(1);
            selectedItem.Should().Be(5);

            // in multi selection toggle selection using row click method
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.MultiSelection, true));
            comp.Render();
            await comp.Instance.SetSelectedItemAsync(4);

            // two way binding should have updated
            selectedItems.Should().Contain(4);
            selectedItems.Should().Contain(5);
            selectedItems.Count.Should().Be(2);
            selectedItem.Should().Be(4);
        }

        [Test]
        public async Task DataGridSelectedItemEvents()
        {
            var comp = Context.Render<DataGridEventCallbacksTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEventCallbacksTest.Item>>();

            // Test single selection mode
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.MultiSelection, false));
            comp.Render();

            // Select an item
            var firstItem = dataGrid.Instance.Items.First();
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(true, firstItem));

            // Verify events
            comp.Instance.SelectedItemChanged.Should().BeTrue();
            comp.Instance.SelectedItemsChanged.Should().BeTrue();

            // Reset event flags
            comp.Instance.SelectedItemChanged = false;
            comp.Instance.SelectedItemsChanged = false;

            // Deselect the item
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, firstItem));

            // Verify events for deselection
            comp.Instance.SelectedItemChanged.Should().BeTrue();
            comp.Instance.SelectedItemsChanged.Should().BeTrue();

            // Reset event flags
            comp.Instance.SelectedItemChanged = false;
            comp.Instance.SelectedItemsChanged = false;

            // Test multi-selection mode
            await dataGrid.SetParametersAndRenderAsync(parameters => parameters.Add(x => x.MultiSelection, true));
            comp.Render();

            // Select all items
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(true));

            // Verify events
            comp.Instance.SelectedItemChanged.Should().BeFalse();
            comp.Instance.SelectedItemsChanged.Should().BeTrue();

            // Reset event flags
            comp.Instance.SelectedItemChanged = false;
            comp.Instance.SelectedItemsChanged = false;

            // Deselect all items
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));

            // Verify events for deselection
            comp.Instance.SelectedItemChanged.Should().BeFalse();
            comp.Instance.SelectedItemsChanged.Should().BeTrue();

            // Reset event flags
            comp.Instance.SelectedItemChanged = false;
            comp.Instance.SelectedItemsChanged = false;

            // Test row click select
            // find first mud-table-row and second mud-table-cell
            var firstRow = dataGrid.FindAll(".mud-table-row")[1];
            await firstRow.ClickAsync();

            // Verify events for row click
            await comp.WaitForAssertionAsync(() => comp.Instance.SelectedItemChanged.Should().BeTrue());
            comp.Instance.SelectedItemsChanged.Should().BeTrue();

            // Reset event flags
            comp.Instance.SelectedItemChanged = false;
            comp.Instance.SelectedItemsChanged = false;
        }

        [Test]
        public async Task DataGridHeaderToggleHierarchy()
        {
            // Render with EnableHeaderToggle = true to enable header toggle functionality
            var comp = Context.Render<DataGridHierarchyColumnTest>(parameters =>
                parameters.Add(p => p.EnableHeaderToggle, true));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            // Find the header cell that should include hierarchy toggle
            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridHierarchyColumnTest.Model>>().First();

            var headerElement = comp.Find("th.mud-header-togglehierarchy");
            headerElement.Should().NotBeNull("Header should have mud-header-togglehierarchy class when EnableHeaderToggle is true");
            headerCell.Instance.IncludeHierarchyToggle.Should().BeTrue();

            // Check that the HierarchyToggle button exists in the header
            var toggleButton = headerElement.QuerySelector(".mud-hierarchy-toggle-button");
            toggleButton.Should().NotBeNull("HierarchyToggle button should be rendered in header");

            // The initial state should be expanded (Anders and Ira items are initially expanded)
            dataGrid.Instance._openHierarchies.Count.Should().Be(2);

            // Click the toggle button to collapse all hierarchies
            await toggleButton.ClickAsync();
            await comp.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(0));

            // Click again to expand all
            toggleButton = headerElement.QuerySelector(".mud-hierarchy-toggle-button");
            await toggleButton.ClickAsync();
            await comp.WaitForAssertionAsync(() => dataGrid.Instance._openHierarchies.Count.Should().Be(4)); // one disabled
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task DataGridHeaderToggleIcon(bool rightToLeft)
        {
            // Render with EnableHeaderToggle = true and set RTL mode
            var comp = Context.Render<DataGridHierarchyColumnTest>(parameters =>
            {
                parameters.Add(p => p.EnableHeaderToggle, true);
                parameters.Add(p => p.RightToLeft, rightToLeft);
            });
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            // Find the header with toggle
            var headerElement = comp.Find("th.mud-header-togglehierarchy");

            // Find the toggle button in header
            var toggleButton = headerElement.QuerySelector(".mud-hierarchy-toggle-button");
            var icon = toggleButton.QuerySelector(".mud-icon-root");

            // Initial state should show expanded icon (ExpandMore)
            var iconPath = icon.InnerHtml;
            iconPath.Should().Contain("M16.59 8.59L12 13.17 7.41 8.59 6 10l6 6 6-6z",
                "Icon should be ExpandMore when hierarchies are expanded");

            // Click to collapse all
            await toggleButton.ClickAsync();

            // Now the icon should change based on RTL mode
            icon = headerElement.QuerySelector(".mud-hierarchy-toggle-button .mud-icon-root");
            iconPath = icon.InnerHtml;

            if (rightToLeft)
            {
                iconPath.Should().Contain("M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z",
                    "Icon should be ChevronLeft in RTL mode when hierarchies are collapsed");
            }
            else
            {
                iconPath.Should().Contain("M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z",
                    "Icon should be ChevronRight in LTR mode when hierarchies are collapsed");
            }
        }

        [Test]
        public async Task DataGridToggleHierarchyMethod()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridHierarchyColumnTest.Model>>().First();

            // Initially, there should be 2 expanded items
            dataGrid.Instance._openHierarchies.Count.Should().Be(2);
            var accessor = headerCell.Instance;
            await accessor.ToggleHierarchyAsync();

            // After calling ToggleHierarchy when some hierarchies are open, all should be collapsed
            dataGrid.Instance._openHierarchies.Count.Should().Be(0);

            // Call ToggleHierarchy again
            await accessor.ToggleHierarchyAsync();

            // Now all hierarchies should be expanded (except the disabled one)
            dataGrid.Instance._openHierarchies.Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridGetHierarchyGroupIcon()
        {
            // Create a test component
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            // Get a reference to a HeaderCell to test GetGroupIcon method
            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridHierarchyColumnTest.Model>>().First();

            // Create a PrivateAccessor to invoke the GetGroupIcon method
            var accessor = headerCell.Instance;

            // When expanded (RTL doesn't matter in this case)
            var expandedIcon = accessor.GetGroupIcon();
            expandedIcon.Should().Be(Icons.Material.Filled.ExpandMore);

            await accessor.ToggleHierarchyAsync(); // collapse all

            // When collapsed + LTR
            var collapsedIcon = accessor.GetGroupIcon();
            await comp.WaitForAssertionAsync(() => collapsedIcon.Should().Be(Icons.Material.Filled.ChevronRight));

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.RightToLeft, true));
            // When collapsed + RTL
            await comp.WaitForAssertionAsync(() => accessor.GetGroupIcon().Should().Be(Icons.Material.Filled.ChevronLeft));
        }

        [Test]
        public async Task DataGrid_HierarchyExpandSingleRow()
        {
            var comp = Context.Render<DataGridHierarchyColumnTest>(parameters => parameters
                .Add(p => p.ExpandSingleRow, false));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            dataGrid.Instance._openHierarchies.Count.Should().Be(2);
            var item = dataGrid.Instance._openHierarchies.First();
            item.Should().NotBeNull();

            await comp.SetParametersAndRenderAsync(p => p.Add(p => p.ExpandSingleRow, true));

            dataGrid.Instance._openHierarchies.Count.Should().Be(1);

            dataGrid.Instance._openHierarchies.First().Should().Be(item);
        }

        public class TestDataItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool ShouldBeDisabled { get; set; }
        }

        private static RenderFragment SelectColumnWithFunc => builder =>
        {
            builder.OpenComponent<SelectColumn<TestDataItem>>(0);
            builder.AddAttribute(1, nameof(SelectColumn<TestDataItem>.DisabledFunc), (Func<TestDataItem, bool>)(item => item.ShouldBeDisabled));
            builder.CloseComponent();
            builder.OpenComponent<PropertyColumn<TestDataItem, int>>(2);
            builder.AddAttribute(3, nameof(PropertyColumn<TestDataItem, int>.Property), (Expression<Func<TestDataItem, int>>)(x => x.Id));
            builder.CloseComponent();
        };

        private static RenderFragment SelectColumnNoFunc => builder =>
        {
            builder.OpenComponent<SelectColumn<TestDataItem>>(0);
            builder.CloseComponent();
            builder.OpenComponent<PropertyColumn<TestDataItem, int>>(1);
            builder.AddAttribute(2, nameof(PropertyColumn<TestDataItem, int>.Property), (Expression<Func<TestDataItem, int>>)(x => x.Id));
            builder.CloseComponent();
        };

        [Test]
        public void SelectColumn_RowCheckbox_ShouldBeDisabled_WhenDisabledFuncReturnsTrue()
        {
            var items = new List<TestDataItem> { new() { Id = 1, Name = "Item 1", ShouldBeDisabled = true } };

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.MultiSelection, true)
                .Add(p => p.Columns, SelectColumnWithFunc)
            );

            // Find the checkbox input element for the row
            var checkbox = comp.Find("td.mud-table-cell .mud-checkbox input");
            checkbox.Should().NotBeNull();
            checkbox.HasAttribute("disabled").Should().BeTrue();
        }

        [Test]
        public void SelectColumn_RowCheckbox_ShouldBeEnabled_WhenDisabledFuncReturnsFalse()
        {
            var items = new List<TestDataItem> { new() { Id = 1, Name = "Item 1", ShouldBeDisabled = false } };

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.Columns, SelectColumnWithFunc)
            );

            var checkbox = comp.Find("td.mud-table-cell .mud-checkbox input");
            checkbox.Should().NotBeNull();
            checkbox.HasAttribute("disabled").Should().BeFalse();
        }

        [Test]
        public void SelectColumn_RowCheckbox_ShouldBeEnabled_WhenNoDisabledFuncIsProvided()
        {
            var items = new List<TestDataItem> { new() { Id = 1, Name = "Item 1" } };

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.Columns, SelectColumnNoFunc)
            );

            var checkbox = comp.Find("td.mud-table-cell .mud-checkbox input");
            checkbox.Should().NotBeNull();
            checkbox.HasAttribute("disabled").Should().BeFalse();
        }

        [Test]
        public void SelectColumn_HeaderCheckbox_ShouldNotRender_WhenMultiSelectionIsFalse()
        {
            var items = new List<TestDataItem> { new() { Id = 1, Name = "Item 1" } };

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.MultiSelection, false) // Explicitly set MultiSelection to false
                .Add(p => p.Columns, SelectColumnNoFunc)
            );

            // Check if the header checkbox is rendered
            var headerCheckbox = comp.FindAll("th.mud-table-cell .mud-checkbox input");
            headerCheckbox.Should().BeEmpty();
        }

        [Test]
        public void SelectColumn_HeaderCheckbox_ShouldRender_WhenMultiSelectionIsTrue()
        {
            var items = new List<TestDataItem> { new TestDataItem { Id = 1, Name = "Item 1" } };

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.MultiSelection, true) // Explicitly set MultiSelection to true
                .Add(p => p.Columns, SelectColumnNoFunc)
            );

            var headerCheckbox = comp.Find("th.mud-table-cell .mud-checkbox input");
            headerCheckbox.Should().NotBeNull();
        }

        [Test]
        public async Task SelectOnRowClick_IgnoresDisabledRows()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem { Id = 1, Name = "Enabled Item 1", ShouldBeDisabled = false },
                new TestDataItem { Id = 2, Name = "Disabled Item 1", ShouldBeDisabled = true },
                new TestDataItem { Id = 3, Name = "Enabled Item 2", ShouldBeDisabled = false }
            };
            Func<TestDataItem, bool> disabledFunc = item => item.ShouldBeDisabled;

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.SelectOnRowClick, true)
                .Add(p => p.MultiSelection, true) // Enable multi-selection to check SelectedItems
                .Add(p => p.Columns, SelectColumnWithFunc)
            );

            // Simulate click on the disabled row (row index 1 for "Disabled Item 1")
            var rows = comp.FindAll("tbody tr");
            await rows[1].ClickAsync(); // Click on the row of "Disabled Item 1"

            comp.Instance.GetState(x => x.SelectedItems).Should().NotContain(items[1]); // Disabled item should not be selected
            comp.Instance.GetState(x => x.SelectedItems).Should().BeEmpty(); // Or be the previously selected item if any, but not items[1]

            // Simulate click on an enabled row (row index 0 for "Enabled Item 1")
            rows = comp.FindAll("tbody tr");
            await rows[0].ClickAsync(); // Click on the row of "Enabled Item 1"

            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[0]);
            comp.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);

            // Further check: click another enabled item to ensure multi-selection works for enabled items
            // and that the disabled item is still not selected.
            rows = comp.FindAll("tbody tr");
            await rows[2].ClickAsync(); // Click on the row of "Enabled Item 2"
            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[0]);
            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[2]);
            comp.Instance.GetState(x => x.SelectedItems).Should().NotContain(items[1]);
        }

        [Test]
        public async Task SelectAll_IgnoresDisabledRows()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem { Id = 1, Name = "Enabled Item 1", ShouldBeDisabled = false },
                new TestDataItem { Id = 2, Name = "Disabled Item 1", ShouldBeDisabled = true },
                new TestDataItem { Id = 3, Name = "Enabled Item 2", ShouldBeDisabled = false },
                new TestDataItem { Id = 4, Name = "Disabled Item 2", ShouldBeDisabled = true }
            };
            Func<TestDataItem, bool> disabledFunc = item => item.ShouldBeDisabled;

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.MultiSelection, true)
                .Add(p => p.Columns, SelectColumnWithFunc)
            );

            await comp.Instance.SetSelectAllAsync(true);

            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[0]); // Enabled Item 1
            comp.Instance.GetState(x => x.SelectedItems).Should().NotContain(items[1]); // Disabled Item 1
            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[2]); // Enabled Item 2
            comp.Instance.GetState(x => x.SelectedItems).Should().NotContain(items[3]); // Disabled Item 2
            comp.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
        }

        [Test]
        public async System.Threading.Tasks.Task SelectAll_WithNoDisabledFunc_StillWorks()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem { Id = 1, Name = "Item 1" },
                new TestDataItem { Id = 2, Name = "Item 2" }
            };

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.MultiSelection, true)
                .Add(p => p.Columns, SelectColumnNoFunc)
            );

            await comp.Instance.SetSelectAllAsync(true);

            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[0]);
            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[1]);
            comp.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
        }

        [Test]
        public async Task SelectAll_WithDisabledFunc_ReturningAllFalse_StillWorks()
        {
            var items = new List<TestDataItem>
            {
                new TestDataItem { Id = 1, Name = "Item 1", ShouldBeDisabled = false },
                new TestDataItem { Id = 2, Name = "Item 2", ShouldBeDisabled = false }
            };
            Func<TestDataItem, bool> disabledFunc = item => false; // All items are effectively enabled

            var comp = Context.Render<MudDataGrid<TestDataItem>>(parameters => parameters
                .Add(p => p.Items, items)
                .Add(p => p.MultiSelection, true)
                .Add(p => p.Columns, SelectColumnWithFunc)
            );

            await comp.Instance.SetSelectAllAsync(true);

            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[0]);
            comp.Instance.GetState(x => x.SelectedItems).Should().Contain(items[1]);
            comp.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
        }

        [Test]
        public void DataGridRowDetailInitiallyExpandedMultiple()
        {
            // just setting Items
            var comp = Context.Render<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            var item = dataGrid.Instance.Items.FirstOrDefault(x => x.Name == "Ira");

            dataGrid.Instance._openHierarchies.Should().Contain(item);

            item = dataGrid.Instance.Items.FirstOrDefault(x => x.Name == "Anders");

            dataGrid.Instance._openHierarchies.Should().Contain(item);

            comp.Markup.Should().Contain("uid = Ira|27|Success|");
            comp.Markup.Should().Contain("uid = Anders|24|Error|");

            comp.Markup.Should().NotContain("uid = Sam|56|Normal|");
            comp.Markup.Should().NotContain("uid = Alicia|54|Info|");
            comp.Markup.Should().NotContain("uid = John|32|Warning|");
        }

        [Test]
        public void DataGridRowDetailInitiallyExpandedObservableMultiple()
        {
            // updating an observable collection of items after initial load
            var comp = Context.Render<DataGridHierarchyInitiallyExpandedItemsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyInitiallyExpandedItemsTest.Model>>();

            var item = dataGrid.Instance.Items.FirstOrDefault(x => x.Name == "Ira");

            dataGrid.Instance._openHierarchies.Should().Contain(item);

            item = dataGrid.Instance.Items.FirstOrDefault(x => x.Name == "Anders");

            dataGrid.Instance._openHierarchies.Should().Contain(item);

            comp.Markup.Should().Contain("uid = Ira|27|Success|");
            comp.Markup.Should().Contain("uid = Anders|24|Error|");

            comp.Markup.Should().NotContain("uid = Sam|56|Normal|");
            comp.Markup.Should().NotContain("uid = Alicia|54|Info|");
            comp.Markup.Should().NotContain("uid = John|32|Warning|");
        }

        [Test]
        public async Task DataGridRowDetailInitiallyExpandedServerMultiple()
        {
            var comp = Context.Render<DataGridHierarchyInitiallyExpandedServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyInitiallyExpandedServerDataTest.Model>>();

            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("uid = Ira|27|Success|"));
            comp.Markup.Should().Contain("uid = Anders|24|Error|");

            comp.Markup.Should().NotContain("uid = Sam|56|Normal|");
            comp.Markup.Should().NotContain("uid = Alicia|54|Info|");
            comp.Markup.Should().NotContain("uid = John|32|Warning|");

            // Collapse Ira
            await comp.InvokeAsync(async () =>
            {
                var iraIndex = comp.FindAll("tr")
                    .Select((row, index) => new { row, index })
                    .First(r => r.row.InnerHtml.Contains("uid = Ira")).index;

                iraIndex.Should().BeGreaterThan(0, "Expected a row above the Ira detail row");

                var toggleButton = comp.FindAll("tr")[iraIndex - 2].QuerySelector("button");
                toggleButton.Should().NotBeNull("Expected a toggle button above the Ira detail row");
                await toggleButton.ClickAsync();
            });

            // Go to next page
            await comp.InvokeAsync(async () =>
            {
                var nextButton = comp.Find("button[aria-label='Next page']");
                nextButton.Should().NotBeNull("Expected a Next Page Button.");
                await nextButton.ClickAsync();
            });

            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("uid = ScarletKuro|27|Success|"));

            comp.Markup.Should().NotContain("uid = Versile2|24|Error|");
            comp.Markup.Should().NotContain("uid = Anu6is|56|Normal|");
            comp.Markup.Should().NotContain("uid = Garderoben|32|Warning|");
            comp.Markup.Should().NotContain("uid = Henon|54|Info|");

            // Go back to previous page
            await comp.InvokeAsync(async () =>
            {
                var prevButton = comp.Find("button[aria-label='Previous page']");
                prevButton.Should().NotBeNull("Expected a Previous Page Button.");
                await prevButton.ClickAsync();
            });

            await comp.WaitForAssertionAsync(() => comp.Markup.Should().Contain("uid = Anders|24|Error|"));

            comp.Markup.Should().NotContain("uid = Ira|27|Success|");
            comp.Markup.Should().NotContain("uid = Sam|56|Normal|");
            comp.Markup.Should().NotContain("uid = Alicia|54|Info|");
            comp.Markup.Should().NotContain("uid = John|32|Warning|");
        }

        [Test]
        public async Task DataGridShouldAllowUnsortedAscDescOnly()
        {
            var comp = Context.Render<DataGridAllowUnsortedTest>(parameters => parameters
                .Add(p => p.AllowUnsorted, false));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridAllowUnsortedTest.Item>>();
            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridAllowUnsortedTest.Item>>()[0];

            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Ascending);
            var cells = dataGrid.FindAll("td");
            cells[0].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[6].TextContent.Should().Be("C");

            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Descending);
            cells = dataGrid.FindAll("td");
            cells[0].TextContent.Should().Be("C");
            cells[3].TextContent.Should().Be("B");
            cells[6].TextContent.Should().Be("A");

            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Ascending);
            cells = dataGrid.FindAll("td");
            cells[0].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[6].TextContent.Should().Be("C");

            comp = Context.Render<DataGridAllowUnsortedTest>(parameters => parameters
                .Add(p => p.AllowUnsorted, true));
            dataGrid = comp.FindComponent<MudDataGrid<DataGridAllowUnsortedTest.Item>>();
            headerCell = dataGrid.FindComponents<HeaderCell<DataGridAllowUnsortedTest.Item>>()[0];

            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Ascending);
            cells = dataGrid.FindAll("td");
            cells[0].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[6].TextContent.Should().Be("C");

            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Descending);
            cells = dataGrid.FindAll("td");
            cells[0].TextContent.Should().Be("C");
            cells[3].TextContent.Should().Be("B");
            cells[6].TextContent.Should().Be("A");

            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.None);
            cells = dataGrid.FindAll("td");
            cells[0].TextContent.Should().Be("C");
            cells[3].TextContent.Should().Be("A");
            cells[6].TextContent.Should().Be("B");
        }

        [Test]
        public async Task DataGrid_HierarchyVisibilityToggled_SingleRowToggle()
        {
            var comp = Context.Render<DataGridHierarchyVisibilityToggledTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyVisibilityToggledTest.Model>>();
            var testComponent = comp.Instance;

            await comp.InvokeAsync(() => dataGrid.Instance
                .ToggleHierarchyVisibilityAsync(dataGrid.Instance.Items.First()));

            testComponent.ToggledEvents.Should().HaveCount(1);
            testComponent.ToggledEvents[0].Item.Name.Should().Be("John");
            testComponent.ToggledEvents[0].Expanded.Should().BeTrue();

            await comp.InvokeAsync(() => dataGrid.Instance
                .ToggleHierarchyVisibilityAsync(dataGrid.Instance.Items.First()));

            testComponent.ToggledEvents.Should().HaveCount(2);
            testComponent.ToggledEvents[1].Item.Name.Should().Be("John");
            testComponent.ToggledEvents[1].Expanded.Should().BeFalse();
        }

        [Test]
        public async Task DataGrid_HierarchyVisibilityToggled_CollapseAll()
        {
            var comp = Context.Render<DataGridHierarchyVisibilityToggledTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyVisibilityToggledTest.Model>>();
            var testComponent = comp.Instance;

            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllHierarchy());
            testComponent.ToggledEvents.Clear();

            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllHierarchy());

            testComponent.ToggledEvents.Should().HaveCount(3);
            testComponent.ToggledEvents.Select(x => x.Item.Name).Should().BeEquivalentTo(["John", "Jane", "Bob"]);
        }

        [Test]
        public async Task DataGrid_HierarchyVisibilityToggled_ExpandAll()
        {
            var comp = Context.Render<DataGridHierarchyVisibilityToggledTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyVisibilityToggledTest.Model>>();
            var testComponent = comp.Instance;

            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllHierarchy());

            testComponent.ToggledEvents.Should().HaveCount(3);
            testComponent.ToggledEvents.Should().OnlyContain(x => x.Expanded == true);
            testComponent.ToggledEvents.Select(x => x.Item.Name).Should().BeEquivalentTo(["John", "Jane", "Bob"]);
        }

        [Test]
        public async Task DataGridFilterIcons()
        {
            var comp = Context.Render<DataGridFilterIconsTest>();
            MudIconButton FirstFilterButton() =>
                comp.FindComponents<MudIconButton>().FirstOrDefault(x => x.Markup.Contains("filter-button"))?.Instance;

            // Check filter buttons when no filter applied
            var mudIconButton = FirstFilterButton();
            mudIconButton.Icon.Should().Be(Icons.Material.Filled.Battery0Bar);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FilterMode, DataGridFilterMode.ColumnFilterMenu));

            mudIconButton = FirstFilterButton();
            mudIconButton.Icon.Should().Be(Icons.Material.Filled.Battery0Bar);

            // Check filter buttons when filter applied
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FilterMode, DataGridFilterMode.Simple));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterIconsTest.Model>>();
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterIconsTest.Model>
            {
                Column = dataGrid.Instance.RenderedColumns.First(),
                Operator = FilterOperator.String.Contains,
                Value = "Sam"
            }));

            mudIconButton = FirstFilterButton();
            mudIconButton.Icon.Should().Be(Icons.Material.Filled.BatteryFull);

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FilterMode, DataGridFilterMode.ColumnFilterMenu));

            mudIconButton = FirstFilterButton();
            mudIconButton.Icon.Should().Be(Icons.Material.Filled.BatteryFull);

            // Check filter buttons when FilterMode is ColumnFilterRow
            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.FilterMode, DataGridFilterMode.ColumnFilterRow));

            var mudMenu = comp.FindComponents<MudMenu>().FirstOrDefault(x => x.Markup.Contains("column-filter-menu"))?.Instance;
            mudMenu.Icon.Should().Be(Icons.Material.Filled.BatteryFull);

            mudIconButton = FirstFilterButton();
            mudIconButton.Icon.Should().Be(Icons.Material.Filled.BatteryAlert);
        }

        #region Selection Cleanup Tests (ObservableCollection)

        [Test]
        public async Task DataGrid_SelectedItems_ShouldUpdateWhenSingleItemRemoved()
        {
            var comp = Context.Render<DataGridSelectionCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            var firstItem = testComponent.Items.First();
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
            });

            await comp.InvokeAsync(() => testComponent.RemoveItem(firstItem));

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0));
            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(3));
        }

        [Test]
        public async Task DataGrid_SelectedItems_ShouldUpdateWhenMultipleItemsRemoved()
        {
            var comp = Context.Render<DataGridSelectionCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectAllAsync(true));
            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(4));

            var firstItem = testComponent.Items.First();
            var secondItem = testComponent.Items.Skip(1).First();

            await comp.InvokeAsync(() =>
            {
                testComponent.RemoveItem(firstItem);
                testComponent.RemoveItem(secondItem);
            });

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().NotContain(firstItem);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().NotContain(secondItem);
            });

            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(2));
        }

        [Test]
        public async Task DataGrid_SelectedItems_ShouldClearWhenCollectionCleared()
        {
            var comp = Context.Render<DataGridSelectionCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectAllAsync(true));
            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(4));

            await comp.InvokeAsync(() => testComponent.ClearItems());

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0));
            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(0));
        }

        [Test]
        public async Task DataGrid_SelectedItem_ShouldUpdateWhenItemRemoved()
        {
            var comp = Context.Render<DataGridHierarchyCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            var firstItem = testComponent.Items.First();
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
            });

            await comp.InvokeAsync(() => testComponent.RemoveItem(firstItem));

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Should().BeEmpty());
            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0));
        }

        [Test]
        public async Task DataGrid_SelectedItems_ShouldNotAffectNonSelectedItems()
        {
            var comp = Context.Render<DataGridSelectionCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            var firstItem = testComponent.Items.First();
            var secondItem = testComponent.Items.Skip(1).First();

            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, secondItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(secondItem);
            });

            var thirdItem = testComponent.Items.Skip(2).First();
            await comp.InvokeAsync(() => testComponent.RemoveItem(thirdItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(secondItem);
            });

            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(3));
        }

        [Test]
        public async Task DataGrid_SelectedItem_ShouldNotReferenceRemovedItem_WhenMultipleItemsSelectedAndOneRemoved()
        {
            var comp = Context.Render<DataGridSelectionCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            var firstItem = testComponent.Items.First();
            var secondItem = testComponent.Items.Skip(1).First();

            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, secondItem));

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2));

            await comp.InvokeAsync(() => testComponent.RemoveItem(secondItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().NotContain(secondItem);
            });

            // SelectedItem must never reference a removed item.
            // It should either be null or point to an item still in the Items collection.
            await comp.WaitForAssertionAsync(() =>
            {
                var selectedItem = dataGrid.Instance.GetState(x => x.SelectedItem);
                if (selectedItem != null)
                {
                    testComponent.Items.Should().Contain(selectedItem,
                        "SelectedItem must reference an item still in the collection");
                }
            });
        }

        [Test]
        public async Task DataGrid_SelectedItems_ShouldKeepRemainingSelectionsWhenOneRemoved()
        {
            var comp = Context.Render<DataGridSelectionCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            // Select 3 items (not using SelectAll)
            var firstItem = testComponent.Items.ElementAt(0);
            var secondItem = testComponent.Items.ElementAt(1);
            var thirdItem = testComponent.Items.ElementAt(2);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, secondItem));
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, thirdItem));

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(3));

            await comp.InvokeAsync(() => testComponent.RemoveItem(secondItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(2);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(thirdItem);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().NotContain(secondItem);
            });

            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(3));
        }

        #endregion

        #region Selection Cleanup Tests (List)

        [Test]
        public async Task DataGrid_SelectedItems_ShouldUpdateWhenItemRemovedFromList()
        {
            var comp = Context.Render<DataGridSelectionCleanupListTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupListTest.Model>>();
            var testComponent = comp.Instance;

            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4));
            dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0);

            var firstItem = testComponent.Items.First();
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
            });

            // Remove item by reassigning the Items list (not ObservableCollection)
            await comp.InvokeAsync(() => testComponent.RemoveItem(firstItem));

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0));
            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(3));
        }

        [Test]
        public async Task DataGrid_SelectedItems_ShouldClearWhenListCleared()
        {
            var comp = Context.Render<DataGridSelectionCleanupListTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupListTest.Model>>();
            var testComponent = comp.Instance;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectAllAsync(true));
            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(4));

            // Clear by reassigning to empty list
            await comp.InvokeAsync(() => testComponent.ClearItems());

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0));
            await comp.WaitForAssertionAsync(() =>
                dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(0));
        }

        [Test]
        public async Task DataGrid_SelectedItem_ShouldUpdateWhenItemRemovedFromList()
        {
            var comp = Context.Render<DataGridSelectionCleanupListTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionCleanupListTest.Model>>();
            var testComponent = comp.Instance;

            var firstItem = testComponent.Items.First();
            await comp.InvokeAsync(() => dataGrid.Instance.SetSelectedItemAsync(true, firstItem));

            await comp.WaitForAssertionAsync(() =>
            {
                dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(1);
                dataGrid.Instance.GetState(x => x.SelectedItems).Should().Contain(firstItem);
            });

            // Remove by reassigning filtered list
            await comp.InvokeAsync(() => testComponent.RemoveItem(firstItem));

            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Should().BeEmpty());
            await comp.WaitForAssertionAsync(() => dataGrid.Instance.GetState(x => x.SelectedItems).Count.Should().Be(0));
        }

        #endregion

        #region Hierarchy Cleanup Tests (ObservableCollection)

        [Test]
        public async Task DataGrid_OpenHierarchies_ShouldClearWhenExpandedItemRemovedFromObservableCollection()
        {
            var comp = Context.Render<DataGridHierarchyCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            // Ira should be initially expanded
            var iraItem = testComponent.Items.First(x => x.Name == "Ira");

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Should().Contain(iraItem));

            // Remove the expanded item
            await comp.InvokeAsync(() => testComponent.RemoveItem(iraItem));

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Should().NotContain(iraItem));
        }

        [Test]
        public async Task DataGrid_OpenHierarchies_ShouldClearWhenManuallyExpandedItemRemovedFromObservableCollection()
        {
            var comp = Context.Render<DataGridHierarchyCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            var samItem = testComponent.Items.First(x => x.Name == "Sam");

            // Manually expand Sam
            await comp.InvokeAsync(() => dataGrid.Instance.ToggleHierarchyVisibilityAsync(samItem));

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Should().Contain(samItem));

            // Remove Sam
            await comp.InvokeAsync(() => testComponent.RemoveItem(samItem));

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Should().NotContain(samItem));
        }

        [Test]
        public async Task DataGrid_OpenHierarchies_ShouldClearWhenObservableCollectionCleared()
        {
            var comp = Context.Render<DataGridHierarchyCleanupObservableCollectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyCleanupObservableCollectionTest.Model>>();
            var testComponent = comp.Instance;

            // Ira should be initially expanded
            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Count.Should().BeGreaterThan(0));

            // Clear all items
            await comp.InvokeAsync(() => testComponent.ClearItems());

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Count.Should().Be(0));
        }

        #endregion

        #region Hierarchy Cleanup Tests (List)

        [Test]
        public async Task DataGrid_OpenHierarchies_ShouldClearWhenExpandedItemRemovedFromList()
        {
            var comp = Context.Render<DataGridHierarchyCleanupListTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyCleanupListTest.Model>>();
            var testComponent = comp.Instance;

            // Ira should be initially expanded
            var iraItem = testComponent.Items.First(x => x.Name == "Ira");

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Should().Contain(iraItem));

            // Remove the expanded item by reassigning the list
            await comp.InvokeAsync(() => testComponent.RemoveItem(iraItem));

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Should().NotContain(iraItem));
        }

        [Test]
        public async Task DataGrid_OpenHierarchies_ShouldClearWhenListCleared()
        {
            var comp = Context.Render<DataGridHierarchyCleanupListTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyCleanupListTest.Model>>();
            var testComponent = comp.Instance;

            // Ira should be initially expanded
            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Count.Should().BeGreaterThan(0));

            // Clear all items by reassigning to empty list
            await comp.InvokeAsync(() => testComponent.ClearItems());

            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance._openHierarchies.Count.Should().Be(0));
        }

        [Test]
        public async Task DataGridFilterDefinitionsPreloadTest()
        {
            var comp = Context.Render<DataGridFilterDefinitionsPreloadTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterDefinitionsPreloadTest.Model>>();

            // Wait for the filter to be applied after OnAfterRenderAsync
            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance.FilterDefinitions.Count.Should().Be(1));

            // Check that the filter definition has the correct column and value
            var filterDef = dataGrid.Instance.FilterDefinitions.First();
            filterDef.Column.Should().NotBeNull("Filter definition should have a column reference");
            filterDef.Value.Should().Be("Sam", "Filter definition should have the correct value");

            // Get the Name column
            var nameColumn = dataGrid.Instance.GetColumnByPropertyName("Name");
            nameColumn.Should().NotBeNull("Name column should exist");

            // Verify that the filter's column matches the name column
            filterDef.Column.Should().BeSameAs(nameColumn, "Filter definition's column should be the same instance as the Name column");

            // Access FilterContext to verify it picks up the existing filter
            var filterContext = nameColumn!.FilterContext;
            filterContext.Should().NotBeNull("FilterContext should not be null");
            filterContext.FilterDefinition.Should().NotBeNull("FilterContext.FilterDefinition should not be null");

            // This is the key test - the FilterContext should have found the existing filter definition
            filterContext.FilterDefinition.Should().BeSameAs(filterDef, "FilterContext should reference the same filter definition that was added");
            filterContext.FilterDefinition!.Value.Should().Be("Sam", "FilterContext's filter definition should have the correct value");
            filterContext.FilterDefinition.Operator.Should().Be(FilterOperator.String.Contains);
        }

        [Test]
        public async Task DataGridFilterDefinitionsPreloadColumnFilterRowTest()
        {
            var comp = Context.Render<DataGridFilterDefinitionsPreloadColumnFilterRowTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterDefinitionsPreloadColumnFilterRowTest.Model>>();

            // Wait for the filter to be applied after OnAfterRenderAsync
            await comp.WaitForAssertionAsync(() =>
                dataGrid.Instance.FilterDefinitions.Count.Should().Be(1));

            // Check that the filter definition has the correct column and value
            var filterDef = dataGrid.Instance.FilterDefinitions.First();
            filterDef.Column.Should().NotBeNull("Filter definition should have a column reference");
            filterDef.Value.Should().Be("C", "Filter definition should have the correct value as reported in issue #8060");

            // Get the Name column
            var nameColumn = dataGrid.Instance.GetColumnByPropertyName("Name");
            nameColumn.Should().NotBeNull("Name column should exist");

            // Verify that the filter's column matches the name column
            filterDef.Column.Should().BeSameAs(nameColumn, "Filter definition's column should be the same instance as the Name column");

            // Access FilterContext to verify it picks up the existing filter - this is what was broken in #8060
            var filterContext = nameColumn!.FilterContext;
            filterContext.Should().NotBeNull("FilterContext should not be null");
            filterContext.FilterDefinition.Should().NotBeNull("FilterContext.FilterDefinition should not be null");

            // The key fix - the FilterContext should reference the programmatically added filter
            filterContext.FilterDefinition.Should().BeSameAs(filterDef, "FilterContext should reference the same filter definition that was added");
            filterContext.FilterDefinition!.Value.Should().Be("C", "FilterContext should show the filter value 'C' in the UI");
            filterContext.FilterDefinition.Operator.Should().Be(FilterOperator.String.Contains, "FilterContext should show the correct operator");

            // Verify the user can now modify the filter
            filterContext.FilterDefinition.Value = "A";
            filterContext.FilterDefinition.Value.Should().Be("A", "User should be able to modify the filter value");

            // Verify the filter can be removed
            await dataGrid.InvokeAsync(() => dataGrid.Instance.ClearFiltersAsync());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0, "Filter should be removable");
        }

        #endregion
    }
}
