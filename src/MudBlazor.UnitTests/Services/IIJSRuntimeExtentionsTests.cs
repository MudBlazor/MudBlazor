// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services
{
    public class IIJSRuntimeExtentionsTests
    {
        static object[] CatchedExceptions =
        {
            #if DEBUG
#else
            new object[] { new JSException("only testing") },
#endif
            new object[] { new TaskCanceledException() },
            new object[] { new JSDisconnectedException("only testing") },
        };

        [Test]
        public async Task InvokeVoidAsyncWithErrorHandling_NoException()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>()).Verifiable();

            var runtime = runtimeMock.Object;

            await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", 42, "blub");

            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(CatchedExceptions))]
        public async Task InvokeVoidAsyncWithErrorHandling_Exception<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .Throws(ex).Verifiable();

            var runtime = runtimeMock.Object;

            await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", 42, "blub");

            runtimeMock.Verify();

        }

        [Test]
        public void InvokeVoidAsyncWithErrorHandling_ThrowsForUncatchedExceptions()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .Throws(new InvalidOperationException("mhh that is odd")).Verifiable();

            var runtime = runtimeMock.Object;

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", 42, "blub"));

            ex.Message.Should().Be("mhh that is odd");
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeAsyncWithErrorHandling_NoException()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                .ReturnsAsync(42.0).Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", 42, "blub");

            success.Should().Be(true);
            value.Should().Be(42.0);
            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(CatchedExceptions))]
        public async Task InvokeAsyncWithErrorHandling_Exception_WithDefaultValue<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                .Throws(ex).Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", 42, "blub");

            success.Should().Be(false);
            value.Should().Be(0.0);
            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(CatchedExceptions))]
        public async Task InvokeAsyncWithErrorHandling_Exception_WithFallbackValue<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                .Throws(ex).Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling<double>(37.5, "myMethod", 42, "blub");

            success.Should().Be(false);
            value.Should().Be(37.5);
            runtimeMock.Verify();
        }

        [Test]
        public void InvokeAsyncWithErrorHandling_ThrowsForUncatchedExceptions()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                 .Throws(new InvalidOperationException("mhh that is odd")).Verifiable();

            var runtime = runtimeMock.Object;

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", 42, "blub"));

            ex.Message.Should().Be("mhh that is odd");
            runtimeMock.Verify();
        }

    }
}
