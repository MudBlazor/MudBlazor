// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    public class EventListenerTests
    {
        private Mock<IJSRuntime> _runtimeMock;
        private EventListener _service;

        [SetUp]
        public void SetUp()
        {
            _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
            _service = new EventListener(_runtimeMock.Object);
        }

        private bool ContainsEqual(IEnumerable<String> firstColl, IEnumerable<string> secondColl)
        {
            try
            {
                CollectionAssert.AreEqual(firstColl, secondColl);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Test]
        public async Task Subscribe()
        {
            string eventName = "onMyCustomEvent";
            string elementId = "my-customer-dom-element";
            int throttleInterval = 20;
            string projectionName = "mynamespace.myfunction";

            Func<Object, Task> callback = (x) => Task.Delay(10);

            string[] expectedProperties = new[] {
             "detail", "screenX", "screenY", "clientX", "clientY", "offsetX", "offsetY",
             "button", "buttons", "ctrlKey", "shiftKey", "altKey", "metaKey", "type"
            };

            _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                    (string)z[0] == eventName &&
                    (string)z[1] == elementId &&
                    (string)z[2] == projectionName &&
                    (int)z[3] == throttleInterval &&
                    (Guid)z[4] != Guid.Empty &&
                    ContainsEqual((IEnumerable<string>)z[5], expectedProperties) == true &&
                    z[6] is DotNetObjectReference<EventListener>
                ))).ReturnsAsync(true);

            Guid result = await _service.Subscribe<MouseEventArgs>(eventName, elementId, projectionName, throttleInterval, callback);

            result.Should().NotBe(Guid.Empty);

            _runtimeMock.Verify(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                (Guid)z[4] == result
            )));
        }

        [Test]
        public async Task Subscribe_AndCallback()
        {
            string eventName = "onMyCustomEvent";
            string elementId = "my-customer-dom-element";
            string projectionName = "mynamespace.myfunction";

            int throttleInterval = 20;

            bool callbackCalled = false;

            double offsetX = 200.24;
            double offsetY = 12425.2;

            Func<Object, Task> callback = (x) =>
            {
                try
                {
                    x.Should().BeAssignableTo<MouseEventArgs>();
                    MouseEventArgs args = (MouseEventArgs)x;
                    args.OffsetX.Should().Be(offsetX);
                    args.OffsetY.Should().Be(offsetY);

                    callbackCalled = true;
                }
                catch (Exception)
                {
                    callbackCalled = false;
                }

                return Task.CompletedTask;
            };

            string[] expectedProperties = new[] {
             "detail", "screenX", "screenY", "clientX", "clientY", "offsetX", "offsetY",
             "button", "buttons", "ctrlKey", "shiftKey", "altKey", "metaKey", "type"
            };

            _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                    (string)z[0] == eventName &&
                    (string)z[1] == elementId &&
                    (string)z[2] == projectionName &&
                    (int)z[3] == throttleInterval &&
                    (Guid)z[4] != Guid.Empty &&
                    ContainsEqual((IEnumerable<string>)z[5], expectedProperties) == true &&
                    z[6] is DotNetObjectReference<EventListener>
                ))).ReturnsAsync(true);

            Guid result = await _service.Subscribe<MouseEventArgs>(eventName, elementId, projectionName, throttleInterval, callback);

            result.Should().NotBe(Guid.Empty);

            await _service.OnEventOccur(result, System.Text.Json.JsonSerializer.Serialize(new
            {
                offsetX = offsetX,
                offsetY = offsetY,
            }));

            callbackCalled.Should().BeTrue();
        }

        [Test]
        public async Task Unsubscribe_KeyNotFound()
        {
            bool result = await _service.Unsubscribe(Guid.NewGuid());
            result.Should().BeFalse();
        }

        [Test]
        public async Task Subscribe_AndUnsubscribe()
        {
            string eventName = "onMyCustomEvent";
            string elementId = "my-customer-dom-element";
            int throttleInterval = 20;
            string projectionName = null;

            Func<Object, Task> callback = (x) => Task.Delay(10);

            string[] expectedProperties = new[] {
             "detail", "screenX", "screenY", "clientX", "clientY", "offsetX", "offsetY",
             "button", "buttons", "ctrlKey", "shiftKey", "altKey", "metaKey", "type"
            };

            _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                    z.Length == 7 &&
                    (string)z[0] == eventName &&
                    (string)z[1] == elementId &&
                    (string)z[2] == projectionName &&
                    (int)z[3] == throttleInterval &&
                    (Guid)z[4] != Guid.Empty &&
                    ContainsEqual((IEnumerable<string>)z[5], expectedProperties) == true &&
                    z[6] is DotNetObjectReference<EventListener>
                ))).ReturnsAsync(true);


            Guid result = await _service.Subscribe<MouseEventArgs>(eventName, elementId, projectionName, throttleInterval, callback);

            _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.unsubscribe", It.Is<object[]>(z =>
                z.Length == 1 &&
                (Guid)z[0] == result
            ))).ReturnsAsync(true);

            result.Should().NotBe(Guid.Empty);

            bool unsubscribeResult = await _service.Unsubscribe(result);
            unsubscribeResult.Should().BeTrue();
        }

        [Test]
        public async Task Subscribe_AndUnsubscribe_WithError()
        {
            string eventName = "onMyCustomEvent";
            string elementId = "my-customer-dom-element";
            int throttleInterval = 20;
            string projectionName = "mynamspace.something.somethingelse";

            Func<Object, Task> callback = (x) => Task.Delay(10);

            string[] expectedProperties = new[] {
             "detail", "screenX", "screenY", "clientX", "clientY", "offsetX", "offsetY",
             "button", "buttons", "ctrlKey", "shiftKey", "altKey", "metaKey", "type"
            };

            _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                    z.Length == 7 &&
                    (string)z[0] == eventName &&
                    (string)z[1] == elementId &&
                    (string)z[2] == projectionName &&
                    (int)z[3] == throttleInterval &&
                    (Guid)z[4] != Guid.Empty &&
                    ContainsEqual((IEnumerable<string>)z[5], expectedProperties) == true &&
                    z[6] is DotNetObjectReference<EventListener>
                ))).ReturnsAsync(true);


            Guid result = await _service.Subscribe<MouseEventArgs>(eventName, elementId, projectionName, throttleInterval, callback);

            _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.unsubscribe", It.Is<object[]>(z =>
                z.Length == 1 &&
                (Guid)z[0] == result
            ))).Throws(new InvalidOperationException("something went wrong! :("));

            result.Should().NotBe(Guid.Empty);

            bool unsubscribeResult = await _service.Unsubscribe(result);
            unsubscribeResult.Should().BeFalse();
        }


        [Test]
        public async Task DisposeAsync()
        {
            string eventName = "onMyCustomEvent";
            int throttleInterval = 20;
            string projectionName = "mynamspace.something.somethingelse";

            Func<Object, Task> callback = (x) => Task.Delay(10);

            string[] expectedProperties = new[] {
             "detail", "screenX", "screenY", "clientX", "clientY", "offsetX", "offsetY",
             "button", "buttons", "ctrlKey", "shiftKey", "altKey", "metaKey", "type"
            };

            for (int i = 0; i < 10; i++)
            {
                string elementId = $"my-customer-dom-element-{i}";

                _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                        z.Length == 7 &&
                        (string)z[0] == eventName &&
                        (string)z[1] == elementId &&
                        (string)z[2] == projectionName &&
                        (int)z[3] == throttleInterval &&
                        (Guid)z[4] != Guid.Empty &&
                        ContainsEqual((IEnumerable<string>)z[5], expectedProperties) == true &&
                        z[6] is DotNetObjectReference<EventListener>
                    ))).ReturnsAsync(true);


                Guid result = await _service.Subscribe<MouseEventArgs>(eventName, elementId, projectionName, throttleInterval, callback);

                var flow = _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.unsubscribe", It.Is<object[]>(z =>
                        z.Length == 1 &&
                        (Guid)z[0] == result
                    )));

                if (i % 2 == 0)
                {
                    flow.Throws(new InvalidOperationException("something went wrong! :("));
                }
                else
                {
                    flow.ReturnsAsync(true);
                }
            }

            await _service.DisposeAsync();

            // a second time shouldn't change something
            await _service.DisposeAsync();

            // a normal dispose shouldnt' change something
            _service.Dispose();

            _runtimeMock.Verify(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                true
            )), Times.Exactly(10));
        }

        [Test]
        public async Task Dispose()
        {
            string eventName = "onMyCustomEvent";
            int throttleInterval = 20;
            string projectionName = "mynamspace.something.somethingelse";

            Func<Object, Task> callback = (x) => Task.Delay(10);

            string[] expectedProperties = new[] {
             "detail", "screenX", "screenY", "clientX", "clientY", "offsetX", "offsetY",
             "button", "buttons", "ctrlKey", "shiftKey", "altKey", "metaKey", "type"
            };

            for (int i = 0; i < 10; i++)
            {
                string elementId = $"my-customer-dom-element-{i}";

                _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                        z.Length == 7 &&
                        (string)z[0] == eventName &&
                        (string)z[1] == elementId &&
                        (string)z[2] == projectionName &&
                        (int)z[3] == throttleInterval &&
                        (Guid)z[4] != Guid.Empty &&
                        ContainsEqual((IEnumerable<string>)z[5], expectedProperties) == true &&
                        z[6] is DotNetObjectReference<EventListener>
                    ))).ReturnsAsync(true);


                Guid result = await _service.Subscribe<MouseEventArgs>(eventName, elementId, projectionName, throttleInterval, callback);

                var flow = _runtimeMock.Setup(x => x.InvokeAsync<Object>("mudThrottledEventManager.unsubscribe", It.Is<object[]>(z =>
                        z.Length == 1 &&
                        (Guid)z[0] == result
                    )));

                if (i % 2 == 0)
                {
                    flow.Throws(new InvalidOperationException("something went wrong! :("));
                }
                else
                {
                    flow.ReturnsAsync(true);
                }
            }

            _service.Dispose();

            // a second time shouldn't change something
            _service.Dispose();

            _runtimeMock.Verify(x => x.InvokeAsync<Object>("mudThrottledEventManager.subscribe", It.Is<object[]>(z =>
                true
            )), Times.Exactly(10));
        }
    }
}
