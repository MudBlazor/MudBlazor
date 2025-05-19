// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.DataGrid;
using NUnit.Framework;

#nullable enable
namespace MudBlazor.UnitTests.Components
{
    public class DataGridGroupingTests : BunitTest
    {
        [Test]
        public async Task DataGridGroupExpandedTrueTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedTest.Fruit>>();
            // until a change happens this bool tracks whether GroupExpanded is applied.
            dataGrid.Instance._groupInitialExpanded = true;
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(7);
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroupsAsync());
            // collapsing group rows counts
            dataGrid.Instance._groupInitialExpanded = false;
            dataGrid.Render();
            // after all groups are collapsed
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should not be expanded with the new category since CollapseAll collapsed it (Even if it was empty)
            dataGrid.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridGroupExpandedTrueAsyncTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedAsyncTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedAsyncTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroupsAsync());
            dataGrid.Render();
            // after all groups are collapsed
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should not be expanded with the new category since CollapseAll collapsed it (Even if it was empty)
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(3));
        }

        [Test]
        public async Task DataGridGroupExpandedTrueServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedServerDataTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroupsAsync());
            dataGrid.Render();
            // after all groups are collapsed
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() => comp.Instance.AddFruit());
            // datagrid should not be expanded with the new category since CollapseAll collapsed it (Even if it was empty)
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(3));
        }

        [Test]
        public async Task DataGridGroupExpandedFalseTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedFalseTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedFalseTest.Fruit>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            dataGrid.Render();
            // after all groups are expanded
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(7);
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should not be collapsed with the new category since ExpandAll expanded it (Even if it was empty)
            dataGrid.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(10);
        }

        [Test]
        public async Task DataGridGroupExpandedFalseAsyncTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedFalseAsyncTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedFalseAsyncTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            dataGrid.Render();
            // after all groups are expanded
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should not be collapsed with the new category since ExpandAll expanded it (Even if it was empty)
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(10));
        }

        [Test]
        public async Task DataGridGroupExpandedFalseServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedFalseServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedFalseServerDataTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            dataGrid.Render();
            // after all groups are expanded
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() => comp.Instance.AddFruit());
            // datagrid should not be collapsed with the new category since ExpandAll expanded it (Even if it was empty)
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(10));
        }

        [Test]
        public async Task DataGridGroupCollapseAllTest()
        {
            var comp = Context.RenderComponent<DataGridGroupCollapseAllTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupCollapseAllTest.TestObject>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(15);
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroupsAsync());
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
            comp.Instance.RefreshList();
            comp.Render();
            // after all groups are expanded
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridGroupExpandAllCollapseAllTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandAllCollapseAllTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandAllCollapseAllTest.Element>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(14);
            await dataGrid.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.First));
            await dataGrid.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Next));
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(18);
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroupsAsync());
            await dataGrid.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.First));
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            comp.Instance.RefreshList();
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
        }


        [Test]
        public void ShouldSetIsGenderGroupedToTrueWhenGroupingIsApplied()
        {
            // Render the DataGridGroupingTest component for testing.
            var comp = Context.RenderComponent<DataGridColumnGroupingTest>();

            // Attempt to find the MudPopoverProvider component within the rendered component.
            // MudPopoverProvider is used to manage popovers in the component, including the grouping popover.
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            // Assert that initially, before any user interaction, IsGenderGrouped should be false.
            comp.Instance.IsGenderGrouped.Should().Be(false);

            // Find the button within the 'th' element with class 'gender' that triggers the popover for grouping.
            var genderHeaderOption = comp.Find("th.gender .mud-menu button");

            // Simulate a click on the gender header group button to open the popover with grouping options.
            genderHeaderOption.Click();

            // Find all MudListItem components within the popoverProvider.
            // These list items represent the individual options within the grouping popover.
            var listItems = popoverProvider.FindComponents<MudMenuItem>();

            // Assert that there are exactly 2 list items (options) available in the popover.
            listItems.Count.Should().Be(2);

            // From the list items found, select the second one which is expected to be the clickable option for grouping.
            var clickablePopover = listItems[1].Find(".mud-menu-item");

            // click on the grouping option to apply grouping to the data grid.
            clickablePopover.Click();

            // After clicking the grouping option, assert that IsGenderGrouped is now true, indicating that
            // the action of applying grouping has successfully updated the component's state.
            comp.Instance.IsGenderGrouped.Should().Be(true);
        }

        [Test]
        [SetCulture("en-US")]
        public void DataGridServerGroupUngroupingTest()
        {
            var comp = Context.RenderComponent<DataGridServerDataColumnGroupingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<Examples.Data.Models.Element>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            //ungroup Category
            var headerCategory = comp.Find("th.group .mud-menu button");
            headerCategory.Click();
            var catListItems = popoverProvider.FindComponents<MudMenuItem>();
            catListItems.Count.Should().Be(4);
            var clickableCategoryUngroup = catListItems[3].Find(".mud-menu-item");
            clickableCategoryUngroup.Click();

            //click name grouping in grid
            var headerOption = comp.Find("th.name .mud-menu button");
            headerOption.Click();
            var listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(4);
            var clickablePopover = listItems[3].Find(".mud-menu-item");
            clickablePopover.Click();
            var cells = dataGrid.FindAll("td");

            //checking cell content is the most reliable way to verify grouping
            cells[0].TextContent.Should().Be("Name: Hydrogen");
            cells[1].TextContent.Should().Be("Name: Helium");
            cells[2].TextContent.Should().Be("Name: Lithium");
            cells[3].TextContent.Should().Be("Name: Beryllium");
            cells[4].TextContent.Should().Be("Name: Boron");
            cells[5].TextContent.Should().Be("Name: Carbon");
            cells[6].TextContent.Should().Be("Name: Nitrogen");
            cells[7].TextContent.Should().Be("Name: Oxygen");
            cells[8].TextContent.Should().Be("Name: Fluorine");
            cells[9].TextContent.Should().Be("Name: Neon");
            dataGrid.Instance.IsGrouped.Should().BeTrue();

            //click name ungrouping in grid
            headerOption = comp.Find("th.name .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(4);
            clickablePopover = listItems[3].Find(".mud-menu-item");
            clickablePopover.Click();
            cells = dataGrid.FindAll("td");
            // We do not need check all 10 rows as it's clear that it's ungrouped if first row pass
            cells[0].TextContent.Should().Be("1");
            cells[1].TextContent.Should().Be("H");
            cells[2].TextContent.Should().Be("Hydrogen");
            cells[3].TextContent.Should().Be("0");
            cells[4].TextContent.Should().Be("1.00794");
            cells[5].TextContent.Should().Be("Other");
            dataGrid.Instance.IsGrouped.Should().BeFalse();
        }

        [Test]
        public void DataGridGroupingTestBoundAndUnboundScenarios()
        {
            var comp = Context.RenderComponent<DataGridColumnGroupingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnGroupingTest.Model>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            IRefreshableElementCollection<IElement> Rows() => dataGrid.FindAll("tr");
            IRefreshableElementCollection<IElement> Cells() => dataGrid.FindAll("td");

            // Assert that initially, before any user interaction, IsGenderGrouped and IsAgeGrouped should be false
            // The default grouping is by name
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);

            var nameCells = Cells();
            nameCells.Count.Should().Be(4, because: "4 data rows");
            nameCells[0].TextContent.Should().Be("Name: John");
            nameCells[1].TextContent.Should().Be("Name: Johanna");
            nameCells[2].TextContent.Should().Be("Name: Steve");
            nameCells[3].TextContent.Should().Be("Name: Alice");
            Rows().Count.Should().Be(6, because: "1 header row + 4 data rows + 1 footer row");

            var ageGrouping = comp.Find(".GroupByAge");
            ageGrouping.Click();
            comp.Instance.IsAgeGrouped.Should().Be(true);
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            var ageCells = Cells();
            ageCells.Count.Should().Be(3, because: "3 data rows");
            ageCells[0].TextContent.Should().Be("Age: 45");
            ageCells[1].TextContent.Should().Be("Age: 23");
            ageCells[2].TextContent.Should().Be("Age: 32");
            Rows().Count.Should().Be(5, because: "1 header row + 3 data rows + 1 footer row");

            var genderGrouping = comp.Find(".GroupByGender");
            genderGrouping.Click();
            comp.Instance.IsGenderGrouped.Should().Be(true);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            var genderCells = Cells();
            genderCells.Count.Should().Be(2, because: "2 data rows");
            genderCells[0].TextContent.Should().Be("Gender: Male");
            genderCells[1].TextContent.Should().Be("Gender: Female");
            Rows().Count.Should().Be(4, because: "1 header row + 2 data rows + 1 footer row");

            var professionGrouping = comp.Find(".GroupByProfession");
            professionGrouping.Click();
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(true);
            var professionCells = Cells();
            professionCells.Count.Should().Be(2, because: "2 data rows");
            professionCells[0].TextContent.Should().Be("Profession: Cook");
            professionCells[1].TextContent.Should().Be("Profession: (None)");
            Rows().Count.Should().Be(4, because: "1 header row + 2 data rows + 1 footer row");

            //click age grouping in grid
            var headerOption = comp.Find("th.age .mud-menu button");
            headerOption.Click();
            var listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(2);
            var clickablePopover = listItems[1].Find(".mud-menu-item");
            clickablePopover.Click();
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(true);
            Rows().Count.Should().Be(5, because: "1 header row + 3 data rows + 1 footer row");

            //click gender grouping in grid
            headerOption = comp.Find("th.gender .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(2);
            clickablePopover = listItems[1].Find(".mud-menu-item");
            clickablePopover.Click();
            comp.Instance.IsGenderGrouped.Should().Be(true);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(true);
            Rows().Count.Should().Be(5, because: "1 header row + 3 data rows + 1 footer row");

            //click Name grouping in grid
            headerOption = comp.Find("th.name .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(2);
            clickablePopover = listItems[1].Find(".mud-menu-item");
            clickablePopover.Click();
            comp.Instance.IsGenderGrouped.Should().Be(true);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(true);
            Rows().Count.Should().Be(6, because: "1 header row + 4 data rows + 1 footer row");

            //click profession grouping in grid
            headerOption = comp.Find("th.profession .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudMenuItem>();
            listItems.Count.Should().Be(2);
            clickablePopover = listItems[1].Find(".mud-menu-item");
            clickablePopover.Click();
            comp.Instance.IsGenderGrouped.Should().Be(true);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            Rows().Count.Should().Be(6, because: "1 header row + 4 data rows + 1 footer row");
        }


        [Test]
        public async Task DataGridGroupedWithServerDataPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridGroupableServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupableServerDataTest.Item>>();
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(12, because: "1 header row + 10 data rows + 1 footer row");
            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(10, because: "We have 10 data rows with one group collapsed");
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(32, because: "1 header row + 10 data rows + 1 footer row + 10 group rows + 10 footer group rows");
            cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(30, because: "We have 10 data rows with one group + 10*2 cells inside groups");
            //check cells
            cells[0].TextContent.Should().Be("Number: 1");
            cells[1].TextContent.Should().Be("Hydrogen"); cells[2].TextContent.Should().Be("1");
            cells[3].TextContent.Should().Be("Number: 2");
            cells[4].TextContent.Should().Be("Helium"); cells[5].TextContent.Should().Be("2");
            cells[6].TextContent.Should().Be("Number: 3");
            cells[7].TextContent.Should().Be("Lithium"); cells[8].TextContent.Should().Be("3");
            cells[9].TextContent.Should().Be("Number: 4");
            cells[10].TextContent.Should().Be("Beryllium"); cells[11].TextContent.Should().Be("4");
            cells[12].TextContent.Should().Be("Number: 5");
            cells[13].TextContent.Should().Be("Boron"); cells[14].TextContent.Should().Be("5");
            cells[15].TextContent.Should().Be("Number: 6");
            cells[16].TextContent.Should().Be("Carbon"); cells[17].TextContent.Should().Be("6");
            cells[18].TextContent.Should().Be("Number: 7");
            cells[19].TextContent.Should().Be("Nitrogen"); cells[20].TextContent.Should().Be("7");
            cells[21].TextContent.Should().Be("Number: 8");
            cells[22].TextContent.Should().Be("Oxygen"); cells[23].TextContent.Should().Be("8");
            cells[24].TextContent.Should().Be("Number: 9");
            cells[25].TextContent.Should().Be("Fluorine"); cells[26].TextContent.Should().Be("9");
            cells[27].TextContent.Should().Be("Number: 10");
            cells[28].TextContent.Should().Be("Neon"); cells[29].TextContent.Should().Be("10");
            //get next page
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Next));
            comp.Render();
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroupsAsync());
            cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(10, because: "We have 10 data rows with one group collapsed from next page");
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroupsAsync());
            cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(30, because: "We have next 10 data rows with one group + 10*2 cells inside groups");
            //cells should have data from next page
            cells[0].TextContent.Should().Be("Number: 11");
            cells[1].TextContent.Should().Be("Sodium"); cells[2].TextContent.Should().Be("11");
            cells[3].TextContent.Should().Be("Number: 12");
            cells[4].TextContent.Should().Be("Magnesium"); cells[5].TextContent.Should().Be("12");
            cells[6].TextContent.Should().Be("Number: 13");
            cells[7].TextContent.Should().Be("Aluminium"); cells[8].TextContent.Should().Be("13");
            cells[9].TextContent.Should().Be("Number: 14");
            cells[10].TextContent.Should().Be("Silicon"); cells[11].TextContent.Should().Be("14");
            cells[12].TextContent.Should().Be("Number: 15");
            cells[13].TextContent.Should().Be("Phosphorus"); cells[14].TextContent.Should().Be("15");
            cells[15].TextContent.Should().Be("Number: 16");
            cells[16].TextContent.Should().Be("Sulfur"); cells[17].TextContent.Should().Be("16");
            cells[18].TextContent.Should().Be("Number: 17");
            cells[19].TextContent.Should().Be("Chlorine"); cells[20].TextContent.Should().Be("17");
            cells[21].TextContent.Should().Be("Number: 18");
            cells[22].TextContent.Should().Be("Argon"); cells[23].TextContent.Should().Be("18");
            cells[24].TextContent.Should().Be("Number: 19");
            cells[25].TextContent.Should().Be("Potassium"); cells[26].TextContent.Should().Be("19");
            cells[27].TextContent.Should().Be("Number: 20");
            cells[28].TextContent.Should().Be("Calcium"); cells[29].TextContent.Should().Be("20");
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void TestRtlGroupIconMethod(bool isRightToLeft, bool isExpanded)
        {
            var test = new MudDataGrid<int>();
            if (isExpanded)
            {
                test.GetGroupIcon(isExpanded, isRightToLeft).Should().Be(Icons.Material.Filled.ExpandMore);
            }
            else
            {
                test.GetGroupIcon(isExpanded, isRightToLeft).Should().Be(isRightToLeft ? Icons.Material.Filled.ChevronLeft : Icons.Material.Filled.ChevronRight);
            }
        }

        [Test]
        public async Task GroupExpandClick_ShouldToggleExpandedState()
        {
            // Arrange
            var component = Context.RenderComponent<DataGridGroupingMultiLevelTest>();

            var dataGrid = component.FindComponent<MudDataGrid<DataGridGroupingMultiLevelTest.USState>>();
            await component.InvokeAsync(() => dataGrid.Instance.ReloadServerData());

            var rows = component.FindComponents<DataGridGroupRow<DataGridGroupingMultiLevelTest.USState>>();
            rows.Count.Should().Be(15);
            var row = rows[0];
            // Test the method
            // Act
            void GetCount(bool currExpanded)
            {
                var defaultExpanded = row.Instance.GroupDefinition.Expanded;
                // Whatever the expanded state is if it differs from the default it should be in the dictionary
                dataGrid.Instance._groupExpansionsDict.Count.Should().Be(currExpanded != defaultExpanded ? 1 : 0);
            }

            // Test the UI
            var expandButton = () => row.Find(".mud-datagrid-group-button");
            expandButton.Should().NotBeNull();
            expandButton().Click();

            row.WaitForAssertion(() => row.Instance._expanded.Should().BeFalse());
            row.WaitForAssertion(() => GetCount(false));
            expandButton().Click();

            row.WaitForAssertion(() => row.Instance._expanded.Should().BeTrue());
            row.WaitForAssertion(() => GetCount(true));
        }

        [Test]
        public async Task DataGrid_Grouping_TestGroupableSets()
        {
            var component = Context.RenderComponent<DataGridGroupingMultiLevelTest>();

            var dataGrid = component.FindComponent<MudDataGrid<DataGridGroupingMultiLevelTest.USState>>();
            // by default has a groupdefinition
            dataGrid.WaitForAssertion(() => dataGrid.Instance._groupDefinition.Should().NotBeNull());
            // turn off grouping for the whole grid
            dataGrid.SetParam(x => x.Groupable, false);
            dataGrid.Render();
            await component.InvokeAsync(() => dataGrid.Instance.ReloadServerData());

            // grouping shouldn't exist
            dataGrid.Instance._groupDefinition.Should().BeNull();
            foreach (var column in dataGrid.Instance.RenderedColumns)
            {
                column.GroupingState.Value.Should().Be(false);
            }

            // no grouping rows
            var rows = component.FindComponents<DataGridGroupRow<DataGridGroupingMultiLevelTest.USState>>();
            rows.Count.Should().Be(0);
        }

        [Test]
        public async Task DataGrid_Grouping_GroupDefinition()
        {
            var component = Context.RenderComponent<DataGridGroupingMultiLevelTest>();

            var dataGrid = component.FindComponent<MudDataGrid<DataGridGroupingMultiLevelTest.USState>>();
            await component.InvokeAsync(() => dataGrid.Instance.ReloadServerData());
            // grouping is already setup make sure group definition is not null and it's first inner definition is not null
            dataGrid.WaitForAssertion(() => dataGrid.Instance._groupDefinition.Should().NotBeNull());
            dataGrid.Instance._groupDefinition.InnerGroup.Should().NotBeNull();
            dataGrid.Instance._groupDefinition.Grouping.Should().BeNullOrEmpty();
            // _groupDefinition is the definition for all the groups but isn't combined into the items until display so we need to 
            // check the final definitions from within the DataGridGroupRow            

            var rows = component.FindComponents<DataGridGroupRow<DataGridGroupingMultiLevelTest.USState>>();
            rows.Count.Should().Be(15);
            var row = rows[0];

            // Only One Manufacturing Primary Industry
            row.Instance.GroupDefinition.Title.Should().Be("Primary Industry");
            row.Instance.GroupDefinition.Grouping.Key.Should().Be("Manufacturing");
            row.Instance.Items.Should().NotBeNull();
            row.Instance.Items.Count().Should().Be(1);
            // Agriculture should have 2 items
            row = rows[6];
            row.Instance.Items.Should().NotBeNull();
            row.Instance.Items.Count().Should().Be(2);
            // the next row is a sub group of Agriculture and should also have 2 items
            row = rows[7];
            row.Instance.Items.Should().NotBeNull();
            row.Instance.Items.Count().Should().Be(2);
        }
    }
}
