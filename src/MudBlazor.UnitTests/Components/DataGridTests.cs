#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
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

            // Check the values of rows - should not be sorted and should be in the original order.
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");

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
                Operator = "equals",
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
        }

        [Test]
        public async Task DataGridPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");
        }

        [Test]
        public async Task DataGridInlineEditTest()
        {
            var comp = Context.RenderComponent<DataGridInlineEditTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridInlineEditTest.Item>>();

            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[4].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll(".mud-table-body tr")[0].Click();
            dataGrid.FindAll(".mud-table-body tr input")[0].Change("Z");
            dataGrid.FindAll(".mud-table-body tr td:nth-child(2) button")[0].Click();
            dataGrid.FindAll(".mud-table-body tr td")[0].TextContent.Trim().Should().Be("Z");

            var name = dataGrid.Instance.Items.First().Name;
            name.Should().Be("Z");
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
            dataGrid.Instance.SelectedItemsChanged.HasDelegate.Should().Be(true);
            dataGrid.Instance.StartedEditingItem.HasDelegate.Should().Be(true);
            dataGrid.Instance.StartedCommittingItemChanges.HasDelegate.Should().Be(true);
            dataGrid.Instance.EditingItemCancelled.HasDelegate.Should().Be(true);

            // Set some parameters manually so that they are covered.
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.MultiSelection), true));
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ReadOnly), false));
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.EditMode), DataGridEditMode.Inline));
            dataGrid.SetParametersAndRender(parameters.ToArray());

            // Make sure that the callbacks have not been fired yet.
            comp.Instance.RowClicked.Should().Be(false);
            comp.Instance.SelectedItemChanged.Should().Be(false);
            comp.Instance.SelectedItemsChanged.Should().Be(false);
            comp.Instance.StartedEditingItem.Should().Be(false);
            comp.Instance.StartedCommittingItemChanges.Should().Be(false);
            comp.Instance.EditingItemCancelled.Should().Be(false);

            // Fire RowClick, SelectedItemChanged, SelectedItemsChanged, and StartedEditingItem callbacks.
            dataGrid.FindAll(".mud-table-body tr")[0].Click();

            Console.WriteLine(dataGrid.Markup);
            // Fire StartedCommittingItemChanges callback.
            dataGrid.FindAll(".mud-table-body tr td button")[0].Click();
            // Go into edit mode once again. 
            dataGrid.FindAll(".mud-table-body tr")[0].Click();
            // Fire EditingItemCancelled callback.
            dataGrid.FindAll(".mud-table-body tr td button")[1].Click();

            // Make sure that the callbacks have been fired.
            comp.Instance.RowClicked.Should().Be(true);
            comp.Instance.SelectedItemChanged.Should().Be(true);
            comp.Instance.SelectedItemsChanged.Should().Be(true);
            comp.Instance.StartedEditingItem.Should().Be(true);
            comp.Instance.StartedCommittingItemChanges.Should().Be(true);
            comp.Instance.EditingItemCancelled.Should().Be(true);
        }
    }
}
