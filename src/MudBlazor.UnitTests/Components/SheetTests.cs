// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
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

            // ensure popover contains base class
            var prop = typeof(MudSheet).GetProperty("PopoverClassname", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.Should().NotBeNull();
            var popoverClass = prop!.GetValue(comp.Instance) as string;
            popoverClass.Should().Contain("mud-sheet-popover");

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
                .Add(p => p.OpenChanged, value => openCallback = value)
                .Add(p => p.CurrentSizeChanged, value => currentSizeCallback = true)
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
        public async Task Sheet_TestDragging_Full_WithSnapMode()
        {
            var jsInterop = Context.JSInterop;

            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.EnableDragToSize, true)
                .Add(p => p.CurrentSize, 25)
                .Add(p => p.SnapMode, true)
                .Add(p => p.PresetSizes, [25, 50, 75, 100]));

            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));

            // make sure the component is initialized and handle is rendered
            var handleRef = comp.Instance._handleRef;
            handleRef.Should().NotBe(default(ElementReference));

            // get the handle
            var handle = provider.Find(".mud-sheet-handle");
            handle.Should().NotBeNull();

            // simulate a drag start
            var pointer = new PointerEventArgs
            {
                Button = 0,
                ClientX = 200,
                ClientY = 360,
                PointerId = 1,
                PointerType = "mouse"
            };
            // double[] is width and height in that order
            var dragStartMock = jsInterop
                .Setup<double[]>("window.mudsheetHelper.startDrag", handleRef, pointer.PointerId)
                .SetResult([640, 480]); // width, then height

            await handle.TriggerEventAsync("onpointerdown", pointer);

            // verify the drag start was called
            dragStartMock.VerifyInvoke("window.mudsheetHelper.startDrag").Arguments.Should().BeEquivalentTo(new object[] { handleRef, pointer.PointerId });

            // verify the ViewPortSize struct was passed correctly
            var viewSize = comp.Instance._viewportSize;
            viewSize.Should().NotBeNull();
            viewSize.Value.Width.Should().Be(640);
            viewSize.Value.Height.Should().Be(480);

            // verify the DragPoints struct was initialized and set properly
            var dragPoints = comp.Instance._points;
            dragPoints.Should().NotBeNull();
            dragPoints.Value.XDown.Should().Be(200);
            dragPoints.Value.YDown.Should().Be(360);
            dragPoints.Value.StartSize.Should().Be(25);

            // verify drag mode enabled
            comp.Instance._dragging.Should().BeTrue();

            // setup the mock to test the throttle before we trigger the first move
            var throttleCheckPoint = new PointerEventArgs
            {
                Button = 0,
                ClientX = 205,
                ClientY = 200, // not quite 60% but a tad over 60% of the height
                PointerId = 1,
                PointerType = "mouse"
            };
            // 25 + ((dragPoints.Value.YDown - 200) / viewSize.Value.Height * 100);
            // 25 + ((360 - 200) / 480 * 100) = 25 + (160 / 480 * 100) = 25 + 33.3333 = 58.3333
            var newHeight = dragPoints.Value.StartSize + ((dragPoints.Value.YDown - 200) / viewSize.Value.Height * 100);
            newHeight.Should().BeApproximately(58.3333, 0.001);

            // trigger the first drag move
            // get the updated height by method, should "SnapMode" to 50% since it's closer than 75%
            // start size is 25 + the result of ydown (360) minus the new y position (235) or 125 / the total height 480 or -.26041 * 100 or -26.041 + 25
            newHeight = dragPoints.Value.StartSize + ((dragPoints.Value.YDown - 235) / viewSize.Value.Height * 100);
            newHeight.Should().BeApproximately(51.041, 0.001);
            await handle.TriggerEventAsync("onpointermove", new PointerEventArgs
            {
                Button = 0,
                ClientX = 205,
                ClientY = 235, // not quite 60% but a tad over 60% of the height
                PointerId = 1,
                PointerType = "mouse"
            });

            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(51);

            // 2nd drag move no wait
            await handle.TriggerEventAsync("onpointermove", throttleCheckPoint);
            // move will fail due to throttle of PointerMoveThrottle (16ms) and stay at 51 instead of the new 58
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(51);

            // ensure time elapsed for throttle
            await Task.Delay(20);
            // 3nd drag move with wait
            await handle.TriggerEventAsync("onpointermove", throttleCheckPoint);
            // new value of 58
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(58);

            // finish the drag at 51% with snap mode enabled
            await handle.TriggerEventAsync("onpointerup", new PointerEventArgs
            {
                Button = 0,
                ClientX = 205,
                ClientY = 235, // not quite 60% but a tad over 60% of the height
                PointerId = 1,
                PointerType = "mouse"
            });

            // we already know the math (same as above) so we can just verify the final size
            // which will be 50 since snap mode is enabled
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(50);

            // verify the drag stop was called
            Context.JSInterop.VerifyInvoke("window.mudsheetHelper.cancelDrag")
                .Arguments.Should().BeEquivalentTo(new object[] { handleRef, pointer.PointerId });
            comp.Instance._points.Should().BeNull();
            comp.Instance._dragging.Should().BeFalse();
            comp.Instance._viewportSize.Should().BeNull();
            await comp.Instance.DisposeAsync();
        }

        [Test]
        public async Task Sheet_TestDragging_NoSnapMode()
        {
            var jsInterop = Context.JSInterop;
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.EnableDragToSize, true)
                .Add(p => p.CurrentSize, 25)
                .Add(p => p.SnapMode, false)
                .Add(p => p.PresetSizes, [25, 50, 75, 100]));

            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));

            // get the handle
            var handle = provider.Find(".mud-sheet-handle");
            handle.Should().NotBeNull();

            var handleRef = comp.Instance._handleRef;

            // start drag again to verify no snap mode
            // simulate a drag start
            var pointer = new PointerEventArgs
            {
                Button = 0,
                ClientX = 200,
                ClientY = 360,
                PointerId = 1,
                PointerType = "mouse"
            };

            // double[] is width and height in that order
            jsInterop
                .Setup<double[]>("window.mudsheetHelper.startDrag", handleRef, pointer.PointerId)
                .SetResult([640, 480]); // width, then height

            await handle.TriggerEventAsync("onpointerdown", pointer);
            comp.Instance._dragging.Should().BeTrue();

            // trigger a drag move
            await handle.TriggerEventAsync("onpointermove", new PointerEventArgs
            {
                Button = 0,
                ClientX = 205,
                ClientY = 235, // not quite 60% but a tad over 60% of the height
                PointerId = 1,
                PointerType = "mouse"
            });

            // same math as previous method, "move" doesn't snap so it should be 51
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(51);

            // finish the drag at 58% with snap mode disabled, same math as previous method
            await handle.TriggerEventAsync("onpointerup", new PointerEventArgs
            {
                Button = 0,
                ClientX = 205,
                ClientY = 200, // not quite 60%
                PointerId = 1,
                PointerType = "mouse"
            });

            // verify no snapmode and drops any deciaml points (straight int cast)
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(58);
            await comp.Instance.DisposeAsync();
        }

        [Test]
        public async Task Sheet_EnableDrag_PreventsDrag()
        {
            var jsInterop = Context.JSInterop;
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.EnableDragToSize, false));

            comp.Instance.Should().NotBeNull();

            // open the sheet
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.WaitForAssertion(() => provider.FindAll(".mud-sheet-container").Count.Should().Be(1));

            var handle = provider.Find(".mud-sheet-handle");

            // start drag again to verify no snap mode
            // simulate a drag start
            var pointer = new PointerEventArgs
            {
                Button = 0,
                ClientX = 200,
                ClientY = 360,
                PointerId = 1,
                PointerType = "mouse"
            };

            await handle.TriggerEventAsync("onpointerdown", pointer);
            comp.Instance._dragging.Should().BeFalse();

            await comp.Instance.DisposeAsync();
        }

        [Test]
        public async Task Sheet_TestToggleSize()
        {
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.EnableDragToSize, true)
                .Add(p => p.Open, true)
                .Add(p => p.CurrentSize, 25)
                .Add(p => p.PresetSizes, []));
            comp.Instance.Should().NotBeNull();
            var sheet = comp.Instance;

            // verify sheet started open
            sheet.GetState<bool>(nameof(sheet.Open)).Should().BeTrue();

            // toggle with no sizes should close the sheet
            await comp.InvokeAsync(async () => await sheet.ToggleSizeAsync());
            sheet.GetState<bool>(nameof(sheet.Open)).Should().BeFalse();

            comp.SetParametersAndRender(p => p
                .Add(p => p.PresetSizes, [25, 50, 75, 100]));

            await comp.InvokeAsync(async () => await sheet.OpenSheetAsync());
            sheet.GetState<bool>(nameof(sheet.Open)).Should().BeTrue();

            // verify current size is 25%
            sheet.GetState<int>(nameof(sheet.CurrentSize)).Should().Be(25);
            // toggle size should change to 50% (next in preset sizes)
            await comp.InvokeAsync(async () => await sheet.ToggleSizeAsync());
            sheet.GetState<int>(nameof(sheet.CurrentSize)).Should().Be(50);

            await comp.Instance.DisposeAsync();
        }

        [Test]
        public async Task Sheet_TestDispose()
        {
            // make sure it disposes correctly
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.Open, true));
            comp.Instance.Should().NotBeNull();
            var sheet = comp.Instance;

            // reflection to update IsJsRuntimeAvailable
            var prop = typeof(MudComponentBase).GetProperty("IsJSRuntimeAvailable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            prop!.SetValue(comp.Instance, false);
            sheet.JSRuntimeReady.Should().BeFalse();
            sheet._dragging = true;

            // won't run because JSRuntimeReady is false
            await comp.Instance.DisposeAsync();

            prop!.SetValue(comp.Instance, true);
            sheet._dragging = false;

            // won't run because _dragging is false but sets IsJsRuntimeAvailable to false either way
            sheet.JSRuntimeReady.Should().BeTrue();
            await comp.Instance.DisposeAsync();
            sheet.JSRuntimeReady.Should().BeFalse();

            sheet._dragging = true;
            prop!.SetValue(comp.Instance, true);
            sheet.JSRuntimeReady.Should().BeTrue();

            // should run now
            await comp.Instance.DisposeAsync();

            // verify the calls can't run anymore
            sheet.JSRuntimeReady.Should().BeFalse();

            // verify it won't run twice
            await comp.Instance.DisposeAsync();

            // verify the JS interop was called to cancel the drag but only once even though we disposed multiple times
            Context.JSInterop.VerifyInvoke("window.mudsheetHelper.cancelDrag");
        }

        [Test]
        public void Sheet_OnOpenChanged_And_OnCurrentSizeChanged_Handlers()
        {
            var comp = Context.RenderComponent<MudSheet>();
            // Open should be false initially
            comp.Instance.GetState<bool>(nameof(comp.Instance.Open)).Should().BeFalse();

            // Change Open to true via SetParametersAndRender (triggers OnOpenChanged)
            comp.SetParametersAndRender(p => p.Add(p => p.Open, true));
            comp.Instance.GetState<bool>(nameof(comp.Instance.Open)).Should().BeTrue();

            // Change CurrentSize via SetParametersAndRender (triggers OnCurrentSizeChanged)
            comp.SetParametersAndRender(p => p.Add(p => p.CurrentSize, 42));
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(42);
        }

        [Test]
        public async Task Sheet_HandleKeyDownAsync_Closes_On_Escape()
        {
            var provider = Context.RenderComponent<MudPopoverProvider>();
            var comp = Context.RenderComponent<MudSheet>(p => p.Add(p => p.Open, true));
            comp.Instance.GetState<bool>(nameof(comp.Instance.Open)).Should().BeTrue();

            // Simulate Escape keydown
            var sheetDiv = provider.Find(".mud-sheet-container");
            await sheetDiv.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" });

            // Should be closed
            comp.Instance.GetState<bool>(nameof(comp.Instance.Open)).Should().BeFalse();

            // reopen
            comp.SetParametersAndRender(p => p.Add(p => p.CloseOnEscapeKey, false));
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.Instance.GetState<bool>(nameof(comp.Instance.Open)).Should().BeTrue();

            // simulate escape keydown again
            sheetDiv = provider.Find(".mud-sheet-container");
            await sheetDiv.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Escape" });
            // Should still be open since CloseOnEscapeKey is false
            comp.Instance.GetState<bool>(nameof(comp.Instance.Open)).Should().BeTrue();
        }

        [Test]
        public async Task Sheet_OnAfterRender_Triggers_ReRender_When_UpdateState()
        {
            var comp = Context.RenderComponent<MudSheet>();
            // Use reflection to set _updateState to true
            var field = typeof(MudSheet).GetField("_updateState", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(comp.Instance, true);

            // Track render count
            int renderCount = 0;
            comp.OnAfterRender += (_, _) => renderCount++;

            // Trigger OnAfterRender
            await comp.InvokeAsync(() => comp.Instance.GetType()
                .GetMethod("OnAfterRender", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(comp.Instance, new object[] { false }));

            // Should have re-rendered at least once
            renderCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Sheet_CanDrag()
        {
            var comp = Context.RenderComponent<MudSheet>();

            var prop = typeof(MudSheet).GetProperty("CanDrag", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            // _dragging starts false, _openState.Value starts false, 
            // _points is null, and _viewportSize is null, all of these have to flip for CanDrag to return true.

            // with sheet open , CanDrag should still be false
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            prop!.GetValue(comp.Instance).Should().Be(false);

            // with _dragging set to true, CanDrag should still be false
            comp.Instance._dragging = true;
            prop!.GetValue(comp.Instance).Should().Be(false);

            // with _points set to a valid value, CanDrag should still be false
            comp.Instance._points = new MudSheet.DragPoints(0, 0, 0);
            prop!.GetValue(comp.Instance).Should().Be(false);

            // test methods that rely on CanDrag
            var method1 = typeof(MudSheet).GetMethod("HandlePointerMoveAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var method2 = typeof(MudSheet).GetMethod("HandlePointerCancelAsync", BindingFlags.NonPublic | BindingFlags.Instance);

            // these methods should not throw when CanDrag is false and just return a Task
            var task = method1!.Invoke(comp.Instance, new object[] { new PointerEventArgs() });
            task.Should().Be(Task.CompletedTask);

            // with _viewportSize set to a valid value, CanDrag should finally be true
            comp.Instance._viewportSize = new MudSheet.ViewPortSize(640, 480);
            prop!.GetValue(comp.Instance).Should().Be(true);
        }

        [Test]
        public async Task Sheet_SetParametersAsync_Clamps_On_Invalid_CurrentSize()
        {
            var comp = Context.RenderComponent<MudSheet>();
            // Try to set CurrentSize to an invalid value (<0)
            comp.SetParametersAndRender(p => p.Add(p => p.CurrentSize, -1));
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(0);
            await comp.InvokeAsync(async () => await comp.Instance.CloseSheetAsync());
            // Try to set CurrentSize to an invalid value (>100)
            comp.SetParametersAndRender(p => p.Add(p => p.CurrentSize, 101));
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(100);
        }

        [Test]
        public async Task Sheet_HandlePointerCancelAsync_ResetsDraggingState()
        {
            var comp = Context.RenderComponent<MudSheet>(p => p.Add(p => p.EnableDragToSize, true));
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());

            // Set up drag state
            comp.Instance._dragging = true;
            comp.Instance._points = new MudSheet.DragPoints(10, 20, 30);
            comp.Instance._viewportSize = new MudSheet.ViewPortSize(100, 100);

            var pointerArgs = new PointerEventArgs { PointerId = 1 };

            // Use reflection to invoke private method
            var method = typeof(MudSheet).GetMethod("HandlePointerCancelAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task)method!.Invoke(comp.Instance, new object[] { pointerArgs })!;
            await task;

            // After cancel, dragging and points should be reset
            comp.Instance._dragging.Should().BeFalse();
            comp.Instance._points.Should().BeNull();
            comp.Instance._viewportSize.Should().BeNull();
        }

        [TestCase(100, 100, 100, 120, 50, 70, Position.Top)]     // Top: delta = 20, height = 100
        [TestCase(100, 100, 120, 100, 50, 70, Position.Left)]    // Left: delta = 20, width = 100
        [TestCase(120, 100, 100, 100, 50, 70, Position.Right)]   // Right: delta = 20, width = 100
        [TestCase(100, 100, 100, 120, 50, 10, Position.Center)]  // Center: delta = -20, height/2 = 50, 50 - (-20 / 50 * 100)
        [TestCase(100, 100, 100, 120, 50, 50, Position.Right)] // An else case to test the math
        [Test]
        public async Task Sheet_PerformPointerDrag_Math(double startX, double startY, double currentX, double currentY, int baseSize, int expected, Position pos)
        {
            var comp = Context.RenderComponent<MudSheet>(p => p
                .Add(p => p.SnapMode, false)
                .Add(p => p.Position, pos));
            await comp.InvokeAsync(async () => await comp.Instance.OpenSheetAsync());

            // Set up viewport size
            comp.Instance._viewportSize = new MudSheet.ViewPortSize(100, 100);

            // Use reflection to invoke private method
            var method = typeof(MudSheet).GetMethod("PerformPointerDrag", BindingFlags.NonPublic | BindingFlags.Instance);

            // Call PerformPointerDrag
            var task = (Task)method!.Invoke(comp.Instance, new object[] { startX, startY, currentX, currentY, baseSize })!;
            await task;

            // Check the new CurrentSize
            comp.Instance.GetState<int>(nameof(comp.Instance.CurrentSize)).Should().Be(expected);
        }
    }
}
