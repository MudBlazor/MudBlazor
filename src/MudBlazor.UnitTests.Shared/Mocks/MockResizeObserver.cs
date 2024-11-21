using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Shared.Mocks;

public class MockResizeObserverFactory : IResizeObserverFactory
{
    private readonly MockResizeObserver? _observer;

    public MockResizeObserverFactory()
    {

    }

    public MockResizeObserverFactory(MockResizeObserver observer)
    {
        _observer = observer;
    }

    public IResizeObserver Create(ResizeObserverOptions options) => _observer ?? new MockResizeObserver();

    public IResizeObserver Create() => Create(new ResizeObserverOptions());
}

public class MockResizeObserver : IResizeObserver
{
    private readonly Dictionary<ElementReference, BoundingClientRect> _cachedValues = new(ResizeObserver.ElementReferenceComparer.Default);

    public bool IsVertical { get; set; } = false;

    public event SizeChanged? OnResized;

    public void UpdateTotalPanelSize(double newSize)
    {
        var entry = _cachedValues.Last();

        if (IsVertical == false)
        {
            entry.Value.Width = newSize;
        }
        else
        {
            entry.Value.Height = newSize;
        }

        OnResized?.Invoke(new Dictionary<ElementReference, BoundingClientRect>(ResizeObserver.ElementReferenceComparer.Default)
        {
            { entry.Key, entry.Value }
        });
    }

    public void UpdatePanelSize(int index, double newSize)
    {
        var entry = _cachedValues.ElementAt(index);

        if (IsVertical == false)
        {
            entry.Value.Width = newSize;
        }
        else
        {
            entry.Value.Height = newSize;
        }

        OnResized?.Invoke(new Dictionary<ElementReference, BoundingClientRect>(ResizeObserver.ElementReferenceComparer.Default)
        {
            { entry.Key, entry.Value }
        });
    }

    public double PanelSize { get; set; } = 250;

    public double PanelTotalSize { get; set; } = 3000;

    public async Task<BoundingClientRect?> Observe(ElementReference element) => (await Observe(new[] { element })).FirstOrDefault();

    private bool _firstBatchProcessed = false;

    public Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements)
    {
        var result = new List<BoundingClientRect>();
        foreach (var item in elements)
        {
            var size = PanelSize;
            // last element is always TabsContentSize
            if (item.Id == elements.Last().Id && _firstBatchProcessed == false)
            {
                size = PanelTotalSize;
            }
            var rect = new BoundingClientRect { Width = size };
            if (IsVertical)
            {
                rect = new BoundingClientRect { Height = size };
            }
            _cachedValues.Add(item, rect);
        }

        _firstBatchProcessed = true;

        return Task.FromResult<IEnumerable<BoundingClientRect>>(result);
    }

    public Task Unobserve(ElementReference element)
    {
        _cachedValues.Remove(element);
        return Task.CompletedTask;
    }

    public BoundingClientRect? GetSizeInfo(ElementReference reference) => _cachedValues.GetValueOrDefault(reference);

    public double GetHeight(ElementReference reference) => GetSizeInfo(reference)?.Height ?? 0.0;

    public double GetWidth(ElementReference reference) => GetSizeInfo(reference)?.Width ?? 0.0;

    public bool IsElementObserved(ElementReference reference) => _cachedValues.ContainsKey(reference);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public void Dispose()
    {
    }
}
