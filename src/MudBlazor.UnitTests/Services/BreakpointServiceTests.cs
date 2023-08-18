// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BreakpointServiceTests
    {
        private Mock<IBrowserWindowSizeProvider> _browserWindowSizeProvider;
        private Mock<IJSRuntime> _jsRuntimeMock;
        private BreakpointService _service;

        [SetUp]
        public void SetUp()
        {
            _browserWindowSizeProvider = new Mock<IBrowserWindowSizeProvider>();
            _browserWindowSizeProvider.Setup(x => x.GetBrowserWindowSize()).ReturnsAsync(new BrowserWindowSize { Width = 970, Height = 30 }).Verifiable();
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _service = new BreakpointService(_jsRuntimeMock.Object, _browserWindowSizeProvider.Object);
        }


        private record ListenForResizeCallbackInfo(DotNetObjectReference<BreakpointService> DotnetRef, ResizeOptions Options, Guid ListenerId);

        private void SetupJsMockForSubscription(ResizeOptions expectedOptions, bool setBreakpoints, Action<ListenForResizeCallbackInfo> callbackInfo = null)
        {
            if (setBreakpoints)
            {
                expectedOptions.BreakpointDefinitions = BreakpointGlobalOptions.DefaultBreakpointDefinitions.ToDictionary(x => x.Key, x => x.Value);
            }

            _jsRuntimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize",
                    It.Is<object[]>(args =>
                        args[0] is DotNetObjectReference<BreakpointService> == true &&
                        (ResizeOptions)args[1] == expectedOptions &&
                        (Guid)args[2] != default
                    )))
                .ReturnsAsync(Mock.Of<IJSVoidResult>)
                .Callback<string, object[]>((_, args) =>
                {
                    var dotnetReference = (DotNetObjectReference<BreakpointService>)args[0];
                    var resizeOptions = (ResizeOptions)args[1];
                    var listenerId = (Guid)args[2];
                    callbackInfo?.Invoke(new ListenForResizeCallbackInfo(dotnetReference, resizeOptions, listenerId));
                })
                .Verifiable();
        }

        private void SetupJsMockForUnsubscription(Guid listenerId)
        {
            _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListener",
               It.Is<object[]>(z =>
                   (Guid)z[0] == listenerId
               ))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();
        }

        private async Task CheckSubscriptionOptions(ResizeOptions expectedOptions, bool setBreakpoint)
        {
            SetupJsMockForSubscription(expectedOptions, setBreakpoint);

            var subscriptionResult = await _service.SubscribeAsync((Breakpoint size) => { }, expectedOptions);

            subscriptionResult.Should().NotBeNull();
            subscriptionResult.SubscriptionId.Should().NotBe(Guid.Empty);
            subscriptionResult.Breakpoint.Should().Be(Breakpoint.Md);

            _jsRuntimeMock.Verify();
        }

        [Test]
        public async Task Subscribe_WithDefaultOptions()
        {
            await CheckSubscriptionOptions(new ResizeOptions(), true);
        }


        [Test]
        public async Task Subscribe_WithDefaultOptions_SetBreakpoint()
        {
            var option = new ResizeOptions();
            await CheckSubscriptionOptions(option, true);

            option.BreakpointDefinitions.Should().NotBeNull();

            option.BreakpointDefinitions.Should().NotBeNull();
            option.BreakpointDefinitions.Should().BeEquivalentTo(BreakpointGlobalOptions.DefaultBreakpointDefinitions);
        }

        [Test]
        public async Task Subscribe_WithDefaultOptions_DontSetBreakpoint()
        {
            var breakpointDict = new Dictionary<Breakpoint, int>
            {
                { Breakpoint.Xl, 12 },
            };

            var option = new ResizeOptions
            {
                BreakpointDefinitions = breakpointDict
            };

            await CheckSubscriptionOptions(option, false);

            option.BreakpointDefinitions.Should().BeEquivalentTo(breakpointDict);
        }

        [Test]
        public async Task Subscribe_WithOptionsSetInConstructor()
        {
            var customResizeOptions = new ResizeOptions
            {
                ReportRate = 120,
            };

            var optionGetter = new Mock<IOptions<ResizeOptions>>();
            optionGetter.SetupGet(x => x.Value).Returns(customResizeOptions);

            _service = new BreakpointService(_jsRuntimeMock.Object, _browserWindowSizeProvider.Object, optionGetter.Object);
            await CheckSubscriptionOptions(customResizeOptions, true);
        }

        [Test]
        public async Task Subscribe_WithPerCallOption()
        {
            var customResizeOptions = new ResizeOptions
            {
                ReportRate = 130,
            };

            SetupJsMockForSubscription(customResizeOptions, true);

            var subscriptionResult = await _service.SubscribeAsync((Breakpoint size) => { }, customResizeOptions);

            subscriptionResult.Should().NotBeNull();
            subscriptionResult.SubscriptionId.Should().NotBe(Guid.Empty);
            subscriptionResult.Breakpoint.Should().Be(Breakpoint.Md);

            _jsRuntimeMock.Verify();
        }

        [Test]
        public async Task Subscribe_WithPerCallOptionSetAsNull()
        {
            var customResizeOptions = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptions, true);
            var subscriptionResult = await _service.SubscribeAsync((Breakpoint size) => { }, null);

            subscriptionResult.Should().NotBeNull();
            subscriptionResult.SubscriptionId.Should().NotBe(Guid.Empty);
            subscriptionResult.Breakpoint.Should().Be(Breakpoint.Md);

            _jsRuntimeMock.Verify();
        }

        [Test]
        public void Subscribe_Failed_NullCallback()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SubscribeAsync(null!));
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.SubscribeAsync(null!, new ResizeOptions()));
        }

        [Test]
        public async Task SubscribeAndUnsubscribe_SingleSubscription()
        {
            var customResizeOptions = new ResizeOptions();

            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) =>
            {
                if (x.ListenerId == default)
                {
                    throw new ArgumentException();
                }

                SetupJsMockForUnsubscription(x.ListenerId);

            };

            SetupJsMockForSubscription(customResizeOptions, true, feedbackCaller);
            var subscriptionId = await _service.SubscribeAsync((Breakpoint size) => { }, null);

            var result = await _service.UnsubscribeAsync(subscriptionId.SubscriptionId);

            result.Should().BeTrue();

            _browserWindowSizeProvider.Verify(x => x.GetBrowserWindowSize(), Times.Once());
            _jsRuntimeMock.Verify();

        }

        [Test]
        public async Task SubscribeAndUnsubscribe_DisposeOfDotNet()
        {
            var customResizeOptions = new ResizeOptions();

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

                SetupJsMockForSubscription(customResizeOptions, true, feedbackCaller);
                var subscritionResult = await _service.SubscribeAsync((Breakpoint size) => { }, null);

                var result = await _service.UnsubscribeAsync(subscritionResult.SubscriptionId);

                result.Should().BeTrue();

                _jsRuntimeMock.Verify();
            }
        }

        [Test]
        public async Task SubscribeAndUnsubscribe_MultipleSubscription()
        {
            var customResizeOptions = new ResizeOptions();

            var feedbackCallerCount = 0;

            void FeedbackCaller(ListenForResizeCallbackInfo callbackInfo)
            {
                if (callbackInfo.ListenerId == default)
                {
                    throw new ArgumentException();
                }

                feedbackCallerCount++;
            }

            SetupJsMockForSubscription(customResizeOptions, true, FeedbackCaller);

            var subscriptionIds = new HashSet<Guid>();

            for (int i = 0; i < 10; i++)
            {
                var subscriptionResult = await _service.SubscribeAsync((Breakpoint size) => { });
                subscriptionIds.Add(subscriptionResult.SubscriptionId);
            }

            feedbackCallerCount.Should().Be(1);
            subscriptionIds.Should().HaveCount(10);

            _jsRuntimeMock.Verify();
            _browserWindowSizeProvider.Verify(x => x.GetBrowserWindowSize(), Times.Once());
        }

        [Test]
        public async Task SubscribeAndUnsubscribe_MultipleSubscription_WithMultipleOptions()
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
                SetupJsMockForSubscription(item, true, feedbackCaller);
            }

            var subscriptionIds = options.ToDictionary(x => x, x => new HashSet<Guid>());

            for (int i = 0; i < 40; i++)
            {
                var index = i % 4;
                var option = options[index];

                var subscritionResult = await _service.SubscribeAsync((Breakpoint size) => { }, option);
                subscriptionIds[option].Add(subscritionResult.SubscriptionId);
            }

            foreach (var item in subscriptionIds)
            {
                item.Value.Should().HaveCount(10);
            }

            callerFeedbacks.Should().HaveCount(4);
            for (int i = 0; i < callerFeedbacks.Count; i++)
            {
                var feedback = callerFeedbacks[i];
                feedback.Options.Should().Be(options[i]);
            }

            callerFeedbacks.Select(x => x.ListenerId).ToHashSet().Should().HaveCount(4);

            _jsRuntimeMock.Verify();
            _browserWindowSizeProvider.Verify(x => x.GetBrowserWindowSize(), Times.Once());
        }

        [Test]
        public async Task Unsubscribe_Failed_NoActiveSubscription()
        {
            var result = await _service.UnsubscribeAsync(Guid.NewGuid());

            result.Should().BeFalse();
        }

        [Test]
        public async Task Unsubscribe_Failed_SubscriptionIdNotFound()
        {
            var customResizeOptions = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptions, true);
            var subscriptionId = await _service.SubscribeAsync((Breakpoint size) => { }, null);

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
                SetupJsMockForSubscription(item, true, feedbackCaller);
            }

            for (int i = 0; i < 40; i++)
            {
                var index = i % 4;
                var option = options[index];

                await _service.SubscribeAsync((Breakpoint size) => { }, option);
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

            _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListeners",
             It.Is<object[]>(z =>
                 z[0] is IEnumerable<Guid> &&
                 idChecker((IEnumerable<Guid>)z[0]) == true
             ))).ReturnsAsync(Mock.Of<IJSVoidResult>).Verifiable();

            await _service.DisposeAsync();

            _jsRuntimeMock.Verify();
        }

        private class FakeSubscriber
        {
            public Guid SubscriptionId { get; private set; }
            public Breakpoint ActualSize { get; private set; } = Breakpoint.None;

            public async Task Subscribe(BreakpointService service,
                ResizeOptions options)
            {
                var result = await service.SubscribeAsync((x) => ActualSize = x, options);
                SubscriptionId = result.SubscriptionId;
            }
        }

        [Test]
        public async Task RaiseOnResized_MultipleSubscription_WithMultipleOptions()
        {
            Dictionary<ResizeOptions, Guid> listenerIds = new();
            Action<ListenForResizeCallbackInfo> feedbackCaller = (x) => listenerIds.Add(x.Options, x.ListenerId);

            var options = new[]
            {
                (new ResizeOptions(), Breakpoint.Xl ),
                (new ResizeOptions { EnableLogging = !(new ResizeOptions().EnableLogging) }, Breakpoint.Sm),
                (new ResizeOptions { ReportRate = 50 },Breakpoint.Xs),
                (new ResizeOptions { SuppressInitEvent = !(new ResizeOptions().SuppressInitEvent) }, Breakpoint.Lg)
            };

            Dictionary<FakeSubscriber, Breakpoint> subscribers = new();

            foreach (var item in options)
            {
                SetupJsMockForSubscription(item.Item1, true, feedbackCaller);
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
                _service.RaiseOnResized(new BrowserWindowSize { }, item.Item2, listenerId);
            }

            foreach (var item in subscribers)
            {
                item.Key.ActualSize.Should().Be(item.Value);
            }

            _jsRuntimeMock.Verify();
            _browserWindowSizeProvider.Verify(x => x.GetBrowserWindowSize(), Times.Once());
        }

        [Test]
        public async Task RaiseOnResized_InvalidListenerId()
        {
            var customResizeOptions = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptions, true);

            var subscriber = new FakeSubscriber();
            await subscriber.Subscribe(_service, customResizeOptions);

            _service.RaiseOnResized(new BrowserWindowSize { }, Breakpoint.Xl, Guid.NewGuid());

            subscriber.ActualSize.Should().Be(Breakpoint.None);
        }

        [Test]
        public async Task Subscribe_ChangeHappens_SecondSubscriptionGetNewValue()
        {
            var customResizeOptions = new ResizeOptions();

            SetupJsMockForSubscription(customResizeOptions, true);
            var firstSubscribeResult = await _service.SubscribeAsync((Breakpoint size) => { }, customResizeOptions);

            firstSubscribeResult.Breakpoint.Should().Be(Breakpoint.Md);

            _service.RaiseOnResized(new BrowserWindowSize { }, Breakpoint.Xl, Guid.NewGuid());

            var secondSubscribeResult = await _service.SubscribeAsync((Breakpoint size) => { }, customResizeOptions);
            secondSubscribeResult.Breakpoint.Should().Be(Breakpoint.Xl);

            _browserWindowSizeProvider.Verify(x => x.GetBrowserWindowSize(), Times.Once());
            _jsRuntimeMock.Verify();

        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchMedia(bool interopResult)
        {
            var input = "some input, not relevant for test";

            _jsRuntimeMock.Setup(x => x.InvokeAsync<bool>("mudResizeListener.matchMedia",
            It.Is<object[]>(z =>
                z[0] is string &&
                (string)z[0] == input
            ))).ReturnsAsync(interopResult).Verifiable();

            var result = await _service.MatchMedia(input);

            result.Should().Be(interopResult);
        }

        [Test]
        public void DefaultBreakpointDefinitions()
        {
            BreakpointGlobalOptions.DefaultBreakpointDefinitions.Keys.Should().BeEquivalentTo(new[] {
                Breakpoint.Xxl, Breakpoint.Xl, Breakpoint.Lg, Breakpoint.Md, Breakpoint.Sm, Breakpoint.Xs });

            BreakpointGlobalOptions.DefaultBreakpointDefinitions.Values.Should().BeEquivalentTo(new[] {
                2560, 1920, 1280, 960, 600, 0 });
        }

        // 0 - 599
        [TestCase(Breakpoint.Xs, 0, true)]
        [TestCase(Breakpoint.Xs, 599, true)]
        [TestCase(Breakpoint.Xs, 600, false)]

        // 600 - 959
        [TestCase(Breakpoint.Sm, 599, false)]
        [TestCase(Breakpoint.Sm, 600, true)]
        [TestCase(Breakpoint.Sm, 959, true)]
        [TestCase(Breakpoint.Sm, 960, false)]

        // 960 - 1279
        [TestCase(Breakpoint.Md, 959, false)]
        [TestCase(Breakpoint.Md, 960, true)]
        [TestCase(Breakpoint.Md, 1279, true)]
        [TestCase(Breakpoint.Md, 1280, false)]

        // 1280 - 1919
        [TestCase(Breakpoint.Lg, 1279, false)]
        [TestCase(Breakpoint.Lg, 1280, true)]
        [TestCase(Breakpoint.Lg, 1919, true)]
        [TestCase(Breakpoint.Lg, 1920, false)]

        // 1920 - *
        [TestCase(Breakpoint.Xl, 1919, false)]
        [TestCase(Breakpoint.Xl, 1920, true)]
        [TestCase(Breakpoint.Xl, 9999, true)]

        // >= 600
        [TestCase(Breakpoint.SmAndUp, 599, false)]
        [TestCase(Breakpoint.SmAndUp, 600, true)]
        [TestCase(Breakpoint.SmAndUp, 9999, true)]

        // >= 960
        [TestCase(Breakpoint.MdAndUp, 959, false)]
        [TestCase(Breakpoint.MdAndUp, 960, true)]
        [TestCase(Breakpoint.MdAndUp, 9999, true)]

        // >= 1280
        [TestCase(Breakpoint.LgAndUp, 1279, false)]
        [TestCase(Breakpoint.LgAndUp, 1280, true)]
        [TestCase(Breakpoint.LgAndUp, 9999, true)]

        // < 960
        [TestCase(Breakpoint.SmAndDown, 960, false)]
        [TestCase(Breakpoint.SmAndDown, 959, true)]
        [TestCase(Breakpoint.SmAndDown, 0, true)]

        // < 1280
        [TestCase(Breakpoint.MdAndDown, 1280, false)]
        [TestCase(Breakpoint.MdAndDown, 1279, true)]
        [TestCase(Breakpoint.MdAndDown, 0, true)]

        // < 1920
        [TestCase(Breakpoint.LgAndDown, 1920, false)]
        [TestCase(Breakpoint.LgAndDown, 1919, true)]
        [TestCase(Breakpoint.LgAndDown, 0, true)]
        public async Task IsMediaSizeReturnsCorrectValue(Breakpoint breakpoint, int browserWidth, bool expectedValue)
        {
            // Arrange
            _browserWindowSizeProvider
                .Setup(p => p.GetBrowserWindowSize())
                .ReturnsAsync(new BrowserWindowSize { Width = browserWidth })
                .Verifiable();

            // Act
            var actual = await _service.IsMediaSize(breakpoint);

            // Assert
            actual.Should().Be(expectedValue);

            _browserWindowSizeProvider.Verify();
        }

        [Test]
        public async Task IsMediaSize_ReturnFalseForNoneBreakpoint()
        {
            var actual = await _service.IsMediaSize(Breakpoint.None);
            actual.Should().BeFalse();
        }

        [Test]
        public async Task IsMediaSize_ReturnTrueForAlwaysBreakpoint()
        {
            var actual = await _service.IsMediaSize(Breakpoint.Always);
            actual.Should().BeTrue();
        }
    }
}
