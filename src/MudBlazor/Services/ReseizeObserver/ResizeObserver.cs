using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public class ResizeObserver : IResizeObserver
    {
        private readonly DotNetObjectReference<ResizeObserver> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;

        private readonly Dictionary<Guid, ElementReference> _cachedValueIds = new();
        private readonly Dictionary<ElementReference, BoundingClientRect> _cachedValues = new();

        private Guid _id = Guid.NewGuid();
        private ResizeObserverOptions _options;

        public ResizeObserver(IJSRuntime jsRuntime, ResizeObserverOptions options)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
            _options = options;
        }

        public ResizeObserver(IJSRuntime jsRuntime, IOptions<ResizeObserverOptions> options = null) : this(jsRuntime, options?.Value ?? new ResizeObserverOptions())
        {
        }

        public async Task<BoundingClientRect> Observe(ElementReference element) => (await Observe(new[] { element })).FirstOrDefault();

        public async Task<IEnumerable<BoundingClientRect>> Observe(IEnumerable<ElementReference> elements)
        {
            var filteredElements = elements.Where(x => x.Context != null && _cachedValues.ContainsKey(x) == false).ToList();
            if (filteredElements.Any() == false)
            {
                return Array.Empty<BoundingClientRect>();
            }

            List<Guid> elementIds = new();

            foreach (var item in filteredElements)
            {
                Guid id = Guid.NewGuid();
                elementIds.Add(id);
                _cachedValueIds.Add(id, item);
            }

            var result = await _jsRuntime.InvokeAsync<IEnumerable<BoundingClientRect>>("mudResizeObserver.connect", _id, _dotNetRef, filteredElements, elementIds, _options);
            var counter = 0;
            foreach (var item in result)
            {
                _cachedValues.Add(filteredElements.ElementAt(counter), item);
                counter++;
            }

            return result;
        }

        public async Task Unobserve(ElementReference element)
        {
            Guid elementId = _cachedValueIds.FirstOrDefault(x => x.Value.Id == element.Id).Key;
            if (elementId == default) { return; }

            await _jsRuntime.InvokeVoidAsync($"mudResizeObserver.disconnect", _id, elementId);

            _cachedValueIds.Remove(elementId);
            _cachedValues.Remove(element);
        }

        public bool IsElementObserved(ElementReference reference) => _cachedValues.ContainsKey(reference);

        public record SizeChangeUpdateInfo(Guid Id, BoundingClientRect Size);

        [JSInvokable]
        public void OnSizeChanged(IEnumerable<SizeChangeUpdateInfo> changes)
        {
            Dictionary<ElementReference, BoundingClientRect> parsedChanges = new();
            foreach (var item in changes)
            {
                if (_cachedValueIds.ContainsKey(item.Id) == false) { continue; }

                var elementRef = _cachedValueIds[item.Id];
                _cachedValues[elementRef] = item.Size;
                parsedChanges.Add(elementRef, item.Size);
            }

            OnResized?.Invoke(parsedChanges);
        }

        public event SizeChanged OnResized;

        public BoundingClientRect GetSizeInfo(ElementReference reference)
        {
            if (_cachedValues.ContainsKey(reference) == false)
            {
                return null;
            }

            return _cachedValues[reference];
        }

        public double GetHeight(ElementReference reference) => GetSizeInfo(reference)?.Height ?? 0.0;
        public double GetWidth(ElementReference reference) => GetSizeInfo(reference)?.Width ?? 0.0;

        protected virtual void Dispose(bool disposing)
        {
            _dotNetRef.Dispose();
            _cachedValueIds.Clear();
            _cachedValues.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            _dotNetRef.Dispose();
            _cachedValueIds.Clear();
            _cachedValues.Clear();
            await _jsRuntime.InvokeVoidAsync($"mudResizeObserver.cancelListener", _id);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
