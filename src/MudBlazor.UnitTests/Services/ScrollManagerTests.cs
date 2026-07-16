using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Interop;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class ScrollManagerTests
{
    private Mock<IJSRuntime> _runtimeMock;
    private Mock<ILogger<ScrollManager>> _loggerMock;
    private ScrollManager _service;

    [SetUp]
    public void SetUp()
    {
        _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _loggerMock = new Mock<ILogger<ScrollManager>>();
        _service = new ScrollManager(_runtimeMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task ScrollToAsync_CallsJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.scrollTo", args =>
            args.Length == 4 &&
            (args[0] as string) == "#list" &&
            (int)args[1] == 10 &&
            (int)args[2] == 20 &&
            (args[3] as string) == "smooth");

        await _service.ScrollToAsync("#list", 10, 20, ScrollBehavior.Smooth);

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollIntoViewAsync_CallsJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.scrollIntoView", args =>
            args.Length == 2 &&
            (args[0] as string) == ".target" &&
            (args[1] as string) == "auto");

        await _service.ScrollIntoViewAsync(".target", ScrollBehavior.Auto);

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToTopAsync_CallsScrollToWithZeroOffsets()
    {
        SetupVoidInvocation("mudScrollManager.scrollTo", args =>
            args.Length == 4 &&
            args[0] == null &&
            (int)args[1] == 0 &&
            (int)args[2] == 0 &&
            (args[3] as string) == "auto");

        await _service.ScrollToTopAsync(null);

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToBottomAsync_CallsJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.scrollToBottom", args =>
            args.Length == 2 &&
            (args[0] as string) == "messages" &&
            (args[1] as string) == "smooth");

        await _service.ScrollToBottomAsync("messages", ScrollBehavior.Smooth);

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToYearAsync_CallsJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.scrollToYear", args =>
            args.Length == 1 &&
            (args[0] as string) == "year-2026");

        await _service.ScrollToYearAsync("year-2026");

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToListItemAsync_CallsJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.scrollToListItem", args =>
            args.Length == 1 &&
            (args[0] as string) == "item-42");

        await _service.ScrollToListItemAsync("item-42");

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task LockAndUnlockScroll_CallJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.lockScroll", args =>
            args.Length == 2 &&
            (args[0] as string) == "#dialog" &&
            (args[1] as string) == "locked");
        SetupVoidInvocation("mudScrollManager.unlockScroll", args =>
            args.Length == 2 &&
            (args[0] as string) == "#dialog" &&
            (args[1] as string) == "locked");

        await _service.LockScrollAsync("#dialog", "locked");
        await _service.UnlockScrollAsync("#dialog", "locked");

        _runtimeMock.VerifyAll();
    }

    private static readonly Exception[] BenignInteropExceptions =
    [
        new JSDisconnectedException("disconnected"),
        new TaskCanceledException(),
        new ObjectDisposedException("circuit"),
    ];

    [TestCaseSource(nameof(BenignInteropExceptions))]
    public async Task LockScrollAsync_SwallowsBenignInteropError(Exception exception)
    {
        // MudOverlay awaits this from its lifecycle methods, so a disconnect or teardown mid-render must not surface as an unhandled exception.
        SetupThrowingInvocation("mudScrollManager.lockScroll", exception);

        var act = async () => await _service.LockScrollAsync("#dialog", "locked");

        await act.Should().NotThrowAsync();
    }

    [Test]
    [NonParallelizable] // ScriptDiagnostics.MissingScriptLogged is process-wide; keep other fixtures from racing the reset.
    public async Task LockScrollAsync_MissingScript_LogsGuidanceOnceAndDoesNotThrow()
    {
        // https://github.com/MudBlazor/MudBlazor/issues/13477
        // Every dialog and overlay locks scroll from a lifecycle method; when the MudBlazor script
        // isn't referenced this must log actionable guidance once instead of killing the circuit.
        SetupThrowingInvocation("mudScrollManager.lockScroll", new JSException("Could not find 'mudScrollManager.lockScroll' ('mudScrollManager' was undefined)."));
        ScriptDiagnostics.MissingScriptLogged = false;

        var act = async () =>
        {
            await _service.LockScrollAsync("#dialog", "locked");
            await _service.LockScrollAsync("#dialog", "locked");
        };

        await act.Should().NotThrowAsync();
        _loggerMock.VerifyLogging(ScriptDiagnostics.MissingScriptMessage, LogLevel.Error, Times.Once());
    }

    [Test]
    public async Task UnlockScrollAsync_SwallowsJsRuntimeError()
    {
        // Unlock runs during teardown when the circuit may already be gone, so it must not throw.
        SetupThrowingInvocation("mudScrollManager.unlockScroll", new JSDisconnectedException("disconnected"));

        var act = async () => await _service.UnlockScrollAsync("#dialog", "locked");

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task ScrollToVirtualizedItemAsync_CallsJsWithExpectedArguments()
    {
        SetupVoidInvocation("mudScrollManager.scrollToVirtualizedItem", args =>
            args.Length == 5 &&
            (args[0] as string) == "table" &&
            (int)args[1] == 7 &&
            (double)args[2] == 42.5 &&
            (args[3] as string) == "row-7" &&
            (args[4] as string) == "smooth");

        await _service.ScrollToVirtualizedItemAsync("table", 7, 42.5, "row-7", ScrollBehavior.Smooth);

        _runtimeMock.VerifyAll();
    }

    private void SetupVoidInvocation(string identifier, Func<object[], bool> argumentMatcher)
    {
        _runtimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>(identifier, It.Is<object[]>(args => argumentMatcher(args))))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();
    }

    private void SetupThrowingInvocation(string identifier, Exception exception)
    {
        _runtimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>(identifier, It.IsAny<object[]>()))
            .Throws(exception);
    }
}
