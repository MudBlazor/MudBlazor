using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Extensions;
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
        /// <param name="initiallySelectedPage">The index of initially selected page.</param>
        /// <param name="expectedSelectedPage">The expected selected page after clicking numberOfClicks times on the button.</param>
        /// <param name="expectedDisabled">The expected disabled state after clicking numberOfClicks times on the button.</param>
        [TestCase(Page.First, 1, 6, 1, true)]
        [TestCase(Page.Previous, 3, 5, 2, false)]
        [TestCase(Page.Previous, 7, 6, 1, true)]
        [TestCase(Page.Next, 3, 5, 8, false)]
        [TestCase(Page.Next, 7, 6, 11, true)]
        [TestCase(Page.Last, 1, 6, 11, true)]
        [Test]
        public async Task PaginationControlButtonClickTest(Page controlButton, int numberOfClicks, int initiallySelectedPage, int expectedSelectedPage, bool expectedDisabled)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;
            //navigate to the specified page
            await comp.InvokeAsync(async () => { await pagination.NavigateToAsync(initiallySelectedPage - 1); });

            //Click numberOfClicks times on the control button
            for (var i = 0; i < numberOfClicks; i++)
            {
                var button = FindControlButton(comp, controlButton);
                button.Click();
            }

            //Expected values
            pagination.GetState(x => x.Selected).Should().Be(expectedSelectedPage);
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
        [TestCase(6, "Current page 6")]
        [TestCase(8, "Edit page number")]
        [TestCase(9, "Page 10")]
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
        /// Tests if the page buttons are hidden
        /// </summary>
        [Test]
        public void HidePageButtonTest()
        {
            var comp = Context.RenderComponent<PaginationHidePageButtonsTest>();

            comp.FindAll(".mud-pagination-item button").Count.Should().Be(2);
        }

        /// <summary>
        /// Tests the clicking on page buttons
        /// </summary>
        /// <param name="clickIndexPage">The index of the clicked page button.</param>
        /// <param name="initiallySelectedPage">The initially selected page.</param>
        /// <param name="expectedSelectedPage">The expected selected page.</param>
        [TestCase(0, 6, 1)]
        [TestCase(8, 6, 11)]
        [TestCase(5, 5, 6)]
        [TestCase(2, 5, 3)]
        [Test]
        public async Task PaginationPageButtonClickTest(int clickIndexPage, int initiallySelectedPage,
            int expectedSelectedPage)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;
            //navigate to the specified page
            await comp.InvokeAsync(async () => { await pagination.NavigateToAsync(initiallySelectedPage - 1); });

            //Click on the page button, +2 because of the first two control buttons
            comp.FindAll(".mud-pagination-item button")[clickIndexPage + 2].Click();

            //Expected values
            pagination.GetState(x => x.Selected).Should().Be(expectedSelectedPage);
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
                    @"This control button type is not supported!")
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
        [TestCase((Page)50, 6)]
        [Test]
        public async Task PaginationNavigateToPageTest(Page page, int expectedSelectedPage)
        {
            var comp = Context.RenderComponent<PaginationButtonTest>();

            var pagination = comp.FindComponent<MudPagination>().Instance;

            //navigate to the specified page
            await comp.InvokeAsync(async () => { await pagination.NavigateToAsync(page); });

            //Expected values
            pagination.GetState(x => x.Selected).Should().Be(expectedSelectedPage);
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
            await comp.InvokeAsync(async () => { await pagination.NavigateToAsync(page - 1); });

            //Expected values
            pagination.GetState(x => x.Selected).Should().Be(expectedSelectedPage);
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
            pagination.GetState(x => x.MiddleCount).Should().Be(Math.Max(1, middleCount));
            pagination.GetState(x => x.BoundaryCount).Should().Be(Math.Max(1, boundaryCount));

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
            await comp.InvokeAsync(async () => { await pagination.NavigateToAsync(selectedPage - 1); });

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
            buttons.Count.Should().Be(9); //8 number + 1 ellipsis

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

        [Test]
        public async Task Ellipsis_Click_ShowsInput()
        {
            Context.Services.AddSingleton<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            Context.Services.AddSingleton<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            Context.Services.AddTransient<InternalMudLocalizer>();
            Context.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            Context.JSInterop.SetupVoid("mudElementRef.removeOnBlurEvent", _ => true);
            var selectedPage = 1;
            var comp = Context.RenderComponent<MudPagination>(parameters => parameters
                .Add(p => p.Count, 10)
                .Add(p => p.MiddleCount, 2) // Ensure ellipsis shows near start
                .Add(p => p.BoundaryCount, 1)
                .Add(p => p.Selected, selectedPage)
                .Add(p => p.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, newVal => selectedPage = newVal))
            );

            selectedPage = 5;
            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.Count, 10)
                .Add(p => p.MiddleCount, 1)
                .Add(p => p.BoundaryCount, 1)
                .Add(p => p.Selected, selectedPage)
            );

            // Find the first ellipsis button.
            var ellipsisButtons = comp.FindAll(".mud-pagination-ellipsis-button");
            ellipsisButtons.Should().NotBeEmpty();
            var ellipsisButton = ellipsisButtons.First();

            await ellipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            // Assert input is visible
            comp.Find(".mud-pagination-ellipsis-input").Should().NotBeNull();

            // Assert that there is now only one ellipsis button remaining
            comp.FindAll(".mud-pagination-ellipsis-button", enableAutoRefresh: true).Count.Should().Be(1);
        }

        [Test]
        public async Task Ellipsis_EnterValidPage_NavigatesAndHidesInput()
        {
            Context.Services.AddSingleton<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            Context.Services.AddSingleton<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            Context.Services.AddTransient<InternalMudLocalizer>();
            Context.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            Context.JSInterop.SetupVoid("mudElementRef.removeOnBlurEvent", _ => true);
            var selectedPage = 5; // Start page where an ellipsis is visible
            var newPageViaInput = 0;
            var comp = Context.RenderComponent<MudPagination>(parameters => parameters
                .Add(p => p.Count, 10)
                .Add(p => p.MiddleCount, 1) // e.g., 1 ... 5 ... 10
                .Add(p => p.BoundaryCount, 1)
                .Add(p => p.Selected, selectedPage)
                .Add(p => p.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, newVal => { selectedPage = newVal; newPageViaInput = newVal; }))
            );

            var ellipsisButton = comp.FindAll(".mud-pagination-ellipsis-button").First();
            await ellipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            var inputField = comp.Find(".mud-pagination-ellipsis-input input"); // Find the actual input element
            inputField.Should().NotBeNull();

            await inputField.InputAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "3" });
            await inputField.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

            newPageViaInput.Should().Be(3); // SelectedChanged was invoked with 3
            selectedPage.Should().Be(3); // Component's selected page updated

            // Input field should be hidden, and ellipsis button should be back
            comp.FindAll(".mud-pagination-ellipsis-input", enableAutoRefresh: true).Should().BeEmpty();
            comp.FindAll(".mud-pagination-ellipsis-button", enableAutoRefresh: true).Should().NotBeEmpty();

            comp.Find(".mud-pagination-item-selected button").TextContent.Should().Be("3");
        }

        [Test]
        [TestCase("0")] // Too low
        [TestCase("99")] // Too high
        [TestCase("abc")] // Non-numeric
        public async Task Ellipsis_EnterInvalidPage_NoNavigationAndHidesInput(string invalidInput)
        {
            Context.Services.AddSingleton<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            Context.Services.AddSingleton<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            Context.Services.AddTransient<InternalMudLocalizer>();
            Context.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            Context.JSInterop.SetupVoid("mudElementRef.removeOnBlurEvent", _ => true);
            var initialSelectedPage = 5;
            var selectedPageTracker = initialSelectedPage; // To track if SelectedChanged is called
            var comp = Context.RenderComponent<MudPagination>(parameters => parameters
                .Add(p => p.Count, 10)
                .Add(p => p.MiddleCount, 1)
                .Add(p => p.BoundaryCount, 1)
                .Add(p => p.Selected, initialSelectedPage)
                .Add(p => p.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, newVal => selectedPageTracker = newVal))
            );

            var ellipsisButton = comp.FindAll(".mud-pagination-ellipsis-button").First();
            await ellipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            var inputField = comp.Find(".mud-pagination-ellipsis-input input");
            await inputField.InputAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = invalidInput });
            await inputField.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

            selectedPageTracker.Should().Be(initialSelectedPage); // SelectedChanged should not be called with a new value

            comp.FindAll(".mud-pagination-ellipsis-input", enableAutoRefresh: true).Should().BeEmpty();
            comp.FindAll(".mud-pagination-ellipsis-button", enableAutoRefresh: true).Should().NotBeEmpty();
            // Verify active page button by checking its text content
            comp.WaitForAssertion(() => comp.Find("li.mud-pagination-item-selected button.mud-button-root").TextContent.Trim().Should().Be(initialSelectedPage.ToString()), TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task Ellipsis_PressEscape_CancelsAndHidesInput()
        {
            Context.Services.AddSingleton<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            Context.Services.AddSingleton<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            Context.Services.AddTransient<InternalMudLocalizer>();
            Context.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            Context.JSInterop.SetupVoid("mudElementRef.removeOnBlurEvent", _ => true);
            var initialSelectedPage = 5;
            var selectedPageTracker = initialSelectedPage;
            var comp = Context.RenderComponent<MudPagination>(parameters => parameters
                .Add(p => p.Count, 10)
                .Add(p => p.MiddleCount, 1)
                .Add(p => p.BoundaryCount, 1)
                .Add(p => p.Selected, initialSelectedPage)
                .Add(p => p.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, newVal => selectedPageTracker = newVal))
            );

            var ellipsisButton = comp.FindAll(".mud-pagination-ellipsis-button").First();
            await ellipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            var inputField = comp.Find(".mud-pagination-ellipsis-input input");
            await inputField.InputAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "3" }); // User types something
            await inputField.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Escape" });

            selectedPageTracker.Should().Be(initialSelectedPage); // SelectedChanged should not be called

            comp.FindAll(".mud-pagination-ellipsis-input", enableAutoRefresh: true).Should().BeEmpty();
            comp.FindAll(".mud-pagination-ellipsis-button", enableAutoRefresh: true).Should().NotBeEmpty();
            // Verify active page button by checking its text content
            comp.WaitForAssertion(() => comp.Find("li.mud-pagination-item-selected button.mud-button-root").TextContent.Trim().Should().Be(initialSelectedPage.ToString()), TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task Ellipsis_HandlesTwoEllipses_Independently()
        {
            Context.Services.AddSingleton<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            Context.Services.AddSingleton<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            Context.Services.AddTransient<InternalMudLocalizer>();
            Context.JSInterop.SetupVoid("mudElementRef.observeFocus", _ => true).SetVoidResult();
            Context.JSInterop.SetupVoid("mudElementRef.activateFocusTrap", _ => true).SetVoidResult();
            Context.JSInterop.Setup<Interop.BoundingClientRect>("mudElementRef.getBoundingClientRect", _ => true).SetResult(new Interop.BoundingClientRect());
            Context.JSInterop.SetupVoid("mudElementRef.restoreFocus", _ => true).SetVoidResult();
            Context.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true).SetVoidResult();
            Context.JSInterop.SetupVoid("mudElementRef.removeOnBlurEvent", _ => true).SetVoidResult();

            var selectedPage = 10;
            var initialSelectedPage = selectedPage;
            var pageCount = 20;
            var boundaryCount = 1;
            var middleCount = 1;

            var selectedChangedValue = 0;
            var selectedChangedCalled = false;

            var comp = Context.RenderComponent<MudPagination>(parameters => parameters
                .Add(p => p.Count, pageCount)
                .Add(p => p.Selected, selectedPage)
                .Add(p => p.BoundaryCount, boundaryCount)
                .Add(p => p.MiddleCount, middleCount)
                .Add(p => p.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, newVal =>
                {
                    selectedChangedValue = newVal;
                    selectedChangedCalled = true;
                }))
            );

            // Initial check: Two ellipsis buttons should be present
            comp.WaitForAssertion(() => comp.FindAll(".mud-pagination-ellipsis-button").Count.Should().Be(2, "Initially two ellipsis buttons should be visible."));

            // --- Interact with the first ellipsis ---
            var allEllipses = comp.FindAll(".mud-pagination-ellipsis-button");
            var firstEllipsisButton = allEllipses[0];

            await firstEllipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
            comp.WaitForState(() => comp.FindAll(".mud-pagination-ellipsis-input input").Any(), TimeSpan.FromSeconds(1));

            // Check: one input, one ellipsis button
            comp.FindAll(".mud-pagination-ellipsis-input input").Count.Should().Be(1);
            comp.FindAll(".mud-pagination-ellipsis-button").Count.Should().Be(1, "After clicking first ellipsis, one should remain a button.");

            var inputField = comp.Find(".mud-pagination-ellipsis-input input");
            await inputField.InputAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "5" });
            await inputField.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

            comp.WaitForAssertion(() =>
            {
                selectedChangedCalled.Should().BeTrue();
                selectedChangedValue.Should().Be(5);
                var ellipsisButtons = comp.FindAll(".mud-pagination-ellipsis-button");
                ellipsisButtons.Count.Should().Be(2, "After navigating to page 5, two ellipses should remain (e.g., 1 ... 5 ... 20).");
                comp.Find("li.mud-pagination-item-selected button.mud-button-root").TextContent.Trim().Should().Be("5");
            }, TimeSpan.FromSeconds(1));

            selectedChangedCalled = false;

            var remainingEllipses = comp.FindAll(".mud-pagination-ellipsis-button");
            if (remainingEllipses.Any())
            {
                var secondEllipsisButtonToTest = remainingEllipses.First();

                await secondEllipsisButtonToTest.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
                comp.WaitForState(() => comp.FindAll(".mud-pagination-ellipsis-input input").Any(), TimeSpan.FromSeconds(1));

                inputField = comp.Find(".mud-pagination-ellipsis-input input");
                await inputField.InputAsync(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "18" });
                await inputField.KeyDownAsync(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "Enter" });

                comp.WaitForAssertion(() =>
                {
                    selectedChangedCalled.Should().BeTrue();
                    selectedChangedValue.Should().Be(18);
                    comp.FindAll(".mud-pagination-ellipsis-button").Count.Should().Be(1, "After navigating to page 18, one ellipsis should remain (e.g., 1 ... 18 19 20).");
                    comp.Find("li.mud-pagination-item-selected button.mud-button-root").TextContent.Trim().Should().Be("18");
                }, TimeSpan.FromSeconds(1));
            }
            else
            {
                Assert.Fail("Expected at least one ellipsis to remain for the second part of the test.");
            }
        }

        [Test]
        public async Task EllipsisInput_Blur_RevertsToButton()
        {
            Context.Services.AddSingleton<ILocalizationInterceptor, DefaultLocalizationInterceptor>();
            Context.Services.AddSingleton<ILocalizationEnumInterceptor, DefaultLocalizationEnumInterceptor>();
            Context.Services.AddTransient<InternalMudLocalizer>();

            Context.JSInterop.SetupVoid("mudElementRef.observeFocus", _ => true).SetVoidResult();
            Context.JSInterop.SetupVoid("mudElementRef.activateFocusTrap", _ => true).SetVoidResult();
            Context.JSInterop.Setup<Interop.BoundingClientRect>("mudElementRef.getBoundingClientRect", _ => true).SetResult(new Interop.BoundingClientRect());
            Context.JSInterop.SetupVoid("mudElementRef.restoreFocus", _ => true).SetVoidResult();
            Context.JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true).SetVoidResult();
            Context.JSInterop.SetupVoid("mudElementRef.removeOnBlurEvent", _ => true).SetVoidResult();

            var selectedPage = 10;
            var initialSelectedPage = selectedPage;
            var selectedChangedCalled = false;

            var comp = Context.RenderComponent<MudPagination>(parameters => parameters
                .Add(p => p.Count, 20)
                .Add(p => p.MiddleCount, 2)
                .Add(p => p.BoundaryCount, 1)
                .Add(p => p.Selected, selectedPage)
                .Add(p => p.SelectedChanged, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, newVal =>
                {
                    selectedPage = newVal;
                    selectedChangedCalled = true;
                }))
            );

            var ellipsisButton = comp.FindAll(".mud-pagination-ellipsis-button").First();
            await ellipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
            comp.WaitForState(() => comp.FindAll(".mud-pagination-ellipsis-input input").Any(), TimeSpan.FromSeconds(1));


            // Find the input element
            var inputElement = comp.Find(".mud-pagination-ellipsis-input input");
            inputElement.Should().NotBeNull("Input element should be present after clicking ellipsis.");

            // Simulate blur on the input element
            await inputElement.BlurAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());

            // After blur, the input should be gone, and the button should reappear.
            comp.WaitForAssertion(() =>
            {
                comp.FindAll(".mud-pagination-ellipsis-input input").Should().BeEmpty("Input should disappear after blur.");
                comp.FindAll(".mud-pagination-ellipsis-button").Should().NotBeEmpty("Ellipsis button should reappear after blur.");
            }, TimeSpan.FromSeconds(1));

            selectedChangedCalled.Should().BeFalse("SelectedChanged should not be called on blur.");
            selectedPage.Should().Be(initialSelectedPage, "Page selection should not change on blur.");

            ellipsisButton = comp.FindAll(".mud-pagination-ellipsis-button").Last();
            await ellipsisButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
            comp.WaitForState(() => comp.FindAll(".mud-pagination-ellipsis-input input").Any(), TimeSpan.FromSeconds(1));


            // Find the input element
            inputElement = comp.Find(".mud-pagination-ellipsis-input input");
            inputElement.Should().NotBeNull("Input element should be present after clicking ellipsis.");

            // Simulate blur on the input element
            await inputElement.BlurAsync(new Microsoft.AspNetCore.Components.Web.FocusEventArgs());

            // After blur, the input should be gone, and the button should reappear.
            comp.WaitForAssertion(() =>
            {
                comp.FindAll(".mud-pagination-ellipsis-input input").Should().BeEmpty("Input should disappear after blur.");
                comp.FindAll(".mud-pagination-ellipsis-button").Should().NotBeEmpty("Ellipsis button should reappear after blur.");
            }, TimeSpan.FromSeconds(1));
        }
    }
}
