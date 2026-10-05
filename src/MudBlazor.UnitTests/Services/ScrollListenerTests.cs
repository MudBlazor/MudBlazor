using AwesomeAssertions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class ScrollListenerTests
{
    private Mock<IJSRuntime> _runtimeMock;
    private ScrollListener _service;

    [SetUp]
    public void SetUp()
    {
        _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _service = new ScrollListener(_runtimeMock.Object);
    }

    [Test]
    public async Task CallsJsCorrectly()
    {
        _runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.listenForScroll", It.IsAny<object[]>()))
            .ReturnsAsync((IJSVoidResult)null);
        _runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.cancelListener", It.IsAny<object[]>()))
            .ReturnsAsync((IJSVoidResult)null);
        _runtimeMock.Setup(x => x.InvokeAsync<ScrollEventArgs>("mudScrollListener.getCurrentScrollPosition", It.IsAny<object[]>()))
            .ReturnsAsync(new ScrollEventArgs());

        _service.OnScroll += OnOnScroll;
        await Task.Delay(100);
        _runtimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.listenForScroll", It.IsAny<object[]>()), Times.Exactly(1));

        _service.OnScroll -= OnOnScroll;
        await Task.Delay(100);
        _runtimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.cancelListener", It.IsAny<object[]>()), Times.Exactly(1));

        await _service.GetCurrentScrollDataAsync();
        _runtimeMock.Verify(x => x.InvokeAsync<ScrollEventArgs>("mudScrollListener.getCurrentScrollPosition", It.IsAny<object[]>()), Times.Exactly(1));

        _service.OnScroll += OnOnScroll;
        await Task.Delay(100);
        await _service.DisposeAsync();
        _runtimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.cancelListener", It.IsAny<object[]>()), Times.Exactly(2));
        return;

        void OnOnScroll(object sender, ScrollEventArgs e) { }
    }

    [Test]
    public void OnScroll_SecondSubscriber_DoesNotRestartJsListener()
    {
        // Only the first subscriber starts the JS listener; the second reuses it.
        _runtimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.listenForScroll", It.IsAny<object[]>()))
            .ReturnsAsync((IJSVoidResult)null);

        _service.OnScroll += (_, _) => { };
        _service.OnScroll += (_, _) => { };

        _runtimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudScrollListener.listenForScroll", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public async Task GetCurrentScrollDataAsync_PassesSelectorAndReturnsJsResult()
    {
        _service.Selector = "#content";
        var expected = new ScrollEventArgs { ScrollTop = 13, NodeName = "DIV" };
        // Strict mock only matches when the configured selector is forwarded as the sole argument.
        _runtimeMock.Setup(x => x.InvokeAsync<ScrollEventArgs>(
                "mudScrollListener.getCurrentScrollPosition",
                It.Is<object[]>(a => a.Length == 1 && (string)a[0] == "#content")))
            .ReturnsAsync(expected);

        var actual = await _service.GetCurrentScrollDataAsync();

        actual.Should().BeSameAs(expected);
    }
}
