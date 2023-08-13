// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    [Obsolete]
    public class ResizeBasedServiceTests
    {
        private Mock<IBrowserWindowSizeProvider> _browserWindowSizeProvider;
        private Mock<IJSRuntime> _jsruntimeMock;
        private ResizeService _service;

        [SetUp]
        public void SetUp()
        {
            _browserWindowSizeProvider = new Mock<IBrowserWindowSizeProvider>();
            _jsruntimeMock = new Mock<IJSRuntime>();
            _service = new ResizeService(_jsruntimeMock.Object, _browserWindowSizeProvider.Object);
        }

        [Test]
        public async Task GetBrowserWindowSize()
        {
            var size = new BrowserWindowSize
            {
                Height = 200,
                Width = 200,
            };

            _browserWindowSizeProvider.Setup(x => x.GetBrowserWindowSize()).ReturnsAsync(size);

            var actualSize = await _service.GetBrowserWindowSize();

            actualSize.Should().Be(size);
            _browserWindowSizeProvider.Verify(x => x.GetBrowserWindowSize(), Times.Once());
        }

        private record ListenForResizeCallbackInfo(
            DotNetObjectReference<ResizeService> DotnetRef, ResizeOptions options, Guid ListenerId);


        private void SetupJsMockForSubscription(ResizeOptions expectedOptions, Action<ListenForResizeCallbackInfo> callbackInfo = null)
        {

            _jsruntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize",
               It.Is<object[]>(z =>
                   z[0] is DotNetObjectReference<ResizeService> == true &&
                   (ResizeOptions)z[1] == expectedOptions &&
                   (Guid)z[2] != default
               ))).ReturnsAsync(Mock.Of<IJSVoidResult>).Callback<string, object[]>((x, z) => callbackInfo?.Invoke(new ListenForResizeCallbackInfo(
                    (DotNetObjectReference<ResizeService>)z[0],
                    (ResizeOptions)z[1], (Guid)z[2]
                   ))).Verifiable();

        }

        private void SetupJsMockForUnsubscription(Guid listenerId)
        {
            _jsruntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListener",
               It.Is<object[]>(z =>
                   (Guid)z[0] == listenerId
               ))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();
        }

        private async Task CheckSubscriptionOptions(ResizeOptions expectedOptions)
        {
            SetupJsMockForSubscription(expectedOptions);

            var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { });

            subscriptionId.Should().NotBe(Guid.Empty);

            _jsruntimeMock.Verify();
        }

        [Test]
        public async Task Subscribe_WithDefaultOptions()
        {
            await CheckSubscriptionOptions(new ResizeOptions());
        }

        [Test]
        public async Task Subscribe_WithOptionsSetInConstructor()
        {
            var customResizeOptioons = new ResizeOptions
            {
                ReportRate = 120,
            };

            var optionGetter = new Mock<IOptions<ResizeOptions>>();
            optionGetter.SetupGet(x => x.Value).Returns(customResizeOptioons);

            _service = new ResizeService(_jsruntimeMock.Object, _browserWindowSizeProvider.Object, optionGetter.Object);
            await CheckSubscriptionOptions(customResizeOptioons);
        }

        [Test]
        public async Task Subscribe_WithPerCallOption()
        {
            var customResizeOptioons = new ResizeOptions
            {
                ReportRate = 130,
            };

            SetupJsMockForSubscription(customResizeOptioons);

            var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { }, customResizeOptioons);

            subscriptionId.Should().NotBe(Guid.Empty);
            _jsruntimeMock.Verify();
        }

        [Test]
        public async Task Subscribe_WithPerCallOptionSetAsNull()
        {
            var customResizeOptioons = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptioons);
            var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { }, null);

            subscriptionId.Should().NotBe(Guid.Empty);
            _jsruntimeMock.Verify();
        }

        [Test]
        public void Subscribe_Failed_NullCallback()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SubscribeAsync(null!));
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SubscribeAsync(null!, new ResizeOptions()));

        }

        [Test]
        public async Task SubscribeAndUnsubcribe_SingleSubscription()
        {
            var customResizeOptioons = new ResizeOptions();

            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
            {
                if (x.ListenerId == default)
                {
                    throw new ArgumentException();
                }

                SetupJsMockForUnsubscription(x.ListenerId);

            };

            SetupJsMockForSubscription(customResizeOptioons, feedbackCaller);
            var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { }, null);

            var result = await _service.UnsubscribeAsync(subscriptionId);

            result.Should().BeTrue();

            _jsruntimeMock.Verify();

        }

        [Test]
        public async Task SubscribeAndUnsubcribe_DisposeOfDotNet()
        {
            var customResizeOptioons = new ResizeOptions();

            for (int i = 0; i < 4; i++)
            {
                var lastDotnetRefHashCode = 0;

                Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
                {
                    if (x.ListenerId == default)
                    {
                        throw new ArgumentException();
                    }

                    if (x.DotnetRef.GetHashCode() == lastDotnetRefHashCode)
                    {
                        throw new ArgumentException();
                    }

                    lastDotnetRefHashCode = x.DotnetRef.GetHashCode();

                    SetupJsMockForUnsubscription(x.ListenerId);

                };

                SetupJsMockForSubscription(customResizeOptioons, feedbackCaller);
                var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { }, null);

                var result = await _service.UnsubscribeAsync(subscriptionId);

                result.Should().BeTrue();

                _jsruntimeMock.Verify();
            }
        }

        [Test]
        public async Task SubscribeAndUnsubcribe_MultipleSubscription()
        {
            var customResizeOptioons = new ResizeOptions();

            var feedbackCallerCount = 0;

            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
            {
                if (x.ListenerId == default)
                {
                    throw new ArgumentException();
                }

                feedbackCallerCount++;
            };

            SetupJsMockForSubscription(customResizeOptioons, feedbackCaller);

            HashSet<Guid> subscriptionIds = new HashSet<Guid>();

            for (int i = 0; i < 10; i++)
            {
                var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { });
                subscriptionIds.Add(subscriptionId);
            }

            feedbackCallerCount.Should().Be(1);
            subscriptionIds.Should().HaveCount(10);

            _jsruntimeMock.Verify();

        }

        [Test]
        public async Task SubscribeAndUnsubcribe_MultipleSubscription_WithMultipleOptions()
        {
            List<ListenForResizeCallbackInfo> callerFeedbacks = new();

            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
            {
                if (x.ListenerId == default)
                {
                    throw new ArgumentException();
                }

                callerFeedbacks.Add(x);
            };

            var options = new[]
            {
                new ResizeOptions(),
                new ResizeOptions { EnableLogging = !(new ResizeOptions().EnableLogging) },
                new ResizeOptions { ReportRate = 50 },
                new ResizeOptions { SuppressInitEvent = !(new ResizeOptions().SuppressInitEvent) },
            };

            foreach (var item in options)
            {
                SetupJsMockForSubscription(item, feedbackCaller);
            }

            var subscriptionIds = options.ToDictionary(x => x, x => new HashSet<Guid>());

            for (int i = 0; i < 40; i++)
            {
                var index = i % 4;
                var option = options[index];

                var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { }, option);
                subscriptionIds[option].Add(subscriptionId);
            }

            foreach (var item in subscriptionIds)
            {
                item.Value.Should().HaveCount(10);
            }

            callerFeedbacks.Should().HaveCount(4);
            for (int i = 0; i < callerFeedbacks.Count; i++)
            {
                var feedback = callerFeedbacks[i];
                feedback.options.Should().Be(options[i]);
            }

            callerFeedbacks.Select(x => x.ListenerId).ToHashSet().Should().HaveCount(4);

            _jsruntimeMock.Verify();

        }

        [Test]
        public async Task Unsubscribe_Failed_NoActiveSubscription()
        {
            var result = await _service.UnsubscribeAsync(Guid.NewGuid());

            result.Should().BeFalse();

        }

        [Test]
        public async Task Unsubscribe_Failed_SubscriptioonIdNotFound()
        {
            var customResizeOptioons = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptioons);
            var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { }, null);

            var result = await _service.UnsubscribeAsync(Guid.NewGuid());

            result.Should().BeFalse();
        }


        [Test]
        public async Task DisposeAsync_Failed_NoActiveSubscription()
        {
            await _service.DisposeAsync();

        }

        [Test]
        public async Task DisposeAsync_MultipleSubscription_WithMultipleOptions()
        {
            List<Guid> callerIds = new();

            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
            {
                if (x.ListenerId == default)
                {
                    throw new ArgumentException();
                }
                callerIds.Add(x.ListenerId);
            };

            var options = new[]
            {
                new ResizeOptions(),
                new ResizeOptions { EnableLogging = !(new ResizeOptions().EnableLogging) },
                new ResizeOptions { ReportRate = 50 },
                new ResizeOptions { SuppressInitEvent = !(new ResizeOptions().SuppressInitEvent) },
            };

            foreach (var item in options)
            {
                SetupJsMockForSubscription(item, feedbackCaller);
            }

            for (int i = 0; i < 40; i++)
            {
                var index = i % 4;
                var option = options[index];

                await _service.SubscribeAsync((BrowserWindowSize size) => { }, option);
            }

            Func<IEnumerable<Guid>, bool> idChecker = (ids) =>
             {
                 try
                 {
                     ids.Should().BeEquivalentTo(callerIds);
                     return true;
                 }
                 catch (Exception)
                 {
                     return false;
                 }
             };

            _jsruntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListeners",
             It.Is<object[]>(z =>
                 z[0] is IEnumerable<Guid> &&
                 idChecker((IEnumerable<Guid>)z[0]) == true
             ))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            await _service.DisposeAsync();

            _jsruntimeMock.Verify();

        }

        private class FakeSubscriber
        {
            public Guid SubscriptionId { get; set; }
            public BrowserWindowSize ActualSize { get; set; } = new BrowserWindowSize { };

            public async Task Subscribe(ResizeService service,
                ResizeOptions options)
            {
                SubscriptionId = await service.SubscribeAsync((x) => ActualSize = x, options);
            }
        }


        [Test]
        public async Task RaiseOnResized_MultipleSubscription_WithMultipleOptions()
        {
            Dictionary<ResizeOptions, Guid> listenerIds = new();
            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) => listenerIds.Add(x.options, x.ListenerId); ;

            var options = new[]
            {
                (new ResizeOptions(),new BrowserWindowSize { Height = 20,  Width = 200 }),
                (new ResizeOptions { EnableLogging = !(new ResizeOptions().EnableLogging) }, new BrowserWindowSize { Height = 30,  Width = 300 }),
                (new ResizeOptions { ReportRate = 50 },new BrowserWindowSize { Height = 40,  Width = 400 }),
                (new ResizeOptions { SuppressInitEvent = !(new ResizeOptions().SuppressInitEvent) },new BrowserWindowSize { Height = 50,  Width = 500 })
            };

            Dictionary<FakeSubscriber, BrowserWindowSize> subscribers = new();

            foreach (var item in options)
            {
                SetupJsMockForSubscription(item.Item1, feedbackCaller);
            }

            for (int i = 0; i < 40; i++)
            {
                var index = i % 4;
                var option = options[index];

                var subscriber = new FakeSubscriber();
                await subscriber.Subscribe(_service, option.Item1);

                subscribers.Add(subscriber, option.Item2);
            }

            foreach (var item in options)
            {
                var listenerId = listenerIds[item.Item1];
                _service.RaiseOnResized(item.Item2, Breakpoint.None, listenerId);
            }

            foreach (var item in subscribers)
            {
                item.Key.ActualSize.Should().Be(item.Value);
            }

            _jsruntimeMock.Verify();
        }

        [Test]
        public async Task RaiseOnResized_InvalidListenerId()
        {
            var customResizeOptions = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptions);

            FakeSubscriber subscriber = new FakeSubscriber();
            await subscriber.Subscribe(_service, customResizeOptions);

            _service.RaiseOnResized(new BrowserWindowSize { Height = 2000 }, Breakpoint.None, Guid.NewGuid());

            subscriber.ActualSize.Height.Should().NotBe(2000);
        }

        [Test]
        public async Task Unsubcribe_MultipleSubscription_InParallel()
        {
            var customResizeOptioons = new ResizeOptions();

            var feedbackCallerCount = 0;
            var listenerId = Guid.Empty;

            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
            {
                if (x.ListenerId == default)
                {
                    throw new ArgumentException();
                }
                feedbackCallerCount++;
                listenerId = x.ListenerId;
            };

            SetupJsMockForSubscription(customResizeOptioons, feedbackCaller);

            var subscriptionIds = new HashSet<Guid>();

            for (int i = 0; i < 40; i++)
            {
                var subscriptionId = await _service.SubscribeAsync((BrowserWindowSize size) => { });
                subscriptionIds.Add(subscriptionId);
            }

            subscriptionIds.Should().HaveCount(40);
            feedbackCallerCount.Should().Be(1);

            SetupJsMockForUnsubscription(listenerId);

            var tasksToExecute = new Task[subscriptionIds.Count];
            for (int i = 0; i < subscriptionIds.Count; i++)
            {
                var temp = i;
                var task = Task.Run(async () =>
                {
                    _ = await _service.UnsubscribeAsync(subscriptionIds.ElementAt(temp));
                });

                tasksToExecute[i] = task;
            }

            Task.WaitAll(tasksToExecute);

            _jsruntimeMock.Verify((x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListener",
               It.Is<object[]>(z =>
                   (Guid)z[0] == listenerId
               ))), Times.Once());
        }
    }
}
