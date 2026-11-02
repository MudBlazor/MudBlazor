// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests
{
#nullable enable
    public class IJSRuntimeExtensionsTests
    {
        private static object[] _caughtExceptions =
        {
#if !DEBUG
            new object[] { new JSException("only testing") },
#endif
            new object[] { new TaskCanceledException() },
            new object[] { new JSDisconnectedException("only testing") },
        };

        [TestCaseSource(nameof(_caughtExceptions))]
        public async Task InvokeVoidAsyncIgnoreErrors_Exception_Swallowed<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .Throws(ex)
                .Verifiable();

            var runtime = runtimeMock.Object;

            var act = async () => await runtime.InvokeVoidAsyncIgnoreErrors("myMethod", 42, "blub");

            await act.Should().NotThrowAsync();
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeVoidAsyncIgnoreErrors_ShouldSwallow_WhenUnsupportedJavaScriptRuntime()
        {
            // InvalidOperationException is only swallowed for the unsupported/remote prerender runtimes.
            var jsRuntime1 = new UnsupportedJavaScriptRuntime();
            var jsRuntime2 = new RemoteJSRuntime();
            var cancellationToken = CancellationToken.None;

            var act1 = async () => await jsRuntime1.InvokeVoidAsyncIgnoreErrors("myMethod1", 42, "blub1");
            var act2 = async () => await jsRuntime2.InvokeVoidAsyncIgnoreErrors("myMethod2", 43, "blub2");
            var act3 = async () => await jsRuntime1.InvokeVoidAsyncIgnoreErrors("myMethod3", cancellationToken, 44, "blub3");
            var act4 = async () => await jsRuntime2.InvokeVoidAsyncIgnoreErrors("myMethod4", cancellationToken, 45, "blub4");

            await act1.Should().NotThrowAsync();
            await act2.Should().NotThrowAsync();
            await act3.Should().NotThrowAsync();
            await act4.Should().NotThrowAsync();
        }

        [Test]
        public async Task InvokeVoidAsyncIgnoreErrors_ShouldRethrow_WhenUncaughtException()
        {
            // A plain InvalidOperationException from a supported runtime is not swallowed.
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .Throws(new InvalidOperationException("mhh that is odd"))
                .Verifiable();

            var runtime = runtimeMock.Object;

            var act = async () => await runtime.InvokeVoidAsyncIgnoreErrors("myMethod", 42, "blub");

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("mhh that is odd");
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeVoidAsyncWithErrorHandling_NoException()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>())
                .Verifiable();

            var runtime = runtimeMock.Object;

            await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", 42, "blub");

            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeVoidAsyncWithErrorHandling_WithToken_NoException()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", cancellationToken, It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>())
                .Verifiable();

            var runtime = runtimeMock.Object;

            await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", cancellationToken, 42, "blub");

            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(_caughtExceptions))]
        public async Task InvokeVoidAsyncWithErrorHandling_Exception<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .Throws(ex)
                .Verifiable();

            var runtime = runtimeMock.Object;

            await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", 42, "blub");

            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(_caughtExceptions))]
        public async Task InvokeVoidAsyncWithErrorHandling_WithToken_Exception<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", cancellationToken, It.IsAny<object[]>()))
                .Throws(ex)
                .Verifiable();

            var runtime = runtimeMock.Object;

            await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", cancellationToken, 42, "blub");

            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeVoidAsyncWithErrorHandling_ThrowsForUncaughtExceptions()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<IJSVoidResult>("myMethod", It.IsAny<object[]>()))
                .Throws(new InvalidOperationException("mhh that is odd"))
                .Verifiable();

            var runtime = runtimeMock.Object;

            var exception = async () => { await runtime.InvokeVoidAsyncWithErrorHandling("myMethod", 42, "blub"); };

            await exception.Should().ThrowAsync<InvalidOperationException>().WithMessage("mhh that is odd");
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeVoidAsyncWithErrorHandling_WithToken_ShouldReturnFalse_WhenUnsupportedJavaScriptRuntime()
        {
            // Arrange
            var jsRuntime1 = new UnsupportedJavaScriptRuntime();
            var jsRuntime2 = new RemoteJSRuntime();
            var cancellationToken = CancellationToken.None;

            // Act
            var result1 = await jsRuntime1.InvokeVoidAsyncWithErrorHandling("myMethod1", cancellationToken, 42, "blub1");
            var result2 = await jsRuntime2.InvokeVoidAsyncWithErrorHandling("myMethod2", cancellationToken, 43, "blub2");

            // Assert
            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Test]
        public async Task InvokeAsyncWithErrorHandling_NoException()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                .ReturnsAsync(42.0)
                .Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", 42, "blub");

            success.Should().Be(true);
            value.Should().Be(42.0);
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeAsyncWithErrorHandling_WithToken_NoException()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            runtimeMock
                .Setup(x => x.InvokeAsync<double>("myMethod", cancellationToken, It.IsAny<object[]>()))
                .ReturnsAsync(42.0)
                .Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", cancellationToken, 42, "blub");

            success.Should().Be(true);
            value.Should().Be(42.0);
            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(_caughtExceptions))]
        public async Task InvokeAsyncWithErrorHandling_Exception_WithDefaultValue<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                .Throws(ex)
                .Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", 42, "blub");

            success.Should().Be(false);
            value.Should().Be(0.0);
            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(_caughtExceptions))]
        public async Task InvokeAsyncWithErrorHandling_Exception_WithFallbackValue<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock
                .Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                .Throws(ex)
                .Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling(37.5, "myMethod", 42, "blub");

            success.Should().Be(false);
            value.Should().Be(37.5);
            runtimeMock.Verify();
        }

        [TestCaseSource(nameof(_caughtExceptions))]
        public async Task InvokeAsyncWithErrorHandling_WithToken_Exception_Fallback<T>(T ex) where T : Exception
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            runtimeMock
                .Setup(x => x.InvokeAsync<double>("myMethod", cancellationToken, It.IsAny<object[]>()))
                .Throws(ex)
                .Verifiable();

            var runtime = runtimeMock.Object;

            var (success, value) = await runtime.InvokeAsyncWithErrorHandling(37.5, "myMethod", cancellationToken, 42, "blub");

            success.Should().Be(false);
            value.Should().Be(37.5);
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeAsyncWithErrorHandling_ThrowsForUncaughtExceptions()
        {
            var runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            runtimeMock.Setup(x => x.InvokeAsync<double>("myMethod", It.IsAny<object[]>()))
                 .Throws(new InvalidOperationException("mhh that is odd"))
                 .Verifiable();

            var runtime = runtimeMock.Object;

            var exception = async () => { await runtime.InvokeAsyncWithErrorHandling<double>("myMethod", 42, "blub"); };

            await exception.Should().ThrowAsync<InvalidOperationException>().WithMessage("mhh that is odd");
            runtimeMock.Verify();
        }

        [Test]
        public async Task InvokeAsyncWithErrorHandling_ShouldReturnFallbackValue_WhenUnsupportedJavaScriptRuntime()
        {
            // Arrange
            var jsRuntime1 = new UnsupportedJavaScriptRuntime();
            var jsRuntime2 = new RemoteJSRuntime();

            // Act
            var result1 = await jsRuntime1.InvokeAsyncWithErrorHandling("fallback1", "myMethod1", 42, "blub1");
            var result2 = await jsRuntime2.InvokeAsyncWithErrorHandling("fallback2", "myMethod2", 43, "blub2");

            // Assert
            result1.success.Should().BeFalse();
            result1.value.Should().Be("fallback1");
            result2.success.Should().BeFalse();
            result2.value.Should().Be("fallback2");
        }

        [Test]
        public async Task InvokeAsyncWithErrorHandling_WithToken_ShouldReturnFallbackValue_WhenUnsupportedJavaScriptRuntime()
        {
            // Arrange
            var jsRuntime1 = new UnsupportedJavaScriptRuntime();
            var jsRuntime2 = new RemoteJSRuntime();
            var cancellationToken = CancellationToken.None;

            // Act
            var result1 = await jsRuntime1.InvokeAsyncWithErrorHandling("fallback1", "myMethod1", cancellationToken, 42, "blub1");
            var result2 = await jsRuntime2.InvokeAsyncWithErrorHandling("fallback2", "myMethod2", cancellationToken, 43, "blub2");

            // Assert
            result1.success.Should().BeFalse();
            result1.value.Should().Be("fallback1");
            result2.success.Should().BeFalse();
            result2.value.Should().Be("fallback2");
        }

#if DEBUG
        [Test]
        public async Task InvokeAsyncIgnoreErrors_ShouldThrow_WhenDebugJSException()
        {
            // Arrange
            var jsRuntime1 = new ExceptionJavascriptRuntime();

            // Act
            var act1 = async () => await jsRuntime1.InvokeVoidAsyncIgnoreErrors("myMethod");
            var act2 = async () => await jsRuntime1.InvokeVoidAsyncIgnoreErrors("myMethod", CancellationToken.None);

            // Assert
            await act1.Should().ThrowAsync<JSException>();
            await act2.Should().ThrowAsync<JSException>();
        }
#else
        [Test]
        public async Task InvokeAsyncIgnoreErrors_ShouldSucceed_WhenReleaseJSException()
        {
            // Arrange
            var jsRuntime1 = new ExceptionJavascriptRuntime();

            // Act
            var act1 = async () => await jsRuntime1.InvokeVoidAsyncIgnoreErrors("myMethod");
            var act2 = async () => await jsRuntime1.InvokeVoidAsyncIgnoreErrors("myMethod", CancellationToken.None);

            // Assert
            await act1.Should().NotThrowAsync();
            await act2.Should().NotThrowAsync();
        }
#endif
    }
}
