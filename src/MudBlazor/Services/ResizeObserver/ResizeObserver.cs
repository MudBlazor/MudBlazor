using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public class ResizeObserver : IResizeObserver, IAsyncDisposable
    {
        private bool _isDisposed = false;

        private readonly IJSRuntime _jsRuntime;
        private readonly Guid _id = Guid.NewGuid();
        private readonly ResizeObserverOptions _options;
        private readonly DotNetObjectReference<ResizeObserver> _dotNetRef;
        private readonly Dictionary<Guid, ElementReference> _cachedValueIds = new();
        private readonly Dictionary<ElementReference, BoundingClientRect> _cachedValues = new();


        [DynamicDependency(nameof(OnSizeChanged))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SizeChangeUpdateInfo))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(BoundingClientRect))]
        public ResizeObserver(IJSRuntime jsRuntime, IOptions<ResizeObserverOptions> options = null)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
            _options = options?.Value ?? new ResizeObserverOptions();
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
                var id = Guid.NewGuid();
                elementIds.Add(id);
                _cachedValueIds.Add(id, item);
            }

            var result = await _jsRuntime.InvokeAsync<IEnumerable<BoundingClientRect>>("mudResizeObserver.connect", _id, _dotNetRef, filteredElements, elementIds, _options) ?? Array.Empty<BoundingClientRect>();
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
            var elementId = _cachedValueIds.FirstOrDefault(x => x.Value.Id == element.Id).Key;
            if (elementId == default) { return; }

            //if the unobserve happens during a component teardown, the try-catch is a safe guard to prevent a "pseudo" exception
            try { await _jsRuntime.InvokeVoidAsync($"mudResizeObserver.disconnect", _id, elementId); } catch (Exception) { }

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
                if (_cachedValueIds.TryGetValue(item.Id, out var elementRef))
                {
                    _cachedValues[elementRef] = item.Size;
                    parsedChanges.Add(elementRef, item.Size);
                }
            }

            OnResized?.Invoke(parsedChanges);
        }

        public event SizeChanged OnResized;

        public BoundingClientRect GetSizeInfo(ElementReference reference)
        {
            return _cachedValues.TryGetValue(reference, out var existing) ? existing : null;
        }

        public double GetHeight(ElementReference reference) => GetSizeInfo(reference)?.Height ?? 0.0;
        public double GetWidth(ElementReference reference) => GetSizeInfo(reference)?.Width ?? 0.0;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _isDisposed == false)
            {
                _isDisposed = true;
                _dotNetRef.Dispose();
                _cachedValueIds.Clear();
                _cachedValues.Clear();

                //in a fire and forget manner, we just "trying" to cancel the listener. So, we are not interested in an potential error 
                try { _ = _jsRuntime.InvokeVoidAsync($"mudResizeObserver.cancelListener", _id); } catch (Exception) { }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) { return; }

            _isDisposed = true;

            _dotNetRef.Dispose();
            _cachedValueIds.Clear();
            _cachedValues.Clear();

            //in a fire and forget manner, we just "trying" to cancel the listener. So, we are not interested in an potential error 
            try { await _jsRuntime.InvokeVoidAsync($"mudResizeObserver.cancelListener", _id); } catch (Exception) { }
        }
    }
}
