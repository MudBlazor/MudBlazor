using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Services;
public class ScrollListenerTests : IAsyncDisposable
{
    private Mock<IJSRuntime> _runtimeMock;
    private ScrollListener _service;

    [Before(HookType.Test)]
    public void SetUp()
    {
        _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _service = new ScrollListener(_runtimeMock.Object);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_service != null)
        {
            await _service.DisposeAsync();
        }
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
}
