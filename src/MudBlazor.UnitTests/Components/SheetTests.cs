// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Sheet;
using NUnit.Framework;
#nullable enable
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SheetTests : BunitTest
    {
        [TestCase(Position.Bottom, "bottom")]
        [TestCase(Position.Center, "center")]
        [TestCase(Position.Top, "top")]
        [TestCase(Position.Left, "left")]
        [TestCase(Position.Right, "right")]
        [TestCase(Position.Start, "left")]
        [TestCase(Position.End, "right")]
        [Test]
        public async Task Sheet_ShouldRenderCorrectly(Position pos, string result)
        {
            var textContent = "Example content";
            var exampleContent = @$"<MudText>{textContent}</MudText>";
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.Position, pos)
                .Add(p => p.ChildContent, exampleContent));
            comp.Instance.Should().NotBeNull();
            provider.Instance.Should().NotBeNull();
            // open the sheet
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            // container should be rendered without manual re-render
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));
            provider.Find($"#{comp.Instance.ElementId}").Should().NotBeNull();
            // position should match
            provider.Find($".mud-sheet-position-{result}").Should().NotBeNull();
            // handle should be rendered
            provider.Find($".mud-sheet-container .mud-sheet-handle").Should().NotBeNull();
            // button inside handle should be rendered
            provider.Find($".mud-sheet-container .mud-sheet-handle .mud-sheet-handle-button").Should().NotBeNull();
            // content should be rendered
            var content = provider.Find(".mud-sheet-container .mud-sheet-content");
            content.TextContent.Should().Be(textContent);

            // close the sheet
            await comp.InvokeAsync(async () => await comp.Instance.CloseSheetAsync());
            // container should be removed without manual re-render
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(0));
        }

        [TestCase(Position.Start, "right")]
        [TestCase(Position.End, "left")]
        [Test]
        public async Task Sheet_ShouldUpdateRtlPositions(Position pos, string result)
        {
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.Position, pos)
                .Add(p => p.RightToLeft, true));
            comp.Instance.Should().NotBeNull();
            provider.Instance.Should().NotBeNull();
            // open the sheet
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            // container should be rendered without manual re-render
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));
            provider.Find($"#{comp.Instance.ElementId}").Should().NotBeNull();
            // position should match
            provider.Find($".mud-sheet-position-{result}").Should().NotBeNull();
        }

        [Test]
        public void Sheet_Bindable_Properties()
        {
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<SheetBindTest>();

            // Initial state, not open
            provider.FindAll(".mud-sheet-container").Count.Should().Be(0);

            // test two way binding for open using the open variable
            comp.Find("button.toggle-open").Click();
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));

            // get the sheet default value
            var sheetInstance = comp.Instance.TestSheet;
            sheetInstance.Should().NotBeNull();

            var currentSize = sheetInstance.CurrentSize;
            currentSize.Should().Be(comp.Instance.CurrentSize);

            // change via property
            comp.Instance.CurrentSize = 77;
            comp.Render();
            comp.WaitForAssertion(() => comp.Instance.TestSheet.CurrentSize.Should().Be(77));
            provider.Find("div[mudsheet].mud-popover").GetAttribute("style").Should().Contain("height:77vh");

            // test two way binding for close using the open variable
            comp.Find("button.toggle-open").Click();
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(0));
        }

        private static Dictionary<string, object?> GetAriaAttributes(bool standard)
        {
            if (standard)
            {
                return new Dictionary<string, object?>
                {
                    { "role", "region" },
                    { "tabindex", -1 },
                    { "aria-label", "Bottom Sheet" }
                };
            }
            else
            {
                return new Dictionary<string, object?>
                {
                    { "role", "dialog" },
                    { "tabindex", -1 },
                    { "aria-modal", "true" },
                    { "aria-label", "Bottom Sheet" }
                };
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public async Task Sheet_TestAccessibility(bool standard)
        {
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.Standard, standard)
                .Add(p => p.Position, Position.Bottom));
            comp.Instance.Should().NotBeNull();
            provider.Instance.Should().NotBeNull();

            // open the sheet
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));

            // check accessibility dictionaries to match
            var expectedAttributes = GetAriaAttributes(standard);
            comp.Instance.AriaAttributes.Should().BeEquivalentTo(expectedAttributes);

            // verify overrides work, both AriaLabel and UserAttributes
            comp.Instance.UserAttributes.Add("role", "norole");
            comp.SetParametersAndRender(p => p
                .Add(p => p.AriaLabel, "Test To Test"));

            comp.WaitForAssertion(() => comp.Instance.AriaAttributes["aria-label"].Should().Be("Test To Test"));
            comp.Instance.UpdatedAttributes["role"].Should().Be("norole");
            comp.Instance.UpdatedAttributes["aria-label"].Should().Be("Test To Test");

            // verify controls matches
            var id = comp.Instance.ElementId;
            var sheetControls = provider.Find($"div[id='{id}'] button.mud-sheet-handle-button");
            sheetControls.GetAttribute("aria-controls").Should().Be(id);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public async Task Sheet_TestStandard_Modal(bool standard)
        {
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.Standard, standard)
                .Add(p => p.Position, Position.Bottom));
            comp.Instance.Should().NotBeNull();
            provider.Instance.Should().NotBeNull();

            // open the sheet
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));

            // overlay
            provider.FindAll($"div.mud-overlay.mud-{comp.Instance.ElementId}").Count.Should().Be(standard ? 0 : 1);

            // Focus Trap
            var trap = provider.FindComponent<MudFocusTrap>();
            trap.Should().NotBeNull();
            trap.Instance.DefaultFocus.Should()
                .Be(standard ? DefaultFocus.None : DefaultFocus.FirstChild);

            // Whether CoverAppBar unset is rendered
            provider.FindAll(".mud-sheet-cover-appbar").Count.Should().Be(standard ? 0 : 1);
        }

        [Test]
        public void Sheet_Default_Parameters()
        {
            var comp = Context.RenderComponent<MudSheet>();
            comp.Instance.Should().NotBeNull();
            var sheet = comp.Instance;
            sheet.Standard.Should().BeTrue();
            sheet.RightToLeft.Should().BeFalse();
            sheet.Paper.Should().BeTrue();
            sheet.Elevation.Should().Be(16);
            sheet.Position.Should().Be(Position.Bottom);
            sheet.BorderRadius.Should().Be(16);
            sheet.CoverAppBar.Should().BeNull();
            sheet.VerticalHandle.Should().Be(Icons.Material.Filled.DragHandle);
            sheet.HorizontalHandle.Should().Be(Icons.Material.Filled.DragIndicator);
            sheet.Open.Should().BeFalse();
            sheet.CurrentSize.Should().Be(50);
            sheet.ChildContent.Should().BeNull();
            sheet.SheetHandleFragment.Should().BeNull();
            sheet.AriaLabel.Should().BeNull();
            sheet.EnableDragToSize.Should().BeTrue();
            sheet.PresetSizes.Should().BeEquivalentTo([25, 50, 75, 100]);
            sheet.SnapMode.Should().Be(false);
            sheet.CloseOnEscapeKey.Should().BeTrue();
        }

        [Test]
        public async Task Sheet_EventCallbacks()
        {
            var openCallback = false;
            var currentSizeCallback = false;
            var onDismissedCallback = false;
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.OpenChanged, (bool value) => openCallback = value)
                .Add(p => p.CurrentSizeChanged, (int _) => currentSizeCallback = true)
                .Add(p => p.OnDismissed, () => { onDismissedCallback = true; }));
            comp.Instance.Should().NotBeNull();

            var sheet = comp.Instance;

            await sheet.OpenChanged.InvokeAsync(true);
            openCallback.Should().BeTrue();
            openCallback = false;
            await comp.InvokeAsync(async () => await sheet.OpenSheetAsync());
            openCallback.Should().BeTrue();

            await sheet.CurrentSizeChanged.InvokeAsync(100);
            currentSizeCallback.Should().BeTrue();
            currentSizeCallback = false;
            await comp.InvokeAsync(async () => await sheet.ToggleSizeAsync());
            currentSizeCallback.Should().BeTrue();
            currentSizeCallback = false;
            await comp.InvokeAsync(async () => await sheet.ChangeSize(75));
            currentSizeCallback.Should().BeTrue();

            await sheet.OnDismissed.InvokeAsync();
            onDismissedCallback.Should().BeTrue();
            onDismissedCallback = false;
            await comp.InvokeAsync(async () => await sheet.CloseSheetAsync());
            onDismissedCallback.Should().BeTrue();
        }

        [Test]
        private void Sheet_TestDragging()
        {
            // lots of js
        }

        [Test]
        private void Sheet_TestToggleSize()
        {
            // cycling through preset sizes
        }

        [Test]
        private void Sheet_TestDispose()
        {
            // make sure it disposes correctly
        }
    }
}
