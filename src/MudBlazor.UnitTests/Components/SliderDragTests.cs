using AngleSharp.Dom;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

#nullable enable
namespace MudBlazor.UnitTests.Components
{
    /// <summary>
    /// Verifies the <see cref="MudSlider{T}.OnDragStart"/> and <see cref="MudSlider{T}.OnDragEnd"/> callback contract for complete pointer gestures.
    /// </summary>
    /// <remarks>
    /// Each gesture is replayed in the order a browser fires its native events.
    /// A browser fires every event, but Blazor only dispatches an event the input has a listener for, so events without a listener are skipped instead of failing the test.
    /// </remarks>
    [TestFixture]
    public class SliderDragTests : BunitTest
    {
        private const string SliderInput = ".mud-slider-input";
        private readonly List<string> _callbacks = [];
        private int _value = 20;

        /// <summary>
        /// A primary (left) button press.
        /// </summary>
        private static PointerEventArgs PrimaryPress => new() { PointerId = 1, PointerType = "mouse", IsPrimary = true, Button = 0, Buttons = 1 };

        /// <summary>
        /// A secondary (right) button press.
        /// </summary>
        private static PointerEventArgs SecondaryPress => new() { PointerId = 1, PointerType = "mouse", IsPrimary = true, Button = 2, Buttons = 2 };

        /// <summary>
        /// Releases <paramref name="button"/> with no buttons left pressed.
        /// </summary>
        private static PointerEventArgs Release(int button) => new() { PointerId = 1, PointerType = "mouse", IsPrimary = true, Button = button, Buttons = 0 };

        /// <summary>
        /// Records a callback in the order it was raised and writes it to the test output.
        /// </summary>
        private void Record(string callback)
        {
            _callbacks.Add(callback);
            NUnit.Framework.TestContext.Out.WriteLine($"callback: {callback}");
        }

        /// <summary>
        /// Renders a slider from 0 to 100 whose drag callbacks record the bound value at the time they are raised.
        /// </summary>
        private IRenderedComponent<MudSlider<int>> RenderSlider(bool immediate, Func<int, Task>? valueChanged = null)
        {
            return Context.Render<MudSlider<int>>(p =>
            {
                p.Add(x => x.Value, _value);
                p.Add(x => x.Min, 0);
                p.Add(x => x.Max, 100);
                p.Add(x => x.Immediate, immediate);
                p.Add(x => x.ValueChanged, valueChanged ?? (v =>
                {
                    _value = v;
                    Record($"ValueChanged({v})");
                    return Task.CompletedTask;
                }));
                p.Add(x => x.OnDragStart, () => Record($"DragStart({_value})"));
                p.Add(x => x.OnDragEnd, () => Record($"DragEnd({_value})"));
            });
        }

        /// <summary>
        /// Delivers a native <paramref name="eventName"/> event the way Blazor does, dispatching it only when the input has a listener for it.
        /// </summary>
        private Task NativeEventAsync(IRenderedComponent<MudSlider<int>> comp, string eventName, Func<IElement, Task> dispatch)
        {
            var input = comp.Find(SliderInput);
            if (!input.HasAttribute($"blazor:on{eventName}"))
            {
                NUnit.Framework.TestContext.Out.WriteLine($"native: {eventName} (no Blazor listener, not dispatched)");
                return Task.CompletedTask;
            }

            NUnit.Framework.TestContext.Out.WriteLine($"native: {eventName}");
            return dispatch(input);
        }

        private Task PointerDownAsync(IRenderedComponent<MudSlider<int>> comp, PointerEventArgs press) =>
            NativeEventAsync(comp, "pointerdown", input => input.PointerDownAsync(press));

        private Task PointerUpAsync(IRenderedComponent<MudSlider<int>> comp, int button) =>
            NativeEventAsync(comp, "pointerup", input => input.PointerUpAsync(Release(button)));

        private Task InputAsync(IRenderedComponent<MudSlider<int>> comp, string value) =>
            NativeEventAsync(comp, "input", input => input.InputAsync(new ChangeEventArgs { Value = value }));

        private Task ChangeAsync(IRenderedComponent<MudSlider<int>> comp, string value) =>
            NativeEventAsync(comp, "change", input => input.ChangeAsync(new ChangeEventArgs { Value = value }));

        /// <summary>
        /// A deferred drag raises DragStart, then ValueChanged, then DragEnd, and does not end before the value is committed.
        /// </summary>
        [Test]
        public async Task DeferredDrag_RaisesDragEndAfterCommittedValue()
        {
            var comp = RenderSlider(immediate: false);

            await PointerDownAsync(comp, PrimaryPress);
            await InputAsync(comp, "75");
            await PointerUpAsync(comp, 0);
            _callbacks.Should().Equal(["DragStart(20)"], "the drag must not end before the change event commits the value");

            await ChangeAsync(comp, "75");

            _callbacks.Should().Equal("DragStart(20)", "ValueChanged(75)", "DragEnd(75)");
        }

