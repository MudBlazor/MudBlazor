using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Pagination;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class Pagination : BunitTest
    {

        /// <summary>
        /// Tests the clicking on control buttons
        /// </summary>
        /// <param name="controlButton">The type of the control button. Page.First for the navigate-to-first-page button.</param>
        /// <param name="numberOfClicks">The number of times the control button is clicked.</param>
        /// <param name="expectedSelectedPage">The expected selected page after clicking numberOfClicks times on the button.</param>
        /// <param name="expectedDisabled">The expected disabled state after clicking numberOfClicks times on the button.</param>
        [TestCase(Page.First, 1, 6, 1, true)]
        [TestCase(Page.Previous, 3, 5, 2, false)]
        [TestCase(Page.Previous, 7, 6, 1, true)]
        [TestCase(Page.Next, 3, 5, 8, false)]
        [TestCase(Page.Next, 7, 6, 11, true)]
        [TestCase(Page.Last, 1, 6, 11, true)]
        [Test]
        public async Task PaginationControlButtonClickTest(Page controlButton, int numberOfClicks,
            int initiallySelectedPage, int expectedSelectedPage, bool expectedDisabled)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;
            //navigate to the specified page
            await comp.InvokeAsync(() => { pagination.NavigateTo(initiallySelectedPage - 1); });

            //Click numberOfClicks times on the control button
            for (var i = 0; i < numberOfClicks; i++)
            {
                var button = FindControlButton(comp, controlButton);
                button.Click();
            }

            //Expected values
            pagination.Selected.Should().Be(expectedSelectedPage);
            FindControlButton(comp, controlButton).IsDisabled().Should().Be(expectedDisabled);
            comp.Find("#mud-pagination-test-selected").TextContent.Should()
                .Be("Selected: " + expectedSelectedPage);
        }

        /// <summary>
        /// Tests the aria-labels for the control buttons
        /// </summary>
        /// <param name="controlButton">The type of the control button. Page.First for the navigate-to-first-page button.</param>
        /// <param name="expectedButtonAriaLabel">The expected value in the aria-label.</param>
        [TestCase(Page.First, "First page")]
        [TestCase(Page.Previous, "Previous page")]
        [TestCase(Page.Next, "Next page")]
        [TestCase(Page.Last, "Last page")]
        [Test]
        public void PaginationControlButtonAriaLabelTest(Page controlButton, string expectedButtonAriaLabel)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            //get control button
            var button = FindControlButton(comp, controlButton);

            //Expected values
            button.Attributes.GetNamedItem("aria-label")?.Value.Should().Be(expectedButtonAriaLabel);
        }

        /// <summary>
        /// Tests the aria-labels for the page buttons. . . note the index's aren't sequential because there are elements of "..."
        /// </summary>
        /// <param name="index">The index of the control button. first page button has index 2.</param>
        /// <param name="label">The expected value in the aria-label.</param>
        [TestCase(2, "Page 1")]
        [TestCase(3, "Page 2")]
        [TestCase(5, "Current page 6")]
        [TestCase(7, "Page 10")]
        [TestCase(8, "Page 11")]
        [Test]
        public void PaginationPageButtonAriaLabelTest(int index, string label)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();
            var buttons = comp.FindAll(".mud-pagination-item button");
            var button = buttons[index];
            button.Attributes.GetNamedItem("aria-label")?.Value.Should().Be(label);
            if (index == 5)
            {
                button.Attributes.GetNamedItem("aria-current")?.Value.Should().Be("page");
            }
        }

        /// <summary>
        /// Tests the event callbacks of control button click events
        /// </summary>
        /// <param name="controlButton">The type of the control button. Page.First for the navigate-to-first-page button.</param>
        /// <param name="expectedButtonClickedValue">The expected value in the dom after clicking on the button.</param>
        [TestCase(Page.First, 0)]
        [TestCase(Page.Previous, 1)]
        [TestCase(Page.Next, 2)]
        [TestCase(Page.Last, 3)]
        [Test]
        public void PaginationControlButtonEventCallbackTest(Page controlButton, int expectedButtonClickedValue)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            //Click control button
            FindControlButton(comp, controlButton).Click();

            //Expected values
            comp.Find("#mud-pagination-test-button-clicked").TextContent.Should()
                .Be("Button clicked: " + expectedButtonClickedValue);
        }

        /// <summary>
        /// Tests the clicking on page buttons
        /// </summary>
        /// <param name="clickIndexPage">The index of the clicked page button.</param>
        /// <param name="initiallySelectedPage">The initially selected page.</param>
        /// <param name="expectedSelectedPage">The expected selected page.</param>
        [TestCase(0, 6, 1)]
        [TestCase(6, 6, 11)]
        [TestCase(5, 5, 6)]
        [TestCase(2, 5, 3)]
        [Test]
        public async Task PaginationPageButtonClickTest(int clickIndexPage, int initiallySelectedPage,
            int expectedSelectedPage)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;
            //navigate to the specified page
            await comp.InvokeAsync(() => { pagination.NavigateTo(initiallySelectedPage - 1); });

            //Click on the page button, +2 because of the first two control buttons
            comp.FindAll(".mud-pagination-item button")[clickIndexPage + 2].Click();

            //Expected values
            pagination.Selected.Should().Be(expectedSelectedPage);
            comp.Find("#mud-pagination-test-selected").TextContent.Should()
                .Be("Selected: " + expectedSelectedPage);
        }

        //returns the specified control button
        private static IElement FindControlButton(IRenderedFragment comp, Page controlButton)
        {
            var buttons = comp.FindAll(".mud-pagination-item button");
            var button = controlButton switch
            {
                Page.First => buttons[0],
                Page.Previous => buttons[1],
                Page.Next => buttons[^2],
                Page.Last => buttons[^1],
                _ => throw new ArgumentOutOfRangeException(nameof(controlButton), controlButton,
                    "This control button type is not supported!")
            };
            return button;
        }

        /// <summary>
        /// Tests the NavigateTo(Page) method
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        /// <param name="expectedSelectedPage">The expected selected page.</param>
        [TestCase(Page.First, 1)]
        [TestCase(Page.Previous, 5)]
        [TestCase(Page.Next, 7)]
        [TestCase(Page.Last, 11)]
        [Test]
        public async Task PaginationNavigateToPageTest(Page page, int expectedSelectedPage)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;

            //navigate to the specified page
            await comp.InvokeAsync(() => { pagination.NavigateTo(page); });

            //Expected values
            pagination.Selected.Should().Be(expectedSelectedPage);
            comp.Find("#mud-pagination-test-selected").TextContent.Should()
                .Be("Selected: " + expectedSelectedPage);
        }


        /// <summary>
        /// Tests the NavigateTo(int) method
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        /// <param name="expectedSelectedPage">The expected selected page.</param>
        [TestCase(1, 1)]
        [TestCase(11, 11)]
        [TestCase(-1, 1)]
        [TestCase(12, 11)]
        [Test]
        public async Task PaginationNavigateToPageTest(int page, int expectedSelectedPage)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;

            //navigate to the specified page
            await comp.InvokeAsync(() => { pagination.NavigateTo(page - 1); });

            //Expected values
            pagination.Selected.Should().Be(expectedSelectedPage);
            comp.Find("#mud-pagination-test-selected").TextContent.Should()
                .Be("Selected: " + expectedSelectedPage);
        }

        /// <summary>
        /// Tests if no ellipsis appear
        /// </summary>
        /// <param name="count">The number of total items.</param>
        /// <param name="middleCount">The number of items displayed in the middle.</param>
        /// <param name="boundaryCount">The number of items displayed on the start and end.</param>
        [TestCase(21, 5, 7)]
        [TestCase(9, 3, 2)]
        [TestCase(5, 1, 1)]
        [TestCase(5, -1, 1)]
        [TestCase(5, 1, -1)]
        [Test]
        public void PaginationCountWithoutEllipsisTest(int count, int middleCount, int boundaryCount)
        {
            var comp = Context.RenderComponent<PaginationCountTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;
            comp.Find(".mud-pagination-test-middle-count input").Change(middleCount.ToString());
            comp.Find(".mud-pagination-test-boundary-count input").Change(boundaryCount.ToString());

            //Expected values
            pagination.MiddleCount.Should().Be(Math.Max(1, middleCount));
            pagination.BoundaryCount.Should().Be(Math.Max(1, boundaryCount));

            for (var i = 1; i <= count; i++)
            {
                comp.Find(".mud-pagination-test-count input").Change(i.ToString());
                var buttons = comp.FindAll(".mud-pagination-item");
                //Expected values
                buttons.Count.Should().Be(i);
                for (var j = 0; j < buttons.Count; j++)
                {
                    buttons[j].TextContent.Should().Be((j + 1).ToString());
                }
            }
        }
        /// <summary>
        /// Tests if the items are displayed correctly
        /// </summary>
        /// <param name="selectedPage">The initially selected page.</param>
        /// <param name="count">The number of items.</param>
        /// <param name="middleCount">The number of items between the ellipsis.</param>
        /// <param name="boundaryCount">The number of items at the start and end of the pagination.</param>
        /// <param name="expectedValues">The expected content of the items.</param>
        [TestCase(6, 11, 3, 2, new[] { "1", "2", "...", "5", "6", "7", "...", "10", "11" })]
        [TestCase(7, 11, 3, 2, new[] { "1", "2", "...", "6", "7", "8", "9", "10", "11" })]
        [TestCase(11, 11, 3, 2, new[] { "1", "2", "...", "6", "7", "8", "9", "10", "11" })]
        [TestCase(5, 11, 3, 2, new[] { "1", "2", "3", "4", "5", "6", "...", "10", "11" })]
        [TestCase(3, 11, 3, 2, new[] { "1", "2", "3", "4", "5", "6", "...", "10", "11" })]
        [TestCase(11, 22, 1, 1, new[] { "1", "...", "11", "...", "22" })]
        [TestCase(1, 22, 1, 1, new[] { "1", "2", "3", "...", "22" })]
        [TestCase(8, 22, 5, 3, new[] { "1", "2", "3", "...", "6", "7", "8", "9", "10", "...", "20", "21", "22" })]
        [TestCase(7, 22, 5, 3, new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "...", "20", "21", "22" })]
        [TestCase(16, 22, 5, 3, new[] { "1", "2", "3", "...", "14", "15", "16", "17", "18", "19", "20", "21", "22" })]
        [TestCase(22, 22, 5, 3, new[] { "1", "2", "3", "...", "14", "15", "16", "17", "18", "19", "20", "21", "22" })]
        [Test]
        public async Task PaginationCountWithEllipsisTest(int selectedPage, int count, int middleCount,
            int boundaryCount, string[] expectedValues)
        {
            var comp = Context.RenderComponent<PaginationCountTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;
            //set count variables
            comp.Find(".mud-pagination-test-count input").Change(count.ToString());
            comp.Find(".mud-pagination-test-middle-count input").Change(middleCount.ToString());
            comp.Find(".mud-pagination-test-boundary-count input").Change(boundaryCount.ToString());

            //navigate to the specified page
            await comp.InvokeAsync(() => { pagination.NavigateTo(selectedPage - 1); });

            //Expected values
            var items = comp.FindAll(".mud-pagination-item");
            items.Count.Should().Be(middleCount + (2 * boundaryCount) + 2);
            for (var j = 0; j < items.Count; j++)
            {
                items[j].TextContent.Should().Be(expectedValues[j]);
            }
        }

        /// <summary>
        /// Tests if styles/visual parameters are applied correctly
        /// </summary>
        [Test]
        public void PaginationStylesTest()
        {
            var comp = Context.RenderComponent<PaginationStylesTest>();

            var buttons = comp.FindAll(".mud-pagination-item button");
            var pagination = comp.Find("ul.mud-pagination");
            var paginationItems = comp.FindAll("mud-pagination-item");

            //test if previous and next buttons are hidden
            buttons.Count.Should().Be(8);

            //test if variant is filled
            pagination.ClassName.Should().Contain("mud-pagination-filled");

            //test if color is secondary
            buttons[0].ClassName.Should().Contain("mud-button-filled-secondary");

            //test if items are rectangular
            foreach (var item in paginationItems)
            {
                item.ClassName.Should().Contain("mud-pagination-item-rectangular");
            }

            //test if size is large
            pagination.ClassName.Should().Contain("mud-pagination-large");

            //test if elevation is disabled
            pagination.ClassName.Should().Contain("mud-pagination-disable-elevation");

            //test if all buttons are disabled
            foreach (var button in buttons)
            {
                button.IsDisabled().Should().BeTrue();
            }

            //test if rtl is used
            pagination.ClassName.Should().Contain("mud-pagination-rtl");
        }
    }
}
