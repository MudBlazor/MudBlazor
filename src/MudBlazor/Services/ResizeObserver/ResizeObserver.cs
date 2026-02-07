using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services;

/// <summary>
/// Watches elements for size changes and keeps a local cache of their bounds for fast lookup.
/// </summary>
/// <remarks>
/// This service bridges Blazor components to the browser ResizeObserver API through JS interop.
/// Components use it to subscribe once and then query cached sizes during rendering without extra JS roundtrips.
/// </remarks>
internal sealed class ResizeObserver : IResizeObserver
{
    private bool _disposed;
    private readonly IJSRuntime _jsRuntime;
    private readonly Guid _id = Guid.NewGuid();
    private readonly ResizeObserverOptions _options;
    private readonly DotNetObjectReference<ResizeObserver> _dotNetRef;
    private readonly Dictionary<Guid, ElementReference> _cachedValueIds = new();
    private readonly Dictionary<ElementReference, BoundingClientRect> _cachedValues = new(ElementReferenceComparer.Default);

    /// <inheritdoc />
    public event SizeChanged? OnResized;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResizeObserver"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    /// <param name="options">The options to configure the resize observer.</param>
    [DynamicDependency(nameof(OnSizeChanged))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SizeChangeUpdateInfo))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(BoundingClientRect))]
    public ResizeObserver(IJSRuntime jsRuntime, IOptions<ResizeObserverOptions>? options = null)
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _jsRuntime = jsRuntime;
        _options = options?.Value ?? new ResizeObserverOptions();
    }

    /// <inheritdoc />
    public async Task<BoundingClientRect?> Observe(ElementReference element) => (await Observe(new[] { element })).FirstOrDefault();

    /// <inheritdoc />
    public async Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements)
    {
        var filteredElements = elements.Where(x => x.Context is not null && !_cachedValues.ContainsKey(x)).ToList();
        if (filteredElements.Count == 0)
        {
            return Array.Empty<BoundingClientRect>();
        }

        var elementIds = new List<Guid>();

        foreach (var item in filteredElements)
        {
            var id = Guid.NewGuid();
            elementIds.Add(id);
            _cachedValueIds.Add(id, item);
        }

        var boundingRect = await _jsRuntime.InvokeAsyncWithErrorHandling<BoundingClientRect[]?>([], "mudResizeObserver.connect", _id, _dotNetRef, filteredElements, elementIds, _options);
        var result = boundingRect.value ?? [];
        var counter = 0;
        foreach (var item in result)
        {
            _cachedValues.Add(filteredElements.ElementAt(counter), item);
            counter++;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task Unobserve(ElementReference element)
    {
        var elementId = _cachedValueIds.FirstOrDefault(x => x.Value.Id == element.Id).Key;
        if (elementId == Guid.Empty)
        {
            return;
        }

        await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeObserver.disconnect", _id, elementId);

        _cachedValueIds.Remove(elementId);
        _cachedValues.Remove(element);
    }

    /// <inheritdoc />
    public bool IsElementObserved(ElementReference reference) => _cachedValues.ContainsKey(reference);

    /// <inheritdoc />
    public BoundingClientRect? GetSizeInfo(ElementReference reference) => _cachedValues.GetValueOrDefault(reference);

    /// <inheritdoc />
    public double GetHeight(ElementReference reference) => GetSizeInfo(reference)?.Height ?? 0.0;

    /// <inheritdoc />
    public double GetWidth(ElementReference reference) => GetSizeInfo(reference)?.Width ?? 0.0;

    /// <summary>
    /// Invoked by JavaScript when the size of an observed element changes.
    /// </summary>
    /// <param name="changes">The changes in size.</param>
    [JSInvokable]
    public void OnSizeChanged(IEnumerable<SizeChangeUpdateInfo> changes)
    {
        var parsedChanges = new Dictionary<ElementReference, BoundingClientRect>(ElementReferenceComparer.Default);
        foreach (var item in changes)
        {
            if (_cachedValueIds.TryGetValue(item.Id, out var elementRef))
            {
                _cachedValues[elementRef] = item.Size;
                parsedChanges.Add(elementRef, item.Size);
            }
        }

        OnResized?.Invoke(parsedChanges);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeObserver.cancelListener", _id);

            _dotNetRef.Dispose();
            _cachedValueIds.Clear();
            _cachedValues.Clear();
        }
    }

    /// <summary>
    /// Payload sent from JavaScript to describe which element changed and its latest bounds.
    /// </summary>
    /// <remarks>
    /// The interop layer batches multiple element updates so the observer can refresh its cache in one pass.
    /// </remarks>
    /// <param name="Id">The identifier of the element.</param>
    /// <param name="Size">The new size of the element.</param>
    public record SizeChangeUpdateInfo(Guid Id, BoundingClientRect Size);

    /// <summary>
    /// Compares <see cref="ElementReference"/> instances by their element id to speed dictionary lookups.
    /// </summary>
    /// <remarks>
    /// This avoids the slower default implementation when using <see cref="ElementReference"/> as dictionary keys.
    /// </remarks>
    internal class ElementReferenceComparer : IEqualityComparer<ElementReference>
    {
        /// <inheritdoc />
        public bool Equals(ElementReference x, ElementReference y) => x.Id == y.Id;

        /// <inheritdoc />
        public int GetHashCode(ElementReference obj)
        {
            // Do not modify this null workaround, as the Id can be null when ElementReference is initialized as default(ElementReference).
            // Although the nullable annotation suggests otherwise, we use this unconventional object pattern instead of an if-else statement 
            // to suppress the nullable annotation.
            // https://github.com/dotnet/aspnetcore/issues/58523
            return obj is { Id: null } ? 0 : obj.Id.GetHashCode();
        }

        /// <summary>
        /// Gets the default instance of the comparer.
        /// </summary>
        public static ElementReferenceComparer Default { get; } = new();
    }
}
