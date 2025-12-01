using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor;
#nullable enable
/// <summary>
/// Listens to scroll events on a specified element.
/// </summary>
internal sealed class ScrollListener : IScrollListener
{
    private bool _disposed;
    private readonly IJSRuntime _js;
    private EventHandler<ScrollEventArgs>? _onScroll;
    private DotNetObjectReference<ScrollListener>? _dotNetRef;
    private ScrollEventArgs? _initialScrollEvent;
    private bool _started;

    /// <inheritdoc />
    public string? Selector { get; set; }

    /// <inheritdoc />
    public int ReportRateMs { get; set; }

    /// <inheritdoc />
    public bool FireOnStart { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollListener"/> class with the specified JavaScript runtime.
    /// </summary>
    /// <param name="js">The JavaScript runtime.</param>
    [DynamicDependency(nameof(RaiseOnScroll))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrollEventArgs))]
    public ScrollListener(IJSRuntime js) : this(string.Empty, js) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollListener"/> class with the specified selector and JavaScript runtime.
    /// </summary>
    /// <param name="selector">The CSS selector for the element to listen for scroll events.</param>
    /// <param name="js">The JavaScript runtime.</param>
    /// <param name="reportRateMs"></param>
    /// <param name="fireOnStart"></param>
    public ScrollListener(string? selector, IJSRuntime js, int reportRateMs = 100, bool fireOnStart = false)
    {
        _js = js;
        Selector = selector;
        ReportRateMs = reportRateMs;
        FireOnStart = fireOnStart;
    }

    /// <inheritdoc />
    public event EventHandler<ScrollEventArgs> OnScroll
    {
        add => Subscribe(value);
        remove => Unsubscribe(value);
    }

    private async void Subscribe(EventHandler<ScrollEventArgs> value)
    {
        var isFirstSubscriber = _onScroll == null;
        _onScroll += value;

        if (isFirstSubscriber)
        {
            await Start();
        }

        if (_initialScrollEvent != null)
        {
            value.Invoke(this, _initialScrollEvent);
        }
    }

    private void Unsubscribe(EventHandler<ScrollEventArgs> value)
    {
        _onScroll -= value;
        if (_onScroll == null)
        {
            Cancel().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Invoked in JavaScript, in scroll-listener.js.
    /// </summary>
    /// <param name="e">The scroll event arguments.</param>
    [JSInvokable]
    public void RaiseOnScroll(ScrollEventArgs e)
    {
        // Store the first event if FireOnStart is enabled and we haven't started yet
        if (FireOnStart && !_started)
        {
            _initialScrollEvent = e;
            _started = true;
        }

        _onScroll?.Invoke(this, e);
    }

    /// <summary>
    /// Subscribes to the scroll event in JavaScript.
    /// </summary>
    private ValueTask<bool> Start()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        return _js.InvokeVoidAsyncWithErrorHandling("mudScrollListener.listenForScroll", _dotNetRef, Selector, ReportRateMs, FireOnStart);
    }

    /// <summary>
    /// Unsubscribes from the scroll event in JavaScript.
    /// </summary>
    private async ValueTask Cancel()
    {
        await _js.InvokeVoidAsyncWithErrorHandling("mudScrollListener.cancelListener", Selector);
        _started = false;
        _initialScrollEvent = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _dotNetRef?.Dispose();

            _onScroll = null;
            _initialScrollEvent = null;
        }
    }
}