        /// <summary>
        /// A deferred drag raises DragEnd only after an asynchronous ValueChanged handler completes.
        /// </summary>
        [Test]
        [CancelAfter(10000)]
        public async Task DeferredDrag_RaisesDragEndAfterAwaitedValueChanged()
        {
            var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var comp = RenderSlider(immediate: false, async v =>
            {
                Record($"ValueChanged.Begin({v})");
                await gate.Task;
                _value = v;
                Record($"ValueChanged.End({v})");
            });

            await PointerDownAsync(comp, PrimaryPress);
            await InputAsync(comp, "75");
            await PointerUpAsync(comp, 0);
            var change = comp.Find(SliderInput).ChangeAsync(new ChangeEventArgs { Value = "75" });

            change.IsCompleted.Should().BeFalse("ValueChanged is still awaiting the gate");
            _callbacks.Should().Equal(["DragStart(20)", "ValueChanged.Begin(75)"], "DragEnd must not be raised while ValueChanged is pending");

            gate.SetResult();
            await change.WaitAsync(NUnit.Framework.TestContext.CurrentContext.CancellationToken);

            _callbacks.Should().Equal("DragStart(20)", "ValueChanged.Begin(75)", "ValueChanged.End(75)", "DragEnd(75)");
        }

        /// <summary>
        /// Pressing and releasing the thumb without moving it ends the drag.
        /// </summary>
        /// <remarks>
        /// The browser fires no change event when the value did not change, so the release alone must end the drag.
        /// </remarks>
        [TestCase(false)]
        [TestCase(true)]
        public async Task PressAndReleaseWithoutChange_RaisesDragEnd(bool immediate)
        {
            var comp = RenderSlider(immediate);

            await PointerDownAsync(comp, PrimaryPress);
            await PointerUpAsync(comp, 0);

            _callbacks.Should().Equal("DragStart(20)", "DragEnd(20)");
        }

        /// <summary>
        /// A deferred drag that returns to its original value before release ends the drag.
        /// </summary>
        /// <remarks>
        /// The browser fires no change event because the committed value equals the original value.
        /// </remarks>
        [Test]
        public async Task DeferredDragBackToOriginalValue_RaisesDragEnd()
        {
            var comp = RenderSlider(immediate: false);

            await PointerDownAsync(comp, PrimaryPress);
            await InputAsync(comp, "75");
            await InputAsync(comp, "20");
            await PointerUpAsync(comp, 0);

            _callbacks.Should().Equal("DragStart(20)", "DragEnd(20)");
        }

        /// <summary>
        /// After a deferred press and release without a change, the next drag raises its own DragStart and DragEnd.
        /// </summary>
        [Test]
        public async Task DeferredDragAfterUnchangedPress_RaisesOwnDragStart()
        {
            var comp = RenderSlider(immediate: false);

            await PointerDownAsync(comp, PrimaryPress);
            await PointerUpAsync(comp, 0);
            var firstGesture = _callbacks.ToList();

            await PointerDownAsync(comp, PrimaryPress);
            await InputAsync(comp, "75");
            await PointerUpAsync(comp, 0);
            await ChangeAsync(comp, "75");

            using (new AssertionScope())
            {
                firstGesture.Should().Equal(["DragStart(20)", "DragEnd(20)"], "the first gesture must be complete");
                _callbacks.Skip(firstGesture.Count).Should().Equal(["DragStart(20)", "ValueChanged(75)", "DragEnd(75)"], "the second gesture must be paired on its own");
            }
        }

        /// <summary>
        /// Disabling a slider during a deferred drag ends the drag once, after the pending value is committed.
        /// </summary>
        /// <remarks>
        /// The browser still fires the pending change event on the now disabled input, before the pointer is released.
        /// </remarks>
        [Test]
        public async Task DisableDuringDeferredDrag_RaisesSingleDragEndAfterCommittedValue()
        {
            var comp = RenderSlider(immediate: false);

            await PointerDownAsync(comp, PrimaryPress);
            await InputAsync(comp, "75");
            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));
            await ChangeAsync(comp, "75");
            await PointerUpAsync(comp, 0);

            using (new AssertionScope())
            {
                _callbacks.Should().Equal("DragStart(20)", "ValueChanged(75)", "DragEnd(75)");
                _callbacks.Count(c => c.StartsWith("DragEnd", StringComparison.Ordinal)).Should().Be(1);
                _callbacks.Count(c => c.StartsWith("ValueChanged", StringComparison.Ordinal)).Should().Be(1);
            }
        }

        /// <summary>
        /// A secondary button press raises no drag callbacks, and the next primary drag is paired on its own.
        /// </summary>
        /// <remarks>
        /// The browser follows the secondary press with a contextmenu event, which the slider does not listen for.
        /// </remarks>
        [TestCase(false)]
        [TestCase(true)]
        public async Task SecondaryButtonPress_RaisesNoDragCallbacks(bool immediate)
        {
            var comp = RenderSlider(immediate);

            await PointerDownAsync(comp, SecondaryPress);
            await PointerUpAsync(comp, 2);
            var secondaryGesture = _callbacks.ToList();

            await PointerDownAsync(comp, PrimaryPress);
            await InputAsync(comp, "75");
            await PointerUpAsync(comp, 0);
            await ChangeAsync(comp, "75");

            using (new AssertionScope())
            {
                secondaryGesture.Should().BeEmpty("a secondary button press is not a drag");
                _callbacks.Skip(secondaryGesture.Count).Should().Equal(["DragStart(20)", "ValueChanged(75)", "DragEnd(75)"], "the primary drag must be paired on its own");
            }
        }
    }
}
